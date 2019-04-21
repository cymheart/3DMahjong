using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace ActionDesgin
{
    public class SortPaiStateData: ActionStateData
    {
       
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
