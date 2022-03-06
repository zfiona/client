using GameUtils;
using System.Text;
using UnityEngine;


public static class LuaHelper
{
    public static bool isLocalFile = true;

    public static string LoadProtoFile(string fn)
    {
        byte[] data = null;
        string path;
        if (isLocalFile)
        {
            path = Application.dataPath.Replace("Assets", AssetUpdater.Lua_Src_Path + "/") + fn + ".lua";
            data = FileUtils.ins.getBytes(path);
        }
        else
        {
            path = AssetUpdater.Lua_Output_Path + "/" + fn + ".bytes";
            //path = AssetBundleMgr.Instance.GetAssetPathByName(fn);
            TextAsset textAsset = AssetBundleMgr.Instance.LoadTextAsset(path);
            if (textAsset != null)
            {
                data = EncryptUtil.Decrypt(textAsset.bytes);
                Resources.UnloadAsset(textAsset);
            }
        }
        GameDebug.LogGame(path);
        return Encoding.UTF8.GetString(data);
    }

    public static byte[] DoFile(ref string fn)
    {
        byte[] data = null;
        string path;
        if (isLocalFile)
        {
            path = Application.dataPath.Replace("Assets", AssetUpdater.Lua_Src_Path + "/") + fn.Replace(".", "/") + ".lua";
            data = FileUtils.ins.getBytes(path);
        }
        else
        {
            path = AssetUpdater.Lua_Output_Path + "/" + fn.Replace(".", "/") + ".bytes";
            //path = AssetBundleMgr.Instance.GetAssetPathByName(fn.Replace(".", "/"));
            TextAsset textAsset = AssetBundleMgr.Instance.LoadTextAsset(path);
            if (textAsset == null)
            {
                GameDebug.LogError("文件加载失败:"+ path);
            }
            else
            {
                data = EncryptUtil.Decrypt(textAsset.bytes);
                Resources.UnloadAsset(textAsset);
            }
        }
        //GameDebug.Log("lua路径： " + path);
        return data;
    }

}
