local DailyItem = class("DailyItem",BaseUI)

function DailyItem:ctor(pos)
	self.pos = pos
end

function DailyItem:update(index)
	self.gameObject.transform.localPosition = self.pos
	self.txt_day.text = index
	self.txt_coin.text = FormatCoins(Cache.signData.sign_reward[index])

	self.had_sign.gameObject:SetActive(index <= Cache.signData.sign_days)
	self.canSign = Cache.signData.can_sign and index == Cache.signData.sign_days + 1 
	self.can_sign.gameObject:SetActive(self.canSign)
end

function DailyItem:onClick()
	if self.canSign then
		App.RetrieveProxy("HallProxy"):SignGetReq()
	end
end

return DailyItem