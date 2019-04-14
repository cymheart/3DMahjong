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
    public class DaPaiStateData: ActionStateData
    {
        /// <summary>
        /// 开始打牌
        /// </summary>
        public const int DA_PAI_START = 16;
        public const int DA_PAI_READY_FIRST_HAND = 17;
        public const int DA_PAI_MOVE_HAND_TO_DST_POS = 18;
        public const int DA_PAI_CHUPAI = 19;
        public const int DA_PAI_CHUPAI_ZHENGPAI = 20;
        public const int DA_PAI_CHUPAI_ZHENGPAI_ADJUSTPAI = 21;
        public const int DA_PAI_CHUPAI_TIAOZHENG_HAND = 22;
        public const int DA_PAI_CHUPAI_TIAOZHENG_HAND_MOVPAI1 = 23;
        public const int DA_PAI_CHUPAI_MOVPAI2 = 24;
        public const int DA_PAI_CHUPAI2_MOVPAI = 25;
        public const int DA_PAI_CHUPAI_TAIHAND = 26;

        /// <summary>
        /// 打牌结束
        /// </summary>
        public const int DA_PAI_END = 27;

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
