---------------------------------消息框-----------------------------

local DialogUI = class("DialogUI", BaseUI)

function DialogUI:ctor()
    self:load(UriConst.ui_dialog, UIType.PopUp, UIAnim.None)
end

function DialogUI:onClick(go,name)
    if name == "btn_cancel" or name == "btn_mask" then
        self:closePage()
        if self._onCancel then 
            self._onCancel()
        end
    elseif name == "btn_ok" then
        self:closePage()
        if self._onOk then 
            self._onOk()
        end
    end
end

function DialogUI:Refresh()
    self:setMsg(self.data)
end

function DialogUI:setMsg(msg)
    -- local text,tp,title,tips
    local text,tp,tips
    if type(msg) == "string" then 
        text = msg
        tp = DialogType.NoButton
        self._onOk = nil
        self._onCancel = nil
    else
        text = msg.msg
        tp = msg.type or DialogType.OneButton
        self._onOk = msg.onOk 
        self._onCancel = msg.onCancel 
        if self._onOk and self.onCancel then
            tp = DialogType.TwoButton
        end
    end
    -- logError(msg)
    if tp ~= DialogType.NoButton then 
        self.txt_content.text = text
        self.txt_title.text = msg.msg2 or ""

        self.box.gameObject:SetActive(true)
        self.btn_ok.gameObject:SetActive(true)
        self.btn_cancel.gameObject:SetActive(tp == DialogType.TwoButton)
        local pos = tp == DialogType.OneButton and Vector3(0,-166,0) or Vector3(160,-166,0)
        self.btn_ok.transform.localPosition = pos
    else
        self.box.gameObject:SetActive(false)
        self.txt_tip.text = text

        if self.tween then
            self.tween:Kill()
            GameObject.Destroy(self.copyRect.gameObject)
        end
        self.copyRect = Instantiate(self.tip,self.transform)
        self.tween = self.copyRect:DOAnchorPosY(150,2,false)
        self.tween:OnComplete(function ()
            self:closePage()
            GameObject.Destroy(self.copyRect.gameObject)
            self.tween = nil
        end)
    end
end

return DialogUI