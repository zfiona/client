local BaseModel = App.RequireCommon("mvc.BaseModel")
local BaseView = App.RequireCommon("mvc.BaseView")
local BaseController = App.RequireCommon("mvc.BaseController")
local BaseNotify = App.RequireCommon("mvc.BaseNotify")

BaseFacade = class("BaseFacade")

function BaseFacade:ctor()
    self.view = BaseView:create()
    self.controller = BaseController:create(self.view)
    self.model = BaseModel:create()
end

function BaseFacade:RegisterProxy(proxy)
    self.model:RegisterProxy(proxy)
end

function BaseFacade:RetrieveProxy( proxyName )
    return self.model:RetrieveProxy(proxyName)
end

function BaseFacade:RemoveProxy( proxyName )
    return self.model:RemoveProxy( proxyName )
end

function BaseFacade:HasProxy( proxyName )
    return self.model:HasProxy( proxyName )
end

function BaseFacade:RegisterCommand(cmd_name, cmd)
    self.controller:RegisterCommand(cmd_name, cmd)
end

function BaseFacade:RemoveCommand(cmd_name)
    self.controller:RemoveCommand(cmd_name)
end

function BaseFacade:HasCommand(cmd_name)
    return self.controller:HasCommand(cmd_name)
end

function BaseFacade:RegisterMediator( mediator )
    self.view:RegisterMediator(mediator)
end

function BaseFacade:RetrieveMediator( mediatorName )
    return self.view:RetrieveMediator(mediatorName)
end

function BaseFacade:RemoveMediator( mediatorName )
    return self.view:RemoveMediator(mediatorName)
end

function BaseFacade:HasMediator( mediatorName )
    return self.view:HasMediator(mediatorName)
end

-- function BaseFacade:NotifyObservers( notifycation )
--     self.view:NotifyObservers(notifycation)
-- end

function BaseFacade:SendNotification(msg_name, body)
    local notify = BaseNotify:create(msg_name, body)
    self.view:NotifyObservers(notify)
end

