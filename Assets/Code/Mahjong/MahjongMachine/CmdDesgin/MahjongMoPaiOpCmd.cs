using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 摸牌操作
    /// </summary>
    public class MahjongMoPaiOpCmd : BaseHandActionCmd
    {
        public MahjongFaceValue mjFaceValue;

        public MahjongMoPaiOpCmd()
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            MoPaiAction.Instance.MoPai(seatIdx, mjFaceValue, opCmdNode);
        }
    }

}
