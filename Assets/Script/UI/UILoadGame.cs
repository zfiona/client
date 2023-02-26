using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameUtils;
using System.Collections;
public class UILoadGame : Page
{
    //加载页面
    Text txt_version;
    Slider slider_progress;
    Text txt_info;
    Text txt_progress;
    //提示框
    GameObject tips;
    Button btnOk;
    Button btnCancel;
    Text txt_lab;
    AssetUpdater mgr;
    public UILoadGame() : base("ui_loading", UIType.Normal, UIAnim.None)
    {
        uiName = "ui_loading";
    }
    public override void Awake(GameObject go)
    {
        Transform root = transform.Find("root");
        txt_version = root.Find("txt_version").GetComponent<Text>();

        slider_progress = root.Find("slider_progress").GetComponent<Slider>();
        txt_info = root.Find("slider_progress/txt_info").GetComponent<Text>();
        txt_progress = root.Find("slider_progress/txt_progress").GetComponent<Text>();

        tips = root.Find("tips").gameObject;
        txt_lab = root.Find("tips/txt_lab").GetComponent<Text>();
        btnOk = root.Find("tips/btn_Panel/btn_ok").GetComponent<Button>();
        btnCancel = root.Find("tips/btn_Panel/btn_cancel").GetComponent<Button>();
        tips.gameObject.SetActive(false);
    }

    public override void Refresh(object data)
    {
        if (AppConst.config != null)     
            txt_version.text = AppConst.config.GetVersion();
        else
            txt_version.text = "version: Unity Editor";

        if (Setting.Get().resPath == ResPath.PersistentData)
        {
            mgr = new AssetUpdater();
            mgr.StartUpdate(OnCallBack);
        }
        else
        {
            StartCoroutine(UpToData(true));
        }
    }

    private IEnumerator UpToData(bool needVirtual)
    {
        if (needVirtual)
        {
            totalSize = 100;
            float curLength = 0;
            for (int i = 0; i < 25; i++)
            {
                yield return null;
                curLength += 4f;
                OnCallBack(ProgressState.Virtual, curLength);
            }
        }
        yield return new WaitForSeconds(0.1f);
        gameObject.DestroySelf();
        XLua.LuaManager.getInstance().StartMain();
    }


    private void ShowDialog(string tip,string okText,string cancelText, UnityAction onOk, UnityAction onCancel = null)
    {
        tips.gameObject.SetActive(true);
        txt_lab.text = tip;
        btnOk.GetComponentInChildren<Text>().text = okText;
        btnCancel.GetComponentInChildren<Text>().text = cancelText;
        btnOk.onClick.AddListener(() =>
        {
            tips.gameObject.SetActive(false);
            onOk?.Invoke();
        });

        if (onCancel == null)
        {
            btnCancel.gameObject.SetActive(false);
        }
        else
        {
            btnCancel.gameObject.SetActive(true);
            btnCancel.onClick.AddListener(onCancel);
        }
    }

    private void switchOfflineMode()
    {
        AppConst.offlineMode = true;
        tips.gameObject.SetActive(false);

        AssetBundleMgr.Instance.Init();
        StartCoroutine(UpToData(true));
    }

    private float totalSize = 0;
    private string strTotal = "";
    private void OnCallBack(ProgressState state, float p)
    {
        switch (state)
        {
            case ProgressState.Virtual:
                slider_progress.value = p / totalSize;
                txt_info.text = "资源加载中...";
                txt_progress.text = "";
                break;
            case ProgressState.Checking:
                txt_info.text = "版本检测...";
                break;
            case ProgressState.Checked:
                txt_version.text = AppConst.config.GetVersion();
                totalSize = p;
                strTotal = SizeFormat(p);
                GameDebug.Log("总大小: " + strTotal);
                break;
            case ProgressState.CheckError:
                if (mgr.IsLastCompleted())
                {
                    ShowDialog("网络异常\n请连接网络或离线运行", "重试", "离线模式",
                       () => { mgr.DownloadConfig(); },
                       () => { switchOfflineMode(); }
                    );
                }
                else
                {
                    ShowDialog("网络异常\n请检查网络后重试或退出", "重试", "退出",
                       () => { mgr.DownloadConfig(); },
                       () => { Application.Quit(); }
                    );
                }
                break;
            case ProgressState.Updating:
                slider_progress.value = p / totalSize;
                txt_info.text = string.Format("资源下载中:  {0} / {1} ", SizeFormat(p), strTotal);
                txt_progress.text = string.Format("{0:f1}%", p / totalSize * 100);
                break;
            case ProgressState.Updated:
                AssetBundleMgr.Instance.Init();
                StartCoroutine(UpToData(p == 0));
                break;
            case ProgressState.UpdateError:
                ShowDialog("网络异常\n请检查网络后重试", "重试", "",
                   () => { mgr.DownloadAssets(); }
                );
                break;
            case ProgressState.BigVersion:
                ShowDialog("游戏版本更新\n需要重新下载安装", "下载", "退出",
                    () => { DownLoadApp(); },
                    () => { Application.Quit(); }
                );
                break;
            case ProgressState.BigUpdating:
                slider_progress.value = p;
                txt_info.text = string.Format("APP下载中:  {0:f1}% ", p * 100);
                //txt_progress.text = string.Format("{0:f1}% ", p * 100);
                break;
            case ProgressState.BigUpdated:
                InstallApp();
                break;
            case ProgressState.BigError:
                ShowDialog("网络异常\n请检查网络后重新下载", "下载", "",
                   () => { DownLoadApp(); }
                );
                break;
            default:
                break;
        }
    }

    private string SizeFormat(float p)
    {
        string[] tag = new string[] { "B", "K", "M", "G" };
        int i = 0;
        float f = 0;
        while (p / 1024 > 1)
        {
            i++;
            f = p % 1024;
            p = p / 1024;
        }

        if (i == 0) return p.ToString("0.0") + tag[i];
        f = f / 1024f;
        return (p + f).ToString("0.0") + tag[i];
    }

    private void DownLoadApp()
    {
        //Application.OpenURL(AppConst.config.appUrl);
        //Application.Quit();

#if UNITY_EDITOR || UNITY_IOS
        Application.OpenURL(AppConst.config.GetApkPath());
        Application.Quit();
#elif UNITY_ANDROID
        mgr.StartDownloadApk();
#endif
    }

    private void InstallApp()
    {
        string path = FileUtils.ins.getPresistentPath(false);
        FileUtils.ins.removeDirectory(path);

        path = Application.persistentDataPath + "/" + AssetUpdater.Apk_Name;
        SdkMgr.Instance.InstallApp(path);
    }
}
