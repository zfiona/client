local SettingUI = class("SettingUI", BaseUI)
function SettingUI:ctor()
    self:load(UriConst.ui_setting, UIType.PopUp,UIAnim.MiddleAppear)
end

function SettingUI:Awake()
    self.tog_music.isOn = PlayerPrefs.GetFloat("music",0.5)>0
    self.tog_sound.isOn = PlayerPrefs.GetFloat("sound",1)>0
end

function SettingUI:onClick(go, name)
    if name == "btn_exit" or name == "btn_mask" then
        self:closePage()
    elseif name == "btn_laguage" then
        App.Notice(AppMsg.DialogShow,"COMING SOON!")
    elseif name == "btn_logout" then
        App.Notice(AppCommand.ActionLoginOut)
    end
end

--tog click callback
function SettingUI:onValueChange(go,name,val)
    if name == "tog_music" then
        PlayerPrefs.SetFloat("music",val and 0.5 or 0)
        AudioManager:SetBgVoice()
    elseif name == "tog_sound" then
        PlayerPrefs.SetFloat("sound",val and 1 or 0)
    end
    findChild(go,"on").gameObject:SetActive(val)
    findChild(go,"off").gameObject:SetActive(not val)
end

return SettingUI