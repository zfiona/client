local DailyUI = class("DailyUI", BaseUI)
local DailyItem = App.RequireHall("view/bonus/DailyItem")
local posList = {
    [1] = Vector3(-510,200),
    [2] = Vector3(-170,200),
    [3] = Vector3(170,200),
    [4] = Vector3(510,200),
    [5] = Vector3(-380,-120),
    [6] = Vector3(0,-120),
    [7] = Vector3(380,-120)
}

function DailyUI:ctor()
    self:load(UriConst.ui_daily, UIType.Fixed, UIAnim.MiddleAppear)
end

function DailyUI:Awake()
    self.item.gameObject:SetActive(false)
    self.pageItems = {}
    for i,v in ipairs(posList) do
        local bind = Instantiate(self.item,self.root)
        local page = DailyItem:create(v)
        bind:Init(page)
        table.insert(self.pageItems,page)
    end
end

function DailyUI:Refresh()
    self.btn_get.gameObject:SetActive(Const.isYD)
    App.RetrieveProxy("HallProxy"):SignOpenReq()
end

function DailyUI:UpdateData()
    if not table.isEmpty(Cache.signData) then
        for i,v in ipairs(self.pageItems) do
            v:update(i)
        end
    end
end

function DailyUI:onClick(go, name)
    if name == "btn_exit" or name == "btn_mask" then
        self:closePage()
    elseif name == "btn_get" then
        self:closePage()
        App.Notice(AppMsg.CashInShow)
    end
end

return DailyUI