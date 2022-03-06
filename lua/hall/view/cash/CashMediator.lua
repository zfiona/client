local CashMediator = class("CashMediator",BaseMediator)
local CashOutUI = App.RequireHall("view.cash.CashOutUI")
local CashoutRecordUI = App.RequireHall("view.cash.CashoutRecordUI")
local AccountInfoUI = App.RequireHall("view.cash.AccountInfoUI")
local ChargeInfoUI = App.RequireHall("view.cash.ChargeInfoUI")
local FirstChargeUI = App.RequireHall("view.cash.FirstChargeUI")
local CashChannelUI = App.RequireHall("view.cash.CashChannelUI")

function CashMediator:OnRegister()

end

function CashMediator:HandleNotification(notification)
    local name = notification.Name
    if name == AppMsg.CashInShow then
        if not self._page then
            local view = self:getComponent()
            self._page = view:create()
        end
        self._page:openPage(notification.Body)
    elseif name == AppMsg.UpdateCashInInfo then
        if self._page and self._page:isActive() then 
            self._page:UpdateData()
        end
    elseif name == AppMsg.CashoutShow then
        if not self._cashOutPage then
            self._cashOutPage = CashOutUI:create()
        end
        self._cashOutPage:openPage(notification.Body)
    elseif name == AppMsg.UpdateCashoutInfo then
        if self._cashOutPage and self._cashOutPage:isActive() then 
            self._cashOutPage:UpdateData()
        end
    elseif name == AppMsg.CashoutRecordShow then
        if not self._record then
            self._record = CashoutRecordUI:create()
        end
        self._record:openPage(notification.Body)
    elseif name == AppMsg.UpdateCashoutRecords then
        if self._record and self._record:isActive() then 
            self._record:UpdateData()
        end
    elseif name == AppMsg.AccountInfoShow then
        if not self._accountPage then
            self._accountPage = AccountInfoUI:create()
        end
        self._accountPage:openPage(notification.Body)
    elseif name == AppMsg.ChargeInfoShow then
        if not self._chargePage then
            self._chargePage = ChargeInfoUI:create()
        end
        self._chargePage:openPage(notification.Body)
    elseif name == AppMsg.FirshChargeShow then
        if not self._firstPage then
            self._firstPage = FirstChargeUI:create()
        end
        self._firstPage:openPage(notification.Body)
    elseif name == AppMsg.UpdateFirstCharge then
        if self._firstPage and self._firstPage:isActive() then 
            self._firstPage:UpdateData()
        end
    elseif name == AppMsg.CashChannelShow then
        if not self._channelPage then
            self._channelPage = CashChannelUI:create()
        end
        self._channelPage:openPage(notification.Body)
    elseif name == AppMsg.UpdateCashChannel then
        if self._channelPage and self._channelPage:isActive() then 
            self._channelPage:UpdateData(notification.Body)
        end
    end
end

function CashMediator:ListNotificationInterests()
    local list = {}
    table.insert(list, AppMsg.CashInShow)
    table.insert(list, AppMsg.UpdateCashInInfo)
    table.insert(list, AppMsg.CashoutShow)
    table.insert(list, AppMsg.CashoutRecordShow)
    table.insert(list, AppMsg.UpdateCashoutInfo)
    table.insert(list, AppMsg.UpdateCashoutRecords)
    table.insert(list, AppMsg.AccountInfoShow)
    table.insert(list, AppMsg.ChargeInfoShow)
    table.insert(list, AppMsg.FirshChargeShow)
    table.insert(list, AppMsg.UpdateFirstCharge)
    table.insert(list, AppMsg.CashChannelShow)
    table.insert(list, AppMsg.UpdateCashChannel)
    return list
end

return CashMediator
