local BaseController = class("BaseController")
local BaseView = App.RequireCommon("mvc.BaseView")
local BaseObserver = App.RequireCommon("mvc.BaseObserver")
function BaseController:ctor(view)
    self.command_map = {}
    self:InitializeController(view)
end

function BaseController:InitializeController(view)
    self.view = view
end

function BaseController:RegisterCommand(cmd_name, cmd)
    if self.command_map[cmd_name] ~= nil then
        self.command_map[cmd_name] = nil
    else
        self.view:RegisterObserver(cmd_name, BaseObserver:create("ExecuteCommand",self))
    end
    self.command_map[cmd_name] = cmd
end

function BaseController:ExecuteCommand(notification)
    local cmd = self.command_map[notification.Name]
    if cmd ~= nil then
        cmd:Execute(notification)
    end
end

function BaseController:RemoveCommand(cmd_name)
    if self.command_map[cmd_name] ~= nil then
        self.command_map[cmd_name] = nil
    end
end

function BaseController:RemoveAllCommand( )
    self.command_map = {}
end

function BaseController:HasCommand(cmd_name)
    return self.command_map[cmd_name] ~= nil
end

return BaseController