using System.Collections.Generic;

namespace MahjongMachineNS
{
    public class SwapPaiHintStateData
    {
        public SwapPaiHintState state = SwapPaiHintState.HINT_END;

        /// <summary>
        /// 当前状态开始时间
        /// </summary>
        public float stateStartTime = 0;

        /// <summary>
        /// 当前状态生存时间
        /// </summary>
        public float stateLiveTime = -1;

        public SwapPaiDirection swapPaiDirection = SwapPaiDirection.CLOCKWISE;

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="startTime">状态开始时间</param>
        /// <param name="liveTime">状态生存时间</param>
        public void SetState(SwapPaiHintState state, float startTime, float liveTime)
        {
            this.state = state;
            stateStartTime = startTime;
            stateLiveTime = liveTime;
        }


        public void SetData(SwapPaiDirection swapPaiDirection)
        {
            this.swapPaiDirection = swapPaiDirection;
        }

    }
}

