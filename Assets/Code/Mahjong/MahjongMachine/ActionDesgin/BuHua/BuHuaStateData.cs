using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;

namespace ActionDesgin
{
    public class BuHuaStateData : ActionStateData
    {
        #region 补花动作数据
        public PlayerType handStyle = PlayerType.FEMALE;
        public ActionCombineNum actionCombineNum = ActionCombineNum.End;
        public int buHuaPaiMjPosIdx;
        public MahjongFaceValue buHuaPaiFaceValue;
        #endregion

        /// <summary>
        /// 设置补花动作数据
        /// </summary>
        public void SetBuHuaPaiData(
            PlayerType handStyle, int buHuaPaiPos, MahjongFaceValue buHuaPaiFaceValue, ActionCombineNum actionCombineNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            buHuaPaiMjPosIdx = buHuaPaiPos;
            this.buHuaPaiFaceValue = buHuaPaiFaceValue;
            this.actionCombineNum = actionCombineNum;
            this.opCmdNode = opCmdNode;
        }

    }
}
