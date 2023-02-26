using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using XLua;

public class UIPointEvent : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IPointerExitHandler
{
    private Action<LuaTable,string, PointerEventData> _OnPointerEnter;
    private Action<LuaTable,string, PointerEventData> _OnPointerDown;
    private Action<LuaTable, string, PointerEventData> _OnPointerClick;
    private Action<LuaTable, string, PointerEventData> _OnPointerUp;
    private Action<LuaTable, string, PointerEventData> _OnPointerExit;

    private LuaTable self;


    public void Bind(LuaTable table)
    {
        self = table;
        self.Get("OnPointerEnter", out _OnPointerEnter);
        self.Get("OnPointerDown", out _OnPointerDown);
        self.Get("OnPointerClick", out _OnPointerClick);
        self.Get("OnPointerUp", out _OnPointerUp);
        self.Get("OnPointerExit", out _OnPointerExit);
    }

    public void UnBind()
    {
        self = null;
        _OnPointerDown = null;
        _OnPointerClick = null;
        _OnPointerEnter = null;
        _OnPointerExit = null;
        _OnPointerUp = null;
    }

    public void OnPointerClick( PointerEventData eventData)
    {
        this._OnPointerClick?.Invoke(self,name,eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this._OnPointerDown?.Invoke(self, name, eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this._OnPointerEnter?.Invoke(self, name, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this._OnPointerExit?.Invoke(self, name, eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this._OnPointerUp?.Invoke(self, name, eventData);
    }

    void OnDestroy()
    {
        self = null;
        _OnPointerDown = null;
        _OnPointerClick = null;
        _OnPointerEnter = null;
        _OnPointerExit = null;
        _OnPointerUp = null;
    }
}
