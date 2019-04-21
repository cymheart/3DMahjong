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
