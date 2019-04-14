using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActionDesgin
{
    public class PengChiGangPaiStateData : ActionStateData
    {
        /// <summary>
        /// 开始碰吃杠牌
        /// </summary>
        public const int PENG_CHI_GANG_PAI_START = 53;

        public const int PENG_CHI_GANG_PAI_READY_FIRST_HAND = 54;
        public const int PENG_CHI_GANG_PAI_MOVE_HAND_TO_DST_POS = 55;
        public const int PENG_CHI_GANG_PAI_PCG_PAI = 56;
        public const int PENG_CHI_GANG_PAI_MOVE_PAI = 57;
        public const int PENG_CHI_GANG_PAI_TAIHAND = 58;

        /// <summary>
        /// 碰吃杠牌结束
        /// </summary>
        public const int PENG_CHI_GANG_PAI_END = 59;


        #region 碰吃杠牌动作数据
        public PlayerType handStyle = PlayerType.FEMALE;
        public ActionCombineNum actionCombineNum = ActionCombineNum.End;
        public bool pcgPaiIsMoveHand;
        public float pcgPaiMoveHandDist;
        public int pcgPaiTargetSeatIdx;
        public Vector3Int pcgPaiTargetMjIdx;
        public int pcgPaiTargetMjKey = -1;
        public PengChiGangPaiType pcgPaiType;
        public MahjongFaceValue[] pcgPaiMjfaceValues;
        public int pcgPaiLayoutIdx;
        public Vector3[] pcgDstStartPos;
        public Vector3[] pcgOrgStartPos;
        public GameObject[] pcgMjList;
        public int pcgMjIdx;
        public EffectFengRainEtcType fengRainEtcEffect;
        #endregion


        /// <summary>
        /// 设置碰吃杠牌动作数据
        /// </summary>
        /// <param name="handStyle"></param>
        /// <param name="isMoveHand"></param>
        /// <param name="moveHandDist"></param>
        /// <param name="pcgPaiType"></param>
        /// <param name="mjfaceValues"></param>
        /// <param name="paiLayoutIdx"></param>
        /// <param name="targetSeatIdx"></param>
        /// <param name="targetMjIdx"></param>
        /// <param name="actionCombineNum"></param>
        /// <param name="opCmdNode"></param>
        public void SetPengChiGangPaiData(PlayerType handStyle, bool isMoveHand, float moveHandDist,
                PengChiGangPaiType pcgPaiType, MahjongFaceValue[] mjfaceValues, int paiLayoutIdx,
                int targetSeatIdx, Vector3Int targetMjIdx,
                EffectFengRainEtcType fengRainEtcEffect,
                ActionCombineNum actionCombineNum,
                LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            pcgPaiIsMoveHand = isMoveHand;
            pcgPaiMoveHandDist = moveHandDist;
            this.pcgPaiType = pcgPaiType;
            this.pcgPaiMjfaceValues = mjfaceValues;
            this.pcgPaiLayoutIdx = paiLayoutIdx;
            this.pcgPaiTargetSeatIdx = targetSeatIdx;
            this.pcgPaiTargetMjIdx = targetMjIdx;
            this.fengRainEtcEffect = fengRainEtcEffect;
            this.actionCombineNum = actionCombineNum;
            this.opCmdNode = opCmdNode;
        }
    }
}
