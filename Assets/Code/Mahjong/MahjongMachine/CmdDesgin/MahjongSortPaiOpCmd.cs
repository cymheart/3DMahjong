using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 整理牌操作
    /// </summary>
    public class MahjongSortPaiOpCmd : BaseHandActionCmd
    {
        public MahjongSortPaiOpCmd()
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            SortPaiAction.Instance.SortPai(seatIdx, SortPaiType.LEFT, opCmdNode);
        }
    }
}
