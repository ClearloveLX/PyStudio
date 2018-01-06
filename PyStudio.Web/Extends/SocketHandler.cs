using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PyStudio.Web.Extends
{
    public class SocketHandler
    {
        private static List<WebSocket> _sockets = new List<WebSocket>();
        public const int BufferSize = 4096;
        public static object objLock = new object();
        public static List<ChatData> historicalMessg = new List<ChatData>();//存放歷史消息--後續改為數據庫存儲。

        static async Task Acceptor(HttpContext httpContext, Func<Task> n)
        {
            if (!httpContext.WebSockets.IsWebSocketRequest)
            {
                return;
            }
            //建立一个WebSocket连接请求
            var socket = await httpContext.WebSockets.AcceptWebSocketAsync();
            //判断限制最大连接数
            if (_sockets.Count >= 100)
            {
                //限制加入
                await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "链接超过最大限制，请稍后加入群聊...", CancellationToken.None);
                return;
            }
            lock (objLock)
            {
                _sockets.Add(socket);//加入群聊
            }
            var buffer = new byte[BufferSize];

            //根据请求头获取用户名
            string userName = httpContext.Request.Query["userName"].ToString();

            var chatData = new ChatData() { Info = $"{userName}进入房间。当前在线{_sockets.Count}人~~~" };
            //群发上线通知
            await SendToWebSocketsAsync(_sockets, chatData);

            while (true)
            {
                try
                {
                    //建立连接，阻塞等待接收消息
                    var incoming = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    //如果主动退出，则移除
                    if (incoming.MessageType == WebSocketMessageType.Close)
                    {
                        lock (objLock)
                        {
                            _sockets.Remove(socket);//移除用户
                        }
                        chatData = new ChatData() { Info = $"{userName}离开房间。还剩{_sockets.Count}~~~" };
                        await SendToWebSocketsAsync(_sockets, chatData);
                        break;//这里一定要跳出循环
                    }
                    var chatDataStr = await ArraySegmentToStringAsync(new ArraySegment<byte>(buffer, 0, incoming.Count));
                    if (chatDataStr == "heartbeat")//如果是心跳检查，则直接跳过
                        continue;
                    chatData = JsonConvert.DeserializeObject<ChatData>(chatDataStr);
                    chatData.Time = DateTime.Now;//使用服务器时间 
                    await SendToWebSocketsAsync(_sockets.Where(t => t != socket).ToList(), chatData);
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    _sockets.Remove(socket);//移除
                    chatData = new ChatData() { Info = $"{userName}离开房间。还剩{_sockets.Count}人~~~" };
                    await SendToWebSocketsAsync(_sockets, chatData);
                    //发送关闭 这里很重要 （如果不发送关闭会一直循环，且不能直接break。）
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "未知异常 ...", CancellationToken.None);
                }
            }
        }

        /// <summary>
        /// 发送消息到所有人
        /// </summary>
        /// <param name="sockets"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async static Task SendToWebSocketsAsync(List<WebSocket> sockets, ChatData data)
        {
            SaveHistoricalMessg(data);//保存历史消息
            var chatData = JsonConvert.SerializeObject(data);
            var buffer = Encoding.UTF8.GetBytes(chatData);
            ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer);
            //循环发送消息
            for (int i = 0; i < sockets.Count; i++)
            {
                var tempsocket = sockets[i];
                if (tempsocket.State == WebSocketState.Open)
                {
                    //发送消息
                    await tempsocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        static object lockSaveMsg = new object();

        /// <summary>
        /// 保存历史消息
        /// </summary>
        /// <param name="data"></param>
        public static void SaveHistoricalMessg(ChatData data)
        {
            var size = 40;
            lock (lockSaveMsg)
            {
                historicalMessg.Add(data);
            }
            if (historicalMessg.Count >= size)
            {
                lock (lockSaveMsg)
                {
                    historicalMessg.RemoveRange(0, 30);
                }
            }
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="arraySegment"></param>
        /// <returns></returns>
        static async Task<string> ArraySegmentToStringAsync(ArraySegment<byte> arraySegment)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets(); //nuget   导入 Microsoft.AspNetCore.WebSockets.Server
            app.Use(Acceptor);
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="message"></param>
        private static void Log(string message)
        {
            dynamic type = (new Program()).GetType();
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location) + "/log.txt";
            File.WriteAllText(currentDirectory, message);
        }
    }

    public class ChatData
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("time")]
        public DateTime Time { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("sex")]
        public string Sex { get; set; }
        [JsonProperty("info")]
        public string Info { get; set; }
    }
}
