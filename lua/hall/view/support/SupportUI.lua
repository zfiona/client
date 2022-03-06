local SupportUI = class("SupportUI", BaseUI)
function SupportUI:ctor()
    self:load(UriConst.ui_support, UIType.PopUp,UIAnim.MiddleAppear)
end

function SupportUI:onClick(go, name)
    if name == "btn_exit" or name == "btn_mask" then
        self:closePage()
    elseif name == "btn_copyPhone" then
        SDKManager:CopyToBoard(self.txt_phoneNum.text)
        App.Notice(AppMsg.DialogShow,"COPY SUCCESS!")
    elseif name == "btn_copyEmail" then
        SDKManager:CopyToBoard(self.txt_email.text)
        App.Notice(AppMsg.DialogShow,"COPY SUCCESS!")
    end
end

return SupportUI