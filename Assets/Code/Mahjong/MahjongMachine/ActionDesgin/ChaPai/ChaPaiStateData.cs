using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ActionDesgin
{
    public class ChaPaiStateData : ActionStateData
    {
      
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
