local CashOutUI = class("CashOutUI", BaseUI)

function CashOutUI:ctor()
    self:load(UriConst.ui_cashOut, UIType.Fixed, UIAnim.DownToUp)
end

function CashOutUI:Awake()
    self.proxy = App.RetrieveProxy("MainProxy")
    self.slider.onValueChanged:AddListener(Handler(self.OnSlider,self))
end

function CashOutUI:Hide()
    self.slider.onValueChanged:RemoveAllListeners()
end

function CashOutUI:Refresh()
    self:UpdateData()
    self.proxy:CashoutOpenReq(0)
end

function CashOutUI:OnSlider(val)
    self.input_amount.text = math.floor(val * 50)
end

function CashOutUI:UpdateData()
    if not table.isEmpty(Cache.cashoutInfo) then
        self.txt_balance.text = FormatCoins(Cache.cashoutInfo.total_balance)
        self.txt_withdrawable.text = FormatCoins(Cache.cashoutInfo.with_draw_balance)

        local maxOut = Cache.cashoutInfo.total_balance
        if Cache.cashoutInfo.total_balance > Cache.cashoutInfo.with_draw_balance then
            maxOut = Cache.cashoutInfo.with_draw_balance
        end
        self.slider.maxValue = math.floor(FormatCoins(maxOut) / 50)

        if self:IsBindCashOut() then
            self.bindBank.gameObject:SetActive(false)
            self.bankInfo.gameObject:SetActive(true)
            self.txt_bankcard.text = Cache.cashoutInfo.cash_out_info.bank_account_number
        else
            self.bindBank.gameObject:SetActive(true)
            self.bankInfo.gameObject:SetActive(false)
        end
    end
    self.slider.value = 0
end

function CashOutUI:IsBindCashOut()
    if Cache.cashoutInfo == nil or table.isEmpty(Cache.cashoutInfo.cash_out_info) then
        return false
    end
    local data = Cache.cashoutInfo.cash_out_info
    if string.isEmpty(data.bank_account_number) then
        return false
    elseif string.isEmpty(data.bank_user_name) then
        return false
    elseif string.isEmpty(data.ifsc_code) then
        return false
    elseif string.isEmpty(data.bank_name) then
        return false
    elseif string.isEmpty(data.phone_number) then
        return false
    end
    return true
end

function CashOutUI:OnBtnWithDraw()
    if self:IsBindCashOut() then
        self.proxy:CashoutReq(tonumber(self.input_amount.text))
        App.Notice(AppMsg.DialogShow,"We'll deal with it as soon as possible")
    else
        App.Notice(AppMsg.AccountInfoShow)
    end
end

function CashOutUI:onClick(go, name)
    if name == "btn_exit" then
        self:closePage()
    elseif name == "btn_rule" then
        App.Notice(AppMsg.RuleShow,UriConst.ui_cashOutRule)
    elseif name == "btn_record" then
        App.Notice(AppMsg.CashoutRecordShow)
    elseif name == "btn_withDraw" then
        self:OnBtnWithDraw()
    elseif name == "btn_all" then
        self.slider.value = self.slider.maxValue
    elseif name == "btn_edit" then
        App.Notice(AppMsg.AccountInfoShow)
    elseif name == "btn_account" then 
        App.Notice(AppMsg.AccountInfoShow)
    end
end

return CashOutUI