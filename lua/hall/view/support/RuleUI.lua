local RuleUI = class("RuleUI", BaseUI)
function RuleUI:ctor(path)
    local uiAnim = path == UriConst.ui_rule and UIAnim.DownToUp or UIAnim.MiddleAppear
    self:load(path, UIType.PopUp, uiAnim)
end

function RuleUI:Refresh()
    -- if self.data == UriConst.ui_rule then
    --     local isRummy = Const.game_id == 4
    --     self.tog_teen.isOn = not isRummy
    --     self.tog_rummy.isOn = isRummy
    -- end
end

function RuleUI:onClick(go, name)
    self:removePage()
end

function RuleUI:onValueChange(go,name,val)
    local key = string.gsub(name,"tog","part")
    log(key)
    self[key].gameObject:SetActive(val)
end

return RuleUI