using System;
using UnityEngine;
using GameUtils;

public enum BuildPlatform
{
    Android,
    iOS,
}

public enum ResPath
{
    Art,
    StreamingAssets,
    PersistentData
}

public class Setting
{
    //资源加载路径
    public ResPath resPath { get; set; }
    //打包信息
    public BuildPlatform buildTarget { get; set; }
    public string buildPath { get; set; }
    public bool isABHash { get; set; }
    public bool delManifest { get; set; }
    public bool createAssets { get; set; }
    //工程运行信息
    public bool isLuaZip { get; set; }
    //打包签名
    public string keystore { get; set; }
    public string keypass { get; set; }
    public string keyaliname { get; set; }
    public string keyalipass { get; set; }

    public static string settingPath()
    {       
        return Application.dataPath.Replace("Assets", "editor_config/") + "setting.json";        
    }

    public static string versionPath()
    {
        return Application.dataPath.Replace("Assets", "editor_config/") + AssetUpdater.Config_Name;
       
    }

    static Setting m_setting;
    public static Setting Get()
    {
        if(m_setting == null)
        {                  
#if UNITY_EDITOR          
            m_setting = FileUtils.loadObjectFromJsonFile<Setting>(settingPath());
#else
            m_setting = new Setting();
            m_setting.isLuaZip = true;
            m_setting.resPath = ResPath.PersistentData;
#endif
        }
        return m_setting;
    }

}
