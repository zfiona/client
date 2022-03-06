using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetExtension
{
    public class SocketClient
    {
        enum NetState
        {
            None = 0,
            Connecting,
            ConnectError,
            Established,
            Disconnected,
        }

        private TcpClient client;
        private NetworkStream netStream;
        private MemoryStream readerStream;
        private BinaryReader reader;
        //private const int head_flag = 0x1234;//校验码
        private const int MAX_READ = 2048;//读缓冲大小
        private byte[] byteBuffer = new byte[MAX_READ];
        private NetState curNetState = NetState.None;
        private int curConnectId = 0;

        public SocketClient(XLua.LuaTable table)
        {
            SocketMgr.Instance.NewClient(this, table);
        }
        public SocketClient(Action<int> onConn, Action<int, byte[]> onMsg)
        {
            SocketMgr.Instance.NewClient(this, onConn, onMsg);
        }

        private void OnConnectEvent(NetConnectState state)
        {
            SocketMgr.Instance.AddConnectEvent(state);
        }

        private void OnMessageEvent(NetData data)
        {
            SocketMgr.Instance.AddMessageEvent(data);
        }

        public void ClearMessage()
        {
            if (curNetState == NetState.Established)
            {
                readerStream.Position = 0;
                readerStream.SetLength(0);
                Array.Clear(byteBuffer, 0, byteBuffer.Length);
            }
            SocketMgr.Instance.ClearMessageQueue();
        }

        public void CloseConnet()
        {
            if (curNetState == NetState.Established)
            {
                readerStream.Close();
                reader.Close();
                netStream.Close();
                curNetState = NetState.None;
                try
                {
                    client.Close();
                }
                catch (Exception ex)
                {
                    GameDebug.Log(string.Format("net disconnect error: {0}", ex.Message));
                }
            }
        }

        public void SendConnect(string host, int port, int millionseconds)
        {
            GameDebug.LogYellow("Connect IP:" + host + "  Port:" + port);
            CloseConnet();

            readerStream = new MemoryStream();
            reader = new BinaryReader(readerStream);
            client = new TcpClient();
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;//关闭写缓存

            curConnectId += 1;
            curNetState = NetState.Connecting;
            client.BeginConnect(host, port, new AsyncCallback(OnConnectCallback), curConnectId);
            if (millionseconds > 0)
                checkConnectWithTimeout(millionseconds);
        }

        private async void checkConnectWithTimeout(int timeoutInMillionSeconds)
        {
            await Task.Delay(timeoutInMillionSeconds);
            if (curNetState == NetState.Connecting)
            {
                GameDebug.LogError(string.Format("net connect error: {0}", "timeout"));
                curNetState = NetState.ConnectError;
                client.Close();
                OnConnectEvent(NetConnectState.Fail);
            }
        }

        private void OnConnectCallback(IAsyncResult asr)
        {
            int backId = (int)asr.AsyncState;
            if (backId == curConnectId && curNetState == NetState.Connecting)
            {
                try
                {
                    client.EndConnect(asr);
                }
                catch (Exception ex)
                {
                    GameDebug.LogError(string.Format("net connect error: {0}", ex.Message));
                    curNetState = NetState.ConnectError;
                    OnConnectEvent(NetConnectState.Fail);
                    return;
                }
                curNetState = NetState.Established;
                netStream = client.GetStream();
                netStream.BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnReadCallBack), curConnectId);
                OnConnectEvent(NetConnectState.Succ);
            }
        }

        private void OnReadCallBack(IAsyncResult asr)
        {
            int backId = (int)asr.AsyncState;
            if (backId == curConnectId && curNetState == NetState.Established)
            {
                try
                {
                    int bytesRead = 0;
                    lock (client.GetStream())
                        bytesRead = netStream.EndRead(asr);
                    if (bytesRead < 1)
                    {
                        GameDebug.LogError(string.Format("net receive error"));
                        curNetState = NetState.Disconnected;
                        OnConnectEvent(NetConnectState.Error);
                        return;
                    }
                    ReceiveMessage(byteBuffer, bytesRead);
                    //继续监听
                    lock (client.GetStream())
                    {
                        Array.Clear(byteBuffer, 0, byteBuffer.Length);
                        netStream.BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnReadCallBack), curConnectId);
                    }  
                }
                catch (Exception ex)
                {
                    GameDebug.LogError(string.Format("net receive error: {0}", ex.Message));
                    curNetState = NetState.Disconnected;
                    OnConnectEvent(NetConnectState.Error);
                }
            }
        }

        private void ReceiveMessage(byte[] bytes, int length)
        {
            //GameDebug.Log(length);
            readerStream.Seek(0, SeekOrigin.End);
            readerStream.Write(bytes, 0, length);
            while (true)
            {
                readerStream.Seek(0, SeekOrigin.Begin);
                if (RemainingBytes() < 2)
                    return;

                short msgLen = reader.ReadShort();
                if (RemainingBytes() < msgLen)
                {
                    readerStream.Position -= 2;
                    return;
                }  
                short cmd = reader.ReadShort();
                byte[] message = reader.ReadBytes(msgLen - 2);
                GameDebug.LogYellow("receive : " + msgLen + " | " + cmd);
                OnMessageEvent(new NetData(cmd, message));
                //粘包
                byte[] unread_bytes = reader.ReadBytes(RemainingBytes());
                readerStream.Position = 0;
                readerStream.SetLength(0);
                readerStream.Write(unread_bytes, 0, unread_bytes.Length);
            }
        }

        private int RemainingBytes()
        {
            return (int)(readerStream.Length - readerStream.Position);
        }

        public void SendMessage(short cmd, byte[] bytes)
        {
            if (curNetState == NetState.Established && client.Connected)
            {
                MemoryStream ms = null;
                using (ms = new MemoryStream())
                {
                    ms.Position = 0;
                    BinaryWriter writer = new BinaryWriter(ms);
                    writer.WriteShort((short)(2 + bytes.Length));
                    writer.WriteShort(cmd);
                    writer.Write(bytes);
                    writer.Flush();

                    GameDebug.LogYellow("send : " + bytes.Length + " | " + cmd);
                    byte[] message = ms.ToArray();
                    netStream.BeginWrite(message, 0, message.Length, new AsyncCallback(OnSendCallBack), curConnectId);
                }
            }
            else
            {
                GameDebug.LogError("server is disconnected");
            }
        }

        private void OnSendCallBack(IAsyncResult asr)
        {
            int backId = (int)asr.AsyncState;
            if (backId == curConnectId && curNetState == NetState.Established)
            {
                try
                {
                    netStream.EndWrite(asr);
                }
                catch (Exception ex)
                {
                    GameDebug.LogError(string.Format("net send error: {0}", ex.Message));
                    curNetState = NetState.Disconnected;
                    OnConnectEvent(NetConnectState.Error);
                }
            }
        }


        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="ms"></param>
        //void OnReceivedMessage(MemoryStream ms)
        //{
        //    BinaryReader r = new BinaryReader(ms);
        //    int length = IPAddress.NetworkToHostOrder(r.ReadInt32());
        //    int cmd = IPAddress.NetworkToHostOrder(r.ReadInt32());
        //    int uid = IPAddress.NetworkToHostOrder(r.ReadInt32());
        //    int sid = IPAddress.NetworkToHostOrder(r.ReadInt32());
        //    byte[] message = r.ReadBytes((int)ms.Length - 16);
        //    ByteBuffer buffer = new ByteBuffer(message);
        //    Debug.Log("接收到包体内容=====>" + " cmd; " + cmd.ToString() + " uid; " + uid.ToString() + " sid; " + sid.ToString() + " length; " + message.Length);
        //    buffer.Remains = message.Length;
        //    NetworkManager.AddEvent(cmd, new LuaByteBuffer(message));
        //}
    }
}

