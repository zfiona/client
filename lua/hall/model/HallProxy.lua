local HallProxy = class("HallProxy", BaseProxy)
local ScoketService = ScoketService

function HallProxy:LoginGuestReq()
    ScoketService.connectSuccBack = function ()
        local data = {}
        data.mac_address = Tool.GetDeviceUID()
        ScoketService.SendMessage(NetCmd.Hall_CS_Login,data)
    end
    ScoketService.SendConnect()
end

function HallProxy:LoginPhoneReq(phoneStr,pwdStr)
    if string.len(phoneStr) < 9 or string.len(phoneStr) > 11 then
        App.Notice(AppMsg.DialogShow, "Please input 9-11 digit mobile phone number")
        return
    end
    if string.len(pwdStr) < 6 or string.len(pwdStr) > 20 then
        App.Notice(AppMsg.DialogShow, "Please enter a 6-20 digit password")
        return
    end
    ScoketService.connectSuccBack = function ()
        local data = {}
        data.mac_address = Tool.GetDeviceUID()
        data.phone_num = phoneStr
        data.password = pwdStr
        ScoketService.SendMessage(NetCmd.Hall_CS_Login,data)
    end
    ScoketService.SendConnect()
end

function HallProxy:BindPhoneReq(phoneStr,pwdStr)
    if string.len(phoneStr) < 9 or string.len(phoneStr) > 11 then
        App.Notice(AppMsg.DialogShow,"Please input 9-11 digit mobile phone number")
        return
    end
    if string.len(pwdStr) < 6 or string.len(pwdStr) > 20 then
        App.Notice(AppMsg.DialogShow,"Please enter a 6-20 digit password")
        return
    end
    local data = {}
    data.phone_num = phoneStr
    data.password = pwdStr
    data.code = ""
    ScoketService.SendMessage(NetCmd.Hall_CS_Bind,data)
end

function HallProxy:EnterRoomReq(gameId,roomId)
    local data = {}
    data.game_id = gameId
    data.room_id = roomId
    ScoketService.SendMessage(NetCmd.Hall_CS_EnterRoom,data)
end

function HallProxy:ChatReq(face_id,receiver)
    local data = {}
    data.face_id = face_id
    data.receiver = receiver
    ScoketService.SendMessage(NetCmd.Hall_CS_Chat,data)
end

function HallProxy:NameChangeReq(name)
    local data = {}
    data.nick_name = name
    ScoketService.SendMessage(NetCmd.Hall_CS_ChangeName,data)
end

function HallProxy:HeadChangeReq(headId)
    local data = {}
    data.head_id = headId
    ScoketService.SendMessage(NetCmd.Hall_CS_ChangeHead,data)
end

function HallProxy:SignOpenReq()
    ScoketService.SendMessage(NetCmd.Hall_CS_SignInfo)
end

function HallProxy:SignGetReq()
    ScoketService.SendMessage(NetCmd.Hall_CS_Sign)
end

function HallProxy:EmailListReq()
    ScoketService.SendMessage(NetCmd.Hall_CS_EmailInfo)
end

function HallProxy:EmailReadReq(id)
    local data = {}
    data.email_id = id
    ScoketService.SendMessage(NetCmd.Hall_CS_ReadEmail,data)
end

function HallProxy:EmailClearReq()
    ScoketService.SendMessage(NetCmd.Hall_CS_ClearEmail)
end

---------------------  SC -----------------
function HallProxy:ShowDialog(data)
    if data.code == 0 then
        App.Notice(AppMsg.DialogShow,data.msg)
    elseif data.code == 1 then
        local msg = {}
        msg.msg = data.msg
        App.Notice(AppMsg.DialogShow,msg)
    else
        local msg = {}
        msg.msg = data.msg
        msg.type = DialogType.TwoButton
        msg.onOk = function ()
            App.Notice(AppMsg.CashInShow)
        end
        App.Notice(AppMsg.DialogShow,msg)
    end
end

function HallProxy:LoginSC(data)
    Player = data.p_info
    local g_id = 0
    for i,v in ipairs(data.r_infos) do
        g_id = v.game_id
        if Games[g_id] == nil then
            Games[g_id] = {}
        end
        table.insert(Games[g_id],v)
    end
    if data.game_id > 0 then
        EnterGame(data.game_id)
    else
        EnterHall()
    end
end

function HallProxy:NetBack(cmd,data)
    log(CmdToPb[cmd],data)
    if cmd == NetCmd.Hall_SC_Error then
        self:ShowDialog(data)
    elseif cmd == NetCmd.Hall_SC_Login then
        self:LoginSC(data)
    elseif cmd == NetCmd.Hall_SC_UpdatePlayer then
        if data.name ~= "" then
            Player.name = data.name
        end
        if data.head ~= 0 then
            Player.head = data.head
        end
        if data.phone ~= "" then
            Player.phone = data.phone
        end
        App.Notice(AppMsg.UpdatePlayInfo)
    elseif cmd == NetCmd.Hall_SC_SignInfo then
        Player.sign_days = data.sign_days
        Cache.signData = data
        App.Notice(AppMsg.UpdateDailyInfo)
    elseif cmd == NetCmd.Hall_SC_EmailInfo then
        Cache.emailList = shallowcopy(data.emails)
        App.Notice(AppMsg.UpdateEmailInfo,data.has_mail_noRead)
    elseif cmd == NetCmd.Hall_SC_Buy then

    elseif cmd == NetCmd.Hall_SC_EnterRoom then
        EnterGame(data.game_id)    
    elseif cmd == NetCmd.Hall_SC_UpdateCoins then
        Player.money = data.coins
        App.Notice(AppMsg.UpdatePlayInfo)
        if data.reason >= 1000 then
            App.Notice(AppMsg.AwardShow,data.modify)
        end
    elseif cmd == NetCmd.Hall_SC_Chat then
        if App.gameLauncher then
            App.Notice(AppMsg2.ChatMessage,data)
        end
    end
end

function HallProxy:OnRegister()
    ScoketService.AddCallback(NetCmd.Hall_SC_Error, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.Hall_SC_Login, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.Hall_SC_UpdatePlayer, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.Hall_SC_SignInfo, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.Hall_SC_EmailInfo, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.Hall_SC_Buy, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.Hall_SC_UpdateCoins, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.Hall_SC_EnterRoom, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.Hall_SC_Chat, Handler(self.NetBack, self))
end

function HallProxy:OnRemove() 
    ScoketService.RemoveCallBack(NetCmd.Hall_SC_Error, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.Hall_SC_Login, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.Hall_SC_UpdatePlayer, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.Hall_SC_SignInfo, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.Hall_SC_EmailInfo, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.Hall_SC_Buy, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.Hall_SC_UpdateCoins, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.Hall_SC_EnterRoom, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.Hall_SC_Chat, Handler(self.NetBack, self))
end

return HallProxy