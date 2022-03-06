local hallLauncher = {}
App.RequireHall("setting.Const")
App.RequireHall("setting.Cache")
App.RequireHall("setting.MsgType")
App.RequireCommon("proto.protoDic")
--App.RequireHall("setting.Localization")

-- 开始模块
function hallLauncher.StartUp()
    hallLauncher.registerCommand()
    hallLauncher.registerProxy()
    hallLauncher.registerMediator()
end

local LoginUI = App.RequireHall("view.login.LoginUI")
local LoginMediator = App.RequireHall("view.login.LoginMediator")
local DialogUI = App.RequireHall("view.dialog.DialogUI")
local DialogMediator = App.RequireHall("view.dialog.DialogMediator")
local MainUI = App.RequireHall("view.main.MainUI")
local MainMediator = App.RequireHall("view.main.MainMediator")
local PersonalUI = App.RequireHall("view.personal.PersonalUI")
local PersonalMediator = App.RequireHall("view.personal.PersonalMediator")
local SupportUI = App.RequireHall("view.support.SupportUI")
local SupportMediator = App.RequireHall("view.support.SupportMediator")
local BindUI = App.RequireHall("view.bonus.BindUI")
local BonusMediator = App.RequireHall("view.bonus.BonusMediator")
local CashInUI = App.RequireHall("view.cash.CashInUI")
local CashMediator = App.RequireHall("view.cash.CashMediator")


--local LoginProxy = App.RequireHall("model.LoginProxy")
--local MainProxy = App.RequireHall("model.MainProxy")
local HallProxy = App.RequireHall("model.HallProxy")
local GameCommand = App.RequireHall("control.GameCommand")

-- 初始化控制命令
function hallLauncher.registerCommand()
    App.RegisterCommand(GameCommand,AppCommand.ReconnectGame)
    App.RegisterCommand(GameCommand,AppCommand.ApplicationPause)
    App.RegisterCommand(GameCommand,AppCommand.ActionLoginOut)
    App.RegisterCommand(GameCommand,AppCommand.AttributionBack)
    App.RegisterCommand(GameCommand,AppCommand.ActionQuitGame)
end

-- 初始化代理Net
function hallLauncher.registerProxy()
    App.RegisterProxy(HallProxy,nil) 
end

-- 初始化中介View
function hallLauncher.registerMediator()  
    App.RegisterMediator(LoginMediator,LoginUI)

    App.RegisterMediator(DialogMediator,DialogUI)
    App.RegisterMediator(MainMediator,MainUI)
    App.RegisterMediator(PersonalMediator,PersonalUI)
    App.RegisterMediator(SupportMediator,SupportUI)
    App.RegisterMediator(BonusMediator,BindUI)
    App.RegisterMediator(CashMediator,CashInUI)
end

return hallLauncher