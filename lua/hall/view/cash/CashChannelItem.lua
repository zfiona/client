local CashChannelItem = class("CashChannelItem", BaseUI)

function CashChannelItem:init()
    
end

function CashChannelItem:update(index,data)
    self.index = data.index
    if string.isEmpty(data.image_url) then
	   self.img.sprite = ResourceMgr:LoadRes(UriConst.icon_pay .. data.name)
    else
        data.image_url = "https://rummy.oss-cn-beijing.aliyuncs.com/Image/icon_HeadImg_00.png"
        CS.NetExtension.HttpImage.AsyncLoad(data.image_url,function (sprite)
            self.img.sprite = sprite
        end)
    end
    self.tog.isOn = index == 0
end

function CashChannelItem:onValueChange(go,name,val)
    if val then
        App.Notice(AppMsg.UpdateCashChannel,self.index)
    end
end

return CashChannelItem