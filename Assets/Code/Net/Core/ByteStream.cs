using System.IO;
using System.Text;
using System;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Net
{
    public class ByteStream
    {
        public bool canReused = false;
        public bool isWriteHostToNet = true;
        public bool isReadNetToHost = true;
        public bool isError = false;
        public bool isByteAlign = true;
        int maxAlginBytes = 4;
        byte[] defaultBuf = new byte[1024];
        int curtWriteByteNum = 0;
        byte[] externBuf = null;
        byte[] curtBuf = null;
        int curtOffset = 0;
        int curtPos = 0;     
        int curtBufLen = 0;

        public ByteStream(byte[] _externBuf = null, int _externBufOffset = 0, int _externBufLen = 0)
        {
            externBuf = _externBuf;

            if (externBuf == null)
            {
                curtBuf = defaultBuf;
                curtBufLen = defaultBuf.Length;
            }
            else
            {
                curtBuf = externBuf;
                curtOffset = _externBufOffset;

                if (_externBufLen == 0)
                {
                    curtBufLen = curtBuf.Length;
                    curtWriteByteNum = curtBufLen;
                }
                else
                {
                    curtBufLen = _externBufLen;
                    curtWriteByteNum = _externBufLen;
                }
            }

            curtPos = 0;
        }

        //根据设置有可能会拷贝当前buf，或者直接获取当前buf
        public byte[] TakeBuf()
        {
            byte[] buf;

            if (canReused)
            {
                buf = new byte[GetNumberOfWriteBytes()];
                Buffer.BlockCopy(curtBuf, curtOffset, buf, 0, GetNumberOfWriteBytes());
                return buf;
            }

            if (curtBuf != defaultBuf)
            {
                return curtBuf;
            }

            buf = new byte[GetNumberOfWriteBytes()];
            Buffer.BlockCopy(curtBuf, curtOffset, buf, 0, GetNumberOfWriteBytes());
            return buf;
        }

        //直接获取外部buf指针
        public byte[] GetExternBuf()
        {
            return externBuf;
        }

        //直接获取buf指针
        public byte[] GetBuf()
        {
            return curtBuf;
        }

        //直接获取buf的当前位置
        public int GetCurtPos()
        {
            return curtPos;
        }

        public int GetCurtOffset()
        {
            return curtOffset;
        }

        //设置buf的当前位置
        public int SetCurt(int _curtPos)
        {
            if (_curtPos > curtWriteByteNum)
                _curtPos = curtWriteByteNum;
            else if (_curtPos < 0)
                _curtPos = 0;

            curtPos = _curtPos;

            return curtPos;
        }

        //设置buf的当前位置到后面位置
        public int Next(int pos)
        {
            return SetCurt(curtPos + pos);
        }

        //获取buf的总大小
        public int GetBufSize()
        {
            return curtBufLen;
        }

        //获取总的写入字节数
        public int GetNumberOfWriteBytes()
        {
            return curtWriteByteNum;
        }

        //获取当前位置的前向字节数
        public int GetNumberOfCurtBytes()
        {
            return curtPos;
        }

        //获取当前位置后剩余已写入的字节数
        public int GetNumberOfRichBytes()
        {
            return curtWriteByteNum - curtPos;
        }

        //重设外部buf
        public void ResetExtrenBuf(byte[] _extrenBuf, int _externBufOffset = 0, int _externBufLen = 0)
        {
            Clear();
            externBuf = _extrenBuf;
            curtBuf = externBuf;
            curtOffset = _externBufOffset;
        
            if (_externBufLen == 0)
            {
                curtBufLen = curtBuf.Length;
                curtWriteByteNum = curtBufLen;
            }
            else
            {
                curtBufLen = _externBufLen;
                curtWriteByteNum = _externBufLen;
            }
        }

        public void Clear()
        {
            isError = false;
            curtWriteByteNum = 0;
            curtPos = 0;
            curtOffset = 0;
            curtBufLen = 0;
        }

        public void ClearCurtRichBytes(int byteNum)
        {
            int richlen = curtPos + byteNum - curtBuf.Length;
            if (richlen > 0)
            {
                if (TestWriteBufSize(curtPos + richlen) != 0)
                    return;
                byteNum -= richlen;
            }

            Array.Clear(curtBuf, curtOffset + curtPos, byteNum);
        }

        int TestWriteBufSize(int testWriteEndPos)
        {
            if (testWriteEndPos >= curtBufLen)
            {
                if (externBuf != null)
                    return 1;

                int allocSize = curtBufLen * 2;
                if (allocSize < testWriteEndPos)
                    allocSize = testWriteEndPos * 2;

                byte[] newBuf = new byte[allocSize];
                Buffer.BlockCopy(curtBuf, curtOffset + curtPos, newBuf, 0, curtBufLen);
                curtBuf = newBuf;
                curtOffset = 0;
                return 0;
            }

            return 0;
        }

        int AlignWriteAddr(int structBytes, bool isTestWriteBuf = false)
        {
            int ret = 0;
            int offsetBytes = 0;
            int alginBytes = 0;

            if (isByteAlign)
            {
                if (structBytes > maxAlginBytes)
                    alginBytes = maxAlginBytes;
                else
                    alginBytes = structBytes;

                int n = curtPos % alginBytes;
                if (n != 0)
                    offsetBytes = alginBytes - n;
            }

            if (isTestWriteBuf)
            {
                ret = TestWriteBufSize(curtPos + offsetBytes + structBytes);
                if (ret != 0)
                    return ret;
            }

            curtPos += offsetBytes;
            return ret;
        }
        public void Write(byte val)
        {
            if (AlignWriteAddr(sizeof(byte), true) != 0)
                return;

            curtBuf[curtOffset + curtPos] = val;
            curtPos += sizeof(byte);
            if (curtPos > curtWriteByteNum)
                curtWriteByteNum = curtPos;
        }

        public void Write(short val)
        {
            if (AlignWriteAddr(sizeof(short), true) != 0)
                return;

            if(isWriteHostToNet)
                val = IPAddress.HostToNetworkOrder(val);

            byte[] btValue = BitConverter.GetBytes(val);
            Buffer.BlockCopy(btValue, 0, curtBuf, curtOffset + curtPos, sizeof(short));
            curtPos += sizeof(short);

            if (curtPos > curtWriteByteNum)
                curtWriteByteNum = curtPos;
        }

        public void Write(int val)
        {
            if (AlignWriteAddr(sizeof(int), true) != 0)
                return;

            if (isWriteHostToNet)
                val = IPAddress.HostToNetworkOrder(val);

            byte[] btValue = BitConverter.GetBytes(val);
            Buffer.BlockCopy(btValue, 0, curtBuf, curtOffset + curtPos, sizeof(int));
            curtPos += sizeof(int);

            if (curtPos > curtWriteByteNum)
                curtWriteByteNum = curtPos;
        }

        public void Write(long val)
        {
            if (AlignWriteAddr(sizeof(long), true) != 0)
                return;

            if (isWriteHostToNet)
                val = IPAddress.HostToNetworkOrder(val);

            byte[] btValue = BitConverter.GetBytes(val);
            Buffer.BlockCopy(btValue, 0, curtBuf, curtOffset + curtPos, sizeof(long));
            curtPos += sizeof(long);

            if (curtPos > curtWriteByteNum)
                curtWriteByteNum = curtPos;
        }


        public void Write(uint val)
        {
            Write((int)val);
        }

        public void Write(ulong val)
        {
            Write((long)val);
        }

        public void Write(ushort val)
        {
            Write((short)val);
        }

        public void Write(float val)
        {
            if (AlignWriteAddr(sizeof(float), true) != 0)
                return;

            byte[] btValue = BitConverter.GetBytes(val);
            int m = BitConverter.ToInt32(btValue, 0);

            if (isWriteHostToNet)
                m = IPAddress.HostToNetworkOrder(m);

            btValue = BitConverter.GetBytes(m);
            Buffer.BlockCopy(btValue, 0, curtBuf, curtOffset + curtPos, sizeof(int));
            curtPos += sizeof(float);

            if (curtPos > curtWriteByteNum)
                curtWriteByteNum = curtPos;
        }

        public void Write(double val)
        {
            if (AlignWriteAddr(sizeof(double), true) != 0)
                return;

            byte[] btValue = BitConverter.GetBytes(val);
            long m = BitConverter.ToInt64(btValue, 0);

            if (isWriteHostToNet)
                m = IPAddress.HostToNetworkOrder(m);

            btValue = BitConverter.GetBytes(m);
            Buffer.BlockCopy(btValue, 0, curtBuf, curtOffset + curtPos, sizeof(long));
            curtPos += sizeof(double);

            if (curtPos > curtWriteByteNum)
                curtWriteByteNum = curtPos;
        }

        public void Write(object val)
        {
            GCHandle handle = GCHandle.Alloc(val);  //记得要释放  handle.Free();
            IntPtr mpVoid = GCHandle.ToIntPtr(handle);
            Write(mpVoid.ToInt64());
        }

        public void WriteBytes(byte[] inByteArray, int inByteArrayOffset,  int numberOfBytesToWrite)
        {
            int offsetBytes = 0;

            if (isByteAlign)
            {
                int n = curtPos % maxAlginBytes;
                if (n != 0) { offsetBytes = maxAlginBytes - n; }
            }

            if (TestWriteBufSize(curtPos + offsetBytes + numberOfBytesToWrite) != 0)
                return;

            curtPos += offsetBytes;
            Buffer.BlockCopy(inByteArray, inByteArrayOffset, curtBuf, curtOffset + curtPos, numberOfBytesToWrite);
            curtPos += numberOfBytesToWrite;

            WriteAlignBytes();
        }

        public void WriteAlignBytes()
        {
            int offsetBytes = 0;

            if (isByteAlign)
            {
                int n = curtPos % maxAlginBytes;
                if (n != 0) { offsetBytes = maxAlginBytes - n; }

                if (TestWriteBufSize(curtPos + offsetBytes) != 0)
                    return;
            }

            curtPos += offsetBytes;
            if (curtPos > curtWriteByteNum)
                curtWriteByteNum = curtPos;
        }

        public void WriteByteBits(byte bitsValue, int startBit, int endBit, bool toNextByte = false)
        {
            if (startBit < 0 || startBit > 7 ||
                endBit < 0 || endBit > 7 || startBit > endBit)
                return;

            if (TestWriteBufSize(curtPos) != 0)
                return;

            byte byteValue = (byte)((bitsValue << startBit) & (0xff >> (7 - endBit)));
            curtBuf[curtOffset + curtPos] |= byteValue;


            if (curtPos > curtWriteByteNum)
                curtWriteByteNum = curtPos;

            if (toNextByte)
                curtPos++;
        }


        void AlignReadAddr(int structBytes)
        {
            if (!isByteAlign)
                return;

            int alginBytes;
            if (structBytes > maxAlginBytes)
                alginBytes = maxAlginBytes;
            else
                alginBytes = structBytes;

            int n = curtPos % alginBytes;
            int offsetBytes = 0;
            if (n != 0)
                offsetBytes = alginBytes - n;

            curtPos += offsetBytes;
        }


        public void Read(out byte val)
        {
            if (isError)
            {
                val = 0;
                return;
            }

            AlignReadAddr(sizeof(byte));

            if (curtBufLen != 0 &&
                curtPos + sizeof(byte) > curtBufLen)
            {
                val = 0;
                isError = true;
                return;
            }

            val = curtBuf[curtOffset + curtPos];
            curtPos += sizeof(byte);
        }

        public void Read(out short val)
        {
            if (isError)
            {
                val = 0;
                return;
            }

            AlignReadAddr(sizeof(short));

            if (curtBufLen != 0 &&
              curtPos + sizeof(short) > curtBufLen)
            {
                val = 0;
                isError = true;
                return;
            }

            val = BitConverter.ToInt16(curtBuf, curtOffset + curtPos);

            if (isReadNetToHost)
                val = IPAddress.NetworkToHostOrder(val);

            curtPos += sizeof(short);
        }

        public void Read(out int val)
        {
            if (isError)
            {
                val = 0;
                return;
            }

            AlignReadAddr(sizeof(int));

            if (curtBufLen != 0 &&
              curtPos + sizeof(int) > curtBufLen)
            {
                val = 0;
                isError = true;
                return;
            }

            val = BitConverter.ToInt32(curtBuf, curtOffset + curtPos);
            if(isReadNetToHost)
                val = IPAddress.NetworkToHostOrder(val);
 
            curtPos += sizeof(int);
        }

        public void Read(out long val)
        {
            if (isError)
            {
                val = 0;
                return;
            }

            AlignReadAddr(sizeof(long));

            if (curtBufLen != 0 &&
            curtPos + sizeof(long) > curtBufLen)
            {
                val = 0;
                isError = true;
                return;
            }

            val = BitConverter.ToInt64(curtBuf, curtOffset + curtPos);

            if (isReadNetToHost)
                val = IPAddress.NetworkToHostOrder(val);

            curtPos += sizeof(long);
        }

        public void Read(out ushort val)
        {
            short sval = 0;
            Read(out sval);
            val = (ushort)sval;
        }

        public void Read(out uint val)
        {
            int ival = 0;
            Read(out ival);
            val = (uint)ival;
        }

        public void Read(out ulong val)
        {
            long lval = 0;
            Read(out lval);
            val = (ulong)lval;
        }

        public void Read(out float val)
        {
            if (isError)
            {
                val = 0;
                return;
            }

            AlignReadAddr(sizeof(float));

            if (curtBufLen != 0 &&   
                curtPos + sizeof(float) > curtBufLen)
            {
                val = 0;
                isError = true;
                return;
            }

            int m = BitConverter.ToInt32(curtBuf, curtOffset + curtPos);

            if(isReadNetToHost)
                m = IPAddress.NetworkToHostOrder(m);

            byte[] btValue = BitConverter.GetBytes(m);
            val = BitConverter.ToSingle(btValue, 0);
            curtPos += sizeof(float);
        }

        public void Read(out double val)
        {
            if (isError)
            {
                val = 0;
                return;
            }

            AlignReadAddr(sizeof(double));

            if (curtBufLen != 0 &&
               curtPos + sizeof(double) > curtBufLen)
            {
                val = 0;
                isError = true;
                return;
            }

            long m = BitConverter.ToInt64(curtBuf, curtOffset + curtPos);

            if (isReadNetToHost)
                m = IPAddress.NetworkToHostOrder(m);

            byte[] btValue = BitConverter.GetBytes(m);
            val = BitConverter.ToDouble(btValue, 0);
            curtPos += sizeof(double);
        }

        public GCHandle Read(out object val)
        {
            long nobj;
            Read(out nobj);

            if(isError)
            {
                val = 0;
                return GCHandle.FromIntPtr(IntPtr.Zero);
            }
                
            IntPtr p = new IntPtr(nobj);
            GCHandle handle = GCHandle.FromIntPtr(p);
            val = handle.Target;

            return handle;
        }


        public byte[] ReadBytes(int readByteCount = 0)
        {
            if (isError)
            {
                return null;
            }

            ReadAlignBytes();

            curtPos += readByteCount;
            if (curtPos > curtBufLen)
            {
                isError = true;
                return null;
            }

            byte[] readBytes = new byte[readByteCount];
            Buffer.BlockCopy(curtBuf, curtOffset + curtPos - readByteCount, readBytes, 0, readByteCount);

            return readBytes;
        }


        public void ReadAlignBytes()
        {
            if (!isByteAlign)
                return;

            int n = curtPos % maxAlginBytes;
            int offsetBytes = 0;
            if (n != 0) { offsetBytes = maxAlginBytes - n; }
            curtPos += offsetBytes;
        }

        //读取字节位值
        public byte ReadByteBits(int startBit, int endBit, bool toNextByte = false)
        {
            if (curtPos > curtBuf.Length)
            {
                isError = true;
                return 0;
            }

            if (startBit < 0 || startBit > 7 ||
                endBit < 0 || endBit > 7 || startBit > endBit)
                return 0;

            byte byteValue = curtBuf[curtOffset + curtPos];
            int shr = startBit;
            byte bitsValue = (byte)((byteValue & (0xff >> (7 - endBit))) >> shr);

            if (toNextByte)
                curtPos++;

            return bitsValue;
        }

        public void IgroneAlignBytes()
        {
            ReadAlignBytes();
        }

    }
}