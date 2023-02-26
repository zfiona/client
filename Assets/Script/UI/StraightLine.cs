using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StraightLine :MaskableGraphic
{
	
	List<List<UIVertex>> quadVertexs = new List<List<UIVertex>>();//绘制的顶点

	List<UIVertex> quad = new List<UIVertex>();//每个quad顶点

	public int width = 2;//线条宽度

	[HideInInspector] RectTransform uiRect;//uiRect

	[HideInInspector]public Camera UICamera;
	
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		for (int i = 0; i < quadVertexs.Count; i++)
		{
			vh.AddUIVertexQuad(quadVertexs[i].ToArray());
		}
	}
	public void AddPos(Vector3[] pos)
	{
		if (UICamera == null)
		{
			UICamera = Root.Instance.gameObject.transform.Find("UICamera").GetComponent<Camera>();
		}
		if (UICamera == null)
		{
			return;
		}
		if (uiRect == null)
		{
			uiRect = Root.Instance.gameObject.GetComponent<RectTransform>();
		}
		if (pos == null || pos.Length < 2)
		{
			return;
		}
		var isChangeDirection = false;//是否换方向
		var curDirection = 0;//当前方向 1:沿x 2:沿y
		for (int i = 0; i < pos.Length; i++)
		{
			isChangeDirection = false;
			var curUIPos = ScreenPosToUIPos(UICamera.WorldToScreenPoint(pos[i]));
			if (i < pos.Length - 1)
			{
				var nextUIPos = ScreenPosToUIPos(UICamera.WorldToScreenPoint(pos[i + 1]));
				var xAdd = nextUIPos.x == curUIPos.x ? width : 0;
				var yAdd = nextUIPos.y == curUIPos.y ? width : 0;
				if(curDirection!=0)
				{
					if (curDirection == 1 && nextUIPos.x == curUIPos.x)
					{
						isChangeDirection = true;
					}
					else if (curDirection==2 && nextUIPos.y == curUIPos.y)
					{
						isChangeDirection = true;
					}
				}
				curDirection = nextUIPos.x == curUIPos.x ? 2 : 1;
				for (int j = 0; j < 3; j++)
				{
					quad = new List<UIVertex>();
					var startPos = curUIPos + (nextUIPos - curUIPos) * 2 * j / 5;
					var finishPos = curUIPos + (nextUIPos - curUIPos) * (2 * j +1)/ 5;
					if(!isChangeDirection && j==0)
					{
						startPos = curUIPos + (nextUIPos - curUIPos) /10;
						finishPos = curUIPos + (nextUIPos - curUIPos) * 3 / 10;
					}
					var vert1 = new Vector3(startPos.x - xAdd, startPos.y - yAdd, curUIPos.z);
					var vert2 = new Vector3(startPos.x + xAdd, startPos.y + yAdd, curUIPos.z);
					var vert4 = new Vector3(finishPos.x - xAdd, finishPos.y - yAdd, curUIPos.z);
					var vert3 = new Vector3(finishPos.x + xAdd, finishPos.y + yAdd, curUIPos.z);

					AddVertex(vert1);
					AddVertex(vert2);
					AddVertex(vert3);
					AddVertex(vert4);
					quadVertexs.Add(quad);
				}
			}
		}
		SetVerticesDirty();
	}
	public void Clear()
	{
		quadVertexs.Clear();
		quad.Clear();
		SetVerticesDirty();
	}
	void AddVertex(Vector3 vertexPos)
	{
		UIVertex uIVertex = new UIVertex();
		uIVertex.position = vertexPos;
		uIVertex.color = color;
		quad.Add(uIVertex);
	}
	public Vector3 ScreenPosToUIPos(Vector3 screenPos)
	{
		var uiPos = new Vector3();
		uiPos.x = screenPos.x * uiRect.sizeDelta.x / Screen.width;
		uiPos.y = screenPos.y * uiRect.sizeDelta.y / Screen.height;
		uiPos.z = screenPos.z;
		uiPos = uiPos - new Vector3(uiRect.sizeDelta.x / 2, (1f * Root.designWidth / Screen.width * Screen.height) / 2, 0);
		return uiPos;

	}
}
