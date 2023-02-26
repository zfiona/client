using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using GameUtils;
using System.Collections.Generic;

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

    [MenuItem("Tools/Build/Clear Assetbundles Name", false, 100)]
    static void ClearAllAbName()
    {
        Object[] selObj = Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered);
        foreach (Object item in selObj)
        {
            string objPath = AssetDatabase.GetAssetPath(item);
            List<string> selectedPicsPathList = new List<string>();
            if (!Directory.Exists(objPath))
                selectedPicsPathList.Add(objPath);
            else
                selectedPicsPathList = GetFilesRecursively(objPath);
            
            for (int i = 0; i < selectedPicsPathList.Count; i++)
            {
                string filePath = selectedPicsPathList[i];
                AssetImporter ai = AssetImporter.GetAtPath(filePath);
                ai.assetBundleName = null;
                EditorUtility.DisplayProgressBar("清除AssetBundle", filePath, 1f * i / selectedPicsPathList.Count);
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        Debug.Log("清除AssetBundle完成");
    }
    static List<string> GetFilesRecursively(string folder)
    {
        List<string> tar = new List<string>();
        string[] dirs = Directory.GetFileSystemEntries(folder);
        for (int j = 0; j < dirs.Length; j++)
        {
            string tarPath = dirs[j];
            if (Directory.Exists(tarPath))
            {
                tar.AddRange(GetFilesRecursively(tarPath));
            }
            if (File.Exists(tarPath))
            {
                if (tarPath.EndsWith("meta") || string.IsNullOrEmpty(tarPath))
                    continue;
                else
                {
                    if (!tar.Contains(tarPath))
                        tar.Add(tarPath);
                }
            }
        }
        return tar;
    }

    [MenuItem("Tools/Build/Clear Atlas Name", false, 101)]
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
        Debug.Log("清除Atlas完成");
    }

    [MenuItem("Tools/Build/Insert keystore", false, 102)]
    static void InsertKeystore()
    {
        Setting setting = FileUtils.loadObjectFromJsonFile<Setting>(Setting.settingPath());
        PlayerSettings.Android.keystoreName = setting.keystore;
        PlayerSettings.Android.keystorePass = setting.keypass;
        PlayerSettings.Android.keyaliasName = setting.keyaliname;
        PlayerSettings.Android.keyaliasPass = setting.keyalipass;
    }

    [MenuItem("Tools/Build/Make Assetbundles")]
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

    [MenuItem("Tools/Build/Gen Package")]
    static void GenPackage()
    {
#if UNITY_ANDROID
        string path = Application.dataPath.Replace("Assets", "im.apk");
        TryGenApk(path);
#endif
    }
    public static void TryGenApk(string outPath)
    {
        BuildTarget target = BuildTarget.Android;
        BuildOptions options = BuildOptions.CompressWithLz4;
        string[] outScenes = new string[] { "Assets/Scene/main.unity" };
        BuildPipeline.BuildPlayer(outScenes, outPath,target, options);
        Debug.Log("生成apk成功");
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
