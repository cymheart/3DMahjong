using CoreDesgin;
using System.Collections.Generic;


namespace ActionDesgin
{
    public class SelectSwapPaiStateData: ActionStateData
    {
       
        /// <summary>
        /// 设置选择交换牌动作数据
        /// </summary>
        /// <param name="opCmdNode"></param>
        public void SetSelectSwapPaiData(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.opCmdNode = opCmdNode;
        }

    }
}
