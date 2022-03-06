using XLua;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NetExtension
{
    public enum NetConnectState
    {
        None,
        Succ,
        Fail,
        Error,
    }
    public class NetData
    {
        public short cmd;
        public byte[] buffer;
        public NetData(short m_id, byte[] b)
        {
            cmd = m_id;
            buffer = b;
        }
    }

    public class SocketMgr : MonoBehaviour
    {
        private static SocketMgr instance = null;
        public static SocketMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("SocketMgr").AddComponent<SocketMgr>();
                    BinaryExt.SetLittleEndian(true);
                }
                return instance;
            }
        }
        
        private SocketClient socket;
        private Queue<NetData> mEvents = new Queue<NetData>();
        private NetConnectState mState = NetConnectState.None;
        private Action<int, byte[]> _onNetworkMsg;
        private Action<int> _onNetworkConn;
        
        public void NewClient(SocketClient client, LuaTable table)
        {
            socket = client;
            table.Get("ConnectCallBack", out _onNetworkConn);
            table.Get("MsgCallback", out _onNetworkMsg);
        }

        public void NewClient(SocketClient client, Action<int> onConn, Action<int, byte[]> onMsg)
        {
            socket = client;
            _onNetworkConn = onConn;
            _onNetworkMsg = onMsg;
        }

        public void AddMessageEvent(NetData netData)
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

        void Update()
        {
            if (mEvents.Count > 0)
            {
                while (mEvents.Count > 0)
                {
                    NetData data = mEvents.Dequeue();
                    _onNetworkMsg?.Invoke(data.cmd, data.buffer);
                }
            }
            if (mState != NetConnectState.None)
            {
                int code = (int)mState;
                mState = NetConnectState.None;
                _onNetworkConn?.Invoke(code);
            }
        }

        void OnDestroy()
        {
            _onNetworkMsg = null;
            _onNetworkConn = null;
            socket.CloseConnet();
        }
    }
}