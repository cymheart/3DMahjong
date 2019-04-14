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
    public class QiDongDiceMachineStateData : ActionStateData
    {
        /// <summary>
        /// 启动骰子器开始
        /// </summary>
        public const int QIDONG_DICEMACHINE_START = 0;

        public const int QIDONG_DICEMACHINE_READY_FIRST_HAND = 1;
        public const int QIDONG_DICEMACHINE_MOVE_HAND_TO_DST_POS = 2;
        public const int QIDONG_DICEMACHINE_QIDONG = 3;
        public const int QIDONG_DICEMACHINE_TAIHAND = 4;

        /// <summary>
        /// 结束启动骰子器
        /// </summary>
        public const int QIDONG_DICEMACHINE_END = 5;


        #region 启动骰子机动作数据
        public PlayerType handStyle = PlayerType.FEMALE;
        public ActionCombineNum actionCombineNum = ActionCombineNum.End;
        public int dice1Point = -1;
        public int dice2Point = -1;
        #endregion

        /// <summary>
        /// 设置启动骰子机动作数据
        /// </summary>
        /// <param name="opCmdNode"></param>
        public void SetQiDongDiceMachineData(int dice1Point = -1, int dice2Point = -1, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.dice1Point = dice1Point;
            this.dice2Point = dice2Point;
            this.opCmdNode = opCmdNode;
        }
    }
}
