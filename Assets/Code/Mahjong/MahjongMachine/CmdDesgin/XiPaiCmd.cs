using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 洗牌
    /// </summary>
    public class XiPaiCmd : MahjongMachineCmd
    {
        public int dealerSeatIdx;
        public FengWei fengWei;
        public XiPaiCmd()
        {
            isBlock = false;
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            XiPaiAction.Instance.XiPai(dealerSeatIdx, fengWei, opCmdNode);
        }
    }

}
