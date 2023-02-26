using GameUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDrawCtrl : MonoBehaviour
{
    public bool _IsActive;
    public RawImage _BoardTex;
    public RawImage _BrushTex;
    public RectTransform _DrawRect;

    private Canvas m_canvas;
    private Vector2 m_CurUV_Paint;
    private Vector2 m_OldUV_Paint;

    private Vector2 m_originBrush = new Vector2(16, 16);
    private int m_BrushSize = 5;
    private int m_BrushWidth;      
    private int m_BrushHeight;    
    private Color32[] m_BrushPiexls;
    private Color32 m_BrushColor;
    private Texture2D m_BrushTex;

    private Vector2 m_originBoard = new Vector2(400, 400);
    private int m_BoardSize = 5;
    private int m_BoardWidth;   
    private int m_BoardHeight;

    private Texture2D m_DrawingTex_MainTex;
    private Color32[] m_DrawingPixels_MainTex;
    private Texture2D m_DrawingTex;
    private Color32[] m_DrawingPiexls;

    private bool m_EraserModel;
    private Stack<OperateInfo> s_optInfos;
    private bool m_canDraw;
    private bool m_canScale;
    private bool m_preview;

    private float minX, maxX, minY, maxY;

    class OperateInfo
    {
        public int size;
        public Color32[] piexls;
    }
    public void SwitchEraserModel(bool isEraser)
    {
        m_EraserModel = isEraser;
        Color color = m_EraserModel ? Color.green : Color.red;
        _BrushTex.material.SetColor("_Color", color);
    }
    public void SwitchClipModel(bool isClip)
    {
        _BoardTex.material.SetInt("_UseClip", isClip ? 1 : 0);
    }
    public void SwitchPreviewModel(bool isPreview)
    {
        m_preview = isPreview;
    }
    public void UndoOperate()
    {
        if (s_optInfos.Count == 0) return;
        OperateInfo info = s_optInfos.Pop();
        if(m_BoardSize == info.size)
        {
            m_DrawingPiexls = info.piexls;
            m_DrawingTex.SetPixels32(m_DrawingPiexls);
            m_DrawingTex.Apply(false);
        }
        else
        {
            m_BoardSize = info.size;
            _BoardTex.rectTransform.sizeDelta = m_originBoard + m_originBoard * 0.25f * (m_BoardSize - 1);
            m_BoardWidth = (int)_BoardTex.rectTransform.sizeDelta.x;
            m_BoardHeight = (int)_BoardTex.rectTransform.sizeDelta.y;
            m_DrawingTex = new Texture2D(m_BoardWidth, m_BoardHeight, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Point
            };
            m_DrawingPiexls = info.piexls;
            m_DrawingTex.SetPixels32(m_DrawingPiexls);
            m_DrawingTex.Apply(false);
            _BoardTex.material.SetTexture("_MaskTex", m_DrawingTex);
        }
    }
    public void CleanOperate()
    {
        s_optInfos.Clear();
        int count = m_DrawingPiexls.Length;
        for (int i = 0; i < count; i++)
        {
            if(!m_DrawingPiexls[i].Equals(Color.black))
                m_DrawingPiexls[i] = Color.black;
        }
        m_DrawingTex.SetPixels32(m_DrawingPiexls);
        m_DrawingTex.Apply(false);
    }
    public void SetBrushSize(int size)
    {
        if (size < 1 || size > 9) return;
        m_BrushSize = size;
        setBrushInfo();
    }
    public void SetBoardSize(int size)
    {
        if (size < 1 || size > 12) return;
        CachePanel();
        m_BoardSize = size;
        setBoardInfo();
    }
    public void SetBoardTex(Sprite sprite)
    {
        CleanOperate();
        _BoardTex.texture = sprite.texture;
        AutoDrawEdge();
        //m_DrawingTex_MainTex = sprite.texture;
        //_BoardTex.material.SetTexture("_MainTex", m_DrawingTex_MainTex);
    }
    public void CaptureTex(string savePath, Action<Sprite> action)
    {
        StartCoroutine(ICaptureTexture(savePath,action));
    }
    public void FillScreen()
    {
        int fillHeight = Root.designWidth * Screen.height / Screen.width;
        int size = Math.Min(fillHeight - 750, Root.designWidth);
        m_BoardSize = (size - (int)m_originBoard.x) / 100 + 1;
        setBoardInfo();
    }
    public void AutoClip()
    {
        CachePanel();
        m_DrawingPiexls = Tool.ScaleTexture(_BoardTex.texture as Texture2D, m_BoardWidth, m_BoardHeight).GetPixels32();
        for (int i = 0; i < m_DrawingPiexls.Length; i++)
        {
            Color32 c = m_DrawingPiexls[i];
            bool isSkin = c.r > 95 && c.g > 40 && c.b > 20 && Mathf.Max(c.r, c.g, c.b) - Mathf.Min(c.r, c.g, c.b) > 15 && Mathf.Abs(c.r - c.g) > 15 && c.r > c.g && c.r > c.b;
            if (isSkin)
                m_DrawingPiexls[i] = Color.black;
            else
                m_DrawingPiexls[i] = m_BrushColor;
        }
        m_DrawingTex.SetPixels32(m_DrawingPiexls);
        m_DrawingTex.Apply(false);
    }

    private void AutoDrawEdge()
    {
        int halfBrushWeight = m_BrushWidth / 2;
        m_DrawingPiexls = Tool.ScaleTexture(_BoardTex.texture as Texture2D, m_BoardWidth, m_BoardHeight).GetPixels32();
        for (int i = 0; i < m_DrawingPiexls.Length; i++)
        {
            int x = i / m_BoardWidth;
            int y = i % m_BoardWidth;
            bool isEdge = x <= halfBrushWeight || y <= halfBrushWeight || x > m_BoardWidth - halfBrushWeight || y > m_BoardWidth - halfBrushWeight;
            if (isEdge)
                m_DrawingPiexls[i] = m_BrushColor;
        }
        m_DrawingTex.SetPixels32(m_DrawingPiexls);
        m_DrawingTex.Apply(false);
    }

    private void GetLimitedRect()
    {
        float previewPanel_hight = 750;
        float avatarBg_width = 460;
        float frameHight = previewPanel_hight * (1f * Screen.width / Root.designWidth);
        float rectSize = avatarBg_width * (1f * Screen.width / Root.designWidth);
        minX = Screen.width * 0.5f - rectSize * 0.25f; //ui适配模式为固定宽度
        maxX = Screen.width * 0.5f + rectSize * 0.25f;
        minY = frameHight * 0.5f + 50;
        maxY = frameHight * 0.5f + rectSize * 0.5f;
    }
    void Awake()
    {
        m_canvas = GameObject.FindGameObjectWithTag("Root").GetComponent<Canvas>();
        m_BrushColor = Color.white;
        s_optInfos = new Stack<OperateInfo>();

        _IsActive = true;
        GetLimitedRect();
        StartDraw();
    }

    void Update()
    {
        if (!_IsActive) return;
#if UNITY_EDITOR
        if (m_preview)
            pcPreview();
        else
            pcDraw();
#else
        if (m_preview)
            phonePreview();
        else
            phoneDraw();
#endif
        //测试
        if (Input.GetKeyDown(KeyCode.S))
        {
            AutoDrawEdge();
        }
    }

    private void pcDraw()
    {
        //画笔
        if (Input.GetMouseButtonDown(0))
        {
            Tool.GetRectPosFromScreenPos(m_canvas, Input.mousePosition, out Vector2 rect_pos);
            m_canDraw = GetInputUV(rect_pos, _BoardTex.rectTransform, out _);
            if (m_canDraw)
            {
                CachePanel();
                m_OldUV_Paint = Vector2.zero;
                m_CurUV_Paint = Vector2.zero;
            }
        }
        if (Input.GetMouseButton(0) && m_canDraw)
        {
            TouchMoveBrush(Input.mousePosition);
        }
        //缩放
        if (!m_canScale && Input.GetAxis("Mouse ScrollWheel") == 0)
        {
            m_canScale = true;
            return;
        }
        if (m_canScale && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            m_canScale = false;
            SetBoardSize(m_BoardSize - 1);
        }
        if (m_canScale && Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            m_canScale = false;
            SetBoardSize(m_BoardSize + 1);
        }
    }
    private void pcPreview()
    {
        if (Input.GetMouseButton(0) && isInRect(Input.mousePosition))
        {
            Tool.GetWorldPosFromScreenPos(m_canvas, Input.mousePosition, out Vector3 world_pos);
            _DrawRect.position = world_pos;
        }

        if (!m_canScale && Input.GetAxis("Mouse ScrollWheel") == 0)
        {
            m_canScale = true;
            return;
        }
        if (m_canScale && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            m_canScale = false;
            _DrawRect.sizeDelta += Vector2.one * 100;
        }
        if (m_canScale && Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            m_canScale = false;
            _DrawRect.sizeDelta -= Vector2.one * 100;
        }
    }

    private bool isInRect(Vector3 pos)
    {
        if (pos.x < minX || pos.x > maxX)
            return false;
        if (pos.y < minY || pos.y > maxY)
            return false;
        return true;
    }

    private Touch touch1;  //上次触摸点1(手指1)
    private Touch touch2;  //上次触摸点2(手指2)
    private void phoneDraw()
    {
        if (Input.touchCount == 1)
        {
            touch1 = Input.GetTouch(0);
            if (touch1.phase == TouchPhase.Began)
            {
                Tool.GetRectPosFromScreenPos(m_canvas, touch1.position, out Vector2 rect_pos);
                m_canDraw = GetInputUV(rect_pos, _BoardTex.rectTransform, out _);
                if (m_canDraw)
                {
                    CachePanel();
                    m_OldUV_Paint = Vector2.zero;
                    m_CurUV_Paint = Vector2.zero;
                }
            }
            if (touch1.phase == TouchPhase.Moved && m_canDraw)
                TouchMoveBrush(touch1.position);
        }
        if (Input.touchCount >= 2)
        {
            Touch newTouch1 = Input.GetTouch(0);
            Touch newTouch2 = Input.GetTouch(1);
            if (newTouch2.phase == TouchPhase.Began)
            {
                touch1 = newTouch1;
                touch2 = newTouch2;
                m_canScale = true;
                return;
            }
            if (newTouch2.phase == TouchPhase.Moved)
            {
                float oldDistance = Vector2.Distance(touch1.position, touch2.position);
                float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);
                float offset = newDistance - oldDistance;
                if (Mathf.Abs(offset) > 10 && m_canScale)
                {
                    m_canScale = false;
                    if (offset > 0)
                        SetBoardSize(m_BoardSize + 1);
                    else
                        SetBoardSize(m_BoardSize - 1);
                }
            }
        }
    }
    private void phonePreview()
    {
        if (_DrawRect == null) return;
        if (Input.touchCount == 1 && isInRect(Input.GetTouch(0).position))
        {
            touch1 = Input.GetTouch(0);
            if (touch1.phase == TouchPhase.Moved)
            {
                Tool.GetWorldPosFromScreenPos(m_canvas, touch1.position, out Vector3 world_pos);
                _DrawRect.position = world_pos;
            }
        }
        if (Input.touchCount >= 2)
        {
            Touch newTouch1 = Input.GetTouch(0);
            Touch newTouch2 = Input.GetTouch(1);
            if (newTouch2.phase == TouchPhase.Began)
            {
                touch1 = newTouch1;
                touch2 = newTouch2;
                m_canScale = true;
                return;
            }
            if (newTouch2.phase == TouchPhase.Moved)
            {
                float oldDistance = Vector2.Distance(touch1.position, touch2.position);
                float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);
                float offset = newDistance - oldDistance;
                if (Mathf.Abs(offset) > 10 && m_canScale)
                {
                    m_canScale = false;
                    if (offset > 0)
                        _DrawRect.sizeDelta += Vector2.one * 100;
                    else
                        _DrawRect.sizeDelta -= Vector2.one * 100;
                }
            }
        }
    }

    IEnumerator ICaptureTexture(string path, Action<Sprite> action)
    {
        _BrushTex.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        Sprite sprite = Tool.CaptureTexture(_BoardTex.rectTransform, path, Vector2.one * 200);
        action?.Invoke(sprite);
        _BrushTex.gameObject.SetActive(true);
    }

    private void StartDraw()
    {
        //画板
        //_BoardTex.rectTransform.sizeDelta = m_originBoard + m_originBoard * 0.25f * (m_BoardSize - 1);
        m_BoardWidth = (int)_BoardTex.rectTransform.sizeDelta.x;
        m_BoardHeight = (int)_BoardTex.rectTransform.sizeDelta.y;       
        m_DrawingTex = new Texture2D(m_BoardWidth, m_BoardHeight, TextureFormat.ARGB32, false, false)
        {
            filterMode = FilterMode.Point
        };
        m_DrawingPiexls = m_DrawingTex.GetPixels32();
        for (int i = 0; i < m_DrawingPiexls.Length; i++)
        {
            m_DrawingPiexls[i] = Color.black;
        }
        m_DrawingTex.SetPixels32(m_DrawingPiexls);
        m_DrawingTex.Apply(false);
        //蒙板
        _BoardTex.material.SetTexture("_MaskTex", m_DrawingTex);
        //_BoardTex.SetNativeSize();
        //刷子
        _BrushTex.rectTransform.sizeDelta = m_originBrush * m_BrushSize;
        m_BrushWidth = (int)m_originBrush.x * m_BrushSize;
        m_BrushHeight = (int)m_originBrush.y * m_BrushSize;
        m_BrushTex = new Texture2D(m_BrushWidth, m_BrushHeight, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        int midRow = m_BrushWidth / 2;
        int midColumn = m_BrushHeight / 2;
        int radiu = m_BrushWidth * m_BrushHeight / 4;
        for (int i = 0; i < m_BrushTex.GetPixels32().Length; i++)
        {
            int row = i / m_BrushWidth;
            int column = i % m_BrushWidth;
            if ((midRow - row) * (midRow - row) + (midColumn - column) * (midColumn - column) > radiu)
                m_BrushTex.SetPixel(row, column, Color.clear);
            else
                m_BrushTex.SetPixel(row, column, m_BrushColor);
        }
        m_BrushPiexls = m_BrushTex.GetPixels32();
    }

    private void CachePanel()
    {
        if (s_optInfos.Count > 20) return;
        OperateInfo info = new OperateInfo
        {
            size = m_BoardSize,
            piexls = new Color32[m_DrawingPiexls.Length]
        };
        m_DrawingPiexls.CopyTo(info.piexls, 0);
        s_optInfos.Push(info);
    }

    private void setBoardInfo()
    {
        _BoardTex.rectTransform.sizeDelta = m_originBoard + m_originBoard * 0.25f * (m_BoardSize-1);
        m_BoardWidth = (int)_BoardTex.rectTransform.sizeDelta.x;
        m_BoardHeight = (int)_BoardTex.rectTransform.sizeDelta.y;

        m_DrawingTex = Tool.ScaleTexture(m_DrawingTex, m_BoardWidth, m_BoardHeight);
        m_DrawingPiexls = m_DrawingTex.GetPixels32();
        _BoardTex.material.SetTexture("_MaskTex", m_DrawingTex);
    }

    private void setBrushInfo()
    {
        _BrushTex.rectTransform.sizeDelta = m_originBrush * m_BrushSize;
        m_BrushWidth = (int)m_originBrush.x * m_BrushSize;
        m_BrushHeight = (int)m_originBrush.y * m_BrushSize;

        m_BrushTex = Tool.ScaleTexture(m_BrushTex, m_BrushWidth, m_BrushHeight);
        m_BrushPiexls = m_BrushTex.GetPixels32();
    }

    private void TouchMoveBrush(Vector3 inputScreenPosition)
    {
        Tool.GetRectPosFromScreenPos(m_canvas, inputScreenPosition, out Vector2 rect_pos);
        _BrushTex.rectTransform.anchoredPosition = rect_pos;

        if (GetInputUV(rect_pos, _BoardTex.rectTransform, out Vector2 uv))
        {
            m_OldUV_Paint = m_CurUV_Paint;
            m_CurUV_Paint = uv;
            if (m_OldUV_Paint == Vector2.zero)
                m_OldUV_Paint = m_CurUV_Paint;
            
            DrawBrush((int)m_CurUV_Paint.x, (int)m_CurUV_Paint.y);
            if (Vector2.Distance(m_OldUV_Paint, m_CurUV_Paint) > (m_BrushWidth * 0.33f))
            {
                DrawLineWithBrush(m_OldUV_Paint, m_CurUV_Paint);
            }
            m_DrawingTex.SetPixels32(0, 0, m_BoardWidth, m_BoardHeight, m_DrawingPiexls, 0);
            m_DrawingTex.Apply(false);
        }
    }

    private bool GetInputUV(Vector2 brushAnchorPos, RectTransform rect, out Vector2 uv)
    {
        Vector2 filltex_sizedelta = rect.sizeDelta;
        Vector2 originAnchorPoint = rect.anchoredPosition - new Vector2(filltex_sizedelta.x, filltex_sizedelta.y) / 2;
        Vector2 dir = brushAnchorPos - originAnchorPoint;

        uv.x = (int)dir.x;
        uv.y = (int)dir.y;
        bool isInside = uv.x >= 0 && uv.y >= 0 && uv.x <= filltex_sizedelta.x && uv.y <= filltex_sizedelta.y;
    
        uv.x = Mathf.Clamp(uv.x, 0, (int)filltex_sizedelta.x);
        uv.y = Mathf.Clamp(uv.y, 0, (int)filltex_sizedelta.y);
        return isInside;
    }

    private void DrawBrush(int px, int py)
    {
        int stX = Mathf.Clamp(px - m_BrushWidth / 2, 0, m_BoardWidth);
        int stY = Mathf.Clamp(py - m_BrushHeight / 2, 0, m_BoardHeight);
        int endX = Mathf.Clamp(px + m_BrushWidth / 2, 0, m_BoardWidth);
        int endY = Mathf.Clamp(py + m_BrushHeight / 2, 0, m_BoardHeight);

        int lengthX = endX - stX;
        int lengthY = endY - stY;

        int max = m_BoardWidth * m_BoardHeight - 1;
        int pixel = m_BoardHeight * stY + stX;   //当前刷子起始像素在目标图片的位置
        //Debug.LogError("pixel:" + pixel);

        bool isEdge = lengthX < m_BrushWidth-1 || lengthY < m_BrushHeight-1;
        int BrushPiexl;
        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                BrushPiexl = m_BrushWidth * y + x;
                if (m_EraserModel)
                {
                    if (isEdge || m_BrushPiexls[BrushPiexl].a > 100)
                        m_DrawingPiexls[pixel] = Color.clear;
                    else
                        m_DrawingPiexls[pixel] = Color32.Lerp(m_DrawingPiexls[pixel], Color.clear, m_BrushPiexls[BrushPiexl].a / 255f);
                }
                else
                {
                    if (isEdge || m_BrushPiexls[BrushPiexl].a > 100)
                        m_DrawingPiexls[pixel] = m_BrushColor;
                    else
                        m_DrawingPiexls[pixel] = Color32.Lerp(m_DrawingPiexls[pixel], m_BrushColor, m_BrushPiexls[BrushPiexl].a / 255f);
                }
                pixel++;
            }
            pixel = Mathf.Clamp((m_BoardWidth * (stY + y) + stX), 0, max);
        }
    }

    private void DrawLineWithBrush(Vector2 start, Vector2 end)
    {
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx, sy;
        if (x0 < x1) { sx = 1; } else { sx = -1; }
        if (y0 < y1) { sy = 1; } else { sy = -1; }
        int err = dx - dy;
        bool loop = true;
        int minDistance = m_BrushWidth / 4;
        int pixelCount = 0;
        int e2;
        while (loop)
        {
            pixelCount++;
            if (pixelCount > minDistance)
            {
                pixelCount = 0;
                DrawBrush(x0, y0);
            }
            if ((x0 == x1) && (y0 == y1)) loop = false;
            e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
   
}
