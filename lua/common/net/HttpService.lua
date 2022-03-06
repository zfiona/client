--全局table，常驻内存
HttpService = {}
local this = HttpService
local BaseUrl = "http://admin.webgameoss.com"
local apiBacks = {}
local waitUrls = {}
local waitCount = 0
local http = nil

function HttpService.ShowWaitBox(num)
	waitCount = waitCount + num
	if waitCount == 1 then
		App.Notice(AppMsg.WaitingShow)
	end
	if waitCount <= 0 then
		waitCount = 0
		App.Notice(AppMsg.WaitingHide)
	end
end

function HttpService.RequestWaiting(api,tbParams,func)
	local url = BaseUrl .. api
	waitUrls[url] = 1
	this.ShowWaitBox(1)

	this.Request(api,tbParams,func)
end

function HttpService.Request(api,tbParams,func)
	local url = BaseUrl .. api
	if apiBacks[url] then return end
	if func then
		apiBacks[url] = func
	end

	if not http then
		http = CS.NetExtension.HttpRequest(this)
	end
	if tbParams then
		http:Post(url,tbParams)
	else
		http:Get(url)
	end
end

function HttpService.OnDone(url,text)
	log(text)
	local func = apiBacks[url]
	if func then 
		if text then
			local data = json.decode(text)
			func(data)
		else
			App.Notice(AppMsg.DialogShow,"Request Failed!")
		end
	end
	apiBacks[url] = nil

	if waitUrls[url] then
		this.ShowWaitBox(-1)
		waitUrls[url] = nil
	end
end