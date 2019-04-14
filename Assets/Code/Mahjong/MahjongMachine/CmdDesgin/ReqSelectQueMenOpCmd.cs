using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 请求选择缺门命令
    /// </summary>
    public class ReqSelectQueMenOpCmd : MahjongMachineRequestCmd
    {
        public MahjongHuaSe defaultQueMenHuaSe = MahjongHuaSe.TIAO;

        public ReqSelectQueMenOpCmd()
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            SelectQueMenAction.Instance.SelectQueMen(defaultQueMenHuaSe, opCmdNode);
        }
    }
}
