

namespace ComponentDesgin
{
    /// <summary>
    /// 交换牌提示状态
    /// </summary>
    public enum SwapPaiHintState
    {
        HINT_START = 0,
        HINTTING = 1,
        HINT_END = 2
    }

    /// <summary>
    /// 麻将机状态
    /// </summary>
    public enum MjMachineState
    {
        #region 洗牌动作状态
        /// <summary>
        /// 洗牌开始
        /// </summary>
        XIPAI_START = 0,

        /// <summary>
        /// 洗牌结束
        /// </summary>
        XIPAI_END = 1,
        #endregion

        #region 发牌动作状态
        /// <summary>
        /// 发牌开始
        /// </summary>
        FAPAI_START = 2,

        FAPAI_FEN_SINGLE_DENGING = 3,
        FAPAI_FEN_SINGLE_DENG_END = 4,
        FAPAI_FEN_DENG_END = 5,
        FAPAI_BUHUA = 6,
        FAPAI_SORT = 7,

        /// <summary>
        /// 发牌结束
        /// </summary>
        FAPAI_END = 8,

        #endregion

        END
    }


    /// <summary>
    /// 手部动作状态
    /// </summary>
    public enum HandActionState
    {
        #region 启动骰子器动作状态
        /// <summary>
        /// 启动骰子器开始
        /// </summary>
        QIDONG_DICEMACHINE_START = 0,

        QIDONG_DICEMACHINE_READY_FIRST_HAND = 1,
        QIDONG_DICEMACHINE_MOVE_HAND_TO_DST_POS = 2,
        QIDONG_DICEMACHINE_QIDONG = 3,
        QIDONG_DICEMACHINE_TAIHAND = 4,

        /// <summary>
        /// 结束启动骰子器
        /// </summary>
        QIDONG_DICEMACHINE_END = 5,

        #endregion

        #region 交换牌动作状态

        /// <summary>
        /// 交换牌开始
        /// </summary>
        SWAP_PAI_START = 6,
        SWAP_PAI_READY_FIRST_HAND = 7,
        SWAP_PAI_MOVE_HAND_TO_DST_POS = 8,
        SWAP_PAI_CHUPAI = 9,
        SWAP_PAI_CHUPAI_TAIHAND = 10,
        SWAP_PAI_TAIHAND_END = 11,
        SWAP_PAI_ROTATE = 12,

        /// <summary>
        /// 交换牌结束
        /// </summary>
        SWAP_PAI_END = 13,
        #endregion

        #region 摸牌动作状态
        /// <summary>
        /// 摸牌开始
        /// </summary>
        MO_PAI_START = 14,

        /// <summary>
        /// 摸牌结束
        /// </summary>
        MO_PAI_END = 15,

        #endregion

        #region 打牌动作状态
        /// <summary>
        /// 开始打牌
        /// </summary>
        DA_PAI_START = 16,
        DA_PAI_READY_FIRST_HAND = 17,
        DA_PAI_MOVE_HAND_TO_DST_POS = 18,
        DA_PAI_CHUPAI = 19,
        DA_PAI_CHUPAI_ZHENGPAI = 20,
        DA_PAI_CHUPAI_ZHENGPAI_ADJUSTPAI = 21,
        DA_PAI_CHUPAI_TIAOZHENG_HAND = 22,
        DA_PAI_CHUPAI_TIAOZHENG_HAND_MOVPAI1 = 23,
        DA_PAI_CHUPAI_MOVPAI2 = 24,
        DA_PAI_CHUPAI2_MOVPAI = 25,
        DA_PAI_CHUPAI_TAIHAND = 26,

        /// <summary>
        /// 打牌结束
        /// </summary>
        DA_PAI_END = 27,

        #endregion




        #region 插牌动作状态
        /// <summary>
        /// 开始插牌
        /// </summary>
        CHA_PAI_START = 28,

        CHA_PAI_ZHUA_HAND_PAI = 29,
        CHA_PAI_TI_HAND_PAI = 30,
        CHA_PAI_TI_HAND_PAI_MOVE = 31,
        CHA_PAI_PUTDOWNHAND = 32,
        CHA_PAI_ADJUST_PAI = 33,
        CHA_PAI_TAIHAND = 34,

        /// <summary>
        /// 插牌结束
        /// </summary>
        CHA_PAI_END = 35,

        #endregion


        #region 整理牌动作状态
        /// <summary>
        /// 开始整理牌
        /// </summary>
        SORT_PAI_START = 37,

        /// <summary>
        /// 整理牌结束
        /// </summary>
        SORT_PAI_END = 38,

        #endregion

        #region 补花动作状态
        /// <summary>
        /// 开始补花
        /// </summary>
        BUHUA_PAI_START = 39,

        BUHUA_PAI_READY_FIRST_HAND = 40,
        BUHUA_PAI_MOVE_HAND_TO_DST_POS = 41,
        BUHUA_PAI_BU = 42,
        BUHUA_PAI_GET_PAI = 43,
        BUHUA_PAI_TAIHAND = 44,

        /// <summary>
        /// 补花结束
        /// </summary>
        BUHUA_PAI_END = 45,

        #endregion




        #region 胡牌动作状态
        /// <summary>
        /// 开始胡牌
        /// </summary>
        HU_PAI_START = 46,

        HU_PAI_READY_FIRST_HAND = 47,
        HU_PAI_MOVE_HAND_TO_DST_POS = 48,
        HU_PAI_HU = 49,
        HU_PAI_GET_PAI = 50,
        HU_PAI_TAIHAND = 51,

        /// <summary>
        /// 胡牌结束
        /// </summary>
        HU_PAI_END = 52,
        #endregion

        #region 碰吃杠牌动作状态
        /// <summary>
        /// 开始碰吃杠牌
        /// </summary>
        PENG_CHI_GANG_PAI_START = 53,

        PENG_CHI_GANG_PAI_READY_FIRST_HAND = 54,
        PENG_CHI_GANG_PAI_MOVE_HAND_TO_DST_POS = 55,
        PENG_CHI_GANG_PAI_PCG_PAI = 56,
        PENG_CHI_GANG_PAI_MOVE_PAI = 57,
        PENG_CHI_GANG_PAI_TAIHAND = 58,

        /// <summary>
        /// 碰吃杠牌结束
        /// </summary>
        PENG_CHI_GANG_PAI_END = 59,
        #endregion

        #region 推倒牌动作状态
        /// <summary>
        /// 推倒牌开始
        /// </summary>
        TUIDAO_PAI_START = 60,

        TUIDAO_PAI_ZHUA_HAND_PAI = 61,
        TUIDAO_PAI_BACK_MOVE_HAND_PAI = 62,
        TUIDAO_PAI_TUIDAO_HAND_PAI = 63,
        TUIDAO_PAI_TUIDAO_HAND_PAI_TAIHAND = 64,

        /// <summary>
        /// 推倒牌结束
        /// </summary>
        TUIDAO_PAI_END = 65,
        #endregion

        #region 选择牌动作状态
        /// <summary>
        /// 选择牌开始
        /// </summary>
        SELECT_PAI_START = 66,

        SELECT_PAI_READY_CLICK = 67,
        SELECT_PAIING = 68,

        /// <summary>
        /// 选择牌结束
        /// </summary>
        SELECT_PAI_END = 69,
        #endregion


        #region 选择打牌动作状态
        /// <summary>
        /// 选择打牌开始
        /// </summary>
        SELECT_DA_PAI_START = 70,

        SELECT_DA_PAI_READY_CLICK = 71,
        SELECT_DA_PAIING = 72,
        SELECT_DA_PAI_RESTORE = 73,

        /// <summary>
        /// 选择打牌结束
        /// </summary>
        SELECT_DA_PAI_END = 74,
        #endregion


        #region 选择交换牌动作状态
        /// <summary>
        /// 选择交换牌开始
        /// </summary>
        SELECT_SWAP_PAI_START = 75,

        SELECT_SWAP_PAI_SELECTTING = 76,

        /// <summary>
        /// 选择交换牌结束
        /// </summary>
        SELECT_SWAP_PAI_END = 77,
        #endregion

        #region 选择缺门动作状态
        /// <summary>
        /// 选择缺门开始
        /// </summary>
        SELECT_QUE_MEN_START = 78,

        SELECT_QUE_MEN_SELECTTING = 79,
        SELECT_QUE_MEN_SELECT_END = 81,

        /// <summary>
        /// 选择缺门结束
        /// </summary>
        SELECT_QUE_MEN_END = 81,
        #endregion

        #region 选择碰吃杠听胡牌动作状态
        /// <summary>
        /// 选择碰吃杠听胡牌开始
        /// </summary>
        SELECT_PCGTH_PAI_START = 82,
        SELECT_PCGTH_PAI_SELECTTING = 83,
        SELECT_PCGTH_PAI_SELECTTING_CHIPAI = 84,
        SELECT_PCGTH_PAI_SELECT_TING_PAI_START = 85,
        SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK = 86,
        SELECT_PCGTH_PAI_SELECT_TING_PAIING = 87,
        SELECT_PCGTH_PAI_SELECT_TING_PAI_RESTORE = 88,
        SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END = 89,

        /// <summary>
        /// 选择碰吃杠听胡牌结束
        /// </summary>
        SELECT_PCGTH_PAI_END = 90,

        #endregion




        END

    }




}
