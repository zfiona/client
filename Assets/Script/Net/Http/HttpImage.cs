using System;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using GameUtils;

namespace NetExtension
{
    public class HttpImage
    {
        private const string _CACHE = "imageCache";
        private static string cachePath = Application.persistentDataPath + "/" + _CACHE + "/";
        private static Dictionary<string, Sprite> sprDic = new Dictionary<string, Sprite>();
        public static int timeOut = 5;

        static HttpImage()
        {
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
        }

        public static void ClearCache()
        {
            sprDic.Clear();
            if (Directory.Exists(cachePath))
            {
                Directory.Delete(cachePath, true);
            }
            Directory.CreateDirectory(cachePath);
        }

        public static string GetRootPath()
        {
            return cachePath;
        }

        //从网络加载图片
        public static void AsyncLoad(string url, Action<Sprite> action)
        {
            Debug.Assert(!string.IsNullOrEmpty(url), "Image url can't be null");
            if (sprDic.ContainsKey(url))
            {
                action?.Invoke(sprDic[url]);
                return;
            }
            //string savePath = cachePath + FileUtils.ins.GetMD5FromString(url);
            string savePath = cachePath + url.Substring(url.LastIndexOf('/')+1);
            if (File.Exists(savePath))
            {
                url = "file://" + savePath;
                HttpMgr.Instance.StartRequestTask(IDownload(url, null, action));
            }
            else
            {
                HttpMgr.Instance.StartRequestTask(IDownload(url, savePath, action));
            }
        }

        //从本地加载图片
        public static void LocalLoad(string path,Action<Sprite> action)
        {
            Debug.Assert(!string.IsNullOrEmpty(path), "Image path can't be null");
            if (sprDic.ContainsKey(path))
            {
                action?.Invoke(sprDic[path]);
                return;
            }
            string url;
            if (path.Contains(cachePath))
                url = "file://" + path;
            else
                url = "file://" + cachePath + path;
            HttpMgr.Instance.StartRequestTask(IDownload(url, null, action));
        }

        //从网络或本地缓存加载图片
        static IEnumerator IDownload(string url, string savePath, Action<Sprite> action)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                uwr.timeout = timeOut;
                yield return uwr.SendWebRequest();
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    Debug.LogError(uwr.error);
                    action?.Invoke(null);
                }
                else
                {
                    if (!string.IsNullOrEmpty(savePath))
                        File.WriteAllBytes(savePath, uwr.downloadHandler.data);
                    Texture2D tex2d = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), Vector2.one * 0.5f);
                    sprDic[url] = sprite;
                    action?.Invoke(sprite); 
                }
            }
            HttpMgr.Instance.EndRequest();
        }
    }

}

