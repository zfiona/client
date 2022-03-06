local InviteDetailItem = class("InviteDetailItem", BaseUI)

function InviteDetailItem:init()
    
end

--msg {userid,recharge,reward}
function InviteDetailItem:update(index,data)
	self.txt_friendID.text = data.userid
	self.txt_totalReward.text = FormatCoins(data.recharge)
	self.txt_available.text = FormatCoins(data.reward)
end

function InviteDetailItem:onClick(go, name)
	
end

return InviteDetailItem