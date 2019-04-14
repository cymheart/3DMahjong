using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActionDesgin
{
    public class FaPaiStateData : ActionStateData
    {
        /// <summary>
        /// 发牌开始
        /// </summary>
        public const int FAPAI_START = 2;

        public const int FAPAI_FEN_SINGLE_DENGING = 3;
        public const int FAPAI_FEN_SINGLE_DENG_END = 4;
        public const int FAPAI_FEN_DENG_END = 5;
        public const int FAPAI_BUHUA = 6;
        public const int FAPAI_SORT = 7;

        /// <summary>
        /// 发牌结束
        /// </summary>
        public const int FAPAI_END = 8;

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
