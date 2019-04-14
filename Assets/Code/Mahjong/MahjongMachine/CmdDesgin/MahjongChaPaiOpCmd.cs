using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 插牌命令
    /// </summary>
    public class MahjongChaPaiOpCmd : BaseHandActionCmd
    {
        public int orgPaiIdx;
        public int dstHandPaiIdx;
        public HandPaiType orgPaiType;
        public HandPaiAdjustDirection adjustDirection;

        public MahjongChaPaiOpCmd()
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            ChaPaiAction.Instance.ChaPai(seatIdx, handStyle, orgPaiIdx, dstHandPaiIdx, orgPaiType, adjustDirection, opCmdNode);
        }
    }
}
