using System.Collections.Generic;
using UnityEngine;

namespace MahjongMachineNS
{
    public class MahjongMachineCmdList
    {
        MahjongMachine mjMachine = null;
        LinkedList<MahjongMachineCmd> mjOpCmdList = new LinkedList<MahjongMachineCmd>();
        LinkedList<MahjongMachineCmd> delayOpCmdList = new LinkedList<MahjongMachineCmd>();

        LinkedList<MahjongMachineCmd>[] playerActionOpCmdLists = new LinkedList<MahjongMachineCmd>[4]
        {
        new LinkedList<MahjongMachineCmd>(),
        new LinkedList<MahjongMachineCmd>(),
        new LinkedList<MahjongMachineCmd>(),
        new LinkedList<MahjongMachineCmd>()
        };

        MahjongMachineCmd curtOpCmd = null;
        int blockCmdCount = 0;

        public void Init(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
        }


        public void ReleaseCmd(MahjongMachineCmd cmd)
        {
            CmdPool.Instance.ReleaseCmd(cmd);
        }

        public LinkedListNode<MahjongMachineCmd> CreateCmdNode(MahjongMachineCmd mjOpCmd)
        {
            return CmdPool.Instance.CreateCmdNode(mjOpCmd);
        }

        public void ReleaseCmdNode(LinkedListNode<MahjongMachineCmd> cmdNode)
        {
            CmdPool.Instance.ReleaseCmdNode(cmdNode);
        }


        public LinkedList<MahjongMachineCmd> CreateCmdLinkedList()
        {
            return CmdPool.Instance.CreateCmdLinkedList();
        }

        public void ReleaseCmdLinkedList(LinkedList<MahjongMachineCmd> list)
        {
            CmdPool.Instance.ReleaseCmdLikedList(list);
        }

        public void Append(MahjongMachineCmd mjOpCmd)
        {
            var node = CreateCmdNode(mjOpCmd);
            mjOpCmdList.AddLast(node);
        }

        public void Reset()
        {
            curtOpCmd = null;
            blockCmdCount = 0;

            mjOpCmdList.Clear();
            delayOpCmdList.Clear();

            for (int i = 0; i < playerActionOpCmdLists.Length; i++)
                playerActionOpCmdLists[i].Clear();
        }

        public LinkedList<MahjongMachineCmd> GetPlayerActionOpCmdList(int seatIdx)
        {
            return playerActionOpCmdLists[seatIdx];
        }

        public int GetPlayerActionOpCmdCount(int seatIdx)
        {
            return playerActionOpCmdLists[seatIdx].Count;
        }

        public void RemoveHandActionOpCmd(int seatIdx, LinkedListNode<MahjongMachineCmd> opCmdNode)
        {
            if (opCmdNode != null)
            {
                if (opCmdNode.Value.isBlock)
                    blockCmdCount--;

                playerActionOpCmdLists[seatIdx].Remove(opCmdNode);
                ReleaseCmd(opCmdNode.Value);
                ReleaseCmdNode(opCmdNode);
            }

            CmdOp(playerActionOpCmdLists[seatIdx].First);
        }


        public void RemoveCommonActionOpCmd(LinkedListNode<MahjongMachineCmd> opCmdNode)
        {
            if (opCmdNode != null)
            {
                if (opCmdNode.Value.isBlock)
                    blockCmdCount--;

                ReleaseCmd(opCmdNode.Value);
                ReleaseCmdNode(opCmdNode);
            }
        }

        void CmdOp(LinkedListNode<MahjongMachineCmd> opCmdNode, MahjongMachineCmd mjOpCmd = null)
        {
            if (opCmdNode == null && mjOpCmd == null)
                return;

            MahjongMachineCmd opCmd = mjOpCmd;

            if (opCmdNode != null)
            {
                opCmd = opCmdNode.Value;

                if (opCmd.isBlock)
                    blockCmdCount++;
            }

            if (opCmd.delayOpCmdLinkedList != null && opCmd.delayOpCmdLinkedList.Count > 0)
            {
                for (LinkedListNode<MahjongMachineCmd> node = opCmd.delayOpCmdLinkedList.First; node != null; node = opCmd.delayOpCmdLinkedList.First)
                {
                    opCmd.delayOpCmdLinkedList.RemoveFirst();
                    AppendCmdToDelayCmdList(node);
                }

                opCmd.delayOpCmdLinkedList.Clear();
            }

            switch (opCmd.opCode)
            {
                case MahjongOpCode.PlayEffectAudio:
                    {
                        PlayEffectAudioOpCmd cmd = (PlayEffectAudioOpCmd)opCmd;
                        mjMachine.PlayEffectAudio(cmd.audioIdx, cmd.numIdx);
                        RemoveCommonActionOpCmd(opCmdNode);
                    }
                    break;

                case MahjongOpCode.XiPai:
                    {
                        XiPaiCmd cmd = (XiPaiCmd)opCmd;
                        mjMachine.XiPai(cmd.dealerSeatIdx, cmd.fengWei, opCmdNode);
                    }
                    break;


                case MahjongOpCode.FaPai:
                    {
                        FaPaiCmd cmd = (FaPaiCmd)opCmd;
                        mjMachine.FaPai(cmd.startPaiIdx, cmd.mjHandSelfPaiFaceValueList, cmd.selfHuaList, cmd.selfBuPaiList, opCmdNode);
                    }
                    break;

                case MahjongOpCode.TurnNextPlayer:
                    {
                        TurnNextPlayerOpCmd cmd = (TurnNextPlayerOpCmd)opCmd;

                        FengWei fw = mjMachine.GetSeatFengWei(cmd.turnToPlayerSeatIdx);
                        mjMachine.diceMachine.OnFengWei(fw);
                        mjMachine.diceMachine.SetLimitTime(cmd.time);
                        mjMachine.diceMachine.StartTime();
                        RemoveCommonActionOpCmd(opCmdNode);
                    }
                    break;

                case MahjongOpCode.ShowScore:
                    {
                        ShowScoreCmd cmd = (ShowScoreCmd)opCmd;
                        mjMachine.uiScore.Show(cmd.seatScores);
                        RemoveCommonActionOpCmd(opCmdNode);
                    }
                    break;

                case MahjongOpCode.HideSwapPaiUI:
                    {
                        mjMachine.uiSelectSwapHandPai.CompleteSwapPaiSelected();
                        RemoveCommonActionOpCmd(opCmdNode);
                    }
                    break;

                case MahjongOpCode.QueMen:
                    {
                        QueMenCmd cmd = (QueMenCmd)opCmd;
                        mjMachine.uiSelectQueMen.AppendPlayQueMenForSeatToList(cmd.seatIdx, cmd.queMenHuaSe);
                        RemoveCommonActionOpCmd(opCmdNode);
                    }
                    break;


                case MahjongOpCode.ReqSelectSwapPai:
                    {
                        mjMachine.SelectSwapPai(opCmdNode);
                    }
                    break;

                case MahjongOpCode.ReqSelectQueMen:
                    {
                        ReqSelectQueMenOpCmd cmd = (ReqSelectQueMenOpCmd)opCmd;
                        mjMachine.SelectQueMen(cmd.defaultQueMenHuaSe, opCmdNode);
                    }
                    break;

                case MahjongOpCode.ReqSelectDaPai:
                    {
                        ReqSelectDaPaiOpCmd cmd = (ReqSelectDaPaiOpCmd)opCmd;

                        mjMachine.SelectDaPai(
                            cmd.huPaiInHandPaiIdxs, cmd.huPaiInfosInHandPai,
                            cmd.huPaiInMoPaiIdxs, cmd.huPaiInfosInMoPai, opCmdNode);
                    }
                    break;

                case MahjongOpCode.ReqSelectPCGTHPai:
                    {
                        ReqSelectPCGTHPaiOpCmd cmd = (ReqSelectPCGTHPaiOpCmd)opCmd;

                        mjMachine.SelectPCGTHPai(cmd.pcgthBtnTypes,
                            cmd.chiPaiMjValueList, cmd.tingPaiInHandPaiIdxs,
                            cmd.tingPaiInfosInHandPai, cmd.tingPaiInMoPaiIdxs, cmd.tingPaiInfosInMoPai, opCmdNode);
                    }
                    break;

                case MahjongOpCode.QiDongDiceMachine:
                    {
                        QiDongDiceMachineCmd cmd = (QiDongDiceMachineCmd)opCmd;
                        mjMachine.QiDongDiceMachine(cmd.seatIdx, cmd.dice1Point, cmd.dice2Point, opCmdNode);
                    }
                    break;

                case MahjongOpCode.ShowSwapPaiHint:
                    {
                        ShowSwapPaiHintCmd cmd = (ShowSwapPaiHintCmd)opCmd;
                        mjMachine.ShowSwapPaiHint(cmd.swapPaiDirection);
                        RemoveCommonActionOpCmd(opCmdNode);
                    }
                    break;

                case MahjongOpCode.SwapPai:
                    {
                        MahjongSwapPaiCmd cmd = (MahjongSwapPaiCmd)opCmd;
                        mjMachine.SwapPai(
                            cmd.fromSeatIdx, cmd.toSeatIdx, cmd.swapMjCount,
                            cmd.toSeatHandPaiIdx,
                            cmd.mjFaceValues, cmd.fromSeatHandPaiIdx,
                            cmd.mjMoPaiFaceValues, cmd.fromSeatMoPaiIdx,
                            cmd.isShowBack, cmd.swapDir, opCmdNode);
                    }
                    break;



                case MahjongOpCode.MoPai:
                    {
                        MahjongMoPaiOpCmd cmd = (MahjongMoPaiOpCmd)opCmd;
                        mjMachine.MoPai(cmd.seatIdx, cmd.mjFaceValue, opCmdNode);
                    }
                    break;

                case MahjongOpCode.DaPai:
                    {
                        MahjongDaPaiOpCmd cmd = (MahjongDaPaiOpCmd)opCmd;
                        ActionCombineNum actionCombineNum = mjMachine.GetRandomHandDaPaiActionNumForNextDeskMjPos(cmd.seatIdx);
                        mjMachine.DaPai(cmd.seatIdx, cmd.handStyle, cmd.paiIdx, cmd.paiType, cmd.mjFaceValue, cmd.isJiaoTing, actionCombineNum, opCmdNode);
                    }
                    break;

                case MahjongOpCode.ChaPai:
                    {
                        MahjongChaPaiOpCmd cmd = (MahjongChaPaiOpCmd)opCmd;
                        mjMachine.ChaPai(cmd.seatIdx, cmd.handStyle, cmd.orgPaiIdx, cmd.dstHandPaiIdx, cmd.orgPaiType, cmd.adjustDirection, opCmdNode);
                    }
                    break;

                case MahjongOpCode.SortPai:
                    {
                        MahjongPaiOpCmd cmd = (MahjongPaiOpCmd)opCmd;
                        mjMachine.SortPai(cmd.seatIdx, SortPaiType.LEFT, opCmdNode);
                    }
                    break;

                case MahjongOpCode.BuHuaPai:
                    {
                        MahjongBuHuaPaiOpCmd cmd = (MahjongBuHuaPaiOpCmd)opCmd;
                        mjMachine.BuHua(cmd.seatIdx, cmd.handStyle, cmd.buHuaPaiFaceValue, ActionCombineNum.HuPai, opCmdNode);
                    }
                    break;

                case MahjongOpCode.HuPai:
                    {
                        MahjongHuPaiOpCmd cmd = (MahjongHuPaiOpCmd)opCmd;
                        mjMachine.HuPai(cmd.seatIdx, cmd.handStyle, cmd.huTargetSeatIdx, cmd.huTargetMjIdx, cmd.huPaiFaceValue, ActionCombineNum.HuPai, opCmdNode);
                    }
                    break;

                case MahjongOpCode.PengChiGangPai:
                    {
                        MahjongPcgPaiOpCmd cmd = (MahjongPcgPaiOpCmd)opCmd;
                        mjMachine.PengChiGangPai(
                            cmd.seatIdx, cmd.handStyle, cmd.isMoveHand, cmd.moveHandDist,
                            cmd.faceValues, cmd.pcgType, cmd.pcgTargetSeatIdx, cmd.pcgTargetMjIdx, cmd.fengRainEffectEtcType,
                            ActionCombineNum.PengPai, opCmdNode);
                    }
                    break;

                case MahjongOpCode.TuiDaoPai:
                    {
                        MahjongTuiDaoPaiOpCmd cmd = (MahjongTuiDaoPaiOpCmd)opCmd;
                        mjMachine.TuiDaoPai(cmd.seatIdx, cmd.handStyle, cmd.handPaiValueList, ActionCombineNum.TuiDaoPai, opCmdNode);
                    }
                    break;
            }
        }



        public void AppendCmdToDelayCmdList(MahjongMachineCmd cmd)
        {
            var cmdNode = CreateCmdNode(cmd);
            cmd.delayExecuteTime += Time.time;
            delayOpCmdList.AddLast(cmdNode);
        }

        public void AppendCmdToDelayCmdList(LinkedListNode<MahjongMachineCmd> cmdNode)
        {
            cmdNode.Value.delayExecuteTime += Time.time;
            delayOpCmdList.AddLast(cmdNode);
        }

        public void Update()
        {
            if (blockCmdCount > 0)
                return;

            if (curtOpCmd != null)
            {
                switch (curtOpCmd.opCode)
                {
                    case MahjongOpCode.TurnNextPlayer:
                        {
                            if (CanTurnNextPlayer((TurnNextPlayerOpCmd)curtOpCmd))
                            {
                                CmdOp(null, curtOpCmd);
                                curtOpCmd = null;
                            }
                            else
                            {
                                return;
                            }
                        }
                        break;
                }
            }

            bool isBlock = false;
            for (LinkedListNode<MahjongMachineCmd> opCmdNode = mjOpCmdList.First; opCmdNode != null; opCmdNode = mjOpCmdList.First)
            {
                mjOpCmdList.RemoveFirst();

                if (opCmdNode.Value.delayExecuteTime > 0)
                {
                    AppendCmdToDelayCmdList(opCmdNode);
                    continue;
                }

                isBlock = opCmdNode.Value.isBlock;

                if (ProcessCmdNode(opCmdNode) == false)
                    return;

                if (isBlock == true)
                    break;
            }

            UpdateDelayExecuteOpCmd();
        }


        /// <summary>
        /// 更新延迟执行操作命令
        /// </summary>
        public void UpdateDelayExecuteOpCmd()
        {
            MahjongMachineCmd delayOpCmd;
            LinkedListNode<MahjongMachineCmd> nextNode;

            for (LinkedListNode<MahjongMachineCmd> opCmdNode = delayOpCmdList.First; opCmdNode != null;)
            {
                delayOpCmd = opCmdNode.Value;

                if (Time.time >= delayOpCmd.delayExecuteTime)
                {
                    nextNode = opCmdNode.Next;
                    delayOpCmdList.Remove(opCmdNode);
                    ProcessCmdNode(opCmdNode);
                    opCmdNode = nextNode;
                }
                else
                {
                    opCmdNode = opCmdNode.Next;
                }
            }
        }

        bool ProcessCmdNode(LinkedListNode<MahjongMachineCmd> opCmdNode)
        {
            curtOpCmd = opCmdNode.Value;

            switch (curtOpCmd.opCode)
            {
                case MahjongOpCode.TurnNextPlayer:
                    {
                        if (CanTurnNextPlayer((TurnNextPlayerOpCmd)curtOpCmd))
                        {
                            CmdOp(null, curtOpCmd);
                            curtOpCmd = null;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;

                case MahjongOpCode.ReqSelectSwapPai:
                case MahjongOpCode.ReqSelectDaPai:
                case MahjongOpCode.ReqSelectPCGTHPai:
                case MahjongOpCode.ReqSelectQueMen:
                    {
                        playerActionOpCmdLists[0].AddLast(opCmdNode);
                        curtOpCmd = null;

                        if (playerActionOpCmdLists[0].Count == 1)
                        {
                            CmdOp(playerActionOpCmdLists[0].First);
                        }
                    }
                    break;

                case MahjongOpCode.MoPai:
                case MahjongOpCode.DaPai:
                case MahjongOpCode.ChaPai:
                case MahjongOpCode.SortPai:
                case MahjongOpCode.BuHuaPai:
                case MahjongOpCode.HuPai:
                case MahjongOpCode.PengChiGangPai:
                case MahjongOpCode.TuiDaoPai:
                    {
                        MahjongPaiOpCmd cmd = (MahjongPaiOpCmd)curtOpCmd;
                        playerActionOpCmdLists[cmd.seatIdx].AddLast(opCmdNode);
                        curtOpCmd = null;

                        if (playerActionOpCmdLists[cmd.seatIdx].Count == 1)
                        {
                            CmdOp(playerActionOpCmdLists[cmd.seatIdx].First);
                        }
                    }
                    break;


                case MahjongOpCode.QiDongDiceMachine:
                    {
                        QiDongDiceMachineCmd cmd = (QiDongDiceMachineCmd)curtOpCmd;
                        playerActionOpCmdLists[cmd.seatIdx].AddLast(opCmdNode);
                        curtOpCmd = null;

                        if (playerActionOpCmdLists[cmd.seatIdx].Count == 1)
                        {
                            CmdOp(playerActionOpCmdLists[cmd.seatIdx].First);
                        }
                    }
                    break;

                case MahjongOpCode.PlayEffectAudio:
                case MahjongOpCode.XiPai:
                case MahjongOpCode.FaPai:
                case MahjongOpCode.ShowScore:
                case MahjongOpCode.ShowSwapPaiHint:
                case MahjongOpCode.HideSwapPaiUI:
                    {
                        CmdOp(opCmdNode);
                        curtOpCmd = null;
                    }
                    break;

                case MahjongOpCode.SwapPai:
                    {
                        MahjongSwapPaiCmd cmd = (MahjongSwapPaiCmd)curtOpCmd;
                        playerActionOpCmdLists[cmd.fromSeatIdx].AddLast(opCmdNode);
                        curtOpCmd = null;

                        if (playerActionOpCmdLists[cmd.fromSeatIdx].Count == 1)
                        {
                            CmdOp(playerActionOpCmdLists[cmd.fromSeatIdx].First);
                        }
                    }
                    break;

                case MahjongOpCode.QueMen:
                    {
                        CmdOp(opCmdNode);
                        curtOpCmd = null;
                    }
                    break;


                case MahjongOpCode.SwapPaiGroup:
                    {
                        MahjongSwapPaiGroupCmd cmd = (MahjongSwapPaiGroupCmd)curtOpCmd;
                        curtOpCmd = null;

                        for (int i = 0; i < 4; i++)
                        {
                            cmd.cmdSeats[i].isBlock = cmd.isBlock;
                            cmd.cmdSeats[i].swapMjCount = cmd.SwapMjCount;
                            playerActionOpCmdLists[i].AddLast(CreateCmdNode(cmd.cmdSeats[i]));

                            if (playerActionOpCmdLists[i].Count == 1)
                            {
                                CmdOp(playerActionOpCmdLists[i].First);
                            }
                        }

                        ReleaseCmd(cmd);
                        ReleaseCmdNode(opCmdNode);
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// 判断是否可以轮转到下一个玩家
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        bool CanTurnNextPlayer(TurnNextPlayerOpCmd cmd)
        {
            if (playerActionOpCmdLists[cmd.turnToPlayerSeatIdx].Count != 0)
                return false;

            LinkedList<MahjongMachineCmd> cmdList;
            int waitActionEndSeatIdx;
            for (int i = 0; i < cmd.waitActionEndPlayerSeatIdxs.Length; i++)
            {
                waitActionEndSeatIdx = cmd.waitActionEndPlayerSeatIdxs[i];
                cmdList = playerActionOpCmdLists[waitActionEndSeatIdx];

                if (cmdList.Count != 0)
                {
                    for (LinkedListNode<MahjongMachineCmd> node = cmdList.First; node != null; node = node.Next)
                    {
                        if (node.Value.opCode == MahjongOpCode.DaPai &&
                                mjMachine.playerStateData[waitActionEndSeatIdx].playerHandActionState < HandActionState.DA_PAI_CHUPAI_TAIHAND)
                            return false;
                    }
                }
            }

            return true;
        }

    }
}