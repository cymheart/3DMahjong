using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ComponentDesgin;

namespace ActionDesgin
{
    /// <summary>
    /// 补花动作
    /// </summary>
    public class BuHuaAction : BaseHandAction
    {
        private static BuHuaAction instance = null;
        public static BuHuaAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new BuHuaAction();
                return instance;
            }
        }


        /// <summary>
        /// 动作安装
        /// </summary>
        /// <param name="mjMachine"></param>
        public override void Install()
        {
            mjMachineUpdater.Reg("buhua", ActionBuHua);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("buhua");
        }

        /// <summary>
        /// 补花
        /// </summary>
        public void BuHua(int seatIdx, PlayerType handStyle, MahjongFaceValue buHuaPaiFaceValue,
            ActionCombineNum handActionNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].state != StateDataGroup.END)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            desk.NextDeskHuPaiMjPos(seatIdx);
            int idx = desk.GetCurtDeskHuPaiMjPosIdx(seatIdx);

            BuHuaStateData stateData = playerStateData[seatIdx].GetComponent<BuHuaStateData>();

            stateData.SetBuHuaPaiData(handStyle, idx, buHuaPaiFaceValue, handActionNum, opCmdNode);
            playerStateData[seatIdx].SetState(BuHuaStateData.BUHUA_PAI_START, Time.time, -1);
        }

        #region 补花动作
        public void ActionBuHua()
        {
            for (int i = 0; i < Fit.playerCount; i++)
            {
                if (fit.isCanUseSeatPlayer[i])
                    ActionBuHua(i);
            }
        }

        void ActionBuHua(int seatIdx)
        {
            if (playerStateData[seatIdx].state < BuHuaStateData.BUHUA_PAI_START ||
               playerStateData[seatIdx].state > BuHuaStateData.BUHUA_PAI_END ||
               Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
            {
                return;
            }

            BuHuaStateData stateData = playerStateData[seatIdx].GetComponent<BuHuaStateData>();

            PlayerType handStyle = stateData.handStyle;
            float waitTime = 0.3f;
            Animation anim = hands.GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);


            switch (playerStateData[seatIdx].state)
            {
                case BuHuaStateData.BUHUA_PAI_START:
                    {
                        GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);
                        waitTime = ReadyFirstHand(seatIdx, handStyle, HandDirection.RightHand, "HuPaiFirstHand");

                        audio.PlaySpeakAudio(handStyle, AudioIdx.AUDIO_SPEAK_BUHUA, scene.cameraTransform.position);
                        playerStateData[seatIdx].SetState(BuHuaStateData.BUHUA_PAI_READY_FIRST_HAND, Time.time, waitTime);
                    }
                    break;

                case BuHuaStateData.BUHUA_PAI_READY_FIRST_HAND:
                    {
                        Vector3 mjpos = desk.GetDeskHuPaiMjPos(seatIdx, stateData.buHuaPaiMjPosIdx);
                        mjpos.y += fit.GetDeskMjSizeByAxis(Axis.Z) / 2;
                        FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, stateData.actionCombineNum);
                        waitTime = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, mjpos, stateData.actionCombineNum);
                        playerStateData[seatIdx].SetState(BuHuaStateData.BUHUA_PAI_MOVE_HAND_TO_DST_POS, Time.time, waitTime);
                    }
                    break;

                case BuHuaStateData.BUHUA_PAI_MOVE_HAND_TO_DST_POS:
                    {
                        anim.Play("FirstTaiHand1EndHuPai");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand1EndHuPai");
                        playerStateData[seatIdx].SetState(BuHuaStateData.BUHUA_PAI_BU, Time.time, waitTime);
                    }
                    break;

                case BuHuaStateData.BUHUA_PAI_BU:
                    {
                        GameObject mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(stateData.buHuaPaiFaceValue);
                        mj.layer = mjMachine.defaultLayer;
                        fit.FitSeatDeskMj(seatIdx, mj);
                        mj.transform.position = desk.GetDeskHuPaiMjPos(seatIdx, stateData.buHuaPaiMjPosIdx);
                        desk.deskHuPaiMjDicts[seatIdx][stateData.buHuaPaiMjPosIdx] = mj;
                        desk.AppendMjToDeskGlobalMjPaiSetDict(stateData.buHuaPaiFaceValue, mj);

                        if (stateData.buHuaPaiMjPosIdx >=
                            fit.huPaiDeskPosMjLayoutRowCount * fit.huPaiDeskPosMjLayoutColCount)
                        {
                            fit.OffMjShadow(mj);
                        }

                        mj.transform.SetParent(desk.mjtableTransform, true);
                        playerStateData[seatIdx].SetState(BuHuaStateData.BUHUA_PAI_GET_PAI, Time.time, 0.2f);

                    }
                    break;


                case BuHuaStateData.BUHUA_PAI_GET_PAI:
                    {
                        anim.Play("FirstTaiHand1EndHuPaiEndTaiHand");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand1EndHuPaiEndTaiHand");
                        playerStateData[seatIdx].SetState(BuHuaStateData.BUHUA_PAI_END, Time.time, waitTime);
                    }
                    break;

                case BuHuaStateData.BUHUA_PAI_END:
                    {
                        GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(false);
                        playerStateData[seatIdx].state = StateDataGroup.END;

                        ProcessHandActionmjCmdMgr(seatIdx, stateData);
                    }

                    break;
            }
        }

        #endregion


    }

}
