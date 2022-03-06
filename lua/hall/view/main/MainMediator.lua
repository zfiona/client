local MainMediator = class("MainMediator",BaseMediator)
local RoomUI = App.RequireHall("view/main/RoomUI")
local AwardUI = App.RequireHall("view/main/AwardUI")

function MainMediator:OnRegister()
    
end

function MainMediator:HandleNotification(notification)
    local name = notification.Name
    if name == AppMsg.MainShow then
        if not self._page then
            local view = self:getComponent()
            self._page = view:create()
        end
        self._page:openPage(notification.Body)
    elseif name == AppMsg.UpdatePlayInfo then
        if self._page and self._page:isActive() then 
            self._page:UpdatePlay()
        end
    elseif name == AppMsg.UpdateEmailInfo then
        if self._page and self._page:isActive() then 
            self._page:NewEmailAppear(notification.Body)
        end
    elseif name == AppMsg.UpdateFirstCharge then
        if self._page and self._page:isActive() then 
            self._page:UpdateFirstCharge()
        end
    elseif name == AppMsg.MainHide then
        if self._page and self._page:isActive() then 
            self._page:closePage()
        end
    elseif name == AppMsg.RoomShow then
        if not self._roomPage then
            self._roomPage = RoomUI:create()
        end
        self._roomPage:openPage(notification.Body)
    elseif name == AppMsg.RoomHide then
        if self._roomPage and self._roomPage:isActive() then 
            self._roomPage:removePage()
            self._roomPage = nil
        end
    elseif name == AppMsg.AwardShow then
        if not self._awardPage then
            self._awardPage = AwardUI:create()
        end
        self._awardPage:openPage(notification.Body)
    end
end

function MainMediator:ListNotificationInterests()
    local list = {}
    table.insert(list, AppMsg.MainShow)
    table.insert(list, AppMsg.UpdatePlayInfo)
    table.insert(list, AppMsg.MainHide)
    table.insert(list, AppMsg.RoomShow)
    table.insert(list, AppMsg.RoomHide)
    table.insert(list, AppMsg.UpdateEmailInfo)
    table.insert(list, AppMsg.AwardShow)
    table.insert(list, AppMsg.UpdateFirstCharge)
    return list
end

return MainMediator
