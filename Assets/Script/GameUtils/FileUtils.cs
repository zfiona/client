using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameUtils
{
    public enum BuildPlatform
    {
        Android,
        iOS,
    }

    public class FileUtils
    {
        private string streamingAssetsPath;
        private string persistentDataPath;

        public FileUtils()
        {       
            persistentDataPath = Application.persistentDataPath;
            streamingAssetsPath = Application.streamingAssetsPath;
        }
        static public FileUtils ins
        {
            get
            {
                return Singleton.GetInstance<FileUtils>();
            }
        }

        static public T loadObjectFromJsonFile<T>(string path) where T : new()
        {
            string str = ins.getString(path);
            if (string.IsNullOrEmpty(str))
                return new T();

            return loadObjectFromJson<T>(str);
        }

        static public T loadObjectFromJson<T>(string str) where T : new()
        {
            T data;
            try
            {
                str = JsonCompress(str);
                data = LitJson.JsonMapper.ToObject<T>(str);
            }
            catch
            {
                str = EncryptUtil.AESDecrypt(str);
                data = LitJson.JsonMapper.ToObject<T>(str);
            }
            return data;
        }

        static public void saveObjectToJsonFile<T>(T data, string filepath,bool isEncrypt)
        {
            string str = LitJson.JsonMapper.ToJson(data);
            if(isEncrypt)
                str = EncryptUtil.AESEncrypt(str);
            else
                str = JsonFormat(str);

            string path = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            File.WriteAllText(filepath, str, new UTF8Encoding(false));
        }

        static string JsonFormat(string str)
        {
            StringBuilder sb = new StringBuilder();
            int space = 0;
            foreach (char c in str)
            {
                if (c == ',')
                {
                    sb.Append(c + "\n");
                    for (int i = 0; i < space; i++)
                        sb.Append("\t");
                }
                else if (c == '{' || c == '[')
                {
                    space++;
                    sb.Append(c + "\n");
                    for (int i = 0; i < space; i++)
                        sb.Append("\t");
                }
                else if (c == '}' || c == ']')
                {
                    space--;
                    sb.Append("\n");
                    for (int i = 0; i < space; i++)
                        sb.Append("\t");
                    sb.Append(c);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        static string JsonCompress(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c == '\n' || c == '\t')
                {
                    continue;
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public string getStreamingPath(bool endLine)
        {
            string platform = getRuntimePlatform();
            if (endLine)
                return streamingAssetsPath + "/" + platform + "/";
            else
                return streamingAssetsPath + "/" + platform;
        }

        public string getPresistentPath(bool endLine)
        {
            string platform = getRuntimePlatform();
            if (endLine)
                return persistentDataPath + "/" + platform + "/";
            else
                return persistentDataPath + "/" + platform;
        }

        public bool IsConfigExist()
        {
            string path = getPresistentPath(true) + AssetUpdater.Config_Name;
            return isFileExist(path);
        }

        public long GetSize(string fileName)
        {
            long size = 0;
            if (!File.Exists(fileName))
            {
                return 0;
            }

            using (FileStream fs = File.OpenRead(fileName))
            {
                size = fs.Length;
            }

            return size;
        }

        public string GetMD5FromString(string buf)
        {
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] value = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(buf));
            return BitConverter.ToString(value).Replace("-", string.Empty);
        }

        public string GetMd5HashFromFile(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                    return "";
                FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                GameDebug.LogError("GetMd5HashFromFile fail,error: " + e.Message);
            }
            return "";
        }

        /// <summary>
        /// 从文件读字符串
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string getString(string fileName)
        {
            //GameDebug.Log(fileName);
            if (!isFileExist(fileName))
            {
                return null;
            }
#if !UNITY_EDITOR && UNITY_ANDROID
            return getStringAndroid(fileName);
#else
            return File.ReadAllText(fileName, Encoding.UTF8);
#endif
        }
        public string getString(string path, string fileName)
        {
            if (!path.EndsWith("/")) path += "/";
            return getString(path + fileName);
        }
        public byte[] getBytes(string path, string fileName)
        {
            if (!path.EndsWith("/")) path += "/";
            return getBytes(path + fileName);
        }

        /// <summary>
        /// 从文件读二进制
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] getBytes(string fileName)
        {
            if (!isFileExist(fileName))
            {
                return null;
            }
#if !UNITY_EDITOR && UNITY_ANDROID
       
            return getBytesAndroid(fileName);
#else

            return File.ReadAllBytes(fileName);
#endif
        }
        public bool Move(string src, string target)
        {
            try
            {
                if (isFileExist(target))
                {
                    removeFile(target);
                }

                string path = Path.GetDirectoryName(target);
                createDirectory(path);
                File.Move(src, target);
                return true;

            }
            catch (IOException e)
            {
                GameDebug.LogError(e.ToString());
                return false;
            }
        }

        public void CopyFile_Bytes(string sourcesfile, string targetfile)
        {
            byte[] fileBytes = null;
            if (!isFileExist(sourcesfile))
            {
                return;
            }
#if !UNITY_EDITOR && UNITY_ANDROID
      
            fileBytes = getBytesAndroid(sourcesfile);
            writeBytes(targetfile,fileBytes);
            return ;
#else

            fileBytes = File.ReadAllBytes(sourcesfile);
            writeBytes(targetfile, fileBytes);
            return;
#endif
        }

        public void copyFile(string sourcepath, string targetpath)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            copyFileAndroid(sourcepath,targetpath);
#else
            copyNormal(sourcepath, targetpath);
#endif
        }

        private void copyNormal(string sourcepath, string targetpath)
        {
            try
            {
                string path = Path.GetDirectoryName(targetpath);
                //Debug.Log(path);
                createDirectory(path);
                removeFile(@targetpath);
                File.Copy(@sourcepath, @targetpath);
            }
            catch (Exception e)
            {
                GameDebug.LogError("copyFile fail. ");
                throw e;
            }
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool isFileExist(string filePath)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            return isFileExistsAndroid(filePath);
#else
            return File.Exists(filePath);
#endif
        }

        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool isDirectoryExist(string dir)
        {
            return Directory.Exists(dir);
        }

        public void copyDir(string srcDir, string tarDir)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            copyDirAndroid(srcDir, tarDir);
#else
            copyDirNormal(srcDir, tarDir);
#endif
        }
        private void copyDirNormal(string srcDir, string tarDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tarDir);

            if (!source.Exists)
            {
                return;
            }

            if (!target.Exists)
            {
                target.Create();
            }

            FileInfo[] files = source.GetFiles();
            DirectoryInfo[] dirs = source.GetDirectories();
            if (files.Length == 0 && dirs.Length == 0)
            {
                return;
            }
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta")) continue;
                File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
            }
            for (int j = 0; j < dirs.Length; j++)
            {
                copyDirNormal(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
            }
        }

        
        private string getLinuxPath(string path)
        {
#if UNITY_EDITOR
            return Regex.Replace(path, "\\\\", "/");
#else
        return path;
#endif
        }
        public void movePath(string oldPath, string newPath)
        {
            oldPath = getLinuxPath(oldPath);
            newPath = getLinuxPath(newPath);
            ForEachDirectory(oldPath, (path) =>
            {
                path = getLinuxPath(path);
                var p = path.Replace(oldPath, newPath);
                var dir = Path.GetDirectoryName(newPath);
                createDirectory(dir);
                //File.Move(path, p);
                CopyFile_Bytes(path, p);
            });
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool removeDirectory(string dir)
        {
            if (isDirectoryExist(dir))
            {
                Directory.Delete(dir, true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool removeFile(string file)
        {
            if (isFileExist(file))
            {
#if !UNITY_EDITOR
            if (file.IndexOf(streamingAssetsPath) == -1)
#endif
                {
                    File.Delete(file);
                    return true;
                }
            }
            return false;
        } 

        public bool writeBytes(string filePath, byte[] bytes)
        {
            try
            {
                string path = Path.GetDirectoryName(filePath);
                createDirectory(path);
                File.WriteAllBytes(filePath, bytes);
                return true;
            }
            catch (IOException e)
            {
                GameDebug.LogError("writeFIle fail. " + filePath);
                throw e;
            }
        }
        private void Write(FileStream fs, byte[] data)
        {
            fs.Write(data, 0, data.Length);
        }
        public bool writeFileStream(string path, List<byte[]> dataes)
        {
            createDirectory(Path.GetDirectoryName(path));
            using (FileStream fs = new FileStream(path, System.IO.FileMode.Append))
            {
                for (int i = 0; i < dataes.Count; ++i)
                    Write(fs, dataes[i]);
            }
            return true;
        }
        public bool writeFileStream(string dir, string filename, List<byte[]> dataes)
        {
            return writeFileStream(Path.Combine(dir, filename), dataes);
        }

        public void createDirectory(string path)
        {
            if (!isDirectoryExist(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void clearPath(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                return;
            }
            FileInfo[] files = info.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                files[i].Delete();
            }
            DirectoryInfo[] diries = info.GetDirectories();
            for (int j = 0; j < diries.Length; j++)
            {
                diries[j].Delete(true);
            }
        }

        public void ForEachDirectory(string path, Action<string> callBack)
        {
            ForEachDirectory(path, "*", callBack);
        }

        /// <summary>
        /// 遍历文件夹下所有文件。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="callBack"></param>
        public void ForEachDirectory(string path, string searchPattern, Action<string> callBack)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                return;
            }
            FileInfo[] files = info.GetFiles(searchPattern, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log(files[i].FullName);
                callBack(getLinuxPath(files[i].FullName));
            }

        }

        /// <summary>
        /// 获得目录下所有文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> getAllFileInPath(string path)
        {
            return getAllFileInPathWithSearchPattern(path, null);
        }

        /// <summary>
        /// 获得目录下所有后缀为{searchPattern}的文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public List<string> getAllFileInPathWithSearchPattern(string path, string searchPattern)
        {
            List<string> list = new List<string>();
            ForEachDirectory(path, searchPattern, (string file) =>
            {
                list.Add(file);
            });

            return list;
        }

        public string getRuntimePlatform()
        {           
            string pf = "";
#if UNITY_EDITOR
            pf = Setting.Get().buildTarget.ToString().ToLower();
#elif UNITY_ANDROID
            pf = BuildPlatform.Android.ToString().ToLower();
#elif UNITY_IOS
            pf = BuildPlatform.iOS.ToString().ToLower();
#endif
            return pf.ToLower();
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        private AndroidJavaClass _helper;
        private AndroidJavaClass helper
        {
            get
            {
                if (_helper == null) 
                    _helper = new AndroidJavaClass("com.yixun.tools.FileUtil");
                return _helper;
            }
        }

        private void copyFileAndroid(string path, string desPath)
        {
            if (path.IndexOf(streamingAssetsPath) > -1)
            {
                path = path.Replace(streamingAssetsPath + "/", "");
                if(path.EndsWith("/"))
                    path = path.Substring(0,path.Length-1);
            }
            else if (path.IndexOf(persistentDataPath) > -1)
            {
                copyNormal(path,desPath);
            }
            helper.CallStatic("copyFileFromAssets", path, desPath);
        }

        private void copyDirAndroid(string path, string desPath)
        {
            if (path.IndexOf(streamingAssetsPath) > -1)
            {
                path = path.Replace(streamingAssetsPath + "/", "");
                if(path.EndsWith("/"))
                    path = path.Substring(0,path.Length-1);
            }
            else if (path.IndexOf(persistentDataPath) > -1)
            {
                copyDirNormal(path,desPath);
            }
            helper.CallStatic("copyFilesFromAssets", path, desPath);
        }
        
        private byte[] getBytesAndroid(string path)
        {
            if (path.IndexOf(streamingAssetsPath) > -1)
            {              
                path = path.Replace(streamingAssetsPath + "/", "");
            }
            else if (path.IndexOf(persistentDataPath) > -1)
            {
                return File.ReadAllBytes(path);
            }
            return helper.CallStatic<byte[]>("getBytes", path);
        }
        private string getStringAndroid(string path)
        {
            if (path.IndexOf(streamingAssetsPath) > -1)
            {
                path = path.Replace(streamingAssetsPath + "/", "");
            }
            else if (path.IndexOf(persistentDataPath) > -1)
            {
                return File.ReadAllText(path);
            }
            return helper.CallStatic<string>("getString", path);
        }
        private bool isFileExistsAndroid(string path)
        {
            if(path.IndexOf(streamingAssetsPath) > -1)
            {
                path = path.Replace(streamingAssetsPath + "/", "");
            }
            else if(path.IndexOf(persistentDataPath) > -1)
            {
                return File.Exists(path);
            }
            return helper.CallStatic<bool>("isFileExists", path);
        }
#endif

    }
}
