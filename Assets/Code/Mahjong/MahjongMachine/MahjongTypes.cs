using UnityEngine;

namespace MahjongMachineNS
{
    /// <summary>
    /// 麻将面值
    /// </summary>
    public enum MahjongFaceValue
    {
        MJ_WANG_1,
        MJ_WANG_2,
        MJ_WANG_3,
        MJ_WANG_4,
        MJ_WANG_5,
        MJ_WANG_6,
        MJ_WANG_7,
        MJ_WANG_8,
        MJ_WANG_9,
        MJ_TONG_1,
        MJ_TONG_2,
        MJ_TONG_3,
        MJ_TONG_4,
        MJ_TONG_5,
        MJ_TONG_6,
        MJ_TONG_7,
        MJ_TONG_8,
        MJ_TONG_9,
        MJ_TIAO_1,
        MJ_TIAO_2,
        MJ_TIAO_3,
        MJ_TIAO_4,
        MJ_TIAO_5,
        MJ_TIAO_6,
        MJ_TIAO_7,
        MJ_TIAO_8,
        MJ_TIAO_9,
        MJ_FENG_DONG,
        MJ_FENG_NAN,
        MJ_FENG_XI,
        MJ_FENG_BEI,
        MJ_ZFB_HONGZHONG,
        MJ_ZFB_FACAI,
        MJ_ZFB_BAIBAN,
        MJ_HUA_CHUN,
        MJ_HUA_XIA,
        MJ_HUA_QIU,
        MJ_HUA_DONG,
        MJ_HUA_MEI,
        MJ_HUA_LAN,
        MJ_HUA_ZHU,
        MJ_HUA_JU,
        MJ_UNKNOWN,
    }

    /// <summary>
    /// 麻将花色
    /// </summary>
    public enum MahjongHuaSe
    {
        /// <summary>
        /// 万
        /// </summary>
        WANG,

        /// <summary>
        /// 筒
        /// </summary>
        TONG,

        /// <summary>
        /// 条
        /// </summary>
        TIAO,

        /// <summary>
        /// 风
        /// </summary>
        FENG,

        /// <summary>
        /// 花
        /// </summary>
        HUA,
    }

    public enum AudioIdx
    {
        AUDIO_SPEAK_MJ_WANG_1,
        AUDIO_SPEAK_MJ_WANG_2,
        AUDIO_SPEAK_MJ_WANG_3,
        AUDIO_SPEAK_MJ_WANG_4,
        AUDIO_SPEAK_MJ_WANG_5,
        AUDIO_SPEAK_MJ_WANG_6,
        AUDIO_SPEAK_MJ_WANG_7,
        AUDIO_SPEAK_MJ_WANG_8,
        AUDIO_SPEAK_MJ_WANG_9,
        AUDIO_SPEAK_MJ_TONG_1,
        AUDIO_SPEAK_MJ_TONG_2,
        AUDIO_SPEAK_MJ_TONG_3,
        AUDIO_SPEAK_MJ_TONG_4,
        AUDIO_SPEAK_MJ_TONG_5,
        AUDIO_SPEAK_MJ_TONG_6,
        AUDIO_SPEAK_MJ_TONG_7,
        AUDIO_SPEAK_MJ_TONG_8,
        AUDIO_SPEAK_MJ_TONG_9,
        AUDIO_SPEAK_MJ_TIAO_1,
        AUDIO_SPEAK_MJ_TIAO_2,
        AUDIO_SPEAK_MJ_TIAO_3,
        AUDIO_SPEAK_MJ_TIAO_4,
        AUDIO_SPEAK_MJ_TIAO_5,
        AUDIO_SPEAK_MJ_TIAO_6,
        AUDIO_SPEAK_MJ_TIAO_7,
        AUDIO_SPEAK_MJ_TIAO_8,
        AUDIO_SPEAK_MJ_TIAO_9,
        AUDIO_SPEAK_MJ_FENG_DONG,
        AUDIO_SPEAK_MJ_FENG_NAN,
        AUDIO_SPEAK_MJ_FENG_XI,
        AUDIO_SPEAK_MJ_FENG_BEI,
        AUDIO_SPEAK_MJ_ZFB_HONGZHONG,
        AUDIO_SPEAK_MJ_ZFB_FACAI,
        AUDIO_SPEAK_MJ_ZFB_BAIBAN,
        AUDIO_SPEAK_MJ_HUA_CHUN,
        AUDIO_SPEAK_MJ_HUA_XIA,
        AUDIO_SPEAK_MJ_HUA_QIU,
        AUDIO_SPEAK_MJ_HUA_DONG,
        AUDIO_SPEAK_MJ_HUA_MEI,
        AUDIO_SPEAK_MJ_HUA_LAN,
        AUDIO_SPEAK_MJ_HUA_ZHU,
        AUDIO_SPEAK_MJ_HUA_JU,
        AUDIO_SPEAK_BUHUA,
        AUDIO_SPEAK_HU,
        AUDIO_SPEAK_PENG,
        AUDIO_SPEAK_CHI,
        AUDIO_SPEAK_GANG,
        AUDIO_SPEAK_TING,
        AUDIO_SPEAK_ZIMO,
        AUDIO_BG_MUSIC,
        AUDIO_EFFECT_SHANDIAN,
        AUDIO_EFFECT_TUIDAO,
        AUDIO_EFFECT_DEAL,
        AUDIO_EFFECT_GIVE,
        AUDIO_EFFECT_DICE,
        AUDIO_EFFECT_FAPAI,
        AUDIO_EFFECT_SORTPAI,
        AUDIO_EFFECT_ZIMO,
        AUDIO_EFFECT_WINDY,
        AUDIO_EFFECT_RAINY,
        AUDIO_EFFECT_DOWN,
        AUDIO_EFFECT_FEIDINGQUE,
    }

    public enum SpritesType
    {
        DICE_MACHINE_TIMER_NUM = 0,
        SELECT,
        QUE_YI_MEN,
    }

    public enum SpriteIdx
    {
        BLUE_NUMS = 0,
        SELECT_NUMS,
        SELECT_NUMS_GREY,
        SELECT_QUE_TEXT,
        SELECT_DING_TEXT,
        SELECT_KUOHAO,
        QUE_HUA_SE,
        QUE_HUA_SE_MOVE,
        HUA_SE_FLAG,
    }

    public enum PrefabsType
    {
        DEFAULT = 0,
        EFFECT,
        MJPAI,
        UI
    }

    public enum PrefabIdx
    {
        MJ_WANG_1,
        MJ_WANG_2,
        MJ_WANG_3,
        MJ_WANG_4,
        MJ_WANG_5,
        MJ_WANG_6,
        MJ_WANG_7,
        MJ_WANG_8,
        MJ_WANG_9,
        MJ_TONG_1,
        MJ_TONG_2,
        MJ_TONG_3,
        MJ_TONG_4,
        MJ_TONG_5,
        MJ_TONG_6,
        MJ_TONG_7,
        MJ_TONG_8,
        MJ_TONG_9,
        MJ_TIAO_1,
        MJ_TIAO_2,
        MJ_TIAO_3,
        MJ_TIAO_4,
        MJ_TIAO_5,
        MJ_TIAO_6,
        MJ_TIAO_7,
        MJ_TIAO_8,
        MJ_TIAO_9,
        MJ_FENG_DONG,
        MJ_FENG_NAN,
        MJ_FENG_XI,
        MJ_FENG_BEI,
        MJ_ZFB_HONGZHONG,
        MJ_ZFB_FACAI,
        MJ_ZFB_BAIBAN,
        MJ_HUA_CHUN,
        MJ_HUA_XIA,
        MJ_HUA_QIU,
        MJ_HUA_DONG,
        MJ_HUA_MEI,
        MJ_HUA_LAN,
        MJ_HUA_ZHU,
        MJ_HUA_JU,
        MJ_SHADOW,
        MJ_SHADOW_SMALL,
        FEMALE_HAND,
        HAND_SHADOW_PLANE,
        HUPAI_GET_MJ_EFFECT,
        HUPAI_SHAN_DIAN_EFFECT,
        HUPAI_TEXT_EFFECT,
        CHIPAI_TEXT_EFFECT,
        PENGPAI_TEXT_EFFECT,
        GANGPAI_TEXT_EFFECT,
        TINGPAI_TEXT_EFFECT,
        ZIMO_TEXT_EFFECT,
        LONGJUANFENG_EFFECT,
        RAIN_EFFECT,
        PENGPAI_DUST_EFFECT,
        TOUXIANG_HEAD_FIRE_EFFECT,
        TOUXIANG_FIRE_EFFECT,
        UI_SPEED_LINE_EFFECT,
        UI_STAR_SHAN_EFFECT,
        SWAPPAI_TIPS_OPP_ARROW,
        SWAPPAI_TIPS_CLOCKWISE_ARROW,
        SWAPPAI_TIPS_ANTICLOCKWISE_ARROW,
        DICE_MACHINE,
        MJPAI_POINT,
        UI_SELECT_SWAP_HANDPAI,
        UI_SWAPPAI_TIPS,
        UI_HUPAI_TIPS,
        UI_HUPAI_DETAIL_TIPS,
        UI_CHIPAI_TIPS,
        UI_CHIPAI_DETAIL_TIPS,
        UI_TOUXIANG,
        UI_HUPAI_INHANDPAI_TIPS_ARROW,
        UI_PENG_BTN,
        UI_CHI_BTN,
        UI_GANG_BTN,
        UI_TING_BTN,
        UI_HU_BTN,
        UI_GUO_BTN,
        UI_CANCEL_BTN,
        UI_SCORE,
        UI_SPRITE,
        UI_HUASE_FLAG,
        UI_SELECT_QUEMEN,

    }


    /// <summary>
    /// 麻将机状态
    /// </summary>
    public enum MahjongMachineState
    { 
        /// <summary>
        /// 洗牌开始
        /// </summary>
        XIPAI_START,

        /// <summary>
        /// 洗牌结束
        /// </summary>
        XIPAI_END,

        /// <summary>
        /// 发牌开始
        /// </summary>
        FAPAI_START,

        FAPAI_FEN_SINGLE_DENGING,
        FAPAI_FEN_SINGLE_DENG_END,
        FAPAI_FEN_DENG_END,
        FAPAI_BUHUA,
        FAPAI_SORT,

        /// <summary>
        /// 发牌结束
        /// </summary>
        FAPAI_END,

        END,
    }


    /// <summary>
    /// 手部动作状态
    /// </summary>
    public enum HandActionState
    {
        /// <summary>
        /// 启动骰子器开始
        /// </summary>
        QIDONG_DICEMACHINE_START,

        QIDONG_DICEMACHINE_READY_FIRST_HAND,
        QIDONG_DICEMACHINE_MOVE_HAND_TO_DST_POS,
        QIDONG_DICEMACHINE_QIDONG,
        QIDONG_DICEMACHINE_TAIHAND,

        /// <summary>
        /// 结束启动骰子器
        /// </summary>
        QIDONG_DICEMACHINE_END,


        /// <summary>
        /// 交换牌开始
        /// </summary>
        SWAP_PAI_START,
        SWAP_PAI_READY_FIRST_HAND,
        SWAP_PAI_MOVE_HAND_TO_DST_POS,
        SWAP_PAI_CHUPAI,
        SWAP_PAI_CHUPAI_TAIHAND,
        SWAP_PAI_TAIHAND_END,
        SWAP_PAI_ROTATE,
 
        /// <summary>
        /// 交换牌结束
        /// </summary>
        SWAP_PAI_END,

        /// <summary>
        /// 摸牌开始
        /// </summary>
        MO_PAI_START,

        /// <summary>
        /// 摸牌结束
        /// </summary>
        MO_PAI_END,

        /// <summary>
        /// 开始打牌
        /// </summary>
        DA_PAI_START,

        DA_PAI_READY_FIRST_HAND,
        DA_PAI_MOVE_HAND_TO_DST_POS,
        DA_PAI_CHUPAI,
        DA_PAI_CHUPAI_ZHENGPAI,
        DA_PAI_CHUPAI_ZHENGPAI_ADJUSTPAI,
        DA_PAI_CHUPAI_TIAOZHENG_HAND,
        DA_PAI_CHUPAI_TIAOZHENG_HAND_MOVPAI1,
        DA_PAI_CHUPAI_MOVPAI2,
        DA_PAI_CHUPAI2_MOVPAI,
        DA_PAI_CHUPAI_TAIHAND,

        /// <summary>
        /// 打牌结束
        /// </summary>
        DA_PAI_END,

        /// <summary>
        /// 开始插牌
        /// </summary>
        CHA_PAI_START,

        CHA_PAI_READY_ZHUA_HAND_PAI,
        CHA_PAI_ZHUA_HAND_PAI,
        CHA_PAI_TI_HAND_PAI,
        CHA_PAI_TI_HAND_PAI_MOVE,
        CHA_PAI_PUTDOWNHAND,
        CHA_PAI_ADJUST_PAI,
        CHA_PAI_TAIHAND,

        /// <summary>
        /// 插牌结束
        /// </summary>
        CHA_PAI_END,

        /// <summary>
        /// 开始整理牌
        /// </summary>
        SORT_PAI_START,

        /// <summary>
        /// 整理牌结束
        /// </summary>
        SORT_PAI_END,


        /// <summary>
        /// 开始补花
        /// </summary>
        BUHUA_PAI_START,

        BUHUA_PAI_READY_FIRST_HAND,
        BUHUA_PAI_MOVE_HAND_TO_DST_POS,
        BUHUA_PAI_BU,
        BUHUA_PAI_GET_PAI,
        BUHUA_PAI_TAIHAND,

        /// <summary>
        /// 补花结束
        /// </summary>
        BUHUA_PAI_END,


        /// <summary>
        /// 开始胡牌
        /// </summary>
        HU_PAI_START,

        HU_PAI_READY_FIRST_HAND,
        HU_PAI_MOVE_HAND_TO_DST_POS,
        HU_PAI_HU,
        HU_PAI_GET_PAI,
        HU_PAI_TAIHAND,

        /// <summary>
        /// 胡牌结束
        /// </summary>
        HU_PAI_END,

        /// <summary>
        /// 开始碰吃杠牌
        /// </summary>
        PENG_CHI_GANG_PAI_START,

        PENG_CHI_GANG_PAI_READY_FIRST_HAND,
        PENG_CHI_GANG_PAI_MOVE_HAND_TO_DST_POS,
        PENG_CHI_GANG_PAI_PCG_PAI,
        PENG_CHI_GANG_PAI_MOVE_PAI,
        PENG_CHI_GANG_PAI_TAIHAND,

        /// <summary>
        /// 碰吃杠牌结束
        /// </summary>
        PENG_CHI_GANG_PAI_END,


        /// <summary>
        /// 推倒牌开始
        /// </summary>
        TUIDAO_PAI_START,

        TUIDAO_PAI_ZHUA_HAND_PAI,
        TUIDAO_PAI_BACK_MOVE_HAND_PAI,
        TUIDAO_PAI_TUIDAO_HAND_PAI,
        TUIDAO_PAI_TUIDAO_HAND_PAI_TAIHAND,

        /// <summary>
        /// 推倒牌结束
        /// </summary>
        TUIDAO_PAI_END,

        /// <summary>
        /// 选择牌开始
        /// </summary>
        SELECT_PAI_START,

        SELECT_PAI_READY_CLICK,
        SELECT_PAIING,
 
        /// <summary>
        /// 选择牌结束
        /// </summary>
        SELECT_PAI_END,


        /// <summary>
        /// 选择打牌开始
        /// </summary>
        SELECT_DA_PAI_START,

        SELECT_DA_PAI_READY_CLICK,
        SELECT_DA_PAIING,
        SELECT_DA_PAI_RESTORE,

        /// <summary>
        /// 选择打牌结束
        /// </summary>
        SELECT_DA_PAI_END,


        /// <summary>
        /// 选择交换牌开始
        /// </summary>
        SELECT_SWAP_PAI_START,

        SELECT_SWAP_PAI_SELECTTING,

        /// <summary>
        /// 选择交换牌结束
        /// </summary>
        SELECT_SWAP_PAI_END,


        /// <summary>
        /// 选择缺门开始
        /// </summary>
        SELECT_QUE_MEN_START,

        SELECT_QUE_MEN_SELECTTING,
        SELECT_QUE_MEN_SELECT_END,

        /// <summary>
        /// 选择缺门结束
        /// </summary>
        SELECT_QUE_MEN_END,


        /// <summary>
        /// 选择碰吃杠听胡牌开始
        /// </summary>
        SELECT_PCGTH_PAI_START,
        SELECT_PCGTH_PAI_SELECTTING,
        SELECT_PCGTH_PAI_SELECTTING_CHIPAI,
        SELECT_PCGTH_PAI_SELECT_TING_PAI_START,
        SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK,
        SELECT_PCGTH_PAI_SELECT_TING_PAIING,
        SELECT_PCGTH_PAI_SELECT_TING_PAI_RESTORE,
        SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END,

        /// <summary>
        /// 选择碰吃杠听胡牌结束
        /// </summary>
        SELECT_PCGTH_PAI_END,

        ACTION_END,
    }


    /// <summary>
    /// 交换牌提示状态
    /// </summary>
    public enum SwapPaiHintState
    {
        HINT_START,
        HINTTING,
        HINT_END
    }


    /// <summary>
    /// 骰子机状态
    /// </summary>
    public enum DiceMachineState
    {
        START_RUN,

        GAIZI_OPENING,
        GAIZI_OPEND,

        GAIZI_CLOSING,
        GAIZI_CLOSED,

        THROWING_DICE,
        THROWED_DICE,

        TIMING,
        TIMER_END,

        END_RUNNING,
        END_RUN,
    }

    public enum MahjongGameDir
    {
        MG_DIR_X,
        MG_DIR_Z
    }

    /// <summary>
    /// 交换牌方向
    /// </summary>
    public enum SwapPaiDirection
    {
        /// <summary>
        /// 顺时针
        /// </summary>
        CLOCKWISE,

        /// <summary>
        /// 逆时针
        /// </summary>
        ANTICLOCKWISE,

        /// <summary>
        /// 对面
        /// </summary>
        OPPOSITE,

    }

    /// <summary>
    /// 风位
    /// </summary>
    public enum FengWei
    {
        /// <summary>
        /// 东
        /// </summary>
        EAST,

        /// <summary>
        /// 南
        /// </summary>
        SOUTH,

        /// <summary>
        /// 西
        /// </summary>
        WEST,

        /// <summary>
        /// 北
        /// </summary>
        NORTH
    }

    public enum MahjongUpDown
    {
        MG_UP,
        MG_DOWN
    }

    /// <summary>
    /// 轴类型
    /// </summary>
    public enum Axis
    {
        X,
        Y,
        Z
    }

    /// <summary>
    /// 整理牌类型
    /// </summary>
    public enum SortPaiType
    {
        /// <summary>
        /// 靠左
        /// </summary>
        LEFT,

        /// <summary>
        /// 中间
        /// </summary>
        MIDDLE,
    }

    /// <summary>
    /// 动作组合编号
    /// </summary>
    public enum ActionCombineNum
    {
        DaPai1_TaiHand1,
        DaPai1_TaiHand2,
        DaPai1_MovPai1_TaiHand1,
        DaPai1_MovPai1_TaiHand2,
        DaPai1_ZhengPai_TaiHand,
        DaPai1_MovPai1_ZhengPai_TaiHand,
        DaPai2_MovPai_TaiHand1,
        DaPai2_MovPai_TaiHand2,
        DaPai3_TaiHand,
        FirstTaiHand2_DaPai4_TaiHand,
        DaPai5,
        ChaPai,
        PengPai,
        ChiPai,
        GangPai,
        HuPai,
        TuiDaoPai,
        QiDongDiceMachine,
        End
    }

    /// <summary>
    /// 碰吃杠听胡类型
    /// </summary>
    public enum PengChiGangTingHuType
    {
        /// <summary>
        /// 碰
        /// </summary>
        PENG,

        /// <summary>
        /// 胡
        /// </summary>
        HU,

        /// <summary>
        /// 吃
        /// </summary>
        CHI,

        /// <summary>
        /// 杠
        /// </summary>
        GANG,

        /// <summary>
        /// 听
        /// </summary>
        TING,

        /// <summary>
        /// 过
        /// </summary>
        GUO,

        /// <summary>
        /// 取消
        /// </summary>
        CANCEL,


    }

    public enum HandPaiType
    {
        /// <summary>
        /// 手牌
        /// </summary>
        HandPai,

        /// <summary>
        /// 摸牌
        /// </summary>
        MoPai,
    }

    public enum HandPaiAdjustDirection
    {
        /// <summary>
        /// 往手左方
        /// </summary>
        GoToHandLeftDir,

        /// <summary>
        /// 往手右方
        /// </summary>
        GoToHandRightDir
    }


    /// <summary>
    /// 碰吃杠牌类型
    /// </summary>
    public enum PengChiGangPaiType
    {
        /// <summary>
        /// 碰牌
        /// </summary>
        PENG,

        /// <summary>
        /// 吃牌
        /// </summary>
        CHI,

        /// <summary>
        /// 杠牌
        /// </summary>
        GANG,

        /// <summary>
        /// 暗杠
        /// </summary>
        AN_GANG,

        /// <summary>
        /// 补杠
        /// </summary>
        BU_GANG
    }

    /// <summary>
    /// 刮风下雨等特效
    /// </summary>
    public enum EffectFengRainEtcType
    {
        /// <summary>
        /// 无特效
        /// </summary>
        EFFECT_NONE = 0x0,

        /// <summary>
        /// 刮风
        /// </summary>
        EFFECT_FENG = 0x1,

        /// <summary>
        /// 下雨
        /// </summary>
        EFFECT_RAIN = 0x2,

    }


    /// <summary>
    /// 麻将的正反面
    /// </summary>
    public enum MahjongFaceSide
    {
        /// <summary>
        /// 前面
        /// </summary>
        Front,

        /// <summary>
        /// 后面
        /// </summary>
        Back

    }

    /// <summary>
    /// 用手方向
    /// </summary>
    public enum HandDirection
    {
        /// <summary>
        /// 左手
        /// </summary>
        LeftHand,

        /// <summary>
        /// 右手
        /// </summary>
        RightHand
    }


    /// <summary>
    /// 玩家类型
    /// </summary>
    public enum PlayerType
    {
        /// <summary>
        /// 男人
        /// </summary>
        MALE,

        /// <summary>
        /// 女人
        /// </summary>
        FEMALE,

        /// <summary>      
        /// 无类型
        /// </summary>
        NONE,
    }

    public class PengChiGangPaiPos
    {
        public Vector3 pos;
        public int layouyDirSeat;
    }


    public struct ActionDataInfo
    {
        public float speed;
        public float crossFadeNormalTime;
    }

    public struct ScreenFitInfo
    {
        public float screenAspect;
        public Vector3 camPosition;
        public Vector3 camEluerAngle;
        public float camFieldOfView;
        public float mjScale;
    }

    public struct HuaSeInfo
    {
        public int seatIdx;
        public MahjongHuaSe huaSe;
    }

    public class HandShadowPlaneInfo
    {
        public Material planeMat;
        public Vector2[] tiling = new Vector2[3];
        public Vector2[] offset = new Vector2[3];
        public Vector2[] shadowOrgPos = new Vector2[3];
    }

    public struct HuPaiTipsInfo
    {
        public MahjongFaceValue faceValue;
        public int fanAmount;
        public int zhangAmount;
    }


    public enum MahjongOpCode
    {
        /// <summary>
        /// 播放特效音
        /// </summary>
        PlayEffectAudio,

        /// <summary>
        /// 洗牌
        /// </summary>
        XiPai,

        /// <summary>
        /// 发牌
        /// </summary>
        FaPai,

        /// <summary>
        /// 轮转下一个玩家
        /// </summary>
        TurnNextPlayer,

        /// <summary>
        ///请求选择交换牌 
        /// </summary>
        ReqSelectSwapPai,

        /// <summary>
        ///请求选择缺门
        /// </summary>
        ReqSelectQueMen,

        /// <summary>
        ///请求选择出牌 
        /// </summary>
        ReqSelectDaPai,

        /// <summary>
        ///请求选择碰吃杠听胡牌 
        /// </summary>
        ReqSelectPCGTHPai,


        /// <summary>
        /// 显示换牌提示
        /// </summary>
        ShowSwapPaiHint,


        /// <summary>
        /// 隐藏换牌UI
        /// </summary>
        HideSwapPaiUI,

        /// <summary>
        /// 启动骰子器
        /// </summary>
        QiDongDiceMachine,

        /// <summary>
        /// 换牌组合
        /// </summary>
        SwapPaiGroup,

        /// <summary>
        /// 换牌
        /// </summary>
        SwapPai,

        /// <summary>
        /// 缺门
        /// </summary>
        QueMen,

        /// <summary>
        /// 摸牌
        /// </summary>
        MoPai,

        /// <summary>
        /// 打牌
        /// </summary>
        DaPai,

        /// <summary>
        /// 插牌
        /// </summary>
        ChaPai,

        /// <summary>
        /// 整理牌
        /// </summary>
        SortPai,

        /// <summary>
        /// 补花牌
        /// </summary>
        BuHuaPai,

        /// <summary>
        /// 胡牌
        /// </summary>
        HuPai,

        /// <summary>
        /// 碰吃杠牌
        /// </summary>
        PengChiGangPai,


        /// <summary>
        /// 推倒牌
        /// </summary>
        TuiDaoPai,


        /// <summary>
        /// 显示分数
        /// </summary>
        ShowScore,




    }
}
