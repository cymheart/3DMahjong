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
    public class SortPaiStateData: ActionStateData
    {
        /// <summary>
        /// 开始整理牌
        /// </summary>
        public const int SORT_PAI_START = 37;

        /// <summary>
        /// 整理牌结束
        /// </summary>
        public const int SORT_PAI_END = 38;


        #region 整理牌动作数据
        public SortPaiType sortPaiType = SortPaiType.LEFT;
        #endregion

        /// <summary>
        /// 设置整理牌动作数据
        /// </summary>
        /// <param name="sortPaiType"></param>
        /// <param name="opCmdNode"></param>
        public void SetSortPaiData(SortPaiType sortPaiType = SortPaiType.LEFT, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.sortPaiType = sortPaiType;
            this.opCmdNode = opCmdNode;
        }



    }
}
