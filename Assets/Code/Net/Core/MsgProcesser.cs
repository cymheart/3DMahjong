using System;
using System.Collections.Generic;
using System.Linq;

namespace Net
{
    public abstract class BaseMsgProcesser
    {
        public abstract void SetDePacketor(DePacketor dePacketor);
        public abstract void UnPack(SocketEvent ev, NetSocket socket);
    }


    public abstract class MsgProcesser<HeaderType, MsgType> : BaseMsgProcesser where HeaderType : new()
    {
        public delegate bool SocketEventFunc(NetSocket socket);
        public delegate bool MsgFunc(NetSocket socket, HeaderType header, ByteStream dataStream);

        Dictionary<MsgType, List<MsgFunc>> msgFuncMap = new Dictionary<MsgType, List<MsgFunc>>();
        Dictionary<SocketEvent, List<SocketEventFunc>> socketEventFuncMap = new Dictionary<SocketEvent, List<SocketEventFunc>>();
        bool isDecryptData = false;

        public MsgProcesser()
        {

        }

        public override void SetDePacketor(DePacketor dePacketor)
        {
            dePacketor.SetUnPackCallBack(UnPack, this);
        }

        public void SetIsDecryptData(bool isDecrypt)
        {
            isDecryptData = isDecrypt;
        }

        public int RegSocketEvent(SocketEvent ev, SocketEventFunc[] socketEvFuncs)
        {
            if (!socketEventFuncMap.ContainsKey(ev))
                socketEventFuncMap[ev] = new List<SocketEventFunc>();

            List<SocketEventFunc> socketEvCBList = socketEventFuncMap[ev];

            for (int i = 0; i < socketEvFuncs.Length; i++)
            {
                socketEvCBList.Add(socketEvFuncs[i]);
            }
            return 0;
        }

        public int RegSocketEvent(SocketEvent ev, SocketEventFunc socketEventCB)
        {
            if (socketEventCB != null)
            {
                if (!socketEventFuncMap.ContainsKey(ev))
                    socketEventFuncMap[ev] = new List<SocketEventFunc>();

                List<SocketEventFunc> socketEvCBList = socketEventFuncMap[ev];
                socketEvCBList.Add(socketEventCB);
                return 0;
            }
            return 1;
        }

        public void RegMsg(MsgType msgType, MsgFunc[] msgFuncs)
        {
            if (!msgFuncMap.ContainsKey(msgType))
                msgFuncMap[msgType] = new List<MsgFunc>();

            List<MsgFunc> msgCBList = msgFuncMap[msgType];
            for (int i = 0; i < msgFuncs.Length; i++)
            {
                msgCBList.Add(msgFuncs[i]);
            }
        }

        public void RegMsg(MsgType msgType, MsgFunc msgCB)
        {
            if (msgCB != null)
            {
                if (!msgFuncMap.ContainsKey(msgType))
                    msgFuncMap[msgType] = new List<MsgFunc>();

                List<MsgFunc> msgCBList = msgFuncMap[msgType];
                msgCBList.Add(msgCB);
            }
        }


        void ProcessMsg(NetSocket socket)
        {
            HeaderType header = new HeaderType();
            ByteStream recvStream = new ByteStream(socket.GetRecvedPack(), socket.GetRecvedPackOffset(), socket.GetRecvedPackSize());
            int headlen = ReadStreamToHeader(header, recvStream, true);
            recvStream.ResetExtrenBuf(recvStream.GetBuf(), recvStream.GetCurtPos(), recvStream.GetNumberOfRichBytes());

            if (!IsSingleFrame(header))
            {
                if (isDecryptData)
                {
                    ByteStream decryptDataStream = new ByteStream();
                    Decrypt(header, recvStream, decryptDataStream);
                    decryptDataStream.SetCurt(0);
                    _ProcessMsg(GetMsgTypeValue(header), socket, header, decryptDataStream);
                }
                else
                {
                    _ProcessMsg(GetMsgTypeValue(header), socket, header, recvStream);
                }
            }
            else
            {
                _ProcessSingleFrame(socket, header, recvStream);
            }
        }



        void ProcessSocketEvent(SocketEvent ev, NetSocket socket)
        {
            List<SocketEventFunc> socketEvFuncList;

            if(socketEventFuncMap.ContainsKey(ev))
            {
                socketEvFuncList = socketEventFuncMap[ev];

                bool isContinue = true;
                for (int i = 0; i < socketEvFuncList.Count; i++)
                {
                    if (isContinue)
                    {
                        (socketEvFuncList[i])(socket);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }


        //判断此包是否为分片
        public virtual bool IsSingleFrame(HeaderType frameHeader)
        {
            return false;
        }

        //获取当前包消息类型
        public abstract MsgType GetMsgTypeValue(HeaderType header);


        //读取数据流中的头部数据到头部结构中
        public abstract int ReadStreamToHeader(HeaderType header, ByteStream readStream, bool isNetToHost);


        //处理分片包加入到总包中
        public abstract bool ProcessSingleDataFrame(
           HeaderType joinFrameHeader, ByteStream joinFrameDataStream,
           HeaderType singleFrameHeader, ByteStream singleFrameDataStream);

        //解密数据
        public virtual int Decrypt(HeaderType header, ByteStream orgDataStream, ByteStream decryptDataStream)
        {
            return 0;
        }


        void _ProcessSingleFrame(NetSocket socket, HeaderType singleFrameHeader, ByteStream singleFrameDataStream)
        {
            if (socket.joinFrameDataStream == null)
            {
                socket.joinFrameDataStream = new ByteStream();
                socket.joinFrameDataStream.isByteAlign = false;
                socket.joinFrameHeader = singleFrameHeader;
                socket.joinFrameDataStream.WriteBytes(singleFrameDataStream.GetBuf(), singleFrameDataStream.GetCurtOffset(), singleFrameDataStream.GetNumberOfWriteBytes());
                return;
            }

            bool isFinish = ProcessSingleDataFrame(
                (HeaderType)socket.joinFrameHeader, socket.joinFrameDataStream,
                singleFrameHeader, singleFrameDataStream);

            if (isFinish)
            {
                HeaderType header = (HeaderType)socket.joinFrameHeader;
                socket.joinFrameDataStream.SetCurt(0);

                if (isDecryptData)
                {
                    ByteStream decryptDataStream = new ByteStream();
                    Decrypt(header, socket.joinFrameDataStream, decryptDataStream);
                    decryptDataStream.SetCurt(0);
                    _ProcessMsg(GetMsgTypeValue(header), socket, header, decryptDataStream);
                }
                else
                {
                    _ProcessMsg(GetMsgTypeValue(header), socket, header, socket.joinFrameDataStream);
                }

                socket.joinFrameHeader = null;
                socket.joinFrameDataStream = null;
            }
        }

        void _ProcessMsg(MsgType msgType, NetSocket socket, HeaderType header, ByteStream dataStream)
        {
            bool isContinue = true;
            List<MsgFunc> msgfuncList;

            if(msgFuncMap.ContainsKey(msgType))
            {
                msgfuncList = msgFuncMap[msgType];

                for (int i = 0; i < msgfuncList.Count(); i++)
                {
                    if (isContinue)
                    {
                        isContinue = (msgfuncList[i])(socket, header, dataStream);
                    }
                    else
                    {
                        break;
                    }
                }
            }  
            else
            {
                // WARNING("消息(%d)没有对应处理函数!", msgType);
            }
        }

        public override void UnPack(SocketEvent ev, NetSocket socket)
        {
            switch (ev)
            {
                case SocketEvent.EV_SOCKET_RECV:
                    {
                        if (socket.dataTransMode == DataTransMode.MODE_PACK)
                            ProcessMsg(socket);
                        else
                            ProcessSocketEvent(ev, socket);
                    }
                    break;

                default:
                    ProcessSocketEvent(ev, socket);
                    break;
            }
        }

        void _UnPack(SocketEvent ev, NetSocket socket, object param)
        {
            MsgProcesser<HeaderType, MsgType> msgProcesser = (MsgProcesser<HeaderType, MsgType>)param;
            socket.SetMsgProcesser(msgProcesser);
            msgProcesser.UnPack(ev, socket);
        }


    }

}
