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
                _luaMrg = GameObject.FindObjectOfType(typeof(LuaManager)) as LuaManager;
                if (_luaMrg == null)
                {
                    _luaMrg = GameObject.Find(AppConst.SingleObj).AddComponent<LuaManager>();
                }
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
#if UNITY_EDITOR
            bool isLuaLocal = !Setting.Get().isLuaZip;
#else
            bool isLuaLocal = false;
#endif
            if (!isLuaLocal)
                GameDebug.LogYellow("从AssetBundle加载代码");
            else
                GameDebug.LogYellow("从lua本地加载代码");
            LuaHelper.isLocalFile = isLuaLocal;
            luaEnv.AddLoader(LuaHelper.DoFile);
            LuaTimer.init();
        }

        public void StartMain()
        {
            luaEnv.AddBuildin("rapidjson", LuaDLL.Lua.LoadRapidJson);
            luaEnv.AddBuildin("lpeg", LuaDLL.Lua.LoadLpeg);
            luaEnv.AddBuildin("pb", LuaDLL.Lua.LoadLuaProfobuf);
            luaEnv.AddBuildin("ffi", LuaDLL.Lua.LoadFFI);
            luaEnv.DoString("require 'common.main'");
        }

        public LuaFunction GetLuaFunction(string fn)
        {
            if (luaEnv == null) return null;
            LuaFunction func = luaEnv.Global.GetInPath<LuaFunction>(fn);
            return func;
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
        //    luaEnv.Dispose();
        //}

        void OnApplicationPause(bool pause)
        {
            CallLuaFunction("App.OnApplicationPauseCallBack", pause);
        }

        void Update()
        {
            if (luaEnv == null) return;
            LuaTimer.tick(Time.deltaTime);
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


