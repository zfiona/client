using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;

namespace GameUtils
{
    public class Tool
    {
        public static int GetPlatform()
        {
#if UNITY_ANDROID
            return 1;// "android";
#elif UNITY_IPHONE
            return  2;//"ios";
#else
            return  0;//"web";
#endif
        }

        //设备唯一ID
        public static string GetDeviceUID()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public static string GetDeviceModel()
        {
            return SystemInfo.deviceModel;
        }

        public static string GetDeviceName()
        {
            return SystemInfo.deviceName;
        }

        //获取本机时区
        public static double GetZone()
        {
            DateTime utcTime = DateTime.Now.ToUniversalTime();
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - utcTime.Ticks);
            return ts.TotalHours;
        }

        //获取本机语言
        public static string GetLanguage()
        {
            return Application.systemLanguage.ToString();
        }

        public static string getGUID()
        {
            return Guid.NewGuid().ToString();
        }

        public static float Lerp(float a,float b,float t)
        {
            return Mathf.Lerp(a,b,t);
        }

        public static string Format(string str,params object[] objs)
        {
            return string.Format(str, objs);
        }

        public static void ForceRebuildLayoutImmediate(RectTransform rect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        //截屏
        public static Texture2D CaptureScreen(Camera c, Rect r)
        {
            RenderTexture rt = new RenderTexture((int)r.width, (int)r.height, 0);

            c.targetTexture = rt;
            c.Render();

            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(r, 0, 0);
            screenShot.Apply();

            c.targetTexture = null;
            RenderTexture.active = null;

            return screenShot;
        }

        //字符串转图片
        public static Sprite Base64ToImg(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            Texture2D tex2D = new Texture2D(100, 100);
            tex2D.LoadImage(bytes);
            Sprite s = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
            return s;
        }

        // 图片压缩大小
        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear(j / (float)result.width, i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }
            result.Apply();
            return result;
        }
    }
}
