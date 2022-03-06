using DG.Tweening;
using XLua;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 子物体横向移动左上对齐，纵向移动左上对齐
/// </summary>
public class ScrollableAreaController : MonoBehaviour
{
    public ScrollableCell cellPrefab;
    private RectTransform content;
    private RectTransform viewport;
    [SerializeField] private int NUMBER_OF_COLUMNS = 1;  //表示并排显示几个，比如是上下滑动，当此处为2时表示一排有两个cell
    private float cellWidth;
    private float cellHeight;
    [SerializeField] private float cellSpace = 0f; //cell的间距
    [SerializeField] private float gridSpace = 0f;//组间距
    [SerializeField] private bool isInitAnim = false;//生成动画
    private int visibleCellsTotalCount = 0;
    private int visibleCellsRowCount = 0;
    private LinkedList<GameObject> localCellsPool = new LinkedList<GameObject>(); //备用cell
    private LinkedList<GameObject> cellsInUse = new LinkedList<GameObject>(); //被用到的cell
    private ScrollRect rect;

    //private IList allCellsData ;
    private int cellDataCount = 0; //列表数据长度
    private int previousInitialIndex = 0; //先前第一个cell的序号
    private int initialIndex = 0;  //当前第一个cell的序号
    private float initpostion = 0;
   
    //下拉刷新
    private Action mTopAction = null;
    private bool needTopRefresh = false;
    //上拉刷新
    private Action mBottomAction = null;
    private bool needBotRefresh = false;

    private LuaTable mLuaTable = null;

    public void PreInit()
    {
        rect = GetComponent<ScrollRect>();
        content = rect.content;
        viewport = rect.viewport;

        if (NUMBER_OF_COLUMNS == 0) NUMBER_OF_COLUMNS = 1;
        if (NUMBER_OF_COLUMNS == 1) gridSpace = 0;

        RectTransform cell = cellPrefab.GetComponent<RectTransform>();
        cellWidth = cell.sizeDelta.x;
        cellHeight = cell.sizeDelta.y;

        content.anchorMin = Vector2.up;
        content.anchorMax = Vector2.up;
        content.pivot = Vector2.up;
        cell.anchorMin = Vector2.up;
        cell.anchorMax = Vector2.up;
        cell.pivot = Vector2.up;
    }
    public void Init(LuaTable luatable)
    {
        PreInit();

        content.anchoredPosition = Vector2.zero;
        if (horizontal)        
            visibleCellsRowCount = Mathf.CeilToInt(viewport.rect.size.x / (cellWidth + cellSpace));
        else
            visibleCellsRowCount = Mathf.CeilToInt(viewport.rect.size.y / (cellHeight + cellSpace));
        
        visibleCellsTotalCount = visibleCellsRowCount + 1;
        visibleCellsTotalCount *= NUMBER_OF_COLUMNS;

        mLuaTable = luatable;
        CreateCellPool();
    }

    public void AddPageCallback(Action topAction,Action bottomAction)
    {
        mTopAction = topAction;
        mBottomAction = bottomAction;
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
            initialIndex = Mathf.FloorToInt((initpostion - content.localPosition.x) / (cellWidth + cellSpace));
        }
        else
        {
            initialIndex = Mathf.FloorToInt((content.localPosition.y - initpostion) / (cellHeight + cellSpace));
        }
        int limit = Mathf.CeilToInt(1f * cellDataCount / NUMBER_OF_COLUMNS) - visibleCellsRowCount;
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
        int index = scrollingPositive ? ((cellIndex - 1) * NUMBER_OF_COLUMNS) + (visibleCellsTotalCount) : (cellIndex * NUMBER_OF_COLUMNS);
        for (int i = 0; i < NUMBER_OF_COLUMNS; i++)
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
        float rowMod = index % NUMBER_OF_COLUMNS;
        if (horizontal)
            go.transform.localPosition = new Vector2((index / NUMBER_OF_COLUMNS) * (cellWidth + cellSpace), -(cellHeight + gridSpace) * rowMod);
        else
            go.transform.localPosition = new Vector2((cellWidth + gridSpace) * rowMod, -(index / NUMBER_OF_COLUMNS) * (cellHeight + cellSpace));
    }

    private void CheckAction()
    {
        //头部刷新
        if (mTopAction != null)
        {
            if (horizontal)
            {
                if (needTopRefresh && content.anchoredPosition.x < 0.1f * cellWidth)
                {
                    needTopRefresh = false;
                    mTopAction.Invoke();
                    return;
                }
                if (content.anchoredPosition.x > 1f * cellHeight)
                {
                    needTopRefresh = true;
                }
            }
            else
            {
                if (needTopRefresh && content.anchoredPosition.y > -0.1f * cellHeight)
                {
                    needTopRefresh = false;
                    mTopAction.Invoke();
                    return;
                }
                if (content.anchoredPosition.y < -1f * cellHeight)
                {
                    needTopRefresh = true;
                }
            }
        }
        //尾部刷新
        if (mBottomAction != null)
        {
            if (horizontal)
            {
                if (needBotRefresh && content.anchoredPosition.x > viewport.rect.width - content.sizeDelta.x - 0.1f * cellWidth)
                {
                    needBotRefresh = false;
                    mBottomAction.Invoke();
                    return;
                }
                if (content.anchoredPosition.x < viewport.rect.width - content.sizeDelta.x - 1f * cellWidth)
                {
                    needBotRefresh = true;
                }
            }
            else
            {
                if (needBotRefresh && content.anchoredPosition.y <= content.sizeDelta.y - viewport.rect.height + 0.1f * cellHeight)
                {
                    needBotRefresh = false;
                    mBottomAction.Invoke();
                    return;
                }
                if (content.anchoredPosition.y > content.sizeDelta.y - viewport.rect.height + 1f * cellHeight)
                {
                    needBotRefresh = true;
                }
            }
        }
    }

    private bool horizontal {
        get { return rect.horizontal; }
    }
    
    //初始化数据
    public void InitializeWithData(int count)
    {
        content.anchoredPosition = Vector2.zero;
        initialIndex = 0;
        if (cellsInUse.Count > 0)
        {
            foreach (var cell in cellsInUse)
                localCellsPool.AddLast(cell);
            cellsInUse.Clear();
        }
        if (count == cellDataCount)
            InitData();
        else
            StartCoroutine(InitCellAndData());

        cellDataCount = count;
    }

    IEnumerator InitCellAndData() {
        yield return new WaitForSeconds(Page.animTime);
        content.gameObject.SetActive(true);
        previousInitialIndex = 0;
        if (horizontal) {
            content.sizeDelta = new Vector2 ((cellWidth + cellSpace) * Mathf.CeilToInt(1f * cellDataCount / NUMBER_OF_COLUMNS), (cellHeight + gridSpace)* NUMBER_OF_COLUMNS);
            initpostion = content.localPosition.x;         
        } else {
            content.sizeDelta = new Vector2 ((cellWidth + gridSpace) * NUMBER_OF_COLUMNS, (cellHeight + cellSpace) * Mathf.Ceil(1f * cellDataCount / NUMBER_OF_COLUMNS));
            initpostion = content.localPosition.y;                       
        }

        LinkedListNode<GameObject> tempCell;
        for (int i = 0; i < visibleCellsTotalCount; i++) {

            tempCell = GetCellFromPool(true);
            tempCell.Value.SetActive(true);
            int currentDataIndex = i + initialIndex * NUMBER_OF_COLUMNS;

            if (isInitAnim)
                PositionCellAnim(tempCell.Value , currentDataIndex);
            else
                PositionCell(tempCell.Value, currentDataIndex);

            ScrollableCell scrollableCell = tempCell.Value.GetComponent<ScrollableCell>();
            if(currentDataIndex < cellDataCount)
                scrollableCell.InitData(currentDataIndex);
            else
                scrollableCell.InitData(-1);
            scrollableCell.ConfigureCell ();
        }
        isInitAnim = false;
    }

    private void InitData()
    {
        LinkedListNode<GameObject> tempCell;
        for (int i = 0; i < visibleCellsTotalCount; i++)
        {
            tempCell = GetCellFromPool(true);
            tempCell.Value.SetActive(true);
            int currentDataIndex = i + initialIndex * NUMBER_OF_COLUMNS;
            PositionCell(tempCell.Value, currentDataIndex);

            ScrollableCell scrollableCell = tempCell.Value.GetComponent<ScrollableCell>();
            if (currentDataIndex < cellDataCount)
                scrollableCell.InitData(currentDataIndex);
            else
                scrollableCell.InitData(-1);
            scrollableCell.ConfigureCell();
        }
    }

    private float DELAY_PER_ITEM = 0.03f;  //每个元素之间的延迟
    private float SEC_LINE_DELAY = 0.06f;  //第二行延时
    private float ANI_TIME = 0.3f;        //动画总时间
    private void PositionCellAnim (GameObject go, int index)
    {
        int rowMod = index % NUMBER_OF_COLUMNS;
        //Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
        if (horizontal)
        {
            Vector3 endPos = new Vector3((index / NUMBER_OF_COLUMNS) * (cellWidth + cellSpace), -(cellHeight + gridSpace) * rowMod);
            go.transform.localPosition = new Vector3(Screen.width, endPos.y);
            float delay = (index * DELAY_PER_ITEM) + rowMod * SEC_LINE_DELAY;

            Tweener tweener = go.transform.DOLocalMoveX(endPos.x, ANI_TIME);
            tweener.SetDelay(delay).SetEase(Ease.OutBack).easeOvershootOrAmplitude = 0.5f;
        }
        else
        {
            Vector3 endPos = new Vector2((cellWidth + gridSpace) * rowMod, -(index / NUMBER_OF_COLUMNS) * (cellHeight + cellSpace));
            go.transform.localPosition = new Vector3(endPos.x, -1080);
            float delay = (index * DELAY_PER_ITEM) + rowMod * SEC_LINE_DELAY;

            Tweener tweener = go.transform.DOLocalMoveY(endPos.y, ANI_TIME);
            tweener.SetDelay(delay).SetEase(Ease.OutBack).easeOvershootOrAmplitude = 0.5f;
        }
    }
    

    private void CreateCellPool()
    {
        localCellsPool.Clear();
        cellsInUse.Clear();
        content.gameObject.SetActive(false);
        content.transform.DestroyAllChild();
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
            cell.Init(mLuaTable);
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
    }
}
