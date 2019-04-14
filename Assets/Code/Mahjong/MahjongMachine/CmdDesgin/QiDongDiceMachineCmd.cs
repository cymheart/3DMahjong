using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 启动骰子机命令
    /// </summary>
    public class QiDongDiceMachineCmd : MahjongMachineHandActionCmd
    {
        public int dice1Point = -1;
        public int dice2Point = -1;
        public QiDongDiceMachineCmd()
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            QiDongDiceMachineAction.Instance.QiDongDiceMachine(seatIdx, dice1Point, dice2Point, opCmdNode);
        }
    }
}
