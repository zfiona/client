using UnityEngine;
using UnityEngine.EventSystems;
using System;
using XLua;

public class UICardEvent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerExitHandler
{
    private LuaTable self;
    private Action<LuaTable> _onClick;
    private Action<LuaTable> _onDragEnd;

    public bool active = false;
    public bool isDraging = false;
    private int siblingIndex = 0;
    private RectTransform mRect;
    private Vector3 mStartlPosition;
    private Vector3 mStartAngle;
    private Vector2 mStartAnchor;

    public void Bind(LuaTable target)
    {
        mRect = transform as RectTransform;
        self = target;
        target.Get("onClick", out _onClick);
        target.Get("onDragEnd", out _onDragEnd);
    }

    //drag--click--endDrag--exit
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!active) return;
        if (isDraging) return;
        //GameDebug.LogGreen("OnPointerClick");
        _onClick?.Invoke(self);
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (!active) return;
        //GameDebug.LogGreen("OnBeginDrag");
        isDraging = true;
        siblingIndex = mRect.GetSiblingIndex();
        mRect.SetAsLastSibling();

        mStartlPosition = mRect.position;
        mStartAngle = mRect.localEulerAngles;
        mStartAnchor = mRect.anchoredPosition;
    }

    public void OnDrag(PointerEventData data)
    {
        if (!active || !isDraging) return;
        if (data.position.x > Screen.width || data.position.x < 0 || data.position.y > Screen.height || data.position.y < 0)
        {
            OnEndDrag(data);
            return;
        }
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(mRect, data.position, data.enterEventCamera, out Vector3 mousePosition))
        {
            mRect.position = mousePosition;
            mRect.localEulerAngles = Vector3.forward * (float)(Screen.width * 0.5 - data.position.x) * 0.02f;
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (!active || !isDraging) return;
        //GameDebug.LogGreen("OnEndDrag");
        isDraging = false;
        if (mRect.anchoredPosition.y - mStartAnchor.y > mRect.sizeDelta.y * 0.5f)
            _onDragEnd?.Invoke(self);
        else
            Restore();
    }

    //异常退出
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!active) return;
        if (isDraging)
        {
            //GameDebug.LogGreen("OnPointerExit");
            //Restore();
        }
    }

    public void Restore()
    {
        //GameDebug.LogGreen("Restore");
        isDraging = false;
        mRect.position = mStartlPosition;
        mRect.localEulerAngles = mStartAngle;
        mRect.SetSiblingIndex(siblingIndex);
    }

    private void OnDestroy()
    {
        self = null;
        _onClick = null;
        _onDragEnd = null;
    }

}