
using Assets;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Net
{
    public class BaseServer
    {
        App app;
        Server server;
        int listenPort = 0;
        HttpMsgProcesser httpMsgProcesser;
        DePacketor httpDePacketor;

        GamePackMsgProcesser gamepackMsgProcesser;
        DePacketor gamepackDePacketor;
        NetSocket clientSocket;
        ulong clientSocketID;
        uint playerID;
        string token;

        static public readonly string Sec_WebSocket_Key = "caiyeminliyajincaizhua";
        static public readonly string Sec_WebSocket_Key_2 = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        public BaseServer(App app, TaskProcesser taskProcesser)
        {
            this.app = app;
            Init(taskProcesser);
            HttpMsgReg();
        }
        void Init(TaskProcesser taskProcesser)
        {
            httpMsgProcesser = new HttpMsgProcesser(this);
            httpDePacketor = httpMsgProcesser.CreateDePacketor();

            gamepackMsgProcesser = new GamePackMsgProcesser(this);
            gamepackDePacketor = gamepackMsgProcesser.CreateDePacketor();

            server = new Server();
            server.SetServerTaskProcess(-1, taskProcesser);
            server.SetListenPort(listenPort);
            server.SetDePacketor(httpDePacketor);
        }


        Server GetServer()
        {
            return server;
        }

        public void ConnectServer(string ip, int port)
        {
            ServerInfo serverInfo = new ServerInfo();
            serverInfo.serverIP = ip;
            serverInfo.serverPort = port;
            serverInfo.dePacketor = server.dePacketor;
            server.ConnectServer(serverInfo);
        }

        void HttpMsgReg()
        {
            httpMsgProcesser.RegSocketEvent(SocketEvent.EV_SOCKET_CONNECTED, SocketConnected);
            httpMsgProcesser.RegSocketEvent(SocketEvent.EV_SOCKET_OFFLINE, SocketOffline);
            httpMsgProcesser.RegMsg(1, RecvRespUpgradeProtoco);

            gamepackMsgProcesser.RegSocketEvent(SocketEvent.EV_SOCKET_OFFLINE, SocketOffline);
            gamepackMsgProcesser.RegMsg(GamePackType.MSG_LOGIN_RESP_CLIENT_REQ_LOGIN, LoginServerRespReqLogin);
            gamepackMsgProcesser.RegMsg(GamePackType.MSG_CLOSED, RevClosedMsg);
        }


        public void ReqLogin(string account, string password)
        {
            clientSocket = server.GetSocket(clientSocketID);
            if (clientSocket == null)
                return;

            ByteStream sendStream = new ByteStream();
            GamePackHeader header = new GamePackHeader();
            header.type =  GamePackType.MSG_CLIENT_REQ_LOGIN_ENTER;
            header.dataSize = 0;

            int headerlen = gamepackMsgProcesser.WriteGamePackHeader(sendStream, header);

            byte[] accountArray = Encoding.UTF8.GetBytes(account);
            byte[] pwdArray = Encoding.UTF8.GetBytes(password);

            sendStream.Write((int)DefValue.DEF_CLIENT_COMMON_LOGIN);
            sendStream.Write(account.Length);
            sendStream.WriteBytes(accountArray, 0, accountArray.Length);
            sendStream.Write(pwdArray.Length);
            sendStream.WriteBytes(pwdArray, 0, pwdArray.Length);
            clientSocket.Send(sendStream, headerlen, 0);
        }

        bool RevClosedMsg(NetSocket socket, GamePackHeader header, ByteStream dataStream)
        {
            socket.Close();
            return true;
        }

        bool LoginServerRespReqLogin(NetSocket socket, GamePackHeader header, ByteStream dataStream)
        {
            int value;
            int loginWay;
            byte tokenLen;
            byte[] _token;
            byte ipLen;
            string gateIp;
            int gatePort;


            dataStream.Read(out value);
            dataStream.Read(out loginWay);

            switch ((DefValue)value)
            {
                case  DefValue.DEF_NEED_AGAIN_CONNECT_SERVER:
                    {      
                           // MsgBox::ShowMsgBox(WideByte2UTF8(L"服务器断线，请重新连接服务器!"), 0, curtScene);
                    }
                    break;

                case DefValue.DEF_LOGIN_SUCCESS:
                    {
                        dataStream.Read(out playerID);
                        dataStream.Read(out ipLen);
                        byte[] gateIpBytes = dataStream.ReadBytes(ipLen);
                        gateIp = Encoding.UTF8.GetString(gateIpBytes);
                        dataStream.Read(out gatePort);

                        if ((DefValue)loginWay == DefValue.DEF_CLIENT_COMMON_LOGIN)
                        {
                            dataStream.Read(out tokenLen);
                            _token = dataStream.ReadBytes(tokenLen);
                            char[] tokenChars = Encoding.UTF8.GetChars(_token);
                            token = new string(tokenChars);
                            WriteTokenToCache(token);
                        }

                        app.resPool.LoadScene();
                        ConnectServer(gateIp, gatePort);
                    }
                    break;

                case DefValue.DEF_LOGIN_FAILD:
                    {
                        if ((DefValue)loginWay == DefValue.DEF_CLIENT_TOKEN_LOGIN)
                        {
                          //  Director::getInstance()->replaceScene(loginScene);
                        }
                        else if ((DefValue)loginWay == DefValue.DEF_CLIENT_COMMON_LOGIN)
                        {
                            //MsgBox::ShowMsgBox(WideByte2UTF8(L"登陆失败!"), 0, curtScene);
                        }
                    }
                    break;

            }

            return true;
        }


        void WriteTokenToCache(string tokenText)
        {
            //FileUtils* fu = FileUtils::getInstance();
            //tinyxml2::XMLDocument doc;
            //doc.Parse(fu->getStringFromFile("cache.xml").c_str());

            //XMLElement* root = doc.FirstChildElement("Cache");
            //XMLElement* token = root->FirstChildElement("Token");
            //token->SetText(tokenText.c_str());

            //tinyxml2::XMLPrinter printer;
            //doc.Print(&printer);
            //fu->writeStringToFile(printer.CStr(), "cache.xml");
        }


        private bool RecvRespUpgradeProtoco(NetSocket socket, HttpHeader header, ByteStream dataStream)
        {
           // DEBUG("接收到远端%s响应请求更换通信协议。", socket.remoteIP);

            if (header.isVaild == false)
                return true;

            if (header.stateCode != 101)
                return true;


            if (!header.fieldMap.ContainsKey("Upgrade") ||
                !header.fieldMap.ContainsKey("Connection") ||
                !header.fieldMap.ContainsKey("Sec-WebSocket-Accept") ||
                header.fieldMap["Connection"].CompareTo("Upgrade") != 0)
            {
               // DEBUG("远端%s响应请求更换通信协议不合法。", socket.remoteIP);
                socket.Close();
                return true;
            }

            //
            string stroIn = header.fieldMap["Sec-WebSocket-Accept"];
            byte[] byteArray = Convert.FromBase64String(stroIn);
            string pOut = Encoding.Default.GetString(byteArray);


            byte[] bytedata = Encoding.Default.GetBytes(Sec_WebSocket_Key);
            string seckey = Convert.ToBase64String(bytedata);
            string  key = seckey + Sec_WebSocket_Key_2;


            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_in = Encoding.Default.GetBytes(key);
            byte[] bytes_out = sha1.ComputeHash(bytes_in);
            string chSha1 = Encoding.Default.GetString(bytes_out);

            if (chSha1.CompareTo(pOut) != 0)
            {
                //DEBUG("远端%s响应请求更换通信协议不合法。", socket.remoteIP);
                socket.Close();
                return true;
            }

            if (header.fieldMap["Upgrade"].CompareTo("GamePackProtocol") == 0)
            {
                socket.SetDePacketor(gamepackDePacketor);
                socket.SetMsgProcesser(gamepackMsgProcesser);
            }
          
            return true;
        }

        private bool SocketConnected(NetSocket socket)
        {
            clientSocketID = socket.GetID();
            clientSocket = socket;
            SendReqUpgradeProtocol(socket,"", "GamePackProtocol");
            return true;
        }

        private bool SocketOffline(NetSocket socket)
        {
            return true;
        }

        void SendReqUpgradeProtocol(NetSocket socket, string host, string protocol)
        {
           // ServerInfo serverInfo = socket.GetRemoteServerInfo();
          //  ServerType toServerType = (ServerType)serverInfo->GetTag();
           // Debug.Log("请求%s(%s:%d)更换通信协议为：%s!", SERVER_CN_NAME(toServerType), serverInfo->GetServerIP(), serverInfo->GetServerPort(), protocol.c_str());

            ByteStream sendStream = new ByteStream();
            HttpHeader header = new HttpHeader();

            sendStream.isByteAlign = false;

            header.type = 0;
            header.method = HttpRequestMethod.HTTP_METHOD_GET;
            header.url = "/chat";
            header.ver = "HTTP/1.1";

            if (host.Length != 0)
                header.fieldMap["Host"] = host;

            header.fieldMap["Connection"] = "Upgrade";
            header.fieldMap["Upgrade"] = protocol;
            header.fieldMap["Sec-WebSocket-Version"] = "13";

            byte[] bytedata = Encoding.Default.GetBytes(Sec_WebSocket_Key);
            string basevalue = Convert.ToBase64String(bytedata);


            header.fieldMap["Sec-WebSocket-Key"] = basevalue;

            httpMsgProcesser.WriteHttpHeader(sendStream, header, false);
            socket.Send(sendStream);
        }
    }
}
