print("luaMem: " ..collectgarbage("count"))

require("common.tool.class")
require("common.app")
App.RequireCommon("tool.function")
App.RequireCommon("tool.functionEx1")
App.RequireCommon("net.ProtobufMsgMgr")
App.RequireCommon("net.HttpService")
App.RequireCommon("net.SocketService")
App.RequireCommon("setting.Define")

function main() 
	log("start lua ..")
	GameDebug.logEnable = true
   EnterLogin()
end

function EnterLogin()
   ProtobufMsgMgr.register("common/proto/hall.proto")
   local launcher = App.RequireHall("hallLauncher")
   launcher.StartUp()
   App.Notice(AppMsg.LoginShow)
end

function EnterHall()
   Const.game_id = 0
   if App.gameLauncher then
      App.gameLauncher.Dispose()
      App.gameLauncher = nil
   end
   App.Notice(AppMsg.MainShow)
end

function EnterGame(game_id)
   --从大厅进游戏
   if Const.game_id == 0 then
      log("enter game ...")
	   Const.game_id = game_id
      if game_id < 4 then
         ProtobufMsgMgr.register("common/proto/zjh.proto")
         App.gameName = "zjh"
         App.gameLauncher = App.RequireGame("zjhLauncher")
         App.gameLauncher.StartUp()
      else
         App.gameName = "rummy"
         App.gameLauncher = App.RequireGame("rummyLauncher")
         App.gameLauncher.StartUp()
      end
   --在游戏中重连
   else
      log("reconnect game ...")
      local proxy = App.RetrieveProxy("ZJHProxy")
      if proxy then
         proxy:ReconnectCS()
      end
   end
end

main()