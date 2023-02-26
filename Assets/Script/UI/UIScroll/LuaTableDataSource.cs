using UnityEngine;
using XLua;
using System;

[RequireComponent(typeof(ScrollableAreaController))]
public class LuaTableDataSource : MonoBehaviour
{
    private LuaTableCell mTableCell;
    private ScrollableAreaController mControl;

    void PreInit()
    {
        mControl = GetComponent<ScrollableAreaController>();
        mTableCell = (LuaTableCell)mControl.cellPrefab;
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
        mControl.Init(cellTarget);
    }

    /// <summary>
    /// lua数据层和c#逻辑层分离
    /// </summary>
    /// <param name="count">数组个数</param>
    /// <param name="index">初始显示第几个元素</param>
    public void Refresh(int count,int index = 0)
    {
        mControl.InitializeWithData(count, index);
    }

    public void RefreshImmediate(int count, int index = 0)
    {
        mControl.RefreshDataImmediate(count, index);
    }

    public int GetCurIndex()
    {
        return mControl.GetCurIndex();
    }

    public void AddPageCallback(Action topAction,Action bottomAction)
    {
        mControl.AddPageCallback(topAction, bottomAction);
    }
}
