local BaseView = class("BaseView")
local BaseMediator = App.RequireCommon("mvc.BaseMediator")
local BaseObserver = App.RequireCommon("mvc.BaseObserver")

function BaseView:ctor()
    self.mediator_map = { }
    self.observer_map = { }
    self:InitializeView()
end

function BaseView:InitializeView()

end

function BaseView:RegisterObserver(cmd_name, observer)
    if self.observer_map[cmd_name] == nil then
        self.observer_map[cmd_name] = {}
    end
    table.insert(self.observer_map[cmd_name], observer)    
end

function BaseView:NotifyObservers( notification )
	if self.observer_map[notification.Name] then
		local observers = self.observer_map[notification.Name]
		for i=1,#observers do
			local observer = observers[i]
			observer:NotifyObserver(notification)
		end
	end
end

function BaseView:RemoveObserver( notificationName, notifyContext )
	if self.observer_map[notificationName] then
		local observers = self.observer_map[notificationName]

		for i=1,#observers do
			if observers[i]:CompareNotifyContext(notifyContext) then
				table.remove(observers,i)
				break
			end
		end

		if #observers==0 then
			self.observer_map[notificationName] = nil
		end
	end
end

function BaseView:RegisterMediator( mediator )
	if self.mediator_map[mediator.MediatorName] then
		return
	end

	self.mediator_map[mediator.MediatorName] = mediator

	local interests = mediator:ListNotificationInterests()
	
	for i=1,#interests do
		self:RegisterObserver(interests[i], BaseObserver:create("HandleNotification", mediator))
	end

	mediator:OnRegister()
end

function BaseView:RetrieveMediator( mediatorName )
	return self.mediator_map[mediatorName]
end

function BaseView:RemoveAllMediator( )
	self.observer_map = {}
	self.mediator_map = {}
end


function BaseView:RemoveMediator( mediatorName )
	local mediator = self.mediator_map[mediatorName]
	if mediator == nil then
		return
	end

	local interests = mediator:ListNotificationInterests()
	for i=1,#interests do
		self:RemoveObserver(interests[i], mediator)
	end

	self.mediator_map[mediatorName] = nil
	if mediator then
		mediator:OnRemove()
	end
	return mediator
end

function BaseView:HasMediator( mediatorName )
	return self.mediator_map[mediatorName] and true or false
end

return BaseView