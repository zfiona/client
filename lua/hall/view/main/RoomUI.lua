local RoomUI = class("RoomUI", BaseUI)
local RoomItem = App.RequireHall("view/main/RoomItem")
local RoomIcons = {
    [1] = "hall/variation/icon_game_3patti",
    [2] = "hall/variation/icon_game_Joker",
    [3] = "hall/variation/icon_game_AK47",
}

function RoomUI:ctor()
    self:load(UriConst.ui_room, UIType.Fixed, UIAnim.DownToUp)
end

function RoomUI:Awake()
	self.scroll:Init(RoomItem)
	self.scrollRect = self.scroll:GetComponent("ScrollRect")
	self.rect = self.scroll:GetComponent("RectTransform")
end

local size,space = 260,50
function RoomUI:Refresh()
	local len = #Games[self.data]
	local posx = (Const.ScreenWidth - (len-1) * (size+space) - size) * 0.5
	if posx > 0 then
		self.scrollRect.enabled = false
		self.rect.anchoredPosition = Vector2(posx,0)
	else
		self.scrollRect.enabled = true
		self.rect.anchoredPosition = Vector2(0,0)
	end
	self.scroll:Refresh(len)
	self.img_title.sprite = ResourceMgr:LoadRes(RoomIcons[self.data])
	self.img_title:SetNativeSize()
end


function RoomUI:onClick(go, name)
	Const.game_id = 0
	App.Notice(AppMsg.RoomHide)
end

return RoomUI