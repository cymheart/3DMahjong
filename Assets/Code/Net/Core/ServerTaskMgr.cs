using System.Runtime.InteropServices;

namespace Net
{
    public class ServerTaskMgr
    {

        public Server server;
        int serverTaskCount;
        ServerTaskState[] serverTaskStateList;
        ServerTask[] serverTaskList;
        TaskProcesser taskProcesser;


        public ServerTaskMgr(Server _serverCtx)
        {
            server = _serverCtx;
            serverTaskCount = server.serverTaskCount;

            serverTaskList = new ServerTask[serverTaskCount];
            serverTaskStateList = new ServerTaskState[serverTaskCount];

            for (int i = 0; i < serverTaskCount; i++)
            {
                serverTaskList[i] = new ServerTask(this);
            }

            //
            taskProcesser = new CommonTaskProcesser();
        }

        public ServerTask GetServerTask(int serverTaskIdx)
        {
            if (serverTaskIdx > serverTaskCount || serverTaskIdx < 0)
                return null;
            return serverTaskList[serverTaskIdx];
        }

        public int GetServerTaskCount()
        {
            return serverTaskCount;
        }

        public void CreateServerTaskProcess(int serverTaskIdx, TaskProcesser taskProcesser)
        {
            if (serverTaskIdx == -1)
            {
                for (int i = 0; i < serverTaskCount; i++)
                    serverTaskList[i].CreateTaskProcesser(taskProcesser);
                return;
            }

            serverTaskList[serverTaskIdx].CreateTaskProcesser(taskProcesser);
        }
        public int Start()
        {
            if (taskProcesser == null)
                return -1;

            taskProcesser.Start();

            for (int i = 0; i < serverTaskCount; i++)
            {
                serverTaskList[i].Start();
            }

            return 0;
        }

        public void Stop()
        {
            for (int i = 0; i < serverTaskCount; i++)
                serverTaskList[i].Stop();

            taskProcesser.Stop();
        }


        public int StartListener()
        {
            return serverTaskList[0].PostCreateListenerTask();
        }

        public int ConnectServer(ServerInfo serverInfo, int delay = 0)
        {
            ByteStream sendStream = new ByteStream();
            sendStream.isWriteHostToNet = false;

            sendStream.Write(serverInfo);
            sendStream.Write(delay);
            byte[] data = sendStream.TakeBuf();
            return PostServerMessage(ServerMessage.SMSG_REQUEST_CONNECT_SERVER, data);
        }

        public void SetUseSingleSendTaskProcesser(bool isUse)
        {
            for (int i = 0; i < serverTaskCount; i++)
                serverTaskList[i].SetUseSingleSendDataTaskProcesser(isUse);
        }

        public int PostSingleIocpAccpetTask(Packet packet)
        {
            return serverTaskList[0].PostSingleIocpAcceptTask(packet);
        }


        public int PostServerMessage(ServerMessage serverMsg, object data)
        {
            ServerMsgTaskData stateData = new ServerMsgTaskData();
            stateData.msg = serverMsg;
            stateData.dataPtr = data;
            stateData.serverTaskMgr = this;
            return PostTask(StateProcessTask, stateData);
        }

        public int PostTask(TaskCallBack processDataCallBack, object taskData, int delay = 0)
        {
            return _PostTaskData(processDataCallBack, null, taskData, delay);
        }

        public int PostTask(TaskCallBack processDataCallBack, TaskCallBack releaseDataCallBack, object taskData, int delay = 0)
        {
            return _PostTaskData(processDataCallBack, releaseDataCallBack, taskData, delay);
        }

        public int PostTaskToServerTaskLine(TaskCallBack processDataCallBack, object taskData, int delay)
        {
            return serverTaskList[AssignServerTaskContextIdx()].PostTask(processDataCallBack, taskData, delay);
        }

        public int PostTaskToServerTaskLine(TaskCallBack processDataCallBack, TaskCallBack releaseDataCallBack, object taskData, int delay = 0)
        {
            return serverTaskList[AssignServerTaskContextIdx()].PostTask(processDataCallBack, releaseDataCallBack, taskData, delay);
        }

        int AssignServerTaskContextIdx()
        {
            int minClientSizeIdx = 0;

            for (int i = 1; i < serverTaskCount; i++)
            {
                if (serverTaskStateList[i].clientSize <
                    serverTaskStateList[minClientSizeIdx].clientSize)
                {
                    minClientSizeIdx = i;
                }
            }

            return minClientSizeIdx;
        }
        int MessageProcess(ServerMessage msg, object data)
        {
            int idx;

            switch (msg)
            {
                case ServerMessage.SMSG_LISTENER_CREATE_FINISH:
                    serverTaskList[0].PostStartInitIocpAcceptTask();

                    if (server.isCheckHeartBeat)
                    {
                        for (int i = 0; i < serverTaskCount; i++)
                            serverTaskList[i].PostHeartBeatCheckTask();
                    }
                    break;

                case ServerMessage.SMSG_ACCEPT_CLIENT:
                    idx = AssignServerTaskContextIdx();
                    serverTaskList[idx].PostAcceptedClientTask((Packet)data);
                    break;

                case ServerMessage.SMSG_REQUEST_CONNECT_SERVER:
                    {
                        object obj;
                        ServerInfo serverInfo;
                        int delay = 0;
                        byte[] d = (byte[])data;

                        ByteStream recvStream = new ByteStream((byte[])data);
                        recvStream.isReadNetToHost = false;
                        GCHandle objhandle = recvStream.Read(out obj);
                        recvStream.Read(out delay);
                        objhandle.Free();

                        serverInfo = obj as ServerInfo;

                        idx = AssignServerTaskContextIdx();
                        serverTaskList[idx].PostConnectServerTask(serverInfo, delay);
                    }
                    break;

                default:
                    break;
            }

            return 0;
        }

        int _PostTaskData(TaskCallBack processDataCallBack, TaskCallBack releaseDataCallBack, object taskData, int delay = 0)
        {
            return taskProcesser.PostTask(processDataCallBack, releaseDataCallBack, taskData, TaskMsg.TMSG_DATA, delay);
        }

        void StateProcessTask(object data)
        {
            ServerMsgTaskData msgData = (ServerMsgTaskData)data;
            MessageProcess(msgData.msg, msgData.dataPtr);
        }
    }
}
