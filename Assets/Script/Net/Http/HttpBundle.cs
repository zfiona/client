using GameUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace NetExtension
{
    public class HttpBundle
    {
        private const string _CACHE = "bundleCache";
        private static string cachePath = Application.persistentDataPath + "/" + _CACHE + "/";
        public static int timeOut = 10;
        private static AssetBundle mAssetBundle;

        static HttpBundle()
        {
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
        }

        public static void ClearCache()
        {
            if (Directory.Exists(cachePath))
            {
                Directory.Delete(cachePath, true);
            }
            Directory.CreateDirectory(cachePath);
        }

        public static void AsyncLoad(string url, Action<AssetBundle> action, Action<float> progress)
        {
            Debug.Assert(!string.IsNullOrEmpty(url), "Bundle url can't be null");
            string savePath = cachePath + FileUtils.ins.GetMD5FromString(url);
            if (File.Exists(savePath))
            {
                url = "file://" + cachePath + FileUtils.ins.GetMD5FromString(url);
                HttpMgr.Instance.StartRequestDirect(ILoacalload(url, action));
            }
            else
            {
                HttpMgr.Instance.StartRequestDirect(IDownload(url, savePath, action, progress));
            }
        }

        static IEnumerator IDownload(string url, string savePath, Action<AssetBundle> action, Action<float> progress)
        {
            long totalLength = 0;
            using (UnityWebRequest head = UnityWebRequest.Head(url))
            {
                head.timeout = 3;
                yield return head.SendWebRequest();
                
                if (head.isHttpError || head.isNetworkError)
                {
                    action?.Invoke(null);
                    yield break;
                }
                totalLength = long.Parse(head.GetResponseHeader("Content-Length"));
            }
            using (UnityWebRequest uwr = UnityWebRequest.Get(url))
            {
                uwr.timeout = timeOut;
                uwr.SendWebRequest();
                while (!uwr.isDone)
                {
                    progress?.Invoke(1f * uwr.downloadedBytes / totalLength);
                    yield return null;
                }
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    action?.Invoke(null);
                }
                else
                {
                    File.WriteAllBytes(savePath, uwr.downloadHandler.data);
                    if (action != null)
                    {
                        url = "file://" + cachePath + FileUtils.ins.GetMD5FromString(url);
                        yield return ILoacalload(url, action);
                    }
                }
            }
        }

        static IEnumerator ILoacalload(string url, Action<AssetBundle> action)
        {
            using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                yield return uwr.SendWebRequest();
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    action?.Invoke(null);
                }
                else
                {
                    mAssetBundle = ((DownloadHandlerAssetBundle)uwr.downloadHandler).assetBundle;
                    action?.Invoke(mAssetBundle);
                }
            }
            HttpMgr.Instance.EndRequest();
        }
    }
}


