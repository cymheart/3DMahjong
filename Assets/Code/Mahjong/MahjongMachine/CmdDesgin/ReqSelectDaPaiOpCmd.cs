using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 请求选择打牌
    /// </summary>
    public class ReqSelectDaPaiOpCmd : MahjongMachineRequestCmd
    {
        public int[] huPaiInHandPaiIdxs = null;
        public List<HuPaiTipsInfo[]> huPaiInfosInHandPai = null;
        public int[] huPaiInMoPaiIdxs = null;
        public List<HuPaiTipsInfo[]> huPaiInfosInMoPai = null;

        public ReqSelectDaPaiOpCmd()
            : base()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            SelectDaPaiAction.Instance.SelectDaPai(
                huPaiInHandPaiIdxs, huPaiInfosInHandPai,
                huPaiInMoPaiIdxs, huPaiInfosInMoPai, opCmdNode);
        }
    }
}
