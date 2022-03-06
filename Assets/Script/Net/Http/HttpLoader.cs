using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using XLua;

namespace NetExtension
{
    public class HttpLoader
    {
        public int timeOut = 60;
        public Action<long,long> _onProgress;
        public Action<byte[]> _onDone;

        public HttpLoader() { }
        public HttpLoader(Action<byte[]> onDone, Action<long, long> onProgress)
        {
            _onProgress = onProgress;
            _onDone = onDone;
        }
        public HttpLoader(LuaTable table)
        {
            table.Get("OnProgress", out _onProgress);
            table.Get("OnDone", out _onDone);
        }

        public void Dispose()
        {
            _onProgress = null;
            _onDone = null;
        }

        public void StartDownload(string url, string filePath)
        {
            HttpMgr.Instance.StartRequestTask(IStartDownload(url,filePath));
        }

        private IEnumerator IStartDownload(string url, string savePath)
        {
            long totalLength = 0;
            using (UnityWebRequest head = UnityWebRequest.Head(url))
            {
                head.timeout = 3;
                yield return head.SendWebRequest();
                if (head.isHttpError || head.isNetworkError)
                {
                    GameDebug.LogError(head.error);
                    _onDone?.Invoke(null);
                    yield break;
                }
                totalLength = long.Parse(head.GetResponseHeader("Content-Length"));
            }
            yield return IStartDownload(url, savePath, totalLength);
        }

        private IEnumerator IStartDownload(string url, string savePath,long totalLength)
        {
            GameDebug.LogYellow("download : " + url);
            string dirPath = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                long fileLength = fs.Length;
                //GameDebug.Log(fileLength + " | " + totalLength);

                UnityWebRequest req = UnityWebRequest.Get(url);
                req.timeout = timeOut;
                if (fileLength > 0)
                {
                    req.SetRequestHeader("Range", "bytes=" + fileLength + "-");
                    fs.Seek(fileLength, SeekOrigin.Begin);
                }
                req.SendWebRequest();

                long needLoad = totalLength - fileLength;
                long curLoad = 0;
                //while (!req.isDone )
                while (curLoad < needLoad)
                {
                    yield return null;
                    byte[] buff = req.downloadHandler.data;
                    if (buff != null && buff.Length - curLoad > 0)
                    {
                        //Debug.Log((fileLength+curLoad) + " >> " + (fileLength + buff.Length));
                        long length = buff.Length - curLoad;
                        fs.Write(buff, (int)curLoad, (int)length);
                        curLoad += length;

                        _onProgress?.Invoke(curLoad, needLoad);
                    }
                }
                if (req.isHttpError || req.isNetworkError)
                {
                    GameDebug.LogError(req.error);
                    _onDone?.Invoke(null);
                }
                else
                {
                    _onDone?.Invoke(req.downloadHandler.data);
                }
                req.Dispose();
            }        
        }

        public void StartDownloadAll(List<FileItem> paths, string loadRoot,string saveRoot)
        {
            HttpMgr.Instance.StartRequestTask(IStartDownloadAll(paths, loadRoot, saveRoot));
        }

        private IEnumerator IStartDownloadAll(List<FileItem> paths, string loadRoot, string saveRoot)
        {
            for(int i = 0; i < paths.Count; i++)
            {
#if AssetBundleHash
                string url = loadRoot + paths[i].md5;
                string filePath = saveRoot + paths[i].md5;
#else
                string url = loadRoot + paths[i].path;
                string filePath = saveRoot + paths[i].path;
#endif
                yield return HttpMgr.Instance.StartRequestDirect(IStartDownload(url, filePath, paths[i].length));
            }
        }
    }
}
