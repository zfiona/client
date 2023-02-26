using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DrawLine : MaskableGraphic, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //public Texture texture;
    //public override Texture mainTexture => texture;

    List<List<UIVertex>> vertexQuadList = new List<List<UIVertex>>();
    List<UIVertex> vertexQuad;
    public float lineWidth = 2;
    public RectTransform rect;//如果有则需要截屏保存
    Vector3 lastleftPoint;
    Vector3 lastrightPoint;
    Vector3 lastPos;
    public int type; //1是连线，2是钟表
    //每次连续绘制的点
    Stack<int> onceCount = new Stack<int>();
    int preCount = 0;
    //ui canvas的高 宽恒为1536
    float UIHeight = 0;
    string path; //截图的保存地址
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        for (int i = 0; i < vertexQuadList.Count; i++)
        {
            vh.AddUIVertexQuad(vertexQuadList[i].ToArray());
        }
    }
    protected override void Start()
    {
        var ratio = (float)776 / Screen.width;
        UIHeight = ratio * Screen.height;
        path = "";
    }

    public void Delete()
    {
        if (vertexQuadList.Count == 0)
        {
            return;
        }
        vertexQuadList.Clear();
        onceCount.Clear();
        SetVerticesDirty();
    }

    public void Undo()
    {
        if (vertexQuadList.Count == 0 || onceCount.Count==0)
        {
            return;
        }
        var removeCount = onceCount.Pop();
        vertexQuadList.RemoveRange(vertexQuadList.Count - removeCount, removeCount);
        SetVerticesDirty();
    }

    public string GetScreenShootPath()
    {
        return path;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newVec = Input.mousePosition - lastPos;
        if (newVec.magnitude < 0.1f)
        {
            return;
        }
        vertexQuad = new List<UIVertex>();
        Vector3 vec = Vector3.Cross(newVec.normalized, Vector3.forward).normalized;

        var pos = Input.mousePosition;

        pos.x = pos.x * 776 / Screen.width;
        pos.y = pos.y * UIHeight / Screen.height;
        Vector3 newleftPoint = pos - new Vector3(776 / 2, UIHeight / 2, 0) + vec * lineWidth;
        Vector3 newrightPoint = pos - new Vector3(776 / 2, UIHeight / 2, 0) - vec * lineWidth;

        UIVertex uIVertex = new UIVertex();
        uIVertex.position = lastleftPoint;
        uIVertex.color = color;
        vertexQuad.Add(uIVertex);
        UIVertex uIVertex1 = new UIVertex();
        uIVertex1.position = lastrightPoint;
        uIVertex1.color = color;
        vertexQuad.Add(uIVertex1);
        UIVertex uIVertex3 = new UIVertex();
        uIVertex3.position = newrightPoint;
        uIVertex3.color = color;
        vertexQuad.Add(uIVertex3);
        UIVertex uIVertex2 = new UIVertex();
        uIVertex2.position = newleftPoint;
        uIVertex2.color = color;
        vertexQuad.Add(uIVertex2);
        lastleftPoint = newleftPoint;
        lastrightPoint = newrightPoint;
        vertexQuadList.Add(vertexQuad);

        lastPos = Input.mousePosition;

        SetVerticesDirty();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPos = Input.mousePosition;
        lastPos.x = lastPos.x * 776 / Screen.width;
        lastPos.y = lastPos.y * UIHeight / Screen.height;
        lastleftPoint = lastPos - new Vector3(776 / 2, UIHeight / 2, 0) + Vector3.up * lineWidth;
        lastrightPoint = lastPos - new Vector3(776 / 2, UIHeight / 2, 0) - Vector3.up * lineWidth;
        preCount = vertexQuadList.Count;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        onceCount.Push(vertexQuadList.Count - preCount);

        if (rect == null)
        {
            return;
        }
        StartCoroutine(StartScreenShoot()); //暂时屏蔽截屏
        
    }


    IEnumerator StartScreenShoot()
    {
        yield return new WaitForEndOfFrame();
        path = GameUtils.Tool.GetScreenTexture(rect, type);

    }
}

