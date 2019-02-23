using System.Text;

namespace Net
{
    public class HttpMsgProcesser : MsgProcesser<HttpHeader, int>
    {
        BaseServer baseServer;

        public HttpMsgProcesser(BaseServer baseServer)
        {
            this.baseServer = baseServer;
        }

        public DePacketor CreateDePacketor()
        {
            DePacketor httpDePacketor = new DePacketor();
            httpDePacketor.SetPackHeadPreLength(0);
            httpDePacketor.SetPackTailLength(0);
            httpDePacketor.SetGetPackDataLengthCallBack(GetHttpPackDataLength);
            httpDePacketor.SetMsgProcesser(this);

            return httpDePacketor;
        }


        //判断此包是否为分片
        public override bool IsSingleFrame(HttpHeader frameHeader)
        {
            return false;
        }

        //获取当前包消息类型
        public override int GetMsgTypeValue(HttpHeader header)
        {
            return header.type;
        }

        public override bool ProcessSingleDataFrame(HttpHeader joinFrameHeader, ByteStream joinFrameDataStream, HttpHeader singleFrameHeader, ByteStream singleFrameDataStream)
        {
            joinFrameDataStream.isByteAlign = false;
            joinFrameDataStream.WriteBytes(singleFrameDataStream.GetBuf(), singleFrameDataStream.GetCurtOffset(), singleFrameDataStream.GetNumberOfWriteBytes());
            return true;
        }

        private int GetHttpPackDataLength(DePacketor dePacket, byte[] pack, int offset, int packLen, out int realPackHeadLen)
        {
            for (int i = offset; i < packLen - 3; i++)
            {
                if (pack[i] == '\r' &&
                    pack[i + 1] == '\n' &&
                    pack[i + 2] == '\r' &&
                    pack[i + 3] == '\n')
                {
                    realPackHeadLen = i + 4;
                    return 0;
                }
            }

            realPackHeadLen = -1;
            return -1;
        }

        public override int ReadStreamToHeader(HttpHeader header, ByteStream readStream, bool isNetToHost)
        {
            readStream.isByteAlign = false;

            byte[] bytepack = readStream.ReadBytes(readStream.GetBufSize());
            char[] pack = Encoding.ASCII.GetChars(bytepack);
            int packlen = readStream.GetBufSize();
            int state = 0;
            int startpos = 0, identlen = 0, nextpos = 0;

            while (true)
            {
                switch (state)
                {
                    case 0:
                        {
                            GetIdentifier(' ', pack, packlen, ref startpos, ref identlen);
                            if (identlen == 0)
                                goto error;

                            string key = new string(pack, startpos, identlen);
                            header.method = ShardRes.GetInstance().GetHttpRequestMethod(key);
                            if (header.method != HttpRequestMethod.HTTP_METHOD_UNKNOWN)
                                startpos = ReadReqHeaderReqRow(header, pack, packlen, ref startpos, ref identlen);
                            else
                                startpos = ReadRespHeaderStateRow(header, pack, packlen, ref startpos, ref identlen);

                            if (startpos == 0)
                                goto error;

                            state = 1;
                        }
                        break;

                    case 1:
                        {
                            while (true)
                            {
                                //头部字段名
                                nextpos = GetIdentifier(':', pack, packlen, ref startpos, ref identlen, true);
                                if (identlen == 0)
                                {
                                    startpos = nextpos;
                                    nextpos = MatchIdentifier("\r\n", pack, packlen, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        goto error;
                                    goto end;
                                }

                                string field = new string(pack, startpos, identlen);

                                //头部字段值
                                startpos = nextpos + 1;
                                nextpos = GetIdentifier('\r', pack, packlen, ref startpos, ref identlen, true);
                                if (identlen != 0)
                                {
                                    string value = new string(pack, startpos, identlen);
                                    header.fieldMap[field] = value;
                                }

                                startpos = nextpos;
                                nextpos = MatchIdentifier("\r\n", pack, packlen, ref startpos, ref identlen);
                                if (identlen == 0)
                                    goto error;

                                startpos = nextpos;
                            }
                        }
                       // break;

                }
            }


            error:
            header.isVaild = false;
            return 0;

            end:
            header.isVaild = true;
            return readStream.GetNumberOfCurtBytes();
        }

        int ReadReqHeaderReqRow(HttpHeader header, char[] pack, int packlen, ref int startpos, ref int identlen)
        {
            int nextpos;

            header.type = 0;

            //请求方法
            nextpos = GetIdentifier(' ', pack, packlen, ref startpos, ref identlen);
            if (identlen == 0)
                return 0;

            string key = new string(pack, startpos, identlen);
            header.method = ShardRes.GetInstance().GetHttpRequestMethod(key);
            if (header.method == HttpRequestMethod.HTTP_METHOD_UNKNOWN)
                return 0;

            //URL
            startpos = nextpos;
            nextpos = GetIdentifier(' ', pack, packlen, ref startpos, ref identlen);

            string a = new string(pack, startpos, 1);
            int cmp = string.Compare(a, "/");

            if (identlen == 0 || cmp != 0)
                return 0;

            header.url = new string(pack, startpos, identlen);


            //协议版本
            startpos = nextpos;
            nextpos = GetIdentifier('\r', pack, packlen, ref startpos, ref identlen, true);
            if (identlen == 0)
                return 0;

            header.ver = new string(pack, startpos, identlen);

            //\r\n
            startpos = nextpos;
            nextpos = MatchIdentifier("\r\n", pack, packlen, ref startpos, ref identlen);
            if (identlen == 0)
                return 0;

            return nextpos;
        }


        int ReadRespHeaderStateRow(HttpHeader header, char[] pack, int packlen, ref int startpos, ref int identlen)
        {
            int nextpos;

            header.type = 1;

            //协议版本
            nextpos = GetIdentifier(' ', pack, packlen, ref startpos, ref identlen);
            if (identlen == 0)
                return 0;

            header.ver = new string(pack, startpos, identlen);

            //状态码
            startpos = nextpos;
            nextpos = GetIdentifier(' ', pack, packlen, ref startpos, ref identlen);
            if (identlen == 0)
                return 0;

            string statecodestr = new string(pack, startpos, identlen);
            header.stateCode = int.Parse(statecodestr);

            //状态消息
            startpos = nextpos;
            nextpos = GetIdentifier('\r', pack, packlen, ref startpos, ref identlen, true);
            if (identlen == 0)
                return 0;

            header.stateMsg = new string(pack, startpos, identlen);

            //\r\n
            startpos = nextpos;
            nextpos = MatchIdentifier("\r\n", pack, packlen, ref startpos, ref identlen);
            if (identlen == 0)
                return 0;

            return nextpos;
        }

        public int WriteHttpHeader(ByteStream writeStream, HttpHeader header, bool isHostToNet)
        {
            string reqHeaderStr = CreateHttpReqHeader(header);
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(reqHeaderStr);

            writeStream.WriteBytes(byteArray, 0, byteArray.Length);
            return reqHeaderStr.Length;
        }

        string CreateHttpReqHeader(HttpHeader header)
        {
            //请求头
            if (header.type == 0)
            {
                string method = ShardRes.GetInstance().GetHttpRequestMethodStr(header.method);
                string req = method + " " + header.url + " " + header.ver + "\r\n";
                req += CreateHttpHeaderOptionStr(header) + "\r\n";
                return req;
            }

            //响应头
            string first = header.ver + " " + header.stateCode + " " + header.stateMsg + "\r\n";
            string headOpts = CreateHttpHeaderOptionStr(header);
            first += headOpts + "\r\n";
            return first;
        }


        string CreateHttpHeaderOptionStr(HttpHeader header)
        {
            if (header.fieldMap.Count == 0)
                return "";

            string opts = "";

            foreach (var opt in header.fieldMap)
            {
                opts += opt.Key + ":" + opt.Value + "\r\n";
            }

            return opts;
        }


        int MatchIdentifier(string matchStr, char[] pack, int packlen, ref int startpos, ref int identlen)
        {
            startpos = SkipSpace(pack, packlen, startpos);
            identlen = matchStr.Length;

            if (startpos + identlen > packlen)
            {
                identlen = 0;
                return startpos;
            }

            string a = new string(pack, startpos, identlen);
            int cmp = string.Compare(a, matchStr, true);

            if (cmp == 0)
                return startpos + identlen;

            identlen = 0;
            return startpos;
        }


        int GetIdentifier(char sep, char[] pack, int packlen, ref int startpos, ref int identlen, bool isRemoveSeqFrontSpace = false)
        {
            int endpos, nextpos;
            startpos = SkipSpace(pack, packlen, startpos);

            for (int i = startpos; i < packlen; i++)
            {
                if (pack[i] == sep)
                {
                    nextpos = i;
                    endpos = i - 1;

                    if (isRemoveSeqFrontSpace)
                    {
                        for (int j = endpos; j >= startpos; j--)
                        {
                            if (pack[j] == ' ')
                                continue;
                            endpos = j;
                            break;
                        }
                    }

                    identlen = endpos - startpos + 1;
                    return nextpos;
                }
            }

            identlen = 0;
            return startpos;
        }


        int SkipSpace(char[] pack, int packlen, int startpos)
        {
            for (int i = startpos; i < packlen; i++)
            {
                if (pack[i] == ' ')
                    continue;
                return i;
            }

            return packlen;
        }

       

    }
}
