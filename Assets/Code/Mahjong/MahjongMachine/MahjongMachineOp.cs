using Assets;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace MahjongMachineNS
{
    public partial class MahjongMachine
    {
        #region 操作

        /// <summary>
        /// 洗牌
        /// </summary>
        public void XiPai(int dealerSeatIdx, FengWei fengWei, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            if (mjMachineStateData.state != MahjongMachineState.END)
            {
                RemoveCommonActionOpCmd(opCmdNode);
                return;
            }

            NewRound();

            mjMachineStateData.SetXiPaiData(dealerSeatIdx, fengWei, opCmdNode);
            mjMachineStateData.SetState(MahjongMachineState.XIPAI_START, Time.time, -1);
        }


        /// <summary>
        /// 启动骰子器
        /// </summary>
        /// <param name="seatIdx">玩家座号</param>
        public void QiDongDiceMachine(int seatIdx, int dice1Point = -1, int dice2Point = -1, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].playerHandActionState !=  HandActionState.ACTION_END)
            {
                RemoveHandActionOpCmd(seatIdx, opCmdNode);
                return;
            }

            playerStateData[seatIdx].SetQiDongDiceMachineData(dice1Point, dice2Point, opCmdNode);
            playerStateData[seatIdx].SetPlayerState(HandActionState.QIDONG_DICEMACHINE_START, Time.time, -1);
        }

        void QiDong(int seatIdx, int dice1Point = -1, int dice2Point = -1)
        {
            diceMachine.StartRun(dice1Point, dice2Point);
        }

        /// <summary>
        ///发牌
        /// </summary>
        /// <param name="startPaiIdx">在麻将堆中开始发牌的麻将位置号</param>
        public void FaPai(int startPaiIdx,
            List<MahjongFaceValue> mjHandSelfPaiFaceValueList,
            List<MahjongFaceValue> selfHuaPaiValueList,
            List<MahjongFaceValue> selfBuPaiValueList,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            if (mjMachineStateData.state != MahjongMachineState.END)
            {
                RemoveCommonActionOpCmd(opCmdNode);
                return;
            }

            playerStateData[0].handPaiValueList = mjHandSelfPaiFaceValueList;

            mjMachineStateData.SetFaPaiData(startPaiIdx, selfHuaPaiValueList, selfBuPaiValueList, opCmdNode);
            mjMachineStateData.SetState(MahjongMachineState.FAPAI_START, Time.time, -1);
        }

        /// <summary>
        /// 显示交换牌提示
        /// </summary>
        public void ShowSwapPaiHint(SwapPaiDirection swapPaiDir)
        {
            if (swapPaiHintStateData.state != SwapPaiHintState.HINT_END)
            {
                return;
            }

            swapPaiHintStateData.SetData(swapPaiDir);
            swapPaiHintStateData.SetState(SwapPaiHintState.HINT_START, Time.time, -1);
        }


        /// <summary>
        /// 交换牌
        /// </summary>
        /// <param name="fromSeatIdx"></param>
        /// <param name="toSeatIdx"></param>
        /// <param name="swapMjCount"></param>
        /// <param name="mjFaceValues"></param>
        /// <param name="fromSeatHandPaiIdx"></param>
        /// <param name="toSeatHandPaiIdx"></param>
        /// <param name="isShowBack"></param>
        /// <param name="opCmdNode"></param>
        public void SwapPai(
            int fromSeatIdx,
            int toSeatIdx,
            int swapMjCount,
            int[] toSeatHandPaiIdx,
            MahjongFaceValue[] mjHandPaiFaceValues,
            int[] fromSeatHandPaiIdx,
            MahjongFaceValue[] mjMoPaiFaceValues = null,
            int[] fromSeatMoPaiIdx = null,
            bool isShowBack = true,
            SwapPaiDirection swapDir = SwapPaiDirection.CLOCKWISE,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(fromSeatIdx);

            if (playerStateData[fromSeatIdx].playerHandActionState != HandActionState.ACTION_END ||
                mjSeatHandPaiLists[toSeatIdx].Count == 0 || mjSeatHandPaiLists[toSeatIdx][0] == null)
            {
                RemoveHandActionOpCmd(fromSeatIdx, opCmdNode);
                return;
            }


            if (mjHandPaiFaceValues == null)
            {
                mjHandPaiFaceValues = new MahjongFaceValue[swapMjCount];
                for (int i = 0; i < swapMjCount; i++)
                    mjHandPaiFaceValues[i] = MahjongFaceValue.MJ_ZFB_FACAI;
            }

            if (fromSeatHandPaiIdx == null)
            {
                fromSeatHandPaiIdx = Common.GetRandom(swapMjCount, 0, mjSeatHandPaiLists[fromSeatIdx].Count);
                Array.Sort(fromSeatHandPaiIdx);
            }

#if (DEBUG)
            string s = "seat" + fromSeatIdx + "交换牌到" + "seat" + toSeatIdx + ",发起交换seat的手牌idx:";
            string w = "";
            for (int i = 0; i < fromSeatHandPaiIdx.Length; i++)
                w += fromSeatHandPaiIdx[i] + ",";
            Debug.Log(s + w);

#endif


#if (DEBUG)
            if (fromSeatMoPaiIdx != null)
            {
                s = "seat" + fromSeatIdx + "交换牌到" + "seat" + toSeatIdx + ",发起交换seat的摸牌idx:";
                w = "";
                for (int i = 0; i < fromSeatMoPaiIdx.Length; i++)
                    w += fromSeatMoPaiIdx[i] + ",";
                Debug.Log(s + w);
            }
#endif

            if (toSeatHandPaiIdx == null)
            {
                int handPaiLen = mjSeatHandPaiLists[toSeatIdx].Count - mjHandPaiFaceValues.Length;

                if (mjMoPaiFaceValues != null)
                    handPaiLen -= mjMoPaiFaceValues.Length;

                toSeatHandPaiIdx = Common.GetRandom(swapMjCount, 0, handPaiLen);
                Array.Sort(toSeatHandPaiIdx);
            }

#if (DEBUG)
            s = "seat" + fromSeatIdx + "交换牌到" + "seat" + toSeatIdx + ",接受交换seat的手牌目标idx:";
            w = "";
            for (int i = 0; i < toSeatHandPaiIdx.Length; i++)
                w += toSeatHandPaiIdx[i] + ",";
            Debug.Log(s + w);

#endif

            float h = GetDeskMjSizeByAxis(Axis.Z);
            Vector3 orgPos = new Vector3(swapPaiCenterPosSeat[fromSeatIdx].x, deskFacePosY + h / 2, swapPaiCenterPosSeat[fromSeatIdx].z);

            playerStateData[fromSeatIdx].SetSwapPaiData(
                orgPos, toSeatIdx, mjHandPaiFaceValues,
                fromSeatHandPaiIdx, mjMoPaiFaceValues, fromSeatMoPaiIdx, toSeatHandPaiIdx,
                swapDir,
                isShowBack, opCmdNode);

            playerStateData[fromSeatIdx].SetPlayerState(HandActionState.SWAP_PAI_START, Time.time, -1);
        }

        /// <summary>
        /// 摸牌
        /// </summary>
        /// <param name="seatIdx">玩家座号</param>
        public void MoPai(int seatIdx, MahjongFaceValue mjFaceValue, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].playerHandActionState != HandActionState.ACTION_END ||
                    mjSeatHandPaiLists[seatIdx].Count == 0 || mjSeatHandPaiLists[seatIdx][0] == null)
            {
                RemoveHandActionOpCmd(seatIdx, opCmdNode);
                return;
            }


            FenMahjongPaiFromPaiDui(curtPaiDuiPos, 1);

            playerStateData[seatIdx].SetMoPaiData(mjFaceValue, opCmdNode);
            playerStateData[seatIdx].SetPlayerState(HandActionState.MO_PAI_START, Time.time, -1);

        }

        /// <summary>
        /// 打牌
        /// </summary>
        /// <param name="seatIdx">出牌玩家座号</param>
        /// <param name="paiIdx">牌号</param>
        /// <param name="paiType">牌类型（已有手牌还是摸过来的牌）</param>
        /// <param name="mjFaceValue">牌面值</param>
        /// <param name="handActionNum">手部动作编号</param>
        public void DaPai(int seatIdx, PlayerType handStyle,
            int paiIdx, HandPaiType paiType, MahjongFaceValue mjFaceValue,
            bool isJiaoTing,
            ActionCombineNum handActionNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].playerHandActionState != HandActionState.ACTION_END ||
                mjSeatHandPaiLists[seatIdx].Count == 0 || mjSeatHandPaiLists[seatIdx][0] == null)
            {
                RemoveHandActionOpCmd(seatIdx, opCmdNode);
                return;
            }

            if (paiIdx >= 0 &&
                ((paiIdx < mjSeatHandPaiLists[seatIdx].Count && paiType == HandPaiType.HandPai) ||
                 (paiIdx < mjSeatMoPaiLists[seatIdx].Count && paiType == HandPaiType.MoPai)))
            {
                if (paiType == HandPaiType.HandPai)
                {
                    if (paiIdx >= mjSeatHandPaiLists[seatIdx].Count)
                    {
                        RemoveHandActionOpCmd(seatIdx, opCmdNode);
                        return;
                    }

                    GameObject mj = mjSeatHandPaiLists[seatIdx][paiIdx];

                    if (mj == null)
                    {
                        RemoveHandActionOpCmd(seatIdx, opCmdNode);
                        return;
                    }

                    mjSeatHandPaiLists[seatIdx].RemoveAt(paiIdx);

                    if (seatIdx != 0)
                        mjAssetsMgr.PushMjToOtherHandMjPool(mj);
                    else
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mj);
                }
                else
                {
                    if (paiIdx >= mjSeatMoPaiLists[seatIdx].Count)
                    {
                        RemoveHandActionOpCmd(seatIdx, opCmdNode);
                        return;
                    }

                    GameObject mj = mjSeatMoPaiLists[seatIdx][paiIdx];

                    if (mj == null)
                    {
                        RemoveHandActionOpCmd(seatIdx, opCmdNode);
                        return;
                    }

                    mjSeatMoPaiLists[seatIdx].RemoveAt(paiIdx);

                    if (seatIdx != 0)
                        mjAssetsMgr.PushMjToOtherHandMjPool(mj);
                    else
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mj);
                }
            }

            NextDeskMjPos(seatIdx);
            Vector3Int mjposIdx = GetCurtDeskMjPosIdx(seatIdx);

            playerStateData[seatIdx].SetDaPaiData(handStyle, mjposIdx, mjFaceValue, isJiaoTing, handActionNum, opCmdNode);
            playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_START, Time.time, -1);

        }


        /// <summary>
        /// 插牌
        /// </summary>
        /// <param name="seatIdx">对应的玩家座号</param>
        /// <param name="orgPaiIdx">原始需要插牌的麻将号</param>
        /// <param name="dstHandPaiIdx">目标位置号</param>
        /// <param name="orgPaiType">需要插牌的麻将类型</param>
        /// <param name="adjustDirection">插排后手牌列表移动的方向</param>
        public void ChaPai(int seatIdx, PlayerType handStyle, int orgPaiIdx, int dstHandPaiIdx, HandPaiType orgPaiType,
            HandPaiAdjustDirection adjustDirection, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].playerHandActionState != HandActionState.ACTION_END ||
                mjSeatHandPaiLists[seatIdx].Count == 0 || mjSeatHandPaiLists[seatIdx][0] == null)
            {
                RemoveHandActionOpCmd(seatIdx, opCmdNode);
                return;
            }

            if (orgPaiType == HandPaiType.HandPai)
            {
                if (orgPaiIdx >= mjSeatHandPaiLists[seatIdx].Count)
                {
                    RemoveHandActionOpCmd(seatIdx, opCmdNode);
                    {
                        RemoveHandActionOpCmd(seatIdx, opCmdNode);
                        return;
                    }
                }
            }
            else
            {
                if (orgPaiIdx >= mjSeatMoPaiLists[seatIdx].Count)
                {
                    RemoveHandActionOpCmd(seatIdx, opCmdNode);
                    return;
                }

            }

            if (dstHandPaiIdx >= mjSeatHandPaiLists[seatIdx].Count)
            {
                RemoveHandActionOpCmd(seatIdx, opCmdNode);
                return;
            }

            playerStateData[seatIdx].SetChaPaiData(handStyle, orgPaiIdx, dstHandPaiIdx, orgPaiType, adjustDirection, opCmdNode);
            playerStateData[seatIdx].SetPlayerState(HandActionState.CHA_PAI_START, Time.time, -1);
        }

        /// <summary>
        /// 整理牌
        /// </summary>
        /// <param name="seatIdx">对应的玩家座号</param>
        public void SortPai(int seatIdx, SortPaiType sortPaiType = SortPaiType.LEFT, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].playerHandActionState != HandActionState.ACTION_END ||
                mjSeatHandPaiLists[seatIdx].Count == 0 || mjSeatHandPaiLists[seatIdx][0] == null)
            {
                RemoveHandActionOpCmd(seatIdx, opCmdNode);
                return;
            }

            playerStateData[seatIdx].SetSortPaiData(sortPaiType, opCmdNode);
            playerStateData[seatIdx].SetPlayerState(HandActionState.SORT_PAI_START, Time.time, -1);
        }

        /// <summary>
        /// 补花
        /// </summary>
        public void BuHua(int seatIdx, PlayerType handStyle, MahjongFaceValue buHuaPaiFaceValue,
            ActionCombineNum handActionNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].playerHandActionState != HandActionState.ACTION_END)
            {
                RemoveHandActionOpCmd(seatIdx, opCmdNode);
                return;
            }

            NextDeskHuPaiMjPos(seatIdx);
            int idx = GetCurtDeskHuPaiMjPosIdx(seatIdx);

            playerStateData[seatIdx].SetBuHuaPaiData(handStyle, idx, buHuaPaiFaceValue, handActionNum, opCmdNode);
            playerStateData[seatIdx].SetPlayerState(HandActionState.BUHUA_PAI_START, Time.time, -1);
        }


        /// <summary>
        /// 胡牌
        /// </summary>
        /// <param name="seatIdx">胡牌玩家座号</param>
        /// <param name="targetSeatIdx">所胡目标玩家座号，如果为-1,为自摸</param>
        /// <param name="targetMjIdx">目标胡牌麻将编号</param>
        /// <param name="huPaiFaceValue">胡牌麻将面值</param>
        /// <param name="handActionNum">手部动作编号</param>
        public void HuPai(int seatIdx, PlayerType handStyle, int targetSeatIdx, Vector3Int targetMjIdx, MahjongFaceValue huPaiFaceValue,
            ActionCombineNum handActionNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].playerHandActionState != HandActionState.ACTION_END)
            {
                RemoveHandActionOpCmd(seatIdx, opCmdNode);
                return;
            }

            NextDeskHuPaiMjPos(seatIdx);
            int idx = GetCurtDeskHuPaiMjPosIdx(seatIdx);

            playerStateData[seatIdx].SetHuPaiData(handStyle, targetSeatIdx, targetMjIdx, idx, huPaiFaceValue, handActionNum, opCmdNode);
            playerStateData[seatIdx].SetPlayerState(HandActionState.HU_PAI_START, Time.time, -1);
        }

        /// <summary>
        /// 碰吃杠牌
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="handStyle"></param>
        /// <param name="isMoveHand"></param>
        /// <param name="moveHandDist"></param>
        /// <param name="faceValues"></param>
        /// <param name="actionCombineNum"></param>
        public void PengChiGangPai(
            int seatIdx, PlayerType handStyle, bool isMoveHand, float moveHandDist,
            MahjongFaceValue[] faceValues, PengChiGangPaiType pcgType,
            int targetSeatIdx, Vector3Int targetMjIdx,
            EffectFengRainEtcType fengRainEtcEffect,
            ActionCombineNum actionCombineNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].playerHandActionState != HandActionState.ACTION_END)
            {
                RemoveHandActionOpCmd(seatIdx, opCmdNode);
                return;
            }

            int idx = 0;
            if (pcgType == PengChiGangPaiType.PENG)
                idx = Random.Range(0, 3);
            else
                idx = Random.Range(0, 4);

            playerStateData[seatIdx].SetPengChiGangPaiData(
                handStyle,
                isMoveHand, moveHandDist,
                pcgType, faceValues, idx,
                targetSeatIdx, targetMjIdx,
                fengRainEtcEffect,
                actionCombineNum, opCmdNode);

            playerStateData[seatIdx].SetPlayerState(HandActionState.PENG_CHI_GANG_PAI_START, Time.time, -1);
        }

        /// <summary>
        /// 推倒牌
        /// </summary>
        /// <param name="seatIdx">对应的玩家座号</param>
        public void TuiDaoPai(int seatIdx, PlayerType handStyle, List<MahjongFaceValue> handPaiValueList,
            ActionCombineNum handActionNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].playerHandActionState != HandActionState.ACTION_END ||
                mjSeatHandPaiLists[seatIdx].Count == 0 || mjSeatHandPaiLists[seatIdx][0] == null)
            {
                RemoveHandActionOpCmd(seatIdx, opCmdNode);
                return;
            }

            playerStateData[seatIdx].SetTuiDaoPaiData(handStyle, handPaiValueList, handActionNum, opCmdNode);
            playerStateData[seatIdx].SetPlayerState(HandActionState.TUIDAO_PAI_START, Time.time, -1);
        }


        /// <summary>
        /// 选交换牌
        /// </summary>
        public void SelectSwapPai(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(0);

            if (playerStateData[0].playerHandActionState != HandActionState.ACTION_END)
            {
                RemoveHandActionOpCmd(0, opCmdNode);
                return;
            }

            playerStateData[0].SetSelectSwapPaiData(opCmdNode);
            playerStateData[0].SetPlayerState(HandActionState.SELECT_SWAP_PAI_START, Time.time, -1);
        }

        /// <summary>
        /// 选缺门
        /// </summary>
        public void SelectQueMen(MahjongHuaSe defaultQueMenHuaSe, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(0);

            if (playerStateData[0].playerHandActionState != HandActionState.ACTION_END)
            {
                RemoveHandActionOpCmd(0, opCmdNode);
                return;
            }

            playerStateData[0].SetSelectQueMenData(defaultQueMenHuaSe, opCmdNode);
            playerStateData[0].SetPlayerState(HandActionState.SELECT_QUE_MEN_START, Time.time, -1);
        }

        /// <summary>
        /// 选择打出牌
        /// </summary>
        public void SelectDaPai(
            int[] huPaiInHandPaiIdxs, List<HuPaiTipsInfo[]> huPaiInfosInHandPai,
            int[] huPaiInMoPaiIdxs, List<HuPaiTipsInfo[]> huPaiInfosInMoPai,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(0);

            if (playerStateData[0].playerHandActionState != HandActionState.ACTION_END)
            {
                RemoveHandActionOpCmd(0, opCmdNode);
                return;
            }

            if ((huPaiInHandPaiIdxs == null || huPaiInHandPaiIdxs.Length == 0) &&
                (huPaiInMoPaiIdxs == null || huPaiInMoPaiIdxs.Length == 0))
            {
                byte[] cards = new byte[34];

                CreateHandPaiCardList(ref cards);
                MahjongAssetsMgr.mjHuTingCheck.CheckTing(cards, 100, ref tingDatas);

                if (huPaiInfosInHandPai == null)
                    huPaiInfosInHandPai = new List<HuPaiTipsInfo[]>();

                if (huPaiInfosInMoPai == null)
                    huPaiInfosInMoPai = new List<HuPaiTipsInfo[]>();

                CreateHuPaiInfos(mjSeatHandPaiLists[0], tingDatas, ref huPaiInHandPaiIdxs, ref huPaiInfosInHandPai);
                CreateHuPaiInfos(mjSeatMoPaiLists[0], tingDatas, ref huPaiInMoPaiIdxs, ref huPaiInfosInMoPai);
            }

            playerStateData[0].SetSelectDaPaiData(huPaiInHandPaiIdxs, huPaiInfosInHandPai, huPaiInMoPaiIdxs, huPaiInfosInMoPai, opCmdNode);
            playerStateData[0].SetPlayerState(HandActionState.SELECT_DA_PAI_START, Time.time, -1);
        }

        /// <summary>
        /// 选择打牌结束回调
        /// </summary>
        /// <param name="selectPaiHandPaiIdx"></param>
        /// <param name="paiType"></param>
        void SelectDaPaiEnd(int selectPaiHandPaiIdx, HandPaiType paiType)
        {
            MahjongDaPaiOpCmd cmd = (MahjongDaPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.DaPai);
            cmd.seatIdx = 0;
            cmd.handStyle = PlayerType.FEMALE;
            cmd.paiIdx = selectPaiHandPaiIdx;
            cmd.paiType = paiType;
            cmd.mjFaceValue = GetHandPaiMjFaceValue(0, selectPaiHandPaiIdx);
            mjOpCmdList.Append(cmd);

            MahjongPaiOpCmd cmd2 = (MahjongPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.SortPai);
            cmd2.seatIdx = 0;
            cmd2.handStyle = PlayerType.FEMALE;
            cmd2.canSelectPaiAfterCmdEnd = true;
            mjOpCmdList.Append(cmd2);
        }

        /// <summary>
        /// 选碰吃杠听胡牌
        /// </summary>
        public void SelectPCGTHPai(
            PengChiGangTingHuType[] pcgthBtnTypes,
            List<MahjongFaceValue[]> chiPaiMjValueList,
            int[] tingPaiInHandPaiIdxs, List<HuPaiTipsInfo[]> tingPaiInfosInHandPai,
            int[] tingPaiInMoPaiIdxs, List<HuPaiTipsInfo[]> tingPaiInfosInMoPai,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(0);

            if (playerStateData[0].playerHandActionState != HandActionState.ACTION_END)
            {
                RemoveHandActionOpCmd(0, opCmdNode);
                return;
            }

            playerStateData[0].SetSelectPCGTHPaiData(pcgthBtnTypes, chiPaiMjValueList, tingPaiInHandPaiIdxs, tingPaiInfosInHandPai, tingPaiInMoPaiIdxs, tingPaiInfosInMoPai, opCmdNode);
            playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_START, Time.time, -1);
        }

        #endregion

        void StopSelectPaiActionState(int seatIdx)
        {
            if(playerStateData[seatIdx].playerHandActionState >= HandActionState.SELECT_PAI_START &&
                playerStateData[seatIdx].playerHandActionState <= HandActionState.SELECT_PAI_END)
            {
                playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;
            }
        }

        void RemoveHandActionOpCmd(int seatIdx, LinkedListNode<MahjongMachineCmd> opCmdNode)
        {
            mjOpCmdList.RemoveHandActionOpCmd(seatIdx, opCmdNode);
        }

        void RemoveCommonActionOpCmd(LinkedListNode<MahjongMachineCmd> opCmdNode)
        {
            mjOpCmdList.RemoveCommonActionOpCmd(opCmdNode);
        }

        public void PlayBgMusic()
        {
            bgMusicAudioSource.clip = GetMusicAudio((int)AudioIdx.AUDIO_BG_MUSIC, 1);
            bgMusicAudioSource.Play();
            isMusicPlaying = true;
        }

        public void OnBgMusic()
        {
            onBgMusic = true;
        }

        public void OffBgMusic()
        {
            bgMusicAudioSource.Stop();
            isMusicPlaying = false;
            onBgMusic = false;
        }

        
        void SelectSwapPaiEnd(int[] handPaiIdxs, int[] moPaiIdxs)
        {
            MahjongMachineCmd cmdx = CmdPool.Instance.CreateCmd(MahjongOpCode.HideSwapPaiUI);
            mjOpCmdList.Append(cmdx);

            ShowSwapPaiHintCmd showCmd = (ShowSwapPaiHintCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ShowSwapPaiHint);
            showCmd.isBlock = false;
            showCmd.swapPaiDirection = SwapPaiDirection.OPPOSITE;
            mjOpCmdList.Append(showCmd);

            MahjongSwapPaiGroupCmd cmd = (MahjongSwapPaiGroupCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.SwapPaiGroup);
            cmd.SwapHandPaiIdx = handPaiIdxs;
            cmd.SwapMoPaiIdx = moPaiIdxs;
            cmd.SwapDirection = SwapPaiDirection.OPPOSITE;
            cmd.TakeMjFaceValues = new MahjongFaceValue[] { MahjongFaceValue.MJ_TIAO_3, MahjongFaceValue.MJ_WANG_4, MahjongFaceValue.MJ_WANG_9 };
            cmd.delayExecuteTime = 1f;
            showCmd.Append(mjOpCmdList.CreateCmdNode(cmd));
        }

        void SelectQueMenEnd(MahjongHuaSe queMenHuaSe)
        {
            QueMenCmd cmd = (QueMenCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.QueMen);
            cmd.seatIdx = 1;
            cmd.queMenHuaSe = MahjongHuaSe.TIAO;
            mjOpCmdList.Append(cmd);

            cmd = (QueMenCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.QueMen);
            cmd.seatIdx = 2;
            cmd.queMenHuaSe = MahjongHuaSe.WANG;
            mjOpCmdList.Append(cmd);

            cmd = (QueMenCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.QueMen);
            cmd.seatIdx = 3;
            cmd.queMenHuaSe = MahjongHuaSe.TONG;
            mjOpCmdList.Append(cmd);
        }


       

        /// <summary>
        /// 新的一局
        /// </summary>
        void NewRound()
        {
            diceMachine.EndRun();
            ResetData();
        }

    }
}