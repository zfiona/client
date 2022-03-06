using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameUtils;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using EasyAlphabetArabic;

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

    }
    public override void Awake(GameObject go)
    {
        txt_version = transform.Find("txt_version").GetComponent<Text>();
        slider_progress = transform.Find("rect_slider/slider_progress").GetComponent<Slider>();
        txt_info = transform.Find("rect_slider/txt_info").GetComponent<Text>();
        txt_progress = transform.Find("rect_slider/txt_progress").GetComponent<Text>();

        tips = transform.Find("tips").gameObject;
        txt_lab = transform.Find("tips/txt_lab").GetComponent<Text>();
        btnOk = transform.Find("tips/btn_Panel/btn_ok").GetComponent<Button>();
        btnCancel = transform.Find("tips/btn_Panel/btn_cancel").GetComponent<Button>();
    }

    public override void Refresh(object data)
    {
        if (AppConst.config != null)
        {
            GameDebug.Log("version:" + AppConst.config.version);
            txt_version.text = "version: " + AppConst.config.version;
        }
        else
        {
            txt_version.text = "version: Unity Editor";
        }
          
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
            for(int i = 0; i < 40; i++)
            {
                yield return null;
                curLength += 2.5f;
                OnCallBack(ProgressState.Virtual, curLength);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }
        gameObject.DestroySelf();
        XLua.LuaManager.getInstance().StartMain();
    }

    private void ShowDialog(string tip,UnityAction onOk, UnityAction onCancel)
    {
        tips.gameObject.SetActive(true);
        txt_lab.text = tip;
        btnOk.onClick.AddListener(onOk);
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

    private float totalSize = 0;
    private string strTotal = "";
    private void OnCallBack(ProgressState state, float p)
    {
        switch (state)
        {
            case ProgressState.Virtual:
                slider_progress.value = p / totalSize;
                txt_info.text = string.Format("Downloading:  {0:f1}% ", p / totalSize * 100);
                txt_progress.text = "";
                txt_info.text = "Resources Loading...";
                break;
            case ProgressState.Checking:
                txt_info.text = "Version Checking...";
                break;
            case ProgressState.Checked:
                txt_version.text = "version " + AppConst.config.version;
                totalSize = p;
                strTotal = SizeFormat(p);
                GameDebug.Log("总大小: " + strTotal);
                break;
            case ProgressState.CheckError:
                ShowDialog("Checking error,please try again later!",
                   () => { mgr.StartDownload(); tips.gameObject.SetActive(false); },
                   () => { Application.Quit(); }
                );
                break;
            case ProgressState.Updating:
                slider_progress.value = p / totalSize;
                txt_info.text = string.Format("Downloading :  {0:f1}% ", p / totalSize * 100);
                txt_progress.text = string.Format("{0} / {1}", SizeFormat(p), strTotal);
                break;
            case ProgressState.Updated:
                AssetBundleMgr.Instance.Init();
                StartCoroutine(UpToData(p == 0));
                break;
            case ProgressState.UpdateError:
                ShowDialog("Network error,please try again later!",
                   () => { mgr.CheckDownload(); tips.gameObject.SetActive(false); },
                   () => { Application.Quit(); }
                );
                break;
            case ProgressState.BigVersion:
                ShowDialog("New app found,Download now?",
                    () => { DownLoadApp(); tips.gameObject.SetActive(false); },
                    () => { Application.Quit(); }
                );
                break;
            case ProgressState.BigUpdating:
                slider_progress.value = p;
                txt_info.text = string.Format("Downloading :  {0:f1}% ", (p * 100).ToString("F2"));
                break;
            case ProgressState.BigUpdated:
                InstallApp();
                break;
            case ProgressState.BigError:
                ShowDialog("Network error,please try again later!",
                   () => { DownLoadApp(); tips.gameObject.SetActive(false); },
                   () => { Application.Quit(); }
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
#if UNITY_EDITOR || UNITY_IOS
        Application.OpenURL(AppConst.config.appUrl);
        Application.Quit();
#elif UNITY_ANDROID
        mgr.StartDownloadApk();
#endif
    }

    private void InstallApp()
    {
        string path = FileUtils.ins.getPresistentPath(true) + AssetUpdater.Config_Name;
        FileUtils.ins.removeFile(path);

        path = FileUtils.ins.getPresistentPath(true) + AssetUpdater.Apk_Name;
        SdkMgr.Instance.InstallApp(path);
    }
}
