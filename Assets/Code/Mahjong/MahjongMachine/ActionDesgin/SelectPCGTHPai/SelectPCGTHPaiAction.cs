using ComponentDesgin;
using DG.Tweening;
using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ActionDesgin
{
    /// <summary>
    /// 选择碰吃杠听胡牌动作
    /// </summary>
    public class SelectPCGTHPaiAction : BaseHandAction
    {
        private static SelectPCGTHPaiAction instance = null;
        public static SelectPCGTHPaiAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new SelectPCGTHPaiAction();
                return instance;
            }
        }

        UIPCGHTBtnMgr uiPcghtBtnMgr;
        UIChiPaiTips uiChiPaiTips;
        UIHuPaiTips uiHuPaiTips;
        UIHuPaiHandPaiIdxTipsArrow uiHuPaiTipsArrow;

        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);
            uiPcghtBtnMgr = mjMachine.GetComponent<UIPCGHTBtnMgr>();
            uiChiPaiTips = mjMachine.GetComponent<UIChiPaiTips>();
            uiHuPaiTips = mjMachine.GetComponent<UIHuPaiTips>();
            uiHuPaiTipsArrow = mjMachine.GetComponent<UIHuPaiHandPaiIdxTipsArrow>();
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("SelectPCGTHPai", ActionSelectPCGTHPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("SelectPCGTHPai");
        }


        #region 选择碰吃杠胡听牌动作
        /// <summary>
        /// 选碰吃杠听胡牌
        /// </summary>
        public void SelectPCGTHPai(
            PengChiGangTingHuType[] pcgthBtnTypes,
            List<MahjongFaceValue[]> chiPaiMjValueList,
            int[] tingPaiInHandPaiIdxs, List<HuPaiTipsInfo[]> tingPaiInfosInHandPai,
            int[] tingPaiInMoPaiIdxs, List<HuPaiTipsInfo[]> tingPaiInfosInMoPai,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(0);

            if (playerStateData[0].state != HandActionState.END)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            SelectPCGTHPaiStateData stateData = playerStateData[0].GetComponent<SelectPCGTHPaiStateData>();

            stateData.SetSelectPCGTHPaiData(pcgthBtnTypes, chiPaiMjValueList, tingPaiInHandPaiIdxs, tingPaiInfosInHandPai, tingPaiInMoPaiIdxs, tingPaiInfosInMoPai, opCmdNode);
            playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_START, Time.time, -1);
        }


        /// <summary>
        /// 选碰吃杠胡听牌动作
        /// </summary>
        public void ActionSelectPCGTHPai()
        {
            if (playerStateData[0].state < HandActionState.SELECT_PCGTH_PAI_START ||
                playerStateData[0].state > HandActionState.SELECT_PCGTH_PAI_END ||
                Time.time - playerStateData[0].stateStartTime < playerStateData[0].stateLiveTime)
            {
                return;
            }

            SelectPCGTHPaiStateData stateData = playerStateData[0].GetComponent<SelectPCGTHPaiStateData>();
     
            switch (playerStateData[0].state)
            {
                case HandActionState.SELECT_PCGTH_PAI_START:
                    {
                        uiPcghtBtnMgr.Show(stateData.selectPcgthBtnTypes);
                        playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECTTING, Time.time, -1);
                    }
                    break;

                case HandActionState.SELECT_PCGTH_PAI_SELECTTING:
                    {
                        if (uiPcghtBtnMgr.IsClicked == false)
                            break;

                        stateData.selectPcgthedType = uiPcghtBtnMgr.clickedBtnType;
                        stateData.selectPcgthedChiPaiMjValueIdx = -1;
                        stateData.selectPaiHandPaiIdx = -1;


                        switch (uiPcghtBtnMgr.clickedBtnType)
                        {
                            case PengChiGangTingHuType.CHI:
                                {
                                    if (stateData.selectPcgthChiPaiMjValueList == null)
                                        break;

                                    uiChiPaiTips.Show(stateData.selectPcgthChiPaiMjValueList);
                                    uiPcghtBtnMgr.Show(new PengChiGangTingHuType[] { PengChiGangTingHuType.CANCEL }, new Vector3(100f, 0, 0));
                                    playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECTTING_CHIPAI, Time.time, -1);
                                }
                                break;


                            case PengChiGangTingHuType.TING:
                                {
                                    uiPcghtBtnMgr.Show(new PengChiGangTingHuType[] { PengChiGangTingHuType.CANCEL }, new Vector3(100f, 0, 0));
                                    playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_START, Time.time, -1);
                                }
                                break;

                            default:
                                playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_END, Time.time, -1);
                                break;

                        }

                    }
                    break;

                case HandActionState.SELECT_PCGTH_PAI_SELECTTING_CHIPAI:
                    {
                        if (uiChiPaiTips.selectedIdx != -1)
                        {
                            uiPcghtBtnMgr.Hide();
                            stateData.selectPcgthedChiPaiMjValueIdx = uiChiPaiTips.selectedIdx;
                            playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_END, Time.time, -1);
                        }
                        else if (uiPcghtBtnMgr.IsClicked == true)
                        {
                            uiChiPaiTips.Hide();
                            playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_START, Time.time, -1);
                        }
                    }
                    break;

                case HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_START:
                    {
                        uiHuPaiTipsArrow.Show(stateData.selectPaiHuPaiInHandPaiIdxs, stateData.selectPaiHuPaiInMoPaiIdxs);
                        playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK, Time.time, -1);
                    }
                    break;

                case HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK:
                    {
                        if (uiPcghtBtnMgr.IsClicked == true)
                        {
                            RestoreSelectedUpHandPaiToOrgPos(stateData);
                            uiHuPaiTips.RemoveAllDetailTips();
                            uiHuPaiTips.Hide();
                            uiHuPaiTipsArrow.Hide();
                            stateData.rayPickMj = null;
                            playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_START, Time.time, -1);
                        }
                        else if (Input.GetMouseButtonDown(0))
                        {
                            //从摄像机发出到点击坐标的射线
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hitInfo;

                            if (Physics.Raycast(ray, out hitInfo))
                            {
                                //双击出牌
                                if (Time.realtimeSinceStartup - stateData.rayPickMjLastKickTime < 0.15f &&
                                    stateData.rayPickMj == hitInfo.collider.gameObject)
                                {
                                    playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END, Time.time, -1);
                                    return;
                                }


                                stateData.rayPickMj = hitInfo.collider.gameObject;
                                stateData.rayPickMjMouseOrgPos = Input.mousePosition;
                                stateData.rayPickMjOrgPos = stateData.rayPickMj.transform.localPosition;
                                fit.OffMjShadow(stateData.rayPickMj);

                                stateData.rayPickMjLastKickTime = Time.realtimeSinceStartup;
                                playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAIING, Time.time, -1);

                            }
                        }
                    }
                    break;


                case HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAIING:
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
                                    RestoreSelectedUpHandPaiToOrgPos(stateData);

                                    stateData.rayPickMj.transform.localPosition =
                                        new Vector3(stateData.rayPickMjOrgPos.x,
                                        stateData.rayPickMjOrgPos.y + fit.handPaiSelectOffsetHeight,
                                        stateData.rayPickMjOrgPos.z);

                                    fit.OffMjShadow(stateData.rayPickMj);
                                    stateData.selectedUpMj = stateData.rayPickMj;

                                    if (!IsHuPaiMj(stateData.rayPickMj, stateData))
                                    {
                                        uiHuPaiTips.Hide();
                                    }
                                    else
                                    {
                                        ShowHuPaiTips(stateData);
                                    }
                                }
                                else
                                {
                                    RestoreSelectedUpHandPaiToOrgPos(stateData, true);
                                }

                                playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK, Time.time, 0);
                            }

                            //麻将移动速度超过25
                            else if (stateData.rayPickMjMoveDistPreDuration > 50)
                            {
                                stateData.rayPickMjMoveDistPreDuration = 0;
                                playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END, Time.time, -1);
                            }

                            //麻将移动距离未超过指定高度
                            else if (stateData.rayPickMj.transform.localPosition.y < stateData.rayPickMjOrgPos.y + fit.GetCanvasHandMjSizeByAxis(Axis.Y))
                            {
                                stateData.rayPickMj.transform.DOLocalMove(stateData.rayPickMjOrgPos, 0.2f);
                                playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_RESTORE, Time.time, 0.2f);
                                stateData.selectPaiRayPickMj = stateData.rayPickMj;
                            }

                            //
                            else
                            {
                                playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END, Time.time, -1);
                            }

                            if (fit.isUsePlayerSelectMjOutLine)
                                stateData.rayPickMj.layer = LayerMask.NameToLayer("Default");
                        }
                        else   //未松开按键
                        {
                            if (stateData.selectedUpMj != stateData.rayPickMj)
                            {
                                RestoreSelectedUpHandPaiToOrgPos(stateData);
                            }

                            float offsetx = Input.mousePosition.x - stateData.rayPickMjMouseOrgPos.x;
                            float offsety = Input.mousePosition.y - stateData.rayPickMjMouseOrgPos.y;

                            if (Mathf.Abs(offsetx) < 0.001f && Mathf.Abs(offsety) < 0.001f)
                            {
                                if (stateData.selectedUpMj != stateData.rayPickMj &&
                                    !IsHuPaiMj(stateData.rayPickMj, stateData))
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

                case HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_RESTORE:
                    {
                        if (stateData.selectedUpMj != stateData.selectPaiRayPickMj)
                            fit.OnMjShadow(stateData.selectPaiRayPickMj, 1);
                        else
                        {
                            ShowHuPaiTips(stateData);
                        }

                        uiHuPaiTipsArrow.Show(stateData.selectPaiHuPaiInHandPaiIdxs, stateData.selectPaiHuPaiInMoPaiIdxs);

                        playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK, Time.time, -1);
                    }
                    break;


                case HandActionState.SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END:
                    {
                        uiHuPaiTips.RemoveAllDetailTips();
                        uiHuPaiTips.Hide();
                        uiHuPaiTipsArrow.Hide();

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

                        stateData.selectPaiHandPaiIdx = selectPaiHandPaiIdx;
                        stateData.selectPaiType = paiType;

                        stateData.rayPickMj = null;
                        playerStateData[0].SetState(HandActionState.SELECT_PCGTH_PAI_END, Time.time, -1);

                    }
                    break;

                case HandActionState.SELECT_PCGTH_PAI_END:
                    {
                        playerStateData[0].state = HandActionState.END;


                        SelfSelectPCGTHPaiEnd(
                                 stateData.selectPcgthedType,
                                 stateData.selectPcgthedChiPaiMjValueIdx,
                                 stateData.selectPaiHandPaiIdx,
                                stateData.selectPaiType);

                        ProcessHandActionmjCmdMgr(0, stateData);
                    }
                    break;
            }
        }




        void SelfSelectPCGTHPaiEnd(PengChiGangTingHuType selectBtnType, int chiPaiMjValueIdx, int tingPaiHandPaiIdx, HandPaiType tingPaiType)
        {


        }



        bool IsHuPaiMj(GameObject mj, SelectPCGTHPaiStateData stateData)
        {
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

        void ShowHuPaiTips(SelectPCGTHPaiStateData stateData)
        {
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

        void RestoreSelectedUpHandPaiToOrgPos(SelectPCGTHPaiStateData stateData, bool isHideHuPaiTips = false)
        {
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

    }
}
