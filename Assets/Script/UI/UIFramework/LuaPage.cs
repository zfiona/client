using XLua;
using UnityEngine;
using System;

public class LuaPage : Page
{
    LuaTable _target;
    private Action<LuaTable> _luaAwake;
    private Action<LuaTable> _luaRefresh;
    private Action<LuaTable, bool> _luaHide;

    public LuaPage(LuaTable target, string uiPath, UIType type,UIAnim pop) :
        base(uiPath,type, pop)
    {
        _target = target;
        target.Get("Awake", out _luaAwake);
        target.Get("Refresh", out _luaRefresh);
        target.Get("Hide", out _luaHide);
    }

    public override void Awake(GameObject go)
    {
        if (_target == null)
            return;

        LuaBinding bind = go.GetComponent<LuaBinding>();
        if (bind)
            bind.Init(_target);
        _luaAwake?.Invoke(_target);
    }

    public override void Refresh(object data)
    {
        _luaRefresh?.Invoke(_target);
    }

    public override void Hide(bool isRemove)
    {
        _luaHide?.Invoke(_target, isRemove);
        base.Hide(isRemove);
        if (isRemove)
            OnDestroy();
    }

    private void OnDestroy()
    {
        _luaAwake = null;
        _luaRefresh = null;
        _luaHide = null;
        _target = null;
    }
}