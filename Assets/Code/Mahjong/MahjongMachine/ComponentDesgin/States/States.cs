using CoreDesgin;
using System;
using UnityEngine;

namespace ComponentDesgin
{
    public class StateDatas<T> : Component
    {
        public float stateStartTime;
        public float stateLiveTime;
        public T state;

        public void SetState(T state, float startTime, float liveTime)
        {
            this.state = state;
            stateStartTime = startTime;
            stateLiveTime = liveTime;
        }
    }

    public class States : MahjongMachineComponent
    {
        /// <summary>
        /// 麻将机状态数据
        /// </summary>
        public StateDatas<MjMachineState> mjMachineStateData;

        /// <summary>
        /// 交换牌提示状态数据
        /// </summary>
        public StateDatas<SwapPaiHintState> swapPaiHintStateData;

        /// <summary>
        /// 手部动作状态数据组
        /// </summary>
        public StateDatas<HandActionState>[] playerStateData = new StateDatas<HandActionState>[]
        {
            new StateDatas<HandActionState>(),
            new StateDatas<HandActionState>(),
            new StateDatas<HandActionState>(),
            new StateDatas<HandActionState>()
        };
    }
}
