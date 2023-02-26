using System;
using UnityEngine;
using GameUtils;
using System.Collections.Generic;

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

    public List<string> outPaths { get; set; }//出包地址
    public List<string> assetUrls { get; set; }//资源地址
    public List<string> apiUrls { get; set; } //api地址
    public bool createAllZone { get; set; } //全地址打包
    //打包配置
    //public bool isABHash { get; set; }
    //public bool delManifest { get; set; }
    public bool chgAppName { get; set; }
    public bool createAssets { get; set; }
    public bool genPackage { get; set; }

    //打包设置
    public string appName { get; set; }
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
            m_setting.resPath = ResPath.PersistentData;
#endif
        }
        return m_setting;
    }

}