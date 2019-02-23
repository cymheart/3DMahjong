using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Net
{
    public class ServerTask
    {

        Dictionary<ulong, NetSocket> socketMap = new Dictionary<ulong, NetSocket>();
        TaskProcesser taskProcesser = null;
        TaskProcesser sendTaskProcesser  = null;
        ServerTaskMgr serverTaskMgr;

        EventWaitHandle socketErrorWaitSem = new EventWaitHandle(false, EventResetMode.ManualReset);
        bool useSingleSendDataTaskProcesser = true;
        bool useSendedEvent = false;

        public ServerTask(ServerTaskMgr serverTaskMgr)
        {
            this.serverTaskMgr = serverTaskMgr;
            CreateTaskProcesser();
        }


        bool CreateListener()
        {
            Server server = GetServer();
            NetSocket listenCtx = null;

            // 生成用于监听的Socket的信息    
            if (listenCtx == null)
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listenCtx = new NetSocket(this);
                listenCtx.sock = sock;
            }

            listenCtx.SetSocketType(NetSocketType.LISTENER_SOCKET);

            if (null == listenCtx.sock)
            {
                listenCtx.SetSocketState(SocketState.INIT_FAILD);
                SocketError(listenCtx);
                //LOG4CPLUS_ERROR(log.GetInst(), "初始化Socket失败，错误代码:"<<_GetLastError());
                return false;
            }

            SetListenerContext(listenCtx);
            socketMap[listenCtx.GetID()] = listenCtx;

            //实例一个网络端点  传入地址和端口  
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 23456);
            //绑定网络端点  
            listenCtx.sock.Bind(serverEP);
            //设置最大监听数量  
            listenCtx.sock.Listen(0x7fffffff);

            listenCtx.SetSocketState(SocketState.LISTENING);

            serverTaskMgr.PostServerMessage(ServerMessage.SMSG_LISTENER_CREATE_FINISH, null);

            return true;
        }


        public int PostConnectServerTask(ServerInfo serverInfo, int delay = 0)
        {
            return PostTask(ConnectServerTask, serverInfo, delay);
        }

        void ConnectServerTask(object data)
        {
            ServerInfo serverInfo = (ServerInfo)data;
            ConnectServer(serverInfo);
        }
        public bool ConnectServer(ServerInfo serverInfo)
        {
            Server server = GetServer();

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            NetSocket connectSocketCtx = new NetSocket(this);
            connectSocketCtx.sock = sock;

            connectSocketCtx.SetSocketType(NetSocketType.CONNECT_SERVER_SOCKET);
            serverInfo.socketCtx = connectSocketCtx;
            connectSocketCtx.SetRemoteServerInfo(serverInfo);
            connectSocketCtx.dataTransMode = serverInfo.dataTransMode;
            socketMap[connectSocketCtx.GetID()] = connectSocketCtx;


            if (connectSocketCtx.sock == null)
            {
                connectSocketCtx.SetSocketState(SocketState.INIT_FAILD);
                SocketError(connectSocketCtx);
                //LOG4CPLUS_ERROR(log.GetInst(), "初始化Socket失败，错误代码:" << _GetLastError());
                return false;
            }

            connectSocketCtx.UpdataTimeStamp();
            Packet packet = connectSocketCtx.CreatePacket(0);

            // 开始连接服务器
            connectSocketCtx.SetSocketState(SocketState.CONNECTTING_SERVER);
            server.IocpPostConnect(packet);
           
            return true;
        }

        public void Start()
        {
            if (taskProcesser != null)
                taskProcesser.Start();

            if (sendTaskProcesser != null)
                sendTaskProcesser.Start();
        }

        public void Stop()
        {
            if (taskProcesser != null)
                taskProcesser.Stop();

            if (sendTaskProcesser != null)
                sendTaskProcesser.Stop();

            RemoveSocketMap();
        }

        public void Pause()
        {
            if (taskProcesser != null)
                taskProcesser.Pause();

            if (sendTaskProcesser != null)
                sendTaskProcesser.Pause();
        }

        public void Continue()
        {
            if (taskProcesser != null)
                taskProcesser.Continue();

            if (sendTaskProcesser != null)
                sendTaskProcesser.Continue();
        }

        public void CreateTaskProcesser(TaskProcesser newTaskProcesser = null)
        {
            Stop();

            if (useSingleSendDataTaskProcesser)
                sendTaskProcesser = new CommonTaskProcesser();

            Server server = GetServer();
            if (newTaskProcesser == null || server.useDefTaskProcesser)
            {
                taskProcesser = new CommonTaskProcesser();
            }
            else
            {
                taskProcesser = newTaskProcesser;
            }
        }

        public NetSocket GetListenerContext()
        {
            return serverTaskMgr.server.listenSocket;
        }

        public Server GetServer()
        {
            return serverTaskMgr.server;
        }

        public TaskProcesser GetMainTaskProcesser()
        {
            return taskProcesser;
        }

        public void SetUseSingleSendDataTaskProcesser(bool isUse)
        {
            useSingleSendDataTaskProcesser = isUse;
        }


        public void SetListenerContext(NetSocket listenerCtx)
        {
            serverTaskMgr.server.listenSocket = listenerCtx;
        }


        public int PostCreateListenerTask()
        {
            return PostTask(CreateListenerTask, this);

        }

        public int PostStartInitIocpAcceptTask()
        {
            int ret = PostTask(StartInitIocpAcceptTask, this);
            return ret;
        }

        void StartInitIocpAcceptTask(object data)
        {
            Server server = GetServer();
            Packet packet;
            NetSocket newSocketCtx;

            // 为AcceptEx 准备参数，然后投递AcceptEx I/O请求
            for (int i = 0; i < 20; i++)
            {
                newSocketCtx = new NetSocket(this);
                newSocketCtx.SetSocketType(NetSocketType.LISTEN_CLIENT_SOCKET);

                packet = newSocketCtx.CreatePacket(0);
                server.IocpPostAccept(packet);
            }
        }

        public int PostAcceptedClientTask(Packet packet)
        {
            packet.serverTask = this;
            packet.socketCtx.serverTask = this;
            return PostTask(AcceptedClientTask, packet);
        }

        void AcceptedClientTask(object data)
        {
            Packet packet = (Packet)data;
            NetSocket socketCtx = packet.socketCtx;
            Server server = GetServer();

            // 把这个有效的客户端信息，加入到socketMap中去
            socketMap[socketCtx.GetID()] = socketCtx;

            socketCtx.SetPack(packet.buf, 0, packet.transferedBytes);
            socketCtx.dePacketor.UnPack(SocketEvent.EV_SOCKET_ACCEPTED, socketCtx);
            socketCtx.RemovePack();

            int bufSize = socketCtx.dePacketor.GetMaxBufferSize();

            if (packet.packBuf.len != bufSize)
            {
                packet = socketCtx.CreatePacket(bufSize);
            }

            server.IocpPostRecv(packet);
        }

        public int PostSingleIocpAcceptTask(Packet packet)
        {
            packet.serverTask = this;
            return PostTask(SingleIocpAcceptTask, packet);
        }

        void SingleIocpAcceptTask(object data)
        {
            Packet packet = (Packet)data;
            Server server = GetServer();
            NetSocket socketCtx = null;

            // 投递新的AcceptEx
            socketCtx = new NetSocket(this);
            socketCtx.SetSocketType(NetSocketType.LISTEN_CLIENT_SOCKET);
            packet.socketCtx = socketCtx;

            server.IocpPostAccept(packet);
        }



        public int PostConnectedServerTask(Packet packet)
        {
            return PostTask(ConnectedServerTask, packet);
        }

        void ConnectedServerTask(object data)
        {
            Packet packet = (Packet)data;
            NetSocket socketCtx = packet.socketCtx;
            Server server = GetServer();

            if (CheckingPacketVaild(packet) == 0)
                return;

            if(socketCtx.sock.Connected == false)
            {
                SocketError(socketCtx);
                return;
            }

            socketCtx.sock.EndConnect(packet.ar);
            socketCtx.SetSocketState(SocketState.CONNECTED_SERVER);
            socketCtx.UpdataTimeStamp();

            int bufSize = socketCtx.dePacketor.GetMaxBufferSize();

            if (packet.packBuf.len != bufSize)
            {
                packet = socketCtx.CreatePacket(bufSize);
            }

            socketCtx.dePacketor.UnPack(SocketEvent.EV_SOCKET_CONNECTED, socketCtx);
            server.IocpPostRecv(packet);
        }

        public int PostRecvedTask(Packet packet)
        {
            return PostTask(RecvedTask, packet);
        }

        void RecvedTask(object data)
        {
            Packet packet = (Packet)data;
            NetSocket socketCtx = packet.socketCtx;
            Server server = GetServer();

            if (CheckingPacketVaild(packet) == 0)
                return;

            SocketError socketError =  System.Net.Sockets.SocketError.Success;
            packet.transferedBytes = socketCtx.sock.EndReceive(packet.ar, out socketError);

            if(socketError != System.Net.Sockets.SocketError.Success)
            {
                SocketError(socketCtx);
                return;
            }

            socketCtx.UpdataTimeStamp();

            if (socketCtx.dataTransMode == DataTransMode.MODE_PACK)
            {
                socketCtx.dePacketor.SetCurtPack(socketCtx, packet.buf, packet.transferedBytes);
                int ret = socketCtx.dePacketor.Extract();

                if (ret == 2)
                {
                    socketCtx.SetSocketState(SocketState.RECV_DATA_TOO_BIG);
                    SocketError(socketCtx.GetID());
                }
            }
            else
            {
                socketCtx.SetPack(packet.buf, 0, packet.transferedBytes);
                socketCtx.dePacketor.UnPack(SocketEvent.EV_SOCKET_RECV, socketCtx);
                socketCtx.RemovePack();
            }

            // 然后开始投递下一个WSARecv请求
            server.IocpPostRecv(packet);
        }



        public int PostSendTask(Packet packet, int delay = 0)
        {
            if (useSingleSendDataTaskProcesser)
                return PostSendDataTask(SendTask, packet, delay);
            return PostTask(SendTask, packet, delay);
        }


        //投递已发送数据任务
        public int PostSendedTask(Packet packet)
        {
            if (useSingleSendDataTaskProcesser)
                return PostSendDataTask(SendedTask, packet);
            return PostTask(SendedTask, packet);
        }

        //此任务根据设置可能在其它任务处理线上执行
        void SendTask(object data)
        {
            Packet packet = (Packet)data;
            NetSocket socketCtx = packet.socketCtx;
            Server server = GetServer();

            if (CheckingPacketVaild(packet) == 0)
                return;

            List<Packet> sendList = socketCtx.sendList;

            if (sendList.Count == 0)
            {
                server.IocpPostSend(packet);
            }
            else
            {
                sendList.Add(packet);
            }
        }

        //此任务根据设置可能在其它任务处理线上执行
        void SendedTask(object data)
        {
            Packet packet = (Packet)data;
            NetSocket socketCtx = packet.socketCtx;
            Server server = GetServer();

            if (CheckingPacketVaild(packet) == 0)
                return;

            SocketError socketError = System.Net.Sockets.SocketError.Success;
            packet.transferedBytes = socketCtx.sock.EndSend(packet.ar, out socketError);
            if (socketError != System.Net.Sockets.SocketError.Success)
            {
                SocketError(socketCtx);
                return;
            }


            if (useSendedEvent)
            {
                if (useSingleSendDataTaskProcesser)
                    PostTask(ProcessSendedMsgTask, packet.socketID);
                else
                    socketCtx.dePacketor.UnPack(SocketEvent.EV_SOCKET_SEND, socketCtx);
            }


            List<Packet> sendList = socketCtx.sendList;
            if (sendList.Count == 0)
                return;

            packet = sendList[0];
            sendList.RemoveAt(0);
            server.IocpPostSend(packet);
        }

        void ProcessSendedMsgTask(object data)
        {
            NetSocket socket;
            ulong socketID = (ulong)data;

            if (GetSocket(socketID) == null)
                return;

            socket = socketMap[socketID];
            socket.dePacketor.UnPack(SocketEvent.EV_SOCKET_SEND, socket);
        }

        //投递心跳检测任务
        public int PostHeartBeatCheckTask()
        {
            Server server = GetServer();
            return PostTask(HeartBeatCheckTask, this, server.childLifeDelayTime - 1000);
        }

        void HeartBeatCheckTask(object data)
        {
            Server server = GetServer();

            if (server.isCheckHeartBeat)
            {
                HeartBeatCheck();
                PostHeartBeatCheckTask();
            }
        }

        void HeartBeatCheck()
        {
            NetSocket socketCtx;
            DateTime time = DateTime.Now;
            int tm;
            Server server = GetServer();
            List<NetSocket> errorSocketList = new List<NetSocket>();

            foreach (var item in socketMap)
            {
                socketCtx = item.Value;
                tm = (int)((time - socketCtx.timeStamp).TotalMilliseconds);

                if (tm > server.childLifeDelayTime)
                {
                    socketCtx.SetSocketState(SocketState.RESPONSE_TIMEOUT);
                    errorSocketList.Add(socketCtx);
                }
            }

            foreach (NetSocket s in errorSocketList)
            {
                SocketError(s.GetID());
            }
        }


        //投递错误消息任务
        int PostErrorTask(Packet packet, int delay)
        {
            return PostTask(ErrorTask, packet, delay);
        }

        void ErrorTask(object data)
        {
            Packet packet = (Packet)data;
            NetSocket socketCtx = packet.socketCtx;
            Server server = GetServer();

            if (CheckingPacketVaild(packet) == 0)
                return;

            SocketError(socketCtx);
        }


        public int PostSocketErrorTask(NetSocket socketCtx, int delay = 0)
        {
            return PostTask(SocketErrorTask, socketCtx.GetID(), delay);
        }

        void SocketErrorTask(object data)
        {
            SocketError((ulong)data);
        }

        int SocketError(ulong socketID)
        {
            NetSocket socket = GetSocket(socketID);
            if (socket == null)
                return 0;
            SocketError(socket);
            return 0;
        }

        int SocketError(NetSocket socket)
        {
            if (useSingleSendDataTaskProcesser)
            {
                sendTaskProcesser.PostTask(PasueSendTask, null, this, TaskMsg.TMSG_PAUSE);
                socket.dePacketor.UnPack(SocketEvent.EV_SOCKET_OFFLINE, socket);
                socketErrorWaitSem.WaitOne();
                socketErrorWaitSem.Reset();
                RemoveSocket(socket.GetID());
                sendTaskProcesser.Continue();
            }
            else
            {
                socket.dePacketor.UnPack(SocketEvent.EV_SOCKET_OFFLINE, socket);
                RemoveSocket(socket.GetID());
            }
            return 0;
        }

        void PasueSendTask(object data)
        {
            socketErrorWaitSem.Set();
        }

        void RemoveSocket(ulong socketID)
        {
            socketMap[socketID].SafeClose();
            socketMap.Remove(socketID);
        }

        public int PostTask(TaskCallBack processDataCallBack, object taskData, int delay = 0)
        {
            return _PostTaskData(processDataCallBack, null, taskData, delay);
        }

        public int PostTask(TaskCallBack processDataCallBack, TaskCallBack releaseDataCallBack, object taskData, int delay = 0)
        {
            return _PostTaskData(processDataCallBack, releaseDataCallBack, taskData, delay);
        }

        public int PostSendDataTask(TaskCallBack processDataCallBack, object taskData, int delay = 0)
        {
            return _PostSendTaskData(processDataCallBack, null, taskData, delay);
        }

        public int PostSendDataTask(TaskCallBack processDataCallBack, TaskCallBack releaseDataCallBack, object taskData, int delay = 0)
        {
            return _PostSendTaskData(processDataCallBack, releaseDataCallBack, taskData, delay);
        }

        public NetSocket GetSocket(ulong socketID)
        {
            if (!socketMap.ContainsKey(socketID))
                return null;

            NetSocket socket = socketMap[socketID];
            return socket;
        }

        int CheckingPacketVaild(Packet packet)
        {
            if (GetSocket(packet.socketID) == null)
                return 0;
            return 1;
        }

        void RemoveSocketMap()
        {
            socketMap.Clear();
        }

        int _PostTaskData(TaskCallBack processDataCallBack, TaskCallBack releaseDataCallBack, object taskData, int delay = 0)
        {
            return taskProcesser.PostTask(processDataCallBack, releaseDataCallBack, taskData, TaskMsg.TMSG_DATA, delay);
        }

        int _PostSendTaskData(TaskCallBack processDataCallBack, TaskCallBack releaseDataCallBack, object taskData, int delay = 0)
        {
            return sendTaskProcesser.PostTask(processDataCallBack, releaseDataCallBack, taskData, TaskMsg.TMSG_DATA, delay);
        }

        void CreateListenerTask(object data)
        {
            CreateListener();
        }

    }
}
