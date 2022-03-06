using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;


    public class UIDragEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private Action<LuaTable> _OnDrag;
        private Action<LuaTable> _OnEndDrag;
        private Action<LuaTable> _OnBeginDrag;
        private LuaTable self;

        public void Bind(LuaTable table)
        {
            self = table;
            self.Get("OnDrag", out _OnDrag);
            self.Get("OnEndDrag", out _OnEndDrag);
            self.Get("OnBeginDrag", out _OnBeginDrag);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _OnDrag?.Invoke(self);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _OnEndDrag?.Invoke(self);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _OnBeginDrag?.Invoke(self);
        }

        void OnDestroy()
        {
            self = null;
            _OnBeginDrag = null;
            _OnDrag = null;
            _OnEndDrag = null;
                
        }

        
    }
