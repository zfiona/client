BaseUI = class("BaseUI")
local UIManager = CS.UIManager

function BaseUI:ctor(...)
	self.data = nil
end

--派生类加载prefab
function BaseUI:load(ui_path,ui_type,ui_anim)
    UIManager.CreatePage(self,ui_path,ui_type,ui_anim)
end

--派生类
function BaseUI:Awake()

end

--派生类
function BaseUI:Refresh(data)
    
end

--派生类
function BaseUI:Hide(isRemove)
    
end

--meditor打开面板
function BaseUI:openPage(data)
    --logGreen("显示面板：" .. self.NAME)
    self.data = data   
    UIManager.ShowPage(self.NAME)
end

--meditor关闭面板
function BaseUI:closePage()
    UIManager.ClosePage(self.NAME)
end

--meditor销毁面板
function BaseUI:removePage()
    self.data = nil
    UIManager.RemovePage(self.NAME)
end

function BaseUI:CloseAll()
    UIManager.CloseAll(self.NAME)
end

function BaseUI:isActive()
    return UIManager.IsPageActive(self.NAME)
end

function BaseUI:onClickSound()
    AudioManager:PlayClick(UriConst.audio_btn1)
end

function BaseUI:onValueChangeSound()
    AudioManager:PlayClick(UriConst.audio_btn1)
end




