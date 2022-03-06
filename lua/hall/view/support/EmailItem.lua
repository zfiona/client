local EmailItem = class("EmailItem", BaseUI)

function EmailItem:init()
    
end

--msg {mail_id,title,info,item_num,status 0未读1已读,create_time}
function EmailItem:update(index)
	self.data = Cache.emailList[index]

    self.txt_info.text = self.data.title
    self.txt_sendTime.text = os.date("%B %d,%Y",self.data.create_time)
    local path = UriConst.icon_email_state .. (self.data.status + (self.data.item_num > 0 and 2 or 0))
    self.img_icon.sprite = ResourceMgr:LoadRes(path)
    if self.data.status == 0 then
        self.img_bg.material = nil
    else
        self.img_bg.material = ResourceMgr:LoadMaterial(UriConst.mat_grey)
    end
end

function EmailItem:onClick(go, name)
    App.Notice(AppMsg.EmailDetailShow,self.data)
end

function EmailItem:clean()
    
end

return EmailItem