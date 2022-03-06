local ChargeInfoUI = class("ChargeInfoUI", BaseUI)

function ChargeInfoUI:ctor()
    self:load(UriConst.ui_chargeInfo, UIType.PopUp, UIAnim.MiddleAppear)
end

function ChargeInfoUI:Refresh()
    local data = Player.info
    if data then
        self.input_name.text = Player.info.recharge_name
        self.input_phone.text = Player.info.recharge_phone
        self.input_email.text = Player.info.recharge_email
    end
end

function ChargeInfoUI:ReqBind()
    local data = {}
    data.recharge_name = self.input_name.text
    data.recharge_phone = self.input_phone.text
    data.recharge_email = self.input_email.text
    App.RetrieveProxy("MainProxy"):ChargeBindReq(data)
end

function ChargeInfoUI:onClick(go, name)
    if name == "btn_ok" then
        self:ReqBind()
    end
    self:closePage()
end

return ChargeInfoUI