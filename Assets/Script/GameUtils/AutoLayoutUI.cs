using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 只针对于横向适配做纵向的自动缩放
/// </summary>
public class AutoLayoutUI : MonoBehaviour
{
    public bool resetPosition;
    public List<RectTransform> rectAreaList;

    private void Awake()
    {
        float newHeight = 1f * Screen.height * Root.designWidth /  Screen.width;     
        float scale = newHeight / Root.designHeight;
        if(rectAreaList != null)
        {
            for (int i = 0;i < rectAreaList.Count;i++)
            {
                RectTransform rt = rectAreaList[i];
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y * scale);
                if (resetPosition)
                    rt.position = new Vector3(rt.position.x, rt.position.y * scale,0);
            }
        }
    }
}
