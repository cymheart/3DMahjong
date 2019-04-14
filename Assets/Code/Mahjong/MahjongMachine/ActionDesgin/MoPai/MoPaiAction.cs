using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ComponentDesgin;

namespace ActionDesgin
{
    /// <summary>
    /// 摸牌动作
    /// </summary>
    public class MoPaiAction : BaseHandAction
    {
        private static MoPaiAction instance = null;
        public static MoPaiAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new MoPaiAction();
                return instance;
            }
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("MoPai", ActionMoPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("MoPai");
        }


        #region 摸牌动作

        /// <summary>
        /// 摸牌
        /// </summary>
        /// <param name="seatIdx">玩家座号</param>
        public void MoPai(int seatIdx, MahjongFaceValue mjFaceValue, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].state != StateDataGroup.END ||
                   desk.mjSeatHandPaiLists[seatIdx].Count == 0 || desk.mjSeatHandPaiLists[seatIdx][0] == null)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }


            desk.FenMahjongPaiFromPaiDui(fit.curtPaiDuiPos, 1);
            MoPaiStateData stateData = playerStateData[seatIdx].GetComponent<MoPaiStateData>();

            stateData.SetMoPaiData(mjFaceValue, opCmdNode);
            playerStateData[seatIdx].SetState(MoPaiStateData.MO_PAI_START, Time.time, -1);

        }


        /// <summary>
        /// 摸牌动作
        /// </summary>
        /// <param name="seatIdx">摸牌玩家座号</param>
        /// <param name="mjFaceValue">牌面值</param>
        /// <returns></returns>
        public void ActionMoPai()
        {
            for (int i = 0; i < Fit.playerCount; i++)
            {
                if (fit.isCanUseSeatPlayer[i])
                    ActionMoPai(i);
            }
        }
        void ActionMoPai(int seatIdx)
        {
            if (playerStateData[seatIdx].state < MoPaiStateData.MO_PAI_START ||
            playerStateData[seatIdx].state > MoPaiStateData.MO_PAI_END ||
            Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
            {
                return;
            }

            MoPaiStateData stateData = playerStateData[seatIdx].GetComponent<MoPaiStateData>();

            if (playerStateData[seatIdx].state == MoPaiStateData.MO_PAI_END)
            {
                fit.OnMjShadow(stateData.curtMoPaiMj, stateData.curtMoPaiMjShadowIdx);
                playerStateData[seatIdx].state = StateDataGroup.END;

                ProcessHandActionmjCmdMgr(seatIdx, stateData);
                return;
            }

            MahjongFaceValue mjFaceValue = stateData.moPaiFaceValue;
            float waitTime = 0.4f;
            GameObject mj = null;

            switch (seatIdx)
            {
                case 0:
                    {
                        mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValue);
                        mj.layer = mjMachine.defaultLayer;
                        stateData.curtMoPaiMj = mj;
                        stateData.curtMoPaiMjShadowIdx = 1;
                        desk.mjSeatMoPaiLists[seatIdx].Add(mj);

                        fit.FitSeatCanvasHandMj(mj);
                        fit.OffMjShadow(mj);
                        mj.transform.SetParent(desk.canvasHandPaiTransform, true);
                        Vector3 mjOldEulerAngles = mj.transform.localEulerAngles;

                        float mjsize = fit.GetCanvasHandMjSizeByAxis(Axis.X);
                        Vector3 pos;

                        if (desk.mjSeatMoPaiLists[seatIdx].Count > 1)
                        {
                            GameObject lastMoMj = desk.mjSeatMoPaiLists[seatIdx][desk.mjSeatMoPaiLists[seatIdx].Count - 2];
                            pos = new Vector3(lastMoMj.transform.localPosition.x + mjsize, lastMoMj.transform.localPosition.y, lastMoMj.transform.localPosition.z);
                        }
                        else
                        {
                            int idx = desk.GetHandPaiListLastPaiIdx(seatIdx);

                            if (idx == -1)
                            {
                                pos = desk.mjSeatHandPaiPosLists[seatIdx][0];
                            }
                            else
                            {
                                GameObject lastmj = desk.mjSeatHandPaiLists[seatIdx][idx];
                                pos = lastmj.transform.localPosition;
                                pos.x += mjsize + fit.moPaiToHandPaiCanvasOffset;
                            }
                        }

                        float y = fit.GetCanvasHandMjSizeByAxis(Axis.Y);
                        y += y / 3;

                        mj.transform.localPosition = new Vector3(pos.x + 15f, pos.y + y, pos.z);
                        mj.transform.localEulerAngles = new Vector3(mj.transform.localEulerAngles.x, mj.transform.localEulerAngles.y, mj.transform.localEulerAngles.z + 40f);

                        mj.transform.DOLocalMove(pos, waitTime).SetEase(Ease.InCirc);
                        mj.transform.DOLocalRotate(mjOldEulerAngles, waitTime);
                    }

                    break;

                case 1:
                    {
                        mj = mjAssetsMgr.PopMjFromOtherHandMjPool();
                        stateData.curtMoPaiMj = mj;
                        desk.mjSeatMoPaiLists[seatIdx].Add(mj);

                        fit.FitSeatHandMj(seatIdx, mj);
                        Vector3 mjOldEulerAngles = mj.transform.eulerAngles;
                        float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                        Vector3 pos;

                        if (desk.mjSeatMoPaiLists[seatIdx].Count > 1)
                        {
                            GameObject lastMoMj = desk.mjSeatMoPaiLists[seatIdx][desk.mjSeatMoPaiLists[seatIdx].Count - 2];
                            pos = new Vector3(lastMoMj.transform.position.x, lastMoMj.transform.position.y, lastMoMj.transform.position.z + mjsize);
                        }
                        else
                        {
                            int idx = desk.GetHandPaiListLastPaiIdx(seatIdx);

                            if (idx == -1)
                            {
                                pos = desk.mjSeatHandPaiPosLists[seatIdx][0];
                            }
                            else
                            {
                                GameObject lastmj = desk.mjSeatHandPaiLists[seatIdx][idx];
                                pos = lastmj.transform.position;
                                pos.z += mjsize + fit.moPaiToHandPaiOffset;
                            }
                        }

                        fit.OffMjShadow(mj);
                        mj.transform.position = new Vector3(pos.x, pos.y + 0.06f, pos.z + 0.005f);
                        mj.transform.eulerAngles = new Vector3(mj.transform.eulerAngles.x, mj.transform.eulerAngles.y, mj.transform.eulerAngles.z + 40f);

                        mj.transform.SetParent(desk.mjtableTransform, true);
                        mj.transform.DOMove(pos, waitTime).SetEase(Ease.InCirc);
                        mj.transform.DORotate(mjOldEulerAngles, waitTime);
                    }

                    break;

                case 2:
                    {
                        mj = mjAssetsMgr.PopMjFromOtherHandMjPool();
                        stateData.curtMoPaiMj = mj;
                        desk.mjSeatMoPaiLists[seatIdx].Add(mj);

                        fit.FitSeatHandMj(seatIdx, mj);
                        Vector3 mjOldEulerAngles = mj.transform.eulerAngles;
                        float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                        Vector3 pos;

                        if (desk.mjSeatMoPaiLists[seatIdx].Count > 1)
                        {
                            GameObject lastMoMj = desk.mjSeatMoPaiLists[seatIdx][desk.mjSeatMoPaiLists[seatIdx].Count - 2];
                            pos = new Vector3(lastMoMj.transform.position.x + mjsize, lastMoMj.transform.position.y, lastMoMj.transform.position.z);
                        }
                        else
                        {
                            int idx = desk.GetHandPaiListLastPaiIdx(seatIdx);

                            if (idx == -1)
                            {
                                pos = desk.mjSeatHandPaiPosLists[seatIdx][0];
                            }
                            else
                            {
                                GameObject lastmj = desk.mjSeatHandPaiLists[seatIdx][idx];
                                pos = lastmj.transform.position;
                                pos.x += mjsize + fit.moPaiToHandPaiOffset;
                            }
                        }


                        fit.OffMjShadow(mj);
                        mj.transform.position = new Vector3(pos.x + 0.005f, pos.y + 0.06f, pos.z);
                        mj.transform.eulerAngles = new Vector3(mj.transform.eulerAngles.x, mj.transform.eulerAngles.y, mj.transform.eulerAngles.z + 40f);

                        mj.transform.SetParent(desk.mjtableTransform, true);
                        mj.transform.DOMove(pos, waitTime).SetEase(Ease.InCirc);
                        mj.transform.DOLocalRotate(mjOldEulerAngles, waitTime);
                    }

                    break;

                case 3:
                    {
                        mj = mjAssetsMgr.PopMjFromOtherHandMjPool();
                        stateData.curtMoPaiMj = mj;
                        desk.mjSeatMoPaiLists[seatIdx].Add(mj);

                        fit.FitSeatHandMj(seatIdx, mj);
                        Vector3 mjOldEulerAngles = mj.transform.eulerAngles;
                        float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                        Vector3 pos;

                        if (desk.mjSeatMoPaiLists[seatIdx].Count > 1)
                        {
                            GameObject lastMoMj = desk.mjSeatMoPaiLists[seatIdx][desk.mjSeatMoPaiLists[seatIdx].Count - 2];
                            pos = new Vector3(lastMoMj.transform.position.x, lastMoMj.transform.position.y, lastMoMj.transform.position.z - mjsize);
                        }
                        else
                        {
                            int idx = desk.GetHandPaiListLastPaiIdx(seatIdx);

                            if (idx == -1)
                            {
                                pos = desk.mjSeatHandPaiPosLists[seatIdx][0];
                            }
                            else
                            {
                                GameObject lastmj = desk.mjSeatHandPaiLists[seatIdx][idx];
                                pos = lastmj.transform.position;
                                pos.z -= mjsize + fit.moPaiToHandPaiOffset;
                            }
                        }

                        mj.transform.position = pos;
                        mj.transform.SetParent(desk.mjtableTransform, true);

                        fit.OffMjShadow(mj);
                        mj.transform.position = new Vector3(pos.x, pos.y + 0.06f, pos.z - 0.005f);
                        mj.transform.eulerAngles = new Vector3(mj.transform.eulerAngles.x, mj.transform.eulerAngles.y, mj.transform.eulerAngles.z + 40f);

                        mj.transform.SetParent(desk.mjtableTransform, true);
                        mj.transform.DOMove(pos, waitTime).SetEase(Ease.InCirc);
                        mj.transform.DOLocalRotate(mjOldEulerAngles, waitTime);
                    }

                    break;
            }

            playerStateData[seatIdx].SetState(MoPaiStateData.MO_PAI_END, Time.time, waitTime);

        }

        #endregion


    }
}
