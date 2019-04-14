using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ComponentDesgin
{
    public partial class Hand: MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Desk desk;
        Scene scene;
        Fit fit;


        public static string[] taiHandActionName = new string[]
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
        /// 麻将在手中打牌的起始动作位置
        /// </summary>
        public Vector3[] mjDaPaiFirstHandPos;

        /// <summary>
        /// 麻将在手中打牌的起始动作位置(dapai5)
        /// </summary>
        public Vector3[] mjDaPaiFirstHandPos2;

        /// <summary>
        /// 麻将在手中打牌的起始动作角度
        /// </summary>
        public Vector3[] mjDaPaiFirstHandEulerAngles;

        /// <summary>
        /// 麻将在手中打牌的起始动作角度(dapai5)
        /// </summary>
        public Vector3[] mjDaPaiFirstHandEulerAngles2;


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
        /// 是否使用手部动作速度设置文件设置手部动作动画的速度
        /// </summary>
        bool isUseHandActionSpeedSettingFile = true;

        /// <summary>
        /// 各座位手做完动作离屏目标位置
        /// </summary>
        public Vector3[] handActionLeaveScreenPosSeat = new Vector3[]
        {
            new Vector3(-0.223f, 0.05f,0.921f),
            new Vector3(0.99f, 0.05f, 0.143f),
            new Vector3(0.249f, 0.05f, -0.94f),
            new Vector3(-1.08f,0.05f,-0.204f),
        };

        /// <summary>
        /// 各座位手做完动作离屏目标位置2
        /// </summary>
        public Vector3[] handActionLeaveScreenPosSeat2 = new Vector3[]
        {
            new Vector3(-0.223f, 0.05f,1f),
            new Vector3(1.1f, 0.05f, 0.143f),
            new Vector3(0.249f, 0.05f, -1f),
            new Vector3(-1.2f,0.05f,-0.204f),
        };


        /// 手阴影所在平面的尺寸
        /// </summary>
        public Vector3 planeSize;

        /// <summary>
        /// 手的阴影信息
        /// </summary>
        public HandShadowPlaneInfo[] handShadowPlaneInfos = new HandShadowPlaneInfo[4]
        {
            new HandShadowPlaneInfo(),
            new HandShadowPlaneInfo(),
            new HandShadowPlaneInfo(),
            new HandShadowPlaneInfo()
        };

        /// <summary>
        /// 手的阴影贴图名称
        /// </summary>
        public static string[] shaderTexNames = { "_MainTex", "_MainTex1", "_MainTex2" };

        /// <summary>
        /// 手的阴影贴图角度
        /// </summary>
        public static string[] shaderAngNames = { "_Angle", "_Angle1", "_Angle2" };
    }
}
