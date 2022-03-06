using System;
using UnityEngine;
using GameUtils;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    void Awake()
    {
        Instance = this;
        QualitySettings.SetQualityLevel(3);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 30;
        DontDestroyOnLoad(gameObject);
        CheckDebug();
        CheckRes();
    }
    

    void Start()
    {
        GameDebug.LogPurple(Tool.GetDeviceName() + " ：" + Tool.GetDeviceUID());
        UIManager.ShowPage(new UILoadGame(),null);
    }
    
    void CheckDebug()
    {
        if (!GameObject.Find("Reporter"))
        {
            GameDebug.logEnable = false;
        }
        GameDebug.Log(Application.persistentDataPath);
    }

    void CheckRes()
    {
        string[] versions = Application.version.Split('.');
        if (versions[versions.Length-1] == "0")
        {
            GameDebug.LogGame("整包测试版本");
            if(Setting.Get().resPath == ResPath.PersistentData)
                Setting.Get().resPath = ResPath.StreamingAssets;
        }
        else if (!FileUtils.ins.IsConfigExist())
        {
            GameDebug.LogGame("首次启动拷贝配置文件");
            string src = FileUtils.ins.getStreamingPath(false);
            string des = FileUtils.ins.getPresistentPath(false);
            FileUtils.ins.copyDir(src, des);
        }
    }

}
