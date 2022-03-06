using UnityEngine;
// 扩展GameObject方法
public static class GameObjectUtils
{
    /// <summary>
    /// 按名称在自身和子物体中查找
    /// </summary>
    /// <param name="childName"></param>
    /// <param name="recursively">
    /// 递归查找的深度。
    /// 如果小于0，则搜索整个子物体树。
    /// 如果为0，则行为和FindChild相同。
    /// </param>
    /// <returns></returns>
    public static Transform SearchChild(this Transform obj, string childName, int recursivelyDepth = -1)
    {
        if (string.IsNullOrEmpty(childName))
            return null;

        return doSearchChild(obj, childName, recursivelyDepth);
    }

    private static Transform doSearchChild(Transform obj, string childName, int recursivelyDepth)
    {
        Transform child = obj.Find(childName);
        if (null != child)
            return child;

        if (recursivelyDepth != 0)
        {
            if (recursivelyDepth > 0)
                recursivelyDepth--;

            foreach (Transform t in obj.transform)
            {
                child = doSearchChild(t, childName, recursivelyDepth);
                if (child != null)
                    return child;
            }
        }

        return null;
    }


    // 获取对象组建
    public static T GetScript<T>(this GameObject go) where T : Component
    {
        T t = (T)go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }

    // 获取对象组建
    public static void DelScript<T>(this GameObject go) where T : Component
    {
        T t = (T)go.GetComponent<T>();
        if (t != null)
        {
            GameObject.Destroy(t);
        }
    }
  
    /// <summary>
    /// 遍历删除子对象
    /// </summary>
    /// <param name="o"></param>
    public static void DestroyAllChild(this GameObject o)
    {
        if (null != o)
        {          
            for (int i = o.transform.childCount -1; i >=0 ; i--)
            {
                GameObject.DestroyImmediate(o.transform.GetChild(i).gameObject);
            }
        }
    }
    /// <summary>
    /// 遍历删除子对象组件
    /// </summary>
    public static void DeleteAllScript<T>(this GameObject go,bool deleteSelf = true) where T : Component
    {
        if (null != go)
        {
            if(go.transform.childCount == 0)
            {
                GameObject.DestroyImmediate(go.GetComponent<T>());
                return;
            }
            if (deleteSelf)
            {
                T[] tArrary = go.GetComponentsInChildren<T>();
                for (int i = 0; i < tArrary.Length; ++i)
                {
                    GameObject.DestroyImmediate(tArrary[i]);
                }
            }
            else
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    T t = go.transform.GetChild(i).GetComponent<T>();
                    if (t != null)
                        GameObject.DestroyImmediate(t);
                }
            }
                  
        }
    }
    /// 遍历设置子对象组件
    public static void SetAllChildScript<T>(this GameObject go, bool deleteSelf = true) where T : Component
    {
        if (null != go)
        {           
            if (deleteSelf)
            {
                go.GetScript<T>();
            }
            
            for (int i = 0; i < go.transform.childCount; i++)
            {
                go.transform.GetChild(i).gameObject.GetScript<T>();
            }           
        }
    }
    public static T TryGetComponent<T>(this Transform trans) where T : Component
    {
        T t = trans.gameObject.GetComponent<T>();
        if (t == null)
            t = trans.gameObject.AddComponent<T>();
        return t;
    }
    public static void DestroyAllChild(this Transform tran)
    {
        for (int i = tran.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(tran.GetChild(i).gameObject);
        }
    }
    public static void DestroySelf(this GameObject o,bool imme = true)
    {
        if (null != o)
        {
            if (imme)
                GameObject.DestroyImmediate(o);
            else
                GameObject.Destroy(o);
        }
    }

    public static T TryGetComponent<T>(this GameObject o) where T:Component
    {
        T t = o.GetComponent<T>();
        if (t == null)
            t = o.AddComponent<T>();
        return t;
    }
	
}
