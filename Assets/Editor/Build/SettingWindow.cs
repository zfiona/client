using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using GameUtils;

public class SettingWindow : EditorWindow
{
    private Setting setting;
    private Config config;

    private static GUIContent
        insertContent = new GUIContent("+", "添加变量"),
        browse = new GUIContent("浏览", "浏览文件夹"),
        save = new GUIContent("保存", "保存设置项"),
        deleteContent = new GUIContent("-", "删除变量");
    private static GUILayoutOption buttonWidth = GUILayout.MaxWidth(20f);
    private GUIStyle titleStyle;
    private List<string> assetUrls = new List<string>();
    private List<string> apiUrls = new List<string>();
    private List<FileItem> assets = new List<FileItem>();
    
    private void Awake()
    {
        titleStyle = new GUIStyle();
        titleStyle.fontSize = 20;
        titleStyle.normal.textColor = Color.yellow;

        readSetting();
    }
    private void saveSetting()
    {
        //setting
        if (setting.resPath == ResPath.Art)
            setting.isLuaZip = false;
        string[] versions = config.version.Split('.');
        if (setting.resPath == ResPath.StreamingAssets)
        {
            versions[versions.Length - 1] = "0";
            config.version = string.Join(".", versions);
        }
        if (setting.resPath != ResPath.StreamingAssets && versions[versions.Length - 1] == "0")
        {
            versions[versions.Length - 1] = "1";
            config.version = string.Join(".", versions);
        }
        FileUtils.saveObjectToJsonFile(setting, Setting.settingPath(), false);
        //version
        config.assetUrls = assetUrls;
        config.apiUrls = apiUrls;
        FileUtils.saveObjectToJsonFile(config, Setting.versionPath(), false);
        FileUtils.saveObjectToJsonFile(config, outPath + "/" + AssetUpdater.Config_Name, setting.resPath > ResPath.Art);

        //打包设置
        SavePlayerSetting();
        AssetDatabase.Refresh();
        Debug.Log("设置保存成功！！！");
    }
    private void readSetting()
    {
        setting = FileUtils.loadObjectFromJsonFile<Setting>(Setting.settingPath());
        config = FileUtils.loadObjectFromJsonFile<Config>(Setting.versionPath());
        if (config.assetUrls != null)
            assetUrls = config.assetUrls;
        if (config.apiUrls != null)
            apiUrls = config.apiUrls;
    }
    private void OnGUI()
    {
        //输入框控件      
        GUILayout.Space(5);
        setting.resPath = (ResPath)EditorGUILayout.EnumPopup("资源路径:", setting.resPath);
        GUILayout.Space(5);
        setting.buildTarget = (BuildPlatform)EditorGUILayout.EnumPopup("打包平台:", setting.buildTarget);
        if (setting.resPath == ResPath.PersistentData)
        {
            GUILayout.Space(5);
            setting.buildPath = EditorGUILayout.TextField("打包路径:", setting.buildPath);
        }
        if (setting.resPath != ResPath.Art)
        {
            GUILayout.Space(5);
            setting.isLuaZip = EditorGUILayout.Toggle("Lua读资源路径:", setting.isLuaZip);
            GUILayout.Space(5);
            config.appUrl = EditorGUILayout.TextField("下载包地址:", config.appUrl);
            GUILayout.Space(5);
            config.zoneId = EditorGUILayout.IntField("区服ID:", config.zoneId);
            GUILayout.Space(5);
            config.appName = EditorGUILayout.TextField("包名:", config.appName);
        }
        GUILayout.Space(5);
        config.version = EditorGUILayout.TextField("版本号:", config.version);
        GUILayout.Space(5);
        config.kefuUrl = EditorGUILayout.TextField("客服地址:", config.kefuUrl);

        DrawLines();
        DrawWebs();
        //打包设置
        GUILayout.Space(5);
        EditorGUILayout.LabelField("打包设置", titleStyle);
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("keystore", GUILayout.Width(146), GUILayout.Height(18f));
        GUILayout.Label(setting.keystore, "HelpBox", GUILayout.Height(18f));
        if (GUILayout.Button(browse))
        {
            string path = EditorUtility.OpenFilePanel("keystore", setting.keystore, "keystore");
            setting.keystore = path.Remove(0, Environment.CurrentDirectory.Length + 1);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        setting.keypass = EditorGUILayout.TextField("keypass:", setting.keypass);
        GUILayout.Space(5);
        setting.keyaliname = EditorGUILayout.TextField("keyaliname:", setting.keyaliname);
        GUILayout.Space(5);
        setting.keyalipass = EditorGUILayout.TextField("keyalipass:", setting.keyalipass);

        EditorTools.DrawSeparator();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        setting.createAssets = EditorGUILayout.Toggle("生成资源:", setting.createAssets);
        if (setting.createAssets)
        {
            setting.delManifest = EditorGUILayout.Toggle("删除manifest", setting.delManifest);
            if(setting.delManifest)
                setting.isABHash = EditorGUILayout.Toggle("ABhash化", setting.isABHash);
        }
            
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        if (GUILayout.Button(save, EditorStyles.miniButtonLeft, GUILayout.MinWidth(180f)))
        {
            Close();

            SetOutPath();
            if (setting.createAssets)
                CreateAssetList();
            saveSetting();
        }
    }

    private void DrawLines()
    {
        EditorTools.DrawSeparator();
        GUILayout.Space(5);
        EditorGUILayout.LabelField("Asset下载地址", titleStyle);
        GUILayout.Space(5);
        for (int i = 0; i < assetUrls.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (i % 2 == 0) GUI.backgroundColor = Color.cyan;
            else GUI.backgroundColor = Color.magenta;
            assetUrls[i] = EditorGUILayout.TextField(string.Format("网络{0}:", i + 1), assetUrls[i]);
            if (GUILayout.Button(deleteContent, EditorStyles.miniButtonLeft, buttonWidth))
            {
                assetUrls.Remove(assetUrls[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.ExpandWidth(true));
        if (GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, buttonWidth))
        {
            assetUrls.Add("");
        }
        EditorGUILayout.EndHorizontal();
        EditorTools.DrawSeparator();
        GUI.backgroundColor = Color.white;
    }

    private void DrawWebs()
    {
        //EditorTools.DrawSeparator();
        GUILayout.Space(5);
        EditorGUILayout.LabelField("Web请求地址", titleStyle);
        GUILayout.Space(5);
        for (int i = 0; i < apiUrls.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (i % 2 == 0) GUI.backgroundColor = Color.cyan;
            else GUI.backgroundColor = Color.magenta;
            apiUrls[i] = EditorGUILayout.TextField(string.Format("网络{0}:", i + 1), apiUrls[i]);
            if (GUILayout.Button(deleteContent, EditorStyles.miniButtonLeft, buttonWidth))
            {
                apiUrls.Remove(apiUrls[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.ExpandWidth(true));
        if (GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, buttonWidth))
        {
            apiUrls.Add("");
        }
        EditorGUILayout.EndHorizontal();
        EditorTools.DrawSeparator();
        GUI.backgroundColor = Color.white;
    }

    private string outPath;
    private void SetOutPath()
    {
        outPath = FileUtils.ins.getStreamingPath(false);
        if (setting.resPath == ResPath.PersistentData && !string.IsNullOrEmpty(setting.buildPath))
            outPath = setting.buildPath;
    }

    private void CreateAssetList()
    {
        BuildHelper.MakeLuaFiles();
        BuildHelper.MakeAssetBundleMap();
        BuildHelper.MakeAssetBundles(setting.delManifest, (BuildTarget)Enum.Parse(typeof(BuildTarget), setting.buildTarget.ToString()), outPath);
        MakeConfigs();
    }

    void MakeConfigs()
    {
        string[] files = Directory.GetFiles(outPath);
        foreach (string file in files)
        {
            if (file.EndsWith(".meta") || file.EndsWith(".manifest") || file.EndsWith(AssetUpdater.Config_Name))
                continue;
            string file2 = file.Replace("\\", "/");

            FileItem item = new FileItem();
            item.path = file2.Split('/')[file2.Split('/').Length - 1];
            item.md5 = file2.Split('_')[file2.Split('_').Length - 1];
            item.length = (int)FileUtils.ins.GetSize(file2);
            assets.Add(item);
            if (setting.isABHash)
                FileUtils.ins.Move(file, outPath + "/" + item.md5);
        }
        config.assets = assets;
    }

    private void SavePlayerSetting()
    {
        PlayerSettings.Android.keystoreName = setting.keystore;
        PlayerSettings.Android.keystorePass = setting.keypass;
        PlayerSettings.Android.keyaliasName = setting.keyaliname;
        PlayerSettings.Android.keyaliasPass = setting.keyalipass;
        PlayerSettings.Android.bundleVersionCode = int.Parse(config.version.Split('.')[0]);
        PlayerSettings.bundleVersion = config.version;

        BuildTargetGroup targetGroup = BuildTargetGroup.Android;
#if UNITY_IOS
        targetGroup = BuildTargetGroup.iOS;
#endif
        string ori = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        List<string> defineSymbols = new List<string>(ori.Split(';'));
        string artType = "LoadFromLocal";
        if (setting.resPath == (int)ResPath.Art && !defineSymbols.Contains(artType))
            defineSymbols.Add(artType);
        else if ((setting.resPath != (int)ResPath.Art && defineSymbols.Contains(artType)))
            defineSymbols.Remove(artType);

        string hashType = "AssetBundleHash";
        if (setting.isABHash && !defineSymbols.Contains(hashType))
            defineSymbols.Add(hashType);
        else if (!setting.isABHash && defineSymbols.Contains(hashType))
            defineSymbols.Remove(hashType);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defineSymbols.ToArray()));
    }

    private void OnFocus()
    {
        //readSetting();
    }
    private void OnLostFocus()
    {
        //saveSetting();
    }
}
