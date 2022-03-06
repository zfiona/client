BaseMediator = class("BaseMediator")

function BaseMediator:ctor(mediatorName, view)
	self.MediatorName = mediatorName
	self.ViewComponent = view
end

function BaseMediator:Call( functionName, arg )
	local fun = self[functionName]
	if fun then
		return fun(self, arg)
	end
end

function BaseMediator:OnRemove()
end

function BaseMediator:OnRegister()
end

function BaseMediator:getView()   --getMediator
	if self.view then return view end
	self.view = App.RetrieveMediator(self.NAME)
	return self.view
end

function BaseMediator:getComponent()
	local view = self:getView()
	if view then return view.ViewComponent end  --mediator.ViewComponent
	return nil
end

function BaseMediator:HandleNotification(notification)
end

function BaseMediator:ListNotificationInterests()
	local list = {}
	return list
end