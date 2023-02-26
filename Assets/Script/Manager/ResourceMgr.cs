using UnityEngine;
using GameUtils;
using System.IO;
using UnityEngine.UI;
using Spine.Unity;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// 资源管理器，带缓存
/// shader资源 常驻内存打包
/// </summary>
public class ResourceMgr
{
    public static ResourceMgr GetInstance
    {
        get
        {           
            return Singleton.GetInstance<ResourceMgr>();
        }
    }

    public ResourceMgr()
    {
        string path = "";
        switch (Setting.Get().resPath)
        {
            case ResPath.StreamingAssets:
                GameDebug.LogYellow("从StreamingAssets加载资源");

                path = FileUtils.ins.getStreamingPath(true) + AssetUpdater.Config_Name;
                AppConst.config = FileUtils.loadObjectFromJsonFile<Config>(path);
                AssetBundleMgr.Instance.Init();
                break;
            case ResPath.PersistentData:
                GameDebug.LogYellow("从PersistentData加载资源");

                path = FileUtils.ins.getPresistentPath(true) + FileUtils.ins.GetMD5FromString(AssetUpdater.Config_Name).ToLower();
                AppConst.config = FileUtils.loadObjectFromJsonFile<Config>(path);              
                break;
            default:
                GameDebug.LogYellow("从Art本地加载资源");
                PreLoadUIPrefab();
                break;
        }
        
    }

    private string pathRoot = "Assets/Art/";  //资源相对路径
    public void SetPathRoot(string path)
    {
        pathRoot = path;
    }

    public void UnLoadRes()
    {
        uiCache.Clear();
    }

    public void PreLoadUIPrefab()
    {
        string resName = "hall/prefab/common/ui_userData";
        LoadUIPrefab(resName);

        resName = "hall/prefab/common/ui_settlement";
        LoadUIPrefab(resName);
    }

    Dictionary<string, GameObject> uiCache = new Dictionary<string, GameObject>();
    public GameObject LoadUIPrefab(string resName)
    {
        //GameDebug.Log("加载路径 ：" + resName);
        if (resName == "ui_loading") //启动界面
            return LoadUILoading(resName);
        //if has cache
        resName = pathRoot + resName + ".prefab";
        if (uiCache.ContainsKey(resName))
            return uiCache[resName];
        //load prefab
        GameObject go = null;
#if UNITY_EDITOR && LoadFromLocal
        go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(resName);
#else
        go = AssetBundleMgr.Instance.LoadPrefab(resName);
#endif
        if(go != null)
            uiCache.Add(resName, go);
        return go;
    }

    private GameObject LoadUILoading(string resName)
    {
        GameObject go = null;
        if (Setting.Get().resPath == ResPath.PersistentData)
        {
            Debug.Log("比对启动界面的AB包...");
            foreach (var item in AppConst.config.assets)
            {
                if (!item.path.StartsWith(AssetUpdater.AssetBundle_Loading))
                    continue;
                string path = FileUtils.ins.getPresistentPath(true) + item.path;
                if (File.Exists(path) && new FileInfo(path).Length == item.length)
                {
                    Debug.Log("loading try from assetsbundle...");
                    go = AssetBundleMgr.Instance.LoadLoadingUI();
                }
            }
        }
        if (go == null)
        {
            Debug.Log("loading from resources...");
            go = Resources.Load<GameObject>(resName);
        }
        return go;
    }

    public Sprite LoadSprite(string resName,string suffix = ".png")
    {
        resName = pathRoot + resName + suffix;
        Sprite sprite = null;
#if UNITY_EDITOR && LoadFromLocal
        sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(resName);
#else
        sprite = AssetBundleMgr.Instance.LoadSprite(resName);
#endif
        return sprite;
    }

    public Texture LoadTexture(string resName, string suffix = ".jpg")
    {
        resName = pathRoot + resName + suffix;
        Texture texture = null;
#if UNITY_EDITOR && LoadFromLocal
        texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>(resName);
#else
        texture = AssetBundleMgr.Instance.LoadTexture(resName);
#endif
        return texture;
    }

    public Material LoadMaterial(string resName)
    {
        resName = pathRoot + resName + ".mat";
        Material material = null;
#if UNITY_EDITOR && LoadFromLocal
        material = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(resName);
#else
        material = AssetBundleMgr.Instance.LoadMaterial(resName);
#endif
        return material;
    }

    public AudioClip LoadAudio(string resName, string suffix = ".mp3")
    {
        resName = pathRoot + resName + suffix;
        AudioClip audio = null;
#if UNITY_EDITOR && LoadFromLocal
        audio = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(resName);
#else
        audio = AssetBundleMgr.Instance.LoadAudio(resName);
#endif
        return audio;
    }
    public SkeletonDataAsset LoadSkeletonDataAsset(string resName, string suffix = ".asset")
    {
        resName = pathRoot + resName+ suffix;
        SkeletonDataAsset data = null;
#if UNITY_EDITOR && LoadFromLocal
        data = UnityEditor.AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(resName);
#else
        data = AssetBundleMgr.Instance.LoadSkeletonDataAsset(resName);
#endif
        return data;
    }


}
