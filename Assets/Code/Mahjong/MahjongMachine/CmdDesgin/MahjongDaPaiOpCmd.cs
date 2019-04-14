using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 打牌命令
    /// </summary>
    public class MahjongDaPaiOpCmd : BaseHandActionCmd
    {
        public int paiIdx;
        public HandPaiType paiType;
        public MahjongFaceValue mjFaceValue;
        public bool isJiaoTing = false;

        public MahjongDaPaiOpCmd()
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            ActionCombineNum actionCombineNum = mjMachine.GetComponent<Hand>().GetRandomHandDaPaiActionNumForNextDeskMjPos(seatIdx);
            DaPaiAction.Instance.DaPai(seatIdx, handStyle, paiIdx, paiType, mjFaceValue, isJiaoTing, actionCombineNum, opCmdNode);
        }
    }
}
