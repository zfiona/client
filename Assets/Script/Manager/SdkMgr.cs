using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using System.Text;
using XLua;
//using com.adjust.sdk;


public class SdkMgr : MonoBehaviour
{
    public static SdkMgr Instance;
    

    void Awake()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        releaseLuaChatCallback();
    }
    #region 语音部分
    private LuaFunction _luaChatCallback;
    private LuaFunction LuaChatCallback
    {
        get
        {
            if (_luaChatCallback == null)
            {
                _luaChatCallback = LuaManager.getInstance().GetLuaFunction("ChatUtil.ChatCallback");
            }
            return _luaChatCallback;
        }
    }

    private void releaseLuaChatCallback()
    {
        if (_luaChatCallback != null)
        {
            _luaChatCallback.Dispose();
        }
        _luaChatCallback = null;
    }

    public void ChatCallback(string msg)
    {
        GameDebug.LogGreen("ChatCallback:" + msg);
        LuaChatCallback.Action(msg);
    }
    
    #endregion

    #region 社交部分
    private Action<string> onFBLoginSucced;
    private Action<string> onFBLoginFaild;
    private Action<string> onFBShareSucced;
    //登录成功回调
    public void OnLoginSuccess(string token)
    {
        onFBLoginSucced?.Invoke(token);
    }
    //登录失败回调
    public void OnLoginFail(string failType)
    {
        onFBLoginFaild?.Invoke(failType);
    }
    //分享成功回调
    public void OnShareSuccess(string msg)
    {
        onFBShareSucced?.Invoke("facebook");
    }

    public void FBLogin(Action<string> onFBLoginSucced, Action<string> onFBLoginFaild)
    {
        this.onFBLoginSucced = onFBLoginSucced;
        this.onFBLoginFaild = onFBLoginFaild;
#if UNITY_EDITOR
        OnLoginSuccess("testToken");
#elif UNITY_ANDROID
        AndroidUtil.SocialUtil("faceBookLogin");
#elif UNITY_IPHONE

#endif

    }

    public void FBShareImage(string imgPath, Action<string> onFBShareSucced)
    {
        this.onFBShareSucced = onFBShareSucced;
#if UNITY_EDITOR
        OnShareSuccess("test");
#elif UNITY_ANDROID
        AndroidUtil.SocialUtil("fbShareImg", imgPath);
#elif UNITY_IPHONE

#endif

    }

    public void FBShareLink(string linkUrl)
    {
#if UNITY_EDITOR
        OnShareSuccess("test");
#elif UNITY_ANDROID
        AndroidUtil.SocialUtil("fbShareLink", linkUrl);
#elif UNITY_IPHONE

#endif
    }

    public void WhatsAppShare(string imgPath, string content)
    {
#if UNITY_EDITOR
        
#elif UNITY_ANDROID
        AndroidUtil.SocialUtil("whatAppShare", imgPath, content);
#elif UNITY_IPHONE

#endif
    }
    #endregion

    #region 拍照部分
    private Action<string> OpenPhotoDelegate;
    //拍照回调
    private void OpenPhotoCallback(string msg)
    {
        if (OpenPhotoDelegate != null)
        {
            OpenPhotoDelegate(msg);
            OpenPhotoDelegate = null;
        }
    }
    private enum PicMsgCode
    {
        SHOW_SELECT_WINDOW = 1,
        CHOOSE_PICTURE = 2,
        TAKE_PICTURE = 3
    }
    
    public void ShowSelectWindow(Action<string> callBack, bool need_crop = true, int cropX = 200, int cropY = 200)
    {
        this.OpenPhotoDelegate = callBack;
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AndroidUtil.Call("OpenPhotoView",(int)PicMsgCode.SHOW_SELECT_WINDOW, need_crop, cropX, cropY);
#elif UNITY_IOS
        IosUtil.CallOpenPhoto( (int)PicMsgCode.SHOW_SELECT_WINDOW, need_crop, cropX, cropY);
#endif
    }

    public void ChoosePicture(Action<string> callBack, bool need_crop = true, int cropX = 200, int cropY = 200)
    {
        this.OpenPhotoDelegate = callBack;
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AndroidUtil.Call("OpenPhotoView",(int)PicMsgCode.CHOOSE_PICTURE, need_crop, cropX, cropY);
#elif UNITY_IOS
        IosUtil.CallOpenPhoto((int)PicMsgCode.CHOOSE_PICTURE, need_crop, cropX, cropY);
#endif
    }

    public void OpenCamera(Action<string> callBack, bool need_crop = true, int cropX = 200, int cropY = 200)
    {
        this.OpenPhotoDelegate = callBack;
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AndroidUtil.Call("OpenPhotoView",(int)PicMsgCode.TAKE_PICTURE, need_crop, cropX, cropY);
#elif UNITY_IOS
        IosUtil.CallOpenPhoto( (int)PicMsgCode.TAKE_PICTURE, need_crop, cropX, cropY);
#endif
    }
    #endregion

    #region 支付部分
    //支付完成回调
    private void OnPurchaseOk(string msg)
    {
        GameDebug.LogGame("购买回调");
        LuaManager.getInstance().CallLuaFunction("purchaseCheck", msg);
    }

    public void ConnectBilling(string json)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AndroidUtil.Call("ConnectBilling", json);
#else

#endif
    }

    public void QuerySkuDetailsAsync(string skuArr)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AndroidUtil.Call("QuerySkuDetailsAsync", skuArr);
#else

#endif
    }

    public void Pay(string sku)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AndroidUtil.Call("LaunchPurchaseFlow", sku);
#else

#endif
    }

    public void ConsumePurchase(string sku)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AndroidUtil.Call("ConsumePurchase", sku);
#else

#endif
    }

    #endregion

    #region 其他部分
    //打印回调
    private void UnityPrint(string msg)
    {
        GameDebug.Log(msg);
    }
    public string GetApkData()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_ANDROID
        return AndroidUtil.Call<string>("GetApkData");
#else
        return "";
#endif

    }

    public void InstallApp(string path)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AndroidUtil.Call("InstallApk", path);
#else
        Application.OpenURL(url);
#endif
    }

    public void OpenWebView(string url)
    {
#if UNITY_EDITOR

#elif UNITY_ANDROID
        AndroidUtil.Call("OpenWebView", url);
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
        return AndroidUtil.Call<int>("GetKeyboardHeight");
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
    #endregion


  

}


