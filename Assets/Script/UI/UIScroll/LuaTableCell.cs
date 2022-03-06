using UnityEngine;
using XLua;
using System;

[RequireComponent(typeof(LuaBinding))]
public class LuaTableCell : ScrollableCell
{
    LuaTable _target;
    private Action<LuaTable> _luaInit;
    private Action<LuaTable,int> _luaUpdate;
    private Action<LuaTable> _luaClean;

    public void Init(LuaTable cls)
    {
        Debug.Assert(cls != null, "LuaTableCell.Init target can't be null");
        try
        {
            _target = cls.Get<Func<LuaTable>>("create").Invoke();

            _target.Get("init", out _luaInit);
            _target.Get("update", out _luaUpdate);
            _target.Get("clean", out _luaClean);
            LuaBinding bind = GetComponent<LuaBinding>();
            if (bind)
            {
                bind.Init(_target);
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        _luaInit?.Invoke(_target);
    }

    public override void ConfigureCellData()
    {
        //lua table begin 1
        _luaUpdate?.Invoke(_target, DataIndex+1);
    }

    public override void CleanData()
    {
        _luaClean?.Invoke(_target);

        _luaInit = null;
        _luaUpdate = null;
        _luaClean = null;
        _target = null;
    }

   
}
