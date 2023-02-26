using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReplaceFontWindow : EditorWindow
{
#if UNITY_EDITOR
    [MenuItem("Tools/更换字体")]
    public static void Open()
    {
        GetWindow(typeof(ReplaceFontWindow)); //开启新窗口
    }

    Font toChange; //替换字体（面板赋值）
    FontStyle toFontStyle;//字体样式（面板赋值）
    static Font toChangeFont;
    static FontStyle toChangeFontStyle;

    //

    void OnGUI()
    {
        toChange = (Font)EditorGUILayout.ObjectField(toChange, typeof(Font), true, GUILayout.MinWidth(100f));
        toFontStyle = (FontStyle)EditorGUILayout.EnumPopup(toFontStyle, GUILayout.MinWidth(100f));
        //赋值
        toChangeFont = toChange;
        toChangeFontStyle = toFontStyle;

        //按钮
        if (GUILayout.Button("更换"))
        {
            var prefabObjs =  GetAllPrefabByAssetDatabase();
            if (!toChangeFont)
            {
                Debug.Log("NO Font");
                return;
            }

            for(int i=0;i<prefabObjs.Count;i++)
            {
                var prefab = prefabObjs[i];
                SetFonts(prefab.transform);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Font replacement succeeded");
        }
    }
    public List<GameObject> GetAllPrefabByAssetDatabase()
    {
        List<GameObject> _prefabList = new List<GameObject>();
        string[] _guids = AssetDatabase.FindAssets("t:Prefab");
        string _prefabPath = "";
        GameObject _prefab;
        foreach (var _guid in _guids)
        {
            _prefabPath = AssetDatabase.GUIDToAssetPath(_guid);
            if (!_prefabPath.StartsWith("Assets/Art/hall/prefab"))
            {
                continue;
            }
            _prefab = AssetDatabase.LoadAssetAtPath(_prefabPath, typeof(GameObject)) as GameObject;
            _prefabList.Add(_prefab);
        }
        return _prefabList;
    }

    Transform childObj;
    public void SetFonts(Transform obj)
    {
        Text rootText = obj.GetComponent<Text>(); //如果预制体根组件为text
        if (rootText)
        {
            //将对象放到撤销记录中，不加此代码 无法还原替换前的操作
            Undo.RecordObject(rootText, rootText.name);
            rootText.font = toChange;
            rootText.fontStyle = toChangeFontStyle;
            //刷新下
            EditorUtility.SetDirty(childObj);
        }
        for (int i = 0; i < obj.childCount; i++)
        {
            childObj = obj.GetChild(i);
            Text t = childObj.GetComponent<Text>();
            if (t)
            {
                //将对象放到撤销记录中，不加此代码 无法还原替换前的操作
                Undo.RecordObject(t, t.name);
                t.font = toChange;
                t.fontStyle = toChangeFontStyle;
                //刷新下
                EditorUtility.SetDirty(childObj);
            }

            //递归查询
            if (childObj.childCount > 0)
            {
                SetFonts(childObj);
            }
        }
    }
#endif
}