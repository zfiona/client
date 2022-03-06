using System;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[ExecuteInEditMode]

public class LuaBinding : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    public LuaVariable[] mVariables = new LuaVariable[] { };

    private Action<LuaTable, Button, string> _luaBtnClick;
    private Action<LuaTable, Toggle, string, bool> _luaToggleChange;
    private Action<LuaTable, InputField, string, string> _luaInputEnd;
    private Action<LuaTable, InputField, string, string> _luaInputChange;
    private Action _luaBtnSound;
    private Action _luaToggleSound;

    public void Init(LuaTable target)
    {
        if (target == null) return;
        for (int i = 0; i < mVariables.Length; i++)
        {
            var val = mVariables[i];
            target.Set(val.name, val.val);
        }
        target.Set("gameObject", gameObject);
        target.Set("transform", gameObject.transform);

        target.Get("onClick", out _luaBtnClick);
        target.Get("onValueChange", out _luaToggleChange);
        target.Get("onClickSound", out _luaBtnSound);
        target.Get("onValueChangeSound", out _luaToggleSound);
        target.Get("onInputEnd", out _luaInputEnd);
        target.Get("onInputChange", out _luaInputChange);

        if (_luaBtnClick == null && _luaToggleChange == null) return;
        for (int i = 0; i < mVariables.Length; i++)
        {
            var val = mVariables[i];
            string name = val.name;
            if (_luaBtnClick != null && val.type == "Button")
            {
                Button btn = val.val as Button;
                btn.onClick.AddListener(()=>
                {
                    _luaBtnClick?.Invoke(target, btn, name);
                    _luaBtnSound?.Invoke();
                });
            }
            if (_luaToggleChange != null && val.type == "Toggle")
            {
                Toggle toggle = val.val as Toggle;
                toggle.onValueChanged.AddListener((ret) =>
                {
                    _luaToggleChange?.Invoke(target, toggle, name, ret);
                    _luaToggleSound?.Invoke();
                });
            }
            if (_luaInputEnd != null && val.type == "InputField")
            {
                InputField input = val.val as InputField;
                input.onEndEdit.AddListener((ret) =>
                {
                    _luaInputEnd?.Invoke(target, input, name, ret);
                });

                input.onValueChanged.AddListener((ret) =>
                {
                    _luaInputChange?.Invoke(target, input, name, ret);
                });

            }
#if !UNITY_EDITOR
            val.variable = null;
            val.type = null;
            val.name = null;
            mVariables[i] = null;
        }
        mVariables = new LuaVariable[] { };
#else
        }
#endif
    }

    
    void OnDestroy()
    {
        _luaBtnClick = null;
        _luaToggleChange = null;
        _luaBtnSound = null;
        _luaToggleSound = null;
        _luaInputEnd = null;
        _luaInputChange = null;
    }
}