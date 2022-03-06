function Handler(func,target,para)
    if para ~= nil then
        return function(...) 
            return func(target,para,...) 
        end
    else
        return function(...) 
            return func(target, ...) 
        end
    end
end

function removeAllChild(parent)
    local tran = parent.transform
    if tran.childCount == 0 then return end
    for i= tran.childCount-1,0,-1 do
        GameObject.Destroy(tran:GetChild(i).gameObject)
    end
end

function PathsInToTable(paths, tab)
    for i,v in ipairs(paths) do
        local ps = string.split(v,"/")
        local key = ps[#ps]
        tab[key] = v      
    end
end

function formatUrl(tab)
    local str = '?'
    for i,v in pairs(tab) do
        str = str .. i .. '=' .. v ..'&'
    end
    return string.sub(str, 1, #str-1)
end

function timeformat(exp_rest_time)
    local  off_hours = nil
    local  off_mins = nil
    local  off_secs = nil
    local  display_time_string = ""

    -- 转小时
    off_hours = math.floor(exp_rest_time / 3600)
    -- 转分钟
    off_mins  = math.floor((exp_rest_time % 3600) / 60)
    -- 转秒
    off_secs  = math.floor((exp_rest_time % 3600) % 60)

    if off_hours > 0 then
        display_time_string = off_hours .. _("SYSTEM.HOURS")
    end
    if off_mins > 0 then
        display_time_string = display_time_string .. off_mins .. _("SYSTEM.MINS")
    end
    if off_secs > 0 then
        display_time_string = display_time_string .. off_secs .. _("SYSTEM.SECS")
    end
    return display_time_string
end

----table转换字符串
function tostringex(v, len)
    if len == nil then 
        len = 0 
    end
    
    local pre = string.rep('  ', len+1)
    local pre1 = string.rep('  ', len)
    local ret = len==0 and "" or "  "
    if type(v) == "table" then
        if len > 10 then return "{ ... }" end
        local t = ""
        for k, v1 in pairs(v) do
            t = t .. "\n" .. pre .. (type(tonumber(k))=="number" and string.format("[%s]",k) or tostring(k) ) .. "  ="
            if type(v1) == "table" then
                t = t .. tostringex(v1, len+1) .. ","
            else
                t = t .. "  " .. (type(v1)=="string" and string.format("'%s'",v1) or tostring(v1) ) .. ","
            end
        end
        if t == "" then
            ret = ret .. "{}"
        else
            ret = ret .. "{" .. t .. "\n" .. pre1 .. "}"
        end
    else
        --value是string
        ret = ret .. (type(v)=="string" and string.format("'%s'",v) or tostring(v) ) .. ","
    end
    return ret
end

function traceback()
    local level = 3
    local ret = ""
    while true do
        local info = debug.getinfo(level, "Sln")
        if not info then
            break
        end
        if info.what == "C" then -- is a C function?
            ret = string.format("%s\n %s,%s",ret, level, "C function")
        else -- a Lua function
            ret = (string.format("%s\n [%s][%s]:%s",ret,info.short_src,info.name, info.currentline))
        end
        level = level + 1
    end
    return ret
end

function getLogStr(fmt, ...)    
    --fmt
    if type(fmt)=="table" then
        fmt = tostringex(fmt)
    end
    if type(fmt) == "boolean" or type(value) == "number" or type(value) == "nil" then
        fmt = tostring(fmt)
    end
    --sub
    local sub = ""
    for _, v in ipairs{...} do
        if type(v)=="table" then
            v = tostringex(v, len)
        end
        if type(v) == "boolean" or type(v) == "number" or type(value) == "nil" then
            v = tostring(v)
        end
        sub = sub .. " , " .. v
    end   
    --string合并
    if sub ~= "" then
        fmt = string.format("%s%s",fmt,sub)
    end
    --替换关键字符
    fmt = string.gsub(fmt,":","：")
    --debug追踪
    local x = debug.traceback()
    local arr = string.split(x,"\n")
    for i=4,#arr do 
        fmt = fmt  .. "\n" .. arr[i]
    end  
    return fmt
end

function log(fmt, ...)
    if GameDebug.logEnable then
        local msg = getLogStr(fmt, ...) or "nil"
        GameDebug.Log('[Lua] '..msg,false)
    end
end

function logWarning(fmt, ...)
    if GameDebug.logEnable then
        local msg = getLogStr(fmt, ...) or "nil"
        GameDebug.LogWarning('[Lua] '..msg,false)
    end
end

function logError(fmt, ...)
    if GameDebug.logEnable then
        local msg = getLogStr(fmt, ...) or "nil"
        GameDebug.LogError('[Lua] '..msg,false)
    end
end

function logGreen(fmt, ...)
    if GameDebug.logEnable then
        local msg = getLogStr(fmt, ...) or "nil"
        GameDebug.LogGreen('[Lua] '..msg,true)
    end
end

function logRed(fmt, ...)
    if GameDebug.logEnable then
        local msg = getLogStr(fmt, ...) or "nil"
        GameDebug.LogRed('[Lua] '..msg,true)
    end
end

function logYellow(fmt, ...)
    if GameDebug.logEnable then
        local msg = getLogStr(fmt, ...) or "nil"
        GameDebug.LogYellow('[Lua] '..msg,true)
    end
end


function setSize(go,size)
    local tran = go.gameObject:GetComponent("RectTransform")
    tran.sizeDelta = size
end

function setPivot(child,pivot)
    local tran = child.transform
    tran.pivot = pivot
    tran.anchorMin = pivot
    tran.anchorMax = pivot
end

function setPivotxy(child,x,y)
    local tran = child.gameObject:GetComponent("RectTransform")
    tran.pivot = Vector2(x,y)
end

function setPosX(go,posX)
    local tran = go.transform
    local pos = tran.anchoredPosition
    tran.anchoredPosition = Vector2(posX,pos.y)
end

function setPosY(go,posY)
    local tran = go.transform
    local pos = tran.anchoredPosition
    tran.anchoredPosition = Vector2(pos.x,posY)
end

function findChild(parent,path)
    local go = nil
    go = parent.transform:Find(path)
    return go
end

function findType(go,path,tp)
    if path then
        go = go.transform:Find(path)
    end
    if not go then
        return nil
    end
    local com = go:GetComponent(tp)
    return com
end

function Instantiate(prefab,parent)
    local comp = GameObject.Instantiate(prefab,parent)
    comp.transform.localPosition = Vector3.zero
    comp.transform.localScale = Vector3.one
    comp.gameObject:SetActive(true)
    return comp
end

function FormatCoins(coin)
    local val = coin / 100
    local integer,decimals = math.modf(val)
    if decimals < 0.1 then
        return integer
    else
        return integer + math.floor(decimals*10)*0.1
    end
end

function SecretName(name)
    local tmp = ""
    local len = string.len(name)
    if len > 3 then
        tmp = string.sub(name,1,2) .. "**" .. string.sub(name,len-1,len)
    else
        tmp = string.sub(name,1,1) .. "**"
    end
    return tmp
end

function LoadHead(headId)
    local path = UriConst.icon_HeadImg .. headId
    return ResourceMgr:LoadRes(path)
end