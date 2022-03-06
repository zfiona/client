local HeadUI = class("HeadUI", BaseUI)
function HeadUI:ctor()
    self:load(UriConst.ui_head, UIType.Fixed,UIAnim.DownToUp)
end

--call every show
function HeadUI:Awake()
    for i=1,10 do
        local head = Instantiate(self.headItem,self.Content)
        head.gameObject:SetActive(true)
        head:Find("mask/head"):GetComponent("Image").sprite = LoadHead(i)
        head:Find("btn_head"):GetComponent("Button").onClick:AddListener(Handler(self.OnItem,self,i))
    end
    self:OnItem(Player.head)
end


function HeadUI:OnItem(id)
    self.id = id
    self.chosedImg:SetParent(self.Content:GetChild(id-1))
    self.chosedImg.localPosition = Vector3.zero
    self.chosedImg.gameObject:SetActive(true)
end

function HeadUI:onClick(go, name)
    if name == "btn_ok" then
        App.RetrieveProxy("HallProxy"):HeadChangeReq(self.id)
    elseif name == "btn_exit" then
        self:closePage()
    end
end

return HeadUI