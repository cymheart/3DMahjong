using CmdDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ActionDesgin
{
    /// <summary>
    /// 选择交换牌动作
    /// </summary>
    public class SelectSwapPaiAction : BaseHandAction
    {
        private static SelectSwapPaiAction instance = null;
        public static SelectSwapPaiAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new SelectSwapPaiAction();
                return instance;
            }
        }

        UISelectSwapHandPai uiSelectSwapHandPai;
        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);
            uiSelectSwapHandPai = mjMachine.GetComponent<UISelectSwapHandPai>();
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("SelectSwapPai", ActionSelectSwapPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("SelectSwapPai");
        }

        #region 选交换牌动作

        /// <summary>
        /// 选交换牌
        /// </summary>
        public void SelectSwapPai(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(0);

            if (playerStateData[0].state != StateDataGroup.END)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            SelectSwapPaiStateData stateData = playerStateData[0].GetComponent<SelectSwapPaiStateData>();

            stateData.SetSelectSwapPaiData(opCmdNode);
            playerStateData[0].SetState(SelectSwapPaiStateData.SELECT_SWAP_PAI_START, Time.time, -1);
        }



        /// <summary>
        /// 选交换牌动作
        /// </summary>
        public void ActionSelectSwapPai()
        {
            if (playerStateData[0].state < SelectSwapPaiStateData.SELECT_SWAP_PAI_START ||
                playerStateData[0].state > SelectSwapPaiStateData.SELECT_SWAP_PAI_END ||
                Time.time - playerStateData[0].stateStartTime < playerStateData[0].stateLiveTime)
            {
                return;
            }

            SelectDaPaiStateData daPaiStateData = playerStateData[0].GetComponent<SelectDaPaiStateData>();

            switch (playerStateData[0].state)
            {
                case SelectSwapPaiStateData.SELECT_SWAP_PAI_START:
                    {
                        uiSelectSwapHandPai.Show();
                        playerStateData[0].SetState(SelectSwapPaiStateData.SELECT_SWAP_PAI_SELECTTING, Time.time, -1);
                    }
                    break;

                case SelectSwapPaiStateData.SELECT_SWAP_PAI_SELECTTING:
                    {
                        int selectedCount = daPaiStateData.rayClickPickHandPaiMjIdxList.Count + daPaiStateData.rayClickPickMoPaiMjIdxList.Count;

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
                                int selectIdx = desk.mjSeatHandPaiLists[0].IndexOf(mj);

                                if (desk.mjSeatHandPaiLists[0].Contains(mj))
                                {
                                    selectIdx = desk.mjSeatHandPaiLists[0].IndexOf(mj);

                                    if (daPaiStateData.rayClickPickHandPaiMjIdxList.Contains(selectIdx))
                                    {
                                        daPaiStateData.rayClickPickHandPaiMjIdxList.Remove(selectIdx);
                                        daPaiStateData.rayPickMjOrgPos = mj.transform.localPosition;

                                        mj.transform.localPosition = 
                                            new Vector3(daPaiStateData.rayPickMjOrgPos.x, 
                                            daPaiStateData.rayPickMjOrgPos.y - fit.handPaiSelectOffsetHeight,
                                            daPaiStateData.rayPickMjOrgPos.z);

                                        fit.OnMjShadow(mj);
                                    }
                                    else
                                    {
                                        daPaiStateData.rayClickPickHandPaiMjIdxList.Add(selectIdx);
                                        daPaiStateData.rayPickMjOrgPos = mj.transform.localPosition;
                                        mj.transform.localPosition = 
                                            new Vector3(daPaiStateData.rayPickMjOrgPos.x,
                                            daPaiStateData.rayPickMjOrgPos.y + fit.handPaiSelectOffsetHeight,
                                            daPaiStateData.rayPickMjOrgPos.z);

                                        fit.OffMjShadow(mj);
                                    }
                                }
                                else
                                {
                                    selectIdx = desk.mjSeatMoPaiLists[0].IndexOf(mj);

                                    if (daPaiStateData.rayClickPickMoPaiMjIdxList.Contains(selectIdx))
                                    {
                                        daPaiStateData.rayClickPickMoPaiMjIdxList.Remove(selectIdx);
                                        daPaiStateData.rayPickMjOrgPos = mj.transform.localPosition;

                                        mj.transform.localPosition =
                                            new Vector3(daPaiStateData.rayPickMjOrgPos.x, 
                                            daPaiStateData.rayPickMjOrgPos.y - fit.handPaiSelectOffsetHeight,
                                            daPaiStateData.rayPickMjOrgPos.z);

                                        fit.OnMjShadow(mj);
                                    }
                                    else
                                    {
                                        daPaiStateData.rayClickPickMoPaiMjIdxList.Add(selectIdx);
                                        daPaiStateData.rayPickMjOrgPos = mj.transform.localPosition;
                                        mj.transform.localPosition = 
                                            new Vector3(daPaiStateData.rayPickMjOrgPos.x,
                                            daPaiStateData.rayPickMjOrgPos.y + fit.handPaiSelectOffsetHeight,
                                            daPaiStateData.rayPickMjOrgPos.z);

                                        fit.OffMjShadow(mj);
                                    }
                                }
                            }
                        }
                        else if (uiSelectSwapHandPai.IsOkClicked)
                        {

                            SelectSwapPaiEnd(daPaiStateData.rayClickPickHandPaiMjIdxList.ToArray(),
                                daPaiStateData.rayClickPickMoPaiMjIdxList.ToArray());
                          
                            daPaiStateData.rayClickPickHandPaiMjIdxList.Clear();
                            daPaiStateData.rayClickPickMoPaiMjIdxList.Clear();
                            playerStateData[0].SetState(SelectSwapPaiStateData.SELECT_SWAP_PAI_END, Time.time, -1);
                        }
                    }
                    break;

                case SelectSwapPaiStateData.SELECT_SWAP_PAI_END:
                    {
                        if (uiSelectSwapHandPai.IsCompleteSwapPaiSelected == true)
                        {
                            SelectSwapPaiStateData stateData = playerStateData[0].GetComponent<SelectSwapPaiStateData>();
                            playerStateData[0].state = StateDataGroup.END;
                            ProcessHandActionmjCmdMgr(0, stateData);
                        }
                    }
                    break;
            }
        }

        void SelectSwapPaiEnd(int[] handPaiIdxs, int[] moPaiIdxs)
        {
            MahjongMachineCmd cmdx = MjMachineCmdPool.Instance.CreateCmd<HideSwapPaiUICmd>();
            mjCmdMgr.Append(cmdx);

            ShowSwapPaiHintCmd showCmd = MjMachineCmdPool.Instance.CreateCmd<ShowSwapPaiHintCmd>();
            showCmd.isBlock = false;
            showCmd.swapPaiDirection = SwapPaiDirection.OPPOSITE;
            mjCmdMgr.Append(showCmd);

            MahjongSwapPaiGroupCmd cmd = MjMachineCmdPool.Instance.CreateCmd<MahjongSwapPaiGroupCmd>();
            cmd.SwapHandPaiIdx = handPaiIdxs;
            cmd.SwapMoPaiIdx = moPaiIdxs;
            cmd.SwapDirection = SwapPaiDirection.OPPOSITE;
            cmd.TakeMjFaceValues = new MahjongFaceValue[] { MahjongFaceValue.MJ_TIAO_3, MahjongFaceValue.MJ_WANG_4, MahjongFaceValue.MJ_WANG_9 };
            cmd.delayExecuteTime = 1f;
            showCmd.Append(mjCmdMgr.CreateCmdNode(cmd));
        }

        #endregion


    }
}
