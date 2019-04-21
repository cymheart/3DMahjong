using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using ComponentDesgin;

namespace ActionDesgin
{
    /// <summary>
    /// 胡牌动作
    /// </summary>
    public class HuPaiAction : BaseHandAction
    {
        private static HuPaiAction instance = null;
        public static HuPaiAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new HuPaiAction();
                return instance;
            }
        }
        public override void Install()
        {
            mjMachineUpdater.Reg("HuPai", ActionHuPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("HuPai");
        }

        #region 胡牌动作
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

            if (playerStateData[seatIdx].state != HandActionState.END)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            desk.NextDeskHuPaiMjPos(seatIdx);
            int idx = desk.GetCurtDeskHuPaiMjPosIdx(seatIdx);
            HuPaiStateData stateData = playerStateData[seatIdx].GetComponent<HuPaiStateData>();

            stateData.SetHuPaiData(handStyle, targetSeatIdx, targetMjIdx, idx, huPaiFaceValue, handActionNum, opCmdNode);
            playerStateData[seatIdx].SetState(HandActionState.HU_PAI_START, Time.time, -1);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="targetSeatIdx">所胡目标玩家座号，如果为-1,为自摸</param>
        /// <param name="targetMjIdx">目标胡牌麻将编号</param>
        /// <param name="mjPosIdx"></param>
        /// <param name="huPaiFaceValue">胡牌麻将面值</param>
        /// <param name="actionCombineNum"></param>
        /// <returns></returns>
        /// 
        public void ActionHuPai()
        {
            for (int i = 0; i < Fit.playerCount; i++)
            {
                if (fit.isCanUseSeatPlayer[i])
                    ActionHuPai(i);
            }
        }
        void ActionHuPai(int seatIdx)
        {
            if (playerStateData[seatIdx].state < HandActionState.HU_PAI_START ||
               playerStateData[seatIdx].state > HandActionState.HU_PAI_END ||
               Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
            {
                return;
            }

            HuPaiStateData stateData = playerStateData[seatIdx].GetComponent<HuPaiStateData>();

            PlayerType handStyle = stateData.handStyle;
            int targetSeatIdx = stateData.huPaiTargetSeatIdx;

            float waitTime = 0.3f;
            Animation anim = hands.GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);


            switch (playerStateData[seatIdx].state)
            {
                case HandActionState.HU_PAI_START:
                    {
                        GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);
                        waitTime = ReadyFirstHand(seatIdx, handStyle, HandDirection.RightHand, "HuPaiFirstHand");

                        //玩家胡牌方式是自摸
                        if (targetSeatIdx == -1)
                        {
                            audio.PlayEffectAudio(AudioIdx.AUDIO_EFFECT_ZIMO);
                            audio.PlaySpeakAudio(handStyle, AudioIdx.AUDIO_SPEAK_ZIMO);
                            GameObject zimoTextParticle = mjAssetsMgr.zimoPaiTextParticlePool.PopGameObject();
                            zimoTextParticle.transform.localPosition = preSettingHelper.pcgthEffectTextPosSeat[seatIdx];
                            zimoTextParticle.SetActive(true);
                            zimoTextParticle.GetComponent<ParticleSystem>().Play();
                        }
                        else
                        {
                            audio.PlaySpeakAudio(handStyle, AudioIdx.AUDIO_SPEAK_HU);
                            GameObject huPaiTextParticle = mjAssetsMgr.huPaiTextParticlePool.PopGameObject();
                            huPaiTextParticle.transform.localPosition = preSettingHelper.pcgthEffectTextPosSeat[seatIdx];
                            huPaiTextParticle.SetActive(true);
                            huPaiTextParticle.GetComponent<ParticleSystem>().Play();
                        }

                        //
                        playerStateData[seatIdx].SetState(HandActionState.HU_PAI_READY_FIRST_HAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.HU_PAI_READY_FIRST_HAND:
                    {
                        Vector3 mjpos = desk.GetDeskHuPaiMjPos(seatIdx, stateData.huPaiMjPosIdx);
                        audio.PlayEffectAudio(AudioIdx.AUDIO_EFFECT_SHANDIAN);
      
                        mjpos.y += fit.GetDeskMjSizeByAxis(Axis.Z) / 2;
                        FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, stateData.actionCombineNum);
                        waitTime = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, mjpos, stateData.actionCombineNum);


                        //玩家胡牌方式不是自摸
                        if (targetSeatIdx >= 0 && targetSeatIdx < Fit.playerCount)
                        {
                            Vector3Int targetMjIdx = stateData.huPaiTargetMjIdx;

                            if (targetMjIdx.x == -1 || targetMjIdx.y == -1 || targetMjIdx.z == -1)
                            {
                                targetMjIdx = desk.GetCurtDeskMjPosIdx(targetSeatIdx);
                            }

                            int key = desk.GetDeskDaPaiMjDictKey(targetMjIdx.x, targetMjIdx.y, targetMjIdx.z);
                            if (desk.deskDaPaiMjDicts[targetSeatIdx].ContainsKey(key))
                            {
                                stateData.huPaiTargetMjKey = key;
                                GameObject targetmj = desk.deskDaPaiMjDicts[targetSeatIdx][key];

                                GameObject huPaiParticle = mjAssetsMgr.huPaiShanDianParticlePool.PopGameObject();
                                huPaiParticle.SetActive(true);
                                huPaiParticle.transform.position = targetmj.transform.position;
                                huPaiParticle.GetComponent<ParticleSystem>().Play();

                                if (targetmj == desk.lastDaPaiMj)
                                    desk.lastDaPaiMj = null;
                            }
                            else
                            {
                                stateData.huPaiTargetMjKey = -1;
                            }
                        }

                        playerStateData[seatIdx].SetState(HandActionState.HU_PAI_MOVE_HAND_TO_DST_POS, Time.time, waitTime);
                    }
                    break;

                case HandActionState.HU_PAI_MOVE_HAND_TO_DST_POS:
                    {
                        int key = stateData.huPaiTargetMjKey;

                        if (key != -1)
                        {
                            GameObject targetmj = desk.deskDaPaiMjDicts[targetSeatIdx][key];
                            desk.deskDaPaiMjDicts[targetSeatIdx].Remove(key);
                            desk.RemoveMjFromDeskGlobalMjPaiSetDict(stateData.huPaiFaceValue, targetmj);
                            mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(targetmj);

                            desk.PrevDeskMjPos(targetSeatIdx);
                        }

                        anim.Play("FirstTaiHand1EndHuPai");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand1EndHuPai");

                        playerStateData[seatIdx].SetState(HandActionState.HU_PAI_HU, Time.time, waitTime);
                    }
                    break;

                case HandActionState.HU_PAI_HU:
                    {
                        GameObject mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(stateData.huPaiFaceValue);
                        mj.layer = mjMachine.defaultLayer;
                        fit.FitSeatDeskMj(seatIdx, mj);
                        mj.transform.position = desk.GetDeskHuPaiMjPos(seatIdx, stateData.huPaiMjPosIdx);
                        desk.deskHuPaiMjDicts[seatIdx][stateData.huPaiMjPosIdx] = mj;
                        desk.AppendMjToDeskGlobalMjPaiSetDict(stateData.huPaiFaceValue, mj);

                        if (stateData.huPaiMjPosIdx >=
                            fit.huPaiDeskPosMjLayoutRowCount * fit.huPaiDeskPosMjLayoutColCount)
                            fit.OffMjShadow(mj);

                        mj.transform.SetParent(desk.mjtableTransform, true);


                        Vector3 handDstPos = mj.transform.position;
                        handDstPos.y += fit.GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.001f;
                        GameObject huPaiParticle = mjAssetsMgr.huPaiGetMjParticlePool.PopGameObject();
                        huPaiParticle.SetActive(true);
                        huPaiParticle.transform.position = handDstPos;
                        huPaiParticle.GetComponent<ParticleSystem>().Play();

                        playerStateData[seatIdx].SetState(HandActionState.HU_PAI_GET_PAI, Time.time, 0.2f);

                    }
                    break;


                case HandActionState.HU_PAI_GET_PAI:
                    {
                        anim.Play("FirstTaiHand1EndHuPaiEndTaiHand");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand1EndHuPaiEndTaiHand");
                        playerStateData[seatIdx].SetState(HandActionState.HU_PAI_END, Time.time, waitTime);
                    }
                    break;

                case HandActionState.HU_PAI_END:
                    {
                        GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(false);
                        playerStateData[seatIdx].state = HandActionState.END;

                        ProcessHandActionmjCmdMgr(seatIdx, stateData);
                    }

                    break;
            }
        }

        #endregion

    }
}
