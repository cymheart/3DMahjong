using ComponentDesgin;
using CoreDesgin;
using UnityEngine;

namespace ActionDesgin
{
    /// <summary>
    /// 选牌动作
    /// </summary>
    public class SelectPaiAction: BaseHandAction
    {
        private static SelectPaiAction instance = null;
        public static SelectPaiAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new SelectPaiAction();
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
            mjMachineUpdater.Reg("SelectPai", ActionSelectPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("SelectPai");
        }


        #region 选牌动作
        /// <summary>
        /// 选牌动作
        /// </summary>
        public void ActionSelectPai()
        {
            if (playerStateData[0].state < HandActionState.SELECT_PAI_START ||
               playerStateData[0].state > HandActionState.SELECT_PAI_END ||
               Time.time - playerStateData[0].stateStartTime < playerStateData[0].stateLiveTime)
            {
                return;
            }

            SelectDaPaiStateData stateData = playerStateData[0].GetComponent<SelectDaPaiStateData>();

            switch (playerStateData[0].state)
            {
                case HandActionState.SELECT_PAI_START:
                    {
                        uiHuPaiTipsArrow.Show(stateData.selectPaiHuPaiInHandPaiIdxs, stateData.selectPaiHuPaiInMoPaiIdxs);
                        playerStateData[0].SetState(HandActionState.SELECT_PAI_READY_CLICK, Time.time, -1);
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
                                stateData.rayPickMj = hitInfo.collider.gameObject;
                                stateData.rayPickMjMouseOrgPos = Input.mousePosition;
                                stateData.rayPickMjOrgPos = stateData.rayPickMj.transform.localPosition;
                                fit.OffMjShadow(stateData.rayPickMj);

                                stateData.rayPickMjLastKickTime = Time.realtimeSinceStartup;
                                playerStateData[0].SetState(HandActionState.SELECT_PAIING, Time.time, -1);

                            }
                        }
                    }
                    break;


                case HandActionState.SELECT_PAIING:
                    {
                        //松开鼠标按键时
                        if (Input.GetMouseButtonUp(0))
                        {
                            stateData.rayPickMjLastKickTime = Time.realtimeSinceStartup;

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

                            playerStateData[0].SetState(HandActionState.SELECT_PAI_READY_CLICK, Time.time, 0);

                        }
                        else   //未松开按键
                        {
                            if (stateData.selectedUpMj != stateData.rayPickMj)
                            {
                                RestoreSelectedUpHandPaiToOrgPos();
                            }

                            if (stateData.selectedUpMj != stateData.rayPickMj &&
                                     !IsHuPaiMj(stateData.rayPickMj))
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

                        playerStateData[0].state = HandActionState.END;
                        stateData.rayPickMj = null;

                    }
                    break;

            }

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
    }
}
