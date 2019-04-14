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
    /// 启动骰子机动作
    /// </summary>
    public class QiDongDiceMachineAction : BaseHandAction
    {
        public static QiDongDiceMachineAction Instance { get; } = new QiDongDiceMachineAction();
        StateDataGroup mjMachineStateData;

        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);
            mjMachineStateData = mjMachine.mjMachineStateData;
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("QiDongDiceMachine", ActionQiDongDiceMachine);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("QiDongDiceMachine");
        }



        #region 启动骰子器动作
        /// <summary>
        /// 启动骰子器
        /// </summary>
        /// <param name="seatIdx">玩家座号</param>
        public void QiDongDiceMachine(int seatIdx, int dice1Point = -1, int dice2Point = -1, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].state != StateDataGroup.END)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            QiDongDiceMachineStateData stateData = playerStateData[seatIdx].GetComponent<QiDongDiceMachineStateData>();

            stateData.SetQiDongDiceMachineData(dice1Point, dice2Point, opCmdNode);
            playerStateData[seatIdx].SetState(QiDongDiceMachineStateData.QIDONG_DICEMACHINE_START, Time.time, -1);
        }


        /// <summary>
        /// 启动骰子器动作
        /// </summary>
        public void ActionQiDongDiceMachine()
        {
            for (int i = 0; i < Fit.playerCount; i++)
            {
                if (fit.isCanUseSeatPlayer[i])
                    ActionQiDongDiceMachine(i);
            }
        }

        void ActionQiDongDiceMachine(int seatIdx)
        {
            if (playerStateData[seatIdx].state < QiDongDiceMachineStateData.QIDONG_DICEMACHINE_START ||
               playerStateData[seatIdx].state > QiDongDiceMachineStateData.QIDONG_DICEMACHINE_END)
            {
                return;
            }

            QiDongDiceMachineStateData stateData = playerStateData[seatIdx].GetComponent<QiDongDiceMachineStateData>();

            if (Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
            {
                MoveHandShadowForDaPai(seatIdx, stateData);
                return;
            }

           
            PlayerType handStyle = stateData.handStyle;

            float waitTime = 0.3f;
            float fadeTime = 0.06f;
            Animation anim = hands.GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);

            switch (playerStateData[seatIdx].state)
            {
                case QiDongDiceMachineStateData.QIDONG_DICEMACHINE_START:
                    {
                        GameObject hand = hands.GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);
                        waitTime = ReadyFirstHand(seatIdx, handStyle, HandDirection.RightHand, "QiDongDiceMachineReadyHand");

                        stateData.handShadowAxis[0] = hands.GetHandShadowAxis(seatIdx, handStyle, HandDirection.RightHand, 0).transform;
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(true);
                        MoveHandShadowForDaPai(seatIdx, stateData);


                        playerStateData[seatIdx].SetState(QiDongDiceMachineStateData.QIDONG_DICEMACHINE_READY_FIRST_HAND, Time.time, waitTime);

                    }
                    break;


                case QiDongDiceMachineStateData.QIDONG_DICEMACHINE_READY_FIRST_HAND:
                    {
                        FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, ActionCombineNum.QiDongDiceMachine);

                        waitTime = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, preSettingHelper.diceQiDongPosSeat[seatIdx], ActionCombineNum.QiDongDiceMachine);
                        playerStateData[seatIdx].SetState(QiDongDiceMachineStateData.QIDONG_DICEMACHINE_MOVE_HAND_TO_DST_POS, Time.time, waitTime);
                    }
                    break;

                case QiDongDiceMachineStateData.QIDONG_DICEMACHINE_MOVE_HAND_TO_DST_POS:
                    {
                        anim.CrossFade("QiDongDiceMachine", fadeTime);
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "QiDongDiceMachine");
                        playerStateData[seatIdx].SetState(QiDongDiceMachineStateData.QIDONG_DICEMACHINE_QIDONG, Time.time, waitTime);
                    }
                    break;

                case QiDongDiceMachineStateData.QIDONG_DICEMACHINE_QIDONG:
                    {
                        QiDong(seatIdx, stateData.dice1Point, stateData.dice2Point);

                        anim.CrossFade("QiDongDiceMachineEndTaiHand", fadeTime);
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "QiDongDiceMachineEndTaiHand");
                        playerStateData[seatIdx].SetState(QiDongDiceMachineStateData.QIDONG_DICEMACHINE_TAIHAND, Time.time, waitTime);
                    }
                    break;


                case QiDongDiceMachineStateData.QIDONG_DICEMACHINE_TAIHAND:
                    {
                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.RightHand, ActionCombineNum.QiDongDiceMachine);
                        playerStateData[seatIdx].SetState(QiDongDiceMachineStateData.QIDONG_DICEMACHINE_END, Time.time, waitTime + 4f);
                    }
                    break;

                case QiDongDiceMachineStateData.QIDONG_DICEMACHINE_END:
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

        void QiDong(int seatIdx, int dice1Point = -1, int dice2Point = -1)
        {
            mjMachine.GetComponent<MahjongDiceMachine>().StartRun(dice1Point, dice2Point);
        }

        #endregion



    }
}
