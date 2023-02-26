using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XLua;

namespace NetExtension
{
    public class WebSocket
    {
        readonly ClientWebSocket ws = null;
        readonly Uri uri = null;
        bool isUserClose = false;//是否最后由用户手动关闭
        bool isReceiving = false;
        int MAX_READ = 4096;

        public int timeoutInMillionSeconds = 6000;
        
        public WebSocket(LuaTable table, string wsUrl)
        {
            uri = new Uri(wsUrl);
            ws = new ClientWebSocket();
            WebSocketMgr.Instance.NewClient(this,table);
        }

        /// <summary>
        /// 打开链接
        /// </summary>
        public void Open()
        {
            Task.Run(async () =>
            {
                if (ws.State == WebSocketState.Connecting || ws.State == WebSocketState.Open)
                    ws.Abort();

                string netErr = string.Empty;
                try
                {
                    //初始化链接
                    isUserClose = false;
                    await ws.ConnectAsync(uri, CancellationToken.None);
                    WebSocketMgr.Instance.AddConnectEvent(NetConnectState.Succ);

                    await Task.Delay(100);
                    //全部消息容器
                    List<byte> bs = new List<byte>();
                    //缓冲区
                    var buffer = new byte[MAX_READ];
                    //监听Socket信息
                    WebSocketReceiveResult result;
                    while (ws.State == WebSocketState.Open)
                    {
                        result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            bs.AddRange(buffer.Take(result.Count));

                            //消息是否已接收完全
                            if (result.EndOfMessage)
                            {
                                //发送过来的消息
                                string mess = Encoding.UTF8.GetString(bs.ToArray(), 0, bs.Count);
                                GameDebug.LogYellow("recv: " + mess);
                                WebSocketMgr.Instance.AddMessageEvent(mess);
                                isReceiving = false;

                                //清空消息容器
                                bs = new List<byte>();
                            }
                        }
                        else if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            GameDebug.LogError("error messageType");
                        }                      
                    }
                }
                catch (Exception ex)
                {
                    netErr = ex.Message;
                    GameDebug.LogError(netErr);
                    WebSocketMgr.Instance.AddConnectEvent(NetConnectState.Error);
                }
                finally
                {
                    if (!isUserClose)
                        Close(ws.CloseStatus.Value, ws.CloseStatusDescription + netErr);
                }
            });
        }

        /// <summary>
        /// 使用连接发送文本消息
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="mess"></param>
        /// <returns>是否尝试了发送</returns>
        public bool Send(string mess)
        {
            GameDebug.LogYellow("send: " + mess);
            if (ws.State != WebSocketState.Open)
            {
                GameDebug.LogError("Connection is not open.");
                return false;
            }
            
            Task.Run(async () =>
            {
                //发送消息
                var replyMess = Encoding.UTF8.GetBytes(mess);
                await ws.SendAsync(new ArraySegment<byte>(replyMess), WebSocketMessageType.Text, true, CancellationToken.None);
                CheckConnectWithTimeout(timeoutInMillionSeconds);
            });

            return true;
        }

        /// <summary>
        /// 使用连接发送字节消息
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="mess"></param>
        /// <returns>是否尝试了发送</returns>
        public bool Send(byte[] bytes)
        {
            if (ws.State != WebSocketState.Open)
            {
                GameDebug.LogError("Connection is not open.");
                return false;
            }

            Task.Run(async () =>
            {
                //发送消息
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
                CheckConnectWithTimeout(timeoutInMillionSeconds);
            });

            return true;
        }

        /// <summary>
        /// 检查网络连接情况
        /// </summary>
        /// <param name="timeOut"></param>
        public async void CheckConnectWithTimeout(int timeOut)
        {
            if (isReceiving)
                return;
            isReceiving = true;
            await Task.Delay(timeOut);
            if (isReceiving)
            {
                GameDebug.LogError(string.Format("net connect error: {0}", "timeout"));
                WebSocketMgr.Instance.AddConnectEvent(NetConnectState.Error);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            isUserClose = true;
            Close(WebSocketCloseStatus.NormalClosure, "client close");
        }

        private void Close(WebSocketCloseStatus closeStatus, string statusDescription)
        {
            Task.Run(async () =>
            {
                //关闭WebSocket（客户端发起）
                await ws.CloseAsync(closeStatus, statusDescription, CancellationToken.None);

                ws.Abort();
                ws.Dispose();
            });
        }
        
    }
}
   