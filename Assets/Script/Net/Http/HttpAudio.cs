using GameUtils;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace NetExtension
{
    public class HttpAudio
    {
        private const string _CACHE = "AudioCache";
        private static string cachePath = Application.persistentDataPath + "/" + _CACHE + "/";
        public static int timeOut = 5;

        static HttpAudio()
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

        //MPEG适合长背景音,WAV适合短音效
        public static void AsyncLoad(string url, Action<AudioClip> action)
        {
            Debug.Assert(!string.IsNullOrEmpty(url), "Audio url can't be null");
            string savePath = cachePath + FileUtils.ins.GetMD5FromString(url);
            if (File.Exists(savePath))
            {
                url = "file://" + cachePath + FileUtils.ins.GetMD5FromString(url);
                HttpMgr.Instance.StartRequestTask(IDownload(url, null, action));
            }
            else
            {
                HttpMgr.Instance.StartRequestTask(IDownload(url, savePath, action));
            }
        }

        static IEnumerator IDownload(string url, string savePath, Action<AudioClip> action)
        {
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                uwr.timeout = timeOut;
                yield return uwr.SendWebRequest();
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    action?.Invoke(null);
                }
                else
                {
                    if (!string.IsNullOrEmpty(savePath))
                        File.WriteAllBytes(savePath, uwr.downloadHandler.data);
                    AudioClip clip = ((DownloadHandlerAudioClip)uwr.downloadHandler).audioClip;
                    action?.Invoke(clip);
                }
            }
            HttpMgr.Instance.EndRequest();
        }
    }
}
