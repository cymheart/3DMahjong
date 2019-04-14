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
    public class TuiDaoPaiStateData: ActionStateData
    {
        /// <summary>
        /// 推倒牌开始
        /// </summary>
        public const int TUIDAO_PAI_START = 60;

        public const int TUIDAO_PAI_ZHUA_HAND_PAI = 61;
        public const int TUIDAO_PAI_BACK_MOVE_HAND_PAI = 62;
        public const int TUIDAO_PAI_TUIDAO_HAND_PAI = 63;
        public const int TUIDAO_PAI_TUIDAO_HAND_PAI_TAIHAND = 64;

        /// <summary>
        /// 推倒牌结束
        /// </summary>
        public const int TUIDAO_PAI_END = 65;


        #region 推倒牌动作数据
        public PlayerType handStyle = PlayerType.FEMALE;
        public ActionCombineNum actionCombineNum = ActionCombineNum.End;
        public List<MahjongFaceValue> handPaiValueList;
        #endregion


        /// <summary>
        /// 设置推倒牌动作数据
        /// </summary>
        /// <param name="handStyle"></param>
        /// <param name="handPaiValueList"></param>
        /// <param name="actionCombineNum"></param>
        /// <param name="opCmdNode"></param>
        public void SetTuiDaoPaiData(
            PlayerType handStyle, List<MahjongFaceValue> handPaiValueList,
            ActionCombineNum actionCombineNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            this.actionCombineNum = actionCombineNum;
            this.handPaiValueList = handPaiValueList;
            this.opCmdNode = opCmdNode;
        }
    }
}
