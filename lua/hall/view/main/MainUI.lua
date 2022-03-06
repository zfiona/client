local MainUI = class("MainUI", BaseUI)
local gamePrefabs = {
    [1] = "hall/prefab/item_game_3Patti",
    [2] = "hall/prefab/item_game_Joker",
    [3] = "hall/prefab/item_game_AK47",
    [4] = "hall/prefab/item_game_Rummy"
}

function MainUI:ctor()
    self:load(UriConst.ui_main, UIType.Normal, UIAnim.None)
end

function MainUI:Awake()
    Const.ScreenWidth = Screen.width * 1080 / Screen.height
    self.email_red = findChild(self.btn_email,"redpoint")
    self:SetFirstCharge()
    self:UpdatePlay()
    self:InitGame()
    self.btn_shop.gameObject:SetActive(false)
end

function MainUI:Refresh()
    App.Notice(AppMsg.LoginHide)
    AudioManager:PlayBg(UriConst.audio_main)
end

function MainUI:Hide()
    if self.timer_charge then
        LuaTimer.Delete(self.timer_charge)
    end
end

function MainUI:UpdatePlay()
    self.img_head.sprite = LoadHead(Player.head)
    self.txt_id.text = Player.userid
    self.txt_name.text = Player.name
    self.text_coin.text = FormatCoins(Player.money)
    self:NewEmailAppear(Player.un_read_mail == 1)
    self.btn_bonus.gameObject:SetActive(string.isEmpty(Player.phone))
    self.txt_day.text = Player.sign_days
end

function MainUI:InitGame()
    self.gameBtns = {}
    for i,v in ipairs(gamePrefabs) do
        local prefab = ResourceMgr:LoadUIPrefab(gamePrefabs[i])      
        local item = Instantiate(prefab,self.Content)
        table.insert(self.gameBtns,item:GetComponent("Button"))
    end
    for i,v in ipairs(self.gameBtns) do
        v.onClick:AddListener(Handler(self.onGameClick,self,i))
    end
end

function MainUI:onGameClick(i)
    self:onClickSound()
    if i == 4 then
        -- App.Notice(AppMsg.DialogShow,"Coming Soon!")         
        EnterGame(4)
        return 
    end
    App.Notice(AppMsg.RoomShow,i)
end

function MainUI:UpdateFirstCharge()
    self.count_time = Player.first_charge_left_time
end

function MainUI:SetFirstCharge()
    if Player.first_charge == 1 then
        self.count_time = Player.first_charge_left_time
        self.timer_charge = LuaTimer.Add(1000,1000,Handler(self.ShowTimer,self))
    end
end

function MainUI:ShowTimer()
    if self.count_time <= 0 then 
        LuaTimer.Delete(self.timer_charge)
        self.timer_charge = nil
        self.firstCharge.gameObject:SetActive(false)
        return false
    else
        local h = math.floor(self.count_time / 3600)
        local m = math.floor((self.count_time % 3600) / 60)
        local s = math.floor((self.count_time % 3600) % 60)
        self.txt_firstCharge.text = string.format("%02d:%02d:%02d",h,m,s)
        self.count_time = self.count_time - 1
        return true
    end
end

function MainUI:NewEmailAppear(hasUnread)
    self.email_red.gameObject:SetActive(hasUnread)
end

function MainUI:onClick(go, name)
    if name == "btn_head" then
        App.Notice(AppMsg.PersonalShow)
    elseif name == "btn_addCoin" then
        --App.Notice(AppMsg.CashInShow)
    elseif name == "btn_shop" then
        --App.Notice(AppMsg.CashInShow)
    elseif name == "btn_support" then
        App.Notice(AppMsg.SupportShow)
    elseif name == "btn_email" then
        self.email_red.gameObject:SetActive(false)
        App.Notice(AppMsg.EmailShow)
    elseif name == "btn_setting" then
        App.Notice(AppMsg.SettingShow)
    elseif name == "btn_rule" then
        App.Notice(AppMsg.RuleShow,UriConst.ui_rule)
    elseif name == "btn_bonus" then
        App.Notice(AppMsg.BindShow)
    elseif name == "btn_vip" then
        App.Notice(AppMsg.VipShow)
    elseif name == "btn_invite" then
        App.Notice(AppMsg.InviteShow)
    elseif name == "btn_firstCharge" then
        App.Notice(AppMsg.FirshChargeShow)
    elseif name == "btn_daily" then
        App.Notice(AppMsg.DailyShow)
    end
end

return MainUI