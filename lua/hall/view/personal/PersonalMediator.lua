local PersonalMediator = class("PersonalMediator",BaseMediator)
local HeadUI = App.RequireHall("view.personal.HeadUI")
local NameUI = App.RequireHall("view.personal.NameUI")

function PersonalMediator:OnRegister()

end

function PersonalMediator:HandleNotification(notification)
    local name = notification.Name
    if name == AppMsg.PersonalShow then
        if not self._page then
            local view = self:getComponent()
            self._page = view:create()
        end
        self._page:openPage(notification.Body)
    elseif name == AppMsg.UpdatePlayInfo then
        if self._page and self._page:isActive() then 
            self._page:UpdatePlay()
        end
        if self._namePage and self._namePage:isActive() then 
            self._namePage:closePage()
        end
        if self._headPage and self._headPage:isActive() then 
            self._headPage:closePage()
        end
    elseif name == AppMsg.UserHeadShow then
        if not self._headPage then
            self._headPage = HeadUI:create()
        end
        self._headPage:openPage(notification.Body)
    elseif name == AppMsg.UserHeadHide then
        if self._headPage and self._headPage:isActive() then 
            self._headPage:closePage()
        end
    elseif name == AppMsg.UserNameShow then
        if not self._namePage then
            self._namePage = NameUI:create()
        end
        self._namePage:openPage(notification.Body)
    elseif name == AppMsg.UserNameHide then
        if self._namePage and self._namePage:isActive() then 
            self._namePage:closePage()
        end
    end
end

function PersonalMediator:ListNotificationInterests()
    local list = {}
    table.insert(list, AppMsg.PersonalShow)
    table.insert(list, AppMsg.UpdatePlayInfo)
    table.insert(list, AppMsg.UserHeadShow)
    table.insert(list, AppMsg.UserHeadHide)
    table.insert(list, AppMsg.UserNameShow)
    table.insert(list, AppMsg.UserNameHide)
    return list
end

return PersonalMediator
