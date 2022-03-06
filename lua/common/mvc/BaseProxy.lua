BaseProxy = class("BaseProxy")

function BaseProxy:ctor(proxyName, data)
	self.ProxyName = proxyName
	self.Data = data
end

function BaseProxy:OnRemove() 

end

function BaseProxy:OnRegister() 

end

function BaseProxy:Call(funname, arg)
	local fun = self[funname]
	if fun then
		return fun(self, arg)
	end
end
