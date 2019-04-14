using CoreDesgin;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ComponentDesgin;

namespace ActionDesgin
{
   
    /// <summary>
    /// 换牌动作
    /// </summary>
    public class SwapPaiAction : BaseHandAction
    {
        private static SwapPaiAction instance = null;
        public static SwapPaiAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new SwapPaiAction();
                return instance;
            }
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("SwapPai", ActionSwapPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("SwapPai");
        }

        #region 换牌动作
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

            if (playerStateData[fromSeatIdx].state != StateDataGroup.END ||
                desk.mjSeatHandPaiLists[toSeatIdx].Count == 0 || desk.mjSeatHandPaiLists[toSeatIdx][0] == null)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
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
                fromSeatHandPaiIdx = Common.GetRandom(swapMjCount, 0, desk.mjSeatHandPaiLists[fromSeatIdx].Count);
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
                int handPaiLen = desk.mjSeatHandPaiLists[toSeatIdx].Count - mjHandPaiFaceValues.Length;

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

            float h = fit.GetDeskMjSizeByAxis(Axis.Z);
            Vector3 orgPos = 
                new Vector3(preSettingHelper.swapPaiCenterPosSeat[fromSeatIdx].x,
                Desk.deskFacePosY + h / 2, preSettingHelper.swapPaiCenterPosSeat[fromSeatIdx].z);

            SwapPaiStateData stateData = playerStateData[fromSeatIdx].GetComponent<SwapPaiStateData>();

            stateData.SetSwapPaiData(
                orgPos, toSeatIdx, mjHandPaiFaceValues,
                fromSeatHandPaiIdx, mjMoPaiFaceValues, fromSeatMoPaiIdx, toSeatHandPaiIdx,
                swapDir,
                isShowBack, opCmdNode);

            playerStateData[fromSeatIdx].SetState(SwapPaiStateData.SWAP_PAI_START, Time.time, -1);
        }


        /// <summary>
        /// 换牌动作
        /// </summary>
        public void ActionSwapPai()
        {
            for (int i = 0; i < Fit.playerCount; i++)
            {
                if (fit.isCanUseSeatPlayer[i])
                    ActionSwapPai(i);
            }
        }
        void ActionSwapPai(int seatIdx)
        {
            if (playerStateData[seatIdx].state < SwapPaiStateData.SWAP_PAI_START ||
               playerStateData[seatIdx].state > SwapPaiStateData.SWAP_PAI_END)
            {
                return;
            }

            SwapPaiStateData stateData = playerStateData[seatIdx].GetComponent<SwapPaiStateData>();

            if (Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
            {
                MoveHandShadowForDaPai(seatIdx, stateData);
                return;
            }
    
            PlayerType handStyle = stateData.handStyle;
            Vector3 orgPos = stateData.swapPaiFromPos;
            ActionCombineNum actionCombineNum = ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand;

            float waitTime = 0.3f;
            Animation anim = hands.GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);
            float fadeTime = 0.06f;

            switch (playerStateData[seatIdx].state)
            {
                case SwapPaiStateData.SWAP_PAI_START:
                    {
                        List<GameObject> mjHandPaiList = desk.mjSeatHandPaiLists[seatIdx];
                        int[] fromIdx = stateData.swapPaiFromSeatPaiIdxs;

                        for (int i = 0; i < fromIdx.Length; i++)
                        {
                            if (seatIdx == 0)
                                mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mjHandPaiList[fromIdx[i]]);
                            else
                                mjAssetsMgr.PushMjToOtherHandMjPool(mjHandPaiList[fromIdx[i]]);

                            mjHandPaiList[fromIdx[i]] = null;
                        }

                        mjHandPaiList.RemoveAll(n => n == null);


                        if (stateData.swapPaiFromSeatMoPaiIdxs != null)
                        {
                            List<GameObject> mjMoPaiList = desk.mjSeatMoPaiLists[seatIdx];
                            int[] fromMoPaiIdx = stateData.swapPaiFromSeatMoPaiIdxs;

                            for (int i = 0; i < fromMoPaiIdx.Length; i++)
                            {
                                if (seatIdx == 0)
                                    mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mjMoPaiList[fromMoPaiIdx[i]]);
                                else
                                    mjAssetsMgr.PushMjToOtherHandMjPool(mjMoPaiList[fromMoPaiIdx[i]]);

                                mjMoPaiList[fromMoPaiIdx[i]] = null;
                            }

                            mjMoPaiList.RemoveAll(n => n == null);
                        }

                        GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);
                        waitTime = ReadyFirstHand(seatIdx, handStyle, HandDirection.RightHand, "DaPaiFirstHand");
                        stateData.handShadowAxis[0] = hands.GetHandShadowAxis(seatIdx, handStyle, HandDirection.RightHand, 0).transform;
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(true);
                        MoveHandShadowForDaPai(seatIdx, stateData);

                        playerStateData[seatIdx].SetState(SwapPaiStateData.SWAP_PAI_READY_FIRST_HAND, Time.time, waitTime);
                    }
                    break;

                case SwapPaiStateData.SWAP_PAI_READY_FIRST_HAND:
                    {
                        FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, actionCombineNum);
                        waitTime = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, orgPos, actionCombineNum);
                        playerStateData[seatIdx].SetState(SwapPaiStateData.SWAP_PAI_MOVE_HAND_TO_DST_POS, Time.time, waitTime);
                    }
                    break;

                case SwapPaiStateData.SWAP_PAI_MOVE_HAND_TO_DST_POS:
                    {
                        anim.CrossFade("FirstTaiHand2EndDaPai4", fadeTime);
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand2EndDaPai4");
                        playerStateData[seatIdx].SetState(SwapPaiStateData.SWAP_PAI_CHUPAI, Time.time, waitTime);
                    }
                    break;

                case SwapPaiStateData.SWAP_PAI_CHUPAI:
                    {
                        AudioClip clip = audio.GetEffectAudio((int)AudioIdx.AUDIO_EFFECT_GIVE);
                        AudioSource.PlayClipAtPoint(clip, orgPos);

                        MahjongFaceValue[] mjFaceValues = stateData.swapPaiFaceValues;

                        if (stateData.swapPaiMoPaiFaceValues != null)
                        {
                            int count = stateData.swapPaiFaceValues.Length + stateData.swapPaiMoPaiFaceValues.Length;
                            mjFaceValues = new MahjongFaceValue[count];

                            for (int i = 0; i < stateData.swapPaiFaceValues.Length; i++)
                                mjFaceValues[i] = stateData.swapPaiFaceValues[i];

                            int j = 0;
                            for (int i = stateData.swapPaiFaceValues.Length; i < count; i++)
                                mjFaceValues[i] = stateData.swapPaiMoPaiFaceValues[j++];
                        }

                        stateData.swapPaiRotControler = 
                            CreateSwapPaiGroup(seatIdx, orgPos, mjFaceValues, stateData.swapPaiDir, stateData.swapPaiIsShowBack);

                        anim.Play(Hand.taiHandActionName[(int)actionCombineNum]);
                        hands.MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, hands.handActionLeaveScreenPosSeat[seatIdx]);
                        playerStateData[seatIdx].SetState(SwapPaiStateData.SWAP_PAI_CHUPAI_TAIHAND, Time.time, waitTime);

                    }
                    break;

                case SwapPaiStateData.SWAP_PAI_CHUPAI_TAIHAND:
                    {
                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.RightHand, actionCombineNum);
                        playerStateData[seatIdx].SetState(SwapPaiStateData.SWAP_PAI_TAIHAND_END, Time.time, waitTime);
                    }
                    break;

                case SwapPaiStateData.SWAP_PAI_TAIHAND_END:
                    {
                        waitTime = 1f;

                        if (stateData.swapPaiDir == SwapPaiDirection.OPPOSITE)
                        {
                            Vector3 dstCenterPos = preSettingHelper.swapPaiCenterPosSeat[stateData.swapPaiToSeatIdx];
                            Vector3 dstpos = new Vector3(dstCenterPos.x, Desk.deskFacePosY + fit.GetDeskMjSizeByAxis(Axis.Z) / 2, dstCenterPos.z);
                            stateData.swapPaiRotControler.transform.DOMove(dstpos, waitTime);
                        }
                        else
                        {
                            stateData.swapPaiRotControler.transform.DORotate(new Vector3(0, stateData.swapPaiToSeatIdx * 90, 0), waitTime);
                        }

                        playerStateData[seatIdx].SetState(SwapPaiStateData.SWAP_PAI_ROTATE, Time.time, waitTime);
                    }

                    break;

                case SwapPaiStateData.SWAP_PAI_ROTATE:
                    {
                        Transform tf = stateData.swapPaiRotControler.transform;
                        GameObject go;
                        int count = tf.childCount;
                        for (int i = 0; i < count; i++)
                        {
                            go = tf.GetChild(0).gameObject;
                            mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(go);
                        }

                        GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(false);
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(false);

                        waitTime = 1f;

                        stateData.swapPaiToSeatTakeMjs =
                           TakeMjsInToHandPaiList(
                                stateData.swapPaiToSeatIdx,
                                stateData.swapPaiToSeatPaiIdxs,
                               stateData.swapPaiFaceValues,
                                waitTime);

                        playerStateData[seatIdx].SetState(SwapPaiStateData.SWAP_PAI_END, Time.time, waitTime);

                    }
                    break;

                case SwapPaiStateData.SWAP_PAI_END:
                    {
                        if (stateData.swapPaiToSeatIdx == 0)
                        {
                            int shadowIdx = stateData.swapPaiToSeatIdx == 0 ? 1 : 0;
                            for (int i = 0; i < stateData.swapPaiToSeatTakeMjs.Length; i++)
                            {
                                fit.OnMjShadow(stateData.swapPaiToSeatTakeMjs[i], shadowIdx);
                            }
                        }

                        playerStateData[seatIdx].state = StateDataGroup.END;
                        ProcessHandActionmjCmdMgr(seatIdx, stateData);
                    }
                    break;
            }

        }

        #endregion

        /// <summary>
        /// 生产交换牌组
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="atDeskPos"></param>
        /// <param name="mjFaceValues"></param>
        /// <param name="dir"></param>
        /// <param name="isShowBack"></param>
        /// <returns></returns>
        public GameObject CreateSwapPaiGroup(int seatIdx, Vector3 atDeskPos, MahjongFaceValue[] mjFaceValues, SwapPaiDirection dir, bool isShowBack = false)
        {
            float width = fit.GetDeskMjSizeByAxis(Axis.X);
            float y = Desk.deskFacePosY + fit.GetDeskMjSizeByAxis(Axis.Z) / 2;
            float totalWidth = mjFaceValues.Length * width;

            if (dir != SwapPaiDirection.OPPOSITE)
                preSettingHelper.swapPaiControlerSeat[seatIdx].position = new Vector3(0, 0, 0);
            else
                preSettingHelper.swapPaiControlerSeat[seatIdx].position = atDeskPos;

            preSettingHelper.swapPaiControlerSeat[seatIdx].localEulerAngles = new Vector3(0, seatIdx * 90, 0);

            switch (seatIdx)
            {
                case 0:
                    {
                        float curtpos = atDeskPos.x + totalWidth / 2 - width / 2;
                        GameObject mj;

                        for (int i = 0; i < mjFaceValues.Length; i++)
                        {
                            mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mj.layer = mjMachine.defaultLayer;
                            fit.FitSeatDeskMj(seatIdx, mj, true, 0.91f, isShowBack);
                            mj.transform.position = new Vector3(curtpos, y, atDeskPos.z);
                            mj.transform.SetParent(preSettingHelper.swapPaiControlerSeat[seatIdx], true);
                            curtpos -= width;
                        }
                    }
                    break;

                case 1:
                    {
                        float curtpos = atDeskPos.z - totalWidth / 2 + width / 2;
                        GameObject mj;

                        for (int i = 0; i < mjFaceValues.Length; i++)
                        {
                            mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mj.layer = mjMachine.defaultLayer;
                            fit.FitSeatDeskMj(seatIdx, mj, true, 0.91f, isShowBack);
                            mj.transform.position = new Vector3(atDeskPos.x, y, curtpos);
                            mj.transform.SetParent(preSettingHelper.swapPaiControlerSeat[seatIdx], true);
                            curtpos += width;
                        }
                    }
                    break;


                case 2:
                    {
                        float curtpos = atDeskPos.x - totalWidth / 2 + width / 2;
                        GameObject mj;

                        for (int i = 0; i < mjFaceValues.Length; i++)
                        {
                            mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mj.layer = mjMachine.defaultLayer;
                            fit.FitSeatDeskMj(seatIdx, mj, true, 0.91f, isShowBack);
                            mj.transform.position = new Vector3(curtpos, y, atDeskPos.z);
                            mj.transform.SetParent(preSettingHelper.swapPaiControlerSeat[seatIdx], true);
                            curtpos += width;
                        }
                    }
                    break;

                case 3:
                    {
                        float curtpos = atDeskPos.z + totalWidth / 2 - width / 2;
                        GameObject mj;

                        for (int i = 0; i < mjFaceValues.Length; i++)
                        {
                            mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mj.layer = mjMachine.defaultLayer;
                            fit.FitSeatDeskMj(seatIdx, mj, true, 0.91f, isShowBack);
                            mj.transform.position = new Vector3(atDeskPos.x, y, curtpos);
                            mj.transform.SetParent(preSettingHelper.swapPaiControlerSeat[seatIdx], true);
                            curtpos -= width;
                        }
                    }
                    break;

            }

            return preSettingHelper.swapPaiControlerSeat[seatIdx].gameObject;
        }


        /// <summary>
        /// 获取麻将牌组到手牌列表中
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="paiIdx"></param>
        /// <param name="mjFaceValues"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public GameObject[] TakeMjsInToHandPaiList(int seatIdx, int[] paiIdx, MahjongFaceValue[] mjFaceValues, float waitTime)
        {
            List<GameObject> mjHandPaiList = desk.mjSeatHandPaiLists[seatIdx];
            float mjSpacing = 0;
            float mjHeight = fit.GetHandMjSizeByAxis(Axis.Y);
            float mjWidth = fit.GetHandMjSizeByAxis(Axis.X);
            GameObject[] mjs = null;

            switch (seatIdx)
            {
                case 0:
                    {
                        float mjCount = mjHandPaiList.Count + paiIdx.Length;
                        mjHeight = fit.GetCanvasHandMjSizeByAxis(Axis.Y);
                        mjWidth = fit.GetCanvasHandMjSizeByAxis(Axis.X);

                        float mjWallLen = mjCount * mjWidth + (mjCount - 1) * mjSpacing;
                        float mjStartPos = -mjWallLen / 2 + mjWidth / 2;
                        float mjAxisSpacing = mjWidth + mjSpacing;
                        Vector3 mjRefPos;

                        if (mjHandPaiList.Count > 0)
                            mjRefPos = mjHandPaiList[0].transform.localPosition;
                        else
                            mjRefPos = desk.mjSeatHandPaiPosLists[seatIdx][0];


                        mjs = new GameObject[paiIdx.Length];
                        for (int i = 0; i < paiIdx.Length; i++)
                        {
                            mjs[i] = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mjs[i].layer = mjMachine.defaultLayer;
                            fit.FitSeatCanvasHandMj(mjs[i], false);
                            mjs[i].transform.SetParent(desk.canvasHandPaiTransform, true);
                        }

                        int idxOffset = 0;
                        for (int i = 0; i < paiIdx.Length; i++)
                        {
                            paiIdx[i] += idxOffset;
                            mjHandPaiList.Insert(paiIdx[i], mjs[i]);
                            idxOffset++;
                        }

                        int j = 0;
                        for (int i = 0; i < mjHandPaiList.Count; i++)
                        {
                            if (j < paiIdx.Length && i == paiIdx[j])
                            {
                                j++;
                                mjHandPaiList[i].transform.localPosition = new Vector3(mjStartPos, mjRefPos.y + mjHeight, mjRefPos.z);
                                mjHandPaiList[i].transform.DOLocalMoveY(mjRefPos.y, waitTime);
                            }
                            else
                            {
                                mjHandPaiList[i].transform.localPosition = new Vector3(mjStartPos, mjRefPos.y, mjRefPos.z);
                            }

                            mjStartPos += mjAxisSpacing;
                        }
                    }
                    break;


                case 1:
                case 3:
                    {
                        float mjCount = mjHandPaiList.Count + paiIdx.Length;

                        float mjWallLen = mjCount * mjWidth + (mjCount - 1) * mjSpacing;
                        float mjStartPos = -mjWallLen / 2 + mjWidth / 2 + desk.mjtableTransform.transform.position.z;
                        float mjAxisSpacing = 0;
                        Vector3 mjRefPos;

                        if (seatIdx == 1)
                        {
                            mjStartPos = -mjWallLen / 2 + mjWidth / 2 + desk.mjtableTransform.transform.position.z;
                            mjAxisSpacing = mjWidth + mjSpacing;
                        }
                        else
                        {
                            mjStartPos = mjWallLen / 2 - mjWidth / 2 + desk.mjtableTransform.transform.position.z;
                            mjAxisSpacing = -(mjWidth + mjSpacing);
                        }

                        if (mjHandPaiList.Count > 0)
                            mjRefPos = mjHandPaiList[0].transform.position;
                        else
                            mjRefPos = desk.mjSeatHandPaiPosLists[seatIdx][0];

                        mjs = new GameObject[paiIdx.Length];
                        for (int i = 0; i < paiIdx.Length; i++)
                        {
                            mjs[i] = mjAssetsMgr.PopMjFromOtherHandMjPool();
                            fit.FitSeatHandMj(seatIdx, mjs[i]);
                        }

                        int idxOffset = 0;
                        for (int i = 0; i < paiIdx.Length; i++)
                        {
                            paiIdx[i] += idxOffset;
                            mjHandPaiList.Insert(paiIdx[i], mjs[i]);
                            idxOffset++;
                        }

                        int j = 0;
                        for (int i = 0; i < mjHandPaiList.Count; i++)
                        {
                            if (j < paiIdx.Length && i == paiIdx[j])
                            {
                                j++;
                                mjHandPaiList[i].transform.position = new Vector3(mjRefPos.x, mjRefPos.y + mjHeight, mjStartPos);
                                mjHandPaiList[i].transform.DOMoveY(mjRefPos.y, waitTime);
                            }
                            else
                            {
                                mjHandPaiList[i].transform.position = new Vector3(mjRefPos.x, mjRefPos.y, mjStartPos);
                            }


                            mjStartPos += mjAxisSpacing;
                        }
                    }
                    break;


                case 2:
                    {
                        float mjCount = mjHandPaiList.Count + paiIdx.Length;

                        float mjWallLen = mjCount * mjWidth + (mjCount - 1) * mjSpacing;
                        float mjStartPos = -mjWallLen / 2 + mjWidth / 2 + desk.mjtableTransform.transform.position.x;
                        float mjAxisSpacing = mjWidth + mjSpacing;
                        Vector3 mjRefPos;

                        if (mjHandPaiList.Count > 0)
                            mjRefPos = mjHandPaiList[0].transform.position;
                        else
                            mjRefPos = desk.mjSeatHandPaiPosLists[seatIdx][0];


                        mjs = new GameObject[paiIdx.Length];
                        for (int i = 0; i < paiIdx.Length; i++)
                        {
                            mjs[i] = mjAssetsMgr.PopMjFromOtherHandMjPool();
                            fit.FitSeatHandMj(seatIdx, mjs[i]);
                        }

                        int idxOffset = 0;
                        for (int i = 0; i < paiIdx.Length; i++)
                        {
                            paiIdx[i] += idxOffset;
                            mjHandPaiList.Insert(paiIdx[i], mjs[i]);
                            idxOffset++;
                        }

                        int j = 0;
                        for (int i = 0; i < mjHandPaiList.Count; i++)
                        {
                            if (j < paiIdx.Length && i == paiIdx[j])
                            {
                                j++;
                                mjHandPaiList[i].transform.position = new Vector3(mjStartPos, mjRefPos.y + mjHeight, mjRefPos.z);
                                mjHandPaiList[i].transform.DOMoveY(mjRefPos.y, waitTime);
                            }
                            else
                            {
                                mjHandPaiList[i].transform.position = new Vector3(mjStartPos, mjRefPos.y, mjRefPos.z);
                            }

                            mjStartPos += mjAxisSpacing;
                        }
                    }
                    break;
            }

            return mjs;
        }
    }
}
