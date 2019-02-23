using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Net
{
    public class NetSocket
    {
        public Socket sock;
        string remoteIP;             // 远端IP地址
        ushort remotePort = 0;               // 远端连接端口
        public DataTransMode dataTransMode = DataTransMode.MODE_PACK;
        public DateTime timeStamp;
        public PackBuf unPackCache = new PackBuf();
        public int unPackCacheSize = 0;
        public int unPackCalcLen = -1;
        public int unPackHeadLen = 0;
        public byte[] cachePack = null;
        public int cachePackOffset = 0;
        public int cachePackLen = 0;
        public ExtractState extractState = ExtractState.ES_PACKET_HEADLEN_NOT_GET;
        public ByteStream joinFrameDataStream = null;
        public object joinFrameHeader = null;

        byte[] pack = null;
        int packOffset = 0;
        int packSize = 0;
        public ServerTask serverTask;
        object tag = null;

        NetSocketType socketType = NetSocketType.UNKNOWN_SOCKET;
        SocketState socketState = SocketState.NEW_CREATE;
        ServerInfo remoteServerInfo = null;

        public DePacketor dePacketor = null;
        BaseMsgProcesser msgProcesser = null;

        public List<Packet> sendList = new List<Packet>();
        ulong id;

        // 初始化
        public NetSocket(ServerTask _serverTaskCtx = null)
        {
            serverTask = _serverTaskCtx;
            unPackCache.buf = null;
            unPackCache.len = 0;

            if (serverTask == null)
                return;

            Server server = serverTask.GetServer();
            SetDePacketor(server.dePacketor);

            unPackHeadLen = dePacketor.GetPackHeadPreLength();
     
            UniqueID uniqueID = UniqueID.GetInstance();
            if (server.serverTaskCount > 1)
                id = uniqueID.gen_multi();
            else
                id = uniqueID.gen();
        }

        public void SafeClose()
        {
            if (sock == null)
                return;

            if (!sock.Connected)
                return;

            try
            {
                sock.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }

            try
            {
                sock.Close();
                sock = null;
            }
            catch
            {
            }
        }

        Server GetServer()
        {
            return serverTask.GetServer();
        }

        public void SetSocketType(NetSocketType type)
        {
            socketType = type;
        }

        public void SetSocketState(SocketState state)
        {
            socketState = state;

            if ((socketState == SocketState.CONNECTED_SERVER ||
                socketState == SocketState.CONNECTTING_SERVER) &&
                remoteServerInfo != null &&
                remoteServerInfo.dePacketor != null)
            {
                dePacketor = remoteServerInfo.dePacketor;
            }
            else
            {
                Server server = serverTask.GetServer();
                dePacketor = server.dePacketor;
            }

            unPackHeadLen = dePacketor.GetPackHeadPreLength();
        }
        public void SetRemoteServerInfo(ServerInfo serverInfo)
        {
            remoteServerInfo = serverInfo;
            SetSocketState(socketState);
        }

        public int Send(Packet packet, int delay = 0)
        {
            return serverTask.PostSendTask(packet, delay);
        }

        public int Send(ByteStream packetStream, int delay = 0)
        {
            byte[] pack = packetStream.TakeBuf();
            Packet packet = CreatePacket(0);
            packet.ResetBuffer(pack, packetStream.GetNumberOfWriteBytes());

            return serverTask.PostSendTask(packet, delay);
        }

        public int Send(ByteStream packetStream, int headerlen, int delay)
        {
            byte[] pack = packetStream.TakeBuf();
            int packLen = packetStream.GetNumberOfWriteBytes();
            dePacketor.SetDataLengthToPackHead(pack, packLen - headerlen - dePacketor.GetPackTailLength());

            Packet packet = CreatePacket(0);
            packet.ResetBuffer(pack, packLen);

            return serverTask.PostSendTask(packet, delay);
        }

        public int Close(int delay = 0)
        {
            SetSocketState(SocketState.NORMAL_CLOSE);
            return serverTask.PostSocketErrorTask(this);
        }

        public Packet CreatePacket(int packSize = 1024)
        {
            Packet packet = new Packet(this, packSize);
            return packet;
        }

        public Packet CreatePacket(byte[] packBuf, int packSize)
        {
            Packet packet =  CreatePacket(packSize);
            Buffer.BlockCopy(packBuf, 0, packet.buf, 0, packSize);
            return packet;
        }


        public void SetPack(byte[] pack, int packoffset, int packlen)
        {
            this.pack = pack;
            packOffset = packoffset;
            packSize = packlen;
        }

        public void RemovePack()
        {
            pack = null;
            packSize = 0;
        }

        public byte[] GetRecvedPack()
        {
            return pack;
        }

        public int GetRecvedPackOffset()
        {
            return packOffset;
        }

        public int GetRecvedPackSize()
        {
            return packSize;
        }

        public ulong GetID()
        {
            return id;
        }

       public void ResetGenID()
        {
            UniqueID uniqueID = UniqueID.GetInstance();
            Server server = GetServer();

            if (server.serverTaskCount > 1)
                id = uniqueID.gen_multi();
            else
                id = uniqueID.gen();
        }


        public void SetDePacketor(DePacketor depacketor)
        {
            if (depacketor == dePacketor)
                return;

            this.dePacketor = depacketor;

            if (dePacketor != null)
            {
                unPackHeadLen = dePacketor.GetPackHeadPreLength();
            }

            RemovePack();

            unPackCache.buf = null;
            unPackCache.len = 0;
            unPackCalcLen = 0;
            cachePack = null;
            cachePackLen = 0;
        }

        public void SetMsgProcesser(BaseMsgProcesser _msgProcesser)
        {
            if (_msgProcesser == msgProcesser)
                return;

            joinFrameHeader = null;
            msgProcesser = _msgProcesser;
        }


        DePacketor GetDePacketor()
        {
            return dePacketor;
        }

        public BaseMsgProcesser GetMsgProcesser()
        {
            return msgProcesser;
        }


        NetSocketType GetSocketType()
        {
            return socketType;
        }

        SocketState GetSocketState()
        {
            return socketState;
        }

        public void UpdataTimeStamp(DateTime _timeStamp)
        {
            timeStamp = _timeStamp;
        }

        public void UpdataTimeStamp()
        {
            timeStamp = DateTime.Now;
        }

        public ServerInfo GetRemoteServerInfo()
        {
            return remoteServerInfo;
        }

        void ChangeDataTransMode(DataTransMode mode)
        {
            dataTransMode = mode;
        }
    }
}
