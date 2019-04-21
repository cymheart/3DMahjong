using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActionDesgin
{
    public class XiPaiStateData : ActionStateData
    {

        #region 洗牌数据
        public int dealerSeatIdx;
        public FengWei fengWei;
        #endregion

        public void SetXiPaiData(int dealerSeatIdx, FengWei fengWei, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.dealerSeatIdx = dealerSeatIdx;
            this.fengWei = fengWei;
            this.opCmdNode = opCmdNode;
        }
    }
}
