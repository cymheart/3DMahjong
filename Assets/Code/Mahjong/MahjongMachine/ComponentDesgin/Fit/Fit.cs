using CoreDesgin;
using System;
using UnityEngine;

namespace ComponentDesgin
{
    /// <summary>
    /// 轴类型
    /// </summary>
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public partial class Fit: MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Desk desk;

        Transform uiPositionHelperTransform;
        Transform mjTablePositionHelperTransform;
        Transform swapPaiControler;

        /// <summary>
        /// 各座位碰吃杠牌惯用手的方向
        /// </summary>
        public HandDirection[] pengPaiHandDirSeat = new HandDirection[]
        {
            HandDirection.RightHand,
            HandDirection.RightHand,
            HandDirection.RightHand,
            HandDirection.RightHand
        };

        #region 设置位置

        /// <summary>
        /// 各座位碰吃杠牌放置的开始位置
        /// </summary>
        public Vector3[] pengPaiStartPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 各座位碰吃杠牌放置的当前位置
        /// </summary>
        public Vector3[] pengPaiCurtPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 各座位交换牌中心位置
        /// </summary>
        public Vector3[] swapPaiCenterPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };


        /// <summary>
        /// 各座位骰子器启动位置
        /// </summary>
        public Vector3[] diceQiDongPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };


        /// <summary>
        /// 各座位碰吃杠听胡牌UI文字特效位置
        /// </summary>
        public Vector3[] pcgthEffectTextPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 各座位UI分数显示位置
        /// </summary>
        public Vector3[] uiScorePosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 各座位UI头像显示位置
        /// </summary>
        public Vector3[] uiTouXiangPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };


        /// <summary>
        /// 各座位定缺移动花色的起始位置
        /// </summary>
        public Vector3[] huaSeStartPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 刮风特效位置
        /// </summary>
        public Vector3[] fengEffectPos = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 下雨特效位置
        /// </summary>
        public Vector3[] rainEffectPos = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 桌面麻将托位置
        /// </summary>
        public Vector3[] deskMjTuoPos = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero
        };


        public Transform[] swapPaiControlerSeat = new Transform[4];

        #endregion


        /// <summary>
        /// 胡牌位置桌面麻将的布局row数量
        /// </summary>
        public int huPaiDeskPosMjLayoutRowCount = 1;

        /// <summary>
        /// 胡牌位置桌面麻将的布局col数量
        /// </summary>
        public int huPaiDeskPosMjLayoutColCount = 4;


        /// <summary>
        /// 按圈风位置顺位的座位号， 0号为当前圈风位号
        /// </summary>
        public int[] orderSeatIdx = new int[4];
        public FengWei[] seatFengWei = new FengWei[4];


        /// <summary>
        /// 麻将一墩的数量
        /// </summary>
        public int mjDengCount = 4;


        /// <summary>
        /// 当前牌堆摸牌位置
        /// </summary>
        public int curtPaiDuiPos = 1;

        /// <summary>
        /// 使用的麻将牌总数量
        /// </summary>
        public int mjPaiTotalCount = 144;

        /// <summary>
        /// 麻将手牌设定最大数量（只影响起手发牌数量）
        /// </summary>
        public int mjHandCount = 14;

        /// <summary>
        /// 牌堆中剩余麻将牌张数(不需要手动赋值)
        /// </summary>
        public int paiDuiRichCount = 0;

        /// <summary>
        /// 开局麻将分发牌速度
        /// </summary>
        public float fenPaiSpeed = 0.05f;

        /// <summary>
        /// 开局麻将分发墩牌翻牌速度
        /// </summary>
        public float fanPaiSpeed = 0.43f;

        /// <summary>
        /// 开局翻牌后整理牌的速度
        /// </summary>
        public float fanPaiAfterSortPaiSpeed = 0.3f;


        /// <summary>
        /// 开局麻将分发墩牌翻牌角度
        /// </summary>
        public float canvasFanPaiAngles = 180f;


        /// <summary>
        /// 开局翻牌后整理牌的角度
        /// </summary>
        public float canvasFanPaiAfterSortAngles = 120f;

        /// <summary>
        /// 麻将游戏玩家最大数量（为固定值，依据4人一桌设计）
        /// </summary>
        public const int playerCount = 4;

        /// <summary>
        /// 设置玩家座位是否可用（用于2人麻将或4人麻将玩家位置设置）
        /// </summary>
        public bool[] isCanUseSeatPlayer = new bool[] { true, true, true, true };

        /// <summary>
        /// 真实玩家数量，和isCanUseSeatPlayer同（不需要手动设置）
        /// </summary>
        public int realPlayerCount = 4;


        /// <summary>
        /// 设置麻将托是否可以放置麻将（用于2人麻将或4人麻将的牌堆数设置）
        /// </summary>
        public bool[] isSeatMjTuoCanDuiPai = new bool[] { true, true, true, true };

        /// <summary>
        /// 可以堆牌的麻将托数量，和isSeatMjTuoCanDuiPai同（不需要手动设置）
        /// </summary>
        public int canDuiPaiMjTuoCount = 4;

        /// <summary>
        /// 麻将桌所打出牌的区域布局范围(代表了每行可以放多少个麻将)
        /// 比如 {6, 8, 12, 14} 或者{6, 6, 6, 6} ： 表示 一共在自己座位桌面区域可以放4行麻将，
        /// 第一行可以放6个麻将，第二行可以放8个，依次类推..
        /// </summary>
        public int[] deskRowMjLayoutCounts = new int[]
        {
             6, 6, 6, 6
        };

        /// <summary>
        /// seat1，2，3位置摸牌距手牌最后一张牌的偏移距离
        /// </summary>
        public float moPaiToHandPaiOffset = 0.01f;

        /// <summary>
        /// seat0位置摸牌距手牌最后一张牌的偏移距离
        /// </summary>
        public float moPaiToHandPaiCanvasOffset = 100f;


        /// <summary>
        /// 碰吃杠牌之间的间隔
        /// </summary>
        public float deskPengPaiSpacing = 0.005f;

        /// <summary>
        /// 碰吃杠粒子效果绕Y轴欧拉角度
        /// </summary>
        public float[] pcgParticleEulerAnglesY = new float[] { 0, 90, 0, 90 };

        bool isStopedGame = false;
        bool isUseHandActionSpeedSettingFile = true;

        /// <summary>
        /// 是否使用玩家选择麻将时显示外边高亮框
        /// </summary>
        public bool isUsePlayerSelectMjOutLine = false;

        /// <summary>
        /// 点击选择手牌时，偏移高度
        /// </summary>
        public float handPaiSelectOffsetHeight = 0;

        /// <summary>
        /// 麻将预制体
        /// </summary>
        GameObject premj;

        /// <summary>
        /// 麻将预制体的尺寸
        /// </summary>
        Vector3 premjSize;

        /// <summary>
        /// 桌面麻将相对于预制麻将缩放值
        /// </summary>
        Vector3 deskMjScale = new Vector3(1.3f, 1.3f, 1.3f);

        /// <summary>
        /// 桌面碰吃杠麻将相对于预制麻将缩放值
        /// </summary>
        Vector3 deskPcgMjScale = new Vector3(1.1f, 1.1f, 1.1f);

        /// <summary>
        /// Canvas手牌麻将相对于预制麻将缩放值
        /// </summary>
        Vector3 canvasHandMjScale = new Vector3(1.4f, 1.4f, 1.4f);

        /// <summary>
        /// 墙堆麻将相对于预制麻将缩放值
        /// </summary>
        public Vector3 wallduiMjScale = new Vector3(1, 1, 1.2f);


        Vector3[] seatDeskMjEulerAngles = new Vector3[]
         {
                new Vector3(-90, 0, 0),
                new Vector3(-90, 90, 0),
                new Vector3(-90, 180, 0),
                new Vector3(-90, 180, 90),
         };

        Vector3[] seatBackDeskMjEulerAngles = new Vector3[]
        {
                new Vector3(90, 180, 0),
                new Vector3(90, 0, 90),
                new Vector3(90, 0, 0),
                new Vector3(90, 180, 90),
        };

        Vector3[] seatHandMjEulerAngles = new Vector3[]
          {
                new Vector3(0, 0, 0),
                new Vector3(0, 90, 0),
                new Vector3(0, 180, 0),
                new Vector3(0, 270, 0),
          };


        Vector3[] seatWallDuiMjEulerAngles = new Vector3[]
         {
                new Vector3(90, 180, 0),
                new Vector3(90, 90, 180),
                new Vector3(90, 0, 0),
                new Vector3(90, 90, 0),

         };

        /// <summary>
        /// CanvasHand麻将的适配欧拉角
        /// </summary>
        public Vector3 canvasHandMjFitEulerAngles = new Vector3(0, -180, 0);

    }
}
