local BaseNotify = class("BaseNotify")

function BaseNotify:ctor(nm, bd)
    self.Name = nm or "empty_name"
    self.Body = bd
end

return BaseNotify