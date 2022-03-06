local WaitingUI = class("WaitingUI", BaseUI)

function WaitingUI:ctor()
    self:load(UriConst.ui_waiting, UIType.PopUp, UIAnim.None)
end

function WaitingUI:Refresh()
	self.data = self.data or 8000
	if self.data > 0 then
		self.m_handle = LuaTimer.Add(self.data, function()
	        App.Notice(AppMsg.DialogShow,"The request timeout！") 
	        App.Notice(AppMsg.WaitingHide)
	    end) 
	end
end

function WaitingUI:Hide()
	if self.m_handle then
		LuaTimer.Delete(self.m_handle)
	end
end

return WaitingUI