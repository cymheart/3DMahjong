using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CoreDesgin
{
    /// <summary>
    /// 状态数据
    /// </summary>
    public class StateData : CommonComponent
    {
        public LinkedListNode<MahjongMachineCmd> opCmdNode;
        public const int END = 10000000;
    }

    /// <summary>
    /// 状态数据组
    /// </summary>
    public class StateDataGroup : CommonComponent
    {
        /// <summary>
        /// 结束状态
        /// </summary>
        public const int END = 10000000;

        public float stateStartTime;
        public float stateLiveTime;

        public int state = END;

        public void SetState(int state, float startTime, float liveTime)
        {
            this.state = state;
            stateStartTime = startTime;
            stateLiveTime = liveTime;
        }
    }
}
