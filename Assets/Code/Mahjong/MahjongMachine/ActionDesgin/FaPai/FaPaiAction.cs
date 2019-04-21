using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ComponentDesgin;

namespace ActionDesgin
{
    /// <summary>
    /// 发牌动作
    /// </summary>
    public class FaPaiAction: BaseHandAction
    {
        private static FaPaiAction instance = null;
        public static FaPaiAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new FaPaiAction();
                return instance;
            }
        }

        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("FaPai", ActionFaPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("FaPai");
        }

        #region 发牌动作

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
            if (mjMachineStateData.state != MjMachineState.END)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            FaPaiStateData stateData = mjMachineStateData.GetComponent<FaPaiStateData>();
            stateData.handPaiValueList = mjHandSelfPaiFaceValueList;

            stateData.SetFaPaiData(startPaiIdx, selfHuaPaiValueList, selfBuPaiValueList, opCmdNode);
            mjMachineStateData.SetState(MjMachineState.FAPAI_START, Time.time, -1);
        }

        /// <summary>
        /// 发牌动作
        /// </summary>
        public void ActionFaPai()
        {
            if (mjMachineStateData.state < MjMachineState.FAPAI_START ||
               mjMachineStateData.state > MjMachineState.FAPAI_END ||
               Time.time - mjMachineStateData.stateStartTime < mjMachineStateData.stateLiveTime)
            {
                return;
            }

            FaPaiStateData stateData = mjMachineStateData.GetComponent<FaPaiStateData>();

            switch (mjMachineStateData.state)
            {
                case MjMachineState.FAPAI_START:
                    {
                        if (desk.mjDuiPaiUpDown[stateData.faPaiStartIdx] == MahjongUpDown.MG_DOWN)
                        {
                            if (stateData.faPaiStartIdx != 1 && 
                                desk.mjDuiPaiUpDown[stateData.faPaiStartIdx - 1] == MahjongUpDown.MG_UP)
                            {
                                stateData.faPaiStartIdx--;
                            }
                        }

                        fit.curtPaiDuiPos = stateData.faPaiStartIdx;
                        stateData.faPaiPlayerOrderIdx = fit.orderSeatIdx[0];
                        stateData.faPaiSingleCount = fit.mjDengCount;
                        stateData.faPaiSeat = fit.orderSeatIdx[stateData.faPaiPlayerOrderIdx];
                        stateData.faPaiTurn = 0;

                        stateData.faPaiMjDengCount = stateData.handPaiValueList.Count / fit.mjDengCount;
                        stateData.faPaiMjTailCount = fit.mjHandCount % fit.mjDengCount;
                        if (stateData.faPaiMjTailCount > 0)
                            stateData.faPaiMjDengCount++;

                        audio.PlayEffectAudio(AudioIdx.AUDIO_EFFECT_FAPAI);

                        mjMachineStateData.SetState(MjMachineState.FAPAI_FEN_SINGLE_DENGING, Time.time, 0);
                    }
                    break;

                case MjMachineState.FAPAI_FEN_SINGLE_DENGING:
                    {
                        desk.FenMahjongPaiFromPaiDui(fit.curtPaiDuiPos, stateData.faPaiSingleCount);
                        FanMahjongPai(stateData.faPaiSeat, stateData.faPaiPosIdx, stateData.faPaiSingleCount);

                        //发牌轮次增加
                        stateData.faPaiTurn++;

                        //获取下一个发牌座位
                        stateData.faPaiSeat = fit.GetNextCanUseSeatIdxByOrderSeat(ref stateData.faPaiPlayerOrderIdx);


                        //已经摸牌一圈,回到庄家位，下一轮摸牌
                        if (stateData.faPaiTurn == fit.realPlayerCount)
                        {
                            stateData.faPaiTurn = 0;
                            stateData.faPaiPosIdx++;         //发牌位置按墩数位后移
                        }

                        //当发牌到麻将墩牌的最后一墩时，处理最后的发牌数量
                        if (stateData.faPaiMjTailCount > 0 &&
                            stateData.faPaiPosIdx == stateData.faPaiMjDengCount - 1)
                        {
                            if (stateData.faPaiSeat == fit.orderSeatIdx[0])
                                stateData.faPaiSingleCount = stateData.faPaiMjTailCount;
                            else
                                stateData.faPaiSingleCount = stateData.faPaiMjTailCount - 1;
                        }


                        if (stateData.faPaiPosIdx >= stateData.faPaiMjDengCount)
                        {
                            mjMachineStateData.SetState(MjMachineState.FAPAI_FEN_DENG_END, Time.time, 0.5f);
                        }
                        else
                        {
                            mjMachineStateData.SetState(MjMachineState.FAPAI_FEN_SINGLE_DENGING, Time.time, fit.fenPaiSpeed);
                        }
                    }
                    break;

                case MjMachineState.FAPAI_FEN_DENG_END:
                    {
                        //补花牌
                        if (stateData.selfHuaPaiValueList.Count > 0)
                        {
                            GameObject[] buPaiMjs = new GameObject[stateData.selfHuaPaiValueList.Count];
                            MahjongFaceValue mjFaceValue;
                            Transform huaMjInList;
                            int n = 0;

                            for (int i = 0; i < stateData.selfHuaPaiValueList.Count; i++)
                            {
                                for (int j = 0; j < desk.mjSeatHandPaiLists[0].Count; j++)
                                {
                                    huaMjInList = desk.mjSeatHandPaiLists[0][j].transform;
                                    mjFaceValue = desk.mjSeatHandPaiLists[0][j].GetComponent<MjPaiData>().mjFaceValue;

                                    if (mjFaceValue == stateData.selfHuaPaiValueList[i])
                                    {
                                        buPaiMjs[n] = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(stateData.selfBuPaiValueList[n]);
                                        //buPaiMjs[n].layer = defaultLayer;
                                        //FitSeatCanvasHandMj(buPaiMjs[n]);
                                        //OffMjShadow(buPaiMjs[n]);
                                        //buPaiMjs[n].transform.SetParent(canvasHandPaiTransform, true);

                                        //buPaiMjs[n].transform.localPosition = huaMjInList.localPosition;
                                        //buPaiMjs[n].transform.localEulerAngles = huaMjInList.localEulerAngles;
                                        //buPaiMjs[n].transform.localScale = huaMjInList.localScale;
                                        n++;

                                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(desk.mjSeatHandPaiLists[0][j]);
                                        desk.mjSeatHandPaiLists[0].RemoveAt(j);
                                        j = -1;
                                    }
                                }
                            }

                            mjMachineStateData.SetState(MjMachineState.FAPAI_BUHUA, Time.time, 0);
                        }
                        else
                        {
                            mjMachineStateData.SetState(MjMachineState.FAPAI_SORT, Time.time, 0);
                        }
                    }
                    break;

                case MjMachineState.FAPAI_BUHUA:
                    {
                        SortPaiType sortPaiType = SortPaiType.LEFT;

                        List<GameObject> mjHandPaiList = desk.mjSeatHandPaiLists[0];
                        List<GameObject> mjMoPaiList = desk.mjSeatMoPaiLists[0];
                        float mjSpacing = 0;
                        float mjCount = mjHandPaiList.Count;
                        Transform mjtf;
                        float tm = 0.2f;
                        Vector3 dstPos = new Vector3();

                        float mjsize = fit.GetCanvasHandMjSizeByAxis(Axis.X);
                        float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
                        float mjStartPos = 0;
                        float mjAxisSpacing = mjsize + mjSpacing;

                        switch (sortPaiType)
                        {
                            case SortPaiType.LEFT:
                                mjStartPos = desk.mjSeatHandPaiPosLists[0][0].x;
                                break;

                            case SortPaiType.MIDDLE:
                                mjStartPos = -mjWallLen / 2 + mjsize / 2;
                                break;
                        }


                        dstPos = new Vector3(mjStartPos, desk.mjSeatHandPaiPosLists[0][0].y, desk.mjSeatHandPaiPosLists[0][0].z);

                        for (int i = 0; i < mjCount; i++)
                        {
                            mjtf = mjHandPaiList[i].transform;
                            dstPos = new Vector3(mjStartPos + i * mjAxisSpacing, mjtf.localPosition.y, mjtf.localPosition.z);
                            mjtf.DOLocalMove(dstPos, tm);
                        }

                        if (mjCount > 0)
                            dstPos.x += mjsize + fit.moPaiToHandPaiCanvasOffset;

                        if (mjMoPaiList.Count > 0)
                        {
                            Vector3 offset = dstPos - mjMoPaiList[0].transform.localPosition;

                            for (int i = 0; i < mjMoPaiList.Count; i++)
                            {
                                mjtf = mjMoPaiList[i].transform;
                                dstPos = mjtf.transform.localPosition + offset;
                                mjtf.DOLocalMove(dstPos, tm);
                            }
                        }

                        mjMachineStateData.SetState(MjMachineState.FAPAI_SORT, Time.time, tm);
                    }
                    break;



                case MjMachineState.FAPAI_SORT:
                    {
                        GameObject mj;
                        Tweener t;

                        //整理牌
                        audio.PlayEffectAudio(AudioIdx.AUDIO_EFFECT_SORTPAI);

                        for (int j = 0; j < desk.mjSeatHandPaiLists[0].Count; j++)
                        {
                            mj = desk.mjSeatHandPaiLists[0][j];
                            if (mj == null)
                                continue;

                            Vector3 eulerAngles = mj.transform.localEulerAngles;
                            Vector3 dstPos = mj.transform.position;
                            Sequence seq = DOTween.Sequence();

                            Quaternion q = Quaternion.Euler(new Vector3(eulerAngles.x + fit.canvasFanPaiAfterSortAngles, eulerAngles.y, eulerAngles.z));
                            t = mj.transform.DOLocalRotateQuaternion(q, fit.fanPaiAfterSortPaiSpeed).SetEase(Ease.InCubic);
                            seq.Append(t);

                            q = Quaternion.Euler(eulerAngles);
                            t = mj.transform.DOLocalRotateQuaternion(q, fit.fanPaiAfterSortPaiSpeed).SetEase(Ease.InCubic);
                            seq.Append(t);
                        }

                        mjMachineStateData.SetState(MjMachineState.FAPAI_END, Time.time, fit.fanPaiAfterSortPaiSpeed * 2);
                    }

                    break;


                case MjMachineState.FAPAI_END:
                    {
                        GameObject mj;
                        for (int j = 0; j < desk.mjSeatHandPaiLists[0].Count; j++)
                        {
                            mj = desk.mjSeatHandPaiLists[0][j];
                            if (mj == null)
                                continue;

                            fit.OnMjShadow(mj);
                        }

                        mjMachineStateData.state = MjMachineState.END;
                        ProcessCommonActionmjCmdMgr(stateData);
                    }
                    break;
            }
        }


        void FanMahjongPai(int seatIdx, int posIdx, int showCount)
        {
            GameObject[] mjHandFanPai = CreateMahjongPaiFan(seatIdx, posIdx, showCount);
            Vector3 eulerAngles;

            switch (seatIdx)
            {
                case 0:
                    {
                        for (int i = 0; i < mjHandFanPai.Length; i++)
                        {
                            int idx = posIdx * fit.mjDengCount + i;
                            mjHandFanPai[i].transform.localPosition = desk.mjSeatHandPaiPosLists[0][idx];
                            eulerAngles = mjHandFanPai[i].transform.localEulerAngles;
                            mjHandFanPai[i].transform.localEulerAngles = new Vector3(eulerAngles.x + fit.canvasFanPaiAngles, eulerAngles.y, eulerAngles.z);

                            Quaternion q = Quaternion.Euler(eulerAngles);
                            mjHandFanPai[i].transform.DOLocalRotateQuaternion(q, fit.fanPaiSpeed).SetEase(Ease.Flash);

                            desk.mjSeatHandPaiLists[seatIdx].Add(mjHandFanPai[i]);
                        }
                    }
                    break;

                case 1:
                case 2:
                case 3:
                    {
                        for (int i = 0; i < mjHandFanPai.Length; i++)
                        {
                            int idx = posIdx * fit.mjDengCount + i;
                            mjHandFanPai[i].transform.position = desk.mjSeatHandPaiPosLists[seatIdx][idx];
                            eulerAngles = mjHandFanPai[i].transform.localEulerAngles;
                            mjHandFanPai[i].transform.localEulerAngles = new Vector3(eulerAngles.x + 25f, eulerAngles.y, eulerAngles.z);

                            mjHandFanPai[i].transform.DOLocalRotate(eulerAngles, fit.fanPaiSpeed).SetEase(Ease.Flash);

                            desk.mjSeatHandPaiLists[seatIdx].Add(mjHandFanPai[i]);
                        }
                    }

                    break;
            }
        }

        GameObject[] CreateMahjongPaiFan(int seatIdx, int posIdx, int count)
        {
            GameObject[] mjHandFanPai = new GameObject[count];

            switch (seatIdx)
            {
                case 0:
                    {
                        MahjongFaceValue[] mjFaceValues = GetSelfHandMahjongFaceValues(posIdx * fit.mjDengCount, mjHandFanPai.Length);

                        for (int i = 0; i < count; i++)
                        {
                            GameObject mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mj.layer = mjMachine.defaultLayer;
                            fit.FitSeatCanvasHandMj(mj);
                            fit.OffMjShadow(mj);
                            mj.transform.SetParent(desk.canvasHandPaiTransform, true);
                            mjHandFanPai[i] = mj;
                        }
                    }

                    break;

                case 1:
                case 2:
                case 3:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            GameObject mj = mjAssetsMgr.PopMjFromOtherHandMjPool();
                            fit.FitSeatHandMj(seatIdx, mj);
                            //OffMjShadow(mj);
                            mj.transform.SetParent(desk.mjtableTransform, true);
                            mjHandFanPai[i] = mj;
                        }
                    }
                    break;
            }

            return mjHandFanPai;
        }

        /// <summary>
        /// 获取自身手牌麻将的面值
        /// 需要修改
        /// </summary>
        /// <param name="mjHandPosIdx"></param>
        /// <param name="mjCount"></param>
        /// <returns></returns>
        public MahjongFaceValue[] GetSelfHandMahjongFaceValues(int mjHandPosIdx, int mjCount)
        {
            FaPaiStateData stateData = mjMachineStateData.GetComponent<FaPaiStateData>();

            if (stateData.handPaiValueList.Count < mjCount + mjHandPosIdx)
                return null;

            MahjongFaceValue[] faceValues = new MahjongFaceValue[mjCount];
            int n = 0;

            for (int i = mjHandPosIdx; i < mjHandPosIdx + mjCount; i++)
            {
                faceValues[n++] = stateData.handPaiValueList[i];
            }

            return faceValues;
        }


        #endregion


    }
}
