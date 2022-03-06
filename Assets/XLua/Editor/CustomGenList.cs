using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameUtils;
using XLua;
using System.Collections;
using UnityEngine.Events;
using NetExtension;

namespace XLuaExtension
{
    public static class CustomGenList
    {
        [LuaCallCSharp]
        public static List<Type> LuaCallCSharp = new List<Type>()
        {
            typeof(ArrayList),
            typeof(List<int>),
            typeof(Action<string>),
            typeof(UIManager),
            typeof(UIAnim), 
            typeof(UIType),
            typeof(LuaTableDataSource),
            typeof(LuaPage),

            typeof(DragonBones.Animation),
            typeof(DragonBones.UnityArmatureComponent),
            typeof(GameDebug),
            typeof(GameController),
            typeof(FileUtils),
            typeof(Tool),
            typeof(SdkMgr),
            typeof(LuaHelper),
            typeof(ResourceMgr),
            typeof(AudioManager),
            typeof(Config),
            typeof(AppConst),
            typeof(ScrollView),
            typeof(ScrollView.ScrollRenderEvent),
            typeof(UnityEvent<int, Transform>),

            typeof(HttpRequest),
            typeof(HttpImage),
            typeof(HttpAudio),
            typeof(SocketClient),
        };

        [CSharpCallLua]
        public static List<Type> CSharpCallLua = new List<Type>()
        {
            typeof(Action),
            typeof(Action<string>),
            //net
            typeof(Action<string,string>),
            typeof(Action<int>),
            typeof(Action<int, byte[]>),
            typeof(Action<byte[]>),
            typeof(Action<long,long>),

            //lua ui
            typeof(Action<LuaTable>),
            typeof(Action<LuaTable,bool>),
            typeof(Action<LuaTable,Button,string>),
            typeof(Action<LuaTable,Toggle,string,bool>),
            typeof(Action<LuaTable,InputField,string,string>),
            typeof(Action<LuaTable,int>),
            typeof(Func<LuaTable>),

            //lua timer
            typeof(Func<int, bool>),
        };
    }
}
