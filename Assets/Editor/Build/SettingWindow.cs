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
    private List<string> outPaths = new List<string>();
    private List<string> assetUrls = new List<string>();
    private List<string> apiUrls = new List<string>();
    private List<FileItem> assets = new List<FileItem>();
    private string[] logOptions = new string[] { "0", "1", "2" };
    private string[] zoneOptions;

    private int curZoneId;
    private string outPathRes;
    private string outPathApk;

    private void Awake()
    {
        titleStyle = new GUIStyle();
        titleStyle.fontSize = 20;
        titleStyle.normal.textColor = Color.yellow;

        readSetting();
    }

    private void readSetting()
    {
        setting = FileUtils.loadObjectFromJsonFile<Setting>(Setting.settingPath());
        config = FileUtils.loadObjectFromJsonFile<Config>(Setting.versionPath());
        if (setting.assetUrls != null)
            assetUrls = setting.assetUrls;
        if (setting.apiUrls != null)
            apiUrls = setting.apiUrls;
        if (setting.outPaths != null)
            outPaths = setting.outPaths;

        resetZoneOptions();
    }

    private void resetZoneOptions()
    {
        zoneOptions = new string[setting.outPaths.Count];
        for (int i = 0; i < setting.outPaths.Count; i++)
            zoneOptions[i] = i.ToString();
    }

    private void OnGUI()
    {
        //输入框控件
        GUILayout.Space(5);
        setting.resPath = (ResPath)EditorGUILayout.EnumPopup("资源路径:", setting.resPath);
        GUILayout.Space(5);
        setting.buildTarget = (BuildPlatform)EditorGUILayout.EnumPopup("打包平台:", setting.buildTarget);
        GUILayout.Space(5);

        if (setting.resPath != ResPath.Art)
        {
            GUILayout.BeginHorizontal();
            config.logLevel = EditorGUILayout.Popup("日志级别:", config.logLevel, logOptions);
            config.version = EditorGUILayout.TextField("版本号:", config.version, GUILayout.Width(246));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            config.zoneId = EditorGUILayout.Popup("区服ID:", config.zoneId, zoneOptions);
            if (setting.resPath == ResPath.PersistentData)
                setting.createAllZone = EditorGUILayout.Toggle("全服打包:", setting.createAllZone, GUILayout.Width(246));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorTools.DrawSeparator();
            DrawPlayerSetting();
        }
        EditorTools.DrawSeparator();
        DrawOutput();
        EditorTools.DrawSeparator();
        DrawDownloads();
        EditorTools.DrawSeparator();
        DrawWebapis();
        EditorTools.DrawSeparator();
        //打包配置
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        if (setting.resPath == ResPath.Art)
            setting.createAssets = EditorGUILayout.Toggle("生成资源:", false);
        else
            setting.createAssets = EditorGUILayout.Toggle("生成资源:", setting.createAssets);
        //if (setting.createAssets)
        //{
        //    setting.delManifest = EditorGUILayout.Toggle("删除manifest", setting.delManifest);
        //    if(setting.delManifest)
        //        setting.isABHash = EditorGUILayout.Toggle("ABhash化", setting.isABHash);
        //}
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        if (setting.resPath == ResPath.Art)
            setting.genPackage = EditorGUILayout.Toggle("生成资源:", false);
        else
            setting.genPackage = EditorGUILayout.Toggle("生成应用包:", setting.genPackage);

        if (GUILayout.Button(save, EditorStyles.miniButtonLeft, GUILayout.MinWidth(180f)))
        {
            if (setting.outPaths.Count != setting.assetUrls.Count || setting.outPaths.Count != setting.apiUrls.Count || setting.outPaths.Count <= config.zoneId)
            {
                Debug.LogError("zoneId或者地址个数不合法");
                return;
            }
            Close();
            if (setting.createAllZone && setting.resPath == ResPath.PersistentData)
            {
                for (int i = 0; i < zoneOptions.Length; i++)
                {
                    Debug.Log("当前打包环境:" + i);
                    curZoneId = i;
                    if (setting.createAssets)
                        CreateAssetList(i == 0);
                    saveSetting(i == 0);
                    if (setting.genPackage)
                        GenApk();
                }
            }
            else
            {
                curZoneId = config.zoneId;
                if (setting.createAssets)
                    CreateAssetList(true);
                saveSetting(true);
                if (setting.genPackage)
                    GenApk();
            }
        }
    }

    private void DrawPlayerSetting()
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField("打包设置", titleStyle);
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        setting.appName = EditorGUILayout.TextField("package name:", setting.appName);
        setting.chgAppName = EditorGUILayout.Toggle("是否生效:", setting.chgAppName, GUILayout.Width(180));
        GUILayout.EndHorizontal();
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
        GUILayout.Space(5);
    }

    private void DrawOutput()
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField("出包地址", titleStyle);
        GUILayout.Space(5);
        for (int i = 0; i < outPaths.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (i % 2 == 0) GUI.backgroundColor = Color.cyan;
            else GUI.backgroundColor = Color.magenta;
            outPaths[i] = EditorGUILayout.TextField(string.Format("地址{0}:", i + 1), outPaths[i]);
            if (GUILayout.Button(deleteContent, EditorStyles.miniButtonLeft, buttonWidth))
            {
                outPaths.Remove(outPaths[i]);
                resetZoneOptions();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.ExpandWidth(true));
        if (GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, buttonWidth))
        {
            outPaths.Add("");
            resetZoneOptions();
        }
        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
    }

    private void DrawDownloads()
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField("下载地址", titleStyle);
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
        GUI.backgroundColor = Color.white;
    }

    private void DrawWebapis()
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField("接口地址", titleStyle);
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
        GUI.backgroundColor = Color.white;
    }

    private void CreateAssetList(bool firstMake)
    {
        if (setting.resPath == ResPath.StreamingAssets)
            outPathRes = FileUtils.ins.getStreamingPath(false);
        if (setting.resPath == ResPath.PersistentData)
            outPathRes = setting.outPaths[curZoneId];

        if (!string.IsNullOrEmpty(outPathRes))
        {
            if (firstMake)
            {
                BuildHelper.MakeLuaFiles();
                BuildHelper.MakeAssetBundleMap();
            }
            //BuildHelper.MakeAssetBundles(setting.delManifest, (BuildTarget)Enum.Parse(typeof(BuildTarget), setting.buildTarget.ToString()), outPathRes);
            BuildHelper.MakeAssetBundles(true, (BuildTarget)Enum.Parse(typeof(BuildTarget), setting.buildTarget.ToString()), outPathRes);
            MD5AndMakeConfigs();
        }
    }

    void MD5AndMakeConfigs()
    {
        string[] files = Directory.GetFiles(outPathRes);
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
            //if (setting.isABHash)
            FileUtils.ins.Move(file, outPathRes + "/" + item.md5);
        }
        config.assets = assets;
    }

    private void saveSetting(bool firstMake)
    {
        if (firstMake)
        {
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
            setting.outPaths = outPaths;
            setting.assetUrls = assetUrls;
            setting.apiUrls = apiUrls;
            //备份
            FileUtils.saveObjectToJsonFile(setting, Setting.settingPath(), false);
            FileUtils.saveObjectToJsonFile(config, Setting.versionPath(), false);
            //设置
            SavePlayerSetting();
        }

        config.assetUrl = assetUrls[curZoneId];
        config.apiUrl = apiUrls[curZoneId];
        //资源包内
        if (!string.IsNullOrEmpty(outPathRes))
        {
            if (setting.resPath < ResPath.PersistentData)
                FileUtils.saveObjectToJsonFile(config, outPathRes + "/" + AssetUpdater.Config_Name, false);
            else
                FileUtils.saveObjectToJsonFile(config, outPathRes + "/" + FileUtils.ins.GetMD5FromString(AssetUpdater.Config_Name).ToLower(), true);
        }
        //工程内
        if (setting.resPath == ResPath.PersistentData)
        {
            string streamPath = FileUtils.ins.getStreamingPath(false);
            FileUtils.saveObjectToJsonFile(config, streamPath + "/" + FileUtils.ins.GetMD5FromString(AssetUpdater.Config_Name).ToLower(), true);
        }
        AssetDatabase.Refresh();
        Debug.Log("设置保存成功！！！");
    }

    private void SavePlayerSetting()
    {
        if (setting.chgAppName)
        {
            string[] names = setting.appName.Split('.');
            if (names.Length != 3)
            {
                GameDebug.LogError("包名不合法");
                return;
            }
            PlayerSettings.applicationIdentifier = setting.appName;
            PlayerSettings.companyName = names[1];
            PlayerSettings.productName = names[2];
        }

        PlayerSettings.Android.keystoreName = setting.keystore;
        PlayerSettings.Android.keystorePass = setting.keypass;
        PlayerSettings.Android.keyaliasName = setting.keyaliname;
        PlayerSettings.Android.keyaliasPass = setting.keyalipass;

        PlayerSettings.Android.bundleVersionCode = config.GetBundleVersionCode();
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
        //if (setting.isABHash && !defineSymbols.Contains(hashType))
        //    defineSymbols.Add(hashType);
        //else if (!setting.isABHash && defineSymbols.Contains(hashType))
        //    defineSymbols.Remove(hashType);
        if (!defineSymbols.Contains(hashType))
            defineSymbols.Add(hashType);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defineSymbols.ToArray()));
    }

    private void GenApk()
    {
        if (setting.resPath == ResPath.StreamingAssets)
            outPathApk = Application.dataPath.Replace("Assets", "im.apk");
        if (setting.resPath == ResPath.PersistentData)
            outPathApk = setting.outPaths[curZoneId].Replace("update\\Hall\\android", "download\\im.apk");
        BuildHelper.TryGenApk(outPathApk);
        AssetDatabase.Refresh();
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
