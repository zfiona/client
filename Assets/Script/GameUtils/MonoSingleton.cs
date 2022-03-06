/// <summary>
/// Generic Mono singleton.
/// </summary>
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{

    public static GameObject go { get; set; }
    private static T _Instance = null;
    public static T Instance
    {
        get
        {
            if (null == _Instance)
            {
                if (null == go)
                {
                    go = GameObject.Find("DDOLGameObject");
                    if (null == go)
                    {
                        go = new GameObject("DDOLGameObject");
                    }
                }
                DontDestroyOnLoad(go);
                _Instance = go.GetScript<T>();
            }
            return _Instance;
        }
    }

    public void OnApplicationQuit()
    {
        _Instance = null;
    }

    public virtual void Init() { }

    private void Awake()
    {
        Init();
    }
}