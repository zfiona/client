using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LuaBinding))]
public class LuaBindingInspector : Editor
{
    private static GUIContent
        //insertContent = new GUIContent("+", "添加变量"),
        deleteContent = new GUIContent("-", "删除变量");
    private static GUILayoutOption buttonWidth = GUILayout.MaxWidth(20f);
    private static GUIContent pointContent = GUIContent.none;
    //private SerializedProperty KeyMap;
    private SerializedProperty mVariables;

    private SerializedObject binding;

    LuaBinding mLua;
    GameObject mGo;
   // string mLastLuaFile;

    bool mAdd = false;
    void OnEnable()
    {
        binding = new SerializedObject(target);
        mVariables = binding.FindProperty("mVariables");
        //KeyMap = binding.FindProperty("KeyMap");
    }

    void OnDisable()
    {
        binding = null;
        mVariables = null;
        //KeyMap = null;
    }

    /// <summary>
    /// 获得对象加载了多少脚本
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public Component[] GetComponents(GameObject go)
    {
        return go.GetComponents<Component>();
    }
    /// <summary>
    /// 获得对象加载了多少脚本
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public Component[] GetComponents(Component go)
    {
        return go.GetComponents<Component>();
    }
    /// <summary>
    /// 获取脚本列表名
    /// </summary>
    /// <param name="comps"></param>
    /// <param name="type"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public string[] GetTypeNames(Component[] comps, string type, out int index)
    {
        index = 0;
        string[] names = new string[comps.Length + 1];
        names[0] = "GameObject";
        for (int i = 0; i < comps.Length; i++)
        {
            try
            {
                string tp = comps[i].GetType().ToString();
                int find = tp.LastIndexOf(".");
                if (find > -1) tp = tp.Substring(find + 1);
                names[i + 1] = tp;
                if (index == 0 && !string.IsNullOrEmpty(type) && type.Equals(tp)) index = i + 1;
            }
            catch{}           
        }
        return names;
    }

    /// <summary>
    /// 选择文件
    /// </summary>
    /// <param name="file"></param>
    void SelectLuaFile(string file)
    {
        binding.Update();
        SerializedProperty sp = binding.FindProperty("mPath");
        sp.stringValue = file;
        binding.ApplyModifiedProperties();
        EditorTools.SetDirty(binding.targetObject);
    }

    public override void OnInspectorGUI()
    {
        if (mVariables == null)
            mVariables = binding.FindProperty("mVariables");

        mLua = target as LuaBinding;
        EditorTools.DrawSeparator();
        BindAttrValue();
        //EditorTools.DrawSeparator();
        //BindConstValue();
        binding.ApplyModifiedProperties();
    }

    private bool KeyIsNumber(string key)
    {
        var reg = new System.Text.RegularExpressions.Regex("^[0-9]+$");//判断是不是数据，要不是就表示没有选择，则从隐藏域里读出来
        System.Text.RegularExpressions.Match ma = reg.Match(key);
        if (ma.Success) EditorUtility.DisplayDialog("错误", "变量名不能为纯数字", "确定");
        return ma.Success;
    }
    private bool KeyHasEmpty(string key)
    {
        var reg = new System.Text.RegularExpressions.Regex("\\s+");
        System.Text.RegularExpressions.Match ma = reg.Match(key);
        if (ma.Success) EditorUtility.DisplayDialog("错误", "变量名不能有空格", "确定");
        return ma.Success;
    }

    private bool KeyIsExit(string key)
    {
        for (int i = 0; i < mVariables.arraySize; i++)
        {
            var val = mLua.mVariables[i];
            if(val.name == key)
            {
                EditorUtility.DisplayDialog("错误", "变量名已存在，位置：" + (i+1), "确定");
                return true;
            }             
        }
        return false;
    }

    /// <summary>
    /// 绑定变量
    /// </summary>

    public void BindAttrValue()
    {
        binding.Update();
        string reName = "";
        bool changed = false;
        List<LuaVariable> vals = new List<LuaVariable>(mLua.mVariables);

        if (EditorTools.DrawHeader("绑定变量"))
        {
            #region 已有变量

            List<LuaVariable> removeList = new List<LuaVariable>();//要删除的变量
            for (int i = 0; i < mVariables.arraySize; i++)
            {
                if (i % 2 == 0) GUI.backgroundColor = Color.cyan;
                else GUI.backgroundColor = Color.white;
                var val = mLua.mVariables[i];

                if (val.val == null)
                {
                    removeList.Add(val);
                    changed = true;
                    continue;
                }
                //更改绑定变量时 名字主键一起改变
                if(val.variable.name != val.name)
                {
                    val.Reset();
                }
                EditorTools.BeginContents();
                GUILayout.Space(5);
                GUI.backgroundColor = Color.white;
                //取出列表中的变量
                SerializedProperty variable = mVariables.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                //类型
                SerializedProperty mType = variable.FindPropertyRelative("type");
                //对象
                SerializedProperty mVal = variable.FindPropertyRelative("variable");
                var comps = GetComponents(val.gameObject);

                int index;
                var names = GetTypeNames(comps, mType.stringValue, out index);
                int choice = 0;
                choice = EditorGUILayout.Popup(index, names, GUILayout.Width(100));
                if (choice > 0 && choice != index)
                {
                    mType.stringValue = names[choice];
                }
                else if (choice != index)
                {
                    mType.stringValue = names[0];
                }
                EditorTools.DrawPadding();
                GUILayout.Space(-16);

                //描绘变量对象
                EditorGUILayout.PropertyField(mVal, pointContent);             
                GUILayout.Space(6);

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                var style = new GUIContent("变量名", "lua中访问的变量名");
                GUILayout.Label(style, GUILayout.Width(40));

                var mName = variable.FindPropertyRelative("name");
                EditorGUILayout.PropertyField(mName, pointContent);

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth))
                {
                    //mUlua.mVariables.Remove(val);
                    mVariables.DeleteArrayElementAtIndex(i);

                    EditorTools.SetDirty(mLua.gameObject);
                    EditorUtility.UnloadUnusedAssetsImmediate();
                    break;
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
                if (i % 2 == 0) GUI.backgroundColor = Color.cyan;
                else GUI.backgroundColor = Color.white;
                EditorTools.EndContents();
            }
            GUI.backgroundColor = Color.white;
            vals = new List<LuaVariable>(mLua.mVariables);
            for (int j = 0; j < removeList.Count; j++)
            {
                vals.Remove(removeList[j]);
                removeList[j] = null;
            }
            removeList.Clear();

            mLua.mVariables = vals.ToArray();
            for (int i = 0; i < vals.Count; i++)
            {
                vals[i] = null;
            }
            vals.Clear();

            if (!string.IsNullOrEmpty(reName))
            {
                EditorTools.BeginContents();

                GUILayout.Space(6f);
                reName = reName + "变量重名了……";

                EditorGUILayout.HelpBox(reName, MessageType.Warning, true);
                GUILayout.Space(6f);

                EditorTools.EndContents();
            }
            #endregion

            #region 新变量
            EditorTools.BeginContents();
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("新变量", GUILayout.Width(40));

            mGo = EditorGUILayout.ObjectField(mGo, typeof(GameObject), true) as GameObject;
            GUILayout.EndHorizontal();
            if (mAdd)
            {
                int index = mVariables.arraySize - 1;
                var variable = mVariables.GetArrayElementAtIndex(index);
                mAdd = false;
                var mName = variable.FindPropertyRelative("name");
                string name = mName.stringValue;
                mName.stringValue = name;
            }
            if (mGo)
            {
                string _name = mGo.name;
                if (KeyHasEmpty(_name) || KeyIsNumber(_name) || KeyIsExit(_name))
                {
                    mGo = null;
                }
                else
                {
                    var val = new LuaVariable();
                    val.name = _name;
                    val.type = "GameObject";
                    val.variable = mGo;
                    vals = new List<LuaVariable>(mLua.mVariables);
                    vals.Add(val);

                    mLua.mVariables = vals.ToArray();
                    for (int i = 0; i < vals.Count; i++)
                    {
                        vals[i] = null;
                    }
                    vals.Clear();
                    EditorTools.SetDirty(mLua.gameObject);
                    mAdd = true;
                    mGo = null;
                }               
            }
            GUILayout.Space(6f);
            EditorTools.EndContents();
            #endregion

        }
        if (changed)
        {
            EditorTools.SetDirty(mLua.gameObject);
        }
    }
    /// <summary>
    /// 绑定常量
    /// </summary>
/*
    public void BindConstValue()
    {
        List<string> list = new List<string>();
        string reName = "";
        for (int j = mUlua.KeyMap.Count - 1; j >= 0; j--)
        {
            var val = mUlua.KeyMap[j];

            if (string.IsNullOrEmpty(val.Key))
            {
                mUlua.KeyMap.RemoveAt(j);
                continue;
            }
          

            if (list.IndexOf(val.Key) == -1) list.Add(val.Key);
            else reName += "(" + val.Key + ")" + " ";
        }
        
        if (EditorTools.DrawHeader("绑定常量"))
        {
            EditorTools.BeginContents();


            for (int i = 0; i < KeyMap.arraySize; i++)
            {
                if (i % 2 == 0) GUI.backgroundColor = Color.cyan;
                else GUI.backgroundColor = Color.magenta;
                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                SerializedProperty map = KeyMap.GetArrayElementAtIndex(i);

                var _type = map.FindPropertyRelative("type");
                EditorGUILayout.PropertyField(_type, pointContent);
                var _key = map.FindPropertyRelative("Key");
                var style = new GUIContent("K", "lua中访问的key");
                GUILayout.Label(style);

                EditorGUILayout.PropertyField(_key, pointContent);
                style = new GUIContent("V", "lua中访问的Value");
                GUILayout.Label(style);
                var type = (LuaKeyValue.ValTypes)_type.enumValueIndex;
                if (LuaKeyValue.ValTypes.Int == type)
                {
                    EditorGUILayout.PropertyField(map.FindPropertyRelative("Int"), pointContent);
                }
                else if (type == LuaKeyValue.ValTypes.Float)
                {
                    EditorGUILayout.PropertyField(map.FindPropertyRelative("Float"), pointContent);
                }
                else if (type == LuaKeyValue.ValTypes.String)
                {
                    EditorGUILayout.PropertyField(map.FindPropertyRelative("Str"), pointContent);
                }

                if (GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, buttonWidth))
                {
                    KeyMap.InsertArrayElementAtIndex(i);
                }
                if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth))
                {
                    KeyMap.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(5);

            GUI.backgroundColor = Color.white;
            if (!string.IsNullOrEmpty(reName))
            {
                EditorTools.BeginContents();

                GUILayout.Space(6f);
                reName = reName + "变量重名了……";

                EditorGUILayout.HelpBox(reName, MessageType.Warning, true);
                GUILayout.Space(6f);

                EditorTools.EndContents();
            }

            if (GUILayout.Button("添加常量", GUILayout.MinWidth(100)))
            {
                mUlua.KeyMap.Add(new LuaKeyValue());
            }

            GUILayout.Space(5);

            EditorTools.EndContents();
        }
    }*/
}