--全局table，常驻内存
ScoketService = {}
local this = ScoketService
local protobufMsgMgr = ProtobufMsgMgr
local netCallbacks={}
local heart_hander = nil
local socket = nil


--C#工程调>消息错误返回
function ScoketService.ConnectCallBack(state)
    if state == 1 then
        log("创建连接成功...")
        if this.connectSuccBack then
            this.connectSuccBack()
        end
        --this.StartHeartBeat()
    elseif state == 2 then
        log("创建连接失败...")
        if this.connectFailBack then
            this.connectFailBack()
        else
            App.Notice(AppMsg.DialogShow, "Failed to connect server，try again later！")
        end
    elseif state == 3 then
        log("连接出现错误...")
        --this.StopHeartBeat()
        if not table.isEmpty(Player) then
            this.TryToConnectServer()
        end
    end
    App.Notice(AppMsg.WaitingHide)
end

-- 在C#工程>消息返回（NetworkManager.Init()调用）
function ScoketService.MsgCallback(cmd, data)
    local func = this.GetCallback(cmd)
    if func then
        local pbTabel,error
        pbTabel,error = protobufMsgMgr.decode(CmdToPb[cmd], data)
        if error then
            logError(error)
        else
            if pbTabel == nil then
                pbTabel = {}
            end
            func(cmd, pbTabel)
        end
    end
end

function ScoketService.SendConnect()
    if not socket then
        socket = CS.NetExtension.SocketClient(this)
    end
    socket:SendConnect(Const.GameIp, Const.GamePort,3000)
    App.Notice(AppMsg.WaitingShow,-1)
end

function ScoketService.SendMessage(cmd, data)
    local pbdata
    data = data or {}
    pbdata = protobufMsgMgr.encode(CmdToPb[cmd], data)
    if socket then
        socket:SendMessage(cmd,pbdata)
    end 
end

function ScoketService.GetCallback(cmd)
    return netCallbacks[cmd]
end

function ScoketService.AddCallback(cmd, func)
    netCallbacks[cmd] = func
end

function ScoketService.RemoveCallBack(cmd)
    netCallbacks[cmd] = nil
end

function ScoketService.StartHeartBeat()
    local count = 0
    heart_hander = LuaTimer.Add(10000, 10000, function()
        count = count + 1
        this.SendMessage(NetCmd.heartBeat,{tick_count = count})
        --log("tick  " .. count)
        return true
    end)
end

function ScoketService.StopHeartBeat()
    if heart_hander then
        LuaTimer.Delete(heart_hander)
        heart_hander = nil
    end
end

function ScoketService.ClearMessageQueue()
    if socket then
        socket:ClearMessage()
    end
end

function ScoketService.CloseConnet()
    this.StopHeartBeat()
    if socket then
        socket:CloseConnet()
        socket = nil
    end
end

-- 尝试连接服务器
function ScoketService.TryToConnectServer()
    this.connectSuccBack = function ()
        App.Notice(AppMsg.WaitingHide)
        App.Notice(AppCommand.ReconnectGame)
    end
    this.connectFailBack = function ()
        App.Notice(AppMsg.DialogShow, { 
            msg = "Unable to connect to server, please try or login again!", 
            type = DialogType.TwoButton,
            onOk = function()
                this.TryToConnectServer()
            end,
            onCancel = function()
                App.Notice(AppCommand.ActionLoginOut)
                App.Notice(AppMsg.WaitingHide)
            end
        })
    end
    App.Notice(AppMsg.WaitingShow,-1)
    this.SendConnect()
end