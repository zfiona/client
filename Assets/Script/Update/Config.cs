using System.Collections.Generic;
using GameUtils;
using System;

public class Config
{
    public string version { get; set; } //版本号1.1.1
    public int zoneId { get; set; } //区服id
    public string apiUrl { get; set; }//web地址
    public string assetUrl { get; set; }//资源地址
    public int logLevel { get; set; } //日志级别(0,1,2)

    public List<FileItem> assets { get; set; } //热更资源信息

    public VersionState CompareVersion(string remoteVersion)
    {
        string[] arg1 = version.Split('.');
        string[] arg2 = remoteVersion.Split('.');
        if (arg1.Length != arg2.Length)
            return VersionState.bigUpdate;
        for (int i = 0; i < arg1.Length - 1; i++)
        {
            if (arg1[i] != arg2[i])
                return VersionState.bigUpdate;
        }
        return VersionState.hotUpdate;
    }

    public string GetApkPath()
    {
        return assetUrl.Replace("/update", "/download/im.apk");
    }

    public string GetAbPath(string gameName = "")
    {
        GameDebug.Log(assetUrl);
        string OriUrl;
        if (string.IsNullOrEmpty(gameName))
            OriUrl = $"{assetUrl}/Hall";
        else
            OriUrl = $"{assetUrl}/{gameName}";

        OriUrl = $"{OriUrl}/{FileUtils.ins.getRuntimePlatform()}/";
        return OriUrl;
    }

    public string GetVersion()
    {
        //if (version.Split('.').Length == 2)
        //    return "version: 1." + version;
        return "version: " + version;
    }

    public int GetBundleVersionCode()
    {
        string[] versions = version.Split('.');
        if (versions.Length == 2)
            return int.Parse(versions[0]);
        else
        {
            int a = int.Parse(versions[0]);
            int b = int.Parse(versions[1]);
            b = Math.Max(b, 9);
            return Math.Max((a - 1) * 10 + b, 1);
        }
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

