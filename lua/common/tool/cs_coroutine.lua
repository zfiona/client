local util = require 'common.tool.util'
local corMap = {}

return{
	start = function(...)
        local cor = GameController:StartCoroutine(util.cs_generator(...))
        table.insert(corMap, cor)
	    return cor
    end,
    
	stop = function(cor)
	    GameController:StopCoroutine(cor)
    end,

    stopAll = function()
        for k, v in pairs(corMap) do
            GameController:StopCoroutine(v)
        end
    end,

    waitSec = function(time)
        coroutine.yield(CS.UnityEngine.WaitForSeconds(time))
    end
}