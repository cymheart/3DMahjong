using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ComponentDesgin;
using CmdDesgin;

namespace ActionDesgin
{
    /// <summary>
    /// 选择打牌动作
    /// </summary>
    public class SelectDaPaiAction : BaseHandAction
    {
        private static SelectDaPaiAction instance = null;
        public static SelectDaPaiAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new SelectDaPaiAction();
                return instance;
            }
        }

        UIHuPaiHandPaiIdxTipsArrow uiHuPaiTipsArrow;
        UIHuPaiTips uiHuPaiTips;
       

        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);
            uiHuPaiTipsArrow = mjMachine.GetComponent<UIHuPaiHandPaiIdxTipsArrow>();
            uiHuPaiTips = mjMachine.GetComponent<UIHuPaiTips>();
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("SelectDaPai", ActionSelectDaPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("SelectDaPai");
        }

        /// <summary>
        /// 选择打出牌
        /// </summary>
        public void SelectDaPai(
            int[] huPaiInHandPaiIdxs, List<HuPaiTipsInfo[]> huPaiInfosInHandPai,
            int[] huPaiInMoPaiIdxs, List<HuPaiTipsInfo[]> huPaiInfosInMoPai,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(0);

            if (playerStateData[0].state != StateDataGroup.END)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            if ((huPaiInHandPaiIdxs == null || huPaiInHandPaiIdxs.Length == 0) &&
                (huPaiInMoPaiIdxs == null || huPaiInMoPaiIdxs.Length == 0))
            {
                byte[] cards = new byte[34];

                desk.CreateHandPaiCardList(ref cards);
                mjHuTingCheck.CheckTing(cards, 100);

                if (huPaiInfosInHandPai == null)
                    huPaiInfosInHandPai = new List<HuPaiTipsInfo[]>();

                if (huPaiInfosInMoPai == null)
                    huPaiInfosInMoPai = new List<HuPaiTipsInfo[]>();

                CreateHuPaiInfos(desk.mjSeatHandPaiLists[0], mjHuTingCheck.tingDatas, ref huPaiInHandPaiIdxs, ref huPaiInfosInHandPai);
                CreateHuPaiInfos(desk.mjSeatMoPaiLists[0], mjHuTingCheck.tingDatas, ref huPaiInMoPaiIdxs, ref huPaiInfosInMoPai);
            }

            SelectDaPaiStateData stateData = playerStateData[0].GetComponent<SelectDaPaiStateData>();
            stateData.SetSelectDaPaiData(huPaiInHandPaiIdxs, huPaiInfosInHandPai, huPaiInMoPaiIdxs, huPaiInfosInMoPai, opCmdNode);
            playerStateData[0].SetState(SelectDaPaiStateData.SELECT_DA_PAI_START, Time.time, -1);
        }

        #region 选打牌动作
        /// <summary>
        /// 选牌动作
        /// </summary>
        public void ActionSelectDaPai()
        {
            if (playerStateData[0].state < SelectDaPaiStateData.SELECT_DA_PAI_START ||
                playerStateData[0].state > SelectDaPaiStateData.SELECT_DA_PAI_END ||
                Time.time - playerStateData[0].stateStartTime < playerStateData[0].stateLiveTime)
            {
                return;
            }

            SelectDaPaiStateData stateData = playerStateData[0].GetComponent<SelectDaPaiStateData>();

            switch (playerStateData[0].state)
            {
                case SelectDaPaiStateData.SELECT_DA_PAI_START:
                    {
                        uiHuPaiTipsArrow.Show(stateData.selectPaiHuPaiInHandPaiIdxs, stateData.selectPaiHuPaiInMoPaiIdxs);
                        playerStateData[0].SetState(SelectDaPaiStateData.SELECT_DA_PAI_READY_CLICK, Time.time, -1);
                    }
                    break;

                case SelectDaPaiStateData.SELECT_DA_PAI_READY_CLICK:
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
                                if (Time.realtimeSinceStartup - stateData.rayPickMjLastKickTime < 0.15f &&
                                    stateData.rayPickMj == hitInfo.collider.gameObject)
                                {
                                    playerStateData[0].SetState(SelectDaPaiStateData.SELECT_DA_PAI_END, Time.time, -1);
                                    return;
                                }


                                stateData.rayPickMj = hitInfo.collider.gameObject;
                                stateData.rayPickMjMouseOrgPos = Input.mousePosition;
                                stateData.rayPickMjOrgPos = stateData.rayPickMj.transform.localPosition;
                                fit.OffMjShadow(stateData.rayPickMj);

                                stateData.rayPickMjLastKickTime = Time.realtimeSinceStartup;
                                playerStateData[0].SetState(SelectDaPaiStateData.SELECT_DA_PAIING, Time.time, -1);

                            }
                        }
                    }
                    break;


                case SelectDaPaiStateData.SELECT_DA_PAIING:
                    {
                        //松开鼠标按键时
                        if (Input.GetMouseButtonUp(0))
                        {
                            stateData.rayPickMjLastKickTime = Time.realtimeSinceStartup;

                            //麻将选中后未作任何移动
                            if (Mathf.Abs(stateData.rayPickMj.transform.localPosition.y - stateData.rayPickMjOrgPos.y) < 0.001f)
                            {
                                if (stateData.selectedUpMj != stateData.rayPickMj)
                                {
                                    RestoreSelectedUpHandPaiToOrgPos();

                                    stateData.rayPickMj.transform.localPosition =
                                        new Vector3(stateData.rayPickMjOrgPos.x,
                                        stateData.rayPickMjOrgPos.y + fit.handPaiSelectOffsetHeight,
                                        stateData.rayPickMjOrgPos.z);

                                    fit.OffMjShadow(stateData.rayPickMj);
                                    stateData.selectedUpMj = stateData.rayPickMj;

                                    if (!IsHuPaiMj(stateData.rayPickMj))
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

                                playerStateData[0].SetState(SelectDaPaiStateData.SELECT_DA_PAI_READY_CLICK, Time.time, 0);
                            }

                            //麻将移动速度超过25
                            else if (stateData.rayPickMjMoveDistPreDuration > 50)
                            {
                                stateData.rayPickMjMoveDistPreDuration = 0;
                                playerStateData[0].SetState(SelectDaPaiStateData.SELECT_DA_PAI_END, Time.time, -1);
                            }

                            //麻将移动距离未超过指定高度
                            else if (stateData.rayPickMj.transform.localPosition.y < stateData.rayPickMjOrgPos.y + fit.GetCanvasHandMjSizeByAxis(Axis.Y))
                            {
                                stateData.rayPickMj.transform.DOLocalMove(stateData.rayPickMjOrgPos, 0.2f);
                                playerStateData[0].SetState(SelectDaPaiStateData.SELECT_DA_PAI_RESTORE, Time.time, 0.2f);
                                stateData.selectPaiRayPickMj = stateData.rayPickMj;
                            }

                            //
                            else
                            {
                                playerStateData[0].SetState(SelectDaPaiStateData.SELECT_DA_PAI_END, Time.time, -1);
                            }

                            if (fit.isUsePlayerSelectMjOutLine)
                                stateData.rayPickMj.layer = LayerMask.NameToLayer("Default");
                        }
                        else   //未松开按键
                        {
                            if (stateData.selectedUpMj != stateData.rayPickMj)
                            {
                                RestoreSelectedUpHandPaiToOrgPos();
                            }

                            float offsetx = Input.mousePosition.x - stateData.rayPickMjMouseOrgPos.x;
                            float offsety = Input.mousePosition.y - stateData.rayPickMjMouseOrgPos.y;

                            if (Mathf.Abs(offsetx) < 0.001f && Mathf.Abs(offsety) < 0.001f)
                            {
                                if (stateData.selectedUpMj != stateData.rayPickMj &&
                                    !IsHuPaiMj(stateData.rayPickMj))
                                {
                                    uiHuPaiTips.Hide();
                                }

                                return;
                            }

                            uiHuPaiTips.Hide();
                            uiHuPaiTipsArrow.Hide();


                            Vector3 newMjPos =
                                new Vector3(stateData.rayPickMjOrgPos.x + offsetx,
                                stateData.rayPickMjOrgPos.y + offsety + fit.GetCanvasHandMjSizeByAxis(Axis.Y) / 5,
                                stateData.rayPickMjOrgPos.z - fit.GetCanvasHandMjSizeByAxis(Axis.Z));

                            stateData.rayPickMjMoveDistPreDuration =
                                Mathf.Pow(stateData.rayPickMj.transform.localPosition.x - newMjPos.x, 2) +
                                    Mathf.Pow(stateData.rayPickMj.transform.localPosition.y - newMjPos.y, 2);

                            stateData.rayPickMj.transform.localPosition = newMjPos;
                        }
                    }

                    break;

                case SelectDaPaiStateData.SELECT_DA_PAI_RESTORE:
                    {
                        if (stateData.selectedUpMj != stateData.selectPaiRayPickMj)
                            fit.OnMjShadow(stateData.selectPaiRayPickMj, 1);
                        else
                        {
                            ShowHuPaiTips();
                        }

                        uiHuPaiTipsArrow.Show(stateData.selectPaiHuPaiInHandPaiIdxs, stateData.selectPaiHuPaiInMoPaiIdxs);

                        playerStateData[0].SetState(SelectDaPaiStateData.SELECT_DA_PAI_READY_CLICK, Time.time, -1);
                    }
                    break;


                case SelectDaPaiStateData.SELECT_DA_PAI_END:
                    {
                        stateData.selectPaiHuPaiInHandPaiIdxs = null;
                        stateData.selectPaiHuPaiInfosInHandPai = null;
                        stateData.selectPaiHuPaiInMoPaiIdxs = null;
                        stateData.selectPaiHuPaiInfosInMoPai = null;

                        uiHuPaiTips.RemoveAllDetailTips();
                        uiHuPaiTips.Hide();
                        uiHuPaiTipsArrow.Hide();

                        playerStateData[0].state = StateDataGroup.END;


                        int selectPaiHandPaiIdx = desk.mjSeatHandPaiLists[0].IndexOf(stateData.rayPickMj);
                        HandPaiType paiType = HandPaiType.MoPai;

                        if (selectPaiHandPaiIdx != -1)
                        {
                            paiType = HandPaiType.HandPai;
                        }
                        else
                        {
                            selectPaiHandPaiIdx = desk.mjSeatMoPaiLists[0].IndexOf(stateData.rayPickMj);
                        }

                        SelfSelectDaPaiEnd(selectPaiHandPaiIdx, paiType);


                        stateData.rayPickMj = null;
                        stateData.selectedUpMj = null;

                        ProcessHandActionmjCmdMgr(0, stateData);
                    }
                    break;

            }

        }

        void SelfSelectDaPaiEnd(int selectPaiHandPaiIdx, HandPaiType paiType)
        {
            MahjongDaPaiOpCmd cmd = MjMachineCmdPool.Instance.CreateCmd<MahjongDaPaiOpCmd>();
            cmd.seatIdx = 0;
            cmd.handStyle = PlayerType.FEMALE;
            cmd.paiIdx = selectPaiHandPaiIdx;
            cmd.paiType = paiType;
            cmd.mjFaceValue = desk.GetHandPaiMjFaceValue(0, selectPaiHandPaiIdx);
            mjCmdMgr.Append(cmd);

            MahjongSortPaiOpCmd cmd2 = MjMachineCmdPool.Instance.CreateCmd<MahjongSortPaiOpCmd>();
            cmd2.seatIdx = 0;
            cmd2.handStyle = PlayerType.FEMALE;
            cmd2.canSelectPaiAfterCmdEnd = true;
            mjCmdMgr.Append(cmd2);

            //test
            ReqSelectDaPaiOpCmd cmdx = MjMachineCmdPool.Instance.CreateCmd<ReqSelectDaPaiOpCmd>();
            mjCmdMgr.Append(cmdx);
        }

        bool IsHuPaiMj(GameObject mj)
        {
            SelectDaPaiStateData stateData = playerStateData[0].GetComponent<SelectDaPaiStateData>();
            int idx = desk.GetPaiIdxInHandPaiList(0, mj);
            if (idx != -1)
            {
                idx = Common.IndexOf(stateData.selectPaiHuPaiInHandPaiIdxs, idx);
            }
            else
            {
                idx = desk.GetPaiIdxInMoPaiList(0, mj);
                if (idx != -1)
                    idx = Common.IndexOf(stateData.selectPaiHuPaiInMoPaiIdxs, idx);
            }

            return idx == -1 ? false : true;
        }

        void ShowHuPaiTips()
        {
            SelectDaPaiStateData stateData = playerStateData[0].GetComponent<SelectDaPaiStateData>();
            int idx = desk.GetPaiIdxInHandPaiList(0, stateData.rayPickMj);

            if (idx != -1 && stateData.selectPaiHuPaiInfosInHandPai != null)
            {
                idx = Common.IndexOf(stateData.selectPaiHuPaiInHandPaiIdxs, idx);
                if (idx != -1)
                {
                    uiHuPaiTips.Show(stateData.selectPaiHuPaiInfosInHandPai[idx]);
                }
            }
            else
            {
                idx = desk.GetPaiIdxInMoPaiList(0, stateData.rayPickMj);
                if (idx != -1 && stateData.selectPaiHuPaiInfosInMoPai != null)
                {
                    idx = Common.IndexOf(stateData.selectPaiHuPaiInMoPaiIdxs, idx);
                    if (idx != -1)
                    {
                        uiHuPaiTips.Show(stateData.selectPaiHuPaiInfosInMoPai[idx]);
                    }
                }
            }
        }

        void RestoreSelectedUpHandPaiToOrgPos(bool isHideHuPaiTips = false)
        {
            SelectDaPaiStateData stateData = playerStateData[0].GetComponent<SelectDaPaiStateData>();

            if (stateData.selectedUpMj == null)
                return;

            Vector3 pos = stateData.selectedUpMj.transform.localPosition;
            pos.y -= fit.handPaiSelectOffsetHeight;
            stateData.selectedUpMj.transform.localPosition = pos;
            fit.OnMjShadow(stateData.selectedUpMj, 1);
            stateData.selectedUpMj = null;

            if (isHideHuPaiTips)
                uiHuPaiTips.Hide();
        }

        #endregion


        public void CreateHuPaiInfos(List<GameObject> paiList, TingData[] tingDatas, ref int[] huPaiIdxs, ref List<HuPaiTipsInfo[]> huPaiInfos)
        {
            int[] tmpHuPaiIdxs = new int[100];
            int tingHuPaiCount = 0;

            HuPaiTipsInfo[] tmpHuPaiTipsInfos = new HuPaiTipsInfo[100];
            int huPaiTipsCount = 0;

            int richTingHuMjCount = 0;
            MahjongFaceValue tingHuFaceValue;
            int cardIdx;
            GameObject[] deskMjs;

            for (int i = 0; tingDatas[i].tingCardIdx != -1; i++)
            {
                for (int j = 0; j < paiList.Count; j++)
                {
                    cardIdx = (int)(paiList[j].GetComponent<MjPaiData>().mjFaceValue);
                    if (cardIdx != tingDatas[i].tingCardIdx)
                        continue;

                    tmpHuPaiIdxs[tingHuPaiCount++] = j;

                    for (int k = 0; k <= tingDatas[i].huCardsEndIdx; k++)
                    {
                        tingHuFaceValue = (MahjongFaceValue)(tingDatas[i].huCards[k]);

                        deskMjs = desk.GetMjGroupByDeskGlobalMjPaiSetDict(tingHuFaceValue);

                        richTingHuMjCount = 4;
                        if (deskMjs != null)
                            richTingHuMjCount -= deskMjs.Length;

                        richTingHuMjCount -=
                            desk.GetHandPaiCountForMjFaceValue(tingHuFaceValue) +
                            desk.GetMoPaiCountForMjFaceValue(tingHuFaceValue);

                        if (richTingHuMjCount <= 0)
                            continue;

                        HuPaiTipsInfo info = new HuPaiTipsInfo();
                        info.faceValue = (MahjongFaceValue)(tingDatas[i].huCards[k]);
                        info.zhangAmount = richTingHuMjCount;

                        tmpHuPaiTipsInfos[huPaiTipsCount++] = info;
                    }

                    if (huPaiTipsCount > 0)
                    {
                        HuPaiTipsInfo[] huPaiTipsInfos = new HuPaiTipsInfo[huPaiTipsCount];
                        for (int f = 0; f < huPaiTipsCount; f++)
                            huPaiTipsInfos[f] = tmpHuPaiTipsInfos[f];

                        huPaiInfos.Add(huPaiTipsInfos);
                        huPaiTipsCount = 0;
                    }
                    else
                    {
                        tingHuPaiCount--;
                    }

                    break;
                }
            }

            if (tingHuPaiCount > 0)
            {
                huPaiIdxs = new int[tingHuPaiCount];
                for (int f = 0; f < tingHuPaiCount; f++)
                    huPaiIdxs[f] = tmpHuPaiIdxs[f];

                tingHuPaiCount = 0;
            }
        }
    }
}
