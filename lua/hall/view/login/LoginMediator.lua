local LoginMediator = class("LoginMediator",BaseMediator)

function LoginMediator:OnRegister()

end

function LoginMediator:HandleNotification(notification)
    local name = notification.Name
    if name == AppMsg.LoginShow then
        if not self._page then
            local view = self:getComponent()
            self._page = view:create()
        end
        self._page:openPage(notification.Body)
    elseif name == AppMsg.LoginHide then
        if self._page and self._page:isActive() then 
            self._page:closePage()
        end
    end
end

function LoginMediator:ListNotificationInterests()
    local list = {}
    table.insert(list, AppMsg.LoginShow)
    table.insert(list, AppMsg.LoginHide)
    return list
end

return LoginMediator
