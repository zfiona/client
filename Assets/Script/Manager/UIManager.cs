using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class UIManager 
{
    private static Dictionary<string, Page> allPages = new Dictionary<string, Page>();

    public static void CreatePage(LuaTable target,string path, UIType type, UIAnim pop)
    {
        string pageName = target.Get<string>("NAME");
        if (!allPages.ContainsKey(pageName))
        {
            LuaPage page = new LuaPage(target, path, type, pop);
            allPages.Add(pageName, page);
            page.uiName = pageName;
        }
    }

    public static bool IsPageActive(string pageName)
    {
        if(!allPages.ContainsKey(pageName))
            return false;
        return allPages[pageName].isActive();
    }

    public static void ShowPage(string pageName)
    {
        if (string.IsNullOrEmpty(pageName) )
        {
            Debug.LogError("[UI] show page error with :" + pageName + " maybe null instance.");
            return;
        }

        Page page = allPages[pageName];
        if(page == null)
        {
            Debug.LogError("[UI] create page first with :" + pageName);
        }

        ShowPage(page, null);
    }

    [BlackList]
    public static void ShowPage(Page page, object pageData)
    {        
        if (page.isActive())
            page.Refresh(pageData);
        else
            page.Show();
    }

    public static Page GetPage(string pageName)
    {
        if (allPages.ContainsKey(pageName))
            return allPages[pageName];
        else
            return null;
    }

    /// <summary>
    /// Remove target page
    /// </summary>
    public static void RemovePage(string pageName)
    {
        if (allPages != null && allPages.ContainsKey(pageName))
        {
            Page target = allPages[pageName];
            Root.Instance.StartCoroutine(IRemovePage(target));
        }
        else
        {
            GameDebug.LogYellow(pageName + " havnt show yet!");
        }
    }

    private static IEnumerator IRemovePage(Page target)
    {
        yield return new WaitForEndOfFrame();

        target.Hide(true);
        allPages.Remove(target.uiName);
    }

    /// <summary>
    /// Close target page
    /// </summary>
    public static void ClosePage(Page target)
    {
        if (target == null) return;
        target.Hide();
    }

    public static void ClosePage(string pageName)
    {
        if (allPages != null && allPages.ContainsKey(pageName))
        {
            ClosePage(allPages[pageName]);
        }
        else
        {
            GameDebug.LogYellow(pageName + " havnt show yet!");
        }
    }

    public static void CloseAll()
    {
        foreach(var page in allPages)
            ClosePage(page.Value);
    }
}
