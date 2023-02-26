using NetExtension;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using System.Threading;

namespace GameUtils
{
    public class LocalDataMgr : MonoBehaviour
    {
        class MessageData
        {
            public string url;
            public string json;
            public string date;
        }
        class NetBackData
        {
            public int code;
            public string error;
            public object data; 
        }
        private static Object lockObj = new Object();
        private static string dir = "";
        private const string pathTmp = "/tmp.txt";
        private Queue<MessageData> mMessages = new Queue<MessageData>();
        private MessageData curMessage;
        private bool mBlock = true;
        private HttpRequest mHttp;

        private static LocalDataMgr instance = null;
        public static LocalDataMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.Find(AppConst.SingleObj).TryGetComponent<LocalDataMgr>();
                }
                return instance;
            }
        }

        void Awake()
        {
            mHttp = new HttpRequest(uploadBack);
            dir = Application.persistentDataPath + "/data";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(dir + pathTmp))
                File.Create(dir + pathTmp).Dispose();

            TryUploadData();
        }

        void Update()
        {
            if (!mBlock)
            {
                mBlock = true;
                if (mMessages.Count > 0)
                {
                    curMessage = mMessages.Dequeue();
                    mHttp.Post(curMessage.url, curMessage.json);
                }        
            }
        }


        public void TryUploadData()
        {
            int userId = XLua.LuaManager.getInstance().GetLuaTableValue<int>("Player", "user_id");
            Thread thread01 = new Thread(() =>
            {
                mMessages = new Queue<MessageData>();
                string[] lines = new string[0];
                lock (lockObj)
                {
                    lines = File.ReadAllLines(dir + pathTmp);
                    File.WriteAllBytes(dir + pathTmp, new byte[0]);
                }
                GameDebug.Log("启动上传本地数据条数: " + lines.Length + ",userId: " + userId);
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;
                    MessageData data = JsonMapper.ToObject<MessageData>(line);
                    checkData(data, userId);
                    mMessages.Enqueue(data);
                }
                mBlock = false;
            });
            thread01.Start();
        }

        private void checkData(MessageData data,int userId)
        {
            if (userId == 0) return;
            JsonData jsonData = JsonMapper.ToObject(data.json);
            if ((int)jsonData["user_id"] == 0)
            {
                jsonData["user_id"] = userId;
                data.json = JsonMapper.ToJson(jsonData);
            }
        }

        //缓存数据到本地
        public void PushData(string url, string json)
        {
            mBlock = false;
            MessageData message = new MessageData();
            message.url = url;
            message.json = json;
            message.date = Tool.GetDateTime((int)JsonMapper.ToObject(json)["time_stamp"]).ToString("/yyyy-MM-dd");
            mMessages.Enqueue(message);
        }

        private void uploadBack(string url, string data)
        {
            Debug.Log(data);
            Thread thread01 = new Thread(() =>
            {
                if (string.IsNullOrEmpty(data) || (int)JsonMapper.ToObject(data)["code"] != 0)
                {
                    GameDebug.LogError("数据上传报错：" + url);
                    writeLine(dir + pathTmp, JsonMapper.ToJson(curMessage));
                }
                else
                {
                    if (!File.Exists(dir + curMessage.date))
                        File.Create(dir + curMessage.date).Dispose();
                    writeLine(dir + curMessage.date, curMessage.url + "|" + curMessage.json);
                }
                mBlock = false;
            });
            thread01.Start();
        }

        private void writeLine(string path,string content)
        {
            lock (lockObj)
            {
                if(new FileInfo(path).Length > 0)
                    content = "\n" + content;
                File.AppendAllText(path, content);
            }
        }

        private List<T> readData<T>(string api,int days)
        {
            List<T> records = new List<T>();
            lock (lockObj)
            {
                for (int i = 1; i <= days; i++)
                {
                    string path = dir + System.DateTime.Now.AddDays(-i).ToString("/yyyy-MM-dd");
                    if (!File.Exists(path))
                        continue;
                    string[] lines = File.ReadAllLines(path);
                    foreach (string line in lines)
                    {
                        string[] contents = line.Split('|');
                        if (contents[0].Contains(api))
                            records.Add(JsonMapper.ToObject<T>(contents[1]));
                    }
                }
            }
            return records;
        }


        //获取本地数据
        class GameScoreInfo
        {
            public string date;
            public int time_stamp;
            public int game_id;
            public int score;
            public int skip_type;
        }
        public void GetPastScore(int gameId, int days, XLua.LuaFunction func)
        {
            List<GameScoreInfo> infos = readData<GameScoreInfo>("/data/game", days);
            for(int i = infos.Count-1; i >= 0; i--)
            {
                GameScoreInfo info = infos[i];
                if (info.game_id == gameId)
                    info.date = Tool.GetDateTime(info.time_stamp).ToString("yyyy-MM-dd");
                else
                    infos.RemoveAt(i);
            }

            NetBackData netData = new NetBackData();
            netData.code = 0;
            netData.data = infos;
            func.Call(JsonMapper.ToJson(netData));
        }
    }
}