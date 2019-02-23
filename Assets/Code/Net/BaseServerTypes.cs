using System.Collections.Generic;



namespace Net
{
    public enum HttpRequestMethod
    {
        HTTP_METHOD_UNKNOWN,
        HTTP_METHOD_GET,
        HTTP_METHOD_POST,
        HTTP_METHOD_PUT,
        HTTP_METHOD_DELETE,
        HTTP_METHOD_HEAD,
        HTTP_METHOD_TRACE,
        HTTP_METHOD_CONNECT,
        HTTP_METHOD_PATCH,
        HTTP_METHOD_OPTIONS,
    }

    public class HttpHeader
    {
        public int type;
        public bool isVaild;
        public HttpRequestMethod method;
        public string url;
        public string ver;
        public int stateCode;
        public string stateMsg;
        public Dictionary<string,string> fieldMap = new Dictionary<string, string>();
    }


    public enum GamePackType
    {
        MSG_CLOSED,
        MSG_HEART_BEAT,                       //心跳消息
        MSG_RESP_HEART_BEAT,                  //响应心跳
        MSG_GAME_REQ_JOIN_SERVERGROUP,       //游戏服务器请求加入服务器组
        MSG_GATE_REQ_JOIN_SERVERGROUP,       //大门服务器请求加入服务器组
        MSG_LOGIN_REQ_JOIN_SERVERGROUP,       //登录服务器请求加入服务器组
        MSG_DB_REQ_JOIN_SERVERGROUP,        //数据服务器请求加入服务器组
        MSG_CENTER_RESP_GAME_JOIN_SERVERGROUP, //中心服务器响应游戏服务器请求加入服务器组
        MSG_CENTER_RESP_GATE_JOIN_SERVERGROUP, //中心服务器响应大门服务器请求加入服务器组
        MSG_CENTER_RESP_DB_JOIN_SERVERGROUP, //中心服务器响应数据服务器请求加入服务器组
        MSG_CENTER_RESP_LOGIN_JOIN_SERVERGROUP, //中心服务器响应登陆服务器请求加入服务器组
        MSG_GATE_RESP_CENTER_REQ_CONNECT_GAMES, //大门服务器响应中心服务器请求连接游戏服务器
        MSG_GATE_RESP_CENTER_REQ_CONNECT_DBS, //大门服务器响应中心服务器请求连接数据服务器
        MSG_GATE_RESP_CENTER_REQ_CONNECT_LOGINS, //大门服务器响应中心服务器请求连接登录服务器
        MSG_LOGIN_RESP_CENTER_REQ_CONNECT_DBS, //登录服务器响应中心服务器请求连接数据服务器
        MSG_LOGIN_RESP_CENTER_REQ_CONNECT_GATES, //登录服务器响应中心服务器请求连接大门服务器
        MSG_DB_RESP_CENTER_REQ_CONNECT_GAMES, //数据服务器响应中心服务器请求连接游戏服务器
        MSG_CENTER_REQ_GATE_CONNECT_GAMES,   //中心服务器请求大门服务器连接游戏服务器 
        MSG_CENTER_REQ_LOGIN_CONNECT_GATES,  //中心服务器请求登录服务器连接大门服务器 
        MSG_CENTER_REQ_DB_CONNECT_GAMES,   //中心服务器请求数据服务器连接游戏服务器 
        MSG_CENTER_REQ_GATE_CONNECT_DBS,   //中心服务器请求大门服务器连接数据服务器 
        MSG_CENTER_REQ_GATE_CONNECT_LOGINS, //中心服务器请求大门服务器连接登录服务器 
        MSG_CENTER_REQ_LOGIN_CONNECT_DBS,  //中心服务器请求登录服务器连接数据服务器 
        MSG_GATE_REQ_GAME_EXCHANGE_MACHINEID,        //大门服务器请求游戏服务器交换机器ID
        MSG_GAME_REQ_GAME_EXCHANGE_MACHINEID,        //游戏服务器请求游戏服务器交换机器ID
        MSG_GAME_REQ_GATE_EXCHANGE_MACHINEID,       //游戏服务器请求大门服务器交换机器ID
        MSG_GAME_REQ_LOGIN_EXCHANGE_MACHINEID,      //游戏服务器请求登录服务器交换机器ID
        MSG_GATE_REQ_GATE_EXCHANGE_MACHINEID,       //大门服务器请求大门服务器交换机器ID
        MSG_GATE_REQ_LOGIN_EXCHANGE_MACHINEID,      //大门服务器请求登录服务器交换机器ID
        MSG_LOGIN_REQ_GATE_EXCHANGE_MACHINEID,      //登录服务器请求大门服务器交换机器ID
        MSG_LOGIN_REQ_LOGIN_EXCHANGE_MACHINEID,     //登录服务器请求登录服务器交换机器ID
        MSG_DB_REQ_GATE_EXCHANGE_MACHINEID,         //数据服务器请求大门服务器交换机器ID
        MSG_DB_REQ_LOGIN_EXCHANGE_MACHINEID,        //数据服务器请求登录服务器交换机器ID
        MSG_DB_REQ_DB_EXCHANGE_MACHINEID,           //数据服务器请求数据服务器交换机器ID
        MSG_LOGIN_REQ_DB_EXCHANGE_MACHINEID,        //登录服务器请求数据服务器交换机器ID
        MSG_GAME_REQ_DB_EXCHANGE_MACHINEID,          //游戏服务器请求数据服务器交换机器ID
        MSG_GATE_REQ_DB_EXCHANGE_MACHINEID,          //大门服务器请求数据服务器交换机器ID
        MSG_DB_REQ_GAME_EXCHANGE_MACHINEID,         //数据服务器请求游戏服务器交换机器ID
        MSG_LOGIN_REQ_GAME_EXCHANGE_MACHINEID,      //登录服务器请求游戏服务器交换机器ID
        MSG_DB_RESP_LOGIN_REQ_EXCHANGE_MACHINEID,   //数据服务器响应登录服务器请求交换机器ID
        MSG_GATE_RESP_LOGIN_REQ_EXCHANGE_MACHINEID, //大门服务器响应登录服务器请求交换机器ID
        MSG_GAME_RESP_GATE_REQ_EXCHANGE_MACHINEID,  //游戏服务器响应大门服务器请求交换机器ID
        MSG_DB_RESP_GATE_REQ_EXCHANGE_MACHINEID,    //数据服务器响应大门服务器请求交换机器ID
        MSG_GAME_RESP_DB_REQ_EXCHANGE_MACHINEID,    //游戏服务器响应数据服务器请求交换机器ID
        MSG_LOGIN_RESP_GATE_REQ_EXCHANGE_MACHINEID, //登录服务器响应大门服务器请求交换机器ID

        MSG_DB_NOTICE_CENTER_PLAYER_AMOUNT,            //数据服务器通知中心服务器它服务器上的当前人数
        MSG_GAME_NOTICE_CENTER_PLAYER_AMOUNT,          //游戏服务器通知中心服务器它服务器上的当前人数
        MSG_GATE_NOTICE_CENTER_PLAYER_AMOUNT,          //大门服务器通知中心服务器它服务器上的当前人数
        MSG_GAME_NOTICE_CENTER_CPU_USAGE,              //游戏服务器通知中心服务器它服务器cpu的使用率
        MSG_GATE_NOTICE_CENTER_CPU_USAGE,              //大门服务器通知中心服务器它服务器cpu的使用率
        MSG_DB_NOTICE_CENTER_CPU_USAGE,                //数据服务器通知中心服务器它服务器cpu的使用率
        MSG_CENTER_NOTICE_GATE_GAME_PLAYER_AMOUNT,     //中心服务器通知大门服务器游戏服务器上的当前人数
        MSG_CENTER_NOTICE_LOGIN_GATE_PLAYER_AMOUNT,    //中心服务器通知登录服务器大门服务器上的当前人数
        MSG_CENTER_NOTICE_LOGIN_DB_PLAYER_AMOUNT,      //中心服务器通知登录服务器数据服务器上的当前人数
        MSG_CENTER_NOTICE_LOGIN_DB_CPU_USAGE,          //中心服务器通知登录服务器数据服务器的CPU使用率
        MSG_CENTER_NOTICE_GATE_DB_CPU_USAGE,           //中心服务器通知大门服务器数据服务器的CPU使用率
        MSG_CENTER_NOTICE_GAME_DB_CPU_USAGE,           //中心服务器通知游戏服务器数据服务器的CPU使用率

        MSG_CLIENT_NOTICE_LOGIN_SELF_INFO,           //游戏客户端通知登录服务器自身信息
        MSG_CLIENT_REQ_LOGIN_ENTER,                 //游戏客户端请求登陆服务器进入游戏    
        MSG_LOGIN_RESP_CLIENT_REQ_LOGIN,            //登陆服务器响应客户端登陆请求
        MSG_LOGIN_REQ_DB_VERIFY_CLIENT_LOGIN,       //登陆服务器请求数据服务器验证客户端登陆
        MSG_DB_RESP_LOGIN_VERIFY_CLIENT_LOGIN,      //数据服务器响应登陆服务器验证客户端登陆

        MSG_CLIENT_REQ_GATE_ACCEPT_SELF,              //游戏客户端请求大门服务器接受自己
        MSG_GATE_REQ_DB_VERIFY_CLIENT_LEGAL,          //大门服务器请求数据服务器验证客户端合法性
        MSG_DB_RESP_GATE_REQ_VERIFY_CLIENT_LEGAL,     //数据服务器响应大门服务器请求验证客户端合法性
        MSG_GATE_RESP_CLIENT_REQ_ACCEPT_SELF,         //大门服务器响应游戏客户端请求接受自己

        MSG_DB_REQ_GATE_KICK_CLIENT,                  //数据服务器请求大门服务器踢客户端下线
        MSG_GATE_RESP_DB_KICK_CLIENT,                 //大门服务器响应数据服务器踢客户端下线

        MSG_GATE_NOTICE_CLIENT_KICK_OFFLINE,          //大门服务器通知客户端被踢除下线
    }


    public enum DefValue
    {
        DEF_CLIENT_COMMON_LOGIN,                  //客户端普通登录           
        DEF_CLIENT_TOKEN_LOGIN,                   //客户端token登录
        DEF_LOGIN_SUCCESS,                        //登陆成功
        DEF_LOGIN_FAILD,                          //登陆失败
        DEF_NEED_AGAIN_CONNECT_SERVER,            //需要重新连接服务器
        DEF_VERIFY_SUCESS,                        //验证成功
        DEF_VERIFY_FAILD,                         //验证失败
        DEF_CLIENT_HAV_LEGAL,                     //客户端合法
        DEF_CLIENT_HAV_NO_LEGAL,                  //客户端不合法
        DEF_KICK_CLIENT_MORE_SAME_CLIENT_ONLINE,  //踢除客户端:此帐号多人同时在线
        DEF_KICK_CLIENT_SUCCESS,                  //踢除客户端成功
        DEF_KICK_CLIENT_FAILD,                    //剔除客户端失败
        DEF_CREATE_PLAYER_FAILD,                  //建立玩家信息失败
        DEF_CREATE_PLAYER_SUCCESS,                //建立玩家信息成功 
    }


    public class GamePackHeader
    {
        public GamePackType type;
        public short dataSize;
    };



}
