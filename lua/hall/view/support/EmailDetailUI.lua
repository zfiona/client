local EmailDetailUI = class("EmailDetailUI", BaseUI)
function EmailDetailUI:ctor()
    self:load(UriConst.ui_emailDetail, UIType.PopUp, UIAnim.None)
end

function EmailDetailUI:Refresh()
    log(self.data)
    self.txt_info.text = self.data.info
    if self.data.item_num > 0 then
        self.gold.gameObject:SetActive(true)
        self.txt_gold.text = FormatCoins(self.data.item_num)
        self.btn_get.gameObject:SetActive(true)

        local isAvailable = self.data.status == 0
        self.btn_get.interactable = isAvailable
        self.get.gameObject:SetActive(isAvailable)
    else
        if self.data.status == 0 then
            App.RetrieveProxy("MainProxy"):EmailReadReq(self.data.mail_id)
        end
    end
end

function EmailDetailUI:onClick(go, name)
    if name == "btn_exit" or name == "btn_mask" then
        self:closePage()
    elseif name == "btn_get" then
        App.RetrieveProxy("MainProxy"):EmailReadReq(self.data.mail_id)
        self:closePage()
    end
end

return EmailDetailUI