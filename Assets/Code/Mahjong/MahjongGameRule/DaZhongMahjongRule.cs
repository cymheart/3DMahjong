using MahjongMachineNS;
using System;
using System.Collections.Generic;
using System.Threading;
using Task;

public class DaZhongMahjongRule
{
    MahjongMachine mjMachine;
    CommonTaskProcesser taskProcesser;

    int mjTotalCount = 144;

    MahjongFaceValue[] paiDuiCards;

    List<MahjongFaceValue>[] handPais = new List<MahjongFaceValue>[]
    {
        new List<MahjongFaceValue>(),
        new List<MahjongFaceValue>(),
        new List<MahjongFaceValue>(),
        new List<MahjongFaceValue>()
    };

    List<MahjongFaceValue>[] huas = new List<MahjongFaceValue>[]
    {
        new List<MahjongFaceValue>(),
        new List<MahjongFaceValue>(),
        new List<MahjongFaceValue>(),
        new List<MahjongFaceValue>()
    };

    List<MahjongFaceValue>[] buPais = new List<MahjongFaceValue>[]
   {
        new List<MahjongFaceValue>(),
        new List<MahjongFaceValue>(),
        new List<MahjongFaceValue>(),
        new List<MahjongFaceValue>()
   };

    int curtPaiDuiPos = 0;
    int richMjCount = 0;

    /// <summary>
    /// 按圈风位置顺位的座位号， 0号为当前圈风位号
    /// </summary>
    public int[] orderSeatIdx = new int[4];
    public FengWei[] seatFengWei = new FengWei[4];

    int dealerSeatIdx = 0;

    int[] dictNum = new int[2] { 1, 1 };

    int state = 0;

    private bool m_IsExitThread = false;
    private Thread m_EventThread = null;

    public DaZhongMahjongRule(MahjongMachine mjMachine, CommonTaskProcesser taskProcesser)
    {
        this.mjMachine = mjMachine;
        this.taskProcesser = taskProcesser;

        CreateMjCards();
        StartThread();
    }

    private void StartThread()
    {
        m_EventThread = new Thread(this.Update);
        m_EventThread.IsBackground = true;
        m_EventThread.Start();
    }

    public void Stop()
    {
        m_IsExitThread = true;

        if (m_EventThread.ThreadState == ThreadState.Running)
        {
            m_EventThread.Abort();
        }
    }

    public void Update()
    {
        while (!m_IsExitThread)
        {
            switch (state)
            {
                case 0:
                    {
                        XiPai();
                        SetDealer(3, FengWei.EAST);
                        Dict();
                        FaPai();
                        SendClientXiPaiCmdList(0);

                        state = 1;
                    }
                    break;

                case 1:
                    {
                       
                        state = 2;
                    }
                    break;
            }
        }
    }

    void TaskCallBack(TaskDataBase data)
    {
        mjMachine.AppendMjOpCmd((MahjongMachineCmd)data);
    }


    void SendClientXiPaiCmdList(int clientSeat0AtServer)
    {
        XiPaiCmd xipaiCmd = (XiPaiCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.XiPai);
        xipaiCmd.dealerSeatIdx = TransformServerSeatToClientSeat(clientSeat0AtServer, orderSeatIdx[0]);
        xipaiCmd.fengWei = seatFengWei[orderSeatIdx[0]];
        taskProcesser.PostTask(TaskCallBack, xipaiCmd);

        //
        QiDongDiceMachineCmd qiDongCmd = (QiDongDiceMachineCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.QiDongDiceMachine);
        qiDongCmd.isBlock = true;
        qiDongCmd.dice1Point = dictNum[0];
        qiDongCmd.dice2Point = dictNum[1];
        qiDongCmd.seatIdx = TransformServerSeatToClientSeat(clientSeat0AtServer, orderSeatIdx[0]);
        taskProcesser.PostTask(TaskCallBack, qiDongCmd);

        //
        FaPaiCmd faPaiCmd = (FaPaiCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.FaPai);
        faPaiCmd.startPaiIdx = GetStartFaPaiPosByDictNumForClient(clientSeat0AtServer, dictNum);
        faPaiCmd.mjHandSelfPaiFaceValueList.AddRange(handPais[clientSeat0AtServer]);
        faPaiCmd.selfHuaList.AddRange(huas[clientSeat0AtServer]);
        faPaiCmd.selfBuPaiList.AddRange(buPais[clientSeat0AtServer]);
        faPaiCmd.isBlock = true;
        taskProcesser.PostTask(TaskCallBack, faPaiCmd);

        //
        int seatIdx;
        MahjongBuHuaPaiOpCmd buHuaCmd;
        for (int i = 0; i < 4; i++)
        {
            seatIdx = TransformServerSeatToClientSeat(clientSeat0AtServer, orderSeatIdx[i]);

            for (int j = 0; j < huas[orderSeatIdx[i]].Count; j++)
            {
                buHuaCmd = (MahjongBuHuaPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.BuHuaPai);
                buHuaCmd.seatIdx = seatIdx;
                buHuaCmd.handStyle = PlayerType.FEMALE;
                buHuaCmd.buHuaPaiFaceValue =huas[orderSeatIdx[i]][j];
                taskProcesser.PostTask(TaskCallBack, buHuaCmd);
            }
        }


        //
        ReqSelectDaPaiOpCmd reqSelectDaPaiCmd = (ReqSelectDaPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ReqSelectDaPai);
        taskProcesser.PostTask(TaskCallBack, reqSelectDaPaiCmd);

    }

    void CreateMjCards()
    {
        richMjCount = mjTotalCount;
        paiDuiCards = new MahjongFaceValue[mjTotalCount + 1];

        int idx = 1;

        for (int i = 0; i < (int)MahjongFaceValue.MJ_UNKNOWN; i++)
        {
            if (i < (int)MahjongFaceValue.MJ_HUA_CHUN)
            {
                for (int j = 0; j < 4; j++)
                    paiDuiCards[idx++] = (MahjongFaceValue)i;
            }
            else
            {
                paiDuiCards[idx++] = (MahjongFaceValue)i;
            }
        }
    }

    /// <summary>
    /// 设置圈风位
    /// </summary>
    /// <param name="dealerSeatIdx"></param>
    public void SetDealer(int dealerSeatIdx, FengWei fengWei)
    {
        int fw = (int)fengWei;
        this.dealerSeatIdx = dealerSeatIdx;

        for (int i = 0; i < 4; i++)
        {
            orderSeatIdx[i] = dealerSeatIdx;

            fw %= 4;
            seatFengWei[dealerSeatIdx] = (FengWei)fw;
            fw++;

            dealerSeatIdx--;
            if (dealerSeatIdx == -1)
                dealerSeatIdx = 3;
        }
    }


    void XiPai()
    { 
        int toIdx = 0;
        MahjongFaceValue tmp;
        Random random = new Random();

        for (int n = 0; n < 2; n++)
        {
            for (int i = 1; i <= mjTotalCount; i++)
            {
                toIdx = random.Next(1, mjTotalCount + 1);
                tmp = paiDuiCards[i];
                paiDuiCards[i] = paiDuiCards[toIdx];
                paiDuiCards[toIdx] = tmp;
            }
        }
    }

 
    void Dict()
    {
        Random random = new Random();
        dictNum[0] = random.Next(1, 7);
        dictNum[1] = random.Next(1, 7);

    }


    /// <summary>
    /// 根据骰子点数获取发牌在牌堆的起始位置（获取的是服务端牌堆起始位置）
    /// </summary>
    /// <param name="dictNum"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    int GetStartFaPaiPosByDictNumForServer(int[] dictNum, int type = 0)
    {
        int totalPoint = dictNum[0] + dictNum[1];
        int minPoint = Math.Min(dictNum[0], dictNum[1]);
        int maxPoint = Math.Min(dictNum[0], dictNum[1]);
        int dunNum = 4;
        int startPos;

        int seatIdx = totalPoint % 4 - 1;
        if (seatIdx < 0) seatIdx = 4 + seatIdx;

        int seatTotalMjCount = mjTotalCount / 4;

        if(type == 0)
        {
            startPos = seatTotalMjCount * seatIdx + minPoint * dunNum + 1;
        }
        else if(type == 1)
        {
            startPos = seatTotalMjCount * seatIdx + totalPoint * dunNum + 1;
        }
        else
        {
            startPos = seatTotalMjCount * seatIdx + maxPoint * dunNum + 1;
        }

        return startPos;
    }


    /// <summary>
    /// 根据骰子点数获取发牌在牌堆的起始位置（获取的是客户端端牌堆起始位置）
    /// </summary>
    /// <param name="clientSeat0AtServer"></param>
    /// <param name="dictNum"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    int GetStartFaPaiPosByDictNumForClient(int clientSeat0AtServer, int[] dictNum,  int type = 0)
    {
        int totalPoint = dictNum[0] + dictNum[1];
        int minPoint = Math.Min(dictNum[0], dictNum[1]);
        int maxPoint = Math.Min(dictNum[0], dictNum[1]);
        int dunNum = 4;
        int startPos;

        int seatIdx = totalPoint % 4 - 1;
        if (seatIdx < 0) seatIdx = 4 + seatIdx;

        seatIdx = TransformServerSeatToClientSeat(clientSeat0AtServer, seatIdx);

        int seatTotalMjCount = mjTotalCount / 4;

        if (type == 0)
        {
            startPos = seatTotalMjCount * seatIdx + minPoint * dunNum + 1;
        }
        else if (type == 1)
        {
            startPos = seatTotalMjCount * seatIdx + totalPoint * dunNum + 1;
        }
        else
        {
            startPos = seatTotalMjCount * seatIdx + maxPoint * dunNum + 1;
        }

        return startPos;
    }

    /// <summary>
    /// 转换服务端座位号到客户端对应座位号
    /// </summary>
    /// <param name="clientSeat0AtServer">客户端座位号在服务端对应的编号</param>
    /// <param name="serverSeat">需要转换的服务端座位号</param>
    /// <returns></returns>
    int TransformServerSeatToClientSeat(int clientSeat0AtServer, int serverSeat)
    {
        int n = serverSeat - clientSeat0AtServer;
        if (n < 0) { n = 4 + n; }
        return n;
    }

    /// <summary>
    /// 转换客户端座位号到服务端对应的座位号
    /// </summary>
    /// <param name="clientSeat0AtServer">客户端座位号在服务端对应的编号</param>
    /// <param name="clientSeat">需要转换的客户端座位号</param>
    /// <returns></returns>
    int TransformClientSeatToServerSeat(int clientSeat0AtServer, int clientSeat)
    {
        int n = clientSeat + clientSeat0AtServer;
        if (n >= 4) { n = n - 4; }
        return n;
    }


    /// <summary>
    /// 从牌堆按逆序获取一张牌
    /// </summary>
    /// <returns></returns>
    MahjongFaceValue TakePaiFromPaiDui()
    {
        if (richMjCount > 0)
        {
            if (curtPaiDuiPos == mjTotalCount)
            {
                curtPaiDuiPos = 1;
                richMjCount--;
            }
            else
            {
                curtPaiDuiPos++;
                richMjCount--;
            }

            return paiDuiCards[curtPaiDuiPos];
        }
        
        return MahjongFaceValue.MJ_UNKNOWN;
    }

    /// <summary>
    /// 从牌堆按逆序获取一墩牌
    /// </summary>
    /// <returns></returns>
    MahjongFaceValue[] TakeDunPaiFromPaiDui()
    {
        MahjongFaceValue[] dunPai = new MahjongFaceValue[4];

        for (int i=0; i<4; i++)
        {
            dunPai[i] = TakePaiFromPaiDui();
        }

        return dunPai; 
    }



    void FaPai()
    {
        curtPaiDuiPos = GetStartFaPaiPosByDictNumForServer(dictNum) - 1;

        for (int i=0; i<3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                handPais[j].AddRange(TakeDunPaiFromPaiDui());
            }
        }

        for(int i=0; i<4; i++)
            handPais[i].Add(TakePaiFromPaiDui());

        handPais[orderSeatIdx[0]].Add(TakePaiFromPaiDui());

        GetHuaInfo();
    }

    void GetHuaInfo()
    {
        List<MahjongFaceValue> handPai;
        MahjongFaceValue mjValue;

        for (int i = 0; i < 4; i++)
        {
            handPai = handPais[i];

            for (int j = 0; j < handPai.Count; j++)
            {
                if (handPai[j] >= MahjongFaceValue.MJ_HUA_CHUN &&
                    handPai[j] <= MahjongFaceValue.MJ_HUA_JU)
                {
                    huas[i].Add(handPai[j]);

                    while(true)
                    {
                        mjValue = TakePaiFromPaiDui();

                        if (mjValue >= MahjongFaceValue.MJ_HUA_CHUN &&
                            mjValue <= MahjongFaceValue.MJ_HUA_JU)
                        {
                            huas[i].Add(mjValue);
                        }
                        else
                        {
                            break;
                        }
                    }
                   
                    buPais[i].Add(mjValue);
                }
            }
        }
    }

    void BuHua()
    {
        List<MahjongFaceValue> handPai;

        for (int i = 0; i < 4; i++)
        {
            handPai = handPais[i];

            for (int j = 0; j < handPai.Count; j++)
            {
                if (handPai[j] >= MahjongFaceValue.MJ_HUA_CHUN &&
                    handPai[j] <= MahjongFaceValue.MJ_HUA_JU)
                {
                    huas[i].RemoveAt(j);
                    j = -1;
                }
            }
        }
    }
}

