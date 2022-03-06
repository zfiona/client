Localization={}
Localization.lan = "CN"

Localization.CN = {
    AgreeUserGuide = "请确认并同意用户协议",
    Logining = "正在登录游戏...",
    BindInviteCodeSucceed = "获得 <color=#fee301>金币×8</color>",  
    ConfirmInviteCode = "您确定绑定推荐码：%d吗？",
}

Localization.EN = {
    AgreeUserGuide = "Please confirm and agree to the User Agreement ",
    Logining = "Logging in...",
    BindInviteCodeSucceed = "Get <color=#fee301>Coins×8</color>",  
    ConfirmInviteCode = "Are you sure to bind the code：%d？",
}

function Localization.getText(key,...)
	local args = ... or nil
    local msg = Localization[lan][key]
    if not msg then return "" end 
    if args ~= nil then msg = string.format(msg, ...) end
    return msg
end

function Localization.setLanguage(key)
    Localization.lan = key
end