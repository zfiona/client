local EmailUI = class("EmailUI", BaseUI)
local EmailItem = App.RequireHall("view/support/EmailItem")

function EmailUI:ctor()
    self:load(UriConst.ui_email, UIType.Fixed, UIAnim.MiddleAppear)
end

function EmailUI:Awake()
    self.proxy = App.RetrieveProxy("HallProxy")
    self.scroll:Init(EmailItem)
end

function EmailUI:Refresh()
    self.proxy:EmailListReq()
end

function EmailUI:UpdateData()
    if table.isEmpty(Cache.emailList) then
        self.noEmail.gameObject:SetActive(true)
        self.btn_clear.gameObject:SetActive(false)
    else
        self.noEmail.gameObject:SetActive(false)
        self.btn_clear.gameObject:SetActive(true)    
    end
    self.scroll:Refresh(#Cache.emailList)
end

function EmailUI:onClick(go, name)
    if name == "btn_exit" or name == "btn_mask" then
        self:closePage()
    elseif name == "btn_clear" then
        self.proxy:EmailClearReq()
    end
end

return EmailUI