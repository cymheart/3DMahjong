using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 显示交换牌提示
    /// </summary>
    public class ShowSwapPaiHintCmd : MahjongMachineCmd
    {
        public SwapPaiDirection swapPaiDirection = SwapPaiDirection.CLOCKWISE;
        public ShowSwapPaiHintCmd()
        {
        }
        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            SwapPaiHintAction.Instance.ShowSwapPaiHint(swapPaiDirection);
        }

    }

}
