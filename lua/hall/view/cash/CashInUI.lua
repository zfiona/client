local CashInUI = class("CashInUI", BaseUI)
local CashInItem = App.RequireHall("view/cash/CashInItem")
function CashInUI:ctor()
    self:load(UriConst.ui_cashIn, UIType.Fixed, UIAnim.RightToLeft)
end

function CashInUI:Awake()
    self.scroll:Init(CashInItem)
    self.scroll:AddPageCallback(function ( ... )
        log(111)
    end,function ( ... )
        log(222)
    end)
end

function CashInUI:Refresh()
    --App.RetrieveProxy("MainProxy"):RechargeOpenReq()
    self:UpdateData()
end

function CashInUI:UpdateData()
    self.scroll:Refresh(#Cache.pay_items)
end

function CashInUI:onClick(go, name)
    if name == "btn_exit" then
        self:closePage()
    elseif name == "btn_support" then
        App.Notice(AppMsg.SupportShow)
    elseif name == "btn_reset" then
        App.Notice(AppMsg.ChargeInfoShow)
    end
end

return CashInUI