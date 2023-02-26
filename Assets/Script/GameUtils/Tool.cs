using DG.Tweening;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace GameUtils
{
    public class Tool
    {
        public static int GetPlatform()
        {
#if UNITY_EDITOR
            return 0;
#elif UNITY_ANDROID
            return 1;// "android";
#elif UNITY_IPHONE
            return  2;//"ios";
#else
            return 3; //"web"
#endif
        }

        //设备唯一ID
        public static string GetDeviceUID()
        {
            return SystemInfo.deviceName + "-" +SystemInfo.deviceUniqueIdentifier;
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

        //获取guid
        public static string getGUID()
        {
            return Guid.NewGuid().ToString();
        }

        public static bool GetRectPosFromScreenPos(Canvas cvs, Vector2 ScreenPos, out Vector2 RectTransPos)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(cvs.transform as RectTransform, ScreenPos, cvs.worldCamera, out RectTransPos);
        }

        public static bool GetWorldPosFromScreenPos(Canvas cvs, Vector2 ScreenPos, out Vector3 worldPos)
        {
            return RectTransformUtility.ScreenPointToWorldPointInRectangle(cvs.transform as RectTransform, ScreenPos, cvs.worldCamera, out worldPos);
        }

        //截图
        public static Sprite CaptureTexture(RectTransform rt,string path,Vector2 size=default)
        {
            Rect rect = getFrameRect(rt);
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0, false);
            screenShot.Apply();

            if (size != default)
                screenShot = ScaleTexture(screenShot, (int)size.x, (int)size.y);
            byte[] bytes = screenShot.EncodeToJPG();

            string dir = path.Remove(path.LastIndexOf('/'));
            FileUtils.ins.createDirectory(dir);
            File.WriteAllBytes(path, bytes); 
            Sprite sprite = Sprite.Create(screenShot, new Rect(0, 0, screenShot.width, screenShot.height), Vector2.one * 0.5f);
            return sprite;
        }

        //获取相对原点rect
        public static Rect getFrameRect(RectTransform rt)
        {
            var worldCorners = new Vector3[4];
            rt.GetWorldCorners(worldCorners);

            var bottomLeft = worldCorners[0];
            var topLeft = worldCorners[1];
            var topRight = worldCorners[2];
            var bottomRight = worldCorners[3];
            var canvas = rt.GetComponentInParent<Canvas>();
            if (canvas == null)
                return rt.rect;

            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    break;
                case RenderMode.ScreenSpaceCamera:
                case RenderMode.WorldSpace:
                    var camera = canvas.worldCamera;
                    if (camera == null)
                        return rt.rect;
                    else
                    {
                        bottomLeft = camera.WorldToScreenPoint(bottomLeft);
                        topLeft = camera.WorldToScreenPoint(topLeft);
                        topRight = camera.WorldToScreenPoint(topRight);
                        bottomRight = camera.WorldToScreenPoint(bottomRight);
                    }
                    break;
            }

            float x = topLeft.x;
            //float y = Screen.height - topLeft.y; //左上原点
            float y = bottomLeft.y; //左下原点
            float width = topRight.x - topLeft.x;
            float height = topRight.y - bottomRight.y;
            return new Rect(x, y, width, height);          
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

        //字符串转图片
        public static Sprite Base64ToSprite(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            Texture2D tex2D = new Texture2D(100, 100);
            tex2D.LoadImage(bytes);
            Sprite s = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), Vector2.one * 0.5f);
            return s;
        }

        // 【秒级】获取时间（北京时间）
        public static DateTime GetDateTime(long timestamp)
        {
            long begtime = timestamp * 10000000;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 8, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime
            return dt;
        }

        // 【秒级】生成10位时间戳（北京时间）
        public static long GetTimeStamp(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return Convert.ToInt64((dt - dateStart).TotalSeconds);
        }

        public static void AutoSelectKeyboard(InputField input)
        {
#if !UNITY_EDITOR
            input.touchScreenKeyboard.selection = new RangeInt(0, input.touchScreenKeyboard.text.Length);
#endif
        }


        //--------------------------------分割线-------------------------------------
        public static string Format(string str, params object[] objs)
        {
            return string.Format(str, objs);
        }

        public static void ForceRebuildLayoutImmediate(RectTransform rect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        public static bool IsNull(UnityEngine.Object o) // 或者名字叫IsDestroyed等等
        {
            return o == null;
        }
        public static string GetScreenTexture(RectTransform rectT, int num)
        {

            var ratio = (float)1536 / Screen.width;
            var UIHeight = ratio * Screen.height;
            float x = rectT.localPosition.x + (1536 - rectT.rect.width) / 2;
            float y = rectT.localPosition.y + (UIHeight - rectT.rect.height) / 2;
            var width = rectT.rect.width;
            var height = rectT.rect.height;
            var screenWidth = width / 1536 * Screen.width;
            var screenHeight = height / UIHeight * Screen.height;
            Texture2D screenShot = new Texture2D((int)screenWidth, (int)screenHeight, TextureFormat.RGB24, true);


            Rect position = new Rect(x, y, screenWidth, screenHeight);
            Debug.Log(position);
            screenShot.ReadPixels(position, 0, 0, true);//按照设定区域读取像素；注意是以左下角为原点读取
            screenShot.Apply();


            string filePath = "";
            filePath = Application.persistentDataPath + "/survey";
            string scrPathName = filePath + "/" + num + ".jpg";

            if (Directory.Exists(scrPathName))
            {
                Directory.Delete(scrPathName);
            }
            Directory.CreateDirectory(filePath);
            //二进制转换
            byte[] byt = screenShot.EncodeToJPG();
            File.WriteAllBytes(scrPathName, byt);
            //System.Diagnostics.Process.Start(scrPathName);

            return scrPathName;
        }
        /// <summary>
        /// 标准正态分布的累计密度函数
        /// </summary>
        /// <param name="x">需要先标准化</param>
        /// <returns></returns>
        public static double Phi(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;
        
            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x) / Math.Sqrt(2.0);
        
            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p*x);
            double y = 1.0 - (((((a5*t + a4)*t) + a3)*t + a2)*t + a1)*t * Math.Exp(-x*x);
        
            return 0.5 * (1.0 + sign*y);
        }
    }
}
