using UnityEngine;

namespace XLua
{
    public class LuaManager : MonoBehaviour
    {
        private static LuaManager _luaMrg = null;
        public static LuaManager getInstance()
        {
            if (_luaMrg == null)
            {
                _luaMrg = GameObject.Find(AppConst.SingleObj).TryGetComponent<LuaManager>();
                _luaMrg.Init();
            }
            return _luaMrg;
        }

        internal const float GCInterval = 1;    //second
        private float lastGCTime = 0;
        public LuaEnv luaEnv;
        private void Init()
        {
            luaEnv = new LuaEnv();
            bool isLuaLocal = Setting.Get().resPath == ResPath.Art;
            if (isLuaLocal)
                GameDebug.LogYellow("从lua本地加载代码");
            else
                GameDebug.LogYellow("从AssetBundle加载代码");
            LuaHelper.isLocalFile = isLuaLocal;
            LuaTimer.init();

            luaEnv.AddLoader(LuaHelper.DoFile);
            luaEnv.AddBuildin("rapidjson", LuaDLL.Lua.LoadRapidJson);
            luaEnv.AddBuildin("lpeg", LuaDLL.Lua.LoadLpeg);
            luaEnv.AddBuildin("pb", LuaDLL.Lua.LoadLuaProfobuf);
            luaEnv.AddBuildin("ffi", LuaDLL.Lua.LoadFFI);
        }

        public void StartMain()
        {         
            luaEnv.DoString("require 'common.main'");
            luaEnv.Global.Get<LuaFunction>("main").Call();
        }

        public T GetLuaTableValue<T>(string t,string key)
        {
            if (luaEnv == null) 
                return default(T);
            LuaTable table = luaEnv.Global.GetInPath<LuaTable>(t);
            if(table == null)
                return default(T);
            return table.Get<T>(key);
        }

        public void CallLuaFunction(string fn, string args)
        {
            if (luaEnv == null) return;
            LuaFunction func = luaEnv.Global.GetInPath<LuaFunction>(fn);
            if (func != null)
            {
                func.Action(args);
                func.Dispose();
            }
        }

        private void CallLuaFunction(string fn, bool args)
        {
            if (luaEnv == null) return;
            LuaFunction func = luaEnv.Global.GetInPath<LuaFunction>(fn);
            if (func != null)
            {
                func.Action(args);
                func.Dispose();
            }
        }

        //void OnDestroy()
        //{
        //    if (luaEnv == null) return;
        //    GameDebug.Log("luaEnv被释放");
        //    luaEnv.Dispose();
        //}

        void OnApplicationPause(bool pause)
        {
            CallLuaFunction("App.OnApplicationPauseCallBack", pause);
        }

        void Update()
        {
            if (luaEnv == null) return;
            LuaTimer.tick(Time.unscaledDeltaTime);//Time.deltaTime受时间缩放影响
            if (Time.time - lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                lastGCTime = Time.time;
            }
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
            {
                CallLuaFunction("App.OnEscape", null);
            }
        }
    }
}


