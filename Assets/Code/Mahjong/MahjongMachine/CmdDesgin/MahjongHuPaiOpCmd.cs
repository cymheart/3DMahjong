using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 胡牌操作
    /// </summary>
    public class MahjongHuPaiOpCmd : BaseHandActionCmd
    {
        public int huTargetSeatIdx;
        public Vector3Int huTargetMjIdx = new Vector3Int(-1, -1, -1);
        public MahjongFaceValue huPaiFaceValue;

        public MahjongHuPaiOpCmd()
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }


        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            HuPaiAction.Instance.HuPai(seatIdx, handStyle, huTargetSeatIdx, huTargetMjIdx, huPaiFaceValue, ActionCombineNum.HuPai, opCmdNode);
        }
    }



}
