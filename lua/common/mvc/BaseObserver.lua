local BaseObserver = class("BaseObserver")

function BaseObserver:ctor( notifyMethod, notifyContext )
	self.notifyMethod = notifyMethod
	self.notifyContext = notifyContext
end

function BaseObserver:NotifyObserver( notifycation )
	local fun = self.notifyContext[self.notifyMethod]
	if fun then
		fun(self.notifyContext, notifycation)
	end
end

function BaseObserver:CompareNotifyContext( obj )
	return self.notifyContext == obj
end

return BaseObserver