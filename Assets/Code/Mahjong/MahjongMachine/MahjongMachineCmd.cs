
using System;
using System.Collections.Generic;
using UnityEngine;
using Task;

namespace MahjongMachineNS
{
    public class MahjongMachineCmd : TaskDataBase
    {
        public MahjongOpCode opCode;

        /// <summary>
        /// 自身相对父节点命令延迟执行时间
        /// </summary>
        public float delayExecuteTime = 0;

        /// <summary>
        /// 是否空闲可以再次利用
        /// </summary>
        public bool isFree = true;

        /// <summary>
        /// 是否是阻塞命令
        /// 当前命令的阻塞开启:意味着所有在这个命令之后添加的命令必须等待这条阻塞命令执行完成才能执行
        /// </summary>
        public bool isBlock = false;

        /// <summary>
        /// 是否在命令执行结束之后可以点击选择查看手牌
        /// </summary>
        public bool canSelectPaiAfterCmdEnd = false;


        public LinkedList<MahjongMachineCmd> delayOpCmdLinkedList = new LinkedList<MahjongMachineCmd>();

        public void Append(LinkedListNode<MahjongMachineCmd> cmdNode)
        {
            delayOpCmdLinkedList.AddLast(cmdNode);
        }

        public void ClearChildCmdList()
        {
            if (delayOpCmdLinkedList != null && delayOpCmdLinkedList.Count > 0)
                delayOpCmdLinkedList.Clear();
        }

    }

    public class PlayEffectAudioOpCmd : MahjongMachineCmd
    {
        public AudioIdx audioIdx = AudioIdx.AUDIO_EFFECT_DEAL;
        public int numIdx = 0;
        public PlayEffectAudioOpCmd()
        {
            opCode = MahjongOpCode.PlayEffectAudio;

        }
    }

    public class XiPaiCmd : MahjongMachineCmd
    {
        public int dealerSeatIdx;
        public FengWei fengWei;
        public XiPaiCmd()
        {
            isBlock = false;
            opCode = MahjongOpCode.XiPai;
        }
    }

    public class FaPaiCmd : MahjongMachineCmd
    {
        public int startPaiIdx = 0;
        public List<MahjongFaceValue> mjHandSelfPaiFaceValueList = new List<MahjongFaceValue>();
        public List<MahjongFaceValue> selfHuaList = new List<MahjongFaceValue>();
        public List<MahjongFaceValue> selfBuPaiList = new List<MahjongFaceValue>();
        public FaPaiCmd()
        {
            opCode = MahjongOpCode.FaPai;
        }
    }


    public class TurnNextPlayerOpCmd : MahjongMachineCmd
    {
        public int[] waitActionEndPlayerSeatIdxs;
        public int turnToPlayerSeatIdx;
        public int time;

        public TurnNextPlayerOpCmd()
        {
            opCode = MahjongOpCode.TurnNextPlayer;
        }
    }


    public class QiDongDiceMachineCmd : MahjongMachineCmd
    {
        public int seatIdx;
        public int dice1Point = -1;
        public int dice2Point = -1;

        public QiDongDiceMachineCmd()
        {
            opCode = MahjongOpCode.QiDongDiceMachine;
        }
    }

    public class ShowSwapPaiHintCmd : MahjongMachineCmd
    {
        public SwapPaiDirection swapPaiDirection = SwapPaiDirection.CLOCKWISE;
        public ShowSwapPaiHintCmd()
        {
            opCode = MahjongOpCode.ShowSwapPaiHint;
        }
    }

    public class QueMenCmd : MahjongMachineCmd
    {
        public int seatIdx;
        public MahjongHuaSe queMenHuaSe;
        public QueMenCmd()
        {
            opCode = MahjongOpCode.QueMen;
        }
    }

    public class MahjongSwapPaiGroupCmd : MahjongMachineCmd
    {
        public MahjongSwapPaiCmd[] cmdSeats = new MahjongSwapPaiCmd[4];

        public int[] SwapHandPaiIdx
        {
            set
            {
                cmdSeats[0].fromSeatHandPaiIdx = value;
            }
        }

        public int[] SwapMoPaiIdx
        {
            set
            {
                cmdSeats[0].fromSeatMoPaiIdx = value;
            }
        }

        public MahjongFaceValue[] TakeMjFaceValues
        {
            set
            {
                takeMjFaceValues = value;

                if (swapDir == SwapPaiDirection.OPPOSITE)
                {
                    cmdSeats[2].mjFaceValues = takeMjFaceValues;
                    swapMjCount = cmdSeats[2].mjFaceValues.Length;
                }
                else if (swapDir == SwapPaiDirection.CLOCKWISE)
                {
                    cmdSeats[3].mjFaceValues = takeMjFaceValues;
                    swapMjCount = cmdSeats[3].mjFaceValues.Length;
                }
                else
                {
                    cmdSeats[1].mjFaceValues = takeMjFaceValues;
                    swapMjCount = cmdSeats[1].mjFaceValues.Length;
                }
            }
        }

        public int SwapMjCount
        {
            get
            {
                return swapMjCount;
            }
        }

        public SwapPaiDirection SwapDirection
        {
            set
            {
                swapDir = value;

                if (takeMjFaceValues != null)
                    TakeMjFaceValues = takeMjFaceValues;

                for (int i = 0; i < 4; i++)
                {
                    cmdSeats[i].swapDir = swapDir;
                }

                switch (swapDir)
                {
                    case SwapPaiDirection.CLOCKWISE:
                        cmdSeats[0].fromSeatIdx = 0;
                        cmdSeats[0].toSeatIdx = 1;

                        cmdSeats[1].fromSeatIdx = 1;
                        cmdSeats[1].toSeatIdx = 2;

                        cmdSeats[2].fromSeatIdx = 2;
                        cmdSeats[2].toSeatIdx = 3;

                        cmdSeats[3].fromSeatIdx = 3;
                        cmdSeats[3].toSeatIdx = 0;
                        break;

                    case SwapPaiDirection.ANTICLOCKWISE:
                        cmdSeats[0].fromSeatIdx = 0;
                        cmdSeats[0].toSeatIdx = 3;

                        cmdSeats[3].fromSeatIdx = 3;
                        cmdSeats[3].toSeatIdx = 2;

                        cmdSeats[2].fromSeatIdx = 2;
                        cmdSeats[2].toSeatIdx = 1;

                        cmdSeats[1].fromSeatIdx = 1;
                        cmdSeats[1].toSeatIdx = 0;
                        break;


                    case SwapPaiDirection.OPPOSITE:
                        cmdSeats[0].fromSeatIdx = 0;
                        cmdSeats[0].toSeatIdx = 2;

                        cmdSeats[3].fromSeatIdx = 3;
                        cmdSeats[3].toSeatIdx = 1;

                        cmdSeats[2].fromSeatIdx = 2;
                        cmdSeats[2].toSeatIdx = 0;

                        cmdSeats[1].fromSeatIdx = 1;
                        cmdSeats[1].toSeatIdx = 3;
                        break;
                }

            }
        }

        MahjongFaceValue[] takeMjFaceValues = null;
        int swapMjCount = 0;
        SwapPaiDirection swapDir = SwapPaiDirection.CLOCKWISE;

        public MahjongSwapPaiGroupCmd()
        {
            opCode = MahjongOpCode.SwapPaiGroup;
        }
    }

    public class MahjongSwapPaiCmd : MahjongMachineCmd
    {
        public int fromSeatIdx;
        public int toSeatIdx;
        public int swapMjCount;
        public MahjongFaceValue[] mjFaceValues = null;
        public int[] fromSeatHandPaiIdx = null;
        public MahjongFaceValue[] mjMoPaiFaceValues = null;
        public int[] fromSeatMoPaiIdx = null;
        public int[] toSeatHandPaiIdx = null;
        public bool isShowBack = true;
        public SwapPaiDirection swapDir = SwapPaiDirection.CLOCKWISE;
        public MahjongSwapPaiCmd()
        {
            opCode = MahjongOpCode.SwapPai;
        }
    }

    public class ReqSelectSwapPaiOpCmd : MahjongMachineCmd
    {
        public int seatIdx;

        public ReqSelectSwapPaiOpCmd()
        {
            opCode = MahjongOpCode.ReqSelectSwapPai;
        }
    }

    public class ReqSelectQueMenOpCmd : MahjongMachineCmd
    {
        public int seatIdx = 0;
        public MahjongHuaSe defaultQueMenHuaSe = MahjongHuaSe.TIAO;

        public ReqSelectQueMenOpCmd()
        {
            opCode = MahjongOpCode.ReqSelectQueMen;
        }
    }


    public class ReqSelectDaPaiOpCmd : MahjongMachineCmd
    {
        public int seatIdx;
        public int[] huPaiInHandPaiIdxs = null;
        public List<HuPaiTipsInfo[]> huPaiInfosInHandPai = null;
        public int[] huPaiInMoPaiIdxs = null;
        public List<HuPaiTipsInfo[]> huPaiInfosInMoPai = null;
        public ReqSelectDaPaiOpCmd()
        {
            opCode = MahjongOpCode.ReqSelectDaPai;
        }
    }


    public class ReqSelectPCGTHPaiOpCmd : MahjongMachineCmd
    {
        public int seatIdx;
        public PengChiGangTingHuType[] pcgthBtnTypes;
        public List<MahjongFaceValue[]> chiPaiMjValueList;
        public int[] tingPaiInHandPaiIdxs = null;
        public List<HuPaiTipsInfo[]> tingPaiInfosInHandPai = null;
        public int[] tingPaiInMoPaiIdxs = null;
        public List<HuPaiTipsInfo[]> tingPaiInfosInMoPai = null;
        public ReqSelectPCGTHPaiOpCmd()
        {
            opCode = MahjongOpCode.ReqSelectPCGTHPai;
        }
    }



    public class MahjongPaiOpCmd : MahjongMachineCmd
    {
        public int seatIdx;
        public PlayerType handStyle;
    }


    /// <summary>
    /// 摸牌操作
    /// </summary>
    public class MahjongMoPaiOpCmd : MahjongPaiOpCmd
    {
        public MahjongFaceValue mjFaceValue;

        public MahjongMoPaiOpCmd()
        {
            opCode = MahjongOpCode.MoPai;
        }
    }

    /// <summary>
    /// 打牌操作
    /// </summary>
    public class MahjongDaPaiOpCmd : MahjongPaiOpCmd
    {
        public int paiIdx;
        public HandPaiType paiType;
        public MahjongFaceValue mjFaceValue;
        public bool isJiaoTing = false;

        public MahjongDaPaiOpCmd()
        {
            opCode = MahjongOpCode.DaPai;
        }
    }


    /// <summary>
    /// 插牌操作
    /// </summary>
    public class MahjongChaPaiOpCmd : MahjongPaiOpCmd
    {
        public int orgPaiIdx;
        public int dstHandPaiIdx;
        public HandPaiType orgPaiType;
        public HandPaiAdjustDirection adjustDirection;

        public MahjongChaPaiOpCmd()
        {
            opCode = MahjongOpCode.ChaPai;
        }
    }

    /// <summary>
    /// 补花牌操作
    /// </summary>
    public class MahjongBuHuaPaiOpCmd : MahjongPaiOpCmd
    {
        public MahjongFaceValue buHuaPaiFaceValue;

        public MahjongBuHuaPaiOpCmd()
        {
            opCode = MahjongOpCode.BuHuaPai;
        }
    }


    /// <summary>
    /// 胡牌操作
    /// </summary>
    public class MahjongHuPaiOpCmd : MahjongPaiOpCmd
    {
        public int huTargetSeatIdx;
        public Vector3Int huTargetMjIdx = new Vector3Int(-1, -1, -1);
        public MahjongFaceValue huPaiFaceValue;

        public MahjongHuPaiOpCmd()
        {
            opCode = MahjongOpCode.HuPai;
        }
    }


    /// <summary>
    /// 碰吃杠牌操作
    /// </summary>
    public class MahjongPcgPaiOpCmd : MahjongPaiOpCmd
    {
        public bool isMoveHand;
        public float moveHandDist;
        public MahjongFaceValue[] faceValues;
        public PengChiGangPaiType pcgType;
        public int pcgTargetSeatIdx;
        public Vector3Int pcgTargetMjIdx = new Vector3Int(-1, -1, -1);
        public EffectFengRainEtcType fengRainEffectEtcType;

        public MahjongPcgPaiOpCmd()
        {
            opCode = MahjongOpCode.PengChiGangPai;
        }
    }

    /// <summary>
    /// 推倒牌操作
    /// </summary>
    public class MahjongTuiDaoPaiOpCmd : MahjongPaiOpCmd
    {
        public List<MahjongFaceValue> handPaiValueList;

        public MahjongTuiDaoPaiOpCmd()
        {
            opCode = MahjongOpCode.TuiDaoPai;
        }
    }

    public class ShowScoreCmd : MahjongPaiOpCmd
    {
        public int[] seatScores = new int[4]
        {
        0,0,0,0
        };

        public ShowScoreCmd()
        {
            opCode = MahjongOpCode.ShowScore;
        }
    }


    public class CmdPool
    {
        List<MahjongMachineCmd>[] cmdLists;
        int[] cmdListIdx;

        //
        List<LinkedListNode<MahjongMachineCmd>> cmdNodeList = new List<LinkedListNode<MahjongMachineCmd>>();
        int cmdNodeListIdx;
        List<bool> isCmdNodeListFree = new List<bool>();

        //
        List<LinkedList<MahjongMachineCmd>> cmdLinkedListList = new List<LinkedList<MahjongMachineCmd>>();
        int cmdLinkedListListIdx;
        List<bool> isCmdLinkedListListFree = new List<bool>();

        private static readonly CmdPool instance = new CmdPool();
        private CmdPool() { }
        public static CmdPool Instance
        {
            get
            {
                return instance;
            }
        }

        public void CreatePool()
        {
            MahjongMachineCmd cmd;
            List<MahjongMachineCmd> cmdList;

            int count = (int)MahjongOpCode.ShowScore + 1;

            cmdLists = new List<MahjongMachineCmd>[count];
            cmdListIdx = new int[count];

            for (int i = 0; i < count; i++)
            {
                cmdList = new List<MahjongMachineCmd>();
                for (int j = 0; j < 3; j++)
                {
                    cmd = NewCmd((MahjongOpCode)i);
                    cmdList.Add(cmd);
                }

                cmdLists[i] = cmdList;
                cmdListIdx[i] = 0;
            }

            //
            cmdNodeListIdx = 0;
            LinkedListNode<MahjongMachineCmd> cmdNode;
            for (int i = 0; i < 50; i++)
            {
                cmdNode = new LinkedListNode<MahjongMachineCmd>(null);
                cmdNodeList.Add(cmdNode);
                isCmdNodeListFree.Add(true);
            }

            //
            cmdNodeListIdx = 0;
            LinkedList<MahjongMachineCmd> linkedList;
            for (int i = 0; i < 10; i++)
            {
                linkedList = new LinkedList<MahjongMachineCmd>();
                cmdLinkedListList.Add(linkedList);
                isCmdNodeListFree.Add(true);
            }
        }

        public void DestroyPool()
        {
            for (int i = 0; i <= (int)MahjongOpCode.ShowScore; i++)
            {
                cmdLists[i].Clear();
            }


            cmdNodeList.Clear();
            cmdLinkedListList.Clear();
        }

        MahjongMachineCmd NewCmd(MahjongOpCode opCode)
        {
            MahjongMachineCmd cmd = null;

            switch (opCode)
            {
                case MahjongOpCode.PlayEffectAudio: cmd = new PlayEffectAudioOpCmd(); break;
                case MahjongOpCode.XiPai: cmd = new XiPaiCmd(); break;
                case MahjongOpCode.FaPai: cmd = new FaPaiCmd(); break;
                case MahjongOpCode.TurnNextPlayer: cmd = new TurnNextPlayerOpCmd(); break;
                case MahjongOpCode.ReqSelectSwapPai: cmd = new ReqSelectSwapPaiOpCmd(); break;
                case MahjongOpCode.ReqSelectQueMen: cmd = new ReqSelectQueMenOpCmd(); break;
                case MahjongOpCode.ReqSelectDaPai: cmd = new ReqSelectDaPaiOpCmd(); break;
                case MahjongOpCode.ReqSelectPCGTHPai: cmd = new ReqSelectPCGTHPaiOpCmd(); break;
                case MahjongOpCode.ShowSwapPaiHint: cmd = new ShowSwapPaiHintCmd(); break;
                case MahjongOpCode.HideSwapPaiUI: cmd = new MahjongMachineCmd(); cmd.opCode = MahjongOpCode.HideSwapPaiUI; break;
                case MahjongOpCode.QiDongDiceMachine: cmd = new QiDongDiceMachineCmd(); break;
                case MahjongOpCode.SwapPaiGroup: cmd = new MahjongSwapPaiGroupCmd(); break;
                case MahjongOpCode.SwapPai: cmd = new MahjongSwapPaiCmd(); break;
                case MahjongOpCode.QueMen: cmd = new QueMenCmd(); break;
                case MahjongOpCode.MoPai: cmd = new MahjongMoPaiOpCmd(); break;
                case MahjongOpCode.DaPai: cmd = new MahjongDaPaiOpCmd(); break;
                case MahjongOpCode.ChaPai: cmd = new MahjongChaPaiOpCmd(); break;
                case MahjongOpCode.SortPai: cmd = new MahjongPaiOpCmd(); cmd.opCode = MahjongOpCode.SortPai; break;
                case MahjongOpCode.BuHuaPai: cmd = new MahjongBuHuaPaiOpCmd(); break;
                case MahjongOpCode.HuPai: cmd = new MahjongHuPaiOpCmd(); break;
                case MahjongOpCode.PengChiGangPai: cmd = new MahjongPcgPaiOpCmd(); break;
                case MahjongOpCode.TuiDaoPai: cmd = new MahjongTuiDaoPaiOpCmd(); break;
                case MahjongOpCode.ShowScore: cmd = new ShowScoreCmd(); break;
            }

            return cmd;
        }

        public MahjongMachineCmd CreateCmd(MahjongOpCode opCode)
        {
            List<MahjongMachineCmd> cmdList = cmdLists[(int)opCode];

            for (int i = 0; i < cmdList.Count; ++i)
            {
                int temI = (cmdListIdx[(int)opCode] + i) % cmdList.Count;
                if (cmdList[temI].isFree)
                {
                    ProcessCmdType(opCode, cmdList[temI]);
                    cmdListIdx[(int)opCode] = (temI + 1) % cmdList.Count;
                    cmdList[temI].isFree = false;
                    return cmdList[temI];
                }
            }

            MahjongMachineCmd cmd = NewCmd(opCode);
            ProcessCmdType(opCode, cmd);
            cmd.isFree = false;
            cmdList.Add(cmd);
            return cmd;
        }

        void ProcessCmdType(MahjongOpCode opCode, MahjongMachineCmd cmd)
        {
            switch (opCode)
            {
                case MahjongOpCode.SwapPaiGroup:
                    {
                        MahjongSwapPaiCmd swapPaiCmd;
                        MahjongSwapPaiGroupCmd swapPaiGroupCmd = (MahjongSwapPaiGroupCmd)cmd;

                        for (int j = 0; j < 4; j++)
                        {
                            swapPaiCmd = (MahjongSwapPaiCmd)CreateCmd(MahjongOpCode.SwapPai);
                            swapPaiGroupCmd.cmdSeats[j] = swapPaiCmd;
                        }
                    }
                    break;

                case MahjongOpCode.FaPai:
                    {
                        FaPaiCmd faPaiCmd = (FaPaiCmd)cmd;
                        faPaiCmd.mjHandSelfPaiFaceValueList.Clear();
                        faPaiCmd.selfHuaList.Clear();
                        faPaiCmd.selfBuPaiList.Clear();
                    }
                    break;
            }

        }


        public void ReleaseCmd(MahjongMachineCmd cmd)
        {
            if (cmd == null)
                return;

            cmd.isFree = true;
            cmd.ClearChildCmdList();
        }

        public LinkedListNode<MahjongMachineCmd> CreateCmdNode(MahjongMachineCmd cmd)
        {
            for (int i = 0; i < cmdNodeList.Count; ++i)
            {
                int temI = (cmdNodeListIdx + i) % cmdNodeList.Count;
                if (isCmdNodeListFree[temI])
                {
                    cmdNodeListIdx = (temI + 1) % cmdNodeList.Count;
                    isCmdNodeListFree[temI] = false;
                    cmdNodeList[temI].Value = cmd;
                    return cmdNodeList[temI];
                }
            }

            LinkedListNode<MahjongMachineCmd> node = new LinkedListNode<MahjongMachineCmd>(cmd);
            cmdNodeList.Add(node);
            isCmdNodeListFree.Add(false);
            return node;
        }

        public void ReleaseCmdNode(LinkedListNode<MahjongMachineCmd> node)
        {
            if (node == null)
                return;

            node.Value = null;
            int idx = cmdNodeList.IndexOf(node);
            isCmdNodeListFree[idx] = true;
        }


        public LinkedList<MahjongMachineCmd> CreateCmdLinkedList()
        {
            for (int i = 0; i < cmdLinkedListList.Count; ++i)
            {
                int temI = (cmdLinkedListListIdx + i) % cmdLinkedListList.Count;
                if (isCmdLinkedListListFree[temI])
                {
                    cmdLinkedListListIdx = (temI + 1) % cmdLinkedListList.Count;
                    isCmdLinkedListListFree[temI] = false;
                    return cmdLinkedListList[temI];
                }
            }

            LinkedList<MahjongMachineCmd> list = new LinkedList<MahjongMachineCmd>();
            cmdLinkedListList.Add(list);
            isCmdLinkedListListFree.Add(false);
            return list;
        }

        public void ReleaseCmdLikedList(LinkedList<MahjongMachineCmd> list)
        {
            int idx = cmdLinkedListList.IndexOf(list);
            isCmdLinkedListListFree[idx] = true;
        }

    }
}