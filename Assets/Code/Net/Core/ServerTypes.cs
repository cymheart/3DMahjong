
namespace Net
{
    public enum DataTransMode
    {
        MODE_STREAM,
        MODE_PACK
    }

    public struct PackBuf
    {
        public int len;
        public byte[] buf;
    }

    public struct ServerTaskState
    {
        public int state;
        public int clientSize;
    }

    public enum ExtractState
    {
        /// <summary>
        /// 包头长度未获取
        /// </summary>
        ES_PACKET_HEADLEN_NOT_GET,  

        /// <summary>
        /// 已获取包头信息
        /// </summary>
        ES_PACKET_HEAD_FULL,     

        /// <summary>
        /// 包头信息不全
        /// </summary>
        ES_PACKET_HEAD_NOT_FULL, 
    }

    public enum SocketEvent
    {
        EV_SOCKET_OFFLINE,
        EV_SOCKET_PORT_BEUSED,
        EV_SOCKET_CONNECTED,
        EV_SOCKET_ACCEPTED,
        EV_SOCKET_SEND,
        EV_SOCKET_RECV
    }

    public enum NetSocketType
    {
        UNKNOWN_SOCKET,                  //未定SOCKET类型
        LISTENER_SOCKET,                 //本机监听SOCKET
        LISTEN_CLIENT_SOCKET,            //本机与远程客户端之间通连的SOCKET
        CONNECT_SERVER_SOCKET            //本机与远程远程服务器之间已连接的SOCKET
    }

    public enum SocketState
    {
        INIT_FAILD,             //初始化失败
        LISTEN_FAILD,           //监听失败
        LISTENING,              //监听所有过来的连接
        CONNECTED_CLIENT,       //已与客户端连接
        CONNECTTING_SERVER,     //与服务器连接中
        CONNECTED_SERVER,       //已连接服务器
        WAIT_REUSED,            //等待重用SOCKET
        NEW_CREATE,             //新生成的SOCKET
        ASSOCIATE_FAILD,        //绑定到完成端口失败
        BIND_FAILD,             //绑定端口失败
        LOCALIP_INVALID,        //无效的本地地址    
        PORT_BEUSED,            //端口被占用
        RESPONSE_TIMEOUT,       //响应超时
        RECV_DATA_TOO_BIG,      //接收的数据过大
        NORMAL_CLOSE,           //正常关闭
    }

    public enum ServerMessage
    {
        SMSG_LISTENER_CREATE_FINISH,    //监听器建立完成
        SMSG_ACCEPT_CLIENT,             //接收到一个客户端连接 
        SMSG_REQUEST_CONNECT_SERVER     //请求连接服务器
    }

    public struct ServerMsgTaskData
    {
        public ServerMessage msg;
        public ServerTaskMgr serverTaskMgr;
        public object dataPtr;
    }
}
