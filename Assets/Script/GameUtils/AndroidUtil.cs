#if UNITY_ANDROID
using UnityEngine;

public class AndroidUtil
{
    /// <summary>
    /// Android主Player
    /// </summary>
    public const string S_MainPlayer = "com.unity3d.player.UnityPlayer";
    /// <summary>
    /// Android主Activity
    /// </summary>
    public const string S_MainAct = "currentActivity";

    public static void Call(string methodName, params object[] param)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass(S_MainPlayer))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>(S_MainAct))
            {
                if (param != null && param.Length > 0)
                {
                    jo.Call(methodName, param);
                }
                else
                {
                    jo.Call(methodName);
                }
            }

        }
    }
    public static T Call<T>(string methodName, params object[] param)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass(S_MainPlayer))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>(S_MainAct))
            {
                if (param != null && param.Length > 0)
                {
                    return jo.Call<T>(methodName, param);
                }
                else
                {
                    return jo.Call<T>(methodName);
                }
            }
        }
    }

    public static T SocialUtil<T>(string methodName, params object[] param)
    {
        using (AndroidJavaObject obj = new AndroidJavaClass("com.yixun.tools.SocialUtil").
                                 CallStatic<AndroidJavaObject>("getInstance"))
        {
            if (param != null && param.Length > 0)
            {
                return obj.Call<T>(methodName, param);
            }
            else
            {
                return obj.Call<T>(methodName);
            }

        }
    }

    public static void SocialUtil(string methodName, params object[] param)
    {

        using (AndroidJavaObject obj = new AndroidJavaClass("com.yixun.tools.SocialUtil").
                                CallStatic<AndroidJavaObject>("getInstance"))
        {
            if (param != null && param.Length > 0)
            {
                obj.Call(methodName, param);
            }
            else
            {
                obj.Call(methodName);
            }
        }
    }
}

#endif