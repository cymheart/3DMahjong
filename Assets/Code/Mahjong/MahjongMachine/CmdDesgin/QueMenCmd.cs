using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 缺门命令
    /// </summary>
    public class QueMenCmd : MahjongMachineCmd
    {
        public int seatIdx;
        public MahjongHuaSe queMenHuaSe;
        UISelectQueMen uiSelectQueMen;
        public QueMenCmd()
        {
            uiSelectQueMen = mjMachine.GetComponent<UISelectQueMen>();
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            uiSelectQueMen.AppendPlayQueMenForSeatToList(seatIdx, queMenHuaSe);
            cmdList.RemoveCmd(opCmdNode);
        }
    }
}
