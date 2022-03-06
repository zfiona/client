local LoginUI = class("LoginUI", BaseUI)
function LoginUI:ctor()
    self:load(UriConst.ui_login, UIType.Normal, UIAnim.None)
end

--call once create
function LoginUI:Awake()
    self.proxy = App.RetrieveProxy("HallProxy")
end

--call while hide
function LoginUI:Hide()
    
end

--call every show
function LoginUI:Refresh()
    -- local isTest = GameObject.Find("Reporter")
    -- if not isTest then
    --     self:AutoLogin()
    -- end
    local zone = Tool.GetZone()
    local lan = Tool.GetLanguage()
    --log(zone,lan)
end


function LoginUI:AutoLogin()
    if PlayerPrefs.HasKey("LoginType") then
        if PlayerPrefs.HasKey("PhoneAccount") then
            self.input_phone.text = PlayerPrefs.GetString("PhoneAccount")
            self.input_password.text = PlayerPrefs.GetString("PhonePassword")
            self:onBtnPhone()
        else
            self:onBtnGuest()
        end
    end
end

function LoginUI:onBtnGuest()
    self.proxy:LoginGuestReq()
end

function LoginUI:onBtnPhone()
    local phoneStr = self.input_phone.text
    local pwdStr = self.input_password.text
    self.proxy:LoginPhoneReq(phoneStr,pwdStr)
end

function LoginUI:onFacebook()
    
end

function LoginUI:onClick(go, name)
    if name == "btn_login" then
        self:onBtnPhone()
    elseif name == "btn_guest" then
        self:onBtnGuest()
    elseif name == "btn_facebook" then
        self:onFacebook()
    end
end

return LoginUI