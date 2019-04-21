using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace ActionDesgin
{
    public class TuiDaoPaiStateData: ActionStateData
    {    
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
