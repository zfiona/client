using System;
using System.Runtime.InteropServices;
using UnityEngine;
using XLua;

public class SdkMgr : MonoBehaviour
{
    private static AndroidJavaObject mainObject;
    private static AndroidJavaClass socialObject;
    private static AndroidJavaClass fileClass;
    private static AndroidJavaClass ossClass;
    private static AndroidJavaObject ttsObject;
    private static AndroidJavaObject trainObject;
    private static AndroidJavaObject recordObject;

    private static SdkMgr instance = null;
    public static SdkMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find(AppConst.SingleObj).TryGetComponent<SdkMgr>();
            }
            return instance;
        }
    }
    void Awake()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        //jc = new AndroidJavaClass("com.boke.util.SocialUtil");
        //socialObject = jc.CallStatic<AndroidJavaObject>("getInstance");

        var mainClass = new AndroidJavaClass("com.BokeCity.activity.UnitySdk");
        mainObject = mainClass.CallStatic<AndroidJavaObject>("getInstance");
        fileClass = new AndroidJavaClass("com.BokeCity.util.FileUtil");
        ossClass = new AndroidJavaClass("com.BokeCity.util.AliOSSUtil");


        ttsObject = new AndroidJavaClass("com.BokeCity.tts.Synthesize").CallStatic<AndroidJavaObject>("getInstance");
        trainObject = new AndroidJavaClass("com.BokeCity.mockingbird.MockTrain").CallStatic<AndroidJavaObject>("getInstance");
        recordObject = new AndroidJavaClass("com.BokeCity.audio.RecordMaker").CallStatic<AndroidJavaObject>("getInstance");
#endif

    }

    #region 社交部分
    private Action<string> onFBLoginSucced;
    private Action<string> onFBLoginFaild;
    private Action<string> onFBShareBack;
    //登录成功回调
    public void OnLoginSuccess(string token)
    {
        onFBLoginSucced?.Invoke(token);
    }
    //登录失败回调
    public void OnLoginFail(string err)
    {
        onFBLoginFaild?.Invoke(err);
    }
    //分享成功回调
    public void OnShareBack(string state)
    {
        onFBShareBack?.Invoke(state);
    }

    public void FBLogin(Action<string> onFBLoginSucced, Action<string> onFBLoginFaild)
    {
        this.onFBLoginSucced = onFBLoginSucced;
        this.onFBLoginFaild = onFBLoginFaild;
#if UNITY_EDITOR
        OnLoginSuccess("testToken");
#elif UNITY_ANDROID
        //socialObject.Call("faceBookLogin");
#elif UNITY_IPHONE

#endif

    }

    public void FBShareImage(string imgPath, Action<string> onFBShareSucced)
    {
        this.onFBShareBack = onFBShareSucced;
#if UNITY_EDITOR
        onFBShareBack("test");
#elif UNITY_ANDROID
        //socialObject.Call("fbShareImg", imgPath);
#elif UNITY_IPHONE

#endif
    }

    public void FBShareLink(string linkUrl)
    {
#if UNITY_EDITOR
        OnShareBack("test");
#elif UNITY_ANDROID
        //socialObject.Call("fbShareLink", linkUrl);
#elif UNITY_IPHONE

#endif
    }

    public void WhatsAppShare(string imgPath, string content)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        //socialObject.Call("whatAppShare", imgPath, content);
#elif UNITY_IPHONE

#endif
    }
#endregion

    #region 支付部分
    //支付完成回调
    private void OnPurchaseOk(string msg)
    {
        GameDebug.LogGame("购买回调");
        XLua.LuaManager.getInstance().CallLuaFunction("purchaseCheck", msg);
    }

    public void ConnectBilling(string json)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        mainObject.Call("ConnectBilling", json);
#else

#endif
    }

    public void QuerySkuDetailsAsync(string skuArr)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        mainObject.Call("QuerySkuDetailsAsync", skuArr);
#else

#endif
    }

    public void Pay(string sku)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        mainObject.Call("LaunchPurchaseFlow", sku);
#else

#endif
    }

    public void ConsumePurchase(string sku)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        mainObject.Call("ConsumePurchase", sku);
#else

#endif
    }

    #endregion

    #region 文件部分
    public void initFileUtil()
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        fileClass.CallStatic("Init",jc.GetStatic<AndroidJavaObject>("currentActivity"));
#endif
    }
    public void copyFileFromAssets(string path, string desPath)
    {
        fileClass.CallStatic("copyFilesFromAssets", path, desPath);
    }
    public byte[] getBytes(string path)
    {
        return fileClass.CallStatic<byte[]>("getBytes", path);
    }
    public string getString(string path)
    {
        return fileClass.CallStatic<string>("getString", path);
    }
    public bool isFileExists(string path)
    {
        return fileClass.CallStatic<bool>("isFileExists", path);
    }

    public void InitOss(string endPoint,string bucketName,string accessId,string accessSecret)
    {
        if (ossClass != null)
        {
            ossClass.CallStatic("InitOss", endPoint, bucketName, accessId, accessSecret);
        }
    }
    /// <summary>
    /// 文件上传到OSS
    /// </summary>
    /// <param name="filePath">可以是相对路径，也可以是绝对路径</param>
    /// <param name="category">取值类型portrait,family,goods,environment,survey,audio</param>
    /// <param name="userId">玩家的账户id</param>
    /// <param name="zoneId">玩家的区域id，0为测试</param>
    /// <returns></returns>
    public string UploadAsset(string filePath, string category, int userId,int zoneId)
    {
        if (ossClass != null)
        {
            if (!filePath.Contains("/")) //相对路径
            {
                string cacheDir = category == "audio" ? "/audioCache/" : "/imageCache/";
                filePath = Application.persistentDataPath + cacheDir + filePath;
            }
            return ossClass.CallStatic<string>("UploadAsset", filePath, category, userId.ToString(), zoneId);
        }
        return "";
    }

    #endregion

    #region 其他部分
    //打印回调
    private void UnityPrint(string msg)
    {
        GameDebug.Log(msg);
    }
    public int GetAgent()
    {
#if UNITY_EDITOR
        return 0;
#elif UNITY_ANDROID
        string content = mainObject.Call<string>("GetAgent");
        if (string.IsNullOrEmpty(content))
            return 0;
        else
            return int.Parse(content);
#else
        return 0;
#endif
    }
    public string GetChannel()
    {
        return "";
    }

    public void InstallApp(string path)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        mainObject.Call("InstallApp", path);
#else
        Application.OpenURL(url);
#endif
    }

    public void OpenWebView(string url)
    {
#if UNITY_EDITOR
        Application.OpenURL(url);
#elif UNITY_ANDROID
        mainObject.Call("OpenWebView", url);
#else
        Application.OpenURL(url);
#endif
    }

    //获取键盘高度
    public int GetKeyboardHeight()
    {
#if UNITY_EDITOR
        return 0;
#elif UNITY_ANDROID
        return mainObject.Call<int>("GetKeyboardHeight");
#else
        return 0;
#endif

    }

    public void SetClipboard(string msg)
    {
        GUIUtility.systemCopyBuffer = msg;
    }

    public string GetClipboard()
    {
        return GUIUtility.systemCopyBuffer;
    }

    public void SetAlarmMsg(string startTime, string text)
    {
        if (mainObject != null)
        {
            mainObject.Call("setAlarmMsg", startTime, text);
        }
    }

    public void LaunchAlarm()
    {
        if (mainObject != null)
        {
            mainObject.Call("launchAlarm");
        }
    }
    #endregion

    #region 拍照部分
    private Action<string> onPhotoBack;
    //[DllImport("__Internal")]
    //public static extern void GetPhoto(int code, bool needCrop, int cropX, int cropY);
    //拍照回调
    private void OnPhotoCallback(string msg)
    {
        if (onPhotoBack != null)
        {
            onPhotoBack?.Invoke(msg);
        }
    }

    public void OpenPhotoView(Action<string> callBack, int code, int cropX, int cropY)
    {
        this.onPhotoBack = callBack;
#if UNITY_EDITOR
        OnPhotoCallback("");
#elif UNITY_ANDROID
        mainObject.Call("OpenPhotoView", code, cropX, cropY);
#endif
    }

    #endregion

    #region 语音部分
    private Action onSpeakStart;
    private Action onSpeakFinish;
    private Action onSpeakError;
    private void OnSpeakCallBack(string param)
    {
        if (param == "0")
            onSpeakStart?.Invoke();
        else if (param == "1")
            onSpeakFinish?.Invoke();
        else
            onSpeakError?.Invoke();
    }
    
    public void TTSInit(LuaTable table, string mode, string name)
    {
        if (ttsObject != null)
        {
            table.Get("onSpeakStart", out onSpeakStart);
            table.Get("onSpeakFinish", out onSpeakFinish);
            table.Get("onSpeakError", out onSpeakError);
            ttsObject.Call("InitTts", mode, name);
        }
    }
    //"M"为男,"F"为女,默认为女
    public void TTSSwitchModel(string mode)
    {
        if (ttsObject != null)
        {
            ttsObject.Call("SwitchModel", mode);
        }
    }
    //切换用户名字
    public void TTSSwitchUserName(string name)
    {
        if (ttsObject != null)
        {
            ttsObject.Call("SwitchUserName", name);
        }
    }
 
    public void TTSSpeak(string content)
    {
        if (ttsObject != null)
        {
            ttsObject.Call("Speak", content);
        }
    }
    public void TTSSpeak(string content, string id_tag, bool includeName)
    {
        if (ttsObject != null)
        {
            ttsObject.Call("Speak", content, id_tag, includeName);
        }
    }
    public void TTSStop()
    {
        if (ttsObject != null)
        {
            ttsObject.Call("Stop");
        }
    }
    public void TTSPause()
    {
        if (ttsObject != null)
        {
            ttsObject.Call("Pause");
        }
    }
    public void TTSResume()
    {
        if (ttsObject != null)
        {
            ttsObject.Call("Resume");
        }
    }
    #endregion

    #region 语音训练部分
    private Action<string> onTrainBack;
    private Action<string> onRecogBack;
    private Action<string> OnRecogError;
    private void OnTrainCallBack(string state)
    {
        onTrainBack?.Invoke(state);
    }
    private void OnRecogCallBack(string index)
    {
        onRecogBack?.Invoke(index);
    }
    private void OnRecogCallError(string msg)
    {
        OnRecogError?.Invoke(msg);
    }
    public void GetTrainState(LuaTable table, string userId)
    {
        if (trainObject != null)
        {
            table.Get("onTrainBack", out onTrainBack);
            table.Get("onRecogBack", out onRecogBack);
            table.Get("OnRecogError", out OnRecogError);
            trainObject.Call("GetTrainState", userId);
        }
    }
    public void InitTrain(string json, float minLevenshtein=0.8f)
    {
        if (trainObject != null)
        {
            trainObject.Call("InitTrain", json, minLevenshtein);
        }
    }
    public void RecogStart(int index)
    {
        if (trainObject != null)
        {
            trainObject.Call("RecogStart", index);
        }
    }
    public void RecogStop()
    {
        if (trainObject != null)
        {
            trainObject.Call("RecogStop");
        }
    }
    public void RecogAudioPlay(int index)
    {
        if (trainObject != null)
        {
            trainObject.Call("RecogAudioPlay", index);
        }
    }
    public void StartTrain()
    {
        if (trainObject != null)
        {
             trainObject.Call("StartTrain");
        }
    }
    public void MockAudioPlay(string content)
    {
        if (trainObject != null)
        {
            trainObject.Call("SynthAudioPlay", content);
        }
    }
    public void MockAudioStop()
    {
        if (trainObject != null)
        {
            trainObject.Call("AudioStop");
        }
    }
    public void MockAudioPause()
    {
        if (trainObject != null)
        {
            trainObject.Call("AudioPause");
        }
    }
    public void MockAudioResume()
    {
        if (trainObject != null)
        {
            trainObject.Call("AudioResume");
        }
    }
    #endregion

    #region 录音部分
    //权限回调
    private void OnPermissionCallBack(string param)
    {
        LuaManager.getInstance().CallLuaFunction("App.OnPermission", param);
    }
    //音频后缀
    public void SetAudioSuffix(string suffix = ".mp3")
    {
        if (recordObject != null)
        {
            recordObject.Call("SetSuffix", suffix);
        }
    }
    //录音初始化
    public void RecordInit()
    {
        if (recordObject != null)
        {
            recordObject.Call("Init");
        }
    }
    //开始录音
    public void RecordStart()
    {
        if (recordObject != null)
        {
            recordObject.Call("StartRecording");
        }
    }
    //结束录音，返回音频路径path和毫秒级时长time
    public string RecordStop()
    {
        string json = "";
        if (recordObject != null)
        {
            json = recordObject.Call<string>("StopRecording");
        }
        Debug.Log("RecordData: " + json);
        return json;
    }
    //暂停录音
    public void RecordPause()
    {
        if (recordObject != null)
        {
            recordObject.Call("PauseRecording");
        }
    }
    //恢复录音
    public void RecordResume()
    {
        if (recordObject != null)
        {
            recordObject.Call("ResumeRecording");
        }
    }
    //从第x毫秒开始播放
    public void RecordPlayingStart(int mSec)
    {
        if (recordObject != null)
        {
            recordObject.Call("StartPlaying", mSec);
        }
    }
    //停止播放语音，返回结束时毫秒时间
    public int RecordPlayingStop()
    {
        int curMSec = 0;
        if (recordObject != null)
        {
            curMSec = recordObject.Call<int>("StopPlaying");
        }
        return curMSec;
    }
    #endregion


    private void OnDestroy()
    {
        onFBLoginSucced = null;
        onFBLoginFaild = null;
        onFBShareBack = null;
        onPhotoBack = null;
        onSpeakStart = null;
        onSpeakFinish = null;
        onSpeakError = null;

        onTrainBack = null;
        onRecogBack = null;
        OnRecogError = null;
    }
}


