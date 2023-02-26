using System;
using System.Collections.Generic;
using System.IO;
using GameUtils;
using NetExtension;
using UnityEngine;

public enum ProgressState
{
    Checking,
    Checked,
    CheckError,
    Updating,
    Updated,
    UpdateError,
    UnCompress,
    BigVersion,
    BigUpdating,
    BigUpdated,
    BigError,
    Virtual,
}

public class AssetUpdater
{
    public static string Apk_Name = "tmp.apk";
    public static string Config_Name = "version.json"; //"3ef5e0d96a4566e6853165551eb193b0";
    public static string Lua_Name = "luazip";
    public static string Manifest_Name = "manifest";
    public const string Lua_Src_Path = "lua";
    public const string Lua_Output_Path = "Assets/Art/LuaRes";
    public const string AssetBundle_Map_Path = "Assets/Art/BundleToAssetsMap.txt";
    public const string AssetBundle_Map_Name = "assetbundle_map";
    public const string AssetBundle_Loading = "loading";
    private Action<ProgressState, float> mCallback;
    
    private List<FileItem> _updateBundles = new List<FileItem>();
    private int _loadBundleIndex = 0;
    private float curbytes;
    private float totalBytes;

    private FileUtils _fileUtils;
    private Config localConfig;
    private Config remoteConfig;

    public void StartUpdate(Action<ProgressState,float> callback)
    {
        mCallback = callback;
        _fileUtils = FileUtils.ins;
        localConfig = AppConst.config;
        mCallback.Invoke(ProgressState.Checking, 0);
        DownloadConfig();
    }

    public void DownloadConfig()
    {
        string url = localConfig.GetAbPath();
        if (string.IsNullOrEmpty(url))
        {
            mCallback.Invoke(ProgressState.CheckError, 0);
        }
        else
        {
            url += FileUtils.ins.GetMD5FromString(Config_Name).ToLower();
            HttpRequest request = new HttpRequest(OnConfigDone);
            request.Get(url);
        }
    }

    public bool IsLastCompleted()
    {
        foreach (var item in localConfig.assets)
        {
#if AssetBundleHash
            string path = _fileUtils.getPresistentPath(true) + item.md5;
#else
            string path = _fileUtils.getPresistentPath(true) + item.path;
#endif
            if (File.Exists(path))
            {
                long length = new FileInfo(path).Length;
                if (length < item.length)
                    return false;
            }
            else
            {
                return false;
            }

        }
        return true;
    }

    private void OnConfigDone(string api,string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            //DownloadConfig();
            mCallback.Invoke(ProgressState.CheckError, 0);
            return;
        }
        remoteConfig = FileUtils.loadObjectFromJson<Config>(text);
        AppConst.config = remoteConfig;

        Config.VersionState state = localConfig.CompareVersion(remoteConfig.version);
        if (state == Config.VersionState.bigUpdate)
            mCallback.Invoke(ProgressState.BigVersion, 0);
        else
            DownloadAssets();
    }

    public void DownloadAssets()
    {
        curbytes = 0;
        totalBytes = 0;
        //del old
        foreach (var item in localConfig.assets)
        {
            FileItem item2 = remoteConfig.assets.FindLast((t) =>
            {
                return t.md5 == item.md5;
            });
            if (item2 == null)
            {
#if AssetBundleHash
                string path = _fileUtils.getPresistentPath(true) + item.md5;
#else
                string path = _fileUtils.getPresistentPath(true) + item.path;
#endif
                if (File.Exists(path))
                {
                    File.Delete(path);
                    GameDebug.Log("del: " + item.path);
                }                    
            }
        }
        //get new
        foreach (var item in remoteConfig.assets)
        {
#if AssetBundleHash
            string path = _fileUtils.getPresistentPath(true) + item.md5;
#else
            string path = _fileUtils.getPresistentPath(true) + item.path;
#endif
            if (!File.Exists(path))
            {
                _updateBundles.Add(item);
                totalBytes += item.length;
                GameDebug.Log("add: " + item.path + " length: " + item.length);
            }
            else
            {
                long length = new FileInfo(path).Length;
                if (length < item.length)
                {
                    _updateBundles.Add(item);
                    totalBytes += (int)(item.length - length);
                    GameDebug.Log("update: " + item.path);
                }
            }
        }
        //load new
        if(totalBytes == 0)
        {
            mCallback.Invoke(ProgressState.Updated, totalBytes);
        }
        else
        {
            mCallback.Invoke(ProgressState.Checked, totalBytes);
            _loadBundleIndex = 0;
            HttpLoader loader = new HttpLoader(OnDone, OnProgress);
            loader.StartDownloadAll(_updateBundles, remoteConfig.GetAbPath(), _fileUtils.getPresistentPath(true));
        }
    }

    private void OnDone(byte[] data)
    {
        if (data == null)
        {
            mCallback.Invoke(ProgressState.UpdateError, 0);
            return;
        }

        _loadBundleIndex++;
        curbytes += data.Length;
        if (_loadBundleIndex == _updateBundles.Count)
        {
            string storgePath = _fileUtils.getPresistentPath(true) + FileUtils.ins.GetMD5FromString(Config_Name).ToLower();
            FileUtils.saveObjectToJsonFile(remoteConfig, storgePath, true);
            mCallback.Invoke(ProgressState.Updated, totalBytes);
        }
    }

    private void OnProgress(long cur, long total)
    {
        mCallback.Invoke(ProgressState.Updating, curbytes + cur);
    }

    public void StartDownloadApk()
    {
        mCallback.Invoke(ProgressState.BigUpdating, 0);
        string url = AppConst.config.GetApkPath();
        string path = Application.persistentDataPath + "/" + Apk_Name;
        if (File.Exists(path))
            File.Delete(path);
        
        HttpLoader loader = new HttpLoader(OnDoneApk, OnProgressApk);
        loader.timeOut = 300;
        loader.StartDownload(url, path);
    }

    private void OnDoneApk(byte[] data)
    {
        GameDebug.Log("Download success!");
        if (data == null)
        {
            mCallback.Invoke(ProgressState.BigError, 0);
            return;
        }
        mCallback.Invoke(ProgressState.BigUpdated, 0);
    }

    private void OnProgressApk(long cur,long total)
    {
        float progress = 1f * cur / total;
        mCallback.Invoke(ProgressState.BigUpdating, progress);
    }
}


