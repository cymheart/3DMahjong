using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MahjongMachineNS
{
    public class MahjongMachineStateData
    {
        public float stateStartTime;
        public float stateLiveTime;

        public MahjongMachineState state = MahjongMachineState.END;
        public LinkedListNode<MahjongMachineCmd> opCmdNode = null;

        #region 洗牌数据
        public int dealerSeatIdx;
        public FengWei fengWei;
        #endregion

        #region 发牌数据
        public int faPaiStartIdx;

        /// <summary>
        /// 指手牌按墩数间隔移位
        /// </summary>
        public int faPaiPosIdx;

        /// <summary>
        /// 一次发牌给对应座位手牌的数量，一般是一墩的数量4张牌
        /// </summary>
        public int faPaiSingleCount;
        public int faPaiSeat;
        public int faPaiPlayerOrderIdx;

        /// <summary>
        /// 发牌轮次
        /// </summary>
        public int faPaiTurn;

        /// <summary>
        /// 手牌麻将的总墩数
        /// </summary>
        public int faPaiMjDengCount;

        /// <summary>
        /// 手牌麻将尾墩的麻将数量
        /// </summary>
        public int faPaiMjTailCount;

        public List<MahjongFaceValue> selfHuaPaiValueList;
        public List<MahjongFaceValue> selfBuPaiValueList;

        #endregion

     
        public void SetState(MahjongMachineState state, float startTime, float liveTime)
        {
            this.state = state;
            stateStartTime = startTime;
            stateLiveTime = liveTime;
        }

        public void SetXiPaiData(int dealerSeatIdx, FengWei fengWei, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.dealerSeatIdx = dealerSeatIdx;
            this.fengWei = fengWei;
            this.opCmdNode = opCmdNode;
        }

        public void SetFaPaiData(int startPaiIdx, 
            List<MahjongFaceValue> selfHuaPaiValueList,
            List<MahjongFaceValue> selfBuPaiValueList, 
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            faPaiStartIdx = startPaiIdx;
            this.selfHuaPaiValueList = selfHuaPaiValueList;
            this.selfBuPaiValueList = selfBuPaiValueList;
            this.opCmdNode = opCmdNode;
        }

    }
}
