using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ComponentDesgin;

namespace ActionDesgin
{
    public class PengChiGangPaiPos
    {
        public Vector3 pos;
        public int layouyDirSeat;
    }

  
    /// <summary>
    ///  碰吃杠牌动作
    /// </summary>
    public class PengChiGangPaiAction : BaseHandAction
    {
        public static PengChiGangPaiAction Instance { get; } = new PengChiGangPaiAction();

        public override void Install()
        {
            mjMachineUpdater.Reg("PengChiGang", ActionPengChiGangPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("PengChiGang");
        }


        #region  碰,吃,杠牌动作
        /// <summary>
        /// 碰吃杠牌
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="handStyle"></param>
        /// <param name="isMoveHand"></param>
        /// <param name="moveHandDist"></param>
        /// <param name="faceValues"></param>
        /// <param name="actionCombineNum"></param>
        public void PengChiGangPai(
            int seatIdx, PlayerType handStyle, bool isMoveHand, float moveHandDist,
            MahjongFaceValue[] faceValues, PengChiGangPaiType pcgType,
            int targetSeatIdx, Vector3Int targetMjIdx,
            EffectFengRainEtcType fengRainEtcEffect,
            ActionCombineNum actionCombineNum,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].state != StateDataGroup.END)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            int idx = 0;
            if (pcgType == PengChiGangPaiType.PENG)
                idx = Random.Range(0, 3);
            else
                idx = Random.Range(0, 4);

            PengChiGangPaiStateData stateData = playerStateData[seatIdx].GetComponent<PengChiGangPaiStateData>();

            stateData.SetPengChiGangPaiData(
                handStyle,
                isMoveHand, moveHandDist,
                pcgType, faceValues, idx,
                targetSeatIdx, targetMjIdx,
                fengRainEtcEffect,
                actionCombineNum, opCmdNode);

            playerStateData[seatIdx].SetState(PengChiGangPaiStateData.PENG_CHI_GANG_PAI_START, Time.time, -1);
        }


        /// <summary>
        /// 碰,吃,杠牌动作
        /// </summary>
        public void ActionPengChiGangPai()
        {
            for (int i = 0; i < Fit.playerCount; i++)
            {
                if (fit.isCanUseSeatPlayer[i])
                    ActionPengChiGangPai(i);
            }
        }


        void ActionPengChiGangPai(int seatIdx)
        {
            if (playerStateData[seatIdx].state < PengChiGangPaiStateData.PENG_CHI_GANG_PAI_START ||
               playerStateData[seatIdx].state > PengChiGangPaiStateData.PENG_CHI_GANG_PAI_END ||
               Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
            {
                return;
            }

            PengChiGangPaiStateData stateData = playerStateData[seatIdx].GetComponent<PengChiGangPaiStateData>();

            PlayerType handStyle = stateData.handStyle;
            bool isMoveHand = stateData.pcgPaiIsMoveHand;
            float moveHandDist = stateData.pcgPaiMoveHandDist;
            PengChiGangPaiType pcgPaiType = stateData.pcgPaiType;
            int targetSeatIdx = stateData.pcgPaiTargetSeatIdx;
            float waitTime;
            float spacing = 0.0002f;

            HandDirection dir = preSettingHelper.pengPaiHandDirSeat[seatIdx];
            Animation anim = hands.GetHandAnimation(seatIdx, handStyle, dir);

            switch (playerStateData[seatIdx].state)
            {
                case PengChiGangPaiStateData.PENG_CHI_GANG_PAI_START:
                    {
                        GameObject hand = hands.GetHand(seatIdx, handStyle, dir);
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

                        audio.PlaySpeakAudio(handStyle, audioIdx);

                        GameObject textEffect = pool.PopGameObject();
                        textEffect.transform.localPosition = preSettingHelper.pcgthEffectTextPosSeat[seatIdx];
                        textEffect.SetActive(true);
                        textEffect.GetComponent<ParticleSystem>().Play();

                        EffectFengRainEtcType effects = stateData.fengRainEtcEffect;
                        EffectFengRainEtcType effect = effects;
                        Vector3 effectPos = Vector3.zero;

                        for (int i = 0; i < 2; i++)
                        {
                            effect = desk.GetEffectFengRainEtcType(effects, i);

                            switch (effect)
                            {
                                case EffectFengRainEtcType.EFFECT_RAIN:
                                    audio.PlayEffectAudio(AudioIdx.AUDIO_EFFECT_RAINY);
                                    pool = mjAssetsMgr.rainEffectPool;
                                    effectPos = preSettingHelper.rainEffectPos[seatIdx];
                                    break;

                                case EffectFengRainEtcType.EFFECT_FENG:
                                    audio.PlayEffectAudio(AudioIdx.AUDIO_EFFECT_WINDY);
                                    pool = mjAssetsMgr.longjuanfengEffectPool;
                                    effectPos = preSettingHelper.fengEffectPos[seatIdx];
                                    break;

                                case EffectFengRainEtcType.EFFECT_NONE:
                                    continue;
                            }

   
                            GameObject eff = pool.PopGameObject();
                            eff.transform.localPosition = effectPos;
                            eff.SetActive(true);
                            eff.GetComponent<ParticleSystem>().Play();
                        }


                        //
                        playerStateData[seatIdx].SetState(PengChiGangPaiStateData.PENG_CHI_GANG_PAI_READY_FIRST_HAND, Time.time, waitTime);
                    }
                    break;

                case PengChiGangPaiStateData.PENG_CHI_GANG_PAI_READY_FIRST_HAND:
                    {
                        FitHandPoseForSeat(seatIdx, handStyle, HandDirection.RightHand, stateData.actionCombineNum);

                        if (pcgPaiType == PengChiGangPaiType.BU_GANG)
                        {
                            stateData.pcgMjIdx = -1;
                            MjPaiData mjData;
                            for (int i = 0; i < desk.deskPengPaiMjList[seatIdx].Count; i++)
                            {
                                mjData = desk.deskPengPaiMjList[seatIdx][i][0].GetComponent<MjPaiData>();

                                if (mjData.mjFaceValue != stateData.pcgPaiMjfaceValues[0])
                                    continue;

                                stateData.pcgMjIdx = i;
                                break;
                            }

                            if (stateData.pcgMjIdx == -1)
                            {
                                playerStateData[seatIdx].SetState(PengChiGangPaiStateData.PENG_CHI_GANG_PAI_END, Time.time, -1);
                                break;
                            }

                            stateData.pcgDstStartPos =
                                desk.deskPengPaiMjPosInfoList[seatIdx][stateData.pcgMjIdx];
                        }
                        else
                        {
                            stateData.pcgDstStartPos =
                               NextPengChiGangPaiPos(seatIdx, pcgPaiType, stateData.pcgPaiLayoutIdx, spacing);
                        }

                        stateData.pcgOrgStartPos = stateData.pcgDstStartPos;

                        if (isMoveHand)
                        {
                            stateData.pcgOrgStartPos =
                               OffsetPengChiGangPaiStartPosByMoveDist(
                                    seatIdx, stateData.pcgDstStartPos, moveHandDist);
                        }

                        waitTime = MoveHandToDstOffsetPos(
                            seatIdx, handStyle, dir,
                            stateData.pcgOrgStartPos[1],
                            stateData.actionCombineNum);

                        playerStateData[seatIdx].SetState(PengChiGangPaiStateData.PENG_CHI_GANG_PAI_MOVE_HAND_TO_DST_POS, Time.time, waitTime);
                    }
                    break;

                case PengChiGangPaiStateData.PENG_CHI_GANG_PAI_MOVE_HAND_TO_DST_POS:
                    {
                        if (targetSeatIdx >= 0 && targetSeatIdx < Fit.playerCount)
                        {
                            Vector3Int targetMjIdx = stateData.pcgPaiTargetMjIdx;

                            if (targetMjIdx.x == -1 || targetMjIdx.y == -1 || targetMjIdx.z == -1)
                            {
                                targetMjIdx = desk.GetCurtDeskMjPosIdx(targetSeatIdx);
                            }

                            int key = desk.GetDeskDaPaiMjDictKey(targetMjIdx.x, targetMjIdx.y, targetMjIdx.z);

                            if (desk.deskDaPaiMjDicts[targetSeatIdx].ContainsKey(key))
                            {
                                GameObject targetmj = desk.deskDaPaiMjDicts[targetSeatIdx][key];
                                desk.deskDaPaiMjDicts[targetSeatIdx].Remove(key);
                                desk.RemoveMjFromDeskGlobalMjPaiSetDict(targetmj.GetComponent<MjPaiData>().mjFaceValue, targetmj);
                                mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(targetmj);

                                desk.PrevDeskMjPos(targetSeatIdx);

                                if (targetmj == desk.lastDaPaiMj)
                                    desk.lastDaPaiMj = null;
                            }
                        }

                        if (pcgPaiType == PengChiGangPaiType.BU_GANG)
                        {
                            anim.Play("PengPai");
                            waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, dir, "PengPai");
                            playerStateData[seatIdx].SetState(PengChiGangPaiStateData.PENG_CHI_GANG_PAI_PCG_PAI, Time.time, waitTime);
                            break;
                        }

                        PengChiGangPaiPos[] pcgPos = GetPengChiGangPaiPosList(
                           seatIdx, pcgPaiType,
                           stateData.pcgOrgStartPos[0],
                           stateData.pcgPaiLayoutIdx,
                           spacing);

                        stateData.pcgMjList = CreatePengChiGangPaiList(
                            pcgPos,
                            stateData.pcgPaiMjfaceValues,
                            pcgPaiType);

                        for (int i = 0; i < stateData.pcgMjList.Length; i++)
                        {
                            stateData.pcgMjList[i].transform.SetParent(desk.mjtableTransform, true);
                        }

                        GameObject pengPaiParticle = mjAssetsMgr.pcgPaiParticlePool.PopGameObject();
                        pengPaiParticle.SetActive(true);

                        Vector3 eulerAngles = pengPaiParticle.transform.eulerAngles;
                        pengPaiParticle.transform.eulerAngles = new Vector3(eulerAngles.x, fit.pcgParticleEulerAnglesY[seatIdx], eulerAngles.z);

                        Vector3 pos = stateData.pcgOrgStartPos[1];
                        pengPaiParticle.transform.position = new Vector3(pos.x, pos.y + fit.GetDeskPengChiGangMjSizeByAxis(Axis.Y) / 3.0f, pos.z);
                        pengPaiParticle.GetComponent<ParticleSystem>().Play();

                        anim.Play("PengPai");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, dir, "PengPai");
                        playerStateData[seatIdx].SetState(PengChiGangPaiStateData.PENG_CHI_GANG_PAI_PCG_PAI, Time.time, waitTime);
                    }
                    break;


                case PengChiGangPaiStateData.PENG_CHI_GANG_PAI_PCG_PAI:
                    {
                        Vector3 distOffset = stateData.pcgDstStartPos[1] - stateData.pcgOrgStartPos[1];
                        GameObject hand = hands.GetHand(seatIdx, handStyle, dir);

                        if (pcgPaiType == PengChiGangPaiType.BU_GANG)
                        {
                            waitTime = 0.04f;
                            hand.transform.DOMove(distOffset, waitTime).SetRelative();

                            int mjIdx = stateData.pcgMjIdx;

                            GameObject[] oldMjList = desk.deskPengPaiMjList[seatIdx][mjIdx];
                            Vector3[] oldPosInfo = desk.deskPengPaiMjPosInfoList[seatIdx][mjIdx];

                            for (int i = 0; i < oldMjList.Length; i++)
                            {
                                desk.RemoveMjFromDeskGlobalMjPaiSetDict(oldMjList[i].GetComponent<MjPaiData>().mjFaceValue, oldMjList[i]);
                                mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(oldMjList[i]);
                            }

                            desk.deskPengPaiMjList[seatIdx].RemoveAt(mjIdx);
                            desk.deskPengPaiMjPosInfoList[seatIdx].RemoveAt(mjIdx);

                            int paiLayoutIdx = Random.Range(0, 4);
                            Vector3 outEndValue;

                            Vector3[] newPosInfo = GetPengChiGangPaiPos(
                                seatIdx, PengChiGangPaiType.GANG, stateData.pcgDstStartPos[0], paiLayoutIdx, out outEndValue, spacing);

                            PengChiGangPaiPos[] newMjPaiPos = GetPengChiGangPaiPosList(
                                          seatIdx, PengChiGangPaiType.GANG,
                                          newPosInfo[0],
                                          paiLayoutIdx,
                                          spacing);

                            MahjongFaceValue value = stateData.pcgPaiMjfaceValues[0];

                            GameObject[] newMjList = CreatePengChiGangPaiList(
                                newMjPaiPos,
                                new MahjongFaceValue[] { value, value, value, value },
                                PengChiGangPaiType.GANG);

                            for (int i = 0; i < newMjList.Length; i++)
                            {
                                newMjList[i].transform.SetParent(desk.mjtableTransform, true);
                                desk.AppendMjToDeskGlobalMjPaiSetDict(newMjList[i].GetComponent<MjPaiData>().mjFaceValue, newMjList[i]);
                            }

                            desk.deskGangPaiMjList[seatIdx].Add(newMjList);

                            float dist = Mathf.Abs(newPosInfo[2].x - oldPosInfo[2].x);

                            Vector3[] offsetPos =
                                OffsetPengChiGangPaiStartPosByMoveDist(
                                    seatIdx, stateData.pcgDstStartPos, dist);

                            distOffset = offsetPos[1] - stateData.pcgDstStartPos[1];

                            preSettingHelper.pengPaiCurtPosSeat[seatIdx] += distOffset;

                            GameObject[] mjs;
                            List<GameObject[]> goList = null;

                            for (int m = 0; m < 3; m++)
                            {
                                switch (m)
                                {
                                    case 0: goList = desk.deskPengPaiMjList[seatIdx]; break;
                                    case 1: goList = desk.deskGangPaiMjList[seatIdx]; break;
                                    case 2: goList = desk.deskChiPaiMjList[seatIdx]; break;
                                }

                                for (int i = 0; i < goList.Count; i++)
                                {
                                    mjs = goList[i];
                                    if (IsAtBackPos(seatIdx, mjs[0].transform.position, newPosInfo[1]))
                                    {
                                        if (m == 0)
                                        {
                                            Vector3[] pos = desk.deskPengPaiMjPosInfoList[seatIdx][i];
                                            pos[0] += distOffset;
                                            pos[1] += distOffset;
                                        }

                                        for (int j = 0; j < mjs.Length; j++)
                                            mjs[j].transform.DOMove(distOffset, waitTime).SetRelative();
                                    }
                                }
                            }


                            playerStateData[seatIdx].SetState(PengChiGangPaiStateData.PENG_CHI_GANG_PAI_MOVE_PAI, Time.time, waitTime);
                            break;
                        }

                        waitTime = 0.3f;
                        hand.transform.DOMove(distOffset, waitTime).SetRelative();

                        for (int i = 0; i < stateData.pcgMjList.Length; i++)
                        {
                            stateData.pcgMjList[i].transform.DOMove(distOffset, waitTime).SetRelative();
                        }

                        playerStateData[seatIdx].SetState(PengChiGangPaiStateData.PENG_CHI_GANG_PAI_MOVE_PAI, Time.time, waitTime);
                    }
                    break;


                case PengChiGangPaiStateData.PENG_CHI_GANG_PAI_MOVE_PAI:
                    {
                        if (pcgPaiType != PengChiGangPaiType.BU_GANG)
                        {
                           AddDeskPaiToPengChiGangPaiList(
                                seatIdx, pcgPaiType,
                                stateData.pcgMjList,
                                stateData.pcgDstStartPos);

                            GameObject mj;
                            for (int i = 0; i < stateData.pcgMjList.Length; i++)
                            {
                                mj = stateData.pcgMjList[i];
                                desk.AppendMjToDeskGlobalMjPaiSetDict(mj.GetComponent<MjPaiData>().mjFaceValue, mj);
                            }
                        }

                        anim.Play("PengPaiEndTaiHand");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, dir, "PengPaiEndTaiHand");
                        playerStateData[seatIdx].SetState(PengChiGangPaiStateData.PENG_CHI_GANG_PAI_TAIHAND, Time.time, waitTime);
                    }
                    break;

                case PengChiGangPaiStateData.PENG_CHI_GANG_PAI_TAIHAND:
                    {
                        waitTime = HandActionEndMovHandOutScreen(seatIdx, handStyle, dir);
                        playerStateData[seatIdx].SetState(PengChiGangPaiStateData.PENG_CHI_GANG_PAI_END, Time.time, waitTime);

                    }
                    break;

                case PengChiGangPaiStateData.PENG_CHI_GANG_PAI_END:
                    {
                        GameObject hand = hands.GetHand(seatIdx, handStyle, dir);
                        hand.SetActive(false);
                        playerStateData[seatIdx].state = StateDataGroup.END;

                        ProcessHandActionmjCmdMgr(seatIdx, stateData);
                    }
                    break;
            }

        }

        #endregion


        #region 生成碰，吃，杠牌的起始位置
       
        /// <summary>
        /// 获取碰吃杠牌的摆放布局位置列表（按照给定牌摆放起始位置paiStartPos）
        /// </summary>
        /// <param name="seatIdx">座位号</param>
        /// <param name="pcgType">碰吃杠类型</param>
        /// <param name="paiStartPos">给定的牌摆放起始位置</param>
        /// <param name="paiLayoutIdx">摆放布局牌型</param>
        /// <param name="mjSpacing"></param>
        /// <returns></returns> 
        public PengChiGangPaiPos[] GetPengChiGangPaiPosList(int seatIdx, PengChiGangPaiType pcgType, Vector3 paiStartPos, int paiLayoutIdx, float mjSpacing = 0.0001f)
        {
            HandDirection dir = preSettingHelper.pengPaiHandDirSeat[seatIdx];
            return GetPengChiGangPaiPosList(seatIdx, dir, pcgType, paiStartPos, paiLayoutIdx, mjSpacing);
        }

        /// <summary>
        /// 获取碰吃杠牌的位置列表
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="dir"></param>
        /// <param name="pcgType"></param>
        /// <param name="paiLayoutIdx"></param>
        /// <param name="mjSpacing"></param>
        /// <returns></returns>
        PengChiGangPaiPos[] GetPengChiGangPaiPosList(
            int seatIdx, HandDirection dir,
            PengChiGangPaiType pcgType, Vector3 paiStartPos, int paiLayoutIdx, float mjSpacing = 0.0001f)
        {
            float mjSizeX = fit.GetDeskPengChiGangMjSizeByAxis(Axis.X);
            float mjSizeY = fit.GetDeskPengChiGangMjSizeByAxis(Axis.Y);
            float handSign = 1;
            float fbSign = 1;

            PengChiGangPaiPos[] mjPos = null;
            Vector3 pevPos = paiStartPos;

            if (pcgType == PengChiGangPaiType.PENG || pcgType == PengChiGangPaiType.CHI)
                mjPos = new PengChiGangPaiPos[3] { new PengChiGangPaiPos(), new PengChiGangPaiPos(), new PengChiGangPaiPos() };
            else
                mjPos = new PengChiGangPaiPos[4] { new PengChiGangPaiPos(), new PengChiGangPaiPos(), new PengChiGangPaiPos(), new PengChiGangPaiPos() };

            switch (seatIdx)
            {
                case 0:
                    if (dir == HandDirection.LeftHand) { handSign = -1; }
                    else { handSign = 1; }
                    fbSign = -1;
                    break;

                case 1:
                    if (dir == HandDirection.LeftHand) { handSign = 1; }
                    else { handSign = -1; }
                    fbSign = -1;
                    break;

                case 2:
                    if (dir == HandDirection.LeftHand) { handSign = 1; }
                    else { handSign = -1; }
                    fbSign = 1;
                    break;

                case 3:
                    if (dir == HandDirection.LeftHand) { handSign = -1; }
                    else { handSign = 1; }
                    fbSign = 1;
                    break;
            }


            switch (seatIdx)
            {
                case 0:
                case 2:

                    if (pcgType == PengChiGangPaiType.CHI)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            mjPos[i].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                            mjPos[i].layouyDirSeat = seatIdx;
                            pevPos.x += handSign * (mjSizeX + mjSpacing);
                        }
                    }
                    else if (pcgType == PengChiGangPaiType.PENG)
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        mjPos[i].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                        mjPos[i].layouyDirSeat = seatIdx;
                                        pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    }
                                }
                                break;

                            case 1:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[0].layouyDirSeat = seatIdx;

                                    pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    mjPos[1].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2);
                                    mjPos[1].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    pevPos.x += handSign * (mjSizeY + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[2].layouyDirSeat = seatIdx;
                                }
                                break;


                            case 2:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2);
                                    mjPos[0].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    mjPos[1].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2);
                                    mjPos[1].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    pevPos.x += handSign * (mjSizeY + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[2].layouyDirSeat = seatIdx;
                                }
                                break;
                        }

                    }
                    else
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[0].layouyDirSeat = seatIdx;

                                    pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    mjPos[1].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2);
                                    mjPos[1].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    mjPos[2].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2);
                                    mjPos[2].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    pevPos.x += handSign * (mjSizeY + mjSpacing);
                                    mjPos[3].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[3].layouyDirSeat = seatIdx;
                                }
                                break;

                            case 1:
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        mjPos[i].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                        mjPos[i].layouyDirSeat = seatIdx;
                                        pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    }
                                }
                                break;

                            case 2:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2);
                                    mjPos[0].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    mjPos[1].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2);
                                    mjPos[1].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    pevPos.x += handSign * (mjSizeY + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[2].layouyDirSeat = seatIdx;

                                    pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    mjPos[3].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[3].layouyDirSeat = seatIdx;

                                }
                                break;


                            case 3:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[0].layouyDirSeat = seatIdx;

                                    pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    mjPos[1].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[1].layouyDirSeat = seatIdx;

                                    pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2);
                                    mjPos[2].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    mjPos[3].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2);
                                    mjPos[3].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                }
                                break;

                        }
                    }

                    break;


                case 1:
                case 3:

                    if (pcgType == PengChiGangPaiType.CHI)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            mjPos[i].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                            mjPos[i].layouyDirSeat = seatIdx;
                            pevPos.z += handSign * (mjSizeX + mjSpacing);
                        }

                    }
                    else if (pcgType == PengChiGangPaiType.PENG)
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                for (int i = 0; i < 3; i++)
                                {
                                    mjPos[i].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[i].layouyDirSeat = seatIdx;
                                    pevPos.z += handSign * (mjSizeX + mjSpacing);
                                }
                                break;

                            case 1:
                                mjPos[0].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                mjPos[0].layouyDirSeat = seatIdx;

                                pevPos.z += handSign * (mjSizeX + mjSpacing);
                                mjPos[1].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                mjPos[1].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                pevPos.z += handSign * (mjSizeY + mjSpacing);
                                mjPos[2].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                mjPos[2].layouyDirSeat = seatIdx;
                                break;


                            case 2:
                                mjPos[0].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                mjPos[0].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                mjPos[1].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                mjPos[1].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                pevPos.z += handSign * (mjSizeY + mjSpacing);
                                mjPos[2].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                mjPos[2].layouyDirSeat = seatIdx;
                                break;

                        }

                    }
                    else
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[0].layouyDirSeat = seatIdx;

                                    pevPos.z += handSign * (mjSizeX + mjSpacing);
                                    mjPos[1].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[1].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    mjPos[2].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[2].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    pevPos.z += handSign * (mjSizeY + mjSpacing);
                                    mjPos[3].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[3].layouyDirSeat = seatIdx;
                                }
                                break;

                            case 1:
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        mjPos[i].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                        mjPos[i].layouyDirSeat = seatIdx;
                                        pevPos.z += handSign * (mjSizeX + mjSpacing);
                                    }
                                }
                                break;

                            case 2:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[0].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    mjPos[1].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[1].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    pevPos.z += handSign * (mjSizeY + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[2].layouyDirSeat = seatIdx;

                                    pevPos.z += handSign * (mjSizeX + mjSpacing);
                                    mjPos[3].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[3].layouyDirSeat = seatIdx;
                                }
                                break;

                            case 3:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[0].layouyDirSeat = seatIdx;

                                    pevPos.z += handSign * (mjSizeX + mjSpacing);
                                    mjPos[1].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[1].layouyDirSeat = seatIdx;

                                    pevPos.z += handSign * (mjSizeX + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[2].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);

                                    mjPos[3].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[3].layouyDirSeat = fit.GetNextSeatIdx(seatIdx);


                                }
                                break;

                        }
                    }
                    break;
            }

            return mjPos;
        }

        /// <summary>
        /// 获取杠牌麻将牌当前牌号的牌的正反摆放方式
        /// </summary>
        /// <param name="paiLayoutIdx"></param>
        /// <param name="paiIdx"></param>
        /// <returns></returns>
        MahjongFaceSide GetGangPaiFaceSide(int paiLayoutIdx, int paiIdx)
        {
            int randIdx = Random.Range(0, 4);
            if (paiIdx == randIdx)
                return MahjongFaceSide.Front;
            return MahjongFaceSide.Back;
        }

        Vector3 GetNextPengChiGangPaiPos(int seatIdx)
        {
            HandDirection dir = preSettingHelper.pengPaiHandDirSeat[seatIdx];
            Vector3 curtPos = preSettingHelper.pengPaiCurtPosSeat[seatIdx];
            float sign;

            switch (seatIdx)
            {
                case 0:
                    if (dir == HandDirection.LeftHand) { sign = -1f; }
                    else { sign = 1f; }
                    curtPos.x += fit.deskPengPaiSpacing * sign;
                    break;

                case 1:
                    if (dir == HandDirection.LeftHand) { sign = 1f; }
                    else { sign = -1f; }
                    curtPos.z += fit.deskPengPaiSpacing * sign;
                    break;

                case 2:
                    if (dir == HandDirection.LeftHand) { sign = 1f; }
                    else { sign = -1f; }
                    curtPos.x += fit.deskPengPaiSpacing * sign; break;

                case 3:
                    if (dir == HandDirection.LeftHand) { sign = -1f; }
                    else { sign = 1f; }
                    curtPos.z += fit.deskPengPaiSpacing * sign;
                    break;
            }

            return curtPos;
        }

        /// <summary>
        /// 获取碰吃杠牌的起始位置[0],中心位置[1], 长度[2]
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="pcgType"></param>
        /// <param name="startPos">起始位置</param>
        /// <param name="paiLayoutIdx"></param>
        /// <param name="mjSpacing"></param>
        /// <returns></returns>
        public Vector3[] GetPengChiGangPaiPos(int seatIdx, PengChiGangPaiType pcgType, Vector3 startPos, int paiLayoutIdx, out Vector3 outEndPos, float mjSpacing = 0.0001f)
        {
            HandDirection dir = preSettingHelper.pengPaiHandDirSeat[seatIdx];

            float mjSizeX = fit.GetDeskPengChiGangMjSizeByAxis(Axis.X);
            float mjSizeY = fit.GetDeskPengChiGangMjSizeByAxis(Axis.Y);
            float handSign = 1;

            Vector3 endPos = startPos;
            Vector3 centerPos = new Vector3(0, 0, 0);
            float len = 0;

            switch (seatIdx)
            {
                case 0:
                    if (dir == HandDirection.LeftHand) { handSign = -1; }
                    else { handSign = 1; }
                    break;

                case 1:
                    if (dir == HandDirection.LeftHand) { handSign = 1; }
                    else { handSign = -1; }
                    break;

                case 2:
                    if (dir == HandDirection.LeftHand) { handSign = 1; }
                    else { handSign = -1; }
                    break;

                case 3:
                    if (dir == HandDirection.LeftHand) { handSign = -1; }
                    else { handSign = 1; }
                    break;
            }


            switch (seatIdx)
            {
                case 0:
                case 2:

                    if (pcgType == PengChiGangPaiType.CHI)
                    {
                        endPos.x += 3 * handSign * (mjSizeX + mjSpacing);
                        centerPos = endPos;
                        centerPos.x = (startPos.x + endPos.x) / 2f;
                        len = endPos.x - startPos.x;
                    }
                    else if (pcgType == PengChiGangPaiType.PENG)
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    endPos.x += 3 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;

                            case 1:
                                {
                                    endPos.x += handSign * (mjSizeX + mjSpacing) + handSign * (mjSizeY + mjSpacing) + handSign * mjSizeX;
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;


                            case 2:
                                {
                                    endPos.x += handSign * (mjSizeY + mjSpacing) + handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    endPos.x += handSign * (mjSizeX + mjSpacing) + handSign * (mjSizeY + mjSpacing) + handSign * mjSizeX;
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;

                            case 1:
                                {
                                    endPos.x += 4 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;

                            case 2:
                            case 3:
                                {
                                    endPos.x += handSign * (mjSizeY + mjSpacing) + 2 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;
                        }
                    }

                    break;


                case 1:
                case 3:

                    if (pcgType == PengChiGangPaiType.CHI)
                    {
                        endPos.z += 3 * handSign * (mjSizeX + mjSpacing);
                        centerPos = endPos;
                        centerPos.z = (startPos.z + endPos.z) / 2f;
                        len = endPos.z - startPos.z;
                    }
                    else if (pcgType == PengChiGangPaiType.PENG)
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    endPos.z += 3 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;

                            case 1:
                                {
                                    endPos.z += handSign * (mjSizeX + mjSpacing) + handSign * (mjSizeY + mjSpacing) + handSign * mjSizeX;
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;


                            case 2:
                                {
                                    endPos.z += handSign * (mjSizeY + mjSpacing) + handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    endPos.z += handSign * (mjSizeX + mjSpacing) + handSign * (mjSizeY + mjSpacing) + handSign * mjSizeX;
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;

                            case 1:
                                {
                                    endPos.z += 4 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;

                            case 2:
                            case 3:
                                {
                                    endPos.z += handSign * (mjSizeY + mjSpacing) + 2 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;

                        }
                    }
                    break;
            }

            outEndPos = endPos;
            return new Vector3[] { startPos, centerPos, new Vector3(len, len, len) };
        }


        /// <summary>
        /// 下一个碰吃杠牌的起始位置[0]和中心位置[1], 长度[2]
        /// </summary>
        /// <param name="seatIdx">座位号</param>
        /// <param name="pcgType">碰吃杠类型</param>
        /// <param name="paiLayoutIdx">摆放牌型</param>
        /// <param name="mjSpacing">每个麻将之间的间距</param>
        /// <returns></returns>
        public Vector3[] NextPengChiGangPaiPos(int seatIdx, PengChiGangPaiType pcgType, int paiLayoutIdx, float mjSpacing = 0.0001f)
        {
            Vector3 startPos = GetNextPengChiGangPaiPos(seatIdx);
            Vector3 outEndValue;

            Vector3[] posinfo = GetPengChiGangPaiPos(seatIdx, pcgType, startPos, paiLayoutIdx, out outEndValue, mjSpacing);
            preSettingHelper.pengPaiCurtPosSeat[seatIdx] = outEndValue;
            return posinfo;
        }

        /// <summary>
        /// 偏移碰吃杠牌的起始位置到指定起始距离(moveHandDist)
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="startPos"></param>
        /// <param name="moveHandDist">指定起始距离</param>
        /// <returns></returns>
        public Vector3[] OffsetPengChiGangPaiStartPosByMoveDist(int seatIdx, Vector3[] startPos, float moveHandDist = 0)
        {
            HandDirection dir = preSettingHelper.pengPaiHandDirSeat[seatIdx];
            Vector3 centerPos = startPos[1];
            float sign = 1;
            Vector3 pevPos = startPos[0];
            float moveDist = 0;

            switch (seatIdx)
            {
                case 0:
                    if (dir == HandDirection.LeftHand) { sign = -1; }
                    else { sign = 1; }
                    break;

                case 1:
                    if (dir == HandDirection.LeftHand) { sign = 1; }
                    else { sign = -1; }
                    break;

                case 2:
                    if (dir == HandDirection.LeftHand) { sign = 1; }
                    else { sign = -1; }
                    break;

                case 3:
                    if (dir == HandDirection.LeftHand) { sign = -1; }
                    else { sign = 1; }
                    break;
            }

            switch (seatIdx)
            {
                case 0:
                case 2:
                    moveDist = sign * moveHandDist;
                    pevPos.x += moveDist;
                    centerPos.x += moveDist;
                    break;

                case 1:
                case 3:
                    moveDist = sign * moveHandDist;
                    pevPos.z += moveDist;
                    centerPos.z += moveDist;
                    break;
            }

            return new Vector3[] { pevPos, centerPos };
        }

        /// <summary>
        /// 根据给定碰吃杠牌布局位置列表(pcgPaiPos)，生成碰吃杠牌实体列表
        /// </summary>
        /// <param name="pcgPaiPos">给定碰吃杠牌布局位置列表</param>
        /// <param name="mjFaceValues"></param>
        /// <param name="paiType"></param>
        /// <returns></returns>
        public GameObject[] CreatePengChiGangPaiList(
            PengChiGangPaiPos[] pcgPaiPos, MahjongFaceValue[] mjFaceValues,
            PengChiGangPaiType paiType)
        {
            GameObject mj;
            List<GameObject> mjList = new List<GameObject>();
            bool isBackSide = false;

            if (paiType == PengChiGangPaiType.AN_GANG)
                isBackSide = true;

            int randIdx = Random.Range(0, 9);
            MahjongFaceValue mjFaceValue;

            for (int i = 0; i < pcgPaiPos.Length; i++)
            {
                if (isBackSide)
                {
                    mjFaceValue = MahjongFaceValue.MJ_ZFB_FACAI;
                }
                else
                {
                    mjFaceValue = mjFaceValues[i];
                }

                mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValue);
                mj.layer = mjMachine.defaultLayer;

                if (paiType == PengChiGangPaiType.GANG)
                {
                    if (i == randIdx || randIdx >= 4)
                        isBackSide = false;
                    else
                        isBackSide = true;
                }

                fit.FitSeatDeskPengChiGangMj(pcgPaiPos[i].layouyDirSeat, mj, true, 0.91f, isBackSide);
                mj.transform.position = pcgPaiPos[i].pos;
                mjList.Add(mj);
            }

            return mjList.ToArray();
        }


        public bool IsAtBackPos(int seatIdx, Vector3 pos, Vector3 orgPos)
        {
            HandDirection dir = preSettingHelper.pengPaiHandDirSeat[seatIdx];

            switch (seatIdx)
            {
                case 0:
                    if (dir == HandDirection.LeftHand)
                    {
                        if (pos.x < orgPos.x)
                            return true;
                    }
                    else
                    {
                        if (pos.x > orgPos.x)
                            return true;
                    }
                    break;

                case 1:
                    if (dir == HandDirection.LeftHand)
                    {
                        if (pos.z > orgPos.z)
                            return true;
                    }

                    else
                    {
                        if (pos.z < orgPos.z)
                            return true;
                    }
                    break;

                case 2:
                    if (dir == HandDirection.LeftHand)
                    {
                        if (pos.x > orgPos.x)
                            return true;
                    }

                    else
                    {
                        if (pos.x < orgPos.x)
                            return true;
                    }
                    break;

                case 3:
                    if (dir == HandDirection.LeftHand)
                    {
                        if (pos.z < orgPos.z)
                            return true;
                    }

                    else
                    {
                        if (pos.z > orgPos.z)
                            return true;
                    }

                    break;
            }

            return false;
        }


        /// <summary>
        /// 增加桌面牌到碰吃杠牌列表
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="pcgType"></param>
        /// <param name="paiList">桌面牌列表</param>
        public void AddDeskPaiToPengChiGangPaiList(int seatIdx, PengChiGangPaiType pcgType, GameObject[] paiList, Vector3[] paiPosInfo)
        {
            switch (pcgType)
            {
                case PengChiGangPaiType.PENG:
                    desk.deskPengPaiMjList[seatIdx].Add(paiList);
                    desk.deskPengPaiMjPosInfoList[seatIdx].Add(paiPosInfo);
                    break;

                case PengChiGangPaiType.CHI:
                    desk.deskChiPaiMjList[seatIdx].Add(paiList);
                    break;

                case PengChiGangPaiType.GANG:
                    desk.deskGangPaiMjList[seatIdx].Add(paiList);
                    break;
            }
        }

        #endregion

    }
}
