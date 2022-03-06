local NameUI = class("NameUI", BaseUI)
function NameUI:ctor()
    self:load(UriConst.ui_name, UIType.PopUp, UIAnim.DownToUp)
end

--call every show
function NameUI:Refresh()
    self.input_name.text = ""
end

function NameUI:onClick(go, name)
    if name == "btn_ok" then
        local name = self.input_name.text
        if string.match(name,"[_%w]+") ~= name then
            self.input_name.text = ""
            App.Notice(AppMsg.DialogShow,"Illegal Character!")
            return
        end
        App.RetrieveProxy("HallProxy"):NameChangeReq(name)
    else
        self:closePage()
    end 
end

return NameUI