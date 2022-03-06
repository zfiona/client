using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using GameUtils;

[RequireComponent(typeof(InputField))]
public class InputFieldAdapt : MonoBehaviour {

    //键盘高度
    private static float upHeight = 0f;
    private static void resetUpHeight()
    {
        upHeight = 0f;
    }

    InputField m_InputField;
    public RectTransform rectWindow = null;

    void Awake () {
        bTookViewChange = false;
        resetUpHeight();
        m_InputField = gameObject.GetComponent<InputField>();

        if (m_InputField!=null)
        {
            m_InputField.onEndEdit.AddListener(OnEndEditEvent);
        }

        var trigger = transform.gameObject.GetComponent<EventTrigger>();
         if (trigger == null)
             trigger = transform.gameObject.AddComponent<EventTrigger>();

         trigger.triggers = new List<EventTrigger.Entry>();
         EventTrigger.Entry entry = new EventTrigger.Entry();
         entry.eventID = EventTriggerType.Select;
         entry.callback = new EventTrigger.TriggerEvent();
         UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(OnSelect);
         entry.callback.AddListener(callback);
         trigger.triggers.Add(entry);        
    }


    float getRectHeight()
    {
        float height = 0f;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        RectTransform rectParent = transform.parent.GetComponent<RectTransform>();
        float realPosY = 0f;

        if (rectParent == null)
        {
            realPosY = rect.anchoredPosition.y + transform.parent.localPosition.y;
        }
        else
        {
            realPosY = rect.anchoredPosition.y + rectParent.anchoredPosition.y;
        }
        if (realPosY < 0.0000001f)//小于0
        {
            height = realPosY + Screen.height / 2;
        }
        else
        {
            height = realPosY + Screen.height / 2;
        }

        return height;
    }

    //注册页面以第一个框为基准
    void OnSelect(BaseEventData data)
    {
        StartCoroutine("changeViewUp");
    }
    
    private void OnDisable()
    {
        resetUpHeight();
        bTookViewChange = false;
    }

    //键盘获取到焦点，抬高视图
    void TakeViewHeight(float fHeight)
    {
        if (rectWindow!=null)
        {
            rectWindow.anchoredPosition = new Vector2(rectWindow.anchoredPosition.x, rectWindow.anchoredPosition.y+fHeight);
        }
    }

    private bool bTookViewChange = false;
    IEnumerator changeViewUp()
    {
        yield return new WaitForSeconds(0.5f);
        float KeyboardHeight = SdkMgr.Instance.GetKeyboardHeight(); 

        while (KeyboardHeight <= 0)
        {
            KeyboardHeight = SdkMgr.Instance.GetKeyboardHeight();
            yield return new WaitForSeconds(0.2f);
        }

        float controlHeight = getRectHeight();

#if UNITY_IOS && !UNITY_EDITOR
        if (controlHeight - 60f < KeyboardHeight)//控件高度小于键盘高度，被挡住了
        {
             upHeight = KeyboardHeight - controlHeight + 60f;
             TakeViewHeight(upHeight);
             bTookViewChange = true;
        }
        else
        {
            upHeight = 0f;
            bTookViewChange = false;
        }
#endif
#if UNITY_ANDROID && !UNITY_EDITOR //对弹出框进行补偿
        float RES_HEIGHT = 1080;
        KeyboardHeight = KeyboardHeight*RES_HEIGHT/Screen.height;
        if (controlHeight - 260f < KeyboardHeight)
        {
            if (Screen.height >= RES_HEIGHT - 2f)
            {
                upHeight = KeyboardHeight - controlHeight;
                upHeight = upHeight + 320f;
            }
            else
            {
                upHeight = KeyboardHeight - controlHeight + 220f*Screen.height/RES_HEIGHT;//弹出框补偿
            } 
            TakeViewHeight(upHeight);
            bTookViewChange = true;
        }
         else
        {
            upHeight = 0f;
            bTookViewChange = false;
        }

#endif
        yield return 0;
    }

    void OnEndEditEvent(string str)
    {
        if (bTookViewChange)
        {
            if (rectWindow != null)
            {
                rectWindow.anchoredPosition = new Vector2(rectWindow.anchoredPosition.x, rectWindow.anchoredPosition.y - upHeight);
            }
            resetUpHeight();
            bTookViewChange = false;
        }
        StopCoroutine("changeViewUp");
        if (!EventSystem.current.alreadySelecting)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
