// UnityPlayableHelper.cs
// 创建者： 张毅文
// 创建时间：2022/06/23
// 概要：

using System;
using UnityEngine.Playables;

public static class UnityPlayableHelper
{
    public static void SetSpeed(this Playable playable, double value)
    {
        PlayableExtensions.SetSpeed(playable, value);
    }
}