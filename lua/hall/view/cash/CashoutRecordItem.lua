local CashoutRecordItem = class("CashoutRecordItem", BaseUI)

function CashoutRecordItem:init()
    
end

--msg {id,create_time,cash_out_num,status}
function CashoutRecordItem:update(index,data)
	self.txt_requestTime.text = os.date("%Y-%m-%d",data.create_time)
    self.txt_orderId.text = data.id
    self.txt_amount.text = FormatCoins(data.cash_out_num)
    self.txt_status.text = self:GetStatus(data.status)
end

function CashoutRecordItem:GetStatus(status)
    if status == 0 then
        return "processing"
    elseif status == 1 then
        return "success"
    else
        return "failed"
    end
end

return CashoutRecordItem