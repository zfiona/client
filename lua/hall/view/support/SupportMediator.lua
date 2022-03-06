local SupportMediator = class("SupportMediator",BaseMediator)
local EmailUI = App.RequireHall("view.support.EmailUI")
local EmailDetailUI = App.RequireHall("view.support.EmailDetailUI")
local SettingUI = App.RequireHall("view.support.SettingUI")
local RuleUI = App.RequireHall("view.support.RuleUI")

function SupportMediator:OnRegister()
    self._rulePages = {}
end

function SupportMediator:HandleNotification(notification)
    local name = notification.Name
    if name == AppMsg.SupportShow then
        if not self._page then
            local view = self:getComponent()
            self._page = view:create()
        end
        self._page:openPage(notification.Body)
    elseif name == AppMsg.EmailShow then
        if not self._emailPage then
            self._emailPage = EmailUI:create()
        end
        self._emailPage:openPage(notification.Body)
    elseif name == AppMsg.EmailHide then
        if self._emailPage and self._emailPage:isActive() then 
            self._emailPage:closePage()
        end
    elseif name == AppMsg.UpdateEmailInfo then
        if self._emailPage and self._emailPage:isActive() then 
            self._emailPage:UpdateData()
        end
    elseif name == AppMsg.EmailDetailShow then
        if not self._emailDetailPage then
            self._emailDetailPage = EmailDetailUI:create()
        end
        self._emailDetailPage:openPage(notification.Body)
    elseif name == AppMsg.SettingShow then
        if not self._settingPage then
            self._settingPage = SettingUI:create()
        end
        self._settingPage:openPage(notification.Body)
    elseif name == AppMsg.RuleShow then
        local key = notification.Body
        if not table.keyof(self._rulePages,key) then
            self._rulePages[key] = RuleUI:create(key)
        end
        self._rulePages[key]:openPage(key)
    end
end

function SupportMediator:ListNotificationInterests()
    local list = {}
    table.insert(list, AppMsg.SupportShow)
    table.insert(list, AppMsg.EmailShow)
    table.insert(list, AppMsg.EmailHide)
    table.insert(list, AppMsg.UpdateEmailInfo)
    table.insert(list, AppMsg.EmailDetailShow)
    table.insert(list, AppMsg.SettingShow)
    table.insert(list, AppMsg.RuleShow)
    return list
end

return SupportMediator
