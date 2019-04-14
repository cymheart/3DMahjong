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
    public class ChaPaiStateData : ActionStateData
    {
        /// <summary>
        /// 开始插牌
        /// </summary>
        public const int CHA_PAI_START = 28;

        public const int CHA_PAI_ZHUA_HAND_PAI = 29;
        public const int CHA_PAI_TI_HAND_PAI = 30;
        public const int CHA_PAI_TI_HAND_PAI_MOVE = 31;
        public const int CHA_PAI_PUTDOWNHAND = 32;
        public const int CHA_PAI_ADJUST_PAI = 33;
        public const int CHA_PAI_TAIHAND = 34;

        /// <summary>
        /// 插牌结束
        /// </summary>
        public const int CHA_PAI_END = 35;


        #region 插牌动作数据
        public PlayerType handStyle = PlayerType.FEMALE;
        public int orgPaiIdx;
        public int chaPaiDstHandPaiIdx;
        public HandPaiType chaPaiHandPaiType = HandPaiType.HandPai;
        public HandPaiAdjustDirection adjustDirection;
        public Vector3 dstHandPaiPostion = Vector3.zero;
        public MahjongFaceValue curtAdjustHandPaiFaceValue = MahjongFaceValue.MJ_UNKNOWN;
        public GameObject curtAdjustHandPai;
        #endregion

        /// <summary>
        /// 设置插牌动作数据
        /// </summary>
        /// <param name="handStyle"></param>
        /// <param name="orgPaiIdx"></param>
        /// <param name="dstHandPaiIdx"></param>
        /// <param name="orgPaiType"></param>
        /// <param name="adjustDirection"></param>
        /// <param name="opCmdNode"></param>
        public void SetChaPaiData(
            PlayerType handStyle, int orgPaiIdx, int dstHandPaiIdx, HandPaiType orgPaiType,
            HandPaiAdjustDirection adjustDirection,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            this.orgPaiIdx = orgPaiIdx;
            this.chaPaiDstHandPaiIdx = dstHandPaiIdx;
            this.chaPaiHandPaiType = orgPaiType;
            this.adjustDirection = adjustDirection;
            this.opCmdNode = opCmdNode;
        }
    }
}
