DialogType = {
    NoButton = 0,
    OneButton = 1,
    TwoButton = 2
}

local DialogMediator = class("DialogMediator",BaseMediator)
local WaitingUI = App.RequireHall("view.dialog.WaitingUI")
function DialogMediator:OnRegister()

end

function DialogMediator:HandleNotification(notification)
    local name = notification.Name
    if name == AppMsg.DialogShow then
        if not self._page then
            local view = self:getComponent()
            self._page = view:create()
        end
        self._page:openPage(notification.Body)
    elseif name == AppMsg.DialogHide then
        if self._page and self._page:isActive() then 
            self._page:closePage()
        end
    elseif name == AppMsg.WaitingShow then
        if not self._waitPage then
            self._waitPage = WaitingUI:create()
        end
        self._waitPage:openPage(notification.Body)
    elseif name == AppMsg.WaitingHide then
        if self._waitPage and self._waitPage:isActive() then 
            self._waitPage:closePage()
        end
    end
end

function DialogMediator:ListNotificationInterests()
    local list = {}
    table.insert(list, AppMsg.DialogShow)
    table.insert(list, AppMsg.DialogHide)
    table.insert(list, AppMsg.WaitingShow)
    table.insert(list, AppMsg.WaitingHide)
    return list
end

return DialogMediator
