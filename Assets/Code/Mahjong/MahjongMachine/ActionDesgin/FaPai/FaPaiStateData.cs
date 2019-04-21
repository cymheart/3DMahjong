using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;

namespace ActionDesgin
{
    public class FaPaiStateData : ActionStateData
    {
       
        #region 发牌数据
        public List<MahjongFaceValue> handPaiValueList;

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
