local AccountInfoUI = class("AccountInfoUI", BaseUI)

function AccountInfoUI:ctor()
    self:load(UriConst.ui_accountInfo, UIType.Fixed, UIAnim.DownToUp)
end

function AccountInfoUI:Awake()
    
end

function AccountInfoUI:Refresh()
    local data = Cache.cashoutInfo.cash_out_info
    if data then
        self.input_account.text = data.bank_account_number
        self.input_name.text = data.bank_user_name
        self.input_code.text = data.ifsc_code
        self.input_bankName.text = data.bank_name
        self.input_phone.text = data.phone_number
    end
end

function AccountInfoUI:ReqBind()
    local info = {}
    info.bank_name = self.input_bankName.text
    info.bank_account_number = self.input_account.text
    info.bank_user_name = self.input_name.text
    info.phone_number = self.input_phone.text
    info.ifsc_code = self.input_code.text
    App.RetrieveProxy("MainProxy"):CashoutBindReq(info)
end

function AccountInfoUI:onClick(go, name)
    if name == "btn_exit" then
        self:closePage()
    elseif name == "btn_save" then
        self:ReqBind()
    end
end

return AccountInfoUI