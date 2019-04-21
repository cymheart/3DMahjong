using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ActionDesgin
{
    public class DaPaiStateData: ActionStateData
    {

        #region 打牌动作数据
        public PlayerType handStyle = PlayerType.FEMALE;
        public ActionCombineNum actionCombineNum = ActionCombineNum.End;
        public Vector3Int mjPosIdx;
        public int daPaiHandPaiIdx;
        public HandPaiType daPaiHandPaiType = HandPaiType.HandPai;
        public MahjongFaceValue daPaiFaceValue = MahjongFaceValue.MJ_ZFB_FACAI;
        public GameObject curtHandReadyPutDeskPai;
        public bool isJiaoTing = false;
        #endregion


        /// <summary>
        /// 设置打牌动作数据
        /// </summary>
        /// <param name="handStyle"></param>
        /// <param name="mjPosIdx"></param>
        /// <param name="mjFaceValue"></param>
        /// <param name="actionCombineNum"></param>
        /// <param name="opCmdNode"></param>
        public void SetDaPaiData(
            PlayerType handStyle, Vector3Int mjPosIdx, MahjongFaceValue mjFaceValue,
            bool isJiaoTing,
            ActionCombineNum actionCombineNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            this.mjPosIdx = mjPosIdx;
            this.daPaiFaceValue = mjFaceValue;
            this.isJiaoTing = isJiaoTing;
            this.actionCombineNum = actionCombineNum;
            this.opCmdNode = opCmdNode;
        }
    }
}
