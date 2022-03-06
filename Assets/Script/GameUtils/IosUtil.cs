#if UNITY_IPHONE
using System;
using System.Collections;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using LitJson;
using UnityEngine;


public class IosUtil
{


    /// <summary>
    /// 电量
    /// </summary>
    static public float Battery
    {
        get
        {
            return _GetBattery();
        }
    }
    /// <summary>
    /// 信号强度
    /// </summary>
    static public int NetStrength
    {
        get
        {
            int result = _GetNetStrength();
            if (result > 3) result = 3;
            return result;
        }
    }

    static string currentOrderId = "";
    static public void Pay(string notifyUrl, string productName, int platform, string orderId, float money, string productCode = "", string playerId = "")
    {
        string orderTime = "";
        currentOrderId = orderId;
        _Pay(platform, notifyUrl, orderId, orderTime, productName, productCode, money.ToString(), playerId);
    }


    //Appstore 购买回调
    static public void ApplePayResponse(string msg)
    {
        Debug.LogError("appstore购买回调:" + msg);
        string responseMessage = "";
        string responseData = "";
        try
        {
            JsonData jsonObjects = JsonMapper.ToObject(msg);
            responseMessage = jsonObjects["result"].ToString();
            if (responseMessage == "Success")
            {
                string productId = jsonObjects["productId"].ToString();
                responseData = jsonObjects["receipt-data"].ToString();
                currentOrderId = jsonObjects["orderID"].ToString();
                //   HNLogger.LogEditor("购买成功:" + productId);
                //   UIPay.instance.StartQueryOrderDetail(currentOrderId, productId, responseData);
            }
            else
            {
                // TextTip.Instance.Show(StrUtil.GetText("购买失败!"));
            }
        }
        catch
        {
            GameDebug.Log("This key is nothing : ");
        }
    }



    public static void getIPType(string oldIp, string serverPorts, out string newIp, out AddressFamily mIPType)
    {
        newIp = oldIp;
        mIPType = AddressFamily.InterNetwork;
        if (Application.platform != RuntimePlatform.IPhonePlayer)
            return;
        try
        {
            string mIPv6 = GetIPv6(oldIp, serverPorts);
            if (!string.IsNullOrEmpty(mIPv6))
            {
                string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
                if (m_StrTemp != null && m_StrTemp.Length >= 2)
                {
                    string IPType = m_StrTemp[1];
                    if (IPType == "ipv6")
                    {
                        newIp = m_StrTemp[0];
                        mIPType = AddressFamily.InterNetworkV6;
                    }
                }
            }
        }
        catch (Exception e)
        {
            GameDebug.LogError("ipv6 error....." + e.StackTrace);
        }
    }


    static public void CallOpenPhoto(int code, bool needCrop, int cropX, int cropY)
    {
        GetPhoto(code, needCrop, cropX, cropY);
    }



    public static string GetIPv6(string mHost, string mPort)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
		string mIPv6 = getIPv6(mHost, mPort);
		return mIPv6;
#else
        return mHost + "&&ipv4";
#endif
    }

    static public bool IsAppInstalled(int type)
    {
        return _GetInstall(type);
    }

    static public void SavePicture(string localPath)
    {
        _SavePhoto(localPath);
    }
    [DllImport("__Internal")]
    public static extern void _InitBaseConfig(string con);
    [DllImport("__Internal")]
    public static extern string getIPv6(string mHost, string mPort);
    [DllImport("__Internal")]
    public static extern void _Pay(int platform, string notifyUrl, string orderId, string orderTime, string productName, string productCode, string money, string playerId);
    [DllImport("__Internal")]
    public static extern void _Service(int type, string msg1, string msg2, string msg3);
    [DllImport("__Internal")]
    public static extern void CopyTextToClipboard(string text);
    [DllImport("__Internal")]
    public static extern string GetUDID();
    [DllImport("__Internal")]
    public static extern void GetPhoto(int code, bool needCrop, int cropX, int cropY);
    [DllImport("__Internal")]
    static extern string _GetInviteInfo();
    [DllImport("__Internal")]
    public static extern bool _GetInstall(int type);
    [DllImport("__Internal")]
    static extern int _GetNetStrength();
    [DllImport("__Internal")]
    static extern float _GetBattery();
    [DllImport("__Internal")]
    public static extern void _FileShare(string filePath, bool preView = false);
    [DllImport("__Internal")]
    private static extern void _SavePhoto(string readAddr);

}


#endif