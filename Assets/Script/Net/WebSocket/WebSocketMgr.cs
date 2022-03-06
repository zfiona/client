using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetExtension
{
    public class WebSocketMgr : MonoBehaviour
    {
        private static WebSocketMgr instance = null;
        public static WebSocketMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("SocketMgr").AddComponent<WebSocketMgr>();
                }
                return instance;
            }
        }

  
        public Action<int> _onNetworkConn;
        public Action<string> _onNetworkMsg;

        private WebSocket socket;
        private Queue<string> mEvents = new Queue<string>();
        private NetConnectState mState = NetConnectState.None;
        public void NewClient(WebSocket client,XLua.LuaTable table)
        {
            socket = client;
            table.Get("OnConnect", out _onNetworkConn);
            table.Get("OnMessage", out _onNetworkMsg);
        }
        public void AddMessageEvent(string netData)
        {
            mEvents.Enqueue(netData);
        }
        public void AddConnectEvent(NetConnectState state)
        {
            mState = state;
        }
        public void ClearMessageQueue()
        {
            mEvents.Clear();
        }
        public void CheckConnect()
        {
            if (socket != null)
                socket.CheckConnectWithTimeout(2000);
        }

        private void Update()
        {
            if (mEvents.Count > 0)
            {
                while (mEvents.Count > 0)
                {
                    string data = mEvents.Dequeue();
                    _onNetworkMsg?.Invoke(data);
                }
            }
            if (mState != NetConnectState.None)
            {
                int code = (int)mState;
                mState = NetConnectState.None;
                _onNetworkConn?.Invoke(code);
            }

            //test
            if (Input.GetKeyDown(KeyCode.A))
            {
                Content content = new Content();
                content.module = "mission";
                content.action = "check";
                content.http_seqno = "hsn16365280242057840";
                Msg msg = new Msg();
                msg.message = content;
                string json = LitJson.JsonMapper.ToJson(msg);

                socket.Send(json);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                socket.Close();
            }
        }

        void OnDestroy()
        {
            _onNetworkConn = null;
            _onNetworkMsg = null;
            socket.Close();
        }

        public class Msg
        {
            public Content message;
        }
        public class Content
        {
            public string module;
            public string action;
            public string http_seqno;
        }
    }
}

