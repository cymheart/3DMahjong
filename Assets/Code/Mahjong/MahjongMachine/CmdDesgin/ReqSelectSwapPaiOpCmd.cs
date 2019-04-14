using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 请求选择交换牌
    /// </summary>
    public class ReqSelectSwapPaiOpCmd : MahjongMachineRequestCmd
    {
        public ReqSelectSwapPaiOpCmd()
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            SelectSwapPaiAction.Instance.SelectSwapPai(opCmdNode);
        }
    }
}
