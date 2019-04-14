using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 隐藏交换牌UI
    /// </summary>
    public class HideSwapPaiUICmd : MahjongMachineCmd
    {
        UISelectSwapHandPai uiSelectSwapHandPai;
        public HideSwapPaiUICmd()
            : base()
        {
            uiSelectSwapHandPai = mjMachine.GetComponent<UISelectSwapHandPai>();
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            uiSelectSwapHandPai.CompleteSwapPaiSelected();
            cmdList.RemoveCmd(opCmdNode);
        }
    }
}
