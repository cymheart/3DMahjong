using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 碰吃杠牌命令
    /// </summary>
    public class MahjongPcgPaiOpCmd : BaseHandActionCmd
    {
        public bool isMoveHand;
        public float moveHandDist;
        public MahjongFaceValue[] faceValues;
        public PengChiGangPaiType pcgType;
        public int pcgTargetSeatIdx;
        public Vector3Int pcgTargetMjIdx = new Vector3Int(-1, -1, -1);
        public EffectFengRainEtcType fengRainEffectEtcType;
        public MahjongPcgPaiOpCmd()
            : base()
        {

        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            PengChiGangPaiAction.Instance.PengChiGangPai(
                           seatIdx, handStyle, isMoveHand, moveHandDist,
                           faceValues, pcgType, pcgTargetSeatIdx, pcgTargetMjIdx, fengRainEffectEtcType,
                           ActionCombineNum.PengPai, opCmdNode);
        }
    }
}
