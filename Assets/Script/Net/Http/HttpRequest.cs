using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using XLua;

namespace NetExtension
{
    public class HttpRequest
    {
        public int timeOut = 3;
        public Action<string, string> _onDone;
        static private string _token = null;

        const string TOKEN_NAME = "Authorization";

        public HttpRequest() { }
        public HttpRequest(Action<string, string> action)
        {
            _onDone = action;
        }
        public HttpRequest(LuaTable table)
        {
            table.Get("OnDone", out _onDone);
        }

        public void Dispose()
        {
            _onDone = null;
        }

        public void Get(string url)
        {
            Debug.Assert(!string.IsNullOrEmpty(url), "request url can't be null");
            HttpMgr.Instance.StartRequestTask(IGet(url));
        }

        public void Post(string url, LuaTable paras)
        {
            Debug.Assert(!string.IsNullOrEmpty(url), "request url can't be null");
            WWWForm form = new WWWForm();
            if (paras != null)
            {
                Hashtable hsTable = paras.Cast<Hashtable>();
                foreach (string key in hsTable.Keys)
                    form.AddField(key, hsTable[key].ToString());
            }
            HttpMgr.Instance.StartRequestTask(IPost(url, form));
        }
        public void PostJson(string url, string jsonStr)
        {
            Debug.Assert(!string.IsNullOrEmpty(url), "request url can't be null");
            HttpMgr.Instance.StartRequestTask(IPost(url, jsonStr));
        }
        public void Post(string url, Dictionary<string, string> paras)
        {
            Debug.Assert(!string.IsNullOrEmpty(url), "request url can't be null");
            WWWForm form = new WWWForm();
            if (paras != null)
            {
                foreach (var v in paras)
                    form.AddField(v.Key, v.Value);
            }
            HttpMgr.Instance.StartRequestTask(IPost(url, form));
        }

        public void Put(string url, byte[] body)
        {
            Debug.Assert(!string.IsNullOrEmpty(url), "request url can't be null");
            HttpMgr.Instance.StartRequestTask(IPut(url, body));
        }


        static public void SetToken(string token)
        {
            _token = token;
            //GameDebug.LogYellow("[setToken]：" + token);
        }

        private class BypassCertificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }

        IEnumerator IGet(string url)
        {
            GameDebug.LogYellow(url);
            using (UnityWebRequest uwr = UnityWebRequest.Get(url))
            {
                uwr.timeout = timeOut;
                if (url.Contains("https"))
                    uwr.certificateHandler = new BypassCertificate();
                if (_token != null)
                {
                    //GameDebug.LogYellow(url + " add token:"+_token);
                    uwr.SetRequestHeader(TOKEN_NAME, _token);
                }
                  
                yield return uwr.SendWebRequest();
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    GameDebug.LogError(url + " : " + uwr.error);
                    _onDone?.Invoke(url, null);
                }
                else
                {
                    _onDone?.Invoke(url, uwr.downloadHandler.text);
                }
            }
            HttpMgr.Instance.EndRequest();
        }

        IEnumerator IPost(string url, WWWForm form)
        {
            GameDebug.LogYellow(url);
            using (UnityWebRequest uwr = UnityWebRequest.Post(url, form))
            {
                uwr.timeout = timeOut;
                if (url.Contains("https"))
                    uwr.certificateHandler = new BypassCertificate();
                if (_token != null)
                    uwr.SetRequestHeader(TOKEN_NAME, _token);
                uwr.SetRequestHeader("Content-Type", "application/json");
                yield return uwr.SendWebRequest();
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    GameDebug.LogError(uwr.error);
                    _onDone?.Invoke(url, null);
                }
                else
                {
                    _onDone?.Invoke(url, uwr.downloadHandler.text);
                }
            }
            HttpMgr.Instance.EndRequest();
        }

        IEnumerator IPost(string url, string json)
        {
            if (!url.Contains("heartbeat"))
                GameDebug.LogYellow(url);
            using (UnityWebRequest uwr = new UnityWebRequest(url, "POST"))
            {
                uwr.timeout = timeOut;
                if (url.Contains("https"))
                    uwr.certificateHandler = new BypassCertificate();
                if (_token != null)
                    uwr.SetRequestHeader(TOKEN_NAME, _token);
                if (!string.IsNullOrEmpty(json))
                {
                    byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(json);
                    uwr.uploadHandler = new UploadHandlerRaw(postBytes);
                    uwr.SetRequestHeader("Content-Type", "application/json");
                }

                uwr.downloadHandler = new DownloadHandlerBuffer();

                yield return uwr.SendWebRequest();

                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    GameDebug.LogError(url + " : " + uwr.error);
                    _onDone?.Invoke(url, null);
                }
                else
                {
                    _onDone?.Invoke(url, uwr.downloadHandler.text);
                }

            }
            HttpMgr.Instance.EndRequest();
        }

        IEnumerator IPut(string url, byte[] body)
        {
            GameDebug.LogYellow(url);
            using (UnityWebRequest uwr = UnityWebRequest.Put(url, body))
            {
                uwr.timeout = timeOut;
                if (_token != null)
                    uwr.SetRequestHeader(TOKEN_NAME, _token);
                yield return uwr.SendWebRequest();
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    GameDebug.LogError(uwr.error);
                    _onDone?.Invoke(url, null);
                }
                else
                {
                    _onDone?.Invoke(url, "ok");
                }
            }
            HttpMgr.Instance.EndRequest();
        }

    }
}
