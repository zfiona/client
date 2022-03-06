using UnityEngine;
using System.Collections.Generic;
using GameUtils;
using System.Collections;

public class AssetBundleMgr
{
    public static AssetBundleMgr Instance
    {
        get
        {
            return Singleton.GetInstance<AssetBundleMgr>();
        }
    }

    private Dictionary<string, string> _assetPath_to_bundleName = new Dictionary<string, string>();
    private Dictionary<string, string> _bundleName_to_hashName = new Dictionary<string, string>();
    private Dictionary<string, AssetBundle> _bundleCache = new Dictionary<string, AssetBundle>();
    //资源总包
    private AssetBundleManifest _manifest = null;

    public void Clear()
    {
        if (_manifest != null)
            Resources.UnloadAsset(_manifest);
        _manifest = null;
        _assetPath_to_bundleName.Clear();
        _bundleName_to_hashName.Clear();
        foreach (var pair in _bundleCache)
        {
            var bundle = pair.Value;
            if (bundle != null)
            {
                bundle.Unload(false);
            }
        }
        _bundleCache.Clear();
    }

    public void Init()
    {
        Clear();
        LoadABFileList();
        LoadAssetsMap();
    }

    private void LoadABFileList()
    {
        foreach (FileItem item in AppConst.config.assets)
        {
            string[] ss = item.path.Split('_');
            string bundleName = ss[0];
            for (int i = 1; i < ss.Length - 1; i++)
                bundleName += "_" + ss[i];
#if AssetBundleHash
            _bundleName_to_hashName[bundleName] = item.md5;
#else
           _bundleName_to_hashName[bundleName] = item.path;
#endif
        }
    }

    private void LoadAssetsMap()
    {
        string assetHash = GetABHashByName(AssetUpdater.AssetBundle_Map_Name);
        string fullPath = GetABPathByHash(assetHash);
        AssetBundle bundle = AssetBundle.LoadFromFile(fullPath);
        if (bundle == null)
        {
            GameDebug.LogGame(fullPath);
            GameDebug.LogError("AssetBundle_Map bundle load error");
            return;
        }
        TextAsset textAsset = bundle.LoadAsset<TextAsset>(AssetUpdater.AssetBundle_Map_Path);
        string[] lines = textAsset.text.Split('\n');
        bundle.Unload(true);
        string bundleName = null;
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            if (line.StartsWith("\t"))
            {
                if (bundleName != null)
                {
                    string assetPath = line.Substring(1);
                    _assetPath_to_bundleName[assetPath] = bundleName;
                }
            }
            else
            {
                bundleName = line;
            }
        }
    }
    private void LoadManifest()
    {
        if (_manifest == null)
        {
            string assetHash = GetABHashByName(AssetUpdater.Manifest_Name);
            string abPath = GetABPathByHash(assetHash);
            if (string.IsNullOrEmpty(abPath))
            {
                GameDebug.LogError("AssetBundleManifest bundle load error");
                return;
            }
            try
            {
                Debug.Log("加载Manifest");
                AssetBundle bundle = AssetBundle.LoadFromFile(abPath);
                _manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                bundle.Unload(false);
            }
            catch
            {
                GameDebug.LogError("AssetBundleManifest bundle load error");
            }
        }
    }

    private string GetABNameByAssetPath(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
            return null;
        string abName = null;
        _assetPath_to_bundleName.TryGetValue(assetPath, out abName);
        return abName;
    }

    private string GetABHashByName(string abName)
    {
        if (string.IsNullOrEmpty(abName))
            return null;
        string abHash;
        _bundleName_to_hashName.TryGetValue(abName, out abHash);
        return abHash;
    }

    private string GetABNameByHash(string hash)
    {
        string abName = "";
        foreach(var pair in _bundleName_to_hashName)
        {
            if(pair.Value == hash)
            {
                abName = pair.Key;
                break;
            }
        }
        return abName;
    }

    private string GetABPathByHash(string abHash)
    {
        if (string.IsNullOrEmpty(abHash))
            return null;
        string abPath = "";
        switch (Setting.Get().resPath)
        {
            case ResPath.StreamingAssets:
                abPath = FileUtils.ins.getStreamingPath(true) + abHash;
                break;
            case ResPath.PersistentData:
                abPath = FileUtils.ins.getPresistentPath(true) + abHash;
                break;
            default:
                break;
        }
        return abPath;
    }

    public void UnloadAssetBundle(string abName)
    {
        var hashName = GetABHashByName(abName);
        if (string.IsNullOrEmpty(hashName))
            return;
        AssetBundle assetBundle = null;
        if (_bundleCache.TryGetValue(hashName, out assetBundle))
        {
            assetBundle.Unload(false);
            _bundleCache.Remove(hashName);
        }
    }

    private void LoadDependencies(string abHashName)
    {
        LoadManifest();
        if(_manifest != null)
        {
#if AssetBundleHash
            abHashName = GetABNameByHash(abHashName) + "_" + abHashName;
#endif
            string[] dependencies = _manifest.GetAllDependencies(abHashName);
            for (int i = 0; i < dependencies.Length; i++)
            {
#if AssetBundleHash
                string dep_hash = dependencies[i].Split('_')[dependencies[i].Split('_').Length - 1];
#else
                string dep_hash = dependencies[i];
#endif
                if (!_bundleCache.ContainsKey(dep_hash))
                {
                    string abPath = GetABPathByHash(dep_hash);
                    var ab = AssetBundle.LoadFromFile(abPath);
                    _bundleCache.Add(dep_hash, ab);
                    if (ab == null)
                        GameDebug.LogError("不存在的AB包：" + abHashName);
                }
            }
        }
    }

    private T LoadAssetFromAssetBundle<T>(string abHashName, string assetName) where T : Object
    {
        AssetBundle ab;
        if (_bundleCache.ContainsKey(abHashName))
        {
            ab = _bundleCache[abHashName];
        }
        else
        {
            string abPath = GetABPathByHash(abHashName);
            if (abPath == "" || !FileUtils.ins.isFileExist(abPath))
            {
                GameDebug.Log("不存在的AB路径：" + abPath);
                return null;
            }
            LoadDependencies(abHashName);
            ab = AssetBundle.LoadFromFile(abPath);
            _bundleCache.Add(abHashName, ab);
            if (ab == null)
            {
                GameDebug.LogError("不存在的AB包：" + abHashName);
                return null;
            }
        }
        return ab.LoadAsset<T>(assetName);
    }

    public GameObject LoadLoadingUI()
    {
        LoadABFileList();
        string fullPath = "Assets/Resources/ui_loading.prefab";
        string abHashName = GetABHashByName(AssetUpdater.AssetBundle_Loading);
        return LoadAssetFromAssetBundle<GameObject>(abHashName, fullPath);
    }

    public GameObject LoadPrefab(string fullPath)
    {
        string abHashName = GetABHashByName(GetABNameByAssetPath(fullPath));
        if (string.IsNullOrEmpty(abHashName))
        {
            return null;
        }
        return LoadAssetFromAssetBundle<GameObject>(abHashName, fullPath);
    }

    public Sprite LoadSprite(string fullPath)
    {
        string abHashName = GetABHashByName(GetABNameByAssetPath(fullPath));
        if (string.IsNullOrEmpty(abHashName))
        {
            return null;
        }
        return LoadAssetFromAssetBundle<Sprite>(abHashName, fullPath);
    }

    public Texture LoadTexture(string fullPath)
    {
        string abHashName = GetABHashByName(GetABNameByAssetPath(fullPath));
        if (string.IsNullOrEmpty(abHashName))
        {
            return null;
        }
        return LoadAssetFromAssetBundle<Texture>(abHashName, fullPath);
    }

    public AudioClip LoadAudio(string fullPath)
    {
        string abHashName = GetABHashByName(GetABNameByAssetPath(fullPath));
        if (string.IsNullOrEmpty(abHashName))
        {
            return null;
        }
        string[] ss = fullPath.Split(new char[] { '/', '.' });
        string resName = ss[ss.Length - 2];
        return LoadAssetFromAssetBundle<AudioClip>(abHashName, fullPath);
    }

    public TextAsset LoadTextAsset(string fullPath)
    {
        string abHashName = GetABHashByName(GetABNameByAssetPath(fullPath));
        if (string.IsNullOrEmpty(abHashName))
        {
            return null;
        }
        return LoadAssetFromAssetBundle<TextAsset>(abHashName, fullPath);
    }

    public Font LoadFont(string fullPath)
    {
        string abHashName = GetABHashByName(GetABNameByAssetPath(fullPath));
        if (string.IsNullOrEmpty(abHashName))
        {
            return null;
        }
        return LoadAssetFromAssetBundle<Font>(abHashName, fullPath);
    }
    public Material LoadMaterial(string fullPath)
    {
        string abHashName = GetABHashByName(GetABNameByAssetPath(fullPath));
        if (string.IsNullOrEmpty(abHashName))
        {
            return null;
        }
        return LoadAssetFromAssetBundle<Material>(abHashName, fullPath);
    }
}

