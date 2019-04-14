using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionDesgin
{
    public class SelectQueMenStateData: ActionStateData
    {
        /// <summary>
        /// 选择缺门开始
        /// </summary>
        public const int SELECT_QUE_MEN_START = 78;

        public const int SELECT_QUE_MEN_SELECTTING = 79;
        public const int SELECT_QUE_MEN_SELECT_END = 81;

        /// <summary>
        /// 选择缺门结束
        /// </summary>
        public const int SELECT_QUE_MEN_END = 81;


        #region 选择缺门动作数据
        public MahjongHuaSe queMenHuaSe;
        public bool queMenIsPlayDownAudio = false;
        public bool queMenIsPlayFeiDingQueAudio = false;
        #endregion


        /// <summary>
        /// 设置选择缺门动作数据
        /// </summary>
        /// <param name="opCmdNode"></param>
        public void SetSelectQueMenData(MahjongHuaSe defaultQueMenHuaSe, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.queMenHuaSe = defaultQueMenHuaSe;
            this.opCmdNode = opCmdNode;
        }

       
    }
}
