local PersonalUI = class("PersonalUI", BaseUI)
function PersonalUI:ctor()
    self:load(UriConst.ui_personal, UIType.Fixed, UIAnim.DownToUp)
end

function PersonalUI:Refresh()
    self:UpdatePlay()
end


function PersonalUI:UpdatePlay()
    self.img_head.sprite = LoadHead(Player.head)
    self.txt_coin.text = FormatCoins(Player.money)
    self.txt_id.text = Player.userid
    self.text_nickName.text = Player.name
    self.txt_guest.gameObject:SetActive(string.isEmpty(Player.phone))
    self.btn_mobile.gameObject:SetActive(string.isEmpty(Player.phone))

    self.btn_addCoin.gameObject:SetActive(false)
end

function PersonalUI:onClick(go, name)
    if name == "btn_back" then
       self:closePage()
    elseif name == "btn_mobile" then
        App.Notice(AppMsg.BindShow)
    elseif name == "btn_addCoin" then
        App.Notice(AppMsg.CashInShow)
    elseif name == "btn_copyID" then
        SDKManager:CopyToBoard(self.txt_id.text)
        App.Notice(AppMsg.DialogShow,"COPY SUCCESS!")
    elseif name == "btn_edit" then
        App.Notice(AppMsg.UserNameShow)
    elseif name == "btn_head" then
        App.Notice(AppMsg.UserHeadShow)
    end
end

return PersonalUI