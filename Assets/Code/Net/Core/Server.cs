using System;
using System.Net;
using System.Net.Sockets;

namespace Net
{
    public class Server
    {
        public NetSocket listenSocket;     // 用于监听的Socket的Context信息
        int localListenPort = 8888;            // 本机监听端口

        public bool useDefTaskProcesser = false;
        public bool isCheckHeartBeat = false;
        public int childLifeDelayTime = 11000;         // 每次检查所有客户端心跳状态的时间间隔
        public bool isStop = true;
        public int serverTaskCount = 1;
        public bool useSingleSendTaskProcesser = true;

        ServerTaskMgr serverTaskMgr = null;
        public DePacketor dePacketor;

        int serverType;
        string localIP;                    // 本机IP地址	

        public Server()
        {
            serverTaskMgr = new ServerTaskMgr(this);
            dePacketor = new DePacketor();

            SetUseSingleSendTaskProcesser(useSingleSendTaskProcesser);
            UniqueID.GetInstance();
        }

        // 设置监听端口
        public void SetListenPort(int nPort)
        {
            if (!isStop)
                return;
            localListenPort = nPort;
        }

        //	启动服务器
        public bool Start()
        {
            if (!isStop)
                return true;

            serverTaskMgr.Start();
            isStop = false;
            return true;
        }

        //	开始发送系统退出消息
        public void Stop()
        {
            if (isStop)
                return;

            serverTaskMgr.Stop();
            isStop = true;
        }

        public void SetServerTaskProcess(int serverTaskIdx, TaskProcesser taskProcesser)
        {
            if (!isStop)
                return;

            serverTaskMgr.CreateServerTaskProcess(serverTaskIdx, taskProcesser);
        }

        public Timer CreateTimer(int serverTaskIdx, Timer.TimerCallBack timerCB, object param, int durationMS)
        {
            if (isStop)
                return null;

            ServerTask serverTask = serverTaskMgr.GetServerTask(serverTaskIdx);
            TaskProcesser taskProcesser = serverTask.GetMainTaskProcesser();
            Timer timer = new Timer(taskProcesser, durationMS, timerCB, param);
            return timer;
        }


        public void SetUnPackCallBack(DePacketor.UnPackCallBack _unPackCallBack, object param)
        {
            if (isStop && dePacketor != null)
                dePacketor.SetUnPackCallBack(_unPackCallBack, param);
        }

        public void SetDePacketor(DePacketor _dePacketor)
        {
            if (!isStop || _dePacketor == null)
                return;

            if (_dePacketor == dePacketor)
                return;

            dePacketor = _dePacketor;
        }

        public void SetServerMachineID(int machineID)
        {
            UniqueID.GetInstance().set_workid(machineID);
        }

        public void SetUseSingleSendTaskProcesser(bool isUse)
        {
            if (!isStop)
                return;

            useSingleSendTaskProcesser = isUse;

            if (serverTaskMgr != null)
                serverTaskMgr.SetUseSingleSendTaskProcesser(isUse);
        }

        public NetSocket GetSocket(ulong socketID, int searchInServerTaskIdx = -1)
        {
            ServerTask serverTask;

            if (searchInServerTaskIdx != -1)
            {
                serverTask = serverTaskMgr.GetServerTask(searchInServerTaskIdx);
                return serverTask.GetSocket(socketID);
            }

            NetSocket socket;
            for (int i = 0; i < serverTaskMgr.GetServerTaskCount(); i++)
            {
                serverTask = serverTaskMgr.GetServerTask(i);
                socket = serverTask.GetSocket(socketID);
                if (socket != null)
                    return socket;
            }

            return null;
        }

        public int StartListener()
        {
            Start();
            return serverTaskMgr.StartListener();
        }

        public int ConnectServer(ServerInfo serverInfo, int delay = 0)
        {
            Start();
            return serverTaskMgr.ConnectServer(serverInfo, delay);
        }


        void OnConnectCallBack(IAsyncResult ar)
        {
            Packet packet = ar.AsyncState as Packet;
            packet.ar = ar;
            ServerTask serverTask = packet.serverTask;
            serverTask.PostConnectedServerTask(packet);
        }

        void OnReceiveCallBack(IAsyncResult ar)
        {
            Packet packet = ar.AsyncState as Packet;
            packet.ar = ar;
            ServerTask serverTask = packet.serverTask;
            serverTask.PostRecvedTask(packet);
        }

        void OnSendCallBack(IAsyncResult ar)
        {
            Packet packet = ar.AsyncState as Packet;
            packet.ar = ar;
            ServerTask serverTask = packet.serverTask;
            serverTask.PostSendedTask(packet);
        }


        void OnAcceptCallBack(IAsyncResult ar)
        {

            Packet packet = ar.AsyncState as Packet;
            NetSocket newSocketCtx = packet.socketCtx;

            //接收结果  
            newSocketCtx.sock = listenSocket.sock.EndAccept(ar);

            newSocketCtx.SetSocketState(SocketState.CONNECTED_CLIENT);
            newSocketCtx.UpdataTimeStamp();
   
            Packet newPacket = newSocketCtx.CreatePacket(dePacketor.GetMaxBufferSize());
            serverTaskMgr.PostServerMessage(ServerMessage.SMSG_ACCEPT_CLIENT, newPacket);


            //继续接受客户端的请求  
            serverTaskMgr.PostSingleIocpAccpetTask(packet);
        }

        public bool IocpPostAccept(Packet packet)
        {
           // Socket sock = packet.socketCtx.sock;
            IAsyncResult ret = listenSocket.sock.BeginAccept(new AsyncCallback(OnAcceptCallBack), packet);
            return true;
        }

           
        public bool IocpPostConnect(Packet packet)
        {
            Socket sock = packet.socketCtx.sock;
            PackBuf p_wbuf = packet.packBuf;
            ServerInfo serverInfo = packet.socketCtx.GetRemoteServerInfo();

            IPAddress mIp = IPAddress.Parse(serverInfo.serverIP);
            IPEndPoint ip_end_point = new IPEndPoint(mIp, serverInfo.serverPort);
            IAsyncResult ret = sock.BeginConnect(ip_end_point, OnConnectCallBack, packet);

            return true;
        }
 
        // 投递接收数据请求
        public bool IocpPostRecv(Packet packet)
        {
            Socket sock = packet.socketCtx.sock;
            PackBuf p_wbuf = packet.packBuf;

            IAsyncResult ret = sock.BeginReceive(p_wbuf.buf/*消息缓存*/,
           0/*接受消息的偏移量 就是从第几个开始*/,
           (int)p_wbuf.len/*设置接受字节数*/,
           SocketFlags.None/*Socket标志位*/,
           new AsyncCallback(OnReceiveCallBack)/*接受回调*/,
           packet/*最后的状态*/);

            return true;
        }

        // 投递发送数据请求
        public bool IocpPostSend(Packet packet)
        {
            Socket sock = packet.socketCtx.sock;
            PackBuf p_wbuf = packet.packBuf;

           IAsyncResult ret = sock.BeginSend(p_wbuf.buf, 0,  (int)p_wbuf.len, SocketFlags.None,
               new AsyncCallback(OnSendCallBack),
               packet);
              
            return true;
        }

    }
}
