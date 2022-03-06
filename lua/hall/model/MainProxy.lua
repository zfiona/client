local MainProxy = class("MainProxy", BaseProxy)
local ScoketService = ScoketService

----------------------------CS 部分 消息驱动-------------------------------------

function MainProxy:SignOpenReq()
    ScoketService.SendMessage(NetCmd.signOpenReq)
end

function MainProxy:SignGetReq(day)
    local data = {}
    data.day = day
    ScoketService.SendMessage(NetCmd.signGetReq,data)
end

function MainProxy:MonthOpenReq()
    ScoketService.SendMessage(NetCmd.monthOpenReq)
end

function MainProxy:MonthGetReq(vip_id,type)
    local data = {}
    data.vip_id = vip_id
    data.type = type
    ScoketService.SendMessage(NetCmd.monthGetReq,data)
end

function MainProxy:NameChangeReq(name)
    local data = {}
    data.new_name = name
    ScoketService.SendMessage(NetCmd.nameChangeReq,data)
end

function MainProxy:HeadChangeReq(headId)
    local data = {}
    data.head = headId
    ScoketService.SendMessage(NetCmd.headChangeReq,data)
end

function MainProxy:BindPhoneReq(phoneStr,pwdStr,verifyStr)
    if string.len(phoneStr) < 9 or string.len(phoneStr) > 11 then
        App.Notice(AppMsg.DialogShow,"Please input 9-11 digit mobile phone number")
        return
    end
    if string.len(pwdStr) < 6 or string.len(pwdStr) > 20 then
        App.Notice(AppMsg.DialogShow,"Please enter a 6-20 digit password")
        return
    end
    if string.len(verifyStr) ~= 4 and string.len(verifyStr) ~= 6 then
        App.Notice(AppMsg.DialogShow,"Please enter a 4 or 6 digit verification code")
        return
    end
    local data = {}
    data.phone = phoneStr
    data.code = verifyStr
    data.pwd = pwdStr
    ScoketService.SendMessage(NetCmd.bindPhoneReq,data)
end

function MainProxy:InviteOpenReq(type)
    local data = {}
    data.type = type
    ScoketService.SendMessage(NetCmd.inviteOpenReq,data)
end

function MainProxy:InviteGetReq(type)
    local data = {}
    data.type = type
    ScoketService.SendMessage(NetCmd.inviteGetReq,data)
end

function MainProxy:EmailListReq()
    ScoketService.SendMessage(NetCmd.emailListReq)
end

function MainProxy:EmailReadReq(id)
    local data = {}
    data.mail_id = id
    ScoketService.SendMessage(NetCmd.emailReadReq,data)
end

function MainProxy:EmailClearReq(ids)
    local data = {}
    data.id_list = ids
    ScoketService.SendMessage(NetCmd.emailRemoveReq,data)
end

function MainProxy:RechargeOpenReq()
    ScoketService.SendMessage(NetCmd.rechargeOpenReq)
end

function MainProxy:RechargeOrderReq(index,pay_type)
    local data = {}
    data.recharge_index = index
    data.pay_type = pay_type
    ScoketService.SendMessage(NetCmd.rechargeOrderReq,data)
end

function MainProxy:CashoutOpenReq(type)
    local data = {}
    data.type = type
    ScoketService.SendMessage(NetCmd.cashoutOpenReq,data)
end

function MainProxy:CashoutBindReq(info)
    local data = {}
    data.cash_out_info = info
    ScoketService.SendMessage(NetCmd.cashoutBindReq,data)
end

function MainProxy:CashoutReq(num)
    local data = {}
    data.cash_out_num = num
    ScoketService.SendMessage(NetCmd.cashoutReq,data)
end

function MainProxy:FirstChargeOpenReq()
    ScoketService.SendMessage(NetCmd.firstChargeOpenReq)
end

function MainProxy:ChargeBindReq(info)
    local data = {}
    data.info = info
    ScoketService.SendMessage(NetCmd.bindChargeReq,data)
end

----------------------------SC 部分-------------------------------------
function MainProxy:ShowDialog(data)
    if data.show_type == 0 then
        App.Notice(AppMsg.DialogShow,data.info)
    elseif data.show_type == 1 then
        local msg = {}
        msg.msg = data.info
        App.Notice(AppMsg.DialogShow,msg)
    else
        local msg = {}
        msg.msg = data.info
        msg.type = DialogType.TwoButton
        msg.onOk = function ()
            App.Notice(data.ui_id)
        end
        App.Notice(AppMsg.DialogShow,msg)
    end
end

function MainProxy:NetBack(subId,data)
    log(data)
    if subId == NetCmd.sysInfoAck.subId then
        self:ShowDialog(data)
    elseif subId == NetCmd.signRefreshRsp.subId then
        Cache.signData = data
        App.Notice(AppMsg.UpdateDailyInfo)
    elseif subId == NetCmd.monthRefreshRsp.subId then
        Cache.vipList = shallowcopy(data.items)
        App.Notice(AppMsg.UpdateVipInfo)
    elseif subId == NetCmd.nameChangeRsp.subId then
        Player.name = data.new_name
        App.Notice(AppMsg.UpdatePlayInfo)
        App.Notice(AppMsg.UserNameHide)
    elseif subId == NetCmd.headChangeRsp.subId then
        Player.head = data.head
        App.Notice(AppMsg.UpdatePlayInfo)
        App.Notice(AppMsg.UserHeadHide)
    elseif subId == NetCmd.bindRefreshRsp.subId then
        if data.result == 1 then
            Player.phone = data.phone
            --通用加钱消息
        else
            App.Notice(AppMsg.DialogShow,"Bind failed")
        end
    elseif subId == NetCmd.inviteRewardsAck.subId then
        Cache.invite_allRewards = data.total_reward
        App.Notice(AppMsg.UpdateInviteInfo)
    elseif subId == NetCmd.inviteListAck.subId then
        Cache.invite_myTotal = data.total_reward
        Cache.invite_myAvaile = data.available_reward
        Cache.invite_items[data.type] = shallowcopy(data.items)
        App.Notice(AppMsg.UpdateInviteDetailInfo)
    elseif subId == NetCmd.rechargeRefreshRsp.subId then
        Cache.pay_type = shallowcopy(data.op)
        Cache.pay_items = {}
        local mid_index = #data.items/2
        for i=1,mid_index do
            table.insert(Cache.pay_items,data.items[i])
            table.insert(Cache.pay_items,data.items[mid_index+i])
        end
        App.Notice(AppMsg.UpdateCashInInfo)
    elseif subId == NetCmd.rechargeOrderRsp.subId then
        if data.result == 1 then
            SDKManager:OpenWebView(data.order_url)
        end
    elseif subId == NetCmd.emailRefreshRsp.subId then
        Cache.emailList = shallowcopy(data.items)
        local hasUnreadEmail = false
        for i,v in ipairs(Cache.emailList) do
            if v.status == 0 then
                hasUnreadEmail = true
                break
            end
        end
        App.Notice(AppMsg.UpdateEmailInfo,hasUnreadEmail)  
    elseif subId == NetCmd.cashoutInfoRsp.subId then
        Cache.cashoutInfo = deepcopy(data)
        App.Notice(AppMsg.UpdateCashoutInfo)
    elseif subId == NetCmd.cashoutRecordRsp.subId then
        Cache.cashoutRecords = shallowcopy(data.items)
        App.Notice(AppMsg.UpdateCashoutRecords)
    elseif subId == NetCmd.firstChargeRefreshRsp.subId then
        Cache.firstChargeItem = shallowcopy(data.item)
        Cache.pay_type = shallowcopy(data.op)
        Player.first_charge_left_time = data.left_time
        App.Notice(AppMsg.UpdateFirstCharge)
    elseif subId == NetCmd.bindChargeRsp.subId then
        Player.info = data.info
    end
end

-----------------------注册和注销 回调函数---------------------------------------
function MainProxy:OnRegister()
    ScoketService.AddCallback(NetCmd.sysInfoAck, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.signRefreshRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.monthRefreshRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.nameChangeRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.headChangeRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.bindRefreshRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.inviteRewardsAck, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.inviteListAck, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.rechargeRefreshRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.rechargeOrderRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.emailRefreshRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.cashoutInfoRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.cashoutRecordRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.firstChargeRefreshRsp, Handler(self.NetBack, self))
    ScoketService.AddCallback(NetCmd.bindChargeRsp, Handler(self.NetBack, self))
end

function MainProxy:OnRemove() 
    ScoketService.RemoveCallBack(NetCmd.sysInfoAck, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.signRefreshRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.monthRefreshRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.nameChangeRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.headChangeRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.bindRefreshRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.inviteRewardsAck, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.inviteListAck, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.rechargeRefreshRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.rechargeOrderRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.emailRefreshRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.cashoutInfoRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.cashoutRecordRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.firstChargeRefreshRsp, Handler(self.NetBack, self))
    ScoketService.RemoveCallBack(NetCmd.bindChargeRsp, Handler(self.NetBack, self))
end

return MainProxy