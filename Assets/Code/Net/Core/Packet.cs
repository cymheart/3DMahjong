using System;

namespace Net
{
    public class Packet
    {
        public PackBuf packBuf;                                   // WSA类型的缓冲区，用于给重叠操作传参数的
        public byte[] buf;                                       // 这个是PackBuf里具体存字符的缓冲区
        public int transferedBytes;
        int maxBufSize;
        public NetSocket socketCtx;
        public ulong socketID;
        public ServerTask serverTask;
        public IAsyncResult ar;

        public Packet(NetSocket _socketCtx, int _maxBufSize = 1024)
        {
 
            socketCtx = _socketCtx;
            serverTask = socketCtx.serverTask;
            socketID = socketCtx.GetID();  
            maxBufSize = _maxBufSize;

            if (_maxBufSize != 0)
            {
                buf = new byte[maxBufSize];
                Array.Clear(buf, 0, buf.Length);
            }

            packBuf.buf = buf;
            packBuf.len = maxBufSize;
        }

        public void ResetBuffer(byte[] _buf, int bufsize, bool isCopy = false)
        {
            if (_buf == null || bufsize <= 0)
            {
                buf = null;
                maxBufSize = 0;
                packBuf.buf = buf;
                packBuf.len = maxBufSize;
                return;
            }

            if (isCopy)
            {
                Buffer.BlockCopy(_buf, 0, buf, 0, bufsize);
            }
            else
            {
                buf = _buf;
            }

            maxBufSize = bufsize;
            packBuf.buf = buf;
            packBuf.len = maxBufSize;
        }

    }
}
