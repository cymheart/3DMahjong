using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using ComponentDesgin;

namespace ActionDesgin
{

    /// <summary>
    /// 推倒牌动作
    /// </summary>
    public class TuiDaoPaiAction : BaseHandAction
    {
        public static TuiDaoPaiAction Instance { get; } = new TuiDaoPaiAction();
        public override void Install()
        {
            mjMachineUpdater.Reg("TuiDaoPai", ActionTuiDaoPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("TuiDaoPai");
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

            if (playerStateData[seatIdx].state != StateDataGroup.END ||
                desk.mjSeatHandPaiLists[seatIdx].Count == 0 || desk.mjSeatHandPaiLists[seatIdx][0] == null)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            TuiDaoPaiStateData stateData = playerStateData[seatIdx].GetComponent<TuiDaoPaiStateData>();

            stateData.SetTuiDaoPaiData(handStyle, handPaiValueList, handActionNum, opCmdNode);
            playerStateData[seatIdx].SetState(TuiDaoPaiStateData.TUIDAO_PAI_START, Time.time, -1);
        }

        #region 推倒牌动作
        public void ActionTuiDaoPai()
        {
            for (int i = 0; i < Fit.playerCount; i++)
            {
                if (fit.isCanUseSeatPlayer[i])
                    ActionTuiDaoPai(i);
            }
        }
        void ActionTuiDaoPai(int seatIdx)
        {
            if (playerStateData[seatIdx].state < TuiDaoPaiStateData.TUIDAO_PAI_START ||
                playerStateData[seatIdx].state > TuiDaoPaiStateData.TUIDAO_PAI_END)
            {
                return;
            }

            TuiDaoPaiStateData stateData = playerStateData[seatIdx].GetComponent<TuiDaoPaiStateData>();

            if ((seatIdx == 0 && desk.mjSeatHandPaiLists[seatIdx].Count == 0) ||
                stateData.handPaiValueList.Count == 0)
            {
                playerStateData[seatIdx].state = StateDataGroup.END;
                ProcessHandActionmjCmdMgr(seatIdx, stateData);
                return;
            }

            if (Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
            {
                MoveTuiDaoPaiHandShadow(seatIdx, stateData);
                return;
            }

            PlayerType handStyle = stateData.handStyle;

            float waitTime = 0.3f;
            Animation leftHandAnim = hands.GetHandAnimation(seatIdx, handStyle, HandDirection.LeftHand);
            Animation rightHandAnim = hands.GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);

            switch (playerStateData[seatIdx].state)
            {
                case TuiDaoPaiStateData.TUIDAO_PAI_START:
                    {
                        GameObject rightHand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        rightHand.SetActive(true);
                        ReadyZhuaHandPai2(stateData, seatIdx, handStyle);

                        rightHandAnim.Play("ZhuaHandPai2");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "ZhuaHandPai2");
                        stateData.handShadowAxis[1] = hands.GetHandShadowAxis(seatIdx, handStyle, HandDirection.RightHand, 0).transform;


                        if ((seatIdx == 0 && desk.mjSeatHandPaiLists[seatIdx].Count >= 3) ||
                           stateData.handPaiValueList.Count >= 3)
                        {
                            GameObject leftHand = hands.GetHand(seatIdx, handStyle, HandDirection.LeftHand);
                            leftHand.SetActive(true);
                            leftHandAnim.Play("ZhuaHandPai2");

                            stateData.handShadowAxis[0] = hands.GetHandShadowAxis(seatIdx, handStyle, HandDirection.LeftHand, 1).transform;
                        }

                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(true);
                        MoveTuiDaoPaiHandShadow(seatIdx, stateData);

                        playerStateData[seatIdx].SetState(TuiDaoPaiStateData.TUIDAO_PAI_ZHUA_HAND_PAI, Time.time, waitTime + 0.06f);
                    }
                    break;

                case TuiDaoPaiStateData.TUIDAO_PAI_ZHUA_HAND_PAI:
                    {
                        leftHandAnim.Play("ZhuaHandPai2EndBackMoveHandPai");
                        rightHandAnim.Play("ZhuaHandPai2EndBackMoveHandPai");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "ZhuaHandPai2EndBackMoveHandPai");

                        List<GameObject> mjHandPaiList = desk.mjSeatHandPaiLists[seatIdx];
                        List<MahjongFaceValue> handPaiValueList = stateData.handPaiValueList;
                        GameObject mjTmp;
                        Vector3 handPailastMjPos = desk.mjSeatHandPaiPosLists[seatIdx][0];

                        if (mjHandPaiList.Count != 0)
                            handPailastMjPos = mjHandPaiList[mjHandPaiList.Count - 1].transform.position;

                        switch (seatIdx)
                        {
                            case 0:
                                for (int i = 0; i < mjHandPaiList.Count; i++)
                                {
                                    mjHandPaiList[i].transform.DOLocalMoveZ(-200, waitTime).SetRelative();
                                    fit.OffMjShadow(mjHandPaiList[i]);
                                }
                                break;

                            case 2:

                                for (int i = 0; i < handPaiValueList.Count; i++)
                                {
                                    if (i < mjHandPaiList.Count)
                                    {
                                        mjTmp = mjHandPaiList[i];
                                        mjHandPaiList[i] = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(handPaiValueList[i]);
                                        mjHandPaiList[i].layer = mjMachine.defaultLayer;
                                        mjHandPaiList[i].transform.position = mjTmp.transform.position;
                                        mjHandPaiList[i].transform.eulerAngles = mjTmp.transform.eulerAngles;
                                        mjHandPaiList[i].transform.localScale = mjTmp.transform.localScale;
                                        mjHandPaiList[i].SetActive(true);
                                        mjAssetsMgr.PushMjToOtherHandMjPool(mjTmp);
                                        handPailastMjPos = mjHandPaiList[i].transform.position;
                                    }
                                    else
                                    {
                                        mjTmp = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(handPaiValueList[i]);
                                        mjTmp.layer = mjMachine.defaultLayer;
                                        fit.FitSeatHandMj(seatIdx, mjTmp, false);

                                        handPailastMjPos = new Vector3(
                                            handPailastMjPos.x + fit.GetHandMjSizeByAxis(Axis.X),
                                            mjTmp.transform.position.y, handPailastMjPos.z);

                                        mjTmp.transform.position = handPailastMjPos;
                                        mjHandPaiList.Add(mjTmp);
                                    }

                                    mjHandPaiList[i].transform.DOLocalMoveZ(-0.04f, waitTime).SetRelative();
                                }

                                break;

                            case 1:

                                for (int i = 0; i < handPaiValueList.Count; i++)
                                {
                                    if (i < mjHandPaiList.Count)
                                    {
                                        mjTmp = mjHandPaiList[i];
                                        mjHandPaiList[i] = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(handPaiValueList[i]);
                                        mjHandPaiList[i].layer = mjMachine.defaultLayer;
                                        mjHandPaiList[i].transform.position = mjTmp.transform.position;
                                        mjHandPaiList[i].transform.eulerAngles = mjTmp.transform.eulerAngles;
                                        mjHandPaiList[i].transform.localScale = mjTmp.transform.localScale;
                                        mjHandPaiList[i].SetActive(true);
                                        mjAssetsMgr.PushMjToOtherHandMjPool(mjTmp);
                                        handPailastMjPos = mjHandPaiList[i].transform.position;
                                    }
                                    else
                                    {
                                        mjTmp = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(handPaiValueList[i]);
                                        mjTmp.layer = mjMachine.defaultLayer;
                                        fit.FitSeatHandMj(seatIdx, mjTmp, false);
                                        handPailastMjPos = new Vector3(handPailastMjPos.x, mjTmp.transform.position.y, handPailastMjPos.z + fit.GetHandMjSizeByAxis(Axis.X));
                                        mjTmp.transform.position = handPailastMjPos;
                                        mjHandPaiList.Add(mjTmp);
                                    }

                                    mjHandPaiList[i].transform.DOLocalMoveX(0.04f, waitTime).SetRelative();
                                }

                                break;

                            case 3:

                                for (int i = 0; i < handPaiValueList.Count; i++)
                                {
                                    if (i < mjHandPaiList.Count)
                                    {
                                        mjTmp = mjHandPaiList[i];
                                        mjHandPaiList[i] = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(handPaiValueList[i]);
                                        mjHandPaiList[i].layer = mjMachine.defaultLayer;
                                        mjHandPaiList[i].transform.position = mjTmp.transform.position;
                                        mjHandPaiList[i].transform.eulerAngles = mjTmp.transform.eulerAngles;
                                        mjHandPaiList[i].transform.localScale = mjTmp.transform.localScale;
                                        mjHandPaiList[i].SetActive(true);
                                        mjAssetsMgr.PushMjToOtherHandMjPool(mjTmp);
                                        handPailastMjPos = mjHandPaiList[i].transform.position;
                                    }
                                    else
                                    {
                                        mjTmp = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(handPaiValueList[i]);
                                        mjTmp.layer = mjMachine.defaultLayer;
                                        fit.FitSeatHandMj(seatIdx, mjTmp, false);
                                        handPailastMjPos = new Vector3(handPailastMjPos.x, mjTmp.transform.position.y, handPailastMjPos.z - fit.GetHandMjSizeByAxis(Axis.X));
                                        mjTmp.transform.position = handPailastMjPos;
                                        mjHandPaiList.Add(mjTmp);
                                    }

                                    mjHandPaiList[i].transform.DOLocalMoveX(-0.04f, waitTime).SetRelative();
                                }

                                break;

                        }

                        if (seatIdx != 0)
                        {
                            for (int i = mjHandPaiList.Count - 1; i >= handPaiValueList.Count; i--)
                            {
                                mjAssetsMgr.PushMjToOtherHandMjPool(mjHandPaiList[mjHandPaiList.Count - 1]);
                                mjHandPaiList.RemoveAt(mjHandPaiList.Count - 1);
                            }
                        }

                        playerStateData[seatIdx].SetState(TuiDaoPaiStateData.TUIDAO_PAI_BACK_MOVE_HAND_PAI, Time.time, waitTime);
                    }
                    break;


                case TuiDaoPaiStateData.TUIDAO_PAI_BACK_MOVE_HAND_PAI:
                    {
                        leftHandAnim.Play("BackMoveHandPaiEndTuiDaoHandPai");
                        rightHandAnim.Play("BackMoveHandPaiEndTuiDaoHandPai");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "BackMoveHandPaiEndTuiDaoHandPai");

                        Vector3 pos;
                        List<GameObject> mjHandPaiList = desk.mjSeatHandPaiLists[seatIdx];
                        switch (seatIdx)
                        {
                            case 0:
                                for (int i = 0; i < mjHandPaiList.Count; i++)
                                {
                                    mjHandPaiList[i].transform.DOLocalRotate(new Vector3(desk.canvasHandPaiRectTransform.localEulerAngles.x - 90, 0, 0), waitTime).SetRelative();

                                    pos = desk.mjSeatHandPaiPosLists[4][i];
                                    //pos.z -=  GetHandMjSizeByAxis(Axis.Y) / 2;
                                    pos.y = Desk.deskFacePosY + fit.GetHandMjSizeByAxis(Axis.Z) / 2 + 0.0001f;
                                    mjHandPaiList[i].transform.DOMove(pos, waitTime + 0.01f);
                                }
                                break;

                            case 2:
                                for (int i = 0; i < mjHandPaiList.Count; i++)
                                {
                                    mjHandPaiList[i].transform.DOLocalRotate(new Vector3(-90, 0, 0), waitTime).SetRelative();

                                    pos = mjHandPaiList[i].transform.position;
                                    pos.z += fit.GetHandMjSizeByAxis(Axis.X) / 2 + fit.GetHandMjSizeByAxis(Axis.Y) / 2;
                                    pos.y = Desk.deskFacePosY + fit.GetHandMjSizeByAxis(Axis.Z) / 2 + 0.0001f;
                                    mjHandPaiList[i].transform.DOMove(pos, waitTime + 0.01f);
                                }
                                break;

                            case 1:

                                for (int i = 0; i < mjHandPaiList.Count; i++)
                                {
                                    mjHandPaiList[i].transform.DOLocalRotate(new Vector3(-90, 0, 0), waitTime).SetRelative();

                                    pos = mjHandPaiList[i].transform.position;
                                    pos.x -= fit.GetHandMjSizeByAxis(Axis.X) / 2 + fit.GetHandMjSizeByAxis(Axis.Y) / 2;
                                    pos.y = Desk.deskFacePosY + fit.GetHandMjSizeByAxis(Axis.Z) / 2 + 0.0001f;
                                    mjHandPaiList[i].transform.DOMove(pos, waitTime + 0.01f);
                                }
                                break;

                            case 3:
                                for (int i = 0; i < mjHandPaiList.Count; i++)
                                {
                                    mjHandPaiList[i].transform.DOLocalRotate(new Vector3(-90, 0, 0), waitTime).SetRelative();

                                    pos = mjHandPaiList[i].transform.position;
                                    pos.x += fit.GetHandMjSizeByAxis(Axis.X) / 2 + fit.GetHandMjSizeByAxis(Axis.Y) / 2;
                                    pos.y = Desk.deskFacePosY + fit.GetHandMjSizeByAxis(Axis.Z) / 2 + 0.0001f;
                                    mjHandPaiList[i].transform.DOMove(pos, waitTime + 0.01f);
                                }
                                break;
                        }

                        playerStateData[seatIdx].SetState(TuiDaoPaiStateData.TUIDAO_PAI_TUIDAO_HAND_PAI, Time.time, waitTime + 0.01f);
                    }
                    break;

                case TuiDaoPaiStateData.TUIDAO_PAI_TUIDAO_HAND_PAI:
                    {
                        List<GameObject> mjHandPaiList = desk.mjSeatHandPaiLists[seatIdx];

                        for (int i = 0; i < mjHandPaiList.Count; i++)
                        {
                            fit.OnMjShadow(mjHandPaiList[i], 0);
                        }

                        leftHandAnim.Play("TuiDaoHandPaiEndTaiHand");
                        rightHandAnim.Play("TuiDaoHandPaiEndTaiHand");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "TuiDaoHandPaiEndTaiHand");

                        playerStateData[seatIdx].SetState(TuiDaoPaiStateData.TUIDAO_PAI_TUIDAO_HAND_PAI_TAIHAND, Time.time, waitTime);
                    }
                    break;

                case TuiDaoPaiStateData.TUIDAO_PAI_TUIDAO_HAND_PAI_TAIHAND:
                    {
                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.RightHand, ActionCombineNum.End, 1);
                        HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.LeftHand, ActionCombineNum.End, 1);
                        playerStateData[seatIdx].SetState(TuiDaoPaiStateData.TUIDAO_PAI_END, Time.time, waitTime);
                    }
                    break;

                case TuiDaoPaiStateData.TUIDAO_PAI_END:
                    {
                        GameObject rightHand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        GameObject leftHand = hands.GetHand(seatIdx, handStyle, HandDirection.LeftHand);
                        leftHand.SetActive(false);
                        rightHand.SetActive(false);
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(false);

                        playerStateData[seatIdx].state = StateDataGroup.END;

                        ProcessHandActionmjCmdMgr(seatIdx, stateData);
                    }
                    break;
            }
        }

        void MoveTuiDaoPaiHandShadow(int seatIdx, TuiDaoPaiStateData stateData)
        {
            Transform handShadowRef;
            float ang;
            float matUOffset;
            float matVOffset;

            if ((seatIdx == 0 && desk.mjSeatHandPaiLists[seatIdx].Count >= 3) ||
                           stateData.handPaiValueList.Count >= 3)
            {
                handShadowRef = stateData.handShadowAxis[0];
                ang = 360 - handShadowRef.eulerAngles.y;

                matUOffset =
                    ((handShadowRef.position.x - hands.handShadowPlaneInfos[seatIdx].shadowOrgPos[1].x) / hands.planeSize.x) *
                    hands.handShadowPlaneInfos[seatIdx].tiling[1].x + hands.handShadowPlaneInfos[seatIdx].offset[1].x;

                matVOffset =
                    ((handShadowRef.position.z - hands.handShadowPlaneInfos[seatIdx].shadowOrgPos[1].y) / hands.planeSize.z) *
                    hands.handShadowPlaneInfos[seatIdx].tiling[1].y + hands.handShadowPlaneInfos[seatIdx].offset[1].y;

                hands.handShadowPlaneInfos[seatIdx].planeMat.SetTextureOffset(Hand.shaderTexNames[1], new Vector2(matUOffset, matVOffset));
                hands.handShadowPlaneInfos[seatIdx].planeMat.SetFloat(Hand.shaderAngNames[1], ang);
            }


            handShadowRef = stateData.handShadowAxis[1];
            ang = 360 - handShadowRef.eulerAngles.y;

            matUOffset = ((handShadowRef.position.x - hands.handShadowPlaneInfos[seatIdx].shadowOrgPos[0].x) / hands.planeSize.x) *
                hands.handShadowPlaneInfos[seatIdx].tiling[0].x + hands.handShadowPlaneInfos[seatIdx].offset[0].x;

            matVOffset = ((handShadowRef.position.z - hands.handShadowPlaneInfos[seatIdx].shadowOrgPos[0].y) / hands.planeSize.z) *
                hands.handShadowPlaneInfos[seatIdx].tiling[0].y + hands.handShadowPlaneInfos[seatIdx].offset[0].y;

            hands.handShadowPlaneInfos[seatIdx].planeMat.SetTextureOffset(Hand.shaderTexNames[0], new Vector2(matUOffset, matVOffset));
            hands.handShadowPlaneInfos[seatIdx].planeMat.SetFloat(Hand.shaderAngNames[0], ang);
        }

        float ReadyZhuaHandPai2(TuiDaoPaiStateData stateData, int seatIdx, PlayerType handStyle)
        {
            List<GameObject> handPaiList = desk.mjSeatHandPaiLists[seatIdx];
            Vector3 leftHandDstPos;
            Vector3 rightHandDstPos;

            if (handPaiList.Count != 0 && handPaiList[0] != null)
            {
                leftHandDstPos = handPaiList[0].transform.position;
                rightHandDstPos = handPaiList[handPaiList.Count - 1].transform.position;
            }
            else
            {
                leftHandDstPos = desk.mjSeatHandPaiPosLists[seatIdx][0];
                rightHandDstPos = leftHandDstPos;
            }

            float count, size;

            switch (seatIdx)
            {
                case 1:
                    count = stateData.handPaiValueList.Count - handPaiList.Count;
                    size = count * fit.GetHandMjSizeByAxis(Axis.X);
                    rightHandDstPos.z += size;
                    break;

                case 2:
                    count = stateData.handPaiValueList.Count - handPaiList.Count;
                    size = count * fit.GetHandMjSizeByAxis(Axis.X);
                    rightHandDstPos.x += size;
                    break;

                case 3:
                    count = stateData.handPaiValueList.Count - handPaiList.Count;
                    size = count * fit.GetHandMjSizeByAxis(Axis.X);
                    rightHandDstPos.z -= size;
                    break;
            }


            FitHandPoseForSeat(seatIdx, handStyle, HandDirection.LeftHand, ActionCombineNum.TuiDaoPai);
            FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, ActionCombineNum.TuiDaoPai);

            float tm = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.LeftHand, leftHandDstPos, ActionCombineNum.TuiDaoPai);
            MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, rightHandDstPos, ActionCombineNum.TuiDaoPai);

            return tm;
        }



        #endregion


    }
}
