local BonusMediator = class("BonusMediator",BaseMediator)
local DailyUI = App.RequireHall("view.bonus.DailyUI")
local VipUI = App.RequireHall("view.bonus.VipUI")
local InviteUI = App.RequireHall("view.bonus.InviteUI")
local InviteDetailUI = App.RequireHall("view.bonus.InviteDetailUI")

function BonusMediator:OnRegister()

end

function BonusMediator:HandleNotification(notification)
    local name = notification.Name
    if name == AppMsg.BindShow then
        if not self._page then
            local view = self:getComponent()
            self._page = view:create()
        end
        self._page:openPage(notification.Body)
    elseif name == AppMsg.BindHide then
        if self._page and self._page:isActive() then 
            self._page:closePage()
        end
    elseif name == AppMsg.UpdatePlayInfo then
        if self._page and self._page:isActive() then 
            self._page:removePage()
        end
    elseif name == AppMsg.DailyShow then
        if not self._dailyPage then
            self._dailyPage = DailyUI:create()
        end
        self._dailyPage:openPage(notification.Body)
    elseif name == AppMsg.UpdateDailyInfo then
        if self._dailyPage and self._dailyPage:isActive() then 
            self._dailyPage:UpdateData()
        end
    elseif name == AppMsg.VipShow then
        if not self._vipPage then
            self._vipPage = VipUI:create()
        end
        self._vipPage:openPage(notification.Body)
    elseif name == AppMsg.UpdateVipInfo then
        if self._vipPage and self._vipPage:isActive() then 
            self._vipPage:UpdateData()
        end
    elseif name == AppMsg.InviteShow then
        if not self._invitePage then
            self._invitePage = InviteUI:create()
        end
        self._invitePage:openPage(notification.Body)
    elseif name == AppMsg.UpdateInviteInfo then
        if self._invitePage and self._invitePage:isActive() then 
            self._invitePage:UpdateData()
        end
    elseif name == AppMsg.InviteDetailShow then
        if not self._inviteDetail then
            self._inviteDetail = InviteDetailUI:create()
        end
        self._inviteDetail:openPage(notification.Body)
    elseif name == AppMsg.UpdateInviteDetailInfo then
        if self._inviteDetail and self._inviteDetail:isActive() then 
            self._inviteDetail:UpdateData()
        end   
    end
end

function BonusMediator:ListNotificationInterests()
    local list = {}
    table.insert(list, AppMsg.BindShow)
    table.insert(list, AppMsg.BindHide)
    table.insert(list, AppMsg.UpdatePlayInfo)
    table.insert(list, AppMsg.DailyShow)
    table.insert(list, AppMsg.UpdateDailyInfo)
    table.insert(list, AppMsg.VipShow)
    table.insert(list, AppMsg.UpdateVipInfo)
    table.insert(list, AppMsg.InviteShow)
    table.insert(list, AppMsg.UpdateInviteInfo)
    table.insert(list, AppMsg.InviteDetailShow)
    table.insert(list, AppMsg.UpdateInviteDetailInfo)
    return list
end

return BonusMediator
