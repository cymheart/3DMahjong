using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 推倒牌操作
    /// </summary>
    public class MahjongTuiDaoPaiOpCmd : BaseHandActionCmd
    {
        public List<MahjongFaceValue> handPaiValueList;
        public MahjongTuiDaoPaiOpCmd()
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            TuiDaoPaiAction.Instance.TuiDaoPai(seatIdx, handStyle, handPaiValueList, ActionCombineNum.TuiDaoPai, opCmdNode);
        }
    }
}
