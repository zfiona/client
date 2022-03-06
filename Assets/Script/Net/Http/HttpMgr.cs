using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NetExtension
{
    public class HttpMgr : MonoBehaviour
    {
        private static int maxExecuteNum = 3;
        private Queue<IEnumerator> httpTasks = new Queue<IEnumerator>();

        private static HttpMgr instance = null;
        public static HttpMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("HttpMgr").AddComponent<HttpMgr>();
                }
                return instance;
            }
        }

        public Coroutine StartRequestDirect(IEnumerator http)
        {
            return StartCoroutine(http);
        }

        public void StartRequestTask(IEnumerator http)
        {
            if (httpTasks.Count < maxExecuteNum)
                StartCoroutine(http);
            else
                httpTasks.Enqueue(http);

        }

        public void EndRequest()
        {
            if (httpTasks.Count > 0)
                StartCoroutine(httpTasks.Dequeue());
        }

        public void ClearCache()
        {
            HttpImage.ClearCache();
            HttpAudio.ClearCache();
            HttpBundle.ClearCache();
        }

    }
}
