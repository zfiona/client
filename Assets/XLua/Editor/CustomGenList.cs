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
            typeof(Array),
            typeof(ArrayList),
            typeof(List<int>),
            typeof(Action<string>),
            typeof(UIManager),
            typeof(Root),
            typeof(UIAnim), 
            typeof(UIType),
            typeof(LuaTableDataSource),
            typeof(Spine.Unity.SkeletonGraphic),
            typeof(Spine.Skeleton),
            typeof(Spine.Skin),
            typeof(Spine.AnimationState),
            typeof(Spine.TrackEntry),
            typeof(GameDebug),
            typeof(GameController),
            typeof(FileUtils),
            typeof(EncryptUtil),
            typeof(Tool),
            typeof(SdkMgr),
            typeof(LuaHelper),
            typeof(ResourceMgr),
            typeof(AudioManager),
            typeof(Config),
            typeof(AppConst),
          
            typeof(HttpRequest),
            typeof(HttpImage),
            typeof(HttpAudio),
            typeof(SocketClient),
            typeof(UICardEvent),
            typeof(UIDragEvent),
            typeof(UIPointEvent),
            typeof(UIUpdateEvent),
            typeof(LocalDataMgr),
            typeof(XCharts.Runtime.BaseChart),
            typeof(XChartLuaHelper),
            typeof(UnityPlayableHelper),
            typeof(DrawLine),
            typeof(LayoutRebuilder),
            typeof(UIDrawCtrl),
			typeof(StraightLine),
        };

        [CSharpCallLua]
        public static List<Type> CSharpCallLua = new List<Type>()
        {
            typeof(Action),
            typeof(Action<string>),
            typeof(Action<bool,string>),
            typeof(Action<int,string>),
            //net
            typeof(Action<string,string>),
            typeof(Action<int>),
            typeof(Action<int, byte[]>),
            typeof(Action<byte[]>),
            typeof(Action<long,long>),
            typeof(Action<string[]>),
            //http
            typeof(Action<Texture2D>),
            typeof(Action<Sprite>),
           
            //lua ui
            typeof(Action<LuaTable>),
            typeof(Action<LuaTable,bool>),
            typeof(Action<LuaTable,Button,string>),
            typeof(Action<LuaTable,Toggle,string,bool>),
            typeof(Action<LuaTable,InputField,string,string>),
            typeof(Action<LuaTable,int>),
            typeof(Action<LuaTable,float>),
            typeof(Action<LuaTable,string>),
            typeof(Func<LuaTable>),
           
            //lua timer
            typeof(Func<int, bool>),
            typeof(UnityAction<Vector2>),
        };

        [BlackList]
        public static List<List<string>> BlackList = new List<List<string>>()
        {
            new List<string>(){ "Config", "CompareVersion","System.String"},
        };
    }
}
