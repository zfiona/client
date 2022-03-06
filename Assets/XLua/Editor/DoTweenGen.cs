using DG.Tweening;
using System;
using System.Collections.Generic;
using XLua;

namespace XLuaExtension
{
    public static class DoTweenGen
    {
        [LuaCallCSharp]
        public static List<Type> LuaCallCSharp = new List<Type>()
        {
            typeof(DOTween),
            typeof(Tween),
            typeof(Sequence),
            typeof(Tweener),
            typeof(TweenCallback),
            typeof(Ease),
            typeof(LoopType),
            typeof(PathMode),
            typeof(PathType),
            typeof(RotateMode),
            typeof(ScrambleMode),
            typeof(TweenExtensions),
            typeof(TweenSettingsExtensions),
            typeof(ShortcutExtensions),
            typeof(ShortcutExtensions46),
        };

        [CSharpCallLua]
        public static List<Type> CSharpCallLua = new List<Type>()
        {
            typeof(TweenCallback),
            typeof(TweenCallback<>),
        };
    }
}

