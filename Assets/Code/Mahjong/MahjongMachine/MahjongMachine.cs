
using Assets;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MahjongMachineNS
{
    public partial class MahjongMachine
    {
        public delegate void SelfSelectDaPaiEndDelegate(int selectPaiHandPaiIdx, HandPaiType paiType);
        public delegate void SelfSelectSwapPaiEndDelegate(int[] clickSelectHandPaiIdxs, int[] clickSelectMoPaiIdxs);
        public delegate void SelfSelectPCGTHPaiEndDelegate(PengChiGangTingHuType selectBtnType, int chiPaiMjValueIdx, int tingPaiHandPaiIdx, HandPaiType tingPaiType);
        public delegate void SelfSelectQueMenEndDelegate(MahjongHuaSe queHuaSe);

        public delegate void HandQiDongDiceMachineDelegate(int seatIdx, int dice1Point, int dice2Point);

        /// <summary>
        /// 当前屏幕玩家选择打牌回调
        /// </summary>
        public SelfSelectDaPaiEndDelegate SelfSelectDaPaiEnd = null;

        /// <summary>
        /// 当前屏幕玩家选择交换牌回调
        /// </summary>
        public SelfSelectSwapPaiEndDelegate SelfSelectSwapPaiEnd = null;

        /// <summary>
        /// 当前屏幕玩家选择碰吃杠听胡牌回调
        /// </summary>
        public SelfSelectPCGTHPaiEndDelegate SelfSelectPCGTHPaiEnd = null;

        /// <summary>
        /// 当前屏幕玩家选择缺门回调
        /// </summary>
        public SelfSelectQueMenEndDelegate SelfSelectQueMenEnd = null;

        /// <summary>
        /// 手部启动骰子器动作回调
        /// </summary>
        public HandQiDongDiceMachineDelegate HandQiDongDiceMachine = null;


        public MahjongAssets mjAssets;
        public MahjongGame mjGame;
        GameObject gameObject;
        bool isGameAllReady = false;

        public int uiLayer = LayerMask.NameToLayer("UI");
        public int defaultLayer = LayerMask.NameToLayer("Default");

        /// <summary>
        /// 麻将桌
        /// </summary>
        public Transform mjtableTransform;
        public Transform mjTablePositionHelperTransform;

        public Transform[] swapPaiControlerSeat = new Transform[4];

        public MahjongDiceMachine diceMachine = new MahjongDiceMachine();

        public MahjongPoint mjPoint = new MahjongPoint();

        /// <summary>
        /// 视口UI相关
        /// </summary>
        public Transform uiCanvasTransform;
        public RectTransform canvasRectTransform;
        public Transform uiPositionHelperTransform;

        public Transform canvasHandPaiTransform;
        public RectTransform canvasHandPaiRectTransform;

        public MahjongAssetsMgr mjAssetsMgr = new MahjongAssetsMgr();

        public UITouXiang uiTouXiang = new UITouXiang();
        UIHuPaiHandPaiIdxTipsArrow uiHuPaiTipsArrow = new UIHuPaiHandPaiIdxTipsArrow();

        UISwapPaiingTips uiSwapPaiingTips = new UISwapPaiingTips();
        UIHuPaiTips uiHuPaiTips = new UIHuPaiTips();
        UIChiPaiTips uiChiPaiTips = new UIChiPaiTips();
        UIPCGHTBtnMgr uiPcghtBtnMgr = new UIPCGHTBtnMgr();
        SwapPaiHintArrowEffect swapPaiHintArrowEffect = new SwapPaiHintArrowEffect();

        public UIScore uiScore = new UIScore();
        public UISelectQueMen uiSelectQueMen = new UISelectQueMen();
        public UISelectSwapHandPai uiSelectSwapHandPai = new UISelectSwapHandPai();

        TingData[] tingDatas;

        /// <summary>
        /// 摄像机
        /// </summary>
        public Transform cameraTransform;
        private AudioSource bgMusicAudioSource;
        MahjongMachineCmdList mjOpCmdList = new MahjongMachineCmdList();

        /// <summary>
        /// 桌子上最后打出的麻将
        /// </summary>
        GameObject lastDaPaiMj;

        static string[] shaderTexNames = { "_MainTex", "_MainTex1", "_MainTex2" };
        static string[] shaderAngNames = { "_Angle", "_Angle1", "_Angle2" };

        static string[] taiHandActionName = new string[]
        {
            "DaPai1EndTaiHand1",
            "DaPai1EndTaiHand2",
            "DaPai1EndMovPai1EndTaiHand1",
            "DaPai1EndMovPai1EndTaiHand2",
            "",
            "",
            "DaPai2EndMovPaiEndTaiHand1",
            "DaPai2EndMovPaiEndTaiHand2",
            "DaPai3EndTaiHand",
            "FirstTaiHand2EndDaPai4EndTaiHand",
            "FirstTaiHand1EndHuPaiEndTaiHand"
        };

        /// <summary>
        /// 麻将机状态数据
        /// </summary>
        MahjongMachineStateData mjMachineStateData = new MahjongMachineStateData();

        /// <summary>
        /// 玩家状态数据
        /// </summary>
        public PlayerStateData[] playerStateData = new PlayerStateData[4]
        {
            new PlayerStateData(),
            new PlayerStateData(),
            new PlayerStateData(),
            new PlayerStateData()
        };


        SwapPaiHintStateData swapPaiHintStateData = new SwapPaiHintStateData();


        GameObject[] mjDuiPai = new GameObject[145];
        MahjongUpDown[] mjDuiPaiUpDown = new MahjongUpDown[145];


        /// <summary>
        /// 桌面表面Y位置值
        /// </summary>
        const float deskFacePosY = 0.09f;


        /// <summary>
        /// 手牌麻将列表
        /// </summary>
         List<GameObject>[] mjSeatHandPaiLists = new List<GameObject>[5]
        {
             new List<GameObject>(20),
             new List<GameObject>(20),
             new List<GameObject>(20),
             new List<GameObject>(20),
             new List<GameObject>(20),
        };

        /// <summary>
        /// 摸牌麻将列表
        /// </summary>
        List<GameObject>[] mjSeatMoPaiLists = new List<GameObject>[4]
       {
            new List<GameObject>(20),
            new List<GameObject>(20),
            new List<GameObject>(20),
            new List<GameObject>(20),
       };

        
        /// <summary>
        /// 桌面已打牌麻将列表
        /// </summary>
        Dictionary<int, GameObject>[] deskDaPaiMjDicts = new Dictionary<int, GameObject>[4]
        {
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>()
        };


        /// <summary>
        /// 桌面已胡牌麻将列表
        /// </summary>
        Dictionary<int, GameObject>[] deskHuPaiMjDicts = new Dictionary<int, GameObject>[4]
        {
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>()
        };

        /// <summary>
        /// 桌面已碰牌牌麻将列表
        /// </summary>
        List<GameObject[]>[] deskPengPaiMjList = new List<GameObject[]>[4]
        {
             new List<GameObject[]>(),
             new List<GameObject[]>(),
             new List<GameObject[]>(),
             new List<GameObject[]>()
        };

        /// <summary>
        /// 桌面已碰牌牌麻将位置信息列表
        /// </summary>
        List<Vector3[]>[] deskPengPaiMjPosInfoList = new List<Vector3[]>[4]
        {
             new List<Vector3[]>(),
             new List<Vector3[]>(),
             new List<Vector3[]>(),
             new List<Vector3[]>()
        };


        /// <summary>
        /// 桌面已吃牌牌麻将列表
        /// </summary>
        List<GameObject[]>[] deskChiPaiMjList = new List<GameObject[]>[4]
        {
             new List<GameObject[]>(),
             new List<GameObject[]>(),
             new List<GameObject[]>(),
             new List<GameObject[]>()
        };

        /// <summary>
        /// 桌面已杠牌牌麻将列表
        /// </summary>
        List<GameObject[]>[] deskGangPaiMjList = new List<GameObject[]>[4]
        {
             new List<GameObject[]>(),
             new List<GameObject[]>(),
             new List<GameObject[]>(),
             new List<GameObject[]>()
        };

        /// <summary>
        /// 桌面麻将集索引，以麻将值为key分类
        /// </summary>
        Dictionary<MahjongFaceValue, List<GameObject>> deskGlobalMjPaiSetDict = new Dictionary<MahjongFaceValue, List<GameObject>>();

        /// <summary>
        ///被高亮的麻将值
        /// </summary>
        MahjongFaceValue highLightMjValue = MahjongFaceValue.MJ_UNKNOWN;

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
        /// 手牌初始位置列表
        /// </summary>
        List<Vector3>[] mjSeatHandPaiPosLists = new List<Vector3>[5]
        {
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
        };


        /// <summary>
        /// 打出麻将在桌面的位置布局
        /// </summary>
        List<List<Vector3>>[] mjSeatDeskPaiPosLists = new List<List<Vector3>>[4]
        {
             new List<List<Vector3>>(),
             new List<List<Vector3>>(),
             new List<List<Vector3>>(),
             new List<List<Vector3>>(),
        };

        /// <summary>
        /// 胡牌麻将在桌面上的位置布局
        /// </summary>
        Vector3[,,] mjSeatDeskHuPaiPosList;

        /// <summary>
        /// 胡牌位置桌面麻将的布局row数量
        /// </summary>
        int huPaiDeskPosMjLayoutRowCount = 1;

        /// <summary>
        /// 胡牌位置桌面麻将的布局col数量
        /// </summary>
        int huPaiDeskPosMjLayoutColCount = 4;



        /// <summary>
        /// 各个玩家在桌面的最后打牌位置序号
        /// </summary>
        Vector3Int[] curtDaPaiMjSeatDeskPosIdx = new Vector3Int[4]
        {
            new Vector3Int(0,-1, 0),
            new Vector3Int(0,-1, 0),
            new Vector3Int(0,-1, 0),
            new Vector3Int(0,-1, 0),
        };


        /// <summary>
        /// 各个玩家在桌面的最后胡牌位置序号
        /// </summary>
        int[] curtHuPaiMjSeatDeskPosIdx = new int[4]
        {
            -1,-1,-1,-1,
        };


        List<Vector3>[] mjToFemaleRHandOffsetList = new List<Vector3>[4]
        {
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
        };

        List<Vector3>[] mjToFemaleLHandOffsetList = new List<Vector3>[4]
       {
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
       };


        List<Vector3>[] mjToMaleRHandOffsetList = new List<Vector3>[4]
        {
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
        };

        List<Vector3>[] mjToMaleLHandOffsetList = new List<Vector3>[4]
       {
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
            new List<Vector3>(),
       };


        /// <summary>
        /// 按圈风位置顺位的座位号， 0号为当前圈风位号
        /// </summary>
        public int[] orderSeatIdx = new int[4];
        public FengWei[] seatFengWei = new FengWei[4];

        GameObject[] femaleRHands = new GameObject[4];
        GameObject[,] femaleRHandMjBone = new GameObject[3, 4];
        GameObject[,] femaleRHandShadow = new GameObject[4, 4];
        Animation[] femaleRHandsAnimation = new Animation[4];
        Dictionary<string, ActionDataInfo>[] femaleRHandsActionDataInfo = new Dictionary<string, ActionDataInfo>[4];

        GameObject[] femaleLHands = new GameObject[4];
        GameObject[,] femaleLHandMjBone = new GameObject[3, 4];
        GameObject[,] femaleLHandShadow = new GameObject[3, 4];
        Animation[] femaleLHandsAnimation = new Animation[4];
        Dictionary<string, ActionDataInfo>[] femaleLHandsActionDataInfo = new Dictionary<string, ActionDataInfo>[4];

        GameObject[] maleRHands = new GameObject[4];
        GameObject[,] maleRHandMjBone = new GameObject[3, 4];
        GameObject[,] maleRHandShadow = new GameObject[3, 4];
        Animation[] maleRHandsAnimation = new Animation[4];
        Dictionary<string, ActionDataInfo>[] maleRHandsActionDataInfo = new Dictionary<string, ActionDataInfo>[4];

        GameObject[] maleLHands = new GameObject[4];
        GameObject[,] maleLHandMjBone = new GameObject[3, 4];
        GameObject[,] maleLHandShadow = new GameObject[3, 4];
        Animation[] maleLHandsAnimation = new Animation[4];
        Dictionary<string, ActionDataInfo>[] maleLHandsActionDataInfo = new Dictionary<string, ActionDataInfo>[4];

        /// <summary>
        /// 麻将在手中打牌的起始动作位置
        /// </summary>
        Vector3[] mjDaPaiFirstHandPos;

        /// <summary>
        /// 麻将在手中打牌的起始动作位置(dapai5)
        /// </summary>
        Vector3[] mjDaPaiFirstHandPos2;

        /// <summary>
        /// 麻将在手中打牌的起始动作角度
        /// </summary>
        Vector3[] mjDaPaiFirstHandEulerAngles;

        /// <summary>
        /// 麻将在手中打牌的起始动作角度(dapai5)
        /// </summary>
        Vector3[] mjDaPaiFirstHandEulerAngles2;


        /// <summary>
        /// 各个手部动作在每给桌面麻将位置的出现概率点数
        /// 此三维数组，各列分别代表，麻将的行, 麻将的列, 手部动作编号
        /// </summary>
        float[,,] handActionPoints;

        /// <summary>
        /// 打牌动作按照概率点数量的随机分布位置列表
        /// </summary>
        List<ActionCombineNum>[,] pointActionCombineNumLists;


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
        Vector3 wallduiMjScale = new Vector3(1, 1, 1.2f);

        /// <summary>
        /// 麻将一墩的数量
        /// </summary>
        int mjDengCount = 4;

        int curtPaiDuiPos = 1;

        /// <summary>
        /// 使用的麻将牌总数量
        /// </summary>
        int mjPaiTotalCount = 144;

        /// <summary>
        /// 麻将手牌设定最大数量（只影响起手发牌数量）
        /// </summary>
        int mjHandCount = 14;

        /// <summary>
        /// 牌堆中剩余麻将牌张数(不需要手动赋值)
        /// </summary>
        int paiDuiRichCount = 0;

        /// <summary>
        /// 开局麻将分发牌速度
        /// </summary>
        float fenPaiSpeed = 0.05f;

        /// <summary>
        /// 开局麻将分发墩牌翻牌速度
        /// </summary>
        float fanPaiSpeed = 0.43f;

        /// <summary>
        /// 开局翻牌后整理牌的速度
        /// </summary>
        float fanPaiAfterSortPaiSpeed = 0.3f;


        /// <summary>
        /// 开局麻将分发墩牌翻牌角度
        /// </summary>
        float canvasFanPaiAngles = 180f;


        /// <summary>
        /// 开局翻牌后整理牌的角度
        /// </summary>
        float canvasFanPaiAfterSortAngles = 120f;

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
        int realPlayerCount = 4;


        /// <summary>
        /// 设置麻将托是否可以放置麻将（用于2人麻将或4人麻将的牌堆数设置）
        /// </summary>
        bool[] isSeatMjTuoCanDuiPai = new bool[] { true, true, true, true };

        /// <summary>
        /// 可以堆牌的麻将托数量，和isSeatMjTuoCanDuiPai同（不需要手动设置）
        /// </summary>
        int canDuiPaiMjTuoCount = 4;

        /// <summary>
        /// 麻将桌所打出牌的区域布局范围(代表了每行可以放多少个麻将)
        /// 比如 {6, 8, 12, 14} 或者{6, 6, 6, 6} ： 表示 一共在自己座位桌面区域可以放4行麻将，
        /// 第一行可以放6个麻将，第二行可以放8个，依次类推..
        /// </summary>
        int[] deskRowMjLayoutCounts = new int[]
        {
             6, 6, 6, 6
        };

        /// <summary>
        /// seat1，2，3位置摸牌距手牌最后一张牌的偏移距离
        /// </summary>
        float moPaiToHandPaiOffset = 0.01f;

        /// <summary>
        /// seat0位置摸牌距手牌最后一张牌的偏移距离
        /// </summary>
        float moPaiToHandPaiCanvasOffset = 100f;


        /// <summary>
        /// 碰吃杠牌之间的间隔
        /// </summary>
        float deskPengPaiSpacing = 0.005f;

        /// <summary>
        /// 各座位碰吃杠牌惯用手的方向
        /// </summary>
        HandDirection[] pengPaiHandDirSeat = new HandDirection[]
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
        Vector3[] pengPaiStartPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 各座位碰吃杠牌放置的当前位置
        /// </summary>
        Vector3[] pengPaiCurtPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 各座位交换牌中心位置
        /// </summary>
        Vector3[] swapPaiCenterPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };


        /// <summary>
        /// 各座位骰子器启动位置
        /// </summary>
        Vector3[] diceQiDongPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };


        /// <summary>
        /// 各座位碰吃杠听胡牌UI文字特效位置
        /// </summary>
        Vector3[] pcgthEffectTextPosSeat = new Vector3[]
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
        Vector3[] fengEffectPos = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 下雨特效位置
        /// </summary>
        Vector3[] rainEffectPos = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 桌面麻将托位置
        /// </summary>
        Vector3[] deskMjTuoPos = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero
        };

        #endregion

        /// <summary>
        /// 桌面麻各个将托名称
        /// </summary>
        string[] deskMjTuoName = new string[]
        {
            "desk_mjtuo_self",
            "desk_mjtuo_west",
            "desk_mjtuo_north",
            "desk_mjtuo_east"
        };

        /// <summary>
        /// 各座位手做完动作离屏目标位置
        /// </summary>
        Vector3[] handActionLevelScreenPosSeat = new Vector3[]
        {
            new Vector3(-0.223f, 0.05f,0.921f),
            new Vector3(0.99f, 0.05f, 0.143f),
            new Vector3(0.249f, 0.05f, -0.94f),
            new Vector3(-1.08f,0.05f,-0.204f),
        };

        /// <summary>
        /// 各座位手做完动作离屏目标位置2
        /// </summary>
        Vector3[] handActionLevelScreenPosSeat2 = new Vector3[]
        {
            new Vector3(-0.223f, 0.05f,1f),
            new Vector3(1.1f, 0.05f, 0.143f),
            new Vector3(0.249f, 0.05f, -1f),
            new Vector3(-1.2f,0.05f,-0.204f),
        };


        /// <summary>
        /// 碰吃杠粒子效果绕Y轴欧拉角度
        /// </summary>
        float[] pcgParticleEulerAnglesY = new float[] { 0, 90, 0, 90 };

        bool isStopedGame = false;
        bool isUseHandActionSpeedSettingFile = true;

        /// <summary>
        /// 是否使用玩家选择麻将时显示外边高亮框
        /// </summary>
        bool isUsePlayerSelectMjOutLine = false;


        GameObject premj;
        Vector3 premjSize;


        /// <summary>
        /// 点击选择手牌时，偏移高度
        /// </summary>
        float handPaiSelectOffsetHeight = 0;

        /// <summary>
        /// 声音类型
        /// </summary>
        string voiceType = "PT_SPEAK";
        string[] speakTypes;
        Dictionary<int, AudioClip[]>[] speakAudiosDicts;
        Dictionary<int, AudioClip[]> musicAudiosDict;
        Dictionary<int, AudioClip[]> effectAudiosDict;

        /// <summary>
        /// 是否开启人声
        /// </summary>
        bool onVoice = true;

        /// <summary>
        /// 是否开启背景音乐
        /// </summary>
        bool onBgMusic = true;
        bool isMusicPlaying = false;

        /// <summary>
        /// CanvasHand麻将的适配欧拉角
        /// </summary>
        Vector3 canvasHandMjFitEulerAngles = new Vector3(0, -180, 0);


        Vector3 planeSize;

        HandShadowPlaneInfo[] handShadowPlaneInfos = new HandShadowPlaneInfo[4]
        {
            new HandShadowPlaneInfo(),
            new HandShadowPlaneInfo(),
            new HandShadowPlaneInfo(),
            new HandShadowPlaneInfo()
        };

    }
}