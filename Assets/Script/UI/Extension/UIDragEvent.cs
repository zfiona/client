using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;


public class UIDragEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Action<LuaTable, string, PointerEventData> _OnDrag;
    private Action<LuaTable, string, PointerEventData> _OnEndDrag;
    private Action<LuaTable, string, PointerEventData> _OnBeginDrag;
    private LuaTable self;

    public void Bind(LuaTable table)
    {
        self = table;
        self.Get("OnDrag", out _OnDrag);
        self.Get("OnEndDrag", out _OnEndDrag);
        self.Get("OnBeginDrag", out _OnBeginDrag);
    }

    public void UnBind()
    {
        self = null;
        _OnBeginDrag = null;
        _OnDrag = null;
        _OnEndDrag = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _OnDrag?.Invoke(self,name,eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _OnEndDrag?.Invoke(self, name, eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _OnBeginDrag?.Invoke(self, name, eventData);
    }

    void OnDestroy()
    {
        self = null;
        _OnBeginDrag = null;
        _OnDrag = null;
        _OnEndDrag = null;
                
    }

        
}
