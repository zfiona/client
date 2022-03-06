local FirstChargeUI = class("FirstChargeUI", BaseUI)

function FirstChargeUI:ctor()
    self:load(UriConst.ui_firstCharge, UIType.PopUp,UIAnim.MiddleAppear)
end

function FirstChargeUI:Refresh()
    App.RetrieveProxy("MainProxy"):FirstChargeOpenReq()
end

function FirstChargeUI:Hide()
    if self.timer then
        LuaTimer.Delete(self.timer)
    end
end

function FirstChargeUI:UpdateData()
    if Player.first_charge_left_time then
        self.count_time = Player.first_charge_left_time
        self.timer = LuaTimer.Add(100,100,Handler(self.ShowTimer,self))
    end
end

function FirstChargeUI:ShowTimer()
    if self.count_time > 0 then
        self.count_time = self.count_time - 0.1
        local zs,xs = math.modf(self.count_time)
        local h = math.floor(zs / 3600)
        local m = math.floor((zs % 3600) / 60)
        local s = math.floor((zs % 3600) % 60)
        xs = math.floor(xs*100)
        self.txt_countdown.text = string.format("%02d:%02d:%02d:%02d",h,m,s,xs)
        return true
    else
        return false
    end
end

function FirstChargeUI:onClick(go, name)
    if name == "btn_exit" or name == "btn_mask" then
        self:closePage()
    elseif name == "btn_getCoin" then
        if table.isEmpty(Cache.pay_type) then
            App.Notice(AppMsg.DialogShow,"CashIn channel is under maintenance!")
        else
            App.Notice(AppMsg.CashChannelShow,Cache.firstChargeItem)
        end
    end
end

return FirstChargeUI