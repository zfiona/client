syntax = "proto3";
option go_package="./;msg";

package msg;

message Tarneeb_GameInfo
{
	uint32 seat_d = 1;  //庄家
	CardItem card_d = 2; //底牌
}

message Tarneeb_RoomInfo
{
	uint32 game_id = 1;
    uint32 room_id = 2;
    uint32 opt_time = 3; //操作时长
}

message Tarneeb_PlayerInfo
{
	PlayerItem base_info = 1;
	uint32 seat_id = 2;
	uint32 score = 3;
}


enum StageEnum
{
	StageEnum_Wait = 0; //等待
	StageEnum_Call = 1; //叫分
	StageEnum_Color = 2; //定花色
	StageEnum_Send = 3; //出牌
}

//------------Client to Server------------------------
//匹配请求
//ID:30001
message Tarneeb_CS_Match
{
    uint32 game_id = 1;
	uint32 room_id = 2;
}

//叫分请求
//ID:30002
message Tarneeb_CS_Call
{
    uint32 score = 1; 
}

//定花色请求
//ID:30003
message Tarneeb_CS_Color
{
    uint32 color = 1;
}

//出牌请求
//ID:30004
message Tarneeb_CS_Send
{
    CardItem card = 1;
}

//退出相关
//ID:30005
message Tarneeb_CS_Quit
{
	enum Enum
	{
		request = 0;
		refuse = 1;
		agree = 2;
	}
	Enum status = 1;
}

//------------Server to Client------------------------
//匹配返回通知
//ID:31001
message Tarneeb_SC_Match
{
    Tarneeb_RoomInfo r_info = 1; //房间信息
	Tarneeb_GameInfo g_info = 2;  //庄家
	repeated Tarneeb_PlayerInfo players = 3; //玩家列表
	repeated CardItem cards = 4;  //手牌列表
}

//轮次广播(首轮单独，非首轮跟着叫分广播或者出牌广播一起)
//ID:31002
message Tarneeb_SC_Turn
{
    uint32 seat = 1;
	StageEnum stage = 2; 
}

//叫分广播
//ID:31003
message Tarneeb_SC_Call
{
	uint32 seat = 1;
	uint32 score = 2;//0表示pass
}

//花色结果广播
//ID:31004
message Tarneeb_SC_ColorResult
{
	uint32 seat = 1;
    uint32 color = 2;
}

//出牌广播
//ID:31005
message Tarneeb_SC_Send
{
	uint32 seat = 1;
    CardItem card = 2;
}

//出牌结果广播
//ID:31006
message Tarneeb_SC_SendResult
{
	uint32 seat_win = 1;
}

//结算广播
//ID:31007
message Tarneeb_SC_GameResult
{
	int32 score = 1; //最后得分，正赢负输
}

//退出相关
//ID:31008
message Tarneeb_SC_Quit
{
	enum Enum
	{
		normal = 0;
		team_request = 1;
		team_refuse = 2;
		team_agree = 3;
		player_flee = 4;
		server_dissolve = 5;
	}
	Enum status = 1;
	string flee_name = 2; //逃跑玩家
}