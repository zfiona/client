local VipItem = class("VipItem",BaseUI)

function VipItem:init()
end

--msg {idx,status,today,left_day,direct_get,day_get,price}
function VipItem:update(data)
	self.data = data
	self.txt_total.text = string.format("Total of %s ₹",FormatCoins(data.direct_get + data.reward_day * data.day_get))
	self.txt_directGet.text = string.format("•Get <color=yellow>%s</color> ₹ immediartly",FormatCoins(data.direct_get))
	self.txt_dayGet.text = string.format("•Get <color=yellow>%s</color> ₹ everyday",FormatCoins(data.day_get))
	self.txt_dayValid.text = string.format("•Valid for %s days",FormatCoins(data.reward_day))

	self.txt_left.text = data.left_day
	if data.status == 0 then
		self.btn_cost.gameObject:SetActive(true)
		self.btn_get.gameObject:SetActive(false)
		self.txt_cost.text = FormatCoins(data.price) .. "₹"
	else
		self.btn_cost.gameObject:SetActive(false)
		self.btn_get.gameObject:SetActive(true)
		self.btn_get.interactable = data.today == 0
		self.txt_get.text = data.today == 0 and FormatCoins(data.day_get) .. "₹" or "tomorrow"
	end
end

function VipItem:onClick(go, name)
	if Player.money < self.data.price then
		App.Notice(AppMsg.DialogShow,"Gold is not enough")
		return
	end
	local type = name == "btn_cost" and 1 or 2
	App.RetrieveProxy("MainProxy"):MonthGetReq(self.data.idx,type)
end


return VipItem