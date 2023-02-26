using System;
using UnityEngine;
using XLua;

public class UIUpdateEvent : MonoBehaviour
{
    private Action<LuaTable> luaUpdate;
    private Action<LuaTable> luaFixedUpdate;
    private LuaTable self;
    public void Bind(LuaTable table)
    {
        self = table;
        self.Get("Update", out luaUpdate);
        self.Get("FixedUpdate", out luaFixedUpdate);
    }

    public void UnBind()
    {
        self = null;
        luaUpdate = null;
        luaFixedUpdate = null;
    }

    private void Update()
    {
        luaUpdate?.Invoke(self);
    }

    private void FixedUpdate()
    {
        luaFixedUpdate?.Invoke(self);
    }

    private void OnDestroy()
    {
        UnBind();
    }
}