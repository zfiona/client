local CashChannelUI = class("CashChannelUI", BaseUI)
local CashChannelItem = App.RequireHall("view/cash/CashChannelItem")

function CashChannelUI:ctor()
    self:load(UriConst.ui_cashChannel, UIType.PopUp, UIAnim.MiddleAppear)
end

function CashChannelUI:Awake()
    self.channel = 1
    self.scroll:Init(CashChannelItem:create())
end

function CashChannelUI:UpdateData(channel)
    self.channel = channel
end

function CashChannelUI:Refresh()
    self.txt_cost.text = "₹ ".. self.data.price
    self.scroll:Refresh(Cache.pay_type)
end

function CashChannelUI:IsBindCashIn()
    if Player == nil or table.isEmpty(Player.info) then
        return false
    elseif string.isEmpty(Player.info.recharge_name) then
        return false
    elseif string.isEmpty(Player.info.recharge_phone) then
        return false
    elseif string.isEmpty(Player.info.recharge_email) then
        return false
    end
    return true
end

function CashChannelUI:onClick(go, name)
    if name == "btn_mask" then
        self:closePage()
    elseif name == "btn_support" then
        App.Notice(AppMsg.SupportShow)
    elseif name == "btn_next" then
        if self:IsBindCashIn() then
            local index = Cache.pay_type[self.channel].index
            App.RetrieveProxy("MainProxy"):RechargeOrderReq(self.data.index,index)
        else
            App.Notice(AppMsg.ChargeInfoShow)
        end
    end
end

return CashChannelUI