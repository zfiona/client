App = {}
App.gameLauncher = nil
App.gameName = ""

function App.RequireCommon( path )
	return require("common." .. path)
end

function App.RequireHall( path )
	return require("hall." .. path)
end

function App.HallRes( path )
	return "hall/" .. path
end

function App.RequireGame( path )
	return require(App.gameName .. "." .. path)
end

function App.GameRes( path )
	return App.gameName .. "/" ..path
end

App.RequireCommon("mvc.BaseFacade")
App.RequireCommon("mvc.BaseProxy")
App.RequireCommon("mvc.BaseMediator")
App.RequireCommon("mvc.BaseCommand")
App.RequireCommon("mvc.BaseUI")

local app = BaseFacade:create()

--直接调C#脚本，启动游戏
function App.NoticeCS( notificationName ,body)
	if notificationName == nil then return end
	AppFacade.getInstance():SendNotification(notificationName,body)
end

--发送消息
--notificationName 消息号
--body 传数据
--tp 类型
function App.Notice(notificationName, body)
	if notificationName == nil then return end
	app:SendNotification(notificationName,body)
end

-- 注册Command
-- name 消息id
-- target Command
function App.RegisterCommand(target,cmd)
	assert(target,"RegisterCommand " .. " target can't nil")
	if app:HasCommand(target.NAME) then return end
	app:RegisterCommand(cmd, target:create())
end

-- 注册Proxy
-- name 消息id
-- target Proxy
function App.RegisterProxy(target,data)
	assert(target,"RegisterProxy " .. " target can't nil")
	if app:HasProxy(target.NAME) then return end

	app:RegisterProxy(target:create(target.NAME, data))
end

-- 注册Mediator
-- name 消息id
-- target Mediator
function App.RegisterMediator(target,view)
	assert(target,"RegisterMediator " .. " view can't nil")
	if app:HasMediator(target.NAME) then return end
	app:RegisterMediator(target:create(target.NAME,view))
end

-- 移除Command
-- name 消息id
function App.RemoveCommand(notice)
	app:RemoveCommand(notice)
end

-- 移除Proxy
-- name 消息id
function App.RemoveProxy(name)
	app:RemoveProxy(name)
end

-- 移除Mediator
-- name 消息id
function App.RemoveMediator(name)
	app:RemoveMediator(name)
end

-- 获取一个Mediator
-- name 消息id
function App.RetrieveMediator(mediatorName)
	return app:RetrieveMediator(mediatorName)
end

-- 获取一个Proxy
-- name 消息id
function App.RetrieveProxy(proxyName)
	return app:RetrieveProxy(proxyName)
end


--暂停
function App.OnApplicationPauseCallBack(pause)
	if not pause then
        App.Notice(AppCommand.ApplicationPause)
    end
end

function App.OnAttributionCallBack(param)
	log("获取渠道号",param)
	App.Notice(AppCommand.AttributionBack,param)
end

function App.OnEscape()
	App.Notice(AppCommand.ActionQuitGame)
end

function App.ShowDialog(msg)
	App.Notice(AppMsg.DialogShow,msg)
end