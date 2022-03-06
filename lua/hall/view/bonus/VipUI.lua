local VipUI = class("VipUI", BaseUI)
local VipItem = App.RequireHall("view/bonus/VipItem")

function VipUI:ctor()
    self:load(UriConst.ui_vip, UIType.Fixed, UIAnim.RightToLeft)
end

function VipUI:Awake()
    self.pageItems = {}
    for i=1,3 do
        self["vip"..i]:Init(VipItem:create())
        table.insert(self.pageItems,self["vip"..i])
    end
    App.RetrieveProxy("MainProxy"):MonthOpenReq()
end

function VipUI:Refresh()
    if not table.isEmpty(Cache.vipList) then
        self:UpdateData()
    end
end

function VipUI:UpdateData()
    for i,v in ipairs(self.pageItems) do
        v:Refresh(Cache.vipList[i])
    end
end

function VipUI:onClick(go, name)
    if name == "btn_exit" then
        self:closePage()
    elseif name == "btn_rule" then
        App.Notice(AppMsg.RuleShow,UriConst.ui_vipRule)   
    end
end

return VipUI