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
    public class BuHuaStateData : ActionStateData
    {
        /// <summary>
        /// 开始补花
        /// </summary>
        public const int BUHUA_PAI_START = 39;

        public const int BUHUA_PAI_READY_FIRST_HAND = 40;
        public const int BUHUA_PAI_MOVE_HAND_TO_DST_POS = 41;
        public const int BUHUA_PAI_BU = 42;
        public const int BUHUA_PAI_GET_PAI = 43;
        public const int BUHUA_PAI_TAIHAND = 44;

        /// <summary>
        /// 补花结束
        /// </summary>
        public const int BUHUA_PAI_END = 45;


        #region 补花动作数据
        public PlayerType handStyle = PlayerType.FEMALE;
        public ActionCombineNum actionCombineNum = ActionCombineNum.End;
        public int buHuaPaiMjPosIdx;
        public MahjongFaceValue buHuaPaiFaceValue;

        #endregion

        /// <summary>
        /// 设置胡牌动作数据
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
