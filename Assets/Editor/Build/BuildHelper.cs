using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using GameUtils;

public class BuildHelper
{
    [MenuItem("GameObject/RemoveRaycast", false, 11)]
    static void RemoveRaycast()
    {
        Debug.Log(GameObject.FindObjectsOfType<MaskableGraphic>().Length);
        foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
        {
            if (g.raycastTarget)
            {
                string s = g.name;
                if (s.StartsWith("btn") || s.StartsWith("tog") || s.StartsWith("input") || s.StartsWith("slider") || s.Equals("Viewport") || s.Equals("img_bg"))
                    continue;
                g.raycastTarget = false;
                Debug.Log(g.name);
            }
        }
    }

    [MenuItem("Tools/test", false, 12)]
    static void teset()
    {

    }


    [MenuItem("Tools/Build/Setting", false, 10)]
    static void Set()
    {
        if (EditorApplication.isCompiling)
        {
            Debug.Log("Unity Editor is compiling, please wait.");
            return;
        }
        if (FileUtils.ins.isDirectoryExist(Application.dataPath.Replace("Assets", "editor_config")))
        {
            SettingWindow window = EditorWindow.GetWindow<SettingWindow>("打包工具");
            window.minSize = new Vector2(300, 400);
            window.Show();
        }
    }

    [MenuItem("Tools/Build/set abName", false, 99)]
    static void CreateAbName()
    {
        string path = "Art/hall/spine";
        string abName = "hall_spine";
        SetAssetBundlesName(Path.Combine(Application.dataPath, path), abName);

        Debug.Log("set ok");
    }

    static void SetAssetBundlesName(string dirPath, string _abName)
    {
        DirectoryInfo dir = new DirectoryInfo(dirPath);
        FileSystemInfo[] files = dir.GetFileSystemInfos();

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                SetAssetBundlesName(files[i].FullName, _abName);
            }
            else if (!files[i].Name.EndsWith(".meta"))
            {

                string assetPath = files[i].FullName;
                string importerPath = "Assets" + assetPath.Substring(Application.dataPath.Length);
                AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);
                assetImporter.assetBundleName = _abName;
            }
        }
    }


    [MenuItem("Tools/Build/clear abName", false, 100)]
    static void ClearAllAbName()
    {
        //获取所有的AssetBundle名称
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();

        //强制删除所有AssetBundle名称
        for (int i = 0; i < abNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(abNames[i], true);
        }
        AssetDatabase.Refresh();
        Debug.Log("清除Meta配置文件完成");
    }
    [MenuItem("Tools/Build/clear packingTag", false, 101)]
    static void ClearPackingTag()
    {
        string path = Application.dataPath + "/Art/hall/image";
        string pngPath = "";
        DirectoryInfo info = new DirectoryInfo(path);
        FileInfo[] files = info.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            pngPath = "Assets/" + files[i].FullName.Remove(0, Application.dataPath.Length + 1).Replace("\\", "/");
            if (pngPath.EndsWith(".png"))
            {
                TextureImporter textureImporter = AssetImporter.GetAtPath(pngPath) as TextureImporter;
                textureImporter.spritePackingTag = "";
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("清除packingTag完成");
    }

    [MenuItem("Tools/Build/insert keystore", false, 102)]
    static void InsertKeystore()
    {
        Setting setting = FileUtils.loadObjectFromJsonFile<Setting>(Setting.settingPath());
        PlayerSettings.Android.keystoreName = setting.keystore;
        PlayerSettings.Android.keystorePass = setting.keypass;
        PlayerSettings.Android.keyaliasName = setting.keyaliname;
        PlayerSettings.Android.keyaliasPass = setting.keyalipass;
    }

    [MenuItem("Tools/Build/Make assetbundles")]
    static void MakeAssetBundles()
    {
        MakeAssetBundles(false, EditorUserBuildSettings.activeBuildTarget, FileUtils.ins.getStreamingPath(false));
    }

    public static void MakeAssetBundles(bool delManifest, BuildTarget buildTarget,string outPath)
    {
        if (Directory.Exists(outPath))
            Directory.Delete(outPath, true);
        Directory.CreateDirectory(outPath);

        BuildAssetBundleOptions options =
            BuildAssetBundleOptions.DeterministicAssetBundle
            | BuildAssetBundleOptions.ChunkBasedCompression
            | BuildAssetBundleOptions.StrictMode
            | BuildAssetBundleOptions.AppendHashToAssetBundleName;
        BuildPipeline.BuildAssetBundles(outPath, options, buildTarget);
        if (delManifest)
        {
            string[] files = Directory.GetFiles(outPath);
            foreach (string file in files)
            {
                if (file.EndsWith(".manifest"))
                    File.Delete(file);
            }
        }
        AssetDatabase.Refresh();
        string oldFile = outPath + "/" + Path.GetFileName(outPath);
        string newFile = outPath + "/" + AssetUpdater.Manifest_Name + "_" + FileUtils.ins.GetMd5HashFromFile(oldFile);
        File.Move(oldFile, newFile);
        AssetDatabase.Refresh();
        Debug.Log("make assetbundle over !");
    }

    [MenuItem("Tools/Build/Inject luazip")]
    static void InjectLua()
    {

    }

    public static void MakeLuaFiles()
    {
        CopyLuaFilesToBytes();
        SetLuaAssetBundleName();
        Debug.Log("make lua files over !");
    }

    private static void CopyLuaFilesToBytes()
    {
        string outputPath = Application.dataPath.Replace("Assets", AssetUpdater.Lua_Output_Path);
        if (Directory.Exists(outputPath))
            Directory.Delete(outputPath, true);

        string sourcePath = Application.dataPath.Replace("Assets", AssetUpdater.Lua_Src_Path);
        string[] paths = Directory.GetFiles(sourcePath, "*.lua", SearchOption.AllDirectories);
        foreach (string path in paths)
        {
            string newPath = outputPath + path.Remove(0, sourcePath.Length).Replace(".lua", ".bytes");
            string dir = Path.GetDirectoryName(newPath);
            Directory.CreateDirectory(dir);
            byte[] buffer = File.ReadAllBytes(path);
            File.WriteAllBytes(newPath, EncryptUtil.Encrypt(buffer));
        }
        AssetDatabase.Refresh();
    }

    private static void SetLuaAssetBundleName()
    {
        var guids = AssetDatabase.FindAssets("", new string[] { AssetUpdater.Lua_Output_Path });
        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null && assetPath.EndsWith(".bytes"))
            {
                importer.SetAssetBundleNameAndVariant(AssetUpdater.Lua_Name, string.Empty);
            }
        }
    }

    public static void MakeAssetBundleMap()
    {
        StringBuilder sb = new StringBuilder();
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        foreach (string abName in abNames)
        {
            //Debug.Log(abName);
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
            if (assetPaths != null && assetPaths.Length > 0)
            {
                sb.Append(abName).Append("\n");
                foreach (string assetPath in assetPaths)
                {
                    sb.Append("\t").Append(assetPath).Append("\n");
                }
            }
        }
        string dir = Path.GetDirectoryName(AssetUpdater.AssetBundle_Map_Path);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(AssetUpdater.AssetBundle_Map_Path, sb.ToString());

        AssetImporter importer = AssetImporter.GetAtPath(AssetUpdater.AssetBundle_Map_Path);
        importer.SetAssetBundleNameAndVariant(AssetUpdater.AssetBundle_Map_Name, string.Empty);
        AssetDatabase.Refresh();
        Debug.Log("make abMap over !");
    }
}
