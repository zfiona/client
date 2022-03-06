local AwardUI = class("AwardUI", BaseUI)

function AwardUI:ctor()
    self:load(UriConst.ui_award, UIType.PopUp, UIAnim.None)
end

function AwardUI:Refresh()
	AudioManager:PlayClick(UriConst.audio_gold)
	self.Text.text = FormatCoins(self.data)
end

function AwardUI:onClick(go, name)
	self:closePage()
end

return AwardUI