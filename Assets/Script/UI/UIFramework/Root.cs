using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Init The UI Root
/// 
/// Root
/// -Canvas
/// --FixedRoot
/// --NormalRoot
/// --PopupRoot
/// -Camera
/// </summary>
public class Root : MonoBehaviour
{
    private static Root m_Instance = null;
    public static Root Instance
    {
        get
        {
            if (m_Instance == null)
            {
                InitRoot();
            }
            return m_Instance;
        }
    }

    public EventSystem Event;
    public Transform fixedRoot;
    public Transform normalRoot;
    public Transform popupRoot;
    private CanvasScaler canvasScaler;
    public static int designWidth = 776;//1080
    public static int designHeight = 1680;//1800
    private static int designMatch = 0;
    public static bool isLongScreen = false;

    static void InitRoot()
    {
        GameObject go = new GameObject("Root");
        go.tag = go.name;
        go.layer = LayerMask.NameToLayer("UI");
        go.tag = go.name;
        go.AddComponent<RectTransform>();
        m_Instance = go.AddComponent<Root>();

        //add camera
        GameObject camObj = new GameObject("UICamera");
        camObj.layer = LayerMask.NameToLayer("UI");
        camObj.transform.parent = go.transform;
        camObj.transform.localPosition = new Vector3(0, 0, -100f);
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Depth;
        cam.orthographic = true;
        cam.farClipPlane = 200f;
        cam.cullingMask = 1 << 5;
        cam.nearClipPlane = -50f;
        cam.farClipPlane = 50f;
        camObj.AddComponent<AudioListener>();

        //add type
        Canvas can = go.AddComponent<Canvas>();
        //can.renderMode = RenderMode.ScreenSpaceOverlay;
        can.renderMode = RenderMode.ScreenSpaceCamera;
        can.pixelPerfect = false;
        can.worldCamera = cam;
        CanvasScaler cs = go.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(designWidth, designHeight);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        cs.matchWidthOrHeight = designMatch;
        m_Instance.canvasScaler = cs;

        //add root
        GameObject subRoot = CreateSubCanvasForRoot(go.transform, 100);
        subRoot.name = "NormalRoot";
        m_Instance.normalRoot = subRoot.transform;
        subRoot = CreateSubCanvasForRoot(go.transform, 300);
        subRoot.name = "FixedRoot";
        m_Instance.fixedRoot = subRoot.transform;
        subRoot = CreateSubCanvasForRoot(go.transform, 500);
        subRoot.name = "PopupRoot";
        m_Instance.popupRoot = subRoot.transform;

        //add Event System
        GameObject eventObj = new GameObject("EventSystem");
        eventObj.layer = LayerMask.NameToLayer("UI");
        eventObj.transform.SetParent(go.transform);
        m_Instance.Event = eventObj.AddComponent<EventSystem>();
        eventObj.AddComponent<StandaloneInputModule>();

        //check is long screen
        isLongScreen = 1f * Screen.height / Screen.width > 1f * designHeight / designWidth;
    }

    static GameObject CreateSubCanvasForRoot(Transform root, int sort)
    {
        GameObject go = new GameObject("canvas");
        go.transform.parent = root;
        go.layer = LayerMask.NameToLayer("UI");

        Canvas can = go.AddComponent<Canvas>();
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        can.transform.localScale = Vector2.one;//new Vector3(size,1,1);
        can.overrideSorting = true;
        can.sortingOrder = sort;

        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    public static void SetWidthOrHeightValue(float value)
    {
        m_Instance.canvasScaler.matchWidthOrHeight = value;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }
}