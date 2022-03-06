using UnityEngine;

[System.Serializable]
/// <summary>
/// Lua变量
/// </summary>
public class LuaVariable
{
    private GameObject _go;
    public string name;
    public Object variable;
    public string type;
    public LuaVariable()
    {
    }
    public Object val
    {
        get
        {
            if (isGameObject)
            {
                return gameObject;
            }
            else
            {
                if (gameObject)
                {
                    return gameObject.GetComponent(type);
                }
                return null;
            }
        }
    }
    public bool isGameObject
    {
        get
        {
            return type == "GameObject";
        }
    }
    public GameObject gameObject
    {
        get
        {
            if (_go) return _go;
            if (isGameObject) _go = variable as GameObject;
            else
            {
                if (variable == null) return null;
                if (variable.GetType() == typeof(GameObject))
                {
                    _go = (GameObject)variable;
                    return _go;
                }
                try
                {
                    var v = ((MonoBehaviour)variable);
                    if (v)
                        _go = v.gameObject;
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            return _go;
        }
    }

    public void Reset()
    {
        name = variable.name;
        _go = variable as GameObject;
        type = "GameObject";
    }
}
