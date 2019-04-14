﻿using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ActionDesgin
{
    public class SwapPaiStateData: ActionStateData
    {

        /// <summary>
        /// 交换牌开始
        /// </summary>
        public const int SWAP_PAI_START = 6;
        public const int SWAP_PAI_READY_FIRST_HAND = 7;
        public const int SWAP_PAI_MOVE_HAND_TO_DST_POS = 8;
        public const int SWAP_PAI_CHUPAI = 9;
        public const int SWAP_PAI_CHUPAI_TAIHAND = 10;
        public const int SWAP_PAI_TAIHAND_END = 11;
        public const int SWAP_PAI_ROTATE = 12;

        /// <summary>
        /// 交换牌结束
        /// </summary>
        public const int SWAP_PAI_END = 13;

        #region 交换牌动作数据
        public PlayerType handStyle = PlayerType.FEMALE;
        public ActionCombineNum actionCombineNum = ActionCombineNum.End;
        public int swapPaiToSeatIdx;
        public Vector3 swapPaiFromPos;
        public MahjongFaceValue[] swapPaiFaceValues;
        public int[] swapPaiFromSeatPaiIdxs;
        public MahjongFaceValue[] swapPaiMoPaiFaceValues;
        public int[] swapPaiFromSeatMoPaiIdxs;
        public int[] swapPaiToSeatPaiIdxs;
        public SwapPaiDirection swapPaiDir;

        public bool swapPaiIsShowBack;
        public GameObject swapPaiRotControler;
        public GameObject[] swapPaiToSeatTakeMjs;
        #endregion


        /// <summary>
        /// 设置交换牌动作数据
        /// </summary>
        /// <param name="fromPos"></param>
        /// <param name="toSeatIdx"></param>
        /// <param name="mjHandPaiFaceValues"></param>
        /// <param name="fromSeatHandPaiIdx"></param>
        /// <param name="mjMoPaiFaceValues"></param>
        /// <param name="fromSeatMoPaiIdx"></param>
        /// <param name="toSeatHandPaiIdx"></param>
        /// <param name="swapDir"></param>
        /// <param name="isShowBack"></param>
        /// <param name="opCmdNode"></param>
        public void SetSwapPaiData(
            Vector3 fromPos,
            int toSeatIdx,
            MahjongFaceValue[] mjHandPaiFaceValues,
            int[] fromSeatHandPaiIdx,
            MahjongFaceValue[] mjMoPaiFaceValues,
            int[] fromSeatMoPaiIdx,
            int[] toSeatHandPaiIdx,
            SwapPaiDirection swapDir,
            bool isShowBack,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.swapPaiToSeatIdx = toSeatIdx;
            this.swapPaiFromPos = fromPos;
            this.swapPaiFaceValues = mjHandPaiFaceValues;
            this.swapPaiFromSeatPaiIdxs = fromSeatHandPaiIdx;
            this.swapPaiMoPaiFaceValues = mjMoPaiFaceValues;
            this.swapPaiFromSeatMoPaiIdxs = fromSeatMoPaiIdx;
            this.swapPaiToSeatPaiIdxs = toSeatHandPaiIdx;
            this.swapPaiIsShowBack = isShowBack;
            swapPaiDir = swapDir;
            this.opCmdNode = opCmdNode;
        }


    }
}