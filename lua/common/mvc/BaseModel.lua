local BaseModel = class("BaseModel")

function BaseModel:ctor()
    self.proxy_map = { }
end

function BaseModel:RegisterProxy( proxy )
    self.proxy_map[proxy.ProxyName] = proxy
    proxy:OnRegister()
end

function BaseModel:RetrieveProxy( proxyName )
	return self.proxy_map[proxyName]
end

function BaseModel:HasProxy( proxyName )
	return self.proxy_map[proxyName] and true or false
end

function BaseModel:RemoveAllProxy( )
    self.proxy_map = { }
end

function BaseModel:RemoveProxy( proxyName )
	local proxy = self:RetrieveProxy( proxyName )

	self.proxy_map[proxyName] = nil

	if proxy then 
		proxy:OnRemove()
	end
	
	return proxy
end


return BaseModel