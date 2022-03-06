local InviteDetailUI = class("InviteDetailUI", BaseUI)
local InviteDetailItem = App.RequireHall("view/bonus/InviteDetailItem")
function InviteDetailUI:ctor()
    self:load(UriConst.ui_inviteDetail, UIType.Fixed,UIAnim.DownToUp)
end

function InviteDetailUI:Awake()
    self.scroll:Init(InviteDetailItem:create())
    self.type = 1
end

function InviteDetailUI:Refresh()
    self:ReqData()
end

function InviteDetailUI:ReqData()
    self.nullTip.gameObject:SetActive(true)
    App.RetrieveProxy("MainProxy"):InviteOpenReq(self.type)
end

function InviteDetailUI:UpdateData()
    self.txt_total_reward.text = FormatCoins(Cache.invite_myTotal)
    self.txt_available_reward.text = FormatCoins(Cache.invite_myAvaile)

    local items = Cache.invite_items[self.type]
    if not table.isEmpty(items) then
        self.nullTip.gameObject:SetActive(false)
        self.scroll:Refresh(items)
    end
end

function InviteDetailUI:onClick(go, name)
    if name == "btn_exit" then
        self:closePage()
    elseif name == "btn_get" then
       App.RetrieveProxy("MainProxy"):InviteGetReq(self.type)
    end
end

function InviteDetailUI:onValueChange(go,name,val)
    if val then
        self.type = name=="tog_your" and 1 or 2
        self:ReqData()
    end
end

return InviteDetailUI