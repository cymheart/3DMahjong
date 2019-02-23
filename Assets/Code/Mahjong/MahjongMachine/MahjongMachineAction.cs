using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MahjongMachineNS
{
    public partial class MahjongMachine
    {

        #region 洗牌动作
        /// <summary>
        /// 洗牌动作
        /// </summary>
        public void ActionXiPai()
        {
            if (mjMachineStateData.state < MahjongMachineState.XIPAI_START ||
                mjMachineStateData.state > MahjongMachineState.XIPAI_END ||
                Time.time - mjMachineStateData.stateStartTime < mjMachineStateData.stateLiveTime)
            {
                return;
            }

            switch (mjMachineStateData.state)
            {
                case MahjongMachineState.XIPAI_START:
                    {
                        SetDealer(mjMachineStateData.dealerSeatIdx, mjMachineStateData.fengWei);

                        MjTuoXiPaiShengqi(deskMjTuoName[1], deskMjTuoPos[1], 0.2416f, MahjongGameDir.MG_DIR_X);
                        MjTuoXiPaiShengqi(deskMjTuoName[3], deskMjTuoPos[3], -0.2416f, MahjongGameDir.MG_DIR_X);
                        MjTuoXiPaiShengqi(deskMjTuoName[2], deskMjTuoPos[2], -0.2185f, MahjongGameDir.MG_DIR_Z);
                        MjTuoXiPaiShengqi(deskMjTuoName[0], deskMjTuoPos[0], 0.2185f, MahjongGameDir.MG_DIR_Z);

                        mjMachineStateData.SetState(MahjongMachineState.XIPAI_END, Time.time, 1f);

                    }
                    break;

                case MahjongMachineState.XIPAI_END:
                    {
                        mjMachineStateData.state = MahjongMachineState.END;
                        ProcessCommonActionMjOpCmdList();
                    }
                    break;
            }        
        }
        void MjTuoXiPaiShengqi(string mjtuoName, Vector3 mjtuoOrgPos, float xorzOffsetDstValue, MahjongGameDir dir)
        {
            Transform mjtuo_tf = mjtableTransform.Find(mjtuoName);
            Vector3 orgLocalPos = mjtuoOrgPos;

            Sequence seq = DOTween.Sequence();
            Tweener t;

            if (dir == MahjongGameDir.MG_DIR_X)
            {
                orgLocalPos.x = xorzOffsetDstValue;
                orgLocalPos.y = 0.02f;
                mjtuo_tf.localPosition = orgLocalPos;

                t = mjtuo_tf.DOLocalMoveX(mjtuoOrgPos.x, 0.5f).SetEase(Ease.Flash);
                seq.Append(t);

                t = mjtuo_tf.DOLocalMoveY(mjtuoOrgPos.y, 0.5f).SetEase(Ease.Flash);
                seq.Append(t);
            }
            else
            {
                orgLocalPos.z = xorzOffsetDstValue;
                orgLocalPos.y = 0.02f;
                mjtuo_tf.localPosition = orgLocalPos;

                t = mjtuo_tf.DOLocalMoveZ(mjtuoOrgPos.z, 0.5f).SetEase(Ease.Flash);
                seq.Append(t);

                t = mjtuo_tf.DOLocalMoveY(mjtuoOrgPos.y, 0.5f).SetEase(Ease.Flash);
                seq.Append(t);
            }
        }

        #endregion

        #region 启动骰子器动作
        /// <summary>
        /// 启动骰子器动作
        /// </summary>
        void ActionQiDongDiceMachine()
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (isCanUseSeatPlayer[i])
                    ActionQiDongDiceMachine(i);
            }
        }

        void ActionQiDongDiceMachine(int seatIdx)
        {
            if (playerStateData[seatIdx].playerHandActionState < HandActionState.QIDONG_DICEMACHINE_START ||
               playerStateData[seatIdx].playerHandActionState > HandActionState.QIDONG_DICEMACHINE_END)
            {
                return;
            }

            if (Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
            {
                MoveDaPaiHandShadow(seatIdx);
                return;
            }

            PlayerType handStyle = playerStateData[seatIdx].handStyle;

            float waitTime = 0.3f;
            float fadeTime = 0.06f;
            Animation anim = GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);

            switch (playerStateData[seatIdx].playerHandActionState)
            {
                case HandActionState.QIDONG_DICEMACHINE_START:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);
                        waitTime = ReadyFirstHand(seatIdx, handStyle, HandDirection.RightHand, "QiDongDiceMachineReadyHand");

                        playerStateData[seatIdx].handShadowAxis[0] = GetHandShadowAxis(seatIdx, handStyle, HandDirection.RightHand, 0).transform;
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(true);
                        MoveDaPaiHandShadow(seatIdx);


                        playerStateData[seatIdx].SetPlayerState(HandActionState.QIDONG_DICEMACHINE_READY_FIRST_HAND, Time.time, waitTime);

                    }
                    break;


                case HandActionState.QIDONG_DICEMACHINE_READY_FIRST_HAND:
                    {
                        FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, ActionCombineNum.QiDongDiceMachine);

                        waitTime = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, diceQiDongPosSeat[seatIdx], ActionCombineNum.QiDongDiceMachine);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.QIDONG_DICEMACHINE_MOVE_HAND_TO_DST_POS, Time.time, waitTime);
                    }
                    break;

                case HandActionState.QIDONG_DICEMACHINE_MOVE_HAND_TO_DST_POS:
                    {
                        anim.CrossFade("QiDongDiceMachine", fadeTime);
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "QiDongDiceMachine");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.QIDONG_DICEMACHINE_QIDONG, Time.time, waitTime);
                    }
                    break;

                case HandActionState.QIDONG_DICEMACHINE_QIDONG:
                    {
                        if (HandQiDongDiceMachine != null)
                            HandQiDongDiceMachine(seatIdx, playerStateData[seatIdx].dice1Point, playerStateData[seatIdx].dice2Point);



                        anim.CrossFade("QiDongDiceMachineEndTaiHand", fadeTime);
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "QiDongDiceMachineEndTaiHand");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.QIDONG_DICEMACHINE_TAIHAND, Time.time, waitTime);
                    }
                    break;


                case HandActionState.QIDONG_DICEMACHINE_TAIHAND:
                    {
                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.RightHand, ActionCombineNum.QiDongDiceMachine);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.QIDONG_DICEMACHINE_END, Time.time, waitTime + 4f);
                    }
                    break;

                case HandActionState.QIDONG_DICEMACHINE_END:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(false);
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(false);
                        playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;

                        ProcessHandActionMjOpCmdList(seatIdx);
                    }
                    break;
            }
        }

        #endregion

        #region 发牌动作
        /// <summary>
        /// 发牌动作
        /// </summary>
        void ActionFaPai()
        {
            if (mjMachineStateData.state < MahjongMachineState.FAPAI_START ||
               mjMachineStateData.state > MahjongMachineState.FAPAI_END ||
               Time.time - mjMachineStateData.stateStartTime < mjMachineStateData.stateLiveTime)
            {
                return;
            }

            switch (mjMachineStateData.state)
            {
                case MahjongMachineState.FAPAI_START:
                    {
                        if (mjDuiPaiUpDown[mjMachineStateData.faPaiStartIdx] == MahjongUpDown.MG_DOWN)
                        {
                            if (mjMachineStateData.faPaiStartIdx != 1 && mjDuiPaiUpDown[mjMachineStateData.faPaiStartIdx - 1] == MahjongUpDown.MG_UP)
                            {
                                mjMachineStateData.faPaiStartIdx--;
                            }
                        }

                        curtPaiDuiPos = mjMachineStateData.faPaiStartIdx;
                        mjMachineStateData.faPaiPlayerOrderIdx = orderSeatIdx[0];
                        mjMachineStateData.faPaiSingleCount = mjDengCount;
                        mjMachineStateData.faPaiSeat = orderSeatIdx[mjMachineStateData.faPaiPlayerOrderIdx];
                        mjMachineStateData.faPaiTurn = 0;

                        mjMachineStateData.faPaiMjDengCount = playerStateData[0].handPaiValueList.Count / mjDengCount;
                        mjMachineStateData.faPaiMjTailCount = mjHandCount % mjDengCount;
                        if (mjMachineStateData.faPaiMjTailCount > 0)
                            mjMachineStateData.faPaiMjDengCount++;

                        AudioClip audioClip = GetEffectAudio((int)AudioIdx.AUDIO_EFFECT_FAPAI);
                        AudioSource.PlayClipAtPoint(audioClip, cameraTransform.position);

                        mjMachineStateData.SetState(MahjongMachineState.FAPAI_FEN_SINGLE_DENGING, Time.time, 0);
                    }
                    break;

                case MahjongMachineState.FAPAI_FEN_SINGLE_DENGING:
                    {
                        FenMahjongPaiFromPaiDui(curtPaiDuiPos, mjMachineStateData.faPaiSingleCount);
                        FanMahjongPai(mjMachineStateData.faPaiSeat, mjMachineStateData.faPaiPosIdx, mjMachineStateData.faPaiSingleCount);

                        //发牌轮次增加
                        mjMachineStateData.faPaiTurn++;

                        //获取下一个发牌座位
                        mjMachineStateData.faPaiSeat = GetNextCanUseSeatIdxByOrderSeat(ref mjMachineStateData.faPaiPlayerOrderIdx);

                     
                        //已经摸牌一圈,回到庄家位，下一轮摸牌
                        if (mjMachineStateData.faPaiTurn == realPlayerCount)
                        {
                            mjMachineStateData.faPaiTurn = 0;
                            mjMachineStateData.faPaiPosIdx++;         //发牌位置按墩数位后移
                        }

                        //当发牌到麻将墩牌的最后一墩时，处理最后的发牌数量
                        if (mjMachineStateData.faPaiMjTailCount > 0 &&
                            mjMachineStateData.faPaiPosIdx == mjMachineStateData.faPaiMjDengCount - 1)
                        {
                            if (mjMachineStateData.faPaiSeat == orderSeatIdx[0])
                                mjMachineStateData.faPaiSingleCount = mjMachineStateData.faPaiMjTailCount;
                            else
                                mjMachineStateData.faPaiSingleCount = mjMachineStateData.faPaiMjTailCount - 1;
                        }


                        if (mjMachineStateData.faPaiPosIdx >= mjMachineStateData.faPaiMjDengCount)
                        {
                            mjMachineStateData.SetState(MahjongMachineState.FAPAI_FEN_DENG_END, Time.time, 0.5f);
                        }
                        else
                        {
                            mjMachineStateData.SetState(MahjongMachineState.FAPAI_FEN_SINGLE_DENGING, Time.time, fenPaiSpeed);
                        }
                    }
                    break;

                case MahjongMachineState.FAPAI_FEN_DENG_END:
                    {
                        //补花牌
                        if (mjMachineStateData.selfHuaPaiValueList.Count > 0)
                        {
                            GameObject[] buPaiMjs = new GameObject[mjMachineStateData.selfHuaPaiValueList.Count];
                            MahjongFaceValue mjFaceValue;
                            Transform huaMjInList;
                            int n = 0;

                            for (int i = 0; i < mjMachineStateData.selfHuaPaiValueList.Count; i++)
                            {
                                for (int j = 0; j < mjSeatHandPaiLists[0].Count; j++)
                                {
                                    huaMjInList = mjSeatHandPaiLists[0][j].transform;
                                    mjFaceValue = mjSeatHandPaiLists[0][j].GetComponent<MjPaiData>().mjFaceValue;

                                    if (mjFaceValue == mjMachineStateData.selfHuaPaiValueList[i])
                                    {
                                        buPaiMjs[n] = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjMachineStateData.selfBuPaiValueList[n]);
                                        //buPaiMjs[n].layer = defaultLayer;
                                        //FitSeatCanvasHandMj(buPaiMjs[n]);
                                        //OffMjShadow(buPaiMjs[n]);
                                        //buPaiMjs[n].transform.SetParent(canvasHandPaiTransform, true);

                                        //buPaiMjs[n].transform.localPosition = huaMjInList.localPosition;
                                        //buPaiMjs[n].transform.localEulerAngles = huaMjInList.localEulerAngles;
                                        //buPaiMjs[n].transform.localScale = huaMjInList.localScale;
                                        n++;

                                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mjSeatHandPaiLists[0][j]);
                                        mjSeatHandPaiLists[0].RemoveAt(j);
                                        j = -1;
                                    }
                                }
                            }

                            mjMachineStateData.SetState(MahjongMachineState.FAPAI_BUHUA, Time.time, 0);
                        }
                        else
                        {
                            mjMachineStateData.SetState(MahjongMachineState.FAPAI_SORT, Time.time, 0);
                        }
                    }
                    break;

                case MahjongMachineState.FAPAI_BUHUA:
                    {
                        SortPaiType sortPaiType = SortPaiType.LEFT;

                        List<GameObject> mjHandPaiList = mjSeatHandPaiLists[0];
                        List<GameObject> mjMoPaiList = mjSeatMoPaiLists[0];
                        float mjSpacing = 0;
                        float mjCount = mjHandPaiList.Count;
                        Transform mjtf;
                        float tm = 0.2f;
                        Vector3 dstPos = new Vector3();

                        float mjsize = GetCanvasHandMjSizeByAxis(Axis.X);
                        float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
                        float mjStartPos = 0;
                        float mjAxisSpacing = mjsize + mjSpacing;

                        switch (sortPaiType)
                        {
                            case SortPaiType.LEFT:
                                mjStartPos = mjSeatHandPaiPosLists[0][0].x;
                                break;

                            case SortPaiType.MIDDLE:
                                mjStartPos = -mjWallLen / 2 + mjsize / 2;
                                break;
                        }


                        dstPos = new Vector3(mjStartPos, mjSeatHandPaiPosLists[0][0].y, mjSeatHandPaiPosLists[0][0].z);

                        for (int i = 0; i < mjCount; i++)
                        {
                            mjtf = mjHandPaiList[i].transform;
                            dstPos = new Vector3(mjStartPos + i * mjAxisSpacing, mjtf.localPosition.y, mjtf.localPosition.z);
                            mjtf.DOLocalMove(dstPos, tm);
                        }

                        if (mjCount > 0)
                            dstPos.x += mjsize + moPaiToHandPaiCanvasOffset;

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

                        mjMachineStateData.SetState(MahjongMachineState.FAPAI_SORT, Time.time, tm);
                    }
                    break;



                case MahjongMachineState.FAPAI_SORT:
                    {
                        GameObject mj;
                        Tweener t;

                        //整理牌
                        PlayEffectAudio(AudioIdx.AUDIO_EFFECT_SORTPAI);

                        for (int j = 0; j < mjSeatHandPaiLists[0].Count; j++)
                        {
                            mj = mjSeatHandPaiLists[0][j];
                            if (mj == null)
                                continue;

                            Vector3 eulerAngles = mj.transform.localEulerAngles;
                            Vector3 dstPos = mj.transform.position;
                            Sequence seq = DOTween.Sequence();

                            Quaternion q = Quaternion.Euler(new Vector3(eulerAngles.x + canvasFanPaiAfterSortAngles, eulerAngles.y, eulerAngles.z));
                            t = mj.transform.DOLocalRotateQuaternion(q, fanPaiAfterSortPaiSpeed).SetEase(Ease.InCubic);
                            seq.Append(t);

                            q = Quaternion.Euler(eulerAngles);
                            t = mj.transform.DOLocalRotateQuaternion(q, fanPaiAfterSortPaiSpeed).SetEase(Ease.InCubic);
                            seq.Append(t);
                        }

                        mjMachineStateData.SetState(MahjongMachineState.FAPAI_END, Time.time, fanPaiAfterSortPaiSpeed * 2);
                    }

                    break;


                case MahjongMachineState.FAPAI_END:
                    {
                        GameObject mj;
                        for (int j = 0; j < mjSeatHandPaiLists[0].Count; j++)
                        {
                            mj = mjSeatHandPaiLists[0][j];
                            if (mj == null)
                                continue;

                            OnMjShadow(mj);
                        }

                        mjMachineStateData.state = MahjongMachineState.END;
                        ProcessCommonActionMjOpCmdList();
                    }
                    break;
            }
        }

        void FenMahjongPaiFromPaiDui(int fenPaiStartIdx, int fenPaiCount)
        {
            if (mjDuiPai[fenPaiStartIdx] == null)
                return;

            if (fenPaiCount > paiDuiRichCount)
                fenPaiCount = paiDuiRichCount;

            for (int i = 0; i < fenPaiCount;)
            {
                if (fenPaiStartIdx > mjPaiTotalCount)
                    fenPaiStartIdx -= mjPaiTotalCount;

                if (mjDuiPai[fenPaiStartIdx].activeSelf == false)
                {
                    fenPaiStartIdx++;
                    continue;
                }

                mjDuiPai[fenPaiStartIdx].SetActive(false);
                fenPaiStartIdx++;
                paiDuiRichCount--;
                i++;
            }

            curtPaiDuiPos = fenPaiStartIdx;
            if (curtPaiDuiPos > mjPaiTotalCount)
                curtPaiDuiPos -= mjPaiTotalCount;
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
                            int idx = posIdx * mjDengCount + i;
                            mjHandFanPai[i].transform.localPosition = mjSeatHandPaiPosLists[0][idx];
                            eulerAngles = mjHandFanPai[i].transform.localEulerAngles;
                            mjHandFanPai[i].transform.localEulerAngles = new Vector3(eulerAngles.x + canvasFanPaiAngles, eulerAngles.y, eulerAngles.z);

                            Quaternion q = Quaternion.Euler(eulerAngles);
                            mjHandFanPai[i].transform.DOLocalRotateQuaternion(q, fanPaiSpeed).SetEase(Ease.Flash);

                            mjSeatHandPaiLists[seatIdx].Add(mjHandFanPai[i]);
                        }
                    }
                    break;

                case 1:
                case 2:
                case 3:
                    {
                        for (int i = 0; i < mjHandFanPai.Length; i++)
                        {
                            int idx = posIdx * mjDengCount + i;
                            mjHandFanPai[i].transform.position = mjSeatHandPaiPosLists[seatIdx][idx];
                            eulerAngles = mjHandFanPai[i].transform.localEulerAngles;
                            mjHandFanPai[i].transform.localEulerAngles = new Vector3(eulerAngles.x + 25f, eulerAngles.y, eulerAngles.z);

                            mjHandFanPai[i].transform.DOLocalRotate(eulerAngles, fanPaiSpeed).SetEase(Ease.Flash);

                            mjSeatHandPaiLists[seatIdx].Add(mjHandFanPai[i]);
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
                        MahjongFaceValue[] mjFaceValues = GetSelfHandMahjongFaceValues(posIdx * mjDengCount, mjHandFanPai.Length);

                        for (int i = 0; i < count; i++)
                        {
                            GameObject mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mj.layer = defaultLayer;
                            FitSeatCanvasHandMj(mj);
                            OffMjShadow(mj);
                            mj.transform.SetParent(canvasHandPaiTransform, true);
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
                            FitSeatHandMj(seatIdx, mj);
                            //OffMjShadow(mj);
                            mj.transform.SetParent(mjtableTransform, true);
                            mjHandFanPai[i] = mj;
                        }
                    }
                    break;
            }

            return mjHandFanPai;
        }

        #endregion

        #region 选交换牌动作
        /// <summary>
        /// 选交换牌动作
        /// </summary>
        public void ActionSelectSwapPai()
        {
            if (playerStateData[0].playerHandActionState < HandActionState.SELECT_SWAP_PAI_START ||
                playerStateData[0].playerHandActionState > HandActionState.SELECT_SWAP_PAI_END ||
                Time.time - playerStateData[0].playerStateStartTime < playerStateData[0].playerStateLiveTime)
            {
                return;
            }

            switch (playerStateData[0].playerHandActionState)
            {
                case HandActionState.SELECT_SWAP_PAI_START:
                    {
                        uiSelectSwapHandPai.Show();
                        playerStateData[0].SetPlayerState(HandActionState.SELECT_SWAP_PAI_SELECTTING, Time.time, -1);
                    }
                    break;

                case HandActionState.SELECT_SWAP_PAI_SELECTTING:
                    {
                        int selectedCount = playerStateData[0].rayClickPickHandPaiMjIdxList.Count + playerStateData[0].rayClickPickMoPaiMjIdxList.Count;

                        if (selectedCount >= 3)
                        {
                            uiSelectSwapHandPai.Enable();
                        }
                        else
                        {
                            uiSelectSwapHandPai.Disable();
                        }

                        if (Input.GetMouseButtonDown(0))
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hitInfo;

                            if (Physics.Raycast(ray, out hitInfo))
                            {
                                GameObject mj = hitInfo.collider.gameObject;
                                int selectIdx = mjSeatHandPaiLists[0].IndexOf(mj);

                                if (mjSeatHandPaiLists[0].Contains(mj))
                                {
                                    selectIdx = mjSeatHandPaiLists[0].IndexOf(mj);

                                    if (playerStateData[0].rayClickPickHandPaiMjIdxList.Contains(selectIdx))
                                    {
                                        playerStateData[0].rayClickPickHandPaiMjIdxList.Remove(selectIdx);
                                        playerStateData[0].rayPickMjOrgPos = mj.transform.localPosition;
                                        mj.transform.localPosition = new Vector3(playerStateData[0].rayPickMjOrgPos.x, playerStateData[0].rayPickMjOrgPos.y - handPaiSelectOffsetHeight, playerStateData[0].rayPickMjOrgPos.z);
                                        OnMjShadow(mj);
                                    }
                                    else
                                    {
                                        playerStateData[0].rayClickPickHandPaiMjIdxList.Add(selectIdx);
                                        playerStateData[0].rayPickMjOrgPos = mj.transform.localPosition;
                                        mj.transform.localPosition = new Vector3(playerStateData[0].rayPickMjOrgPos.x, playerStateData[0].rayPickMjOrgPos.y + handPaiSelectOffsetHeight, playerStateData[0].rayPickMjOrgPos.z);
                                        OffMjShadow(mj);
                                    }
                                }
                                else
                                {
                                    selectIdx = mjSeatMoPaiLists[0].IndexOf(mj);

                                    if (playerStateData[0].rayClickPickMoPaiMjIdxList.Contains(selectIdx))
                                    {
                                        playerStateData[0].rayClickPickMoPaiMjIdxList.Remove(selectIdx);
                                        playerStateData[0].rayPickMjOrgPos = mj.transform.localPosition;
                                        mj.transform.localPosition = new Vector3(playerStateData[0].rayPickMjOrgPos.x, playerStateData[0].rayPickMjOrgPos.y - handPaiSelectOffsetHeight, playerStateData[0].rayPickMjOrgPos.z);
                                        OnMjShadow(mj);
                                    }
                                    else
                                    {
                                        playerStateData[0].rayClickPickMoPaiMjIdxList.Add(selectIdx);
                                        playerStateData[0].rayPickMjOrgPos = mj.transform.localPosition;
                                        mj.transform.localPosition = new Vector3(playerStateData[0].rayPickMjOrgPos.x, playerStateData[0].rayPickMjOrgPos.y + handPaiSelectOffsetHeight, playerStateData[0].rayPickMjOrgPos.z);
                                        OffMjShadow(mj);
                                    }
                                }
                            }
                        }
                        else if (uiSelectSwapHandPai.IsOkClicked)
                        {
                            if (SelfSelectSwapPaiEnd != null)
                            {
                                SelfSelectSwapPaiEnd(playerStateData[0].rayClickPickHandPaiMjIdxList.ToArray(), playerStateData[0].rayClickPickMoPaiMjIdxList.ToArray());
                            }

                            playerStateData[0].rayClickPickHandPaiMjIdxList.Clear();
                            playerStateData[0].rayClickPickMoPaiMjIdxList.Clear();
                            playerStateData[0].SetPlayerState(HandActionState.SELECT_SWAP_PAI_END, Time.time, -1);
                        }
                    }
                    break;

                case HandActionState.SELECT_SWAP_PAI_END:
                    {
                        if (uiSelectSwapHandPai.IsCompleteSwapPaiSelected == true)
                        {
                            playerStateData[0].playerHandActionState = HandActionState.ACTION_END;
                            ProcessHandActionMjOpCmdList(0);
                        }
                    }
                    break;

            }

        }

        #endregion

        #region 换牌提示动作
        /// <summary>
        /// 换牌提示动作
        /// </summary>
        public void ActionSwapPaiHint()
        {
            if (swapPaiHintStateData.state < SwapPaiHintState.HINT_START ||
                swapPaiHintStateData.state >= SwapPaiHintState.HINT_END ||
                Time.time - swapPaiHintStateData.stateStartTime < swapPaiHintStateData.stateLiveTime)
            {
                return;
            }


            switch (swapPaiHintStateData.state)
            {
                case SwapPaiHintState.HINT_START:
                    {
                        uiSwapPaiingTips.SetHintSwapType(swapPaiHintStateData.swapPaiDirection);
                        uiSwapPaiingTips.Show();
                        swapPaiHintArrowEffect.ShowArrow(swapPaiHintStateData.swapPaiDirection);

                        swapPaiHintStateData.SetState(SwapPaiHintState.HINTTING, Time.time, 2f);
                    }
                    break;

                case SwapPaiHintState.HINTTING:
                    {
                        uiSwapPaiingTips.Hide();
                        swapPaiHintArrowEffect.HideArrow(swapPaiHintStateData.swapPaiDirection);
                        swapPaiHintStateData.state = SwapPaiHintState.HINT_END;
                    }
                    break;
            }
        }

        #endregion

        #region 换牌动作
        /// <summary>
        /// 换牌动作
        /// </summary>
        void ActionSwapPai()
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (isCanUseSeatPlayer[i])
                    ActionSwapPai(i);
            }
        }
        void ActionSwapPai(int seatIdx)
        {
            if (playerStateData[seatIdx].playerHandActionState < HandActionState.SWAP_PAI_START ||
               playerStateData[seatIdx].playerHandActionState > HandActionState.SWAP_PAI_END)
            {
                return;
            }

            if (Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
            {
                MoveDaPaiHandShadow(seatIdx);
                return;
            }

            PlayerType handStyle = playerStateData[seatIdx].handStyle;
            Vector3 orgPos = playerStateData[seatIdx].swapPaiFromPos;
            ActionCombineNum actionCombineNum = ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand;

            float waitTime = 0.3f;
            Animation anim = GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);
            float fadeTime = 0.06f;

            switch (playerStateData[seatIdx].playerHandActionState)
            {
                case HandActionState.SWAP_PAI_START:
                    {
                        List<GameObject> mjHandPaiList = mjSeatHandPaiLists[seatIdx];
                        int[] fromIdx = playerStateData[seatIdx].swapPaiFromSeatPaiIdxs;

                        for (int i = 0; i < fromIdx.Length; i++)
                        {
                            if (seatIdx == 0)
                                mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mjHandPaiList[fromIdx[i]]);
                            else
                                mjAssetsMgr.PushMjToOtherHandMjPool(mjHandPaiList[fromIdx[i]]);

                            mjHandPaiList[fromIdx[i]] = null;
                        }

                        mjHandPaiList.RemoveAll(n => n == null);


                        if (playerStateData[seatIdx].swapPaiFromSeatMoPaiIdxs != null)
                        {
                            List<GameObject> mjMoPaiList = mjSeatMoPaiLists[seatIdx];
                            int[] fromMoPaiIdx = playerStateData[seatIdx].swapPaiFromSeatMoPaiIdxs;

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

                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);
                        waitTime = ReadyFirstHand(seatIdx, handStyle, HandDirection.RightHand, "DaPaiFirstHand");
                        playerStateData[seatIdx].handShadowAxis[0] = GetHandShadowAxis(seatIdx, handStyle, HandDirection.RightHand, 0).transform;
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(true);
                        MoveDaPaiHandShadow(seatIdx);

                        playerStateData[seatIdx].SetPlayerState(HandActionState.SWAP_PAI_READY_FIRST_HAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.SWAP_PAI_READY_FIRST_HAND:
                    {
                        FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, actionCombineNum);
                        waitTime = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, orgPos, actionCombineNum);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.SWAP_PAI_MOVE_HAND_TO_DST_POS, Time.time, waitTime);
                    }
                    break;

                case HandActionState.SWAP_PAI_MOVE_HAND_TO_DST_POS:
                    {
                        anim.CrossFade("FirstTaiHand2EndDaPai4", fadeTime);
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand2EndDaPai4");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.SWAP_PAI_CHUPAI, Time.time, waitTime);
                    }
                    break;

                case HandActionState.SWAP_PAI_CHUPAI:
                    {
                        AudioClip clip = GetEffectAudio((int)AudioIdx.AUDIO_EFFECT_GIVE);
                        AudioSource.PlayClipAtPoint(clip, orgPos);

                        MahjongFaceValue[] mjFaceValues = playerStateData[seatIdx].swapPaiFaceValues;

                        if (playerStateData[seatIdx].swapPaiMoPaiFaceValues != null)
                        {
                            int count = playerStateData[seatIdx].swapPaiFaceValues.Length + playerStateData[seatIdx].swapPaiMoPaiFaceValues.Length;
                            mjFaceValues = new MahjongFaceValue[count];

                            for (int i = 0; i < playerStateData[seatIdx].swapPaiFaceValues.Length; i++)
                                mjFaceValues[i] = playerStateData[seatIdx].swapPaiFaceValues[i];

                            int j = 0;
                            for (int i = playerStateData[seatIdx].swapPaiFaceValues.Length; i < count; i++)
                                mjFaceValues[i] = playerStateData[seatIdx].swapPaiMoPaiFaceValues[j++];
                        }

                        playerStateData[seatIdx].swapPaiRotControler = CreateSwapPaiGroup(
                            seatIdx, orgPos, mjFaceValues, playerStateData[seatIdx].swapPaiDir, playerStateData[seatIdx].swapPaiIsShowBack);

                        anim.Play(taiHandActionName[(int)actionCombineNum]);
                        MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, handActionLevelScreenPosSeat[seatIdx]);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.SWAP_PAI_CHUPAI_TAIHAND, Time.time, waitTime);

                    }
                    break;

                case HandActionState.SWAP_PAI_CHUPAI_TAIHAND:
                    {
                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.RightHand, actionCombineNum);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.SWAP_PAI_TAIHAND_END, Time.time, waitTime);
                    }
                    break;

                case HandActionState.SWAP_PAI_TAIHAND_END:
                    {
                        waitTime = 1f;

                        if (playerStateData[seatIdx].swapPaiDir == SwapPaiDirection.OPPOSITE)
                        {
                            Vector3 dstCenterPos = swapPaiCenterPosSeat[playerStateData[seatIdx].swapPaiToSeatIdx];
                            Vector3 dstpos = new Vector3(dstCenterPos.x, deskFacePosY + GetDeskMjSizeByAxis(Axis.Z) / 2, dstCenterPos.z);
                            playerStateData[seatIdx].swapPaiRotControler.transform.DOMove(dstpos, waitTime);
                        }
                        else
                        {
                            playerStateData[seatIdx].swapPaiRotControler.transform.DORotate(new Vector3(0, playerStateData[seatIdx].swapPaiToSeatIdx * 90, 0), waitTime);
                        }

                        playerStateData[seatIdx].SetPlayerState(HandActionState.SWAP_PAI_ROTATE, Time.time, waitTime);
                    }

                    break;

                case HandActionState.SWAP_PAI_ROTATE:
                    {
                        Transform tf = playerStateData[seatIdx].swapPaiRotControler.transform;
                        GameObject go;
                        int count = tf.childCount;
                        for (int i = 0; i < count; i++)
                        {
                            go = tf.GetChild(0).gameObject;
                            mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(go);
                        }

                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(false);
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(false);

                        waitTime = 1f;

                        playerStateData[seatIdx].swapPaiToSeatTakeMjs =
                            TakeMjsInToHandPaiList(
                                playerStateData[seatIdx].swapPaiToSeatIdx,
                                playerStateData[seatIdx].swapPaiToSeatPaiIdxs,
                                playerStateData[seatIdx].swapPaiFaceValues,
                                waitTime);

                        playerStateData[seatIdx].SetPlayerState(HandActionState.SWAP_PAI_END, Time.time, waitTime);

                    }
                    break;

                case HandActionState.SWAP_PAI_END:
                    {
                        if (playerStateData[seatIdx].swapPaiToSeatIdx == 0)
                        {
                            int shadowIdx = playerStateData[seatIdx].swapPaiToSeatIdx == 0 ? 1 : 0;
                            for (int i = 0; i < playerStateData[seatIdx].swapPaiToSeatTakeMjs.Length; i++)
                            {
                                OnMjShadow(playerStateData[seatIdx].swapPaiToSeatTakeMjs[i], shadowIdx);
                            }
                        }

                        playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;
                        ProcessHandActionMjOpCmdList(seatIdx);
                    }
                    break;
            }

        }

        GameObject[] TakeMjsInToHandPaiList(int seatIdx, int[] paiIdx, MahjongFaceValue[] mjFaceValues, float waitTime)
        {
            List<GameObject> mjHandPaiList = mjSeatHandPaiLists[seatIdx];
            float mjSpacing = 0;
            float mjHeight = GetHandMjSizeByAxis(Axis.Y);
            float mjWidth = GetHandMjSizeByAxis(Axis.X);
            GameObject[] mjs = null;

            switch (seatIdx)
            {
                case 0:
                    {
                        float mjCount = mjHandPaiList.Count + paiIdx.Length;
                        mjHeight = GetCanvasHandMjSizeByAxis(Axis.Y);
                        mjWidth = GetCanvasHandMjSizeByAxis(Axis.X);

                        float mjWallLen = mjCount * mjWidth + (mjCount - 1) * mjSpacing;
                        float mjStartPos = -mjWallLen / 2 + mjWidth / 2;
                        float mjAxisSpacing = mjWidth + mjSpacing;
                        Vector3 mjRefPos;

                        if (mjHandPaiList.Count > 0)
                            mjRefPos = mjHandPaiList[0].transform.localPosition;
                        else
                            mjRefPos = mjSeatHandPaiPosLists[seatIdx][0];


                        mjs = new GameObject[paiIdx.Length];
                        for (int i = 0; i < paiIdx.Length; i++)
                        {
                            mjs[i] = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mjs[i].layer = defaultLayer;
                            FitSeatCanvasHandMj(mjs[i], false);
                            mjs[i].transform.SetParent(canvasHandPaiTransform, true);
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
                        float mjStartPos = -mjWallLen / 2 + mjWidth / 2 + mjtableTransform.transform.position.z;
                        float mjAxisSpacing = 0;
                        Vector3 mjRefPos;

                        if (seatIdx == 1)
                        {
                            mjStartPos = -mjWallLen / 2 + mjWidth / 2 + mjtableTransform.transform.position.z;
                            mjAxisSpacing = mjWidth + mjSpacing;
                        }
                        else
                        {
                            mjStartPos = mjWallLen / 2 - mjWidth / 2 + mjtableTransform.transform.position.z;
                            mjAxisSpacing = -(mjWidth + mjSpacing);
                        }

                        if (mjHandPaiList.Count > 0)
                            mjRefPos = mjHandPaiList[0].transform.position;
                        else
                            mjRefPos = mjSeatHandPaiPosLists[seatIdx][0];

                        mjs = new GameObject[paiIdx.Length];
                        for (int i = 0; i < paiIdx.Length; i++)
                        {
                            mjs[i] = mjAssetsMgr.PopMjFromOtherHandMjPool();
                            FitSeatHandMj(seatIdx, mjs[i]);
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
                        float mjStartPos = -mjWallLen / 2 + mjWidth / 2 + mjtableTransform.transform.position.x;
                        float mjAxisSpacing = mjWidth + mjSpacing;
                        Vector3 mjRefPos;

                        if (mjHandPaiList.Count > 0)
                            mjRefPos = mjHandPaiList[0].transform.position;
                        else
                            mjRefPos = mjSeatHandPaiPosLists[seatIdx][0];


                        mjs = new GameObject[paiIdx.Length];
                        for (int i = 0; i < paiIdx.Length; i++)
                        {
                            mjs[i] = mjAssetsMgr.PopMjFromOtherHandMjPool();
                            FitSeatHandMj(seatIdx, mjs[i]);
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


        #endregion

        #region 选择缺门动作
        public void ActionSelectQueMen()
        {
            if (playerStateData[0].playerHandActionState < HandActionState.SELECT_QUE_MEN_START ||
                playerStateData[0].playerHandActionState > HandActionState.SELECT_QUE_MEN_END ||
                Time.time - playerStateData[0].playerStateStartTime < playerStateData[0].playerStateLiveTime)
            {
                return;
            }

            switch (playerStateData[0].playerHandActionState)
            {
                case HandActionState.SELECT_QUE_MEN_START:
                    {
                        playerStateData[0].queMenIsPlayDownAudio = false;
                        playerStateData[0].queMenIsPlayFeiDingQueAudio = false;
                        uiSelectQueMen.Show();
                        playerStateData[0].SetPlayerState(HandActionState.SELECT_QUE_MEN_SELECTTING, Time.time, -1);
                    }
                    break;

                case HandActionState.SELECT_QUE_MEN_SELECTTING:
                    {
                        if (uiSelectQueMen.IsClicked)
                        {
                            PlayEffectAudio(AudioIdx.AUDIO_EFFECT_DOWN);

                            PlayEffectAudioOpCmd cmd = (PlayEffectAudioOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.PlayEffectAudio);
                            cmd.audioIdx = AudioIdx.AUDIO_EFFECT_DOWN;
                            cmd.delayExecuteTime = 0.07f;
                            mjOpCmdList.AppendCmdToDelayCmdList(cmd);

                            cmd = (PlayEffectAudioOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.PlayEffectAudio);
                            cmd.audioIdx = AudioIdx.AUDIO_EFFECT_FEIDINGQUE;
                            cmd.delayExecuteTime = 0.6f;
                            mjOpCmdList.AppendCmdToDelayCmdList(cmd);

                            playerStateData[0].queMenHuaSe = uiSelectQueMen.ClickedHuaSe;

                            if (SelfSelectQueMenEnd != null)
                                SelfSelectQueMenEnd(playerStateData[0].queMenHuaSe);

                            playerStateData[0].SetPlayerState(HandActionState.SELECT_QUE_MEN_END, Time.time, 0);
                        }
                    }
                    break;


                case HandActionState.SELECT_QUE_MEN_END:
                    {

                        if (uiSelectQueMen.IsCompleteQueMenSelected == true)
                        {
                            playerStateData[0].playerHandActionState = HandActionState.ACTION_END;
                            ProcessHandActionMjOpCmdList(0);
                        }
                        else
                        {
                            uiSelectQueMen.PlayQueMenFromList();
                        }
                    }
                    break;

            }
        }

        #endregion

        #region 选择碰吃杠胡听牌动作
        /// <summary>
        /// 选碰吃杠胡听牌动作
        /// </summary>
        public void ActionSelectPCGTHPai()
        {
            if (playerStateData[0].playerHandActionState < HandActionState.SELECT_PCGTH_PAI_START ||
                playerStateData[0].playerHandActionState > HandActionState.SELECT_PCGTH_PAI_END ||
                Time.time - playerStateData[0].playerStateStartTime < playerStateData[0].playerStateLiveTime)
            {
                return;
            }

            switch (playerStateData[0].playerHandActionState)
            {
                case HandActionState.SELECT_PCGTH_PAI_START:
                    {
                        uiPcghtBtnMgr.Show(playerStateData[0].selectPcgthBtnTypes);
                        playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECTTING, Time.time, -1);
                    }
                    break;

                case HandActionState.SELECT_PCGTH_PAI_SELECTTING:
                    {
                        if (uiPcghtBtnMgr.IsClicked == false)
                            break;

                        playerStateData[0].selectPcgthedType = uiPcghtBtnMgr.clickedBtnType;
                        playerStateData[0].selectPcgthedChiPaiMjValueIdx = -1;
                        playerStateData[0].selectPaiHandPaiIdx = -1;

                        switch (uiPcghtBtnMgr.clickedBtnType)
                        {
                            case PengChiGangTingHuType.CHI:
                                {
                                    if (playerStateData[0].selectPcgthChiPaiMjValueList == null)
                                        break;

                                    uiChiPaiTips.Show(playerStateData[0].selectPcgthChiPaiMjValueList);
                                    uiPcghtBtnMgr.Show(new PengChiGangTingHuType[] { PengChiGangTingHuType.CANCEL }, new Vector3(100f, 0, 0));
                                    playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECTTING_CHIPAI, Time.time, -1);
                                }
                                break;


                            case PengChiGangTingHuType.TING:
                                {
                                    uiPcghtBtnMgr.Show(new PengChiGangTingHuType[] { PengChiGangTingHuType.CANCEL }, new Vector3(100f, 0, 0));
                                    playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_START, Time.time, -1);
                                }
                                break;

                            default:
                                playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_END, Time.time, -1);
                                break;

                        }

                    }
                    break;

                case HandActionState.SELECT_PCGTH_PAI_SELECTTING_CHIPAI:
                    {
                        if (uiChiPaiTips.selectedIdx != -1)
                        {
                            uiPcghtBtnMgr.Hide();
                            playerStateData[0].selectPcgthedChiPaiMjValueIdx = uiChiPaiTips.selectedIdx;
                            playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_END, Time.time, -1);
                        }
                        else if (uiPcghtBtnMgr.IsClicked == true)
                        {
                            uiChiPaiTips.Hide();
                            playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_START, Time.time, -1);
                        }
                    }
                    break;

                case HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_START:
                    {
                        uiHuPaiTipsArrow.Show(playerStateData[0].selectPaiHuPaiInHandPaiIdxs, playerStateData[0].selectPaiHuPaiInMoPaiIdxs);
                        playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK, Time.time, -1);
                    }
                    break;

                case HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK:
                    {
                        if (uiPcghtBtnMgr.IsClicked == true)
                        {
                            RestoreSelectedUpHandPaiToOrgPos();
                            uiHuPaiTips.RemoveAllDetailTips();
                            uiHuPaiTips.Hide();
                            uiHuPaiTipsArrow.Hide();
                            playerStateData[0].rayPickMj = null;
                            playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_START, Time.time, -1);
                        }
                        else if (Input.GetMouseButtonDown(0))
                        {
                            //从摄像机发出到点击坐标的射线
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hitInfo;

                            if (Physics.Raycast(ray, out hitInfo))
                            {
                                //双击出牌
                                if (Time.realtimeSinceStartup - playerStateData[0].rayPickMjLastKickTime < 0.15f &&
                                    playerStateData[0].rayPickMj == hitInfo.collider.gameObject)
                                {
                                    playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END, Time.time, -1);
                                    return;
                                }


                                playerStateData[0].rayPickMj = hitInfo.collider.gameObject;
                                playerStateData[0].rayPickMjMouseOrgPos = Input.mousePosition;
                                playerStateData[0].rayPickMjOrgPos = playerStateData[0].rayPickMj.transform.localPosition;
                                OffMjShadow(playerStateData[0].rayPickMj);

                                playerStateData[0].rayPickMjLastKickTime = Time.realtimeSinceStartup;
                                playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAIING, Time.time, -1);

                            }
                        }
                    }
                    break;


                case HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAIING:
                    {
                        //松开鼠标按键时
                        if (Input.GetMouseButtonUp(0))
                        {
                            playerStateData[0].rayPickMjLastKickTime = Time.realtimeSinceStartup;

                            //麻将选中后未作任何移动
                            if (Mathf.Abs(playerStateData[0].rayPickMj.transform.localPosition.y - playerStateData[0].rayPickMjOrgPos.y) < 0.001f)
                            {
                                if (playerStateData[0].selectedUpMj != playerStateData[0].rayPickMj)
                                {
                                    RestoreSelectedUpHandPaiToOrgPos();

                                    playerStateData[0].rayPickMj.transform.localPosition =
                                        new Vector3(playerStateData[0].rayPickMjOrgPos.x,
                                        playerStateData[0].rayPickMjOrgPos.y + handPaiSelectOffsetHeight,
                                        playerStateData[0].rayPickMjOrgPos.z);

                                    OffMjShadow(playerStateData[0].rayPickMj);
                                    playerStateData[0].selectedUpMj = playerStateData[0].rayPickMj;

                                    if (!IsHuPaiMj(playerStateData[0].rayPickMj))
                                    {
                                        uiHuPaiTips.Hide();
                                    }
                                    else
                                    {
                                        ShowHuPaiTips();
                                    }
                                }
                                else
                                {
                                    RestoreSelectedUpHandPaiToOrgPos(true);
                                }

                                playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK, Time.time, 0);
                            }

                            //麻将移动速度超过25
                            else if (playerStateData[0].rayPickMjMoveDistPreDuration > 50)
                            {
                                playerStateData[0].rayPickMjMoveDistPreDuration = 0;
                                playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END, Time.time, -1);
                            }

                            //麻将移动距离未超过指定高度
                            else if (playerStateData[0].rayPickMj.transform.localPosition.y < playerStateData[0].rayPickMjOrgPos.y + GetCanvasHandMjSizeByAxis(Axis.Y))
                            {
                                playerStateData[0].rayPickMj.transform.DOLocalMove(playerStateData[0].rayPickMjOrgPos, 0.2f);
                                playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_RESTORE, Time.time, 0.2f);
                                playerStateData[0].selectPaiRayPickMj = playerStateData[0].rayPickMj;
                            }

                            //
                            else
                            {
                                playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END, Time.time, -1);
                            }

                            if (isUsePlayerSelectMjOutLine)
                                playerStateData[0].rayPickMj.layer = LayerMask.NameToLayer("Default");
                        }
                        else   //未松开按键
                        {
                            if (playerStateData[0].selectedUpMj != playerStateData[0].rayPickMj)
                            {
                                RestoreSelectedUpHandPaiToOrgPos();
                            }

                            float offsetx = Input.mousePosition.x - playerStateData[0].rayPickMjMouseOrgPos.x;
                            float offsety = Input.mousePosition.y - playerStateData[0].rayPickMjMouseOrgPos.y;

                            if (Mathf.Abs(offsetx) < 0.001f && Mathf.Abs(offsety) < 0.001f)
                            {
                                if (playerStateData[0].selectedUpMj != playerStateData[0].rayPickMj &&
                                    !IsHuPaiMj(playerStateData[0].rayPickMj))
                                {
                                    uiHuPaiTips.Hide();
                                }

                                return;
                            }

                            uiHuPaiTips.Hide();
                            uiHuPaiTipsArrow.Hide();


                            Vector3 newMjPos =
                                new Vector3(playerStateData[0].rayPickMjOrgPos.x + offsetx,
                                playerStateData[0].rayPickMjOrgPos.y + offsety + GetCanvasHandMjSizeByAxis(Axis.Y) / 5,
                                playerStateData[0].rayPickMjOrgPos.z - GetCanvasHandMjSizeByAxis(Axis.Z));

                            playerStateData[0].rayPickMjMoveDistPreDuration =
                                Mathf.Pow(playerStateData[0].rayPickMj.transform.localPosition.x - newMjPos.x, 2) +
                                    Mathf.Pow(playerStateData[0].rayPickMj.transform.localPosition.y - newMjPos.y, 2);

                            playerStateData[0].rayPickMj.transform.localPosition = newMjPos;
                        }
                    }

                    break;

                case HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_RESTORE:
                    {
                        if (playerStateData[0].selectedUpMj != playerStateData[0].selectPaiRayPickMj)
                            OnMjShadow(playerStateData[0].selectPaiRayPickMj, 1);
                        else
                        {
                            ShowHuPaiTips();
                        }

                        uiHuPaiTipsArrow.Show(playerStateData[0].selectPaiHuPaiInHandPaiIdxs, playerStateData[0].selectPaiHuPaiInMoPaiIdxs);

                        playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK, Time.time, -1);
                    }
                    break;


                case HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END:
                    {
                        uiHuPaiTips.RemoveAllDetailTips();
                        uiHuPaiTips.Hide();
                        uiHuPaiTipsArrow.Hide();

                        if (SelfSelectDaPaiEnd != null)
                        {
                            int selectPaiHandPaiIdx = mjSeatHandPaiLists[0].IndexOf(playerStateData[0].rayPickMj);
                            HandPaiType paiType = HandPaiType.MoPai;

                            if (selectPaiHandPaiIdx != -1)
                            {
                                paiType = HandPaiType.HandPai;
                            }
                            else
                            {
                                selectPaiHandPaiIdx = mjSeatMoPaiLists[0].IndexOf(playerStateData[0].rayPickMj);
                            }

                            playerStateData[0].selectPaiHandPaiIdx = selectPaiHandPaiIdx;
                            playerStateData[0].selectPaiType = paiType;
                        }

                        playerStateData[0].rayPickMj = null;
                        playerStateData[0].SetPlayerState(HandActionState.SELECT_PCGTH_PAI_END, Time.time, -1);

                    }
                    break;

                case HandActionState.SELECT_PCGTH_PAI_END:
                    {
                        playerStateData[0].playerHandActionState = HandActionState.ACTION_END;
     

                        if (SelfSelectPCGTHPaiEnd != null)
                        {
                            SelfSelectPCGTHPaiEnd(
                                playerStateData[0].selectPcgthedType,
                                playerStateData[0].selectPcgthedChiPaiMjValueIdx,
                                playerStateData[0].selectPaiHandPaiIdx,
                                playerStateData[0].selectPaiType);
                        }

                        ProcessHandActionMjOpCmdList(0);
                    }
                    break;
            }
        }

        #endregion

        #region 摸牌动作
        /// <summary>
        /// 摸牌动作
        /// </summary>
        /// <param name="seatIdx">摸牌玩家座号</param>
        /// <param name="mjFaceValue">牌面值</param>
        /// <returns></returns>
        void ActionMoPai()
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (isCanUseSeatPlayer[i])
                    ActionMoPai(i);
            }
        }
        void ActionMoPai(int seatIdx)
        {
            if (playerStateData[seatIdx].playerHandActionState < HandActionState.MO_PAI_START ||
            playerStateData[seatIdx].playerHandActionState > HandActionState.MO_PAI_END ||
            Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
            {
                return;
            }

            if (playerStateData[seatIdx].playerHandActionState == HandActionState.MO_PAI_END)
            {
                OnMjShadow(playerStateData[seatIdx].curtMoPaiMj, playerStateData[seatIdx].curtMoPaiMjShadowIdx);
                playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;

                ProcessHandActionMjOpCmdList(seatIdx);
                return;
            }


            MahjongFaceValue mjFaceValue = playerStateData[seatIdx].moPaiFaceValue;
            float waitTime = 0.4f;
            GameObject mj = null;

            switch (seatIdx)
            {
                case 0:
                    {
                        mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValue);
                        mj.layer = defaultLayer;
                        playerStateData[seatIdx].curtMoPaiMj = mj;
                        playerStateData[seatIdx].curtMoPaiMjShadowIdx = 1;
                        mjSeatMoPaiLists[seatIdx].Add(mj);

                        FitSeatCanvasHandMj(mj);
                        OffMjShadow(mj);
                        mj.transform.SetParent(canvasHandPaiTransform, true);
                        Vector3 mjOldEulerAngles = mj.transform.localEulerAngles;

                        float mjsize = GetCanvasHandMjSizeByAxis(Axis.X);
                        Vector3 pos;

                        if (mjSeatMoPaiLists[seatIdx].Count > 1)
                        {
                            GameObject lastMoMj = mjSeatMoPaiLists[seatIdx][mjSeatMoPaiLists[seatIdx].Count - 2];
                            pos = new Vector3(lastMoMj.transform.localPosition.x + mjsize, lastMoMj.transform.localPosition.y, lastMoMj.transform.localPosition.z);
                        }
                        else
                        {
                            int idx = GetHandPaiListLastPaiIdx(seatIdx);

                            if (idx == -1)
                            {
                                pos = mjSeatHandPaiPosLists[seatIdx][0];
                            }
                            else
                            {
                                GameObject lastmj = mjSeatHandPaiLists[seatIdx][idx];
                                pos = lastmj.transform.localPosition;
                                pos.x += mjsize + moPaiToHandPaiCanvasOffset;
                            }
                        }

                        float y = GetCanvasHandMjSizeByAxis(Axis.Y);
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
                        playerStateData[seatIdx].curtMoPaiMj = mj;
                        mjSeatMoPaiLists[seatIdx].Add(mj);

                        FitSeatHandMj(seatIdx, mj);
                        Vector3 mjOldEulerAngles = mj.transform.eulerAngles;
                        float mjsize = GetHandMjSizeByAxis(Axis.X);
                        Vector3 pos;

                        if (mjSeatMoPaiLists[seatIdx].Count > 1)
                        {
                            GameObject lastMoMj = mjSeatMoPaiLists[seatIdx][mjSeatMoPaiLists[seatIdx].Count - 2];
                            pos = new Vector3(lastMoMj.transform.position.x, lastMoMj.transform.position.y, lastMoMj.transform.position.z + mjsize);
                        }
                        else
                        {
                            int idx = GetHandPaiListLastPaiIdx(seatIdx);

                            if (idx == -1)
                            {
                                pos = mjSeatHandPaiPosLists[seatIdx][0];
                            }
                            else
                            {
                                GameObject lastmj = mjSeatHandPaiLists[seatIdx][idx];
                                pos = lastmj.transform.position;
                                pos.z += mjsize + moPaiToHandPaiOffset;
                            }
                        }

                        OffMjShadow(mj);
                        mj.transform.position = new Vector3(pos.x, pos.y + 0.06f, pos.z + 0.005f);
                        mj.transform.eulerAngles = new Vector3(mj.transform.eulerAngles.x, mj.transform.eulerAngles.y, mj.transform.eulerAngles.z + 40f);

                        mj.transform.SetParent(mjtableTransform, true);
                        mj.transform.DOMove(pos, waitTime).SetEase(Ease.InCirc);
                        mj.transform.DORotate(mjOldEulerAngles, waitTime);
                    }

                    break;

                case 2:
                    {
                        mj = mjAssetsMgr.PopMjFromOtherHandMjPool();
                        playerStateData[seatIdx].curtMoPaiMj = mj;
                        mjSeatMoPaiLists[seatIdx].Add(mj);

                        FitSeatHandMj(seatIdx, mj);
                        Vector3 mjOldEulerAngles = mj.transform.eulerAngles;
                        float mjsize = GetHandMjSizeByAxis(Axis.X);
                        Vector3 pos;

                        if (mjSeatMoPaiLists[seatIdx].Count > 1)
                        {
                            GameObject lastMoMj = mjSeatMoPaiLists[seatIdx][mjSeatMoPaiLists[seatIdx].Count - 2];
                            pos = new Vector3(lastMoMj.transform.position.x + mjsize, lastMoMj.transform.position.y, lastMoMj.transform.position.z);
                        }
                        else
                        {
                            int idx = GetHandPaiListLastPaiIdx(seatIdx);

                            if (idx == -1)
                            {
                                pos = mjSeatHandPaiPosLists[seatIdx][0];
                            }
                            else
                            {
                                GameObject lastmj = mjSeatHandPaiLists[seatIdx][idx];
                                pos = lastmj.transform.position;
                                pos.x += mjsize + moPaiToHandPaiOffset;
                            }
                        }


                        OffMjShadow(mj);
                        mj.transform.position = new Vector3(pos.x + 0.005f, pos.y + 0.06f, pos.z);
                        mj.transform.eulerAngles = new Vector3(mj.transform.eulerAngles.x, mj.transform.eulerAngles.y, mj.transform.eulerAngles.z + 40f);

                        mj.transform.SetParent(mjtableTransform, true);
                        mj.transform.DOMove(pos, waitTime).SetEase(Ease.InCirc);
                        mj.transform.DOLocalRotate(mjOldEulerAngles, waitTime);
                    }

                    break;

                case 3:
                    {
                        mj = mjAssetsMgr.PopMjFromOtherHandMjPool();
                        playerStateData[seatIdx].curtMoPaiMj = mj;
                        mjSeatMoPaiLists[seatIdx].Add(mj);

                        FitSeatHandMj(seatIdx, mj);
                        Vector3 mjOldEulerAngles = mj.transform.eulerAngles;
                        float mjsize = GetHandMjSizeByAxis(Axis.X);
                        Vector3 pos;

                        if (mjSeatMoPaiLists[seatIdx].Count > 1)
                        {
                            GameObject lastMoMj = mjSeatMoPaiLists[seatIdx][mjSeatMoPaiLists[seatIdx].Count - 2];
                            pos = new Vector3(lastMoMj.transform.position.x, lastMoMj.transform.position.y, lastMoMj.transform.position.z - mjsize);
                        }
                        else
                        {
                            int idx = GetHandPaiListLastPaiIdx(seatIdx);

                            if (idx == -1)
                            {
                                pos = mjSeatHandPaiPosLists[seatIdx][0];
                            }
                            else
                            {
                                GameObject lastmj = mjSeatHandPaiLists[seatIdx][idx];
                                pos = lastmj.transform.position;
                                pos.z -= mjsize + moPaiToHandPaiOffset;
                            }
                        }

                        mj.transform.position = pos;
                        mj.transform.SetParent(mjtableTransform, true);

                        OffMjShadow(mj);
                        mj.transform.position = new Vector3(pos.x, pos.y + 0.06f, pos.z - 0.005f);
                        mj.transform.eulerAngles = new Vector3(mj.transform.eulerAngles.x, mj.transform.eulerAngles.y, mj.transform.eulerAngles.z + 40f);

                        mj.transform.SetParent(mjtableTransform, true);
                        mj.transform.DOMove(pos, waitTime).SetEase(Ease.InCirc);
                        mj.transform.DOLocalRotate(mjOldEulerAngles, waitTime);
                    }

                    break;
            }

            playerStateData[seatIdx].SetPlayerState(HandActionState.MO_PAI_END, Time.time, waitTime);

        }

        #endregion

        #region 选打牌动作
        /// <summary>
        /// 选牌动作
        /// </summary>
        public void ActionSelectDaPai()
        {
            if (playerStateData[0].playerHandActionState < HandActionState.SELECT_DA_PAI_START ||
                playerStateData[0].playerHandActionState > HandActionState.SELECT_DA_PAI_END ||
                Time.time - playerStateData[0].playerStateStartTime < playerStateData[0].playerStateLiveTime)
            {
                return;
            }

            switch (playerStateData[0].playerHandActionState)
            {
                case HandActionState.SELECT_DA_PAI_START:
                    {
                        uiHuPaiTipsArrow.Show(playerStateData[0].selectPaiHuPaiInHandPaiIdxs, playerStateData[0].selectPaiHuPaiInMoPaiIdxs);
                        playerStateData[0].SetPlayerState(HandActionState.SELECT_DA_PAI_READY_CLICK, Time.time, -1);
                    }
                    break;

                case HandActionState.SELECT_DA_PAI_READY_CLICK:
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            //从摄像机发出到点击坐标的射线
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hitInfo;

                            if (Physics.Raycast(ray, out hitInfo))
                            {

#if (DEBUG)
                                //划出射线，只有在scene视图中才能看到
                                Debug.DrawLine(ray.origin, hitInfo.point);
#endif

                                //双击出牌
                                if (Time.realtimeSinceStartup - playerStateData[0].rayPickMjLastKickTime < 0.15f &&
                                    playerStateData[0].rayPickMj == hitInfo.collider.gameObject)
                                {
                                    playerStateData[0].SetPlayerState(HandActionState.SELECT_DA_PAI_END, Time.time, -1);
                                    return;
                                }


                                playerStateData[0].rayPickMj = hitInfo.collider.gameObject;
                                playerStateData[0].rayPickMjMouseOrgPos = Input.mousePosition;
                                playerStateData[0].rayPickMjOrgPos = playerStateData[0].rayPickMj.transform.localPosition;
                                OffMjShadow(playerStateData[0].rayPickMj);

                                playerStateData[0].rayPickMjLastKickTime = Time.realtimeSinceStartup;
                                playerStateData[0].SetPlayerState(HandActionState.SELECT_DA_PAIING, Time.time, -1);

                            }
                        }
                    }
                    break;


                case HandActionState.SELECT_DA_PAIING:
                    {
                        //松开鼠标按键时
                        if (Input.GetMouseButtonUp(0))
                        {
                            playerStateData[0].rayPickMjLastKickTime = Time.realtimeSinceStartup;

                            //麻将选中后未作任何移动
                            if (Mathf.Abs(playerStateData[0].rayPickMj.transform.localPosition.y - playerStateData[0].rayPickMjOrgPos.y) < 0.001f)
                            {
                                if (playerStateData[0].selectedUpMj != playerStateData[0].rayPickMj)
                                {
                                    RestoreSelectedUpHandPaiToOrgPos();

                                    playerStateData[0].rayPickMj.transform.localPosition =
                                        new Vector3(playerStateData[0].rayPickMjOrgPos.x,
                                        playerStateData[0].rayPickMjOrgPos.y + handPaiSelectOffsetHeight,
                                        playerStateData[0].rayPickMjOrgPos.z);

                                    OffMjShadow(playerStateData[0].rayPickMj);
                                    playerStateData[0].selectedUpMj = playerStateData[0].rayPickMj;

                                    if (!IsHuPaiMj(playerStateData[0].rayPickMj))
                                    {
                                        uiHuPaiTips.Hide();
                                    }
                                    else
                                    {
                                        ShowHuPaiTips();
                                    }
                                }
                                else
                                {
                                    RestoreSelectedUpHandPaiToOrgPos(true);
                                }

                                playerStateData[0].SetPlayerState(HandActionState.SELECT_DA_PAI_READY_CLICK, Time.time, 0);
                            }

                            //麻将移动速度超过25
                            else if (playerStateData[0].rayPickMjMoveDistPreDuration > 50)
                            {
                                playerStateData[0].rayPickMjMoveDistPreDuration = 0;
                                playerStateData[0].SetPlayerState(HandActionState.SELECT_DA_PAI_END, Time.time, -1);
                            }

                            //麻将移动距离未超过指定高度
                            else if (playerStateData[0].rayPickMj.transform.localPosition.y < playerStateData[0].rayPickMjOrgPos.y + GetCanvasHandMjSizeByAxis(Axis.Y))
                            {
                                playerStateData[0].rayPickMj.transform.DOLocalMove(playerStateData[0].rayPickMjOrgPos, 0.2f);
                                playerStateData[0].SetPlayerState(HandActionState.SELECT_DA_PAI_RESTORE, Time.time, 0.2f);
                                playerStateData[0].selectPaiRayPickMj = playerStateData[0].rayPickMj;
                            }

                            //
                            else
                            {
                                playerStateData[0].SetPlayerState(HandActionState.SELECT_DA_PAI_END, Time.time, -1);
                            }

                            if (isUsePlayerSelectMjOutLine)
                                playerStateData[0].rayPickMj.layer = LayerMask.NameToLayer("Default");
                        }
                        else   //未松开按键
                        {
                            if (playerStateData[0].selectedUpMj != playerStateData[0].rayPickMj)
                            {
                                RestoreSelectedUpHandPaiToOrgPos();
                            }

                            float offsetx = Input.mousePosition.x - playerStateData[0].rayPickMjMouseOrgPos.x;
                            float offsety = Input.mousePosition.y - playerStateData[0].rayPickMjMouseOrgPos.y;

                            if (Mathf.Abs(offsetx) < 0.001f && Mathf.Abs(offsety) < 0.001f)
                            {
                                if (playerStateData[0].selectedUpMj != playerStateData[0].rayPickMj &&
                                    !IsHuPaiMj(playerStateData[0].rayPickMj))
                                {
                                    uiHuPaiTips.Hide();
                                }

                                return;
                            }

                            uiHuPaiTips.Hide();
                            uiHuPaiTipsArrow.Hide();


                            Vector3 newMjPos =
                                new Vector3(playerStateData[0].rayPickMjOrgPos.x + offsetx,
                                playerStateData[0].rayPickMjOrgPos.y + offsety + GetCanvasHandMjSizeByAxis(Axis.Y) / 5,
                                playerStateData[0].rayPickMjOrgPos.z - GetCanvasHandMjSizeByAxis(Axis.Z));

                            playerStateData[0].rayPickMjMoveDistPreDuration =
                                Mathf.Pow(playerStateData[0].rayPickMj.transform.localPosition.x - newMjPos.x, 2) +
                                    Mathf.Pow(playerStateData[0].rayPickMj.transform.localPosition.y - newMjPos.y, 2);

                            playerStateData[0].rayPickMj.transform.localPosition = newMjPos;
                        }
                    }

                    break;

                case HandActionState.SELECT_DA_PAI_RESTORE:
                    {
                        if (playerStateData[0].selectedUpMj != playerStateData[0].selectPaiRayPickMj)
                            OnMjShadow(playerStateData[0].selectPaiRayPickMj, 1);
                        else
                        {
                            ShowHuPaiTips();
                        }

                        uiHuPaiTipsArrow.Show(playerStateData[0].selectPaiHuPaiInHandPaiIdxs, playerStateData[0].selectPaiHuPaiInMoPaiIdxs);

                        playerStateData[0].SetPlayerState(HandActionState.SELECT_DA_PAI_READY_CLICK, Time.time, -1);
                    }
                    break;


                case HandActionState.SELECT_DA_PAI_END:
                    {
                        playerStateData[0].selectPaiHuPaiInHandPaiIdxs = null;
                        playerStateData[0].selectPaiHuPaiInfosInHandPai = null;
                        playerStateData[0].selectPaiHuPaiInMoPaiIdxs = null;
                        playerStateData[0].selectPaiHuPaiInfosInMoPai = null;

                        uiHuPaiTips.RemoveAllDetailTips();
                        uiHuPaiTips.Hide();
                        uiHuPaiTipsArrow.Hide();

                        playerStateData[0].playerHandActionState = HandActionState.ACTION_END;

                        if (SelfSelectDaPaiEnd != null)
                        {
                            int selectPaiHandPaiIdx = mjSeatHandPaiLists[0].IndexOf(playerStateData[0].rayPickMj);
                            HandPaiType paiType = HandPaiType.MoPai;

                            if (selectPaiHandPaiIdx != -1)
                            {
                                paiType = HandPaiType.HandPai;
                            }
                            else
                            {
                                selectPaiHandPaiIdx = mjSeatMoPaiLists[0].IndexOf(playerStateData[0].rayPickMj);
                            }

                            SelfSelectDaPaiEnd(selectPaiHandPaiIdx, paiType);
                        }

                        playerStateData[0].rayPickMj = null;
                        playerStateData[0].selectedUpMj = null;

                        ProcessHandActionMjOpCmdList(0);
                    }
                    break;

            }

        }

        bool IsHuPaiMj(GameObject mj)
        {
            int idx = GetPaiIdxInHandPaiList(0, mj);
            if (idx != -1)
            {
                idx = Common.IndexOf(playerStateData[0].selectPaiHuPaiInHandPaiIdxs, idx);
            }
            else
            {
                idx = GetPaiIdxInMoPaiList(0, mj);
                if (idx != -1)
                    idx = Common.IndexOf(playerStateData[0].selectPaiHuPaiInMoPaiIdxs, idx);
            }

            return idx == -1 ? false : true;
        }

        void ShowHuPaiTips()
        {
            int idx = GetPaiIdxInHandPaiList(0, playerStateData[0].rayPickMj);

            if (idx != -1 && playerStateData[0].selectPaiHuPaiInfosInHandPai != null)
            {
                idx = Common.IndexOf(playerStateData[0].selectPaiHuPaiInHandPaiIdxs, idx);
                if (idx != -1)
                {
                    uiHuPaiTips.Show(playerStateData[0].selectPaiHuPaiInfosInHandPai[idx]);
                }
            }
            else
            {
                idx = GetPaiIdxInMoPaiList(0, playerStateData[0].rayPickMj);
                if (idx != -1 && playerStateData[0].selectPaiHuPaiInfosInMoPai != null)
                {
                    idx = Common.IndexOf(playerStateData[0].selectPaiHuPaiInMoPaiIdxs, idx);
                    if (idx != -1)
                    {
                        uiHuPaiTips.Show(playerStateData[0].selectPaiHuPaiInfosInMoPai[idx]);
                    }
                }
            }
        }

        void RestoreSelectedUpHandPaiToOrgPos(bool isHideHuPaiTips = false)
        {
            if (playerStateData[0].selectedUpMj == null)
                return;

            Vector3 pos = playerStateData[0].selectedUpMj.transform.localPosition;
            pos.y -= handPaiSelectOffsetHeight;
            playerStateData[0].selectedUpMj.transform.localPosition = pos;
            OnMjShadow(playerStateData[0].selectedUpMj, 1);
            playerStateData[0].selectedUpMj = null;

            if (isHideHuPaiTips)
                uiHuPaiTips.Hide();
        }

        #endregion

        #region 选牌动作
        /// <summary>
        /// 选牌动作
        /// </summary>
        public void ActionSelectPai()
        {
            if (playerStateData[0].playerHandActionState < HandActionState.SELECT_PAI_START ||
               playerStateData[0].playerHandActionState > HandActionState.SELECT_PAI_END ||
               Time.time - playerStateData[0].playerStateStartTime < playerStateData[0].playerStateLiveTime)
            {
                return;
            }

            switch (playerStateData[0].playerHandActionState)
            {
                case HandActionState.SELECT_PAI_START:
                    {
                        uiHuPaiTipsArrow.Show(playerStateData[0].selectPaiHuPaiInHandPaiIdxs, playerStateData[0].selectPaiHuPaiInMoPaiIdxs);
                        playerStateData[0].SetPlayerState(HandActionState.SELECT_PAI_READY_CLICK, Time.time, -1);
                    }
                    break;

                case HandActionState.SELECT_PAI_READY_CLICK:
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            //从摄像机发出到点击坐标的射线
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hitInfo;

                            if (Physics.Raycast(ray, out hitInfo))
                            {
                                playerStateData[0].rayPickMj = hitInfo.collider.gameObject;
                                playerStateData[0].rayPickMjMouseOrgPos = Input.mousePosition;
                                playerStateData[0].rayPickMjOrgPos = playerStateData[0].rayPickMj.transform.localPosition;
                                OffMjShadow(playerStateData[0].rayPickMj);

                                playerStateData[0].rayPickMjLastKickTime = Time.realtimeSinceStartup;
                                playerStateData[0].SetPlayerState(HandActionState.SELECT_PAIING, Time.time, -1);

                            }
                        }
                    }
                    break;


                case HandActionState.SELECT_PAIING:
                    {
                        //松开鼠标按键时
                        if (Input.GetMouseButtonUp(0))
                        {
                            playerStateData[0].rayPickMjLastKickTime = Time.realtimeSinceStartup;

                            if (playerStateData[0].selectedUpMj != playerStateData[0].rayPickMj)
                            {
                                RestoreSelectedUpHandPaiToOrgPos();

                                playerStateData[0].rayPickMj.transform.localPosition =
                                    new Vector3(playerStateData[0].rayPickMjOrgPos.x,
                                    playerStateData[0].rayPickMjOrgPos.y + handPaiSelectOffsetHeight,
                                    playerStateData[0].rayPickMjOrgPos.z);

                                OffMjShadow(playerStateData[0].rayPickMj);
                                playerStateData[0].selectedUpMj = playerStateData[0].rayPickMj;

                                if (!IsHuPaiMj(playerStateData[0].rayPickMj))
                                {
                                    uiHuPaiTips.Hide();
                                }
                                else
                                {
                                    ShowHuPaiTips();
                                }
                            }
                            else
                            {
                                RestoreSelectedUpHandPaiToOrgPos(true);
                            }

                            playerStateData[0].SetPlayerState(HandActionState.SELECT_PAI_READY_CLICK, Time.time, 0);

                        }
                        else   //未松开按键
                        {
                            if (playerStateData[0].selectedUpMj != playerStateData[0].rayPickMj)
                            {
                                RestoreSelectedUpHandPaiToOrgPos();
                            }

                            if (playerStateData[0].selectedUpMj != playerStateData[0].rayPickMj &&
                                     !IsHuPaiMj(playerStateData[0].rayPickMj))
                            {
                                uiHuPaiTips.Hide();
                            }
                        }
                    }

                    break;


                case HandActionState.SELECT_PAI_END:
                    {
                        uiHuPaiTips.RemoveAllDetailTips();
                        uiHuPaiTips.Hide();
                        uiHuPaiTipsArrow.Hide();

                        playerStateData[0].playerHandActionState = HandActionState.ACTION_END;

                        playerStateData[0].rayPickMj = null;

                        ProcessHandActionMjOpCmdList(0);
                    }
                    break;

            }

        }

        #endregion

        #region 打牌动作

        /// <summary>
        /// 打牌动作
        /// </summary>
        void ActionDaPai()
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (isCanUseSeatPlayer[i])
                    ActionDaPai(i);
            }
        }
        void ActionDaPai(int seatIdx)
        {
            if (playerStateData[seatIdx].playerHandActionState < HandActionState.DA_PAI_START ||
                playerStateData[seatIdx].playerHandActionState > HandActionState.DA_PAI_END)
            {
                return;
            }

            if (Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
            {
                MoveDaPaiHandShadow(seatIdx);
                return;
            }


            PlayerType handStyle = playerStateData[seatIdx].handStyle;
            Vector3Int mjPosIdx = playerStateData[seatIdx].mjPosIdx;
            MahjongFaceValue mjFaceValue = playerStateData[seatIdx].daPaiFaceValue;
            ActionCombineNum actionCombineNum = playerStateData[seatIdx].actionCombineNum;

            float waitTime = 0.3f;
            Animation anim = GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);
            Vector3 mjpos = GetMjDeskPaiPos(seatIdx, mjPosIdx.x, mjPosIdx.y);
            float mjHeight = GetDeskMjSizeByAxis(Axis.Z);
            mjpos.y += mjHeight * mjPosIdx.z;

            float fadeTime = 0.06f;

            switch (playerStateData[seatIdx].playerHandActionState)
            {
                case HandActionState.DA_PAI_START:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);

                        //是否叫听
                        if (playerStateData[seatIdx].isJiaoTing)
                        {
                            PlaySpeakAudio(handStyle, AudioIdx.AUDIO_SPEAK_TING, cameraTransform.position);
                            GameObject tingTextParticle = mjAssetsMgr.tingPaiTextParticlePool.PopGameObject();
                            tingTextParticle.transform.localPosition = pcgthEffectTextPosSeat[seatIdx];
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

                        playerStateData[seatIdx].handShadowAxis[0] = GetHandShadowAxis(seatIdx, handStyle, HandDirection.RightHand, 0).transform;
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(true);
                        MoveDaPaiHandShadow(seatIdx);


                        if (onVoice)
                        {
                            Debug.Log("打牌:" + mjFaceValue);
                            PlaySpeakAudio(handStyle, mjFaceValue, mjpos);
                        }

                        playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_READY_FIRST_HAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.DA_PAI_READY_FIRST_HAND:
                    {
                        playerStateData[seatIdx].curtHandReadyPutDeskPai = ReadyFirstHandMj(seatIdx, handStyle, HandDirection.RightHand, mjFaceValue, actionCombineNum);
                        waitTime = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, mjpos, actionCombineNum);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_MOVE_HAND_TO_DST_POS, Time.time, waitTime);

                    }
                    break;

                case HandActionState.DA_PAI_MOVE_HAND_TO_DST_POS:
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
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1");
                                }
                                break;

                            case ActionCombineNum.DaPai2_MovPai_TaiHand1:
                            case ActionCombineNum.DaPai2_MovPai_TaiHand2:
                                {
                                    anim.CrossFade("DaPai1", fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1");
                                }
                                break;

                            case ActionCombineNum.DaPai3_TaiHand:
                                {
                                    anim.Play("DaPai3");
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai3");
                                }
                                break;


                            case ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand:
                                {
                                    anim.CrossFade("FirstTaiHand2EndDaPai4", fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand2EndDaPai4");
                                }
                                break;


                            case ActionCombineNum.DaPai5:
                                {
                                    anim.CrossFade("FirstTaiHand3EndDaPai5", fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand3EndDaPai5");
                                }
                                break;
                        }


                        playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI, Time.time, waitTime);
                    }
                    break;

                case HandActionState.DA_PAI_CHUPAI:
                    {
                        AudioClip clip = GetEffectAudio((int)AudioIdx.AUDIO_EFFECT_GIVE);
                        AudioSource.PlayClipAtPoint(clip, mjpos);

                        GameObject dropPai = playerStateData[seatIdx].curtHandReadyPutDeskPai;

                        if (dropPai == null)
                        {
                            playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_END, Time.time, -1);
                            return;
                        }

                        OnMjShadow(dropPai, 0);

                        switch (actionCombineNum)
                        {
                            case ActionCombineNum.DaPai1_TaiHand1:
                            case ActionCombineNum.DaPai1_TaiHand2:
                                {
                                    dropPai.transform.SetParent(mjtableTransform, true);
                                    AdjustDeskMjPos(seatIdx, mjpos, 0.04f);

                                    //桌子上最后打出的麻将
                                    lastDaPaiMj = dropPai;

                                    anim.CrossFade(taiHandActionName[(int)actionCombineNum], fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, taiHandActionName[(int)actionCombineNum]);
                                    MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, handActionLevelScreenPosSeat[seatIdx]);
                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                                }
                                break;


                            case ActionCombineNum.DaPai1_MovPai1_TaiHand1:
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand2:
                            case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:
                                {
                                    dropPai.transform.SetParent(mjtableTransform, true);

                                    anim.CrossFade("DaPai1EndTiaoZhengHand", fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndTiaoZhengHand");
                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_TIAOZHENG_HAND, Time.time, waitTime);
                                }
                                break;

                            case ActionCombineNum.DaPai1_ZhengPai_TaiHand:
                                {
                                    dropPai.transform.SetParent(mjtableTransform, true);

                                    anim.CrossFade("DaPai1EndZhengPai", fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndZhengPai");
                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_ZHENGPAI, Time.time, waitTime);
                                }
                                break;


                            case ActionCombineNum.DaPai2_MovPai_TaiHand1:
                            case ActionCombineNum.DaPai2_MovPai_TaiHand2:
                                {
                                    anim.CrossFade("DaPai2EndMovPai", fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai2EndMovPai");
                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI2_MOVPAI, Time.time, waitTime);
                                }
                                break;

                            case ActionCombineNum.DaPai3_TaiHand:
                            case ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand:
                                {
                                    dropPai.transform.SetParent(mjtableTransform, true);
                                    AdjustDeskMjPos(seatIdx, mjpos, 0.02f);

                                    //桌子上最后打出的麻将
                                    lastDaPaiMj = dropPai;

                                    anim.CrossFade(taiHandActionName[(int)actionCombineNum], fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, taiHandActionName[(int)actionCombineNum]);

                                    MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, handActionLevelScreenPosSeat[seatIdx]);
                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                                }
                                break;

                            case ActionCombineNum.DaPai5:
                                {
                                    dropPai.transform.SetParent(mjtableTransform, true);
                                    AdjustDeskMjPos(seatIdx, mjpos, 0.02f);

                                    //桌子上最后打出的麻将
                                    lastDaPaiMj = dropPai;

                                    anim.CrossFade("FirstTaiHand3EndDaPai5EndTaiHand", fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand3EndDaPai5EndTaiHand");

                                    MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, handActionLevelScreenPosSeat[seatIdx]);

                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                                }
                                break;


                            default:
                                {
                                    Debug.Log("不存在此打牌动作编号:" + actionCombineNum + "!");
                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_END, Time.time, -1);
                                }
                                break;
                        }
                    }
                    break;

                case HandActionState.DA_PAI_CHUPAI_TIAOZHENG_HAND:
                    {

                        switch (actionCombineNum)
                        {
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand1:
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand2:
                                {
                                    anim.Play("DaPai1EndTiaoZhengHandEndMovPai1");
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndTiaoZhengHandEndMovPai1");
                                    AdjustDeskMjPos(seatIdx, mjpos, waitTime);
                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_TIAOZHENG_HAND_MOVPAI1, Time.time, waitTime);
                                }
                                break;

                            case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:
                                {
                                    anim.Play("DaPai1EndTiaoZhengHandEndMovPai1");
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndTiaoZhengHandEndMovPai1");
                                    AdjustDeskMjPos(seatIdx, mjpos, waitTime, true, 0.67f, false);
                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_TIAOZHENG_HAND_MOVPAI1, Time.time, waitTime);
                                }
                                break;
                        }
                    }
                    break;

                case HandActionState.DA_PAI_CHUPAI_TIAOZHENG_HAND_MOVPAI1:
                    {
                        switch (actionCombineNum)
                        {
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand1:
                            case ActionCombineNum.DaPai1_MovPai1_TaiHand2:
                                {
                                    //桌子上最后打出的麻将
                                    lastDaPaiMj = playerStateData[seatIdx].curtHandReadyPutDeskPai;

                                    anim.CrossFade(taiHandActionName[(int)actionCombineNum], fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, taiHandActionName[(int)actionCombineNum]);

                                    MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, handActionLevelScreenPosSeat[seatIdx]);
                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);


                                }
                                break;

                            case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:
                                {
                                    anim.CrossFade("DaPai1EndMovPai1EndZhengPai", fadeTime);
                                    waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndMovPai1EndZhengPai");
                                    playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_ZHENGPAI, Time.time, waitTime / 2);
                                }
                                break;
                        }
                    }
                    break;


                case HandActionState.DA_PAI_CHUPAI_ZHENGPAI:
                    {
                        AdjustDeskMjPos(seatIdx, mjpos, playerStateData[seatIdx].playerStateLiveTime);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_ZHENGPAI_ADJUSTPAI, Time.time, playerStateData[seatIdx].playerStateLiveTime);
                    }
                    break;


                case HandActionState.DA_PAI_CHUPAI_ZHENGPAI_ADJUSTPAI:
                    {
                        MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, handActionLevelScreenPosSeat[seatIdx]);

                        switch (actionCombineNum)
                        {
                            case ActionCombineNum.DaPai1_ZhengPai_TaiHand:

                                //桌子上最后打出的麻将
                                lastDaPaiMj = playerStateData[seatIdx].curtHandReadyPutDeskPai;

                                anim.CrossFade("DaPai1EndZhengPaiEndTaiHand", fadeTime);
                                waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndZhengPaiEndTaiHand");
                                playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                                break;

                            case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:

                                //桌子上最后打出的麻将
                                lastDaPaiMj = playerStateData[seatIdx].curtHandReadyPutDeskPai;

                                anim.CrossFade("DaPai1EndMovPai1EndZhengPaiEndTaiHand", fadeTime);
                                waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "DaPai1EndMovPai1EndZhengPaiEndTaiHand");
                                playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                                break;
                        }
                    }
                    break;


                case HandActionState.DA_PAI_CHUPAI2_MOVPAI:
                case HandActionState.DA_PAI_CHUPAI_MOVPAI2:
                    {
                        playerStateData[seatIdx].curtHandReadyPutDeskPai.transform.SetParent(mjtableTransform, true);
                        AdjustDeskMjPos(seatIdx, mjpos, 0.06f);

                        //桌子上最后打出的麻将
                        lastDaPaiMj = playerStateData[seatIdx].curtHandReadyPutDeskPai;

                        anim.CrossFade(taiHandActionName[(int)actionCombineNum], fadeTime);
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, taiHandActionName[(int)actionCombineNum]);

                        MoveHandToDstDirRelative(seatIdx, handStyle, HandDirection.RightHand, handActionLevelScreenPosSeat[seatIdx]);

                        playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_CHUPAI_TAIHAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.DA_PAI_CHUPAI_TAIHAND:
                    {
                        int key = GetDeskDaPaiMjDictKey(mjPosIdx.x, mjPosIdx.y, mjPosIdx.z);
                        deskDaPaiMjDicts[seatIdx][key] = playerStateData[seatIdx].curtHandReadyPutDeskPai;
                        AppendMjToDeskGlobalMjPaiSetDict(mjFaceValue, playerStateData[seatIdx].curtHandReadyPutDeskPai);

                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.RightHand, actionCombineNum);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.DA_PAI_END, Time.time, waitTime);
                    }
                    break;

                case HandActionState.DA_PAI_END:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(false);
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(false);
                        playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;

                        ProcessHandActionMjOpCmdList(seatIdx);
                    }

                    break;
            }
        }
        void MoveDaPaiHandShadow(int seatIdx)
        {
            Transform handShadowRef = playerStateData[seatIdx].handShadowAxis[0];
            float ang = 360 - handShadowRef.eulerAngles.y;

            float matUOffset = ((handShadowRef.position.x - handShadowPlaneInfos[seatIdx].shadowOrgPos[0].x) / planeSize.x) * handShadowPlaneInfos[seatIdx].tiling[0].x + handShadowPlaneInfos[seatIdx].offset[0].x;
            float matVOffset = ((handShadowRef.position.z - handShadowPlaneInfos[seatIdx].shadowOrgPos[0].y) / planeSize.z) * handShadowPlaneInfos[seatIdx].tiling[0].y + handShadowPlaneInfos[seatIdx].offset[0].y;

            handShadowPlaneInfos[seatIdx].planeMat.SetTextureOffset(shaderTexNames[0], new Vector2(matUOffset, matVOffset));
            handShadowPlaneInfos[seatIdx].planeMat.SetFloat(shaderAngNames[0], ang);

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

            mj.layer = defaultLayer;
            FitSeatDeskMj(seatIdx, mj, false, 0.91f, false, false);


            switch (actionCombineNum)
            {
                case ActionCombineNum.DaPai1_ZhengPai_TaiHand:
                    bone = GetHandBone(seatIdx, handStyle, handDir, 1);
                    mj.transform.localPosition = mjDaPaiFirstHandPos[seatIdx];
                    a = mjDaPaiFirstHandEulerAngles[seatIdx];
                    mj.transform.localEulerAngles = new Vector3(a.x, a.y, a.z + sign * 10f);
                    break;

                case ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand:
                    bone = GetHandBone(seatIdx, handStyle, handDir, 1);
                    mj.transform.localPosition = mjDaPaiFirstHandPos[seatIdx];
                    a = mjDaPaiFirstHandEulerAngles[seatIdx];
                    mj.transform.localEulerAngles = new Vector3(a.x, a.y, a.z - sign * 10f);
                    break;

                case ActionCombineNum.DaPai5:
                    bone = GetHandBone(seatIdx, handStyle, handDir, 2);
                    mj.transform.localPosition = mjDaPaiFirstHandPos2[seatIdx];
                    mj.transform.localEulerAngles = mjDaPaiFirstHandEulerAngles2[seatIdx];
                    break;

                default:
                    bone = GetHandBone(seatIdx, handStyle, handDir, 1);
                    mj.transform.localPosition = mjDaPaiFirstHandPos[seatIdx];
                    mj.transform.localEulerAngles = mjDaPaiFirstHandEulerAngles[seatIdx];
                    break;
            }

            mj.transform.SetParent(bone.transform, false);

            FitHandPoseForSeat(seatIdx, handStyle, handDir, actionCombineNum);
            readyPutMj = mj;

            return readyPutMj;
        }

        void AdjustDeskMjPos(int seatIdx, Vector3 mjpos, float adjustTime, bool isMove = true, float moveProgress = 1.0f, bool isRotate = true)
        {
            if (isMove)
            {
                GameObject pai = playerStateData[seatIdx].curtHandReadyPutDeskPai;
                Vector3 offsetPos = mjpos - pai.transform.position;
                offsetPos.x *= moveProgress;
                offsetPos.z *= moveProgress;
                playerStateData[seatIdx].curtHandReadyPutDeskPai.transform.DOMove(offsetPos, adjustTime).SetRelative();
            }

            if (isRotate)
                playerStateData[seatIdx].curtHandReadyPutDeskPai.transform.DORotate(GetSeatDeskMjFitEulerAngles(seatIdx), adjustTime);
        }

        #endregion

        #region 补花动作

        void ActionBuHua()
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (isCanUseSeatPlayer[i])
                    ActionBuHua(i);
            }
        }
        void ActionBuHua(int seatIdx)
        {
            if (playerStateData[seatIdx].playerHandActionState < HandActionState.BUHUA_PAI_START ||
               playerStateData[seatIdx].playerHandActionState > HandActionState.BUHUA_PAI_END ||
               Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
            {
                return;
            }

            PlayerType handStyle = playerStateData[seatIdx].handStyle;
            float waitTime = 0.3f;
            Animation anim = GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);


            switch (playerStateData[seatIdx].playerHandActionState)
            {
                case HandActionState.BUHUA_PAI_START:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);
                        waitTime = ReadyFirstHand(seatIdx, handStyle, HandDirection.RightHand, "HuPaiFirstHand");

                        PlaySpeakAudio(handStyle, AudioIdx.AUDIO_SPEAK_BUHUA, cameraTransform.position);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.BUHUA_PAI_READY_FIRST_HAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.BUHUA_PAI_READY_FIRST_HAND:
                    {
                        Vector3 mjpos = GetDeskHuPaiMjPos(seatIdx, playerStateData[seatIdx].buHuaPaiMjPosIdx);
                        mjpos.y += GetDeskMjSizeByAxis(Axis.Z) / 2;
                        FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, playerStateData[seatIdx].actionCombineNum);
                        waitTime = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, mjpos, playerStateData[seatIdx].actionCombineNum);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.BUHUA_PAI_MOVE_HAND_TO_DST_POS, Time.time, waitTime);
                    }
                    break;

                case HandActionState.BUHUA_PAI_MOVE_HAND_TO_DST_POS:
                    {   
                        anim.Play("FirstTaiHand1EndHuPai");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand1EndHuPai");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.BUHUA_PAI_BU, Time.time, waitTime);
                    }
                    break;

                case HandActionState.BUHUA_PAI_BU:
                    {
                        GameObject mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(playerStateData[seatIdx].buHuaPaiFaceValue);
                        mj.layer = defaultLayer;
                        FitSeatDeskMj(seatIdx, mj);
                        mj.transform.position = GetDeskHuPaiMjPos(seatIdx, playerStateData[seatIdx].buHuaPaiMjPosIdx);
                        deskHuPaiMjDicts[seatIdx][playerStateData[seatIdx].buHuaPaiMjPosIdx] = mj;
                        AppendMjToDeskGlobalMjPaiSetDict(playerStateData[seatIdx].buHuaPaiFaceValue, mj);

                        if (playerStateData[seatIdx].buHuaPaiMjPosIdx >= huPaiDeskPosMjLayoutRowCount * huPaiDeskPosMjLayoutColCount)
                            OffMjShadow(mj);

                        mj.transform.SetParent(mjtableTransform, true);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.BUHUA_PAI_GET_PAI, Time.time, 0.2f);

                    }
                    break;


                case HandActionState.BUHUA_PAI_GET_PAI:
                    {
                        anim.Play("FirstTaiHand1EndHuPaiEndTaiHand");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand1EndHuPaiEndTaiHand");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.BUHUA_PAI_END, Time.time, waitTime);
                    }
                    break;

                case HandActionState.BUHUA_PAI_END:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(false);
                        playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;

                        ProcessHandActionMjOpCmdList(seatIdx);
                    }

                    break;
            }
        }

        #endregion

        #region 胡牌动作
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
        void ActionHuPai()
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (isCanUseSeatPlayer[i])
                    ActionHuPai(i);
            }
        }
        void ActionHuPai(int seatIdx)
        {
            if (playerStateData[seatIdx].playerHandActionState < HandActionState.HU_PAI_START ||
               playerStateData[seatIdx].playerHandActionState > HandActionState.HU_PAI_END ||
               Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
            {
                return;
            }

            PlayerType handStyle = playerStateData[seatIdx].handStyle;
            int targetSeatIdx = playerStateData[seatIdx].huPaiTargetSeatIdx;

            float waitTime = 0.3f;
            Animation anim = GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);


            switch (playerStateData[seatIdx].playerHandActionState)
            {
                case HandActionState.HU_PAI_START:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);
                        waitTime = ReadyFirstHand(seatIdx, handStyle, HandDirection.RightHand, "HuPaiFirstHand");

                        //玩家胡牌方式是自摸
                        if (targetSeatIdx == -1)
                        {
                            PlayEffectAudio(AudioIdx.AUDIO_EFFECT_ZIMO);
                            PlaySpeakAudio(handStyle, AudioIdx.AUDIO_SPEAK_ZIMO, cameraTransform.position);
                            GameObject zimoTextParticle = mjAssetsMgr.zimoPaiTextParticlePool.PopGameObject();
                            zimoTextParticle.transform.localPosition = pcgthEffectTextPosSeat[seatIdx];
                            zimoTextParticle.SetActive(true);
                            zimoTextParticle.GetComponent<ParticleSystem>().Play();
                        }
                        else
                        {
                            PlaySpeakAudio(handStyle, AudioIdx.AUDIO_SPEAK_HU, cameraTransform.position);
                            GameObject huPaiTextParticle = mjAssetsMgr.huPaiTextParticlePool.PopGameObject();
                            huPaiTextParticle.transform.localPosition = pcgthEffectTextPosSeat[seatIdx];
                            huPaiTextParticle.SetActive(true);
                            huPaiTextParticle.GetComponent<ParticleSystem>().Play();
                        }

                        //
                        playerStateData[seatIdx].SetPlayerState(HandActionState.HU_PAI_READY_FIRST_HAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.HU_PAI_READY_FIRST_HAND:
                    {
                        Vector3 mjpos = GetDeskHuPaiMjPos(seatIdx, playerStateData[seatIdx].huPaiMjPosIdx);
                        AudioClip clip = GetEffectAudio((int)AudioIdx.AUDIO_EFFECT_SHANDIAN);
                        AudioSource.PlayClipAtPoint(clip, cameraTransform.position);

                        mjpos.y += GetDeskMjSizeByAxis(Axis.Z) / 2;
                        FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, playerStateData[seatIdx].actionCombineNum);
                        waitTime = MoveHandToDstOffsetPos(seatIdx, handStyle, HandDirection.RightHand, mjpos, playerStateData[seatIdx].actionCombineNum);


                        //玩家胡牌方式不是自摸
                        if (targetSeatIdx >= 0 && targetSeatIdx < playerCount)
                        {
                            Vector3Int targetMjIdx = playerStateData[seatIdx].huPaiTargetMjIdx;

                            if (targetMjIdx.x == -1 || targetMjIdx.y == -1 || targetMjIdx.z == -1)
                            {
                                targetMjIdx = GetCurtDeskMjPosIdx(targetSeatIdx);
                            }

                            int key = GetDeskDaPaiMjDictKey(targetMjIdx.x, targetMjIdx.y, targetMjIdx.z);
                            if (deskDaPaiMjDicts[targetSeatIdx].ContainsKey(key))
                            {
                                playerStateData[seatIdx].huPaiTargetMjKey = key;
                                GameObject targetmj = deskDaPaiMjDicts[targetSeatIdx][key];

                                GameObject huPaiParticle = mjAssetsMgr.huPaiShanDianParticlePool.PopGameObject();
                                huPaiParticle.SetActive(true);
                                huPaiParticle.transform.position = targetmj.transform.position;
                                huPaiParticle.GetComponent<ParticleSystem>().Play();

                                if (targetmj == lastDaPaiMj)
                                    lastDaPaiMj = null;
                            }
                            else
                            {
                                playerStateData[seatIdx].huPaiTargetMjKey = -1;
                            }
                        }

                        playerStateData[seatIdx].SetPlayerState(HandActionState.HU_PAI_MOVE_HAND_TO_DST_POS, Time.time, waitTime);
                    }
                    break;

                case HandActionState.HU_PAI_MOVE_HAND_TO_DST_POS:
                    {
                        int key = playerStateData[seatIdx].huPaiTargetMjKey;

                        if (key != -1)
                        {
                            GameObject targetmj = deskDaPaiMjDicts[targetSeatIdx][key];
                            deskDaPaiMjDicts[targetSeatIdx].Remove(key);
                            RemoveMjFromDeskGlobalMjPaiSetDict(playerStateData[seatIdx].huPaiFaceValue, targetmj);
                            mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(targetmj);

                            PrevDeskMjPos(targetSeatIdx);
                        }

                        anim.Play("FirstTaiHand1EndHuPai");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand1EndHuPai");

                        playerStateData[seatIdx].SetPlayerState(HandActionState.HU_PAI_HU, Time.time, waitTime);
                    }
                    break;

                case HandActionState.HU_PAI_HU:
                    {
                        GameObject mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(playerStateData[seatIdx].huPaiFaceValue);
                        mj.layer = defaultLayer;
                        FitSeatDeskMj(seatIdx, mj);
                        mj.transform.position = GetDeskHuPaiMjPos(seatIdx, playerStateData[seatIdx].huPaiMjPosIdx);
                        deskHuPaiMjDicts[seatIdx][playerStateData[seatIdx].huPaiMjPosIdx] = mj;
                        AppendMjToDeskGlobalMjPaiSetDict(playerStateData[seatIdx].huPaiFaceValue, mj);

                        if (playerStateData[seatIdx].huPaiMjPosIdx >= huPaiDeskPosMjLayoutRowCount * huPaiDeskPosMjLayoutColCount)
                            OffMjShadow(mj);

                        mj.transform.SetParent(mjtableTransform, true);


                        Vector3 handDstPos = mj.transform.position;
                        handDstPos.y += GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.001f;
                        GameObject huPaiParticle = mjAssetsMgr.huPaiGetMjParticlePool.PopGameObject();
                        huPaiParticle.SetActive(true);
                        huPaiParticle.transform.position = handDstPos;
                        huPaiParticle.GetComponent<ParticleSystem>().Play();

                        playerStateData[seatIdx].SetPlayerState(HandActionState.HU_PAI_GET_PAI, Time.time, 0.2f);

                    }
                    break;


                case HandActionState.HU_PAI_GET_PAI:
                    {
                        anim.Play("FirstTaiHand1EndHuPaiEndTaiHand");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "FirstTaiHand1EndHuPaiEndTaiHand");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.HU_PAI_END, Time.time, waitTime);
                    }
                    break;

                case HandActionState.HU_PAI_END:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(false);
                        playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;

                        ProcessHandActionMjOpCmdList(seatIdx);
                    }

                    break;
            }
        }

        #endregion

        #region  碰,吃,杠牌动作
        /// <summary>
        /// 碰,吃,杠牌动作
        /// </summary>
        void ActionPengChiGangPai()
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (isCanUseSeatPlayer[i])
                    ActionPengChiGangPai(i);
            }
        }
        void ActionPengChiGangPai(int seatIdx)
        {
            if (playerStateData[seatIdx].playerHandActionState < HandActionState.PENG_CHI_GANG_PAI_START ||
               playerStateData[seatIdx].playerHandActionState > HandActionState.PENG_CHI_GANG_PAI_END ||
               Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
            {
                return;
            }

            PlayerType handStyle = playerStateData[seatIdx].handStyle;
            bool isMoveHand = playerStateData[seatIdx].pcgPaiIsMoveHand;
            float moveHandDist = playerStateData[seatIdx].pcgPaiMoveHandDist;
            PengChiGangPaiType pcgPaiType = playerStateData[seatIdx].pcgPaiType;
            int targetSeatIdx = playerStateData[seatIdx].pcgPaiTargetSeatIdx;
            float waitTime;
            float spacing = 0.0002f;

            HandDirection dir = pengPaiHandDirSeat[seatIdx];
            Animation anim = GetHandAnimation(seatIdx, handStyle, dir);

            switch (playerStateData[seatIdx].playerHandActionState)
            {
                case HandActionState.PENG_CHI_GANG_PAI_START:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, dir);
                        hand.SetActive(true);

                        waitTime = ReadyFirstHand(seatIdx, handStyle, dir, "PengPaiFirstHand");

                        //
                        AudioIdx audioIdx = AudioIdx.AUDIO_SPEAK_PENG;
                        GameObjectPool pool = mjAssetsMgr.pengPaiTextParticlePool;

                        switch (pcgPaiType)
                        {
                            case PengChiGangPaiType.PENG:
                                audioIdx = AudioIdx.AUDIO_SPEAK_PENG;
                                pool = mjAssetsMgr.pengPaiTextParticlePool;
                                break;

                            case PengChiGangPaiType.CHI:
                                audioIdx = AudioIdx.AUDIO_SPEAK_CHI;
                                pool = mjAssetsMgr.chiPaiTextParticlePool;
                                break;

                            case PengChiGangPaiType.GANG:
                            case PengChiGangPaiType.AN_GANG:
                            case PengChiGangPaiType.BU_GANG:
                                audioIdx = AudioIdx.AUDIO_SPEAK_GANG;
                                pool = mjAssetsMgr.gangPaiTextParticlePool;
                                break;
                        }

                        PlaySpeakAudio(handStyle, audioIdx, cameraTransform.position);

                        GameObject textEffect = pool.PopGameObject();
                        textEffect.transform.localPosition = pcgthEffectTextPosSeat[seatIdx];
                        textEffect.SetActive(true);
                        textEffect.GetComponent<ParticleSystem>().Play();

                        EffectFengRainEtcType effects = playerStateData[seatIdx].fengRainEtcEffect;
                        EffectFengRainEtcType effect = effects;
                        AudioClip effectAudio = null;
                        Vector3 effectPos = Vector3.zero;

                        for (int i = 0; i < 2; i++)
                        {
                            effect = GetEffectFengRainEtcType(effects, i);

                            switch (effect)
                            {
                                case EffectFengRainEtcType.EFFECT_RAIN:
                                    effectAudio = GetEffectAudio((int)AudioIdx.AUDIO_EFFECT_RAINY);
                                    pool = mjAssetsMgr.rainEffectPool;
                                    effectPos = rainEffectPos[seatIdx];
                                    break;

                                case EffectFengRainEtcType.EFFECT_FENG:
                                    effectAudio = GetEffectAudio((int)AudioIdx.AUDIO_EFFECT_WINDY);
                                    pool = mjAssetsMgr.longjuanfengEffectPool;
                                    effectPos = fengEffectPos[seatIdx];
                                    break;

                                case EffectFengRainEtcType.EFFECT_NONE:
                                    continue;
                            }

                            AudioSource.PlayClipAtPoint(effectAudio, cameraTransform.position);

                            GameObject eff = pool.PopGameObject();
                            eff.transform.localPosition = effectPos;
                            eff.SetActive(true);
                            eff.GetComponent<ParticleSystem>().Play();
                        }


                        //
                        playerStateData[seatIdx].SetPlayerState(HandActionState.PENG_CHI_GANG_PAI_READY_FIRST_HAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.PENG_CHI_GANG_PAI_READY_FIRST_HAND:
                    {
                        FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, playerStateData[seatIdx].actionCombineNum);

                        if (pcgPaiType == PengChiGangPaiType.BU_GANG)
                        {
                            playerStateData[seatIdx].pcgMjIdx = -1;
                            MjPaiData mjData;
                            for (int i = 0; i < deskPengPaiMjList[seatIdx].Count; i++)
                            {
                                mjData = deskPengPaiMjList[seatIdx][i][0].GetComponent<MjPaiData>();

                                if (mjData.mjFaceValue != playerStateData[seatIdx].pcgPaiMjfaceValues[0])
                                    continue;

                                playerStateData[seatIdx].pcgMjIdx = i;
                                break;
                            }

                            if (playerStateData[seatIdx].pcgMjIdx == -1)
                            {
                                playerStateData[seatIdx].SetPlayerState(HandActionState.PENG_CHI_GANG_PAI_END, Time.time, -1);
                                break;
                            }

                            playerStateData[seatIdx].pcgDstStartPos =
                                deskPengPaiMjPosInfoList[seatIdx][playerStateData[seatIdx].pcgMjIdx];
                        }
                        else
                        {
                            playerStateData[seatIdx].pcgDstStartPos =
                                NextPengChiGangPaiPos(seatIdx, pcgPaiType, playerStateData[seatIdx].pcgPaiLayoutIdx, spacing);
                        }

                        playerStateData[seatIdx].pcgOrgStartPos = playerStateData[seatIdx].pcgDstStartPos;

                        if (isMoveHand)
                        {
                            playerStateData[seatIdx].pcgOrgStartPos =
                                OffsetPengChiGangPaiStartPosByMoveDist(
                                    seatIdx, playerStateData[seatIdx].pcgDstStartPos, moveHandDist);
                        }

                        waitTime = MoveHandToDstOffsetPos(
                            seatIdx, handStyle, dir,
                            playerStateData[seatIdx].pcgOrgStartPos[1],
                            playerStateData[seatIdx].actionCombineNum);

                        playerStateData[seatIdx].SetPlayerState(HandActionState.PENG_CHI_GANG_PAI_MOVE_HAND_TO_DST_POS, Time.time, waitTime);
                    }
                    break;

                case HandActionState.PENG_CHI_GANG_PAI_MOVE_HAND_TO_DST_POS:
                    {
                        if (targetSeatIdx >= 0 && targetSeatIdx < playerCount)
                        {
                            Vector3Int targetMjIdx = playerStateData[seatIdx].pcgPaiTargetMjIdx;

                            if (targetMjIdx.x == -1 || targetMjIdx.y == -1 || targetMjIdx.z == -1)
                            {
                                targetMjIdx = GetCurtDeskMjPosIdx(targetSeatIdx);
                            }

                            int key = GetDeskDaPaiMjDictKey(targetMjIdx.x, targetMjIdx.y, targetMjIdx.z);

                            if (deskDaPaiMjDicts[targetSeatIdx].ContainsKey(key))
                            {
                                GameObject targetmj = deskDaPaiMjDicts[targetSeatIdx][key];
                                deskDaPaiMjDicts[targetSeatIdx].Remove(key);
                                RemoveMjFromDeskGlobalMjPaiSetDict(targetmj.GetComponent<MjPaiData>().mjFaceValue, targetmj);
                                mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(targetmj);

                                PrevDeskMjPos(targetSeatIdx);

                                if (targetmj == lastDaPaiMj)
                                    lastDaPaiMj = null;
                            }
                        }

                        if (pcgPaiType == PengChiGangPaiType.BU_GANG)
                        {
                            anim.Play("PengPai");
                            waitTime = GetHandActionWaitTime(seatIdx, handStyle, dir, "PengPai");
                            playerStateData[seatIdx].SetPlayerState(HandActionState.PENG_CHI_GANG_PAI_PCG_PAI, Time.time, waitTime);
                            break;
                        }

                        PengChiGangPaiPos[] pcgPos = GetPengChiGangPaiPosList(
                           seatIdx, pcgPaiType,
                           playerStateData[seatIdx].pcgOrgStartPos[0],
                           playerStateData[seatIdx].pcgPaiLayoutIdx,
                           spacing);

                        playerStateData[seatIdx].pcgMjList = CreatePengChiGangPaiList(
                            pcgPos,
                            playerStateData[seatIdx].pcgPaiMjfaceValues,
                            pcgPaiType);

                        for (int i = 0; i < playerStateData[seatIdx].pcgMjList.Length; i++)
                        {
                            playerStateData[seatIdx].pcgMjList[i].transform.SetParent(mjtableTransform, true);
                        }

                        GameObject pengPaiParticle = mjAssetsMgr.pcgPaiParticlePool.PopGameObject();
                        pengPaiParticle.SetActive(true);

                        Vector3 eulerAngles = pengPaiParticle.transform.eulerAngles;
                        pengPaiParticle.transform.eulerAngles = new Vector3(eulerAngles.x, pcgParticleEulerAnglesY[seatIdx], eulerAngles.z);

                        Vector3 pos = playerStateData[seatIdx].pcgOrgStartPos[1];
                        pengPaiParticle.transform.position = new Vector3(pos.x, pos.y + GetDeskPengChiGangMjSizeByAxis(Axis.Y) / 3.0f, pos.z);
                        pengPaiParticle.GetComponent<ParticleSystem>().Play();

                        anim.Play("PengPai");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, dir, "PengPai");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.PENG_CHI_GANG_PAI_PCG_PAI, Time.time, waitTime);
                    }
                    break;


                case HandActionState.PENG_CHI_GANG_PAI_PCG_PAI:
                    {
                        Vector3 distOffset = playerStateData[seatIdx].pcgDstStartPos[1] - playerStateData[seatIdx].pcgOrgStartPos[1];
                        GameObject hand = GetHand(seatIdx, handStyle, dir);

                        if (pcgPaiType == PengChiGangPaiType.BU_GANG)
                        {
                            waitTime = 0.04f;
                            hand.transform.DOMove(distOffset, waitTime).SetRelative();

                            int mjIdx = playerStateData[seatIdx].pcgMjIdx;

                            GameObject[] oldMjList = deskPengPaiMjList[seatIdx][mjIdx];
                            Vector3[] oldPosInfo = deskPengPaiMjPosInfoList[seatIdx][mjIdx];

                            for (int i = 0; i < oldMjList.Length; i++)
                            {
                                RemoveMjFromDeskGlobalMjPaiSetDict(oldMjList[i].GetComponent<MjPaiData>().mjFaceValue, oldMjList[i]);
                                mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(oldMjList[i]);
                            }

                            deskPengPaiMjList[seatIdx].RemoveAt(mjIdx);
                            deskPengPaiMjPosInfoList[seatIdx].RemoveAt(mjIdx);

                            int paiLayoutIdx = Random.Range(0, 4);
                            Vector3 outEndValue;

                            Vector3[] newPosInfo = GetPengChiGangPaiPos(
                                seatIdx, PengChiGangPaiType.GANG, playerStateData[seatIdx].pcgDstStartPos[0], paiLayoutIdx, out outEndValue, spacing);

                            PengChiGangPaiPos[] newMjPaiPos = GetPengChiGangPaiPosList(
                                          seatIdx, PengChiGangPaiType.GANG,
                                          newPosInfo[0],
                                          paiLayoutIdx,
                                          spacing);

                            MahjongFaceValue value = playerStateData[seatIdx].pcgPaiMjfaceValues[0];

                            GameObject[] newMjList = CreatePengChiGangPaiList(
                                newMjPaiPos,
                                new MahjongFaceValue[] { value, value, value, value },
                                PengChiGangPaiType.GANG);

                            for (int i = 0; i < newMjList.Length; i++)
                            {
                                newMjList[i].transform.SetParent(mjtableTransform, true);
                                AppendMjToDeskGlobalMjPaiSetDict(newMjList[i].GetComponent<MjPaiData>().mjFaceValue, newMjList[i]);
                            }

                            deskGangPaiMjList[seatIdx].Add(newMjList);

                            float dist = Mathf.Abs(newPosInfo[2].x - oldPosInfo[2].x);

                            Vector3[] offsetPos =
                                OffsetPengChiGangPaiStartPosByMoveDist(
                                    seatIdx, playerStateData[seatIdx].pcgDstStartPos, dist);

                            distOffset = offsetPos[1] - playerStateData[seatIdx].pcgDstStartPos[1];

                            pengPaiCurtPosSeat[seatIdx] += distOffset;

                            GameObject[] mjs;
                            List<GameObject[]> goList = null;

                            for (int m = 0; m < 3; m++)
                            {
                                switch (m)
                                {
                                    case 0: goList = deskPengPaiMjList[seatIdx]; break;
                                    case 1: goList = deskGangPaiMjList[seatIdx]; break;
                                    case 2: goList = deskChiPaiMjList[seatIdx]; break;
                                }

                                for (int i = 0; i < goList.Count; i++)
                                {
                                    mjs = goList[i];
                                    if (IsAtBackPos(seatIdx, mjs[0].transform.position, newPosInfo[1]))
                                    {
                                        if (m == 0)
                                        {
                                            Vector3[] pos = deskPengPaiMjPosInfoList[seatIdx][i];
                                            pos[0] += distOffset;
                                            pos[1] += distOffset;
                                        }

                                        for (int j = 0; j < mjs.Length; j++)
                                            mjs[j].transform.DOMove(distOffset, waitTime).SetRelative();
                                    }
                                }
                            }


                            playerStateData[seatIdx].SetPlayerState(HandActionState.PENG_CHI_GANG_PAI_MOVE_PAI, Time.time, waitTime);
                            break;
                        }

                        waitTime = 0.3f;
                        hand.transform.DOMove(distOffset, waitTime).SetRelative();

                        for (int i = 0; i < playerStateData[seatIdx].pcgMjList.Length; i++)
                        {
                            playerStateData[seatIdx].pcgMjList[i].transform.DOMove(distOffset, waitTime).SetRelative();
                        }

                        playerStateData[seatIdx].SetPlayerState(HandActionState.PENG_CHI_GANG_PAI_MOVE_PAI, Time.time, waitTime);
                    }
                    break;


                case HandActionState.PENG_CHI_GANG_PAI_MOVE_PAI:
                    {
                        if (pcgPaiType != PengChiGangPaiType.BU_GANG)
                        {
                            AddDeskPaiToPengChiGangPaiList(
                                seatIdx, pcgPaiType,
                                playerStateData[seatIdx].pcgMjList,
                                playerStateData[seatIdx].pcgDstStartPos);

                            GameObject mj;
                            for (int i = 0; i < playerStateData[seatIdx].pcgMjList.Length; i++)
                            {
                                mj = playerStateData[seatIdx].pcgMjList[i];
                                AppendMjToDeskGlobalMjPaiSetDict(mj.GetComponent<MjPaiData>().mjFaceValue, mj);
                            }
                        }

                        anim.Play("PengPaiEndTaiHand");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, dir, "PengPaiEndTaiHand");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.PENG_CHI_GANG_PAI_TAIHAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.PENG_CHI_GANG_PAI_TAIHAND:
                    {
                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, dir);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.PENG_CHI_GANG_PAI_END, Time.time, waitTime);

                    }
                    break;

                case HandActionState.PENG_CHI_GANG_PAI_END:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, dir);
                        hand.SetActive(false);
                        playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;
  
                        ProcessHandActionMjOpCmdList(seatIdx);
                    }
                    break;
            }

        }

        #endregion

        #region 插牌动作
        void ActionChaPai()
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (isCanUseSeatPlayer[i])
                    ActionChaPai(i);
            }
        }
        void ActionChaPai(int seatIdx)
        {
            if (playerStateData[seatIdx].playerHandActionState < HandActionState.CHA_PAI_START ||
             playerStateData[seatIdx].playerHandActionState > HandActionState.CHA_PAI_END)
            {
                return;
            }

            if (seatIdx == 0)
            {
                if (Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
                    return;

                ActionChaPaiForSeat0();
                return;
            }


            if (Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
            {
                MoveChaPaiHandShadow(seatIdx);
                return;
            }

            int orgPaiIdx = playerStateData[seatIdx].orgPaiIdx;
            int dstHandPaiIdx = playerStateData[seatIdx].chaPaiDstHandPaiIdx;
            HandPaiType orgPaiType = playerStateData[seatIdx].chaPaiHandPaiType;
            HandPaiAdjustDirection adjustDirection = playerStateData[seatIdx].adjustDirection;
            PlayerType handStyle = playerStateData[seatIdx].handStyle;
            GameObject mj = playerStateData[seatIdx].curtAdjustHandPai;

            float waitTime = 0.3f;
            Animation anim = GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);

            switch (playerStateData[seatIdx].playerHandActionState)
            {
                case HandActionState.CHA_PAI_START:
                    {
                        GameObject dstPai = mjSeatHandPaiLists[seatIdx][dstHandPaiIdx];
                        playerStateData[seatIdx].dstHandPaiPostion = dstPai.transform.position;

                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(true);
                        ReadyZhuaHandPai(seatIdx, handStyle, HandDirection.RightHand, orgPaiIdx, orgPaiType);

                        anim.Play("zhuaHandPai");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "zhuaHandPai");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.CHA_PAI_ZHUA_HAND_PAI, Time.time, waitTime);

                        playerStateData[seatIdx].handShadowAxis[0] = GetHandShadowAxis(seatIdx, handStyle, HandDirection.RightHand, 2).transform;
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(true);
                        MoveChaPaiHandShadow(seatIdx);
                    }
                    break;


                case HandActionState.CHA_PAI_ZHUA_HAND_PAI:
                    {
                        if (orgPaiType == HandPaiType.HandPai)
                            mj = mjSeatHandPaiLists[seatIdx][orgPaiIdx];
                        else
                            mj = mjSeatMoPaiLists[seatIdx][orgPaiIdx];

                        if (mj == null)
                        {
                            playerStateData[seatIdx].SetPlayerState(HandActionState.CHA_PAI_END, Time.time, 0);
                            return;
                        }

                        OffMjShadow(mj);
                        if (orgPaiType == HandPaiType.HandPai)
                            mjSeatHandPaiLists[seatIdx][orgPaiIdx] = null;
                        else
                            mjSeatMoPaiLists[seatIdx].RemoveAt(orgPaiIdx);

                        playerStateData[seatIdx].curtAdjustHandPai = mj;
                        GameObject bone = GetHandBone(seatIdx, handStyle, HandDirection.RightHand, 0);
                        mj.transform.SetParent(bone.transform, true);

                        anim.Play("TiHandPai");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "TiHandPai");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.CHA_PAI_TI_HAND_PAI, Time.time, waitTime);
                    }
                    break;

                case HandActionState.CHA_PAI_TI_HAND_PAI:
                    {
                        if (orgPaiType == HandPaiType.MoPai || orgPaiIdx != dstHandPaiIdx)
                        {
                            List<Vector3> handOffsetList = GetDeskMjHandOffsetList(seatIdx, handStyle, HandDirection.RightHand);
                            Vector3 dstMjPos = mjSeatHandPaiLists[seatIdx][dstHandPaiIdx].transform.position;
                            Vector3 endValue = dstMjPos + handOffsetList[(int)ActionCombineNum.ChaPai];
                            GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                            Dictionary<string, ActionDataInfo> actionDataDict = GetHandActionDataDict(seatIdx, handStyle, HandDirection.RightHand);

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

                            playerStateData[seatIdx].SetPlayerState(HandActionState.CHA_PAI_TI_HAND_PAI_MOVE, Time.time, waitTime);
                            break;
                        }

                        string actionName = null;
                        if (orgPaiIdx == dstHandPaiIdx && orgPaiType == HandPaiType.HandPai)
                            actionName = "PutDownHandPai1";
                        else
                            actionName = "PutDownHandPai2";

                        anim.Play(actionName);
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, actionName);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.CHA_PAI_PUTDOWNHAND, Time.time, waitTime);
                    }
                    break;


                case HandActionState.CHA_PAI_TI_HAND_PAI_MOVE:
                    {
                        string actionName = null;
                        if (orgPaiIdx == dstHandPaiIdx && orgPaiType == HandPaiType.HandPai)
                            actionName = "PutDownHandPai1";
                        else
                            actionName = "PutDownHandPai2";

                        anim.Play(actionName);
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, actionName);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.CHA_PAI_PUTDOWNHAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.CHA_PAI_PUTDOWNHAND:
                    {
                        AdjustPai(seatIdx, mj, dstHandPaiIdx, adjustDirection, playerStateData[seatIdx].playerStateLiveTime);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.CHA_PAI_ADJUST_PAI, Time.time, playerStateData[seatIdx].playerStateLiveTime);
                    }
                    break;

                case HandActionState.CHA_PAI_ADJUST_PAI:
                    {
                        OnMjShadow(mj, 0);

                        playerStateData[seatIdx].curtAdjustHandPai.transform.SetParent(mjtableTransform, true);
                        playerStateData[seatIdx].curtAdjustHandPai.transform.position = playerStateData[seatIdx].dstHandPaiPostion;
                        playerStateData[seatIdx].curtAdjustHandPai.transform.eulerAngles = GetSeatHandMjFitEulerAngles(seatIdx);
                        playerStateData[seatIdx].curtAdjustHandPai = null;

                        anim.Play("TaiHand");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "TaiHand");
                        playerStateData[seatIdx].SetPlayerState(HandActionState.CHA_PAI_TAIHAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.CHA_PAI_TAIHAND:
                    {
                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.RightHand);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.CHA_PAI_END, Time.time, waitTime);
                    }
                    break;

                case HandActionState.CHA_PAI_END:
                    {
                        GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        hand.SetActive(false);
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(false);
                        playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;

                        ProcessHandActionMjOpCmdList(seatIdx);
                    }
                    break;

            }
        }

        void MoveChaPaiHandShadow(int seatIdx)
        {
            Transform handShadowRef = playerStateData[seatIdx].handShadowAxis[0];
            float ang = 360 - handShadowRef.eulerAngles.y;

            float matUOffset = ((handShadowRef.position.x - handShadowPlaneInfos[seatIdx].shadowOrgPos[2].x) / planeSize.x) * handShadowPlaneInfos[seatIdx].tiling[2].x + handShadowPlaneInfos[seatIdx].offset[2].x;
            float matVOffset = ((handShadowRef.position.z - handShadowPlaneInfos[seatIdx].shadowOrgPos[2].y) / planeSize.z) * handShadowPlaneInfos[seatIdx].tiling[2].y + handShadowPlaneInfos[seatIdx].offset[2].y;

            handShadowPlaneInfos[seatIdx].planeMat.SetTextureOffset(shaderTexNames[2], new Vector2(matUOffset, matVOffset));
            handShadowPlaneInfos[seatIdx].planeMat.SetFloat(shaderAngNames[2], ang);

        }


        void ActionChaPaiForSeat0()
        {
            int orgPaiIdx = playerStateData[0].orgPaiIdx;
            int dstHandPaiIdx = playerStateData[0].chaPaiDstHandPaiIdx;
            HandPaiType orgPaiType = playerStateData[0].chaPaiHandPaiType;
            HandPaiAdjustDirection adjustDirection = playerStateData[0].adjustDirection;

            float waitTime = 0.4f;
            List<GameObject> mjHandPaiList = mjSeatHandPaiLists[0];
            List<GameObject> mjMoPaiList = mjSeatMoPaiLists[0];

            GameObject mj = playerStateData[0].curtAdjustHandPai;
            Vector3 dstPos = playerStateData[0].dstHandPaiPostion;


            float y = GetCanvasHandMjSizeByAxis(Axis.Y);
            y += y / 3;

            switch (playerStateData[0].playerHandActionState)
            {
                case HandActionState.CHA_PAI_START:
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
                            playerStateData[0].SetPlayerState(HandActionState.CHA_PAI_END, Time.time, 0);
                            return;
                        }

                        GameObject dstPai = mjHandPaiList[dstHandPaiIdx];
                        playerStateData[0].dstHandPaiPostion = dstPai.transform.localPosition;

                        if (orgPaiType == HandPaiType.HandPai)
                        {
                            mjHandPaiList[orgPaiIdx] = null;
                        }
                        else
                        {
                            mjMoPaiList.RemoveAt(orgPaiIdx);
                        }


                        OffMjShadow(mj);
                        playerStateData[0].curtAdjustHandPai = mj;
                        Vector3 endPos = mj.transform.localPosition;
                        endPos.y += y;
                        mj.transform.DOLocalMove(endPos, waitTime);
                        playerStateData[0].SetPlayerState(HandActionState.CHA_PAI_TI_HAND_PAI, Time.time, waitTime);
                    }
                    break;

                case HandActionState.CHA_PAI_TI_HAND_PAI:
                    {
                        if (orgPaiType == HandPaiType.MoPai || orgPaiIdx != dstHandPaiIdx)
                        {
                            Vector3 dstEulerAngles = new Vector3(mj.transform.localEulerAngles.x, mj.transform.localEulerAngles.y, mj.transform.localEulerAngles.z + 40f);
                            Vector3 dstPos2 = new Vector3(dstPos.x, dstPos.y + y, dstPos.z);
                            mj.transform.DOLocalMove(dstPos2, waitTime);
                            mj.transform.DOLocalRotate(dstEulerAngles, waitTime);
                            playerStateData[0].SetPlayerState(HandActionState.CHA_PAI_TI_HAND_PAI_MOVE, Time.time, waitTime);
                            break;

                        }

                        if (orgPaiIdx == dstHandPaiIdx && orgPaiType == HandPaiType.HandPai)
                        {
                            mj.transform.DOLocalMove(dstPos, waitTime);
                            playerStateData[0].SetPlayerState(HandActionState.CHA_PAI_PUTDOWNHAND, Time.time, waitTime / 3);
                        }
                        else
                        {
                            mj.transform.DOLocalRotate(canvasHandMjFitEulerAngles, waitTime);
                            mj.transform.DOLocalMove(dstPos, waitTime).SetEase(Ease.InCirc);
                            playerStateData[0].SetPlayerState(HandActionState.CHA_PAI_PUTDOWNHAND, Time.time, waitTime / 3);
                        }
                    }
                    break;

                case HandActionState.CHA_PAI_TI_HAND_PAI_MOVE:
                    {
                        if (orgPaiIdx == dstHandPaiIdx && orgPaiType == HandPaiType.HandPai)
                        {
                            mj.transform.DOLocalMove(dstPos, waitTime);
                            playerStateData[0].SetPlayerState(HandActionState.CHA_PAI_PUTDOWNHAND, Time.time, waitTime / 3);
                        }
                        else
                        {
                            mj.transform.DOLocalRotate(canvasHandMjFitEulerAngles, waitTime);
                            mj.transform.DOLocalMove(dstPos, waitTime).SetEase(Ease.InCirc);
                            playerStateData[0].SetPlayerState(HandActionState.CHA_PAI_PUTDOWNHAND, Time.time, waitTime / 3);
                        }
                    }
                    break;

                case HandActionState.CHA_PAI_PUTDOWNHAND:
                    {
                        AdjustPai(0, mj, dstHandPaiIdx, adjustDirection, waitTime / 3);
                        playerStateData[0].SetPlayerState(HandActionState.CHA_PAI_END, Time.time, waitTime / 3 * 2);
                    }
                    break;

                case HandActionState.CHA_PAI_END:
                    {
                        OnMjShadow(mj);
                        playerStateData[0].curtAdjustHandPai.transform.localPosition = dstPos;
                        playerStateData[0].curtAdjustHandPai.transform.localEulerAngles = canvasHandMjFitEulerAngles;
                        playerStateData[0].curtAdjustHandPai = null;

                        playerStateData[0].playerHandActionState = HandActionState.ACTION_END;

                        if (mjOpCmdList != null)
                            mjOpCmdList.RemoveHandActionOpCmd(0, playerStateData[0].opCmdNode);
                    }
                    break;

            }
        }


        float ReadyZhuaHandPai(int seatIdx, PlayerType handStyle, HandDirection handDir, int handPaiIdx, HandPaiType paiType)
        {
            GameObject mj;
            float tm = 0.3f;

            if (paiType == HandPaiType.HandPai)
                mj = mjSeatHandPaiLists[seatIdx][handPaiIdx];
            else
                mj = mjSeatMoPaiLists[seatIdx][handPaiIdx];

            if (mj == null)
                return 0;

            FitHandPoseForSeat(seatIdx, handStyle, handDir, ActionCombineNum.ChaPai);
            tm = MoveHandToDstOffsetPos(seatIdx, handStyle, handDir, mj.transform.position, ActionCombineNum.ChaPai);
            return tm;
        }


        void AdjustPai(int seatIdx, GameObject orgPai, int dstPaiIdx, HandPaiAdjustDirection adjustDirection, float tm)
        {
            List<GameObject> mjHandPaiList = mjSeatHandPaiLists[seatIdx];
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
                            float mjsize = GetCanvasHandMjSizeByAxis(Axis.X);
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
                            float mjsize = GetHandMjSizeByAxis(Axis.X);
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
                            float mjsize = GetHandMjSizeByAxis(Axis.X);
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
                            float mjsize = GetHandMjSizeByAxis(Axis.X);
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
                            float mjsize = GetCanvasHandMjSizeByAxis(Axis.X);
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
                            float mjsize = GetHandMjSizeByAxis(Axis.X);
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
                            float mjsize = GetHandMjSizeByAxis(Axis.X);
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
                            float mjsize = GetHandMjSizeByAxis(Axis.X);
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

        #region 整理牌动作
        /// <summary>
        /// 整理牌动作
        /// </summary>
        /// <param name="seatIdx">对应的玩家座号</param>
        /// <returns></returns>
        void ActionSortPai()
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (isCanUseSeatPlayer[i])
                    ActionSortPai(i);
            }
        }
        void ActionSortPai(int seatIdx)
        {
            if (playerStateData[seatIdx].playerHandActionState < HandActionState.SORT_PAI_START ||
             playerStateData[seatIdx].playerHandActionState > HandActionState.SORT_PAI_END ||
             Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
            {
                return;
            }

            if (playerStateData[seatIdx].playerHandActionState == HandActionState.SORT_PAI_END)
            {
                playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;
                ProcessHandActionMjOpCmdList(seatIdx);
                return;
            }

            SortPaiType sortPaiType = playerStateData[seatIdx].sortPaiType;

            List<GameObject> mjHandPaiList = mjSeatHandPaiLists[seatIdx];
            List<GameObject> mjMoPaiList = mjSeatMoPaiLists[seatIdx];
            float mjSpacing = 0;
            float mjCount = mjHandPaiList.Count;
            Transform mjtf;
            float tm = 0.2f;
            Vector3 dstPos = new Vector3();

            switch (seatIdx)
            {
                case 0:
                    {
                        float mjsize = GetCanvasHandMjSizeByAxis(Axis.X);
                        float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
                        float mjStartPos = 0;
                        float mjAxisSpacing = mjsize + mjSpacing;

                        switch (sortPaiType)
                        {
                            case SortPaiType.LEFT:
                                mjStartPos = mjSeatHandPaiPosLists[seatIdx][0].x;
                                break;

                            case SortPaiType.MIDDLE:
                                mjStartPos = -mjWallLen / 2 + mjsize / 2;
                                break;
                        }


                        dstPos = new Vector3(mjStartPos, mjSeatHandPaiPosLists[0][0].y, mjSeatHandPaiPosLists[0][0].z);

                        for (int i = 0; i < mjCount; i++)
                        {
                            mjtf = mjHandPaiList[i].transform;
                            dstPos = new Vector3(mjStartPos + i * mjAxisSpacing, mjtf.localPosition.y, mjtf.localPosition.z);
                            mjtf.DOLocalMove(dstPos, tm);
                        }

                        if (mjCount > 0)
                            dstPos.x += mjsize + moPaiToHandPaiCanvasOffset;

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
                    }
                    break;

                case 1:
                    {
                        float mjsize = GetHandMjSizeByAxis(Axis.X);
                        float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
                        float mjStartPos = 0;
                        float mjAxisSpacing = mjsize + mjSpacing;

                        switch (sortPaiType)
                        {
                            case SortPaiType.LEFT:
                                mjStartPos = mjSeatHandPaiPosLists[seatIdx][0].z;
                                break;

                            case SortPaiType.MIDDLE:
                                mjStartPos = -mjWallLen / 2 + mjsize / 2 + mjtableTransform.transform.position.z;
                                break;
                        }


                        dstPos = new Vector3(mjSeatHandPaiPosLists[1][0].x, mjSeatHandPaiPosLists[1][0].y, mjStartPos);

                        for (int i = 0; i < mjCount; i++)
                        {
                            mjtf = mjHandPaiList[i].transform;
                            dstPos = new Vector3(mjtf.position.x, mjtf.position.y, mjStartPos + i * mjAxisSpacing);
                            mjtf.DOMove(dstPos, tm);
                        }

                        if (mjCount > 0)
                            dstPos.z += mjsize + moPaiToHandPaiOffset;

                        if (mjMoPaiList.Count > 0)
                        {
                            Vector3 offset = dstPos - mjMoPaiList[0].transform.position;

                            for (int i = 0; i < mjMoPaiList.Count; i++)
                            {
                                mjtf = mjMoPaiList[i].transform;
                                dstPos = mjtf.transform.position + offset;
                                mjtf.DOMove(dstPos, tm);
                            }
                        }
                    }
                    break;


                case 2:
                    {
                        float mjsize = GetHandMjSizeByAxis(Axis.X);
                        float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
                        float mjStartPos = 0;
                        float mjAxisSpacing = mjsize + mjSpacing;

                        switch (sortPaiType)
                        {
                            case SortPaiType.LEFT:
                                mjStartPos = mjSeatHandPaiPosLists[seatIdx][0].x;
                                break;

                            case SortPaiType.MIDDLE:
                                mjStartPos = -mjWallLen / 2 + mjsize / 2 + mjtableTransform.transform.position.x;
                                break;
                        }

                        dstPos = new Vector3(mjSeatHandPaiPosLists[2][0].x, mjSeatHandPaiPosLists[2][0].y, mjStartPos);

                        for (int i = 0; i < mjCount; i++)
                        {
                            mjtf = mjHandPaiList[i].transform;
                            dstPos = new Vector3(mjStartPos + i * mjAxisSpacing, mjtf.position.y, mjtf.position.z);
                            mjtf.DOMove(dstPos, tm);
                        }

                        if (mjCount > 0)
                            dstPos.x += mjsize + moPaiToHandPaiOffset;

                        if (mjMoPaiList.Count > 0)
                        {
                            Vector3 offset = dstPos - mjMoPaiList[0].transform.position;

                            for (int i = 0; i < mjMoPaiList.Count; i++)
                            {
                                mjtf = mjMoPaiList[i].transform;
                                dstPos = mjtf.transform.position + offset;
                                mjtf.DOMove(dstPos, tm);
                            }
                        }
                    }
                    break;


                case 3:
                    {
                        float mjsize = GetHandMjSizeByAxis(Axis.X);
                        float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
                        float mjStartPos = 0;
                        float mjAxisSpacing = mjsize + mjSpacing;

                        switch (sortPaiType)
                        {
                            case SortPaiType.LEFT:
                                mjStartPos = mjSeatHandPaiPosLists[seatIdx][0].z;
                                break;

                            case SortPaiType.MIDDLE:
                                mjStartPos = mjWallLen / 2 - mjsize / 2 + mjtableTransform.transform.position.z;
                                break;
                        }

                        dstPos = new Vector3(mjSeatHandPaiPosLists[3][0].x, mjSeatHandPaiPosLists[3][0].y, mjStartPos);

                        for (int i = 0; i < mjCount; i++)
                        {
                            mjtf = mjHandPaiList[i].transform;
                            dstPos = new Vector3(mjtf.position.x, mjtf.position.y, mjStartPos - i * mjAxisSpacing);
                            mjtf.DOMove(dstPos, tm);
                        }

                        if (mjCount > 0)
                            dstPos.z -= mjsize + moPaiToHandPaiOffset;

                        if (mjMoPaiList.Count > 0)
                        {
                            Vector3 offset = dstPos - mjMoPaiList[0].transform.position;

                            for (int i = 0; i < mjMoPaiList.Count; i++)
                            {
                                mjtf = mjMoPaiList[i].transform;
                                dstPos = mjtf.transform.position + offset;
                                mjtf.DOMove(dstPos, tm);
                            }
                        }
                    }

                    break;

            }

            playerStateData[seatIdx].SetPlayerState(HandActionState.SORT_PAI_END, Time.time, tm);
        }

        #endregion

        #region 推倒牌动作
        void ActionTuiDaoPai()
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (isCanUseSeatPlayer[i])
                    ActionTuiDaoPai(i);
            }
        }
        void ActionTuiDaoPai(int seatIdx)
        {
            if (playerStateData[seatIdx].playerHandActionState < HandActionState.TUIDAO_PAI_START ||
                playerStateData[seatIdx].playerHandActionState > HandActionState.TUIDAO_PAI_END)
            {
                return;
            }

            if ((seatIdx == 0 && mjSeatHandPaiLists[seatIdx].Count == 0) ||
                playerStateData[seatIdx].handPaiValueList.Count == 0)
            {
                playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;

                if (mjOpCmdList != null)
                    mjOpCmdList.RemoveHandActionOpCmd(seatIdx, playerStateData[seatIdx].opCmdNode);

                return;
            }

            if (Time.time - playerStateData[seatIdx].playerStateStartTime < playerStateData[seatIdx].playerStateLiveTime)
            {
                MoveTuiDaoPaiHandShadow(seatIdx);
                return;
            }

            PlayerType handStyle = playerStateData[seatIdx].handStyle;

            float waitTime = 0.3f;
            Animation leftHandAnim = GetHandAnimation(seatIdx, handStyle, HandDirection.LeftHand);
            Animation rightHandAnim = GetHandAnimation(seatIdx, handStyle, HandDirection.RightHand);

            switch (playerStateData[seatIdx].playerHandActionState)
            {
                case HandActionState.TUIDAO_PAI_START:
                    {
                        GameObject rightHand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        rightHand.SetActive(true);
                        ReadyZhuaHandPai2(seatIdx, handStyle);

                        rightHandAnim.Play("ZhuaHandPai2");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "ZhuaHandPai2");
                        playerStateData[seatIdx].handShadowAxis[1] = GetHandShadowAxis(seatIdx, handStyle, HandDirection.RightHand, 0).transform;


                        if ((seatIdx == 0 && mjSeatHandPaiLists[seatIdx].Count >= 3) ||
                           playerStateData[seatIdx].handPaiValueList.Count >= 3)
                        {
                            GameObject leftHand = GetHand(seatIdx, handStyle, HandDirection.LeftHand);
                            leftHand.SetActive(true);
                            leftHandAnim.Play("ZhuaHandPai2");

                            playerStateData[seatIdx].handShadowAxis[0] = GetHandShadowAxis(seatIdx, handStyle, HandDirection.LeftHand, 1).transform;
                        }

                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(true);
                        MoveTuiDaoPaiHandShadow(seatIdx);

                        playerStateData[seatIdx].SetPlayerState(HandActionState.TUIDAO_PAI_ZHUA_HAND_PAI, Time.time, waitTime + 0.06f);
                    }
                    break;

                case HandActionState.TUIDAO_PAI_ZHUA_HAND_PAI:
                    {
                        leftHandAnim.Play("ZhuaHandPai2EndBackMoveHandPai");
                        rightHandAnim.Play("ZhuaHandPai2EndBackMoveHandPai");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "ZhuaHandPai2EndBackMoveHandPai");

                        List<GameObject> mjHandPaiList = mjSeatHandPaiLists[seatIdx];
                        List<MahjongFaceValue> handPaiValueList = playerStateData[seatIdx].handPaiValueList;
                        GameObject mjTmp;
                        Vector3 handPailastMjPos = mjSeatHandPaiPosLists[seatIdx][0];

                        if (mjHandPaiList.Count != 0)
                            handPailastMjPos = mjHandPaiList[mjHandPaiList.Count - 1].transform.position;

                        switch (seatIdx)
                        {
                            case 0:
                                for (int i = 0; i < mjHandPaiList.Count; i++)
                                {
                                    mjHandPaiList[i].transform.DOLocalMoveZ(-200, waitTime).SetRelative();
                                    OffMjShadow(mjHandPaiList[i]);
                                }
                                break;

                            case 2:

                                for (int i = 0; i < handPaiValueList.Count; i++)
                                {
                                    if (i < mjHandPaiList.Count)
                                    {
                                        mjTmp = mjHandPaiList[i];
                                        mjHandPaiList[i] = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(handPaiValueList[i]);
                                        mjHandPaiList[i].layer = defaultLayer;
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
                                        mjTmp.layer = defaultLayer;
                                        FitSeatHandMj(seatIdx, mjTmp, false);

                                        handPailastMjPos = new Vector3(
                                            handPailastMjPos.x + GetHandMjSizeByAxis(Axis.X),
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
                                        mjHandPaiList[i].layer = defaultLayer;
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
                                        mjTmp.layer = defaultLayer;
                                        FitSeatHandMj(seatIdx, mjTmp, false);
                                        handPailastMjPos = new Vector3(handPailastMjPos.x, mjTmp.transform.position.y, handPailastMjPos.z + GetHandMjSizeByAxis(Axis.X));
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
                                        mjHandPaiList[i].layer = defaultLayer;
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
                                        mjTmp.layer = defaultLayer;
                                        FitSeatHandMj(seatIdx, mjTmp, false);
                                        handPailastMjPos = new Vector3(handPailastMjPos.x, mjTmp.transform.position.y, handPailastMjPos.z - GetHandMjSizeByAxis(Axis.X));
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

                        playerStateData[seatIdx].SetPlayerState(HandActionState.TUIDAO_PAI_BACK_MOVE_HAND_PAI, Time.time, waitTime);
                    }
                    break;


                case HandActionState.TUIDAO_PAI_BACK_MOVE_HAND_PAI:
                    {
                        leftHandAnim.Play("BackMoveHandPaiEndTuiDaoHandPai");
                        rightHandAnim.Play("BackMoveHandPaiEndTuiDaoHandPai");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "BackMoveHandPaiEndTuiDaoHandPai");

                        Vector3 pos;
                        List<GameObject> mjHandPaiList = mjSeatHandPaiLists[seatIdx];
                        switch (seatIdx)
                        {
                            case 0:
                                for (int i = 0; i < mjHandPaiList.Count; i++)
                                {
                                    mjHandPaiList[i].transform.DOLocalRotate(new Vector3(canvasHandPaiRectTransform.localEulerAngles.x - 90, 0, 0), waitTime).SetRelative();

                                    pos = mjSeatHandPaiPosLists[4][i];
                                    //pos.z -=  GetHandMjSizeByAxis(Axis.Y) / 2;
                                    pos.y = deskFacePosY + GetHandMjSizeByAxis(Axis.Z) / 2 + 0.0001f;
                                    mjHandPaiList[i].transform.DOMove(pos, waitTime + 0.01f);
                                }
                                break;

                            case 2:
                                for (int i = 0; i < mjHandPaiList.Count; i++)
                                {
                                    mjHandPaiList[i].transform.DOLocalRotate(new Vector3(-90, 0, 0), waitTime).SetRelative();

                                    pos = mjHandPaiList[i].transform.position;
                                    pos.z += GetHandMjSizeByAxis(Axis.X) / 2 + GetHandMjSizeByAxis(Axis.Y) / 2;
                                    pos.y = deskFacePosY + GetHandMjSizeByAxis(Axis.Z) / 2 + 0.0001f;
                                    mjHandPaiList[i].transform.DOMove(pos, waitTime + 0.01f);
                                }
                                break;

                            case 1:

                                for (int i = 0; i < mjHandPaiList.Count; i++)
                                {
                                    mjHandPaiList[i].transform.DOLocalRotate(new Vector3(-90, 0, 0), waitTime).SetRelative();

                                    pos = mjHandPaiList[i].transform.position;
                                    pos.x -= GetHandMjSizeByAxis(Axis.X) / 2 + GetHandMjSizeByAxis(Axis.Y) / 2;
                                    pos.y = deskFacePosY + GetHandMjSizeByAxis(Axis.Z) / 2 + 0.0001f;
                                    mjHandPaiList[i].transform.DOMove(pos, waitTime + 0.01f);
                                }
                                break;

                            case 3:
                                for (int i = 0; i < mjHandPaiList.Count; i++)
                                {
                                    mjHandPaiList[i].transform.DOLocalRotate(new Vector3(-90, 0, 0), waitTime).SetRelative();

                                    pos = mjHandPaiList[i].transform.position;
                                    pos.x += GetHandMjSizeByAxis(Axis.X) / 2 + GetHandMjSizeByAxis(Axis.Y) / 2;
                                    pos.y = deskFacePosY + GetHandMjSizeByAxis(Axis.Z) / 2 + 0.0001f;
                                    mjHandPaiList[i].transform.DOMove(pos, waitTime + 0.01f);
                                }
                                break;
                        }

                        playerStateData[seatIdx].SetPlayerState(HandActionState.TUIDAO_PAI_TUIDAO_HAND_PAI, Time.time, waitTime + 0.01f);
                    }
                    break;

                case HandActionState.TUIDAO_PAI_TUIDAO_HAND_PAI:
                    {

                        List<GameObject> mjHandPaiList = mjSeatHandPaiLists[seatIdx];

                        for (int i = 0; i < mjHandPaiList.Count; i++)
                        {
                            OnMjShadow(mjHandPaiList[i], 0);
                        }

                        leftHandAnim.Play("TuiDaoHandPaiEndTaiHand");
                        rightHandAnim.Play("TuiDaoHandPaiEndTaiHand");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, HandDirection.RightHand, "TuiDaoHandPaiEndTaiHand");

                        playerStateData[seatIdx].SetPlayerState(HandActionState.TUIDAO_PAI_TUIDAO_HAND_PAI_TAIHAND, Time.time, waitTime);
                    }
                    break;

                case HandActionState.TUIDAO_PAI_TUIDAO_HAND_PAI_TAIHAND:
                    {
                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.RightHand, ActionCombineNum.End, 1);
                        HandActionEndMovHandOutScreen(seatIdx, handStyle, HandDirection.LeftHand, ActionCombineNum.End, 1);
                        playerStateData[seatIdx].SetPlayerState(HandActionState.TUIDAO_PAI_END, Time.time, waitTime);
                    }
                    break;

                case HandActionState.TUIDAO_PAI_END:
                    {
                        GameObject rightHand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
                        GameObject leftHand = GetHand(seatIdx, handStyle, HandDirection.LeftHand);
                        leftHand.SetActive(false);
                        rightHand.SetActive(false);
                        mjAssetsMgr.handShadowPlanes[seatIdx].SetActive(false);

                        playerStateData[seatIdx].playerHandActionState = HandActionState.ACTION_END;

                        ProcessHandActionMjOpCmdList(seatIdx);
                    }
                    break;
            }
        }

        void MoveTuiDaoPaiHandShadow(int seatIdx)
        {
            Transform handShadowRef;
            float ang;
            float matUOffset;
            float matVOffset;

            if ((seatIdx == 0 && mjSeatHandPaiLists[seatIdx].Count >= 3) ||
                           playerStateData[seatIdx].handPaiValueList.Count >= 3)
            {
                handShadowRef = playerStateData[seatIdx].handShadowAxis[0];
                ang = 360 - handShadowRef.eulerAngles.y;
                matUOffset = ((handShadowRef.position.x - handShadowPlaneInfos[seatIdx].shadowOrgPos[1].x) / planeSize.x) * handShadowPlaneInfos[seatIdx].tiling[1].x + handShadowPlaneInfos[seatIdx].offset[1].x;
                matVOffset = ((handShadowRef.position.z - handShadowPlaneInfos[seatIdx].shadowOrgPos[1].y) / planeSize.z) * handShadowPlaneInfos[seatIdx].tiling[1].y + handShadowPlaneInfos[seatIdx].offset[1].y;
                handShadowPlaneInfos[seatIdx].planeMat.SetTextureOffset(shaderTexNames[1], new Vector2(matUOffset, matVOffset));
                handShadowPlaneInfos[seatIdx].planeMat.SetFloat(shaderAngNames[1], ang);
            }


            handShadowRef = playerStateData[seatIdx].handShadowAxis[1];
            ang = 360 - handShadowRef.eulerAngles.y;
            matUOffset = ((handShadowRef.position.x - handShadowPlaneInfos[seatIdx].shadowOrgPos[0].x) / planeSize.x) * handShadowPlaneInfos[seatIdx].tiling[0].x + handShadowPlaneInfos[seatIdx].offset[0].x;
            matVOffset = ((handShadowRef.position.z - handShadowPlaneInfos[seatIdx].shadowOrgPos[0].y) / planeSize.z) * handShadowPlaneInfos[seatIdx].tiling[0].y + handShadowPlaneInfos[seatIdx].offset[0].y;
            handShadowPlaneInfos[seatIdx].planeMat.SetTextureOffset(shaderTexNames[0], new Vector2(matUOffset, matVOffset));
            handShadowPlaneInfos[seatIdx].planeMat.SetFloat(shaderAngNames[0], ang);
        }

        float ReadyZhuaHandPai2(int seatIdx, PlayerType handStyle)
        {
            List<GameObject> handPaiList = mjSeatHandPaiLists[seatIdx];
            Vector3 leftHandDstPos;
            Vector3 rightHandDstPos;

            if (handPaiList.Count != 0 && handPaiList[0] != null)
            {
                leftHandDstPos = handPaiList[0].transform.position;
                rightHandDstPos = handPaiList[handPaiList.Count - 1].transform.position;
            }
            else
            {
                leftHandDstPos = mjSeatHandPaiPosLists[seatIdx][0];
                rightHandDstPos = leftHandDstPos;
            }

            float count, size;

            switch (seatIdx)
            {
                case 1:
                    count = playerStateData[seatIdx].handPaiValueList.Count - handPaiList.Count;
                    size = count * GetHandMjSizeByAxis(Axis.X);
                    rightHandDstPos.z += size;
                    break;

                case 2:
                    count = playerStateData[seatIdx].handPaiValueList.Count - handPaiList.Count;
                    size = count * GetHandMjSizeByAxis(Axis.X);
                    rightHandDstPos.x += size;
                    break;

                case 3:
                    count = playerStateData[seatIdx].handPaiValueList.Count - handPaiList.Count;
                    size = count * GetHandMjSizeByAxis(Axis.X);
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

        #region 动作共用功能

        void ProcessHandActionMjOpCmdList(int seatIdx)
        {
            if (mjOpCmdList != null && playerStateData[seatIdx].opCmdNode != null)
            {
                if (playerStateData[seatIdx].opCmdNode.Value.canSelectPaiAfterCmdEnd == true)
                    playerStateData[seatIdx].playerHandActionState = HandActionState.SELECT_PAI_START;

                LinkedListNode<MahjongMachineCmd> tmp = playerStateData[seatIdx].opCmdNode;
                playerStateData[seatIdx].opCmdNode = null;
                mjOpCmdList.RemoveHandActionOpCmd(seatIdx, tmp);
            }
        }

        void ProcessCommonActionMjOpCmdList()
        {
            if (mjOpCmdList != null && mjMachineStateData.opCmdNode != null)
            {
                LinkedListNode<MahjongMachineCmd> tmp = mjMachineStateData.opCmdNode;
                mjMachineStateData.opCmdNode = null;
                mjOpCmdList.RemoveCommonActionOpCmd(tmp);
            }
        }

        /// <summary>
        /// 准备好动作的初始手势
        /// </summary>
        /// <param name="seatIdx">当前动作玩家座位号</param>
        /// <param name="actionName">初始手势名称</param>
        /// <returns></returns>
        float ReadyFirstHand(int seatIdx, PlayerType handStyle, HandDirection handDir, string actionName)
        {
            float waitTime = 0.3f;
            GameObject hand = GetHand(seatIdx, handStyle, handDir);
            Animation anim = GetHandAnimation(seatIdx, handStyle, handDir);

            hand.transform.position = new Vector3(1.215f, 0, 0);
            hand.transform.eulerAngles = new Vector3(0, 0, 0);

            anim.Play(actionName);
            waitTime = GetHandActionWaitTime(seatIdx, handStyle, handDir, actionName);

            return waitTime;
        }

        /// <summary>
        /// 适配手部姿态到当前座位
        /// </summary>
        /// <param name="seatIdx"></param>
        void FitHandPoseForSeat(int seatIdx, PlayerType handStyle, HandDirection handDirection, ActionCombineNum actionCombineNum)
        {
            GameObject hand = GetHand(seatIdx, handStyle, handDirection);
            Vector3 pos = new Vector3(1.215f, 0, 0);

            switch (actionCombineNum)
            {
                case ActionCombineNum.ChaPai:
                    pos = new Vector3(1.215f, 0, 0.334f);
                    break;
            }

            switch (seatIdx)
            {
                case 0:
                    hand.transform.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                    hand.transform.position = new Vector3(-pos.z, pos.y, pos.x);
                    break;

                case 1:
                    hand.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    hand.transform.position = pos;
                    break;

                case 2:
                    hand.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    hand.transform.position = new Vector3(pos.z, pos.y, -pos.x);
                    break;

                case 3:
                    hand.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    hand.transform.position = new Vector3(-pos.x, pos.y, -pos.z);
                    break;
            }
        }

        /// <summary>
        /// 移动手部到目标偏移位置，为下一步精确动作做好准备
        /// </summary>
        /// <param name="seatIdx">当前动作玩家座位号</param>
        /// <param name="pos">目标位置</param>
        /// <param name="actionCombineNum">打牌动作</param>
        float MoveHandToDstOffsetPos(int seatIdx, PlayerType handStyle, HandDirection handDir, Vector3 dstPos, ActionCombineNum actionCombineNum)
        {
            //
            float waitTime = 0.3f;
            List<Vector3> handOffsetList = GetDeskMjHandOffsetList(seatIdx, handStyle, handDir);
            GameObject hand = GetHand(seatIdx, handStyle, handDir);
            Animation anim = GetHandAnimation(seatIdx, handStyle, handDir);
            Vector3 endValue = dstPos + handOffsetList[(int)actionCombineNum];

            switch (actionCombineNum)
            {
                case ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand:
                    {
                        anim.Play("FirstTaiHand2");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, handDir, "FirstTaiHand2");
                    }
                    break;

                case ActionCombineNum.HuPai:
                    {
                        anim.Play("FirstTaiHand1");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, handDir, "FirstTaiHand1");
                    }
                    break;

                case ActionCombineNum.ChaPai:
                    {
                        anim.Play("FirstHand");

                        Dictionary<string, ActionDataInfo> actionDataDict = GetHandActionDataDict(seatIdx, handStyle, handDir);
                        if (actionDataDict != null)
                        {
                            ActionDataInfo info = actionDataDict["MoveHandToDstPos"];
                            waitTime = 1f / info.speed * 0.3f * info.crossFadeNormalTime;
                        }
                    }
                    break;

                case ActionCombineNum.DaPai5:
                    {
                        anim.Play("FirstTaiHand3");
                        waitTime = GetHandActionWaitTime(seatIdx, handStyle, handDir, "FirstTaiHand3");
                    }
                    break;

                default:
                    {
                        Dictionary<string, ActionDataInfo> actionDataDict = GetHandActionDataDict(seatIdx, handStyle, handDir);
                        if (actionDataDict != null)
                        {
                            ActionDataInfo info = actionDataDict["MoveHandToDstPos"];
                            waitTime = 1f / info.speed * 0.3f * info.crossFadeNormalTime;
                        }
                    }

                    break;
            }

            hand.transform.DOMove(endValue, waitTime).SetEase(Ease.OutSine);
            return waitTime;
        }


        /// <summary>
        /// 移动手部到目标方向相对值
        /// </summary>
        /// <param name="seatIdx">当前动作玩家座位号</param>
        float MoveHandToDstDirRelative(int seatIdx, PlayerType handStyle, HandDirection handDir, Vector3 endPos, float moveScale = 12f)
        {
            //
            GameObject hand = GetHand(seatIdx, handStyle, HandDirection.RightHand);
            Vector3 dstDirValue = (endPos - hand.transform.position) / moveScale;

            float waitTime = 1f;
            hand.transform.DOBlendableMoveBy(dstDirValue, waitTime);
            return waitTime;
        }

        /// <summary>
        /// 手部动作结束移出手到屏幕外
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        float HandActionEndMovHandOutScreen(int seatIdx, PlayerType handStyle, HandDirection handDir, ActionCombineNum actionCombinNum = ActionCombineNum.End, int type = 0)
        {
            GameObject hand = GetHand(seatIdx, handStyle, handDir);
            Dictionary<string, ActionDataInfo> actionDataDict = GetHandActionDataDict(seatIdx, handStyle, handDir);
            float waitTime = 0.6f;
            float speed = 1f;

            if (actionDataDict != null)
            {
                ActionDataInfo info = actionDataDict["MoveHandOutScreen"];
                waitTime = 1f / info.speed * 0.6f * info.crossFadeNormalTime;
                speed = info.speed;

            }


            if (type == 0)
            {
                Vector3[] dstpos;

                switch (actionCombinNum)
                {
                    case ActionCombineNum.DaPai5:
                        dstpos = handActionLevelScreenPosSeat2;
                        break;

                    default:
                        dstpos = handActionLevelScreenPosSeat;
                        break;
                }

                hand.transform.DOMove(dstpos[seatIdx], 1f / speed * 0.6f); //.SetEase(Ease.InQuad);
            }
            else
            {
                switch (seatIdx)
                {
                    case 1:
                        hand.transform.DOLocalMoveX(1f, 1f / speed * 0.6f).SetRelative();
                        break;
                    case 3:
                        hand.transform.DOLocalMoveX(-1f, 1f / speed * 0.6f).SetRelative();
                        break;
                    case 2:
                        hand.transform.DOLocalMoveZ(-1f, 1f / speed * 0.6f).SetRelative();
                        break;
                    case 0:
                        hand.transform.DOLocalMoveZ(1f, 1f / speed * 0.6f).SetRelative();
                        break;
                }

            }

            return waitTime;
        }

        #endregion
    }
}