local CashoutRecordUI = class("CashoutRecordUI", BaseUI)
local CashoutRecordItem = App.RequireHall("view/cash/CashoutRecordItem")

function CashoutRecordUI:ctor()
    self:load(UriConst.ui_cashOutRecord, UIType.Fixed, UIAnim.MiddleAppear)
end

function CashoutRecordUI:Awake()
    self.scroll:Init(CashoutRecordItem)
end

function CashoutRecordUI:Refresh()
    self:UpdateData()
    App.RetrieveProxy("MainProxy"):CashoutOpenReq(1)
end

function CashoutRecordUI:UpdateData()
    self.scroll:Refresh(Cache.cashoutRecords)
end

function CashoutRecordUI:onClick(go, name)
    if name == "btn_exit" or name == "btn_mask" then
        self:closePage()
    end
end

return CashoutRecordUI