using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ComponentDesgin
{ 
    public partial class Desk : MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Fit fit;
        Scene scene;


        /// <summary>
        /// 桌面麻各个将托名称
        /// </summary>
        public string[] deskMjTuoName = new string[]
        {
            "desk_mjtuo_self",
            "desk_mjtuo_west",
            "desk_mjtuo_north",
            "desk_mjtuo_east"
        };

        /// <summary>
        /// 麻将桌
        /// </summary>
        public Transform mjtableTransform;
        public Transform canvasHandPaiTransform;
        public RectTransform canvasHandPaiRectTransform;

        /// <summary>
        /// 桌子上最后打出的麻将
        /// </summary>
        public GameObject lastDaPaiMj;


        GameObject[] mjDuiPai = new GameObject[145];

        /// <summary>
        /// 指示牌堆麻将所在方位为上方还是下方
        /// </summary>
        public MahjongUpDown[] mjDuiPaiUpDown = new MahjongUpDown[145];

        /// <summary>
        /// 桌面表面Y位置值
        /// </summary>
        public const float deskFacePosY = 0.09f;


        /// <summary>
        /// 手牌麻将列表
        /// </summary>
        public List<GameObject>[] mjSeatHandPaiLists = new List<GameObject>[5]
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
        public List<GameObject>[] mjSeatMoPaiLists = new List<GameObject>[4]
       {
            new List<GameObject>(20),
            new List<GameObject>(20),
            new List<GameObject>(20),
            new List<GameObject>(20),
       };


        /// <summary>
        /// 桌面已打出的麻将牌列表
        /// </summary>
        public Dictionary<int, GameObject>[] deskDaPaiMjDicts = new Dictionary<int, GameObject>[4]
        {
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>()
        };


        /// <summary>
        /// 桌面已胡牌麻将列表
        /// </summary>
        public Dictionary<int, GameObject>[] deskHuPaiMjDicts = new Dictionary<int, GameObject>[4]
        {
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>(),
             new Dictionary<int, GameObject>()
        };

        /// <summary>
        /// 桌面已碰牌牌麻将列表
        /// </summary>
        public List<GameObject[]>[] deskPengPaiMjList = new List<GameObject[]>[4]
        {
             new List<GameObject[]>(),
             new List<GameObject[]>(),
             new List<GameObject[]>(),
             new List<GameObject[]>()
        };

        /// <summary>
        /// 桌面已碰牌牌麻将位置信息列表
        /// </summary>
        public List<Vector3[]>[] deskPengPaiMjPosInfoList = new List<Vector3[]>[4]
        {
             new List<Vector3[]>(),
             new List<Vector3[]>(),
             new List<Vector3[]>(),
             new List<Vector3[]>()
        };


        /// <summary>
        /// 桌面已吃牌牌麻将列表
        /// </summary>
        public List<GameObject[]>[] deskChiPaiMjList = new List<GameObject[]>[4]
        {
             new List<GameObject[]>(),
             new List<GameObject[]>(),
             new List<GameObject[]>(),
             new List<GameObject[]>()
        };

        /// <summary>
        /// 桌面已杠牌牌麻将列表
        /// </summary>
        public List<GameObject[]>[] deskGangPaiMjList = new List<GameObject[]>[4]
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
        public MahjongFaceValue highLightMjValue = MahjongFaceValue.MJ_UNKNOWN;


        /// <summary>
        /// 手牌初始位置列表
        /// </summary>
        public List<Vector3>[] mjSeatHandPaiPosLists = new List<Vector3>[5]
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
        public int huPaiDeskPosMjLayoutRowCount = 1;

        /// <summary>
        /// 胡牌位置桌面麻将的布局col数量
        /// </summary>
        public int huPaiDeskPosMjLayoutColCount = 4;



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

        /// <summary>
        /// 当前牌堆摸牌位置
        /// </summary>
        public int curtPaiDuiPos = 1;


        /// <summary>
        /// 牌堆中剩余麻将牌张数(不需要手动赋值)
        /// </summary>
        public int paiDuiRichCount = 0;
    }
}
