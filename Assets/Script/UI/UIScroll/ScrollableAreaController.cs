using DG.Tweening;
using XLua;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Alignment
{
    UpperLeft,
    UpperRight,
    LowerLeft,
    LowerRight
}

public class ScrollableAreaController : MonoBehaviour
{
    public ScrollableCell cellPrefab;
    private float cellWidth;
    private float cellHeight;
    [SerializeField] private Alignment childAlignment = Alignment.UpperLeft;//子物体初始位置
    [SerializeField] private int numberColumn = 1;    //表示并排显示几个，比如是上下滑动，当此处为2时表示一排有两个cell
    [SerializeField] private int numberInvisible = 1; //表示多生成几个冗余cell
    [SerializeField] private float cellSpace = 0f; //cell的间距
    [SerializeField] private float gridSpace = 0f; //组间距
    [SerializeField] private bool initAnim = false;//生成动画
    private int visibleCellsTotalCount = 0;
    private int visibleCellsRowCount = 0;
    private LinkedList<GameObject> localCellsPool = new LinkedList<GameObject>(); //备用cell
    private LinkedList<GameObject> cellsInUse = new LinkedList<GameObject>(); //被用到的cell
    private ScrollRect rect;
    private RectTransform content;
    private RectTransform viewport;

    private int cellDataCount = 0; //列表数据长度
    private int previousInitialIndex = 0; //先前第一个cell的序号
    private int initialIndex = 0;  //当前第一个cell的序号
    private float initpostion = 0;
    private float xDir, yDir;

    //下拉刷新
    private Action mFrontAction = null;
    private bool needFrontRefresh = false;
    //上拉刷新
    private Action mBackAction = null;
    private bool needBackRefresh = false;
    //Lua对象
    private LuaTable mLuaTable = null;

    public void PreInit()
    {
        rect = GetComponent<ScrollRect>();
        content = rect.content;
        viewport = rect.viewport;

        if (numberColumn == 0) numberColumn = 1;
        if (numberColumn == 1) gridSpace = 0;

        RectTransform cell = cellPrefab.GetComponent<RectTransform>();
        cellWidth = cell.sizeDelta.x;
        cellHeight = cell.sizeDelta.y;

        Vector2 setting = Vector2.zero;
        if (childAlignment == Alignment.UpperLeft)
            setting = Vector2.up;
        else if (childAlignment == Alignment.UpperRight)
            setting = Vector2.one;
        else if (childAlignment == Alignment.LowerLeft)
            setting = Vector2.zero;
        else if (childAlignment == Alignment.LowerRight)
            setting = Vector2.right;
        content.anchorMin = setting;
        content.anchorMax = setting;
        content.pivot = setting;
        cell.anchorMin = setting;
        cell.anchorMax = setting;
        cell.pivot = setting;
    }

    public void Init(LuaTable luatable)
    {
        PreInit();

        content.anchoredPosition = Vector2.zero;
        if (horizontal)
            visibleCellsRowCount = Mathf.CeilToInt(viewport.rect.size.x / (cellWidth + cellSpace));
        else
            visibleCellsRowCount = Mathf.CeilToInt(viewport.rect.size.y / (cellHeight + cellSpace));

        visibleCellsTotalCount = visibleCellsRowCount + numberInvisible;
        visibleCellsTotalCount *= numberColumn;

        xDir = (childAlignment == Alignment.UpperLeft || childAlignment == Alignment.LowerLeft) ? 1 : -1;
        yDir = (childAlignment == Alignment.LowerLeft || childAlignment == Alignment.LowerRight) ? 1 : -1;
        mLuaTable = luatable;
        CreateCellPool();
    }

    public void AddPageCallback(Action topAction, Action bottomAction)
    {
        mFrontAction = topAction;
        mBackAction = bottomAction;
    }

    private void Update()
    {
        if (cellsInUse.Count > 0)
        {
            previousInitialIndex = initialIndex;
            CalculateCurrentIndex();
            InternalCellsUpdate();
            CheckAction();
        }
    }

    private void CalculateCurrentIndex()
    {
        if (horizontal)
        {
            if (childAlignment == Alignment.UpperLeft || childAlignment == Alignment.LowerLeft)
                initialIndex = Mathf.FloorToInt((initpostion - content.localPosition.x) / (cellWidth + cellSpace));
            else
                initialIndex = Mathf.FloorToInt((content.localPosition.x - initpostion) / (cellWidth + cellSpace));
        }
        else
        {
            if (childAlignment == Alignment.UpperLeft || childAlignment == Alignment.UpperRight)
                initialIndex = Mathf.FloorToInt((content.localPosition.y - initpostion) / (cellHeight + cellSpace));
            else
                initialIndex = Mathf.FloorToInt((initpostion - content.localPosition.y) / (cellHeight + cellSpace));
        }
        int limit = Mathf.CeilToInt(1f * cellDataCount / numberColumn) - visibleCellsRowCount;
        if (initialIndex < 0)
            initialIndex = 0;
        if (initialIndex >= limit)
            initialIndex = limit - 1;
    }
    private void InternalCellsUpdate()
    {
        //Debug.Log("previousInitialIndex : " + previousInitialIndex + "  initialIndex : " + initialIndex);
        if (previousInitialIndex != initialIndex)
        {
            //正向滑动(右上)
            bool scrollingPositive = previousInitialIndex < initialIndex;
            int indexDelta = Mathf.Abs(previousInitialIndex - initialIndex);
            int deltaSign = scrollingPositive ? +1 : -1;

            for (int i = 1; i <= indexDelta; i++)
                UpdateContent(previousInitialIndex + i * deltaSign, scrollingPositive);
        }
    }

    //cellIndex：当前第一个cell的序号，scrollingPositive：是否向上滑动（序号增大）
    private void UpdateContent(int cellIndex, bool scrollingPositive)
    {
        int index = scrollingPositive ? ((cellIndex - 1) * numberColumn) + (visibleCellsTotalCount) : (cellIndex * numberColumn);
        for (int i = 0; i < numberColumn; i++)
        {
            FreeCell(scrollingPositive);
            LinkedListNode<GameObject> tempCell = GetCellFromPool(scrollingPositive);
            int currentDataIndex = index + i;

            PositionCell(tempCell.Value, currentDataIndex);
            ScrollableCell scrollableCell = tempCell.Value.GetComponent<ScrollableCell>();
            if (currentDataIndex >= 0 && currentDataIndex < cellDataCount)
                scrollableCell.InitData(currentDataIndex);
            else
                scrollableCell.InitData(-1);
            scrollableCell.ConfigureCell();
        }
    }

    private void FreeCell(bool scrollingPositive)
    {
        LinkedListNode<GameObject> cell = null;
        // Add this GameObject to the end of the list
        if (scrollingPositive)
        {
            cell = cellsInUse.First;
            cellsInUse.RemoveFirst();
            localCellsPool.AddLast(cell);
        }
        else
        {
            cell = cellsInUse.Last;
            cellsInUse.RemoveLast();
            localCellsPool.AddFirst(cell);
        }
    }

    private void PositionCell(GameObject go, int index)
    {
        float rowMod = index % numberColumn;
        if (horizontal)
            go.transform.localPosition = new Vector2(xDir * (index / numberColumn) * (cellWidth + cellSpace), yDir * (cellHeight + gridSpace) * rowMod);
        else
            go.transform.localPosition = new Vector2(xDir * (cellWidth + gridSpace) * rowMod, yDir * (index / numberColumn) * (cellHeight + cellSpace));
    }

    private void CheckAction()
    {
        if (mFrontAction == null && mBackAction == null)
        {
            return;
        }
        if (horizontal)
        {
            if (xDir > 0)
            {
                //front
                if (content.anchoredPosition.x > cellWidth)
                    needFrontRefresh = true;
                if (needFrontRefresh && content.anchoredPosition.x < cellWidth * 0.1f)
                {
                    needFrontRefresh = false;
                    mFrontAction?.Invoke();
                }
                //back
                if (content.anchoredPosition.x < viewport.rect.width - content.sizeDelta.x - cellWidth)
                    needBackRefresh = true;
                if (needBackRefresh && content.anchoredPosition.x > viewport.rect.width - content.sizeDelta.x - cellWidth * 0.1f)
                {
                    needBackRefresh = false;
                    mBackAction?.Invoke();
                }
            }
            else
            {
                //front
                if (content.anchoredPosition.x < -cellWidth)
                    needFrontRefresh = true;
                if (needFrontRefresh && content.anchoredPosition.x > -cellWidth * 0.1f)
                {
                    needFrontRefresh = false;
                    mFrontAction?.Invoke();
                }
                //back
                if (content.anchoredPosition.x > -viewport.rect.width + content.sizeDelta.x + cellWidth)
                    needBackRefresh = true;
                if (needBackRefresh && content.anchoredPosition.x < -viewport.rect.width + content.sizeDelta.x + cellWidth * 0.1f)
                {
                    needBackRefresh = false;
                    mBackAction?.Invoke();
                }
            }
        }
        else
        {
            if (yDir > 0)
            {
                //front
                if (content.anchoredPosition.y > cellHeight)
                    needFrontRefresh = true;
                if (needFrontRefresh && content.anchoredPosition.y < cellHeight * 0.1f)
                {
                    needFrontRefresh = false;
                    mFrontAction?.Invoke();
                }
                //back
                if (content.anchoredPosition.y < viewport.rect.height - content.sizeDelta.y - cellHeight)
                    needBackRefresh = true;
                if (needBackRefresh && content.anchoredPosition.y > viewport.rect.height - content.sizeDelta.y - cellHeight * 0.1f)
                {
                    needBackRefresh = false;
                    mBackAction?.Invoke();
                }
            }
            else
            {
                //front
                if (content.anchoredPosition.y < -cellHeight)
                    needFrontRefresh = true;
                if (needFrontRefresh && content.anchoredPosition.y > -cellHeight * 0.1f)
                {
                    needFrontRefresh = false;
                    mFrontAction?.Invoke();
                }
                //back
                if (content.anchoredPosition.y > -viewport.rect.height + content.sizeDelta.y + cellHeight)
                    needBackRefresh = true;
                if (needBackRefresh && content.anchoredPosition.y < -viewport.rect.height + content.sizeDelta.y + cellHeight * 0.1f)
                {
                    needBackRefresh = false;
                    mBackAction?.Invoke();
                }
            }
        }
    }

    private bool horizontal {
        get { return rect.horizontal; }
    }

    //强制初始化
    public void RefreshDataImmediate(int count, int index)
    {
        content.anchoredPosition = Vector2.zero;
        initialIndex = 0;
        if (cellsInUse.Count > 0)
        {
            foreach (var cell in cellsInUse)
                localCellsPool.AddLast(cell);
            cellsInUse.Clear();
        }
        cellDataCount = count;
        InitData(index);
    }

    //初始化数据
    public void InitializeWithData(int count,int index)
    {
        content.anchoredPosition = Vector2.zero;
        initialIndex = 0;
        if (cellsInUse.Count > 0)
        {
            foreach (var cell in cellsInUse)
                localCellsPool.AddLast(cell);
            cellsInUse.Clear();
        }
        if (count == cellDataCount || count == 0)
        {
            cellDataCount = count;
            InitData(index);
        }
        else
        {
            cellDataCount = count;
            StopAllCoroutines();
            StartCoroutine(AsynInitData(index));
        }
    }

    IEnumerator AsynInitData(int index) {
        yield return new WaitForSeconds(Page.animTime);
        initAnim = false;
        InitData(index);
    }

    private void InitData(int index)
    {
        previousInitialIndex = 0;
        if (horizontal)
        {
            initpostion = content.localPosition.x;
            content.sizeDelta = new Vector2((cellWidth + cellSpace) * Mathf.CeilToInt(1f * cellDataCount / numberColumn) - cellSpace, (cellHeight + gridSpace) * numberColumn);
            content.anchoredPosition = new Vector2((cellWidth + cellSpace) * index, 0);
        }
        else
        {
            initpostion = content.localPosition.y;
            content.sizeDelta = new Vector2((cellWidth + gridSpace) * numberColumn, (cellHeight + cellSpace) * Mathf.Ceil(1f * cellDataCount / numberColumn) - cellSpace);
            content.anchoredPosition = new Vector2(0, (cellHeight + cellSpace) * index);
        }
        LinkedListNode<GameObject> tempCell;
        for (int i = 0; i < visibleCellsTotalCount; i++)
        {
            tempCell = GetCellFromPool(true);
            tempCell.Value.SetActive(true);
            int currentDataIndex = i + initialIndex * numberColumn;
            PositionCell(tempCell.Value, currentDataIndex);

            ScrollableCell scrollableCell = tempCell.Value.GetComponent<ScrollableCell>();
            if (currentDataIndex < cellDataCount)
                scrollableCell.InitData(currentDataIndex);
            else
                scrollableCell.InitData(-1);
            scrollableCell.ConfigureCell();
        }     
        
    }

    public int GetCurIndex()
    {
        int index = 0;
        if (horizontal)
            index = Mathf.RoundToInt(content.anchoredPosition.x / (cellWidth + cellSpace));
        else
            index = Mathf.RoundToInt(content.anchoredPosition.y / (cellHeight + cellSpace));
        index *= numberColumn;
        return index;
    }

    private float DELAY_PER_ITEM = 0.03f;  //每个元素之间的延迟
    private float SEC_LINE_DELAY = 0.06f;  //第二行延时
    private float ANI_TIME = 0.3f;        //动画总时间
    private void PositionCellAnim (GameObject go, int index)
    {
        int rowMod = index % numberColumn;
        if (horizontal)
        {
            Vector3 endPos = new Vector3(xDir * (index / numberColumn) * (cellWidth + cellSpace), yDir * (cellHeight + gridSpace) * rowMod);
            go.transform.localPosition = new Vector3(xDir * Screen.width, endPos.y);
            float delay = (index * DELAY_PER_ITEM) + rowMod * SEC_LINE_DELAY;

            Tweener tweener = go.transform.DOLocalMoveX(endPos.x, ANI_TIME);
            tweener.SetDelay(delay).SetEase(Ease.OutBack).easeOvershootOrAmplitude = 0.5f;
        }
        else
        {
            Vector3 endPos = new Vector2(xDir * (cellWidth + gridSpace) * rowMod, yDir * (index / numberColumn) * (cellHeight + cellSpace));
            go.transform.localPosition = new Vector3(endPos.x, yDir * Screen.height);
            float delay = (index * DELAY_PER_ITEM) + rowMod * SEC_LINE_DELAY;

            Tweener tweener = go.transform.DOLocalMoveY(endPos.y, ANI_TIME);
            tweener.SetDelay(delay).SetEase(Ease.OutBack).easeOvershootOrAmplitude = 0.5f;
        }
    }
    

    private void CreateCellPool()
    {
        localCellsPool.Clear();
        cellsInUse.Clear();
        //content.gameObject.SetActive(false);
        //content.transform.DestroyAllChild();
        GameObject tempCell;
        for (int i = 0; i < visibleCellsTotalCount; i++)
        {
            tempCell = InstantiateCell();
            localCellsPool.AddLast(tempCell);
        }
    }

    private GameObject InstantiateCell()
    {
        GameObject cellTempObject = Instantiate(cellPrefab.gameObject,content.transform);
        cellTempObject.transform.localScale = cellPrefab.transform.localScale;
        cellTempObject.transform.localPosition = cellPrefab.transform.localPosition;
        cellTempObject.transform.localRotation = cellPrefab.transform.localRotation;
        cellTempObject.SetActive(false);
        if (mLuaTable != null)
        {
            LuaTableCell cell = cellTempObject.GetComponent<LuaTableCell>();
            cell.Init(mLuaTable,transform.name);
        }
        return cellTempObject;
    }

    //scrollingPositive 正向
    private LinkedListNode<GameObject> GetCellFromPool(bool scrollingPositive)
    {
        if (localCellsPool.Count == 0)
        {
            Debug.LogError("null  ....");
            return null;
        }

        LinkedListNode<GameObject> cell = localCellsPool.First;
        localCellsPool.RemoveFirst();
        if (scrollingPositive)
            cellsInUse.AddLast(cell);
        else
            cellsInUse.AddFirst(cell);
        return cell;
    }

    void OnDestroy()
    {
        mLuaTable = null;
        mFrontAction = null;
        mBackAction = null;
    }
}
