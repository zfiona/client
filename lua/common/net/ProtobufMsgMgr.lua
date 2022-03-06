ProtobufMsgMgr = {}
--pb解析
local pb = require 'pb'
local protoc = require 'common.tool.protoc'

local regDirs = {}
function ProtobufMsgMgr.register(protoName)
	if table.valueof(regDirs,protoNames) then
		log("已经注册了 "..protoNames)
		return
	end
	table.insert(regDirs,protoName)
    local buffer = CS.LuaHelper.LoadProtoFile(protoName)
    protoc:load(buffer,protoName)
end

--pbMsg名带package名,如lobby.pb_msg_CS_LOGIN_CMD
function ProtobufMsgMgr.encode(msgName, msg)
	if msgName ~= nil then
		return pb.encode(msgName, msg)
	end
	return nil
end

function ProtobufMsgMgr.decode(msgName, msg)	
	if msgName ~= nil then
		return pb.decode(msgName, msg)
	end
	return nil
end

function ProtobufMsgMgr.clear(msgName)	
	if msgName ~= nil then
		pb.clear(msgName)
	else
		pb.clear() --清除所有
	end
end

--直接解析字符串/Slice格式的二进制pb数据注册消息
function ProtobufMsgMgr.load(chunk)	
	pb.load(chunk)
end