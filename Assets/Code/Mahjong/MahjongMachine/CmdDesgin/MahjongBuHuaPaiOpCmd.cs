using CoreDesgin;
using System.Collections.Generic;
using ComponentDesgin;
using ActionDesgin;

namespace CmdDesgin
{
    /// <summary>
    /// 补花命令
    /// </summary>
    public class MahjongBuHuaPaiOpCmd : BaseHandActionCmd
    {
        public MahjongFaceValue buHuaPaiFaceValue;

        public MahjongBuHuaPaiOpCmd()
            : base()
        {

        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            BuHuaAction.Instance.BuHua(seatIdx, handStyle, buHuaPaiFaceValue, ActionCombineNum.HuPai, opCmdNode);
        }
    }
}