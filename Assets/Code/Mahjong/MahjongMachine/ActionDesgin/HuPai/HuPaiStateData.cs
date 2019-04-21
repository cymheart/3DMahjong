using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ActionDesgin
{
    public class HuPaiStateData: ActionStateData
    {
        #region 胡牌动作数据
        public PlayerType handStyle = PlayerType.FEMALE;
        public ActionCombineNum actionCombineNum = ActionCombineNum.End;
        public int huPaiMjPosIdx;
        public int huPaiTargetSeatIdx;
        public Vector3Int huPaiTargetMjIdx;
        public int huPaiTargetMjKey = -1;
        public MahjongFaceValue huPaiFaceValue;
        #endregion


        /// <summary>
        /// 设置胡牌动作数据
        /// </summary>
        /// <param name="handStyle"></param>
        /// <param name="targetSeatIdx"></param>
        /// <param name="targetMjIdx"></param>
        /// <param name="huPaiPos"></param>
        /// <param name="huPaiFaceValue"></param>
        /// <param name="actionCombineNum"></param>
        /// <param name="opCmdNode"></param>
        public void SetHuPaiData(
            PlayerType handStyle, int targetSeatIdx, Vector3Int targetMjIdx,
            int huPaiPos, MahjongFaceValue huPaiFaceValue, ActionCombineNum actionCombineNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            huPaiMjPosIdx = huPaiPos;
            huPaiTargetSeatIdx = targetSeatIdx;
            huPaiTargetMjIdx = targetMjIdx;
            huPaiTargetMjKey = -1;
            this.huPaiFaceValue = huPaiFaceValue;
            this.actionCombineNum = actionCombineNum;
            this.opCmdNode = opCmdNode;
        }

    }
}
