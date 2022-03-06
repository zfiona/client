local BindUI = class("BindUI", BaseUI)
function BindUI:ctor()
    self:load(UriConst.ui_bind, UIType.Fixed, UIAnim.MiddleAppear)
end

function BindUI:Awake()
    self.txt_coin.text = "5"
end

function BindUI:Refresh()
    --self.loginProxy:CheckVerifyWait(self.txt_otp)
end

function BindUI:Hide()
    --self.loginProxy:StopVerifyWait()
end

function BindUI:onClick(go, name)
    if name == "btn_mask" then
        self:closePage()
    elseif name == "btn_submit" then
        App.RetrieveProxy("HallProxy"):BindPhoneReq(self.input_phone.text,self.input_pw.text)
    end
end

return BindUI