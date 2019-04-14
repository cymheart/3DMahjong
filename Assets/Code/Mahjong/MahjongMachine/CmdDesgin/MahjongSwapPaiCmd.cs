using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 交换麻将牌
    /// </summary>
    /// 
    public class MahjongSwapPaiCmd : MahjongMachineHandActionCmd
    {
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
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            SwapPaiAction.Instance.SwapPai(
                           seatIdx, toSeatIdx, swapMjCount,
                           toSeatHandPaiIdx,
                           mjFaceValues, fromSeatHandPaiIdx,
                           mjMoPaiFaceValues, fromSeatMoPaiIdx,
                           isShowBack, swapDir, opCmdNode);
        }
    }

}
