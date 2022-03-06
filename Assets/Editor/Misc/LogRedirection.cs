using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

static class LogRedirection
{
    private static void RunCmd(string command, string argument)
    {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command);
        info.Arguments = argument;
        info.CreateNoWindow = false;
        info.ErrorDialog = true;
        info.UseShellExecute = true;

        if (info.UseShellExecute)
        {
            info.RedirectStandardOutput = false;
            info.RedirectStandardError = false;
            info.RedirectStandardInput = false;
        }
        else
        {
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }

        try
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

            if (!info.UseShellExecute)
            {
                UnityEngine.Debug.Log(process.StandardOutput);
                UnityEngine.Debug.Log(process.StandardError);
            }

            process.WaitForExit();
            process.Close();
        }
        catch
        {
            UnityEngine.Debug.LogError("设置sublime路径为环境变量,并重启");
        }
    }


    private static readonly Regex s_LogRegex = new Regex(@"\s*([/.\w]+)\:(\d+)\:");
    [OnOpenAsset(0)]
    private static bool OnOpenAsset(int instanceId, int line)
    {
        string selectedStackTrace = GetSelectedStackTrace();
        if (string.IsNullOrEmpty(selectedStackTrace))
        {
            return false;
        }

        Match match = s_LogRegex.Match(selectedStackTrace);
        if (!match.Success)
        {
            return false;
        }

        string luaPath = match.Groups[1].Value.Replace(".", "/");
        //GameDebug.Log(luaPath);
        string lualine = match.Groups[2].Value;
        string gcmd = "  " + Application.dataPath + "/../lua/" + luaPath + ".lua:" + lualine;
        RunCmd("subl", gcmd);
        return true;
    }

    private static string GetSelectedStackTrace()
    {
        Assembly editorWindowAssembly = typeof(EditorWindow).Assembly;
        if (editorWindowAssembly == null)
        {
            return null;
        }

        System.Type consoleWindowType = editorWindowAssembly.GetType("UnityEditor.ConsoleWindow");
        if (consoleWindowType == null)
        {
            return null;
        }

        FieldInfo consoleWindowFieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
        if (consoleWindowFieldInfo == null)
        {
            return null;
        }

        EditorWindow consoleWindow = consoleWindowFieldInfo.GetValue(null) as EditorWindow;
        if (consoleWindow == null)
        {
            return null;
        }

        if (consoleWindow != EditorWindow.focusedWindow)
        {
            return null;
        }

        FieldInfo activeTextFieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
        if (activeTextFieldInfo == null)
        {
            return null;
        }

        return (string)activeTextFieldInfo.GetValue(consoleWindow);
    }
}