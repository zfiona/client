--重定义常用的Unity类
Canvas = CS.UnityEngine.Canvas
Button = CS.UnityEngine.UI.Button
Image = CS.UnityEngine.UI.Image
Text = CS.UnityEngine.UI.Text
InputField = CS.UnityEngine.UI.InputField
InputType = CS.UnityEngine.UI.InputField.InputType
Toggle = CS.UnityEngine.UI.Toggle
Slider = CS.UnityEngine.UI.Slider
ScrollRect = CS.UnityEngine.UI.ScrollRect
HorizontalLayoutGroup = CS.UnityEngine.UI.HorizontalLayoutGroup
VerticalLayoutGroup = CS.UnityEngine.UI.VerticalLayoutGroup

Application = CS.UnityEngine.Application
GameObject = CS.UnityEngine.GameObject
Transform = CS.UnityEngine.Transform
RectTransform = CS.UnityEngine.RectTransform
PlayerPrefs = CS.UnityEngine.PlayerPrefs
Screen = CS.UnityEngine.Screen
Color = CS.UnityEngine.Color
Vector2 = CS.UnityEngine.Vector2
Vector3 = CS.UnityEngine.Vector3
Quaternion = CS.UnityEngine.Quaternion
DOTween = CS.DG.Tweening.DOTween
Ease = CS.DG.Tweening.Ease
RotateMode = CS.DG.Tweening.RotateMode
LoopType = CS.DG.Tweening.LoopType

--常用自定义类
LuaTimer = CS.XLua.LuaTimer
Tool = CS.GameUtils.Tool
FileUtils = CS.GameUtils.FileUtils
ResourceMgr = CS.ResourceMgr.GetInstance
SDKManager = CS.SdkMgr.Instance
AudioManager = CS.AudioManager.Instance
GameController = CS.GameController.Instance
GameDebug = CS.GameDebug
SysConst = CS.AppConst
UIType = CS.UIType
UIMode = CS.UIMode
UIAnim = CS.UIAnim


--Coroutine
cs_coroutine = require "common.tool.cs_coroutine"
StartCoroutine = cs_coroutine.start
StopCoroutine = cs_coroutine.stop
StopAllCoroutine = cs_coroutine.stopAll
WaitForSeconds = cs_coroutine.waitSec
WaitForOneFrame = coroutine.yield

--JSON解析
json = require "rapidjson"