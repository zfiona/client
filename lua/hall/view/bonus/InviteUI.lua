local InviteUI = class("InviteUI", BaseUI)
local uri = "https://play.google.com/store/apps/details?id=com.wlhr.teenpatti3"
function InviteUI:ctor()
    self:load(UriConst.ui_invite, UIType.Fixed,UIAnim.DownToUp)
end

function InviteUI:Awake()
    --self.url = string.format(uri,Player.userid,string.trim(Application.productName))
    
    self.uri = uri
    log(self.url)
end

function InviteUI:onClick(go, name)
    if name == "btn_exit" then
        self:closePage()
    elseif name == "btn_rule" then
        App.Notice(AppMsg.RuleShow,UriConst.ui_inviteRule)
    elseif name == "btn_copylink" then
        SDKManager:CopyToBoard(self.url)
        App.Notice(AppMsg.DialogShow,"COPY SUCCESS!")
    elseif name == "btn_facebook" then
        local contentTitle = "gold"
        local contentDesc = "the game is wonderful"
        local picUri = "https://rummygold101.com/icon_gold.png"
        SDKManager:FBShareLink(self.url,contentTitle,contentDesc,picUri,function (postId)
            App.Notice(AppMsg.DialogShow,"share facebook success!")
        end)
    elseif name == "btn_whatsapp" then
        Application.OpenURL("whatsapp://send?text=" .. self.url)
    end
end

return InviteUI