syntax = "proto3";
option go_package="./;msg";

package msg;

message card_info
{
    uint32 seat = 1;
    uint32 card_type = 2;//0无，1高牌，2对子，3金花，4顺子，5顺金，6豹子
    repeated CardItem cards = 3;  //joke 第一张牌为53
    repeated CardItem end_cards = 4; //joke和ak47最终的变牌
}

message room_player
{
    uint32 userid = 1;
    string name = 2;
    uint32 head = 3;
    uint32 seat = 4;
    uint32 state = 5; //0旁观，盲牌，看牌，弃牌，比牌淘汰
    uint32 total_bets = 6;
	uint32 card_type = 7;
	repeated CardItem cards = 8;
}

message room_info
{
    uint32 cell_score = 1;
    uint32 max_blinds = 2;
    uint32 chaal_limit = 3;
    uint32 pot_limit = 4;
    uint32 max_round = 5; //最大局数
}

message game_info
{
    uint32 state = 1; //匹配阶段,操作阶段,比牌阶段
    uint32 round = 2;
    uint32 total_bets = 3;
    uint32 cur_bet_level = 4;  // cur_bets改成cur_bet_level,金额为2的level次方,从0开始
    uint32 cur_opt_seat = 5;
    uint32 cur_opt_time = 6;
    uint32 d_seat = 7; //庄家位置
    uint32 opt_time = 8; //常数 操作时间
    repeated bet_info bet_infos = 9; //用于还原桌面筹码
}

message bet_info
{
    uint32 level = 1;
    uint32 add_op = 2;
}

//ID:20000 
message ZJH_SC_Reconnect
{
    repeated room_player users = 1;
    room_info r_info = 2;
    game_info g_info = 3;
}

//ID:20001
message ZJH_SC_GameStart
{
    uint32 cur_turn = 1;
    uint32 op_time = 2;
    uint32 total_bets = 3;//底注总和
}


//ID:20002 
message ZJH_SC_AddScore
{
    uint32 seat = 1;
    uint32 add_op = 2; //0跟住  1加注.
    uint32 cur_level = 3;
    uint32 total_bets = 4; //桌面总额
    uint32 seat_total_bets = 5; //玩家下注总额
}

//ID:20003
message ZJH_SC_GiveUp
{
    uint32 seat = 1;
}


//ID:20004 
message ZJH_SC_GameEnd
{
    uint32 winner = 1;
    uint32 force_end = 2; //0正常结束,1强制比牌
    repeated card_info infos = 3;
}

//ID:20005 
message ZJH_SC_CompareCard
{
    uint32 u1_seat = 1; //发起比牌玩家
    uint32 u2_seat = 2;  //被比牌玩家
    uint32 show_op = 3; //0sideshow,1show
    uint32 total_bets = 4;
    uint32 u1_total_bets = 5; //玩家下注总额
}


//ID:20006 
message ZJH_SC_LookCard
{
    card_info info = 1;
}

//ID:20007 
message ZJH_SC_UserTurn
{
    uint32 seat = 1;
    uint32 round = 2;
}

//ID:20008
message ZJH_SC_CompareResult
{
    uint32 agree = 1;  //0拒绝  1同意.
    uint32 winner = 2;
    card_info infos1 = 3;
    card_info infos2 = 4;
}

//ID:20009 
message ZJH_SC_ReadyToStart
{
    uint32 tick_count = 1;   //倒计时
}

//ID:20010
message ZJH_SC_OtherEnter
{
    uint32 enter = 1; //0 离开 1进入
    uint32 seat = 2; 
    room_player player = 3; //进入的玩家信息
}


//恢复.
//ID:21000
message ZJH_CS_RecoverRoom
{
    
}

//加注 跟住
//ID:21001
message ZJH_CS_AddScore
{
    uint32 add_op = 1;  //0跟住  1加注
}

//弃牌 
//ID:21002
message ZJH_CS_GiveUp
{
    
}

//看牌
//ID:21003
message ZJH_CS_LookCard
{
    
}

//比牌
//ID:21004
message ZJH_CS_Compare
{
    
}

//比牌回复
//ID:21005
message ZJH_CS_Answer_Compare
{
    uint32 answer = 1;  //0拒绝 1同意
}

//ID:21006
message ZJH_CS_Exit
{
    
}