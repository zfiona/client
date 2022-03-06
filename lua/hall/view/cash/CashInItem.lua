local CashInItem = class("CashInItem", BaseUI)

function CashInItem:init()
    
end

--msg {index,base_num,actual_num,price,image_index,percent}
function CashInItem:update(index)
	self.data = Cache.pay_items[index]
    --self.img_coin.sprite = ResourceMgr:LoadRes(UriConst.icon_shop_coin .. self.data.image_index)  
    self.img_bg.sprite = ResourceMgr:LoadRes(UriConst.icon_shop_bg .. index % 2)  
    self.txt_actual.text = FormatCoins(self.data.actual_num)
    self.txt_price.text = "₹ ".. self.data.price

    if self.data.percent > 0 then
        self.txt_percent.text = self.data.percent .. "%"
        self.txt_base.text = FormatCoins(self.data.base_num)
        self.percent.gameObject:SetActive(true)
        self.txt_base.gameObject:SetActive(true)
        self:AddLine(self.txt_base)
    else
        self.percent.gameObject:SetActive(false)
        self.txt_base.gameObject:SetActive(false)
    end
end

local underLineText = "-"
function CashInItem:AddLine(text)
    local underline = Instantiate(text)
    underline.name = "Underline"
    underline.color = Color.grey
    underline.transform:SetParent(text.transform)
    underline.transform.localScale = Vector3.one

    local rt = underline.rectTransform
    rt.anchoredPosition3D = Vector3.zero
    rt.offsetMax = Vector2.zero
    rt.offsetMin = Vector2.zero
    rt.anchorMax = Vector2.one
    rt.anchorMin = Vector2.zero
    underline.text = underLineText
    local perlineWidth = underline.preferredWidth
    local width = text.preferredWidth
    local lineCount = math.floor(width / perlineWidth)
    for i=1,lineCount+2 do
        underline.text = underline.text .. underLineText
    end
end

function CashInItem:onClick(go, name)
    if table.isEmpty(Cache.pay_type) then
        App.Notice(AppMsg.DialogShow,"CashIn channel is under maintenance!")
    else
        App.Notice(AppMsg.CashChannelShow,self.data)
    end
end



return CashInItem