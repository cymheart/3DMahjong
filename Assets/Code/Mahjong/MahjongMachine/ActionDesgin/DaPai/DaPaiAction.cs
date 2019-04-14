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
    /// 打牌动作
    /// </summary>
    public class DaPaiAction : BaseHandAction
    {
        public static DaPaiAction Instance { get; } = new DaPaiAction();
        public override void Install()
        {
            mjMachineUpdater.Reg("DaPai", ActionDaPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("DaPai");
        }

        #region 打牌动作
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

            if (playerStateData[seatIdx].state != StateDataGroup.END ||
                desk.mjSeatHandPaiLists[seatIdx].Count == 0 || desk.mjSeatHandPaiLists[seatIdx][0] == null)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            if (paiIdx >= 0 &&
                ((paiIdx < desk.mjSeatHandPaiLists[seatIdx].Count && paiType == HandPaiType.HandPai) ||
                 (paiIdx < desk.mjSeatMoPaiLists[seatIdx].Count && paiType == HandPaiType.MoPai)))
            {
                if (paiType == HandPaiType.HandPai)
                {
                    if (paiIdx >= desk.mjSeatHandPaiLists[seatIdx].Count)
                    {
                        mjCmdMgr.RemoveCmd(opCmdNode);
                        return;
                    }

                    GameObject mj = desk.mjSeatHandPaiLists[seatIdx][paiIdx];

                    if (mj == null)
                    {
                        mjCmdMgr.RemoveCmd(opCmdNode);
                        return;
                    }

                    desk.mjSeatHandPaiLists[seatIdx].RemoveAt(paiIdx);

                    if (seatIdx != 0)
                        mjAssetsMgr.PushMjToOtherHandMjPool(mj);
                    else
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mj);
                }
                else
                {
                    if (paiIdx >= desk.mjSeatMoPaiLists[seatIdx].Count)
                    {
                        mjCmdMgr.RemoveCmd(opCmdNode);
                        return;
                    }

                    GameObject mj = desk.mjSeatMoPaiLists[seatIdx][paiIdx];

                    if (mj == null)
                    {
                        mjCmdMgr.RemoveCmd(opCmdNode);
                        return;
                    }

                    desk.mjSeatMoPaiLists[seatIdx].RemoveAt(paiIdx);

                    if (seatIdx != 0)
                        mjAssetsMgr.PushMjToOtherHandMjPool(mj);
                    else
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mj);
                }
            }

            desk.NextDeskMjPos(seatIdx);
            Vector3Int mjposIdx = desk.GetCurtDeskMjPosIdx(seatIdx);

            DaPaiStateData stateData = playerStateData[seatIdx].GetComponent<DaPaiStateData>();

            stateData.SetDaPaiData(handStyle, mjposIdx, mjFaceValue, isJiaoTing, handActionNum, opCmdNode);
            playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_START, Time.time, -1);

        }


        /// <summary>
        /// 打牌动作
        /// </summary>
        public void ActionDaPai()
        {
            for (int i = 0; i < Fit.playerCount; i++)
            {
                if (fit.isCanUseSeatPlayer[i])
                    ActionDaPai(i);
            }
        }
        void ActionDaPai(int seatIdx)
        {
            if (playerStateData[seatIdx].state < DaPaiStateData.DA_PAI_START ||
                playerStateData[seatIdx].state > DaPaiStateData.DA_PAI_END)
            {
                return;
            }

            DaPaiStateData stateData = playerStateData[seatIdx].GetComponent<DaPaiStateData>();

            if (Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
            {
                MoveHandShadowForDaPai(seatIdx, stateData);
                return;
            }    

            PlayerType handStyle = stateData.handStyle;
            Vector3Int mjPosIdx = stateData.mjPosIdx;
            MahjongFaceValue mjFaceValue = stateData.daPaiFaceValue;
            ActionCombineNum actionCombineNum = stateData.actionCombineNum;

            float waitTime = 0.3f;
            Animation anim = hands.GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);
            Vector3 mjpos = desk.GetMjDeskPaiPos(seatIdx, mjPosIdx.x, mjPosIdx.y);
            float mjHeight = fit.GetDeskMjSizeByAxis(Axis.Z);
            mjpos.y += mjHeight * mjPosIdx.z;

            float fadeTime = 0.06f;

            switch (playerStateData[seatIdx].state)
            {
                case DaPaiStateData.DA_PAI_START:
                    {
                        GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);

                        //是否叫听
                        if (stateData.isJiaoTing)
                        {
                            audio.PlaySpeakAudio(handStyle, AudioIdx.AUDIO_SPEAK_TING);
                            GameObject tingTextParticle = mjAssetsMgr.tingPaiTextParticlePool.PopGameObject();
                            tingTextParticle.transform.localPosition = preSettingHelper.pcgthEffectTextPosSeat[seatIdx];
                            tingTextParticle.SetActive(true);
                            tingTextParticle.GetComponent<ParticleSystem>().Play();
                        }

                        if (actionCombineNum == ActionCombineNum.DaPai5)
                        {
                            waitTime = ReadyFirstHand(seatIdx, handStyle, HandDirection.RightHand, "DaPaiFirstHand2");
                        }
                        else
                        {
                            waitTime = ReadyFirstHand(seatIdx, handStyle, HandDirection.RightHand, "DaPaiFirstHand");
                        }

                        stateData.handShadowAxis[0] = hands.GetHandShadowAxis(seatIdx, handStyle, HandDirection.RightHand, 0).transform;
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(true);
                        MoveHandShadowForDaPai(seatIdx, stateData);


                        if (audio.onVoice)
                        {
                            Debug.Log("打牌:" + mjFaceValue);
                            audio.PlaySpeakAudio(handStyle, (AudioIdx)mjFaceValue);
                        }

                        playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_READY_FIRST_HAND, Time.time, waitTime);
                    }
                    break;

                case DaPaiStateData.DA_PAI_READY_FIRST_HAND:
                    {
                        stateData.curtHandReadyPutDeskPai = ReadyFirstHandMj(seatIdx, handStyle, HandDirection.RightHand, mjFaceValue, actionCombineNum);
                        waitTime = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, mjpos, actionCombineNum);
                        playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_MOVE_HAND_TO_DST_POS, Time.time, waitTime);

                    }
                    break;

                case DaPaiStateData.DA_PAI_MOVE_HAND_TO_DST_POS:
                    {
                        switch (actionCombineNum)
                        {
                            case ActionCombineNum.DaPai1_TaiHand1:
                            case ActionCombineNum.DaPai1_TaiHand2:
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand1:
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand2:
                            case ActionCombineNum.DaPai1_ZhengPai_TaiHand:
                            case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:
                                {
                                    anim.CrossFade("DaPai1", fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1");
                                }
                                break;

                            case ActionCombineNum.DaPai2_MovPai_TaiHand1:
                            case ActionCombineNum.DaPai2_MovPai_TaiHand2:
                                {
                                    anim.CrossFade("DaPai1", fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1");
                                }
                                break;

                            case ActionCombineNum.DaPai3_TaiHand:
                                {
                                    anim.Play("DaPai3");
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai3");
                                }
                                break;


                            case ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand:
                                {
                                    anim.CrossFade("FirstTaiHand2EndDaPai4", fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand2EndDaPai4");
                                }
                                break;


                            case ActionCombineNum.DaPai5:
                                {
                                    anim.CrossFade("FirstTaiHand3EndDaPai5", fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand3EndDaPai5");
                                }
                                break;
                        }


                        playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI, Time.time, waitTime);
                    }
                    break;

                case DaPaiStateData.DA_PAI_CHUPAI:
                    {
                        audio.PlayEffectAudio(AudioIdx.AUDIO_EFFECT_GIVE);

                        GameObject dropPai = stateData.curtHandReadyPutDeskPai;

                        if (dropPai == null)
                        {
                            playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_END, Time.time, -1);
                            return;
                        }

                        fit.OnMjShadow(dropPai, 0);

                        switch (actionCombineNum)
                        {
                            case ActionCombineNum.DaPai1_TaiHand1:
                            case ActionCombineNum.DaPai1_TaiHand2:
                                {
                                    dropPai.transform.SetParent(desk.mjtableTransform, true);
                                    AdjustDeskMjPos(seatIdx, mjpos, 0.04f);

                                    //桌子上最后打出的麻将
                                    desk.lastDaPaiMj = dropPai;

                                    anim.CrossFade(Hand.taiHandActionName[(int)actionCombineNum], fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, Hand.taiHandActionName[(int)actionCombineNum]);
                                    hands.MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, hands.handActionLeaveScreenPosSeat[seatIdx]);
                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                                }
                                break;


                            case ActionCombineNum.DaPai1_MovPai1_TaiHand1:
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand2:
                            case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:
                                {
                                    dropPai.transform.SetParent(desk.mjtableTransform, true);

                                    anim.CrossFade("DaPai1EndTiaoZhengHand", fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndTiaoZhengHand");
                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_TIAOZHENG_HAND, Time.time, waitTime);
                                }
                                break;

                            case ActionCombineNum.DaPai1_ZhengPai_TaiHand:
                                {
                                    dropPai.transform.SetParent(desk.mjtableTransform, true);

                                    anim.CrossFade("DaPai1EndZhengPai", fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndZhengPai");
                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_ZHENGPAI, Time.time, waitTime);
                                }
                                break;


                            case ActionCombineNum.DaPai2_MovPai_TaiHand1:
                            case ActionCombineNum.DaPai2_MovPai_TaiHand2:
                                {
                                    anim.CrossFade("DaPai2EndMovPai", fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai2EndMovPai");
                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI2_MOVPAI, Time.time, waitTime);
                                }
                                break;

                            case ActionCombineNum.DaPai3_TaiHand:
                            case ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand:
                                {
                                    dropPai.transform.SetParent(desk.mjtableTransform, true);
                                    AdjustDeskMjPos(seatIdx, mjpos, 0.02f);

                                    //桌子上最后打出的麻将
                                    desk.lastDaPaiMj = dropPai;

                                    anim.CrossFade(Hand.taiHandActionName[(int)actionCombineNum], fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, Hand.taiHandActionName[(int)actionCombineNum]);

                                    hands.MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, hands.handActionLeaveScreenPosSeat[seatIdx]);
                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                                }
                                break;

                            case ActionCombineNum.DaPai5:
                                {
                                    dropPai.transform.SetParent(desk.mjtableTransform, true);
                                    AdjustDeskMjPos(seatIdx, mjpos, 0.02f);

                                    //桌子上最后打出的麻将
                                    desk.lastDaPaiMj = dropPai;

                                    anim.CrossFade("FirstTaiHand3EndDaPai5EndTaiHand", fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand3EndDaPai5EndTaiHand");

                                    hands.MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, hands.handActionLeaveScreenPosSeat[seatIdx]);

                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                                }
                                break;


                            default:
                                {
                                    Debug.Log("不存在此打牌动作编号:" + actionCombineNum + "!");
                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_END, Time.time, -1);
                                }
                                break;
                        }
                    }
                    break;

                case DaPaiStateData.DA_PAI_CHUPAI_TIAOZHENG_HAND:
                    {

                        switch (actionCombineNum)
                        {
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand1:
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand2:
                                {
                                    anim.Play("DaPai1EndTiaoZhengHandEndMovPai1");
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndTiaoZhengHandEndMovPai1");
                                    AdjustDeskMjPos(seatIdx, mjpos, waitTime);
                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_TIAOZHENG_HAND_MOVPAI1, Time.time, waitTime);
                                }
                                break;

                            case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:
                                {
                                    anim.Play("DaPai1EndTiaoZhengHandEndMovPai1");
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndTiaoZhengHandEndMovPai1");
                                    AdjustDeskMjPos(seatIdx, mjpos, waitTime, true, 0.67f, false);
                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_TIAOZHENG_HAND_MOVPAI1, Time.time, waitTime);
                                }
                                break;
                        }
                    }
                    break;

                case DaPaiStateData.DA_PAI_CHUPAI_TIAOZHENG_HAND_MOVPAI1:
                    {
                        switch (actionCombineNum)
                        {
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand1:
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand2:
                                {
                                    //桌子上最后打出的麻将
                                    desk.lastDaPaiMj = stateData.curtHandReadyPutDeskPai;

                                    anim.CrossFade(Hand.taiHandActionName[(int)actionCombineNum], fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, Hand.taiHandActionName[(int)actionCombineNum]);

                                    hands.MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, hands.handActionLeaveScreenPosSeat[seatIdx]);
                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);


                                }
                                break;

                            case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:
                                {
                                    anim.CrossFade("DaPai1EndMovPai1EndZhengPai", fadeTime);
                                    waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndMovPai1EndZhengPai");
                                    playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_ZHENGPAI, Time.time, waitTime / 2);
                                }
                                break;
                        }
                    }
                    break;


                case DaPaiStateData.DA_PAI_CHUPAI_ZHENGPAI:
                    {
                        AdjustDeskMjPos(seatIdx, mjpos, playerStateData[seatIdx].stateLiveTime);
                        playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_ZHENGPAI_ADJUSTPAI, Time.time, playerStateData[seatIdx].stateLiveTime);
                    }
                    break;


                case DaPaiStateData.DA_PAI_CHUPAI_ZHENGPAI_ADJUSTPAI:
                    {
                        hands.MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, hands.handActionLeaveScreenPosSeat[seatIdx]);

                        switch (actionCombineNum)
                        {
                            case ActionCombineNum.DaPai1_ZhengPai_TaiHand:

                                //桌子上最后打出的麻将
                                desk.lastDaPaiMj = stateData.curtHandReadyPutDeskPai;

                                anim.CrossFade("DaPai1EndZhengPaiEndTaiHand", fadeTime);
                                waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndZhengPaiEndTaiHand");
                                playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                                break;

                            case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:

                                //桌子上最后打出的麻将
                                desk.lastDaPaiMj = stateData.curtHandReadyPutDeskPai;

                                anim.CrossFade("DaPai1EndMovPai1EndZhengPaiEndTaiHand", fadeTime);
                                waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndMovPai1EndZhengPaiEndTaiHand");
                                playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                                break;
                        }
                    }
                    break;


                case DaPaiStateData.DA_PAI_CHUPAI2_MOVPAI:
                case DaPaiStateData.DA_PAI_CHUPAI_MOVPAI2:
                    {
                        stateData.curtHandReadyPutDeskPai.transform.SetParent(desk.mjtableTransform, true);
                        AdjustDeskMjPos(seatIdx, mjpos, 0.06f);

                        //桌子上最后打出的麻将
                        desk.lastDaPaiMj = stateData.curtHandReadyPutDeskPai;

                        anim.CrossFade(Hand.taiHandActionName[(int)actionCombineNum], fadeTime);
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, Hand.taiHandActionName[(int)actionCombineNum]);

                        hands.MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, hands.handActionLeaveScreenPosSeat[seatIdx]);

                        playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                    }
                    break;

                case DaPaiStateData.DA_PAI_CHUPAI_TAIHAND:
                    {
                        int key = desk.GetDeskDaPaiMjDictKey(mjPosIdx.x, mjPosIdx.y, mjPosIdx.z);
                        desk.deskDaPaiMjDicts[seatIdx][key] = stateData.curtHandReadyPutDeskPai;

                        desk.AppendMjToDeskGlobalMjPaiSetDict(mjFaceValue, stateData.curtHandReadyPutDeskPai);

                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.RightHand, actionCombineNum);
                        playerStateData[seatIdx].SetState(DaPaiStateData.DA_PAI_END, Time.time, waitTime);
                    }
                    break;

                case DaPaiStateData.DA_PAI_END:
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
      

        /// <summary>
        /// 根据打牌动作，准备打牌手中的麻将
        /// </summary>
        /// <param name="seatIdx">座位号</param>    
        /// 
        GameObject ReadyFirstHandMj(int seatIdx, PlayerType handStyle, HandDirection handDir, MahjongFaceValue mjFaceValue, ActionCombineNum actionCombineNum)
        {
            GameObject bone;
            GameObject readyPutMj = null;
            float sign = 1;
            Vector3 a;

            GameObject mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValue);

            if (mj == null)
            {
                Debug.Log("不能获取指定面值麻将资源!");
                return null;
            }

            mj.layer = mjMachine.defaultLayer;
            fit.FitSeatDeskMj(seatIdx, mj, false, 0.91f, false, false);


            switch (actionCombineNum)
            {
                case ActionCombineNum.DaPai1_ZhengPai_TaiHand:
                    bone = hands.GetHandBone(seatIdx, handStyle, handDir, 1);
                    mj.transform.localPosition = hands.mjDaPaiFirstHandPos[seatIdx];
                    a = hands.mjDaPaiFirstHandEulerAngles[seatIdx];
                    mj.transform.localEulerAngles = new Vector3(a.x, a.y, a.z + sign * 10f);
                    break;

                case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:
                    bone = hands.GetHandBone(seatIdx, handStyle, handDir, 1);
                    mj.transform.localPosition = hands.mjDaPaiFirstHandPos[seatIdx];
                    a = hands.mjDaPaiFirstHandEulerAngles[seatIdx];
                    mj.transform.localEulerAngles = new Vector3(a.x, a.y, a.z - sign * 10f);
                    break;

                case ActionCombineNum.DaPai5:
                    bone = hands.GetHandBone(seatIdx, handStyle, handDir, 2);
                    mj.transform.localPosition = hands.mjDaPaiFirstHandPos2[seatIdx];
                    mj.transform.localEulerAngles = hands.mjDaPaiFirstHandEulerAngles2[seatIdx];
                    break;

                default:
                    bone = hands.GetHandBone(seatIdx, handStyle, handDir, 1);
                    mj.transform.localPosition = hands.mjDaPaiFirstHandPos[seatIdx];
                    mj.transform.localEulerAngles = hands.mjDaPaiFirstHandEulerAngles[seatIdx];
                    break;
            }

            mj.transform.SetParent(bone.transform, false);

            FitHandPoseForSeat(seatIdx, handStyle, handDir, actionCombineNum);
            readyPutMj = mj;

            return readyPutMj;
        }

        void AdjustDeskMjPos(int seatIdx, Vector3 mjpos, float adjustTime, bool isMove = true, float moveProgress = 1.0f, bool isRotate = true)
        {
            DaPaiStateData stateData = playerStateData[seatIdx].GetComponent<DaPaiStateData>();

            if (isMove)
            {
                GameObject pai = stateData.curtHandReadyPutDeskPai;
                Vector3 offsetPos = mjpos - pai.transform.position;
                offsetPos.x *= moveProgress;
                offsetPos.z *= moveProgress;
                stateData.curtHandReadyPutDeskPai.transform.DOMove(offsetPos, adjustTime).SetRelative();
            }

            if (isRotate)
                stateData.curtHandReadyPutDeskPai.transform.DORotate(fit.GetSeatDeskMjFitEulerAngles(seatIdx), adjustTime);
        }

        #endregion


    }
}
