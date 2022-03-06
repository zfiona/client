syntax = "proto3";
option go_package="./;msg";
package msg;

message CardItem
{
    uint32 value = 1; //2-14 A为14
    uint32 color = 2; //鬼桃杏梅方 5-1
}

message PlayerItem
{
    uint32 userid = 1;
    string name = 2;
    uint32 head = 3;
	uint32 money = 4;
	string phone = 5;
	uint32 sign_days = 6;
	bool has_new_email = 7;
}

message RoomItem
{
    uint32 game_id = 1;
    uint32 room_id = 2;
	uint32 base_score = 3;
	uint32 min_score = 4;
}

//统一错误弹框
//ID:10000
message Hall_SC_Error
{
	uint32 code = 1; //0文字弹框 1单按钮弹框 2双按钮弹框
	string msg = 2;
}

//ID:10001
message Hall_CS_Login
{
	string mac_address = 1;
	string phone_num = 2;
	string password = 3;
	string fb_token = 4;
}

//ID:10002
message Hall_SC_Login
{
	PlayerItem p_info = 1;
	repeated RoomItem r_infos = 2;
	uint32 game_id = 3; //是否在游戏中
}

//ID:10003
message Hall_CS_ChangeName
{
	string nick_name = 1;
}

//ID:10004
message Hall_CS_ChangeHead
{
    uint32 head_id = 1;
}

//更新玩家信息
//ID:10005
message Hall_SC_UpdatePlayer
{
	string name = 1;
	uint32 head = 2;
	string phone = 3;
}

//更新玩家金币
//ID:10006
message Hall_SC_UpdateCoins
{
	uint32 coins = 1;
	int32 modify = 2;
	uint32 reason = 3; //1000以上弹动效
}

//ID:10007
message Hall_CS_Bind
{
    string phone_num = 1;
	string password = 2;
	string code = 3;
}

//签到
//ID:10010
message Hall_CS_SignInfo
{
	
}
//ID:10011
message Hall_CS_Sign
{
	
}
//ID:10012
message Hall_SC_SignInfo
{
	uint32 sign_days = 1;
	bool can_sign = 2;
	repeated uint32 sign_reward = 3;
}

//邮件
message EmailItem
{
	uint32 mail_id = 1;
	string title = 2;
	string info = 3;
	uint32 create_time = 4;
	bool status = 5;//0未读1已读
	uint32 item_num = 6; //是否可领取金币
}
//ID:10020
message Hall_CS_EmailInfo
{
	
}
//ID:10021
message Hall_CS_ReadEmail
{
	uint32 email_id = 1;
}
//ID:10022
message Hall_CS_ClearEmail
{
	
}
//ID:10023
message Hall_SC_EmailInfo
{
	bool  has_mail_noRead = 1;
	repeated EmailItem emails = 2;
}


//商店 先丢客户端
//ID:10030
message Hall_CS_Buy
{
	string sku_id = 1;
	uint32 coins = 2;
}

//ID:10031
message Hall_SC_Buy
{
	string order_id = 1; //由playId,sku_id,create_time,coins组成
}

//ID:10032
message Hall_CS_BuySuccess
{
	string order_id = 1;
}

//房间
//ID:10040
message Hall_CS_EnterRoom
{
    uint32 game_id = 1;
    uint32 room_id = 2; 
}
//ID:10041
message Hall_SC_EnterRoom
{
    uint32 game_id = 1;
}
//ID:10042
message Hall_CS_Chat
{
    int32 receiver = 1;
    uint32 face_id = 2; 
}
//ID:10043
message Hall_SC_Chat
{
    int32 receiver = 1;
    uint32 face_id = 2; 
	uint32 sender = 3;
}