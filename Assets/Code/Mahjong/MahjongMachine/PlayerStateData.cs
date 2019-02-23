using System.Collections.Generic;
using UnityEngine;

namespace MahjongMachineNS
{
    /// <summary>
    /// 玩家状态数据
    public class PlayerStateData
    {
        /// <summary>
        /// 玩家当前手部动作状态
        /// </summary>
        public HandActionState playerHandActionState = HandActionState.ACTION_END;

        /// <summary>
        /// 玩家当前状态开始时间
        /// </summary>
        public float playerStateStartTime = 0;

        /// <summary>
        /// 玩家当前状态生存时间
        /// </summary>
        public float playerStateLiveTime = -1;

        public PlayerType handStyle = PlayerType.FEMALE;
        public ActionCombineNum actionCombineNum = ActionCombineNum.End;
        public Transform[] handShadowAxis = new Transform[2];
        public LinkedListNode<MahjongMachineCmd> opCmdNode;
        public List<MahjongFaceValue> handPaiValueList;


        #region 交换牌动作数据
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

        #region 打牌动作数据
        public Vector3Int mjPosIdx;
        public int daPaiHandPaiIdx;
        public HandPaiType daPaiHandPaiType = HandPaiType.HandPai;
        public MahjongFaceValue daPaiFaceValue = MahjongFaceValue.MJ_ZFB_FACAI;
        public GameObject curtHandReadyPutDeskPai;
        public bool isJiaoTing = false;
        #endregion

        #region 插牌动作数据
        public int orgPaiIdx;
        public int chaPaiDstHandPaiIdx;
        public HandPaiType chaPaiHandPaiType = HandPaiType.HandPai;
        public HandPaiAdjustDirection adjustDirection;
        public Vector3 dstHandPaiPostion = Vector3.zero;
        public MahjongFaceValue curtAdjustHandPaiFaceValue = MahjongFaceValue.MJ_UNKNOWN;
        public GameObject curtAdjustHandPai;
        #endregion

        #region 整理牌动作数据
        public SortPaiType sortPaiType = SortPaiType.LEFT;
        #endregion

        #region 摸牌动作数据
        public MahjongFaceValue moPaiFaceValue;
        public GameObject curtMoPaiMj;
        public int curtMoPaiMjShadowIdx = 0;
        #endregion

        #region 碰吃杠牌动作数据
        public bool pcgPaiIsMoveHand;
        public float pcgPaiMoveHandDist;
        public int pcgPaiTargetSeatIdx;
        public Vector3Int pcgPaiTargetMjIdx;
        public int pcgPaiTargetMjKey = -1;
        public PengChiGangPaiType pcgPaiType;
        public MahjongFaceValue[] pcgPaiMjfaceValues;
        public int pcgPaiLayoutIdx;
        public Vector3[] pcgDstStartPos;
        public Vector3[] pcgOrgStartPos;
        public GameObject[] pcgMjList;
        public int pcgMjIdx;
        public EffectFengRainEtcType fengRainEtcEffect;
        #endregion

        #region 补花动作数据
        public int buHuaPaiMjPosIdx;
        public MahjongFaceValue buHuaPaiFaceValue;
        #endregion

        #region 胡牌动作数据
        public int huPaiMjPosIdx;
        public int huPaiTargetSeatIdx;
        public Vector3Int huPaiTargetMjIdx;
        public int huPaiTargetMjKey = -1;
        public MahjongFaceValue huPaiFaceValue;
        #endregion

        #region 推倒牌动作数据
        #endregion

        #region 选择牌动作数据
        public PengChiGangTingHuType[] selectPcgthBtnTypes;
        public List<MahjongFaceValue[]> selectPcgthChiPaiMjValueList = null;
        public PengChiGangTingHuType selectPcgthedType;
        public int selectPcgthedChiPaiMjValueIdx;
        #endregion

        #region 选择缺门动作数据
        public MahjongHuaSe queMenHuaSe;
        public bool queMenIsPlayDownAudio = false;
        public bool queMenIsPlayFeiDingQueAudio = false;
        #endregion

        #region 选择打出麻将动作数据
        public GameObject rayPickMj;
        public Vector3 rayPickMjMouseOrgPos;
        public Vector3 rayPickMjOrgPos;
        public float rayPickMjLastKickTime = -1000;
        public float rayPickMjMoveDistPreDuration = 0;
        public GameObject selectPaiRayPickMj;
        public GameObject selectedUpMj;

        public int[] selectPaiHuPaiInHandPaiIdxs;
        public List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInHandPai;
        public int[] selectPaiHuPaiInMoPaiIdxs;
        public List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInMoPai;
        public int selectPaiHandPaiIdx;
        public HandPaiType selectPaiType;

        /// <summary>
        /// 射线点击拾取的麻将位置号列表
        /// </summary>
        public List<int> rayClickPickHandPaiMjIdxList = new List<int>();
        public List<int> rayClickPickMoPaiMjIdxList = new List<int>();
        #endregion

        #region 启动骰子机动作数据
        public int dice1Point = -1;
        public int dice2Point = -1;
        #endregion

        public void Clear()
        {
            handPaiValueList = null;
            actionCombineNum = ActionCombineNum.End;
        }

        /// <summary>
        /// 设置玩家状态
        /// </summary>
        /// <param name="startTime">状态开始时间</param>
        /// <param name="liveTime">状态生存时间</param>
        public void SetPlayerState(HandActionState handActionState, float startTime, float liveTime)
        {
            playerHandActionState = handActionState;
            playerStateStartTime = startTime;
            playerStateLiveTime = liveTime;
        }

        /// <summary>
        /// 设置打牌动作数据
        /// </summary>
        /// <param name="handStyle"></param>
        /// <param name="mjPosIdx"></param>
        /// <param name="mjFaceValue"></param>
        /// <param name="actionCombineNum"></param>
        /// <param name="opCmdNode"></param>
        public void SetDaPaiData(
            PlayerType handStyle, Vector3Int mjPosIdx, MahjongFaceValue mjFaceValue,
            bool isJiaoTing,
            ActionCombineNum actionCombineNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            this.mjPosIdx = mjPosIdx;
            this.daPaiFaceValue = mjFaceValue;
            this.isJiaoTing = isJiaoTing;
            this.actionCombineNum = actionCombineNum;
            this.opCmdNode = opCmdNode;
        }

        /// <summary>
        /// 设置插牌动作数据
        /// </summary>
        /// <param name="handStyle"></param>
        /// <param name="orgPaiIdx"></param>
        /// <param name="dstHandPaiIdx"></param>
        /// <param name="orgPaiType"></param>
        /// <param name="adjustDirection"></param>
        /// <param name="opCmdNode"></param>
        public void SetChaPaiData(
            PlayerType handStyle, int orgPaiIdx, int dstHandPaiIdx, HandPaiType orgPaiType,
            HandPaiAdjustDirection adjustDirection,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            this.orgPaiIdx = orgPaiIdx;
            this.chaPaiDstHandPaiIdx = dstHandPaiIdx;
            this.chaPaiHandPaiType = orgPaiType;
            this.adjustDirection = adjustDirection;
            this.opCmdNode = opCmdNode;
        }

        /// <summary>
        /// 设置整理牌动作数据
        /// </summary>
        /// <param name="sortPaiType"></param>
        /// <param name="opCmdNode"></param>
        public void SetSortPaiData(SortPaiType sortPaiType = SortPaiType.LEFT, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.sortPaiType = sortPaiType;
            this.opCmdNode = opCmdNode;
        }


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

        /// <summary>
        /// 设置摸牌动作数据
        /// </summary>
        /// <param name="mjFaceValue"></param>
        /// <param name="opCmdNode"></param>
        public void SetMoPaiData(MahjongFaceValue mjFaceValue, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.moPaiFaceValue = mjFaceValue;
            this.opCmdNode = opCmdNode;
        }

        /// <summary>
        /// 设置碰吃杠牌动作数据
        /// </summary>
        /// <param name="handStyle"></param>
        /// <param name="isMoveHand"></param>
        /// <param name="moveHandDist"></param>
        /// <param name="pcgPaiType"></param>
        /// <param name="mjfaceValues"></param>
        /// <param name="paiLayoutIdx"></param>
        /// <param name="targetSeatIdx"></param>
        /// <param name="targetMjIdx"></param>
        /// <param name="actionCombineNum"></param>
        /// <param name="opCmdNode"></param>
        public void SetPengChiGangPaiData(PlayerType handStyle, bool isMoveHand, float moveHandDist,
                PengChiGangPaiType pcgPaiType, MahjongFaceValue[] mjfaceValues, int paiLayoutIdx,
                int targetSeatIdx, Vector3Int targetMjIdx,
                EffectFengRainEtcType fengRainEtcEffect,
                ActionCombineNum actionCombineNum,
                LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            pcgPaiIsMoveHand = isMoveHand;
            pcgPaiMoveHandDist = moveHandDist;
            this.pcgPaiType = pcgPaiType;
            this.pcgPaiMjfaceValues = mjfaceValues;
            this.pcgPaiLayoutIdx = paiLayoutIdx;
            this.pcgPaiTargetSeatIdx = targetSeatIdx;
            this.pcgPaiTargetMjIdx = targetMjIdx;
            this.fengRainEtcEffect = fengRainEtcEffect;
            this.actionCombineNum = actionCombineNum;
            this.opCmdNode = opCmdNode;

        }

        /// <summary>
        /// 设置胡牌动作数据
        /// </summary>
        public void SetBuHuaPaiData(
            PlayerType handStyle, int buHuaPaiPos, MahjongFaceValue buHuaPaiFaceValue, ActionCombineNum actionCombineNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            buHuaPaiMjPosIdx = buHuaPaiPos;
            this.buHuaPaiFaceValue = buHuaPaiFaceValue;
            this.actionCombineNum = actionCombineNum;
            this.opCmdNode = opCmdNode;
        }


        /// <summary>
        /// 设置胡牌动作数据
        /// </summary>
        /// <param name="handStyle"></param>
        /// <param name="targetSeatIdx"></param>
        /// <param name="targetMjIdx"></param>
        /// <param name="huPaiPos"></param>
        /// <param name="huPaiFaceValue"></param>
        /// <param name="actionCombineNum"></param>
        /// <param name="opCmdNode"></param>
        public void SetHuPaiData(
            PlayerType handStyle, int targetSeatIdx, Vector3Int targetMjIdx,
            int huPaiPos, MahjongFaceValue huPaiFaceValue, ActionCombineNum actionCombineNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            huPaiMjPosIdx = huPaiPos;
            huPaiTargetSeatIdx = targetSeatIdx;
            huPaiTargetMjIdx = targetMjIdx;
            huPaiTargetMjKey = -1;
            this.huPaiFaceValue = huPaiFaceValue;
            this.actionCombineNum = actionCombineNum;
            this.opCmdNode = opCmdNode;
        }

        /// <summary>
        /// 设置推倒牌动作数据
        /// </summary>
        /// <param name="handStyle"></param>
        /// <param name="handPaiValueList"></param>
        /// <param name="actionCombineNum"></param>
        /// <param name="opCmdNode"></param>
        public void SetTuiDaoPaiData(
            PlayerType handStyle, List<MahjongFaceValue> handPaiValueList,
            ActionCombineNum actionCombineNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.handStyle = handStyle;
            this.actionCombineNum = actionCombineNum;
            this.handPaiValueList = handPaiValueList;
            this.opCmdNode = opCmdNode;
        }

        /// <summary>
        /// 设置选择交换牌动作数据
        /// </summary>
        /// <param name="opCmdNode"></param>
        public void SetSelectSwapPaiData(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.opCmdNode = opCmdNode;
        }

        /// <summary>
        /// 设置选择缺门动作数据
        /// </summary>
        /// <param name="opCmdNode"></param>
        public void SetSelectQueMenData(MahjongHuaSe defaultQueMenHuaSe, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.queMenHuaSe = defaultQueMenHuaSe;
            this.opCmdNode = opCmdNode;
        }



        /// <summary>
        /// 设置选择出牌动作数据
        /// </summary>
        /// <param name="selectPaiHuPaiInHandPaiIdxs"></param>
        /// <param name="selectPaiHuPaiInfosInHandPai"></param>
        /// <param name="selectPaiHuPaiInMoPaiIdxs"></param>
        /// <param name="selectPaiHuPaiInfosInMoPai"></param>
        /// <param name="opCmdNode"></param>
        public void SetSelectDaPaiData(
            int[] selectPaiHuPaiInHandPaiIdxs, List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInHandPai,
            int[] selectPaiHuPaiInMoPaiIdxs, List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInMoPai,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.selectPaiHuPaiInHandPaiIdxs = selectPaiHuPaiInHandPaiIdxs;
            this.selectPaiHuPaiInfosInHandPai = selectPaiHuPaiInfosInHandPai;
            this.selectPaiHuPaiInMoPaiIdxs = selectPaiHuPaiInMoPaiIdxs;
            this.selectPaiHuPaiInfosInMoPai = selectPaiHuPaiInfosInMoPai;
            this.opCmdNode = opCmdNode;
        }


        /// <summary>
        /// 设置选择碰吃杠听胡牌动作数据
        /// </summary>
        /// <param name="pcgthBtnTypes"></param>
        /// <param name="chiPaiMjValueList"></param>
        /// <param name="selectPaiHuPaiInHandPaiIdxs"></param>
        /// <param name="selectPaiHuPaiInfosInHandPai"></param>
        /// <param name="selectPaiHuPaiInMoPaiIdxs"></param>
        /// <param name="selectPaiHuPaiInfosInMoPai"></param>
        /// <param name="opCmdNode"></param>
        public void SetSelectPCGTHPaiData(
            PengChiGangTingHuType[] pcgthBtnTypes,
            List<MahjongFaceValue[]> chiPaiMjValueList,
            int[] selectPaiHuPaiInHandPaiIdxs, List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInHandPai,
            int[] selectPaiHuPaiInMoPaiIdxs, List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInMoPai,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            selectPcgthBtnTypes = pcgthBtnTypes;
            selectPcgthChiPaiMjValueList = chiPaiMjValueList;
            this.selectPaiHuPaiInHandPaiIdxs = selectPaiHuPaiInHandPaiIdxs;
            this.selectPaiHuPaiInfosInHandPai = selectPaiHuPaiInfosInHandPai;
            this.selectPaiHuPaiInMoPaiIdxs = selectPaiHuPaiInMoPaiIdxs;
            this.selectPaiHuPaiInfosInMoPai = selectPaiHuPaiInfosInMoPai;
            this.opCmdNode = opCmdNode;
        }

        /// <summary>
        /// 设置启动骰子机动作数据
        /// </summary>
        /// <param name="opCmdNode"></param>
        public void SetQiDongDiceMachineData(int dice1Point = -1, int dice2Point = -1, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.dice1Point = dice1Point;
            this.dice2Point = dice2Point;
            this.opCmdNode = opCmdNode;
        }

    }

}