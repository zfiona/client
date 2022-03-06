-----------------------登陆----------------------------
local GameCommand = class("GameCommand", BaseCommand)
function GameCommand:Execute(notification)
    local name = notification.Name
 	if name == AppCommand.ReconnectGame then
 		self:ReconnectGame()
    elseif name == AppCommand.ApplicationPause then
        self:ApplicationPause()
    elseif name == AppCommand.ActionLoginOut then
        self:LogOut()
    elseif name == AppCommand.AttributionBack then
        self:AttributionBack()
    elseif name == AppCommand.ActionQuitGame then
        self:QuitGame()
 	end
end

function GameCommand:ReconnectGame()
    local data = {}
    data.login_type = 3
    data.phone = PlayerPrefs.GetString("PhoneAccount")
    data.password = PlayerPrefs.GetString("PhonePassword")
    data.mac_address = Tool.GetDeviceUID() .. "_" .. string.trim(Application.productName)
    data.channel = SDKManager.attributionParam
    data.pack_name = string.trim(Application.productName)
    local apkData = SDKManager:GetApkData(1)
    data.agent_id = string.isEmpty(apkData) and 0 or string.split(apkData,"_")[2]
    data.version = SysConst.config and SysConst.config.version or "1.0"
    ScoketService.SendMessage(NetCmd.loginReq,data)
end

function GameCommand:ApplicationPause()
    if Const.game_id > 0 then
        log("un pause game ...")
        -- local proxy = App.RetrieveProxy("ZJHProxy")
        -- if proxy then
        --     proxy:ReconnectCS()
        -- end
    else
        log("un pause hall ...")
    end
end

function GameCommand:LogOut()
    log("go to login")
    Const.hasEnterHall = false
    Player = nil
    AudioManager:PauseBg(true)
    ScoketService.CloseConnet()
    App.Notice(AppMsg.LoginShow) 
end

function GameCommand:AttributionBack(param)
    
end

function GameCommand:QuitGame()
    log("quit to game")
    local msg = {}
    msg.msg = "Are you sure to leave the game?"
    msg.type = DialogType.TwoButton
    msg.onOk = function ()
        Application.Quit()
    end
    App.Notice(AppMsg.DialogShow,msg)
end

return GameCommand