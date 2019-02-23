
using System;

namespace Net
{
    public class DePacketor
    {
        public delegate void UnPackCallBack(SocketEvent ev, NetSocket socketCtx);
        public delegate int GetPackDataLenCB(DePacketor dePacket, byte[] pack, int offset, int packLen, out int realPackHeadLen);
        public delegate void SetDataLengthToPackHeadCallBack(byte[] pack, int dataSize);
     
        NetSocket curtDepacketSocket = null;     //当前接收包SocketCtx
        byte[] curtPack = null;              //当前接收包 
        int curtPackLen = 0;           //当前接收包的整个数据长度
        byte[] cachePack = null;
        int cachePackOffset = 0;
        int cachePackLen = 0;
        int maxBufferSize = 2048;
        int packHeaderPreLen = 0;
        int packTailSize = 0;

        UnPackCallBack unPack = null;
        object unPackParam = null;

        GetPackDataLenCB getPackDataLength = null;
        SetDataLengthToPackHeadCallBack setDataLengthToPackHead = null;
        BaseMsgProcesser msgProcesser = null;

        public DePacketor()
        {

        }

        public void SetMsgProcesser(BaseMsgProcesser _msgProcesser)
        {
            if (_msgProcesser == msgProcesser)
                return;

            msgProcesser = _msgProcesser;
            if (msgProcesser != null)
                msgProcesser.SetDePacketor(this);
        }

        public void UnPack(SocketEvent ev, NetSocket socket)
        {
            BaseMsgProcesser msgProcesser = socket.GetMsgProcesser();

            if (msgProcesser != null)
            {
                msgProcesser.UnPack(ev, socket);
            }
            else if (unPack != null)
            {
                unPack(ev, socket);
                socket.SetMsgProcesser((BaseMsgProcesser)unPackParam);
            }
        }

        public void SetCurtPack(NetSocket socket, byte[] _curtPack, int _curtPackLen)
        {
            curtDepacketSocket = socket;
            curtPack = _curtPack;
            curtPackLen = _curtPackLen;
            cachePack = curtDepacketSocket.cachePack;
            cachePackLen = curtDepacketSocket.cachePackLen;
            cachePackOffset = curtDepacketSocket.cachePackOffset;
        }

        void CreateCachePackBuf()
        {
            PackBuf cachePackBuf = curtDepacketSocket.unPackCache;

            if (cachePackBuf.buf == null)
            {
                curtDepacketSocket.unPackCacheSize = GetMaxBufferSize() * 2;
                cachePackBuf.buf = new byte[curtDepacketSocket.unPackCacheSize];
                cachePackBuf.len = cachePackLen;

                Buffer.BlockCopy(cachePack, (int)cachePackOffset, cachePackBuf.buf, 0, (int)cachePackLen);
                curtDepacketSocket.cachePack = cachePack;
                curtDepacketSocket.cachePackOffset = cachePackOffset;
                curtDepacketSocket.cachePackLen = cachePackLen;
            }
        }

        void ReleaseCachePackBuf()
        {
            PackBuf cachePackBuf = curtDepacketSocket.unPackCache;
            if (cachePackBuf.buf != null)
            {
                cachePackBuf.buf = null;
                cachePackBuf.len = 0;
                curtDepacketSocket.unPackCacheSize = 0;
                curtDepacketSocket.cachePack = null;
                curtDepacketSocket.cachePackOffset = 0;
                curtDepacketSocket.cachePackLen = 0;
                curtDepacketSocket.unPackCalcLen = -1;
                curtDepacketSocket.unPackHeadLen = GetPackHeadPreLength();
            }
        }

        public int Extract()
        {
            int calcPackLen = curtDepacketSocket.unPackCalcLen;   //当前接受包的一个完整消息的长度
            int packHeadLen = curtDepacketSocket.unPackHeadLen;
            PackBuf cachePackBuf = curtDepacketSocket.unPackCache;
            int realPackHeadLen;
          
            if (cachePackBuf.buf != null)
            {
                int richBufLen = curtDepacketSocket.unPackCacheSize - cachePackBuf.len;

                //新来的数据包curtPack的长度curtPackLen, 不大于缓存cachePackBuf中的富余长度richBufLen
                if (curtPackLen <= richBufLen)
                {
                    Buffer.BlockCopy(curtPack, 0, cachePackBuf.buf, (int)cachePackBuf.len, (int)curtPackLen);
                    cachePackBuf.len += curtPackLen;
                }
                else
                {
                    if (curtDepacketSocket.cachePackLen + curtPackLen > curtDepacketSocket.unPackCacheSize)
                    {
                        curtDepacketSocket.unPackCacheSize *= 2;
                        cachePackBuf.buf = new byte[curtDepacketSocket.unPackCacheSize];
                    }
                    else if (curtDepacketSocket.cachePackLen > curtDepacketSocket.cachePackOffset)
                    {
                        //curtDepacketSocket->cachePack是在cachePackBuf->buf中的指针位置
                        cachePackBuf.buf = new byte[curtDepacketSocket.unPackCacheSize];
                    }

                    cachePackBuf.len = curtDepacketSocket.cachePackLen;
                    Buffer.BlockCopy(curtDepacketSocket.cachePack, (int)curtDepacketSocket.cachePackOffset, cachePackBuf.buf, 0, (int)curtDepacketSocket.cachePackLen);
                    Buffer.BlockCopy(curtPack, 0, cachePackBuf.buf, (int)cachePackBuf.len, (int)curtPackLen);
                    cachePackBuf.len += curtPackLen;

                    curtDepacketSocket.cachePack = cachePackBuf.buf;
                    curtDepacketSocket.cachePackOffset = 0;
                    curtDepacketSocket.cachePackLen = cachePackBuf.len;
                }

                cachePack = cachePackBuf.buf;
                cachePackLen = cachePackBuf.len;
                cachePackOffset = 0;
            }
            else
            {
                cachePack = curtPack;
                cachePackLen = curtPackLen;
                cachePackOffset = 0;
            }

            while (true)
            {
                switch (curtDepacketSocket.extractState)
                {
                    case ExtractState.ES_PACKET_HEADLEN_NOT_GET:
                        {
                            if (cachePackLen >= packHeadLen)
                            {
                                calcPackLen = GetPackLength(cachePack, cachePackOffset, cachePackLen, out realPackHeadLen); //根据包头中的信息，获取包数据长度
                                curtDepacketSocket.unPackCalcLen = calcPackLen;
                                packHeadLen = curtDepacketSocket.unPackHeadLen = realPackHeadLen;

                                if (packHeadLen < 0)
                                {
                                    CreateCachePackBuf();
                                    curtDepacketSocket.extractState = ExtractState.ES_PACKET_HEADLEN_NOT_GET;
                                    return 1;
                                }
                                else if (calcPackLen < 0)
                                {
                                    CreateCachePackBuf();
                                    curtDepacketSocket.extractState = ExtractState.ES_PACKET_HEAD_NOT_FULL;
                                    return 1;
                                }
                                else
                                {
                                    curtDepacketSocket.extractState = ExtractState.ES_PACKET_HEAD_FULL;
                                }
                            }
                            else  //此次包头长度不完整
                            {
                                CreateCachePackBuf();
                                curtDepacketSocket.extractState = ExtractState.ES_PACKET_HEADLEN_NOT_GET;
                                return 1;
                            }
                        }
                        break;

                    case ExtractState.ES_PACKET_HEAD_FULL:
                        {
                            if (calcPackLen == cachePackLen)   //刚好获取的是一个完整的数据包
                            { 
                                curtDepacketSocket.SetPack(cachePack, cachePackOffset, cachePackLen);
                                UnPack(SocketEvent.EV_SOCKET_RECV, curtDepacketSocket);
                                curtDepacketSocket.RemovePack();

                                ReleaseCachePackBuf();
                                curtDepacketSocket.extractState = ExtractState.ES_PACKET_HEADLEN_NOT_GET;
                                return 0;
                            }
                            else if (calcPackLen < cachePackLen)   //获取的数据包长度大于一个完整数据包的长度
                            {
                                curtDepacketSocket.SetPack(cachePack, cachePackOffset, calcPackLen);
                                UnPack(SocketEvent.EV_SOCKET_RECV, curtDepacketSocket);

                                curtDepacketSocket.RemovePack();
           
                                cachePackOffset += calcPackLen;
                                cachePackLen -= calcPackLen;
                                curtDepacketSocket.cachePack = cachePack;
                                curtDepacketSocket.cachePackOffset = cachePackOffset;
                                curtDepacketSocket.cachePackLen = cachePackLen;

                                curtDepacketSocket.extractState = ExtractState.ES_PACKET_HEADLEN_NOT_GET;
                                curtDepacketSocket.unPackCalcLen = -1;
                                packHeadLen = curtDepacketSocket.unPackHeadLen = GetPackHeadPreLength();

                            }
                            else   //获取的数据包不完整
                            {
                                if (calcPackLen > GetMaxBufferSize())
                                {
                                    curtDepacketSocket.extractState = ExtractState.ES_PACKET_HEADLEN_NOT_GET;
                                    return 2;
                                }

                                CreateCachePackBuf();
                                curtDepacketSocket.extractState = ExtractState.ES_PACKET_HEAD_FULL;
                                return 1;
                            }
                        }

                        break;


                    case ExtractState.ES_PACKET_HEAD_NOT_FULL:
                        {
                            int leavePackHeadLen = packHeadLen - (int)cachePackLen;

                            if (curtPackLen < leavePackHeadLen)   //包头信息依然不足
                            {
                                curtDepacketSocket.extractState = ExtractState.ES_PACKET_HEAD_NOT_FULL;
                                return 1;
                            }
                            else   //不完整数据包中获取到完整的包头信息了
                            {
                                curtDepacketSocket.extractState = ExtractState.ES_PACKET_HEAD_FULL;
                            }
                        }

                        break;
                }
            }
        }


        public void SetUnPackCallBack(UnPackCallBack _unPackCallBack, object _unPackParam)
        {
            unPack = _unPackCallBack;
            unPackParam = _unPackParam;
        }

        public void SetGetPackDataLengthCallBack(GetPackDataLenCB _getPackDataLength)
        {
            getPackDataLength = _getPackDataLength;
        }

        public void SetSetDataLengthToPackHeadCallBack(SetDataLengthToPackHeadCallBack _setDataLengthToPackHead)
        {
            setDataLengthToPackHead = _setDataLengthToPackHead;
        }


        //获取包头预设长度	
        public int GetPackHeadPreLength()
        {
            return packHeaderPreLen;
        }


        //获取包数据长度
        public int GetPackDataLength(byte[] pack, int offset, int packLen, out int realPackHeadLen)
        {
            if (getPackDataLength != null)
                return getPackDataLength(this, pack, offset, packLen, out realPackHeadLen);

            realPackHeadLen = -1;
            return -1;
        }

        //获取包尾长度	
        public int GetPackTailLength()
        {
            return packTailSize; //0
        }


        public int GetPackLength(byte[] pack, int offset, int packLen, out int realPackHeadLen)
        {
            int packHeadLen = 0;
            int dataLen = GetPackDataLength(pack, offset, packLen, out packHeadLen);
            if (dataLen == -1 || packHeadLen == -1)
            {
                realPackHeadLen = packHeadLen;
                return -1;
            }

            realPackHeadLen = packHeadLen;
            int size = packHeadLen + dataLen + GetPackTailLength();
            return size;
        }

        //设置包头预设长度	
        public void SetPackHeadPreLength(int sz)
        {
            packHeaderPreLen = sz;
        }

        //设置包尾长度	
        public void SetPackTailLength(int sz)
        {
            packTailSize = sz;
        }



        //设置数据尺寸到包头中保存

        public void SetDataLengthToPackHead(byte[] pack, int dataSize)
        {
            if (setDataLengthToPackHead != null)
                setDataLengthToPackHead(pack, dataSize);
        }

        public int GetMaxBufferSize()
        {
            return maxBufferSize;
        }

    }


}
