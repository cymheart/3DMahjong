using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 请求选择碰吃杠听胡
    /// </summary>
    public class ReqSelectPCGTHPaiOpCmd : MahjongMachineRequestCmd
    {
        public PengChiGangTingHuType[] pcgthBtnTypes;
        public List<MahjongFaceValue[]> chiPaiMjValueList;
        public int[] tingPaiInHandPaiIdxs = null;
        public List<HuPaiTipsInfo[]> tingPaiInfosInHandPai = null;
        public int[] tingPaiInMoPaiIdxs = null;
        public List<HuPaiTipsInfo[]> tingPaiInfosInMoPai = null;

        public ReqSelectPCGTHPaiOpCmd()
            : base()
        {
        }
        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }
        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            SelectPCGTHPaiAction.Instance.SelectPCGTHPai(pcgthBtnTypes,
                          chiPaiMjValueList, tingPaiInHandPaiIdxs,
                          tingPaiInfosInHandPai, tingPaiInMoPaiIdxs, tingPaiInfosInMoPai, opCmdNode);
        }
    }

}
