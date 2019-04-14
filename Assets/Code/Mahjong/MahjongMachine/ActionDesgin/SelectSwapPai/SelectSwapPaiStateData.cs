using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionDesgin
{
    public class SelectSwapPaiStateData: ActionStateData
    {
        /// <summary>
        /// 选择交换牌开始
        /// </summary>
        public const int SELECT_SWAP_PAI_START = 75;

        public const int SELECT_SWAP_PAI_SELECTTING = 76;

        /// <summary>
        /// 选择交换牌结束
        /// </summary>
        public const int SELECT_SWAP_PAI_END = 77;


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
