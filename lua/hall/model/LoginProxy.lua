local LoginProxy = class("LoginProxy", BaseProxy)
local ScoketService = ScoketService
local HttpService = HttpService

local lastVerify = 0
function LoginProxy:CheckVerifyWait(txtCom)
    lastVerify = PlayerPrefs.GetInt("lastVerify",0)
    local waitTime = lastVerify + Const.VerifyWait - os.time()
    if waitTime > 0 then
        self:ShowVerifyWait(txtCom,waitTime)
    else
        txtCom.text = "OTP"
    end
end

function LoginProxy:ShowVerifyWait(txtCom,waitTime)
    txtCom.text = waitTime
    self.phoneTimer = LuaTimer.Add(1000,1000,function ()
        waitTime = waitTime - 1
        if txtCom == nil then return false end
        if waitTime == 0 then
            txtCom.text = "OTP"
            return false
        else
            txtCom.text = waitTime
            return true
        end
    end)
end

function LoginProxy:StopVerifyWait()
    if self.phoneTimer then
        LuaTimer.Delete(self.phoneTimer)
        self.phoneTimer = nil
    end
end

----------------------------CS 部分 消息驱动-------------------------------------
function LoginProxy:GetOptReq(phoneStr,txtCom)
    if string.len(phoneStr) < 9 or string.len(phoneStr) > 11 then
        App.Notice(AppMsg.DialogShow,"Please input 9-11 digit mobile phone number")
        return
    end
    if lastVerify + Const.VerifyWait - os.time() >= 0 then
        App.Notice(AppMsg.DialogShow,"Validation requests are too frequent")
        return
    end
    lastVerify = os.time()
    PlayerPrefs.SetInt("lastVerify",lastVerify)
    self:ShowVerifyWait(txtCom,Const.VerifyWait)

    local data = {}
    data.phoneNumber = phoneStr
    HttpService.Request(HttpApi.getOPT..formatUrl(data),nil)
end

function LoginProxy:GetNetsReq(gameName)
    local data = {}
    data.gameName = gameName
    HttpService.Request(HttpApi.getNets..formatUrl(data),nil,Handler(self.NetsInfoBack, self))
end

function LoginProxy:SetPasswordReq(phoneStr,pwdStr,otpStr)
    if string.len(phoneStr) < 9 or string.len(phoneStr) > 11 then
        App.Notice(AppMsg.DialogShow, "Please input 9-11 digit mobile phone number")
        return
    end
    if string.len(pwdStr) < 6 or string.len(pwdStr) > 20 then
        App.Notice(AppMsg.DialogShow, "Please enter a 6-20 digit password")
        return
    end
    local data = {}
    data.phoneNumber = phoneStr
    data.password = pwdStr
    data.code = otpStr
    data.macAddr = Tool.GetDeviceUID() .. "_" .. string.trim(Application.productName)
    log(data)
    HttpService.Request(HttpApi.setPassword..formatUrl(data),nil,Handler(self.PasswordBack, self))
end


----------------------------SC 部分-------------------------------------
function LoginProxy:NetsInfoBack(data)
    if data.flag then
        Cache.support.whatsapp = data.data.whatsapp
        Cache.support.email = data.data.email
        Cache.gateList = data.data.gateList
        --Const.GameIp, Const.GamePort = data[1].ip,data[1].port
    end
end

function LoginProxy:PasswordBack(data)
    if data.flag then
        App.Notice(AppMsg.DialogShow,"Set Password Success!")
    end
end

function LoginProxy:NetBack(subId,data)
    log(data)
    if subId == NetCmd.loginRsp.subId then
        PlayerPrefs.SetInt("LoginType",self.LoginType)
        Const.login_type = self.LoginType
        Const.ui_control = data.ui_control
        Player = deepcopy(data.user_item)
    elseif subId == NetCmd.gameListAck.subId then
        if Player.game_id == nil or Player.game_id == 0 then
            App.Notice(AppMsg.LoginAnim)
        else
            EnterGame(Player.game_id)
        end
    end
end

-----------------------注册和注销 回调函数---------------------------------------
function LoginProxy:OnRegister()
    ScoketService.AddCallback(NetCmd.loginRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.gameListAck, Handler(self.NetBack, self))
end

function LoginProxy:OnRemove() 
    ScoketService.RemoveCallBack(NetCmd.loginRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.gameListAck, Handler(self.NetBack, self))
end

return LoginProxy