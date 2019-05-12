
namespace ComponentDesgin
{
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

   

    public enum MahjongUpDown
    {
        MG_UP,
        MG_DOWN
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
    public enum PrefabIdx : int
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
        SCENE,
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
}
