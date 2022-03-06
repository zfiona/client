using UnityEngine;
using System.Collections.Generic;
using System;
using GameUtils;

public class Config
{
    public string version { get; set; } //版本号1.1
    public string appUrl { get; set; } //下载包地址
    public string kefuUrl { get; set; } //客服地址
    public List<string> assetUrls { get; set; }//资源地址
    public List<string> apiUrls { get; set; } //web地址
    public int zoneId { get; set; } //区服id
    public string appName { get; set; }//app名

    public List<FileItem> assets { get; set; } //热更资源信息

    public VersionState CompareVersion(string remote)
    {
        string[] arg1 = version.Split('.');
        string[] arg2 = remote.Split('.');
        if (arg1[0] != arg2[0])
            return VersionState.bigUpdate;
        else
            return VersionState.hotUpdate;
    }
    
    public int GetNetApis()
    {
        if (apiUrls == null)
            return 0;
        return apiUrls.Count;
    }

    public string GetPath(int index,string gameName="")
    {
        string OriUrl = "";
        if (assetUrls == null || index >= assetUrls.Count)
            return OriUrl;
        
        if (string.IsNullOrEmpty(gameName))
            OriUrl = $"{assetUrls[index]}/{zoneId}/{appName}";
        else
            OriUrl = $"{assetUrls[index]}/{zoneId}/{gameName}";

        OriUrl = $"{OriUrl}/{FileUtils.ins.getRuntimePlatform()}/";
        return OriUrl;
    }


    public enum VersionState
    {
        hotUpdate,
        bigUpdate,
    }
    
}
public class FileItem
{
    public string path;
    public string md5;
    public int length;
}

