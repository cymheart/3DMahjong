namespace Net
{
    public class GamePackMsgProcesser : MsgProcesser<GamePackHeader, GamePackType>
    {
        BaseServer baseServer;
        public GamePackMsgProcesser(BaseServer baseServer)
        {
            this.baseServer = baseServer;
        }

        public DePacketor CreateDePacketor()
        {
            DePacketor gamepackDePacketor = new DePacketor();
            gamepackDePacketor.SetPackHeadPreLength(GetPackHeaderSize());
            gamepackDePacketor.SetPackTailLength(0);
            gamepackDePacketor.SetGetPackDataLengthCallBack(GetPackDataLength);
            gamepackDePacketor.SetSetDataLengthToPackHeadCallBack(SetDataLengthToPackHead);
            gamepackDePacketor.SetMsgProcesser(this);

            return gamepackDePacketor;
        }


        //判断此包是否为分片
        public override bool IsSingleFrame(GamePackHeader frameHeader)
        {
            return false;
        }

        //获取当前包消息类型
        public override GamePackType GetMsgTypeValue(GamePackHeader header)
        {
            return header.type;
        }

        public override bool ProcessSingleDataFrame(GamePackHeader joinFrameHeader, ByteStream joinFrameDataStream, GamePackHeader singleFrameHeader, ByteStream singleFrameDataStream)
        {
            joinFrameDataStream.isByteAlign = false;
            joinFrameDataStream.WriteBytes(singleFrameDataStream.GetBuf(), singleFrameDataStream.GetCurtOffset(), singleFrameDataStream.GetNumberOfWriteBytes());
            return true;
        }

        int GetPackDataLength(DePacketor dePacketor, byte[] pack, int offset, int packLen, out int realPackHeadLen)
        {
            int type;
            short dataSize;

            ByteStream readStream = new ByteStream(pack, offset, packLen);
            readStream.Read(out type);
            readStream.Read(out dataSize);
            readStream.ReadAlignBytes();

            realPackHeadLen = readStream.GetNumberOfCurtBytes();

            return dataSize;
        }

        void SetDataLengthToPackHead(byte[] pack, int dataSize)
        {
            int headerType = 0;
            short size = (short)dataSize;

            ByteStream readStream = new ByteStream(pack);
            readStream.Read(out headerType);
            readStream.Write(size);
        }

        public override int ReadStreamToHeader(GamePackHeader header, ByteStream readStream, bool isNetToHost)
        {
            int headerType = 0;
            readStream.Read(out headerType);
            header.type = (GamePackType)headerType;

            readStream.Read(out header.dataSize);
            readStream.ReadAlignBytes();
            return readStream.GetNumberOfCurtBytes();
        }

        public int WriteGamePackHeader(ByteStream readStream, GamePackHeader header, bool isHostToNet = true)
        {
            readStream.Write((int)header.type);
            readStream.Write(header.dataSize);
            readStream.WriteAlignBytes();
            return readStream.GetNumberOfWriteBytes();
        }

        int GetPackHeaderSize()
        {
            ByteStream cStream = new ByteStream();
            int headerType = 0;
            short size = 0;

            cStream.Read(out headerType);
            cStream.Read(out size);
            cStream.IgroneAlignBytes();
            return cStream.GetNumberOfCurtBytes();
        }

    }
}
