local RoomItem = class("RoomItem", BaseUI)

function RoomItem:init()
    
end

--msg {game_id,cell_score,limit_enter,roomid}
function RoomItem:update(index)
	local mainUIData = Games[App.RetrieveMediator("MainMediator")._roomPage.data]
	self.data = mainUIData[index]
    self.txt_base.text = FormatCoins(self.data.base_score)
    self.txt_need.text = FormatCoins(self.data.min_score)
end

function RoomItem:onClick(go, name)
	log("enter game " .. self.data.game_id .."_".. self.data.room_id)
	if self.data.game_id == 4 then
		EnterGame(self.data.game_id)
	else
		if Player.money < self.data.min_score then
			App.Notice(AppMsg.DialogShow,"insufficient coins")
			return
		end
		App.RetrieveProxy("HallProxy"):EnterRoomReq(self.data.game_id,self.data.room_id)
	end
	App.Notice(AppMsg.RoomHide)
end

function RoomItem:clean()
    
end
return RoomItem