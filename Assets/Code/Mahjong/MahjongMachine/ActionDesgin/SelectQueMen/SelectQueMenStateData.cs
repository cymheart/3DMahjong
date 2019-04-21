using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;

namespace ActionDesgin
{
    public class SelectQueMenStateData: ActionStateData
    {
      

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
