using UnityEngine;
using XLua;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(ScrollableAreaController))]
public class LuaTableDataSource : MonoBehaviour
{
    private LuaTableCell mTableCell;
    private ScrollableAreaController mScrollControl;

    void PreInit()
    {
        mScrollControl = GetComponent<ScrollableAreaController>();
        mTableCell = (LuaTableCell)mScrollControl.cellPrefab;
        Debug.Assert(mTableCell, "LuaTableCell can't be null");
        mTableCell.gameObject.SetActive(false);
    }
    /// <summary>
    /// cell lua class
    /// </summary>
    /// <param name="cellTarget"></param>
    public void Init(LuaTable cellTarget)
    {
        PreInit();
        Debug.Assert(cellTarget != null, "Call LuaTableDataSource.Init frist!");
        mScrollControl.Init(cellTarget);
    }

    /// <summary>
    /// lua数据层和c#逻辑层分离
    /// </summary>
    /// <param name="count"></param>
    public void Refresh(int count)
    {
        mScrollControl.InitializeWithData(count);
    }
    
    public void AddPageCallback(Action topAction,Action bottomAction)
    {
        mScrollControl.AddPageCallback(topAction, bottomAction);
    }
}
