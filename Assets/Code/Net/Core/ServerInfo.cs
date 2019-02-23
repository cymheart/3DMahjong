
namespace Net
{
    public class ServerInfo
    {
        public string name;
        public string serverIP;                // 远程服务器IP地址
        public int serverPort = 0;                 // 远程服务器的端口
        public int localConnectPort = 0;           // 连接远程服务器的端口
        public NetSocket socketCtx = null;            // 连接远程服务器的Context信息(此变量存储与远程服务器的连接信息)
        public DePacketor dePacketor = null;
        public ServerTask serverTask = null;
        public DataTransMode dataTransMode =  DataTransMode.MODE_PACK;
        public object tag = null;

    }
}
