using CoreDesgin;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ComponentDesgin;

namespace ActionDesgin
{
    /// <summary>
    /// 选择插牌动作
    /// </summary>
    public class ChaPaiAction: BaseHandAction
    {
        public static ChaPaiAction Instance { get; } = new ChaPaiAction();
        public override void Install()
        {
            mjMachineUpdater.Reg("ChaPai", ActionChaPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("ChaPai");
        }

        #region 插牌动作
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

            if (playerStateData[seatIdx].state != StateDataGroup.END ||
                desk.mjSeatHandPaiLists[seatIdx].Count == 0 || desk.mjSeatHandPaiLists[seatIdx][0] == null)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            if (orgPaiType == HandPaiType.HandPai)
            {
                if (orgPaiIdx >= desk.mjSeatHandPaiLists[seatIdx].Count)
                {
                    mjCmdMgr.RemoveCmd(opCmdNode);
                    return;
                }
            }
            else
            {
                if (orgPaiIdx >= desk.mjSeatMoPaiLists[seatIdx].Count)
                {
                    mjCmdMgr.RemoveCmd(opCmdNode);
                    return;
                }

            }

            if (dstHandPaiIdx >= desk.mjSeatHandPaiLists[seatIdx].Count)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            ChaPaiStateData stateData = playerStateData[seatIdx].GetComponent<ChaPaiStateData>();

            stateData.SetChaPaiData(handStyle, orgPaiIdx, dstHandPaiIdx, orgPaiType, adjustDirection, opCmdNode);
            playerStateData[seatIdx].SetState(ChaPaiStateData.CHA_PAI_START, Time.time, -1);
        }

        public void ActionChaPai()
        {
            for (int i = 0; i < Fit.playerCount; i++)
            {
                if (fit.isCanUseSeatPlayer[i])
                    ActionChaPai(i);
            }
        }
        void ActionChaPai(int seatIdx)
        {
            if (playerStateData[seatIdx].state < ChaPaiStateData.CHA_PAI_START ||
             playerStateData[seatIdx].state > ChaPaiStateData.CHA_PAI_END)
            {
                return;
            }

            if (seatIdx == 0)
            {
                if (Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
                    return;

                ActionChaPaiForSeat0();
                return;
            }

            ChaPaiStateData stateData = playerStateData[seatIdx].GetComponent<ChaPaiStateData>();

            if (Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
            {
                MoveHandShadowForChaPai(seatIdx, stateData);
                return;
            }

            int orgPaiIdx = stateData.orgPaiIdx;
            int dstHandPaiIdx = stateData.chaPaiDstHandPaiIdx;
            HandPaiType orgPaiType = stateData.chaPaiHandPaiType;
            HandPaiAdjustDirection adjustDirection = stateData.adjustDirection;
            PlayerType handStyle = stateData.handStyle;
            GameObject mj = stateData.curtAdjustHandPai;

            float waitTime = 0.3f;
            Animation anim = hands.GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);

            switch (playerStateData[seatIdx].state)
            {
                case ChaPaiStateData.CHA_PAI_START:
                    {
                        GameObject dstPai = desk.mjSeatHandPaiLists[seatIdx][dstHandPaiIdx];
                        stateData.dstHandPaiPostion = dstPai.transform.position;

                        GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);
                        ReadyZhuaHandPai(seatIdx, handStyle, HandDirection.RightHand, orgPaiIdx, orgPaiType);

                        anim.Play("zhuaHandPai");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "zhuaHandPai");
                        playerStateData[seatIdx].SetState(ChaPaiStateData.CHA_PAI_ZHUA_HAND_PAI, Time.time, waitTime);

                        stateData.handShadowAxis[0] = hands.GetHandShadowAxis(seatIdx, handStyle, HandDirection.RightHand, 2).transform;
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(true);
                        MoveHandShadowForChaPai(seatIdx, stateData);
                    }
                    break;


                case ChaPaiStateData.CHA_PAI_ZHUA_HAND_PAI:
                    {
                        if (orgPaiType == HandPaiType.HandPai)
                            mj = desk.mjSeatHandPaiLists[seatIdx][orgPaiIdx];
                        else
                            mj = desk.mjSeatMoPaiLists[seatIdx][orgPaiIdx];

                        if (mj == null)
                        {
                            playerStateData[seatIdx].SetState(ChaPaiStateData.CHA_PAI_END, Time.time, 0);
                            return;
                        }

                        fit.OffMjShadow(mj);
                        if (orgPaiType == HandPaiType.HandPai)
                            desk.mjSeatHandPaiLists[seatIdx][orgPaiIdx] = null;
                        else
                            desk.mjSeatMoPaiLists[seatIdx].RemoveAt(orgPaiIdx);

                        stateData.curtAdjustHandPai = mj;
                        GameObject bone = hands.GetHandBone(seatIdx, handStyle, HandDirection.RightHand, 0);
                        mj.transform.SetParent(bone.transform, true);

                        anim.Play("TiHandPai");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "TiHandPai");
                        playerStateData[seatIdx].SetState(ChaPaiStateData.CHA_PAI_TI_HAND_PAI, Time.time, waitTime);
                    }
                    break;

                case ChaPaiStateData.CHA_PAI_TI_HAND_PAI:
                    {
                        if (orgPaiType == HandPaiType.MoPai || orgPaiIdx != dstHandPaiIdx)
                        {
                            List<Vector3> handOffsetList = hands.GetDeskMjHandOffsetList(seatIdx, handStyle, HandDirection.RightHand);
                            Vector3 dstMjPos = desk.mjSeatHandPaiLists[seatIdx][dstHandPaiIdx].transform.position;
                            Vector3 endValue = dstMjPos + handOffsetList[(int)ActionCombineNum.ChaPai];
                            GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                            Dictionary<string, ActionDataInfo> actionDataDict = hands.GetHandActionDataDict(seatIdx, handStyle, HandDirection.RightHand);

                            if (actionDataDict != null)
                            {
                                ActionDataInfo info = actionDataDict["ChaPai_MovHand"];
                                waitTime = 1f / info.speed * 0.3f * info.crossFadeNormalTime;
                                hand.transform.DOMove(endValue, waitTime).SetEase(Ease.Linear);
                            }
                            else
                            {
                                hand.transform.DOMove(endValue, 0.3f).SetEase(Ease.Linear);
                                waitTime = 0.3f;
                            }

                            playerStateData[seatIdx].SetState(ChaPaiStateData.CHA_PAI_TI_HAND_PAI_MOVE, Time.time, waitTime);
                            break;
                        }

                        string actionName = null;
                        if (orgPaiIdx == dstHandPaiIdx && orgPaiType == HandPaiType.HandPai)
                            actionName = "PutDownHandPai1";
                        else
                            actionName = "PutDownHandPai2";

                        anim.Play(actionName);
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, actionName);
                        playerStateData[seatIdx].SetState(ChaPaiStateData.CHA_PAI_PUTDOWNHAND, Time.time, waitTime);
                    }
                    break;


                case ChaPaiStateData.CHA_PAI_TI_HAND_PAI_MOVE:
                    {
                        string actionName = null;
                        if (orgPaiIdx == dstHandPaiIdx && orgPaiType == HandPaiType.HandPai)
                            actionName = "PutDownHandPai1";
                        else
                            actionName = "PutDownHandPai2";

                        anim.Play(actionName);
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, actionName);
                        playerStateData[seatIdx].SetState(ChaPaiStateData.CHA_PAI_PUTDOWNHAND, Time.time, waitTime);
                    }
                    break;

                case ChaPaiStateData.CHA_PAI_PUTDOWNHAND:
                    {
                        AdjustPai(seatIdx, mj, dstHandPaiIdx, adjustDirection, playerStateData[seatIdx].stateLiveTime);
                        playerStateData[seatIdx].SetState(ChaPaiStateData.CHA_PAI_ADJUST_PAI, Time.time, playerStateData[seatIdx].stateLiveTime);
                    }
                    break;

                case ChaPaiStateData.CHA_PAI_ADJUST_PAI:
                    {
                        fit.OnMjShadow(mj, 0);

                        stateData.curtAdjustHandPai.transform.SetParent(desk.mjtableTransform, true);
                        stateData.curtAdjustHandPai.transform.position = stateData.dstHandPaiPostion;
                        stateData.curtAdjustHandPai.transform.eulerAngles = fit.GetSeatHandMjFitEulerAngles(seatIdx);
                        stateData.curtAdjustHandPai = null;

                        anim.Play("TaiHand");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "TaiHand");
                        playerStateData[seatIdx].SetState(ChaPaiStateData.CHA_PAI_TAIHAND, Time.time, waitTime);
                    }
                    break;

                case ChaPaiStateData.CHA_PAI_TAIHAND:
                    {
                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.RightHand);
                        playerStateData[seatIdx].SetState(ChaPaiStateData.CHA_PAI_END, Time.time, waitTime);
                    }
                    break;

                case ChaPaiStateData.CHA_PAI_END:
                    {
                        GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(false);
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(false);
                        playerStateData[seatIdx].state = StateDataGroup.END;

                        ProcessHandActionmjCmdMgr(seatIdx, stateData);
                    }
                    break;

            }
        }

        void ActionChaPaiForSeat0()
        {
            ChaPaiStateData stateData = playerStateData[0].GetComponent<ChaPaiStateData>();
            int orgPaiIdx = stateData.orgPaiIdx;
            int dstHandPaiIdx = stateData.chaPaiDstHandPaiIdx;
            HandPaiType orgPaiType = stateData.chaPaiHandPaiType;
            HandPaiAdjustDirection adjustDirection = stateData.adjustDirection;

            float waitTime = 0.4f;
            List<GameObject> mjHandPaiList = desk.mjSeatHandPaiLists[0];
            List<GameObject> mjMoPaiList = desk.mjSeatMoPaiLists[0];

            GameObject mj = stateData.curtAdjustHandPai;
            Vector3 dstPos = stateData.dstHandPaiPostion;


            float y = fit.GetCanvasHandMjSizeByAxis(Axis.Y);
            y += y / 3;

            switch (playerStateData[0].state)
            {
                case ChaPaiStateData.CHA_PAI_START:
                    {
                        if (orgPaiType == HandPaiType.HandPai)
                        {
                            mj = mjHandPaiList[orgPaiIdx];
                        }
                        else
                        {
                            mj = mjMoPaiList[orgPaiIdx];
                        }

                        if (mj == null)
                        {
                            playerStateData[0].SetState(ChaPaiStateData.CHA_PAI_END, Time.time, 0);
                            return;
                        }

                        GameObject dstPai = mjHandPaiList[dstHandPaiIdx];
                        stateData.dstHandPaiPostion = dstPai.transform.localPosition;

                        if (orgPaiType == HandPaiType.HandPai)
                        {
                            mjHandPaiList[orgPaiIdx] = null;
                        }
                        else
                        {
                            mjMoPaiList.RemoveAt(orgPaiIdx);
                        }


                        fit.OffMjShadow(mj);
                        stateData.curtAdjustHandPai = mj;
                        Vector3 endPos = mj.transform.localPosition;
                        endPos.y += y;
                        mj.transform.DOLocalMove(endPos, waitTime);
                        playerStateData[0].SetState(ChaPaiStateData.CHA_PAI_TI_HAND_PAI, Time.time, waitTime);
                    }
                    break;

                case ChaPaiStateData.CHA_PAI_TI_HAND_PAI:
                    {
                        if (orgPaiType == HandPaiType.MoPai || orgPaiIdx != dstHandPaiIdx)
                        {
                            Vector3 dstEulerAngles = new Vector3(mj.transform.localEulerAngles.x, mj.transform.localEulerAngles.y, mj.transform.localEulerAngles.z + 40f);
                            Vector3 dstPos2 = new Vector3(dstPos.x, dstPos.y + y, dstPos.z);
                            mj.transform.DOLocalMove(dstPos2, waitTime);
                            mj.transform.DOLocalRotate(dstEulerAngles, waitTime);
                            playerStateData[0].SetState(ChaPaiStateData.CHA_PAI_TI_HAND_PAI_MOVE, Time.time, waitTime);
                            break;

                        }

                        if (orgPaiIdx == dstHandPaiIdx && orgPaiType == HandPaiType.HandPai)
                        {
                            mj.transform.DOLocalMove(dstPos, waitTime);
                            playerStateData[0].SetState(ChaPaiStateData.CHA_PAI_PUTDOWNHAND, Time.time, waitTime / 3);
                        }
                        else
                        {
                            mj.transform.DOLocalRotate(fit.canvasHandMjFitEulerAngles, waitTime);
                            mj.transform.DOLocalMove(dstPos, waitTime).SetEase(Ease.InCirc);
                            playerStateData[0].SetState(ChaPaiStateData.CHA_PAI_PUTDOWNHAND, Time.time, waitTime / 3);
                        }
                    }
                    break;

                case ChaPaiStateData.CHA_PAI_TI_HAND_PAI_MOVE:
                    {
                        if (orgPaiIdx == dstHandPaiIdx && orgPaiType == HandPaiType.HandPai)
                        {
                            mj.transform.DOLocalMove(dstPos, waitTime);
                            playerStateData[0].SetState(ChaPaiStateData.CHA_PAI_PUTDOWNHAND, Time.time, waitTime / 3);
                        }
                        else
                        {
                            mj.transform.DOLocalRotate(fit.canvasHandMjFitEulerAngles, waitTime);
                            mj.transform.DOLocalMove(dstPos, waitTime).SetEase(Ease.InCirc);
                            playerStateData[0].SetState(ChaPaiStateData.CHA_PAI_PUTDOWNHAND, Time.time, waitTime / 3);
                        }
                    }
                    break;

                case ChaPaiStateData.CHA_PAI_PUTDOWNHAND:
                    {
                        AdjustPai(0, mj, dstHandPaiIdx, adjustDirection, waitTime / 3);
                        playerStateData[0].SetState(ChaPaiStateData.CHA_PAI_END, Time.time, waitTime / 3 * 2);
                    }
                    break;

                case ChaPaiStateData.CHA_PAI_END:
                    {
                        fit.OnMjShadow(mj);
                        stateData.curtAdjustHandPai.transform.localPosition = dstPos;
                        stateData.curtAdjustHandPai.transform.localEulerAngles = fit.canvasHandMjFitEulerAngles;
                        stateData.curtAdjustHandPai = null;

                        playerStateData[0].state = StateDataGroup.END;
                        ProcessHandActionmjCmdMgr(0, stateData);
                    }
                    break;
            }
        }


        float ReadyZhuaHandPai(int seatIdx, PlayerType handStyle, HandDirection handDir, int handPaiIdx, HandPaiType paiType)
        {
            GameObject mj;
            float tm = 0.3f;

            if (paiType == HandPaiType.HandPai)
                mj = desk.mjSeatHandPaiLists[seatIdx][handPaiIdx];
            else
                mj = desk.mjSeatMoPaiLists[seatIdx][handPaiIdx];

            if (mj == null)
                return 0;

            FitHandPoseForSeat(seatIdx, handStyle, handDir, ActionCombineNum.ChaPai);
            tm = MoveHandToDstOffsetPos(seatIdx, handStyle, handDir, mj.transform.position, ActionCombineNum.ChaPai);
            return tm;
        }


        void AdjustPai(int seatIdx, GameObject orgPai, int dstPaiIdx, HandPaiAdjustDirection adjustDirection, float tm)
        {
            List<GameObject> mjHandPaiList = desk.mjSeatHandPaiLists[seatIdx];
            GameObject dstPai = mjHandPaiList[dstPaiIdx];
            Vector3 dstPos = dstPai.transform.position;

            if (seatIdx == 0)
                dstPos = dstPai.transform.localPosition;

            mjHandPaiList.Insert(dstPaiIdx, orgPai);

            if (adjustDirection == HandPaiAdjustDirection.GoToHandLeftDir)
            {
                mjHandPaiList[dstPaiIdx + 1] = null;
                mjHandPaiList.Insert(dstPaiIdx, dstPai);
            }


            mjHandPaiList.RemoveAll(j => j == null);

            int dstMjIdx = 0;
            for (int i = 0; i < mjHandPaiList.Count; i++)
            {
                if (mjHandPaiList[i] == orgPai)
                {
                    dstMjIdx = i;
                }
            }

            Vector3 prevMjDstPos = dstPos;
            float mjSpacing = 0;

            if (adjustDirection == HandPaiAdjustDirection.GoToHandLeftDir)
            {
                switch (seatIdx)
                {
                    case 0:
                        {
                            float mjsize = fit.GetCanvasHandMjSizeByAxis(Axis.X);
                            for (int i = dstMjIdx - 1; i >= 0; i--)
                            {
                                Vector3 pos = mjHandPaiList[i].transform.localPosition;
                                float offset = pos.x + mjsize / 2 - (prevMjDstPos.x - mjsize / 2);

                                if (offset > -mjSpacing)
                                {
                                    offset += mjSpacing;
                                    pos.x -= offset;
                                    prevMjDstPos = pos;
                                    mjHandPaiList[i].transform.DOLocalMove(pos, tm);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        break;


                    case 1:
                        {
                            float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                            for (int i = dstMjIdx - 1; i >= 0; i--)
                            {
                                Vector3 pos = mjHandPaiList[i].transform.position;
                                float offset = pos.z + mjsize / 2 - (prevMjDstPos.z - mjsize / 2);

                                if (offset > -mjSpacing)
                                {
                                    offset += mjSpacing;
                                    pos.z -= offset;
                                    prevMjDstPos = pos;
                                    mjHandPaiList[i].transform.DOMove(pos, tm);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        break;

                    case 2:
                        {
                            float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                            for (int i = dstMjIdx - 1; i >= 0; i--)
                            {
                                Vector3 pos = mjHandPaiList[i].transform.position;
                                float offset = pos.x + mjsize / 2 - (prevMjDstPos.x - mjsize / 2);

                                if (offset > -mjSpacing)
                                {
                                    offset += mjSpacing;
                                    pos.x -= offset;
                                    prevMjDstPos = pos;
                                    mjHandPaiList[i].transform.DOMove(pos, tm);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        break;

                    case 3:
                        {
                            float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                            for (int i = dstMjIdx - 1; i >= 0; i--)
                            {
                                Vector3 pos = mjHandPaiList[i].transform.position;
                                float offset = pos.z - mjsize / 2 - (prevMjDstPos.z + mjsize / 2);

                                if (offset < mjSpacing)
                                {
                                    offset -= mjSpacing;
                                    pos.z -= offset;
                                    prevMjDstPos = pos;
                                    mjHandPaiList[i].transform.DOMove(pos, tm);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (seatIdx)
                {
                    case 0:
                        {
                            float mjsize = fit.GetCanvasHandMjSizeByAxis(Axis.X);
                            for (int i = dstMjIdx + 1; i < mjHandPaiList.Count; i++)
                            {
                                Vector3 pos = mjHandPaiList[i].transform.localPosition;
                                float offset = pos.x - mjsize / 2 - (prevMjDstPos.x + mjsize / 2);

                                if (offset < mjSpacing)
                                {
                                    offset -= mjSpacing;
                                    pos.x -= offset;
                                    prevMjDstPos = pos;
                                    mjHandPaiList[i].transform.DOLocalMove(pos, tm);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        break;

                    case 1:
                        {
                            float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                            for (int i = dstMjIdx + 1; i < mjHandPaiList.Count; i++)
                            {
                                Vector3 pos = mjHandPaiList[i].transform.position;
                                float offset = pos.z - mjsize / 2 - (prevMjDstPos.z + mjsize / 2);

                                if (offset < mjSpacing)
                                {
                                    offset -= mjSpacing;
                                    pos.z -= offset;
                                    prevMjDstPos = pos;
                                    mjHandPaiList[i].transform.DOMove(pos, tm);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        break;


                    case 2:
                        {
                            float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                            for (int i = dstMjIdx + 1; i < mjHandPaiList.Count; i++)
                            {
                                Vector3 pos = mjHandPaiList[i].transform.position;
                                float offset = pos.x - mjsize / 2 - (prevMjDstPos.x + mjsize / 2);

                                if (offset < mjSpacing)
                                {
                                    offset -= mjSpacing;
                                    pos.x -= offset;
                                    prevMjDstPos = pos;
                                    mjHandPaiList[i].transform.DOMove(pos, tm);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        break;

                    case 3:
                        {
                            float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                            for (int i = dstMjIdx + 1; i < mjHandPaiList.Count; i++)
                            {
                                Vector3 pos = mjHandPaiList[i].transform.position;
                                float offset = pos.z + mjsize / 2 - (prevMjDstPos.z - mjsize / 2);

                                if (offset > -mjSpacing)
                                {
                                    offset += mjSpacing;
                                    pos.z -= offset;
                                    prevMjDstPos = pos;
                                    mjHandPaiList[i].transform.DOMove(pos, tm);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        break;

                }
            }
        }

      

        #endregion




    }
}
