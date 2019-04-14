using CmdDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ActionDesgin
{
    /// <summary>
    /// 选择缺门动作
    /// </summary>
    public class SelectQueMenAction : BaseHandAction
    {
        private static SelectQueMenAction instance = null;
        public static SelectQueMenAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new SelectQueMenAction();
                return instance;
            }
        }

        UISelectQueMen uiSelectQueMen;
        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);
            uiSelectQueMen = mjMachine.GetComponent<UISelectQueMen>();
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("SelectQueMen", ActionSelectQueMen);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("SelectQueMen");
        }

        #region 选择缺门动作

        /// <summary>
        /// 选缺门
        /// </summary>
        public void SelectQueMen(MahjongHuaSe defaultQueMenHuaSe, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(0);

            if (playerStateData[0].state != StateDataGroup.END)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            SelectQueMenStateData stateData = playerStateData[0].GetComponent<SelectQueMenStateData>();

            stateData.SetSelectQueMenData(defaultQueMenHuaSe, opCmdNode);
            playerStateData[0].SetState(SelectQueMenStateData.SELECT_QUE_MEN_START, Time.time, -1);
        }


        public void ActionSelectQueMen()
        {
            if (playerStateData[0].state < SelectQueMenStateData.SELECT_QUE_MEN_START ||
                playerStateData[0].state > SelectQueMenStateData.SELECT_QUE_MEN_END ||
                Time.time - playerStateData[0].stateStartTime < playerStateData[0].stateLiveTime)
            {
                return;
            }

            SelectQueMenStateData stateData = playerStateData[0].GetComponent<SelectQueMenStateData>();

            switch (playerStateData[0].state)
            {
                case SelectQueMenStateData.SELECT_QUE_MEN_START:
                    {
                        stateData.queMenIsPlayDownAudio = false;
                        stateData.queMenIsPlayFeiDingQueAudio = false;
                        uiSelectQueMen.Show();
                        playerStateData[0].SetState(SelectQueMenStateData.SELECT_QUE_MEN_SELECTTING, Time.time, -1);
                    }
                    break;

                case SelectQueMenStateData.SELECT_QUE_MEN_SELECTTING:
                    {
                        if (uiSelectQueMen.IsClicked)
                        {
                            audio.PlayEffectAudio(AudioIdx.AUDIO_EFFECT_DOWN);

                            PlayEffectAudioOpCmd cmd = MjMachineCmdPool.Instance.CreateCmd<PlayEffectAudioOpCmd>();
                            cmd.audioIdx = AudioIdx.AUDIO_EFFECT_DOWN;
                            cmd.delayExecuteTime = 0.07f;
                            mjCmdMgr.AppendCmdToDelayCmdList(cmd);

                            cmd = MjMachineCmdPool.Instance.CreateCmd<PlayEffectAudioOpCmd>();
                            cmd.audioIdx = AudioIdx.AUDIO_EFFECT_FEIDINGQUE;
                            cmd.delayExecuteTime = 0.6f;
                            mjCmdMgr.AppendCmdToDelayCmdList(cmd);

                            stateData.queMenHuaSe = uiSelectQueMen.ClickedHuaSe;
                            SelfSelectQueMenEnd(stateData.queMenHuaSe);
                            playerStateData[0].SetState(SelectQueMenStateData.SELECT_QUE_MEN_END, Time.time, 0);
                        }
                    }
                    break;


                case SelectQueMenStateData.SELECT_QUE_MEN_END:
                    {

                        if (uiSelectQueMen.IsCompleteQueMenSelected == true)
                        {
                            playerStateData[0].state = StateDataGroup.END;
                            ProcessHandActionmjCmdMgr(0, stateData);
                        }
                        else
                        {
                            uiSelectQueMen.PlayQueMenFromList();
                        }
                    }
                    break;
            }
        }

        void SelfSelectQueMenEnd(MahjongHuaSe queHuaSe)
        {
            QueMenCmd cmd = MjMachineCmdPool.Instance.CreateCmd<QueMenCmd>();
            cmd.seatIdx = 1;
            cmd.queMenHuaSe = MahjongHuaSe.TIAO;
            mjCmdMgr.Append(cmd);

            cmd = MjMachineCmdPool.Instance.CreateCmd<QueMenCmd>();
            cmd.seatIdx = 2;
            cmd.queMenHuaSe = MahjongHuaSe.WANG;
            mjCmdMgr.Append(cmd);

            cmd = MjMachineCmdPool.Instance.CreateCmd<QueMenCmd>();
            cmd.seatIdx = 3;
            cmd.queMenHuaSe = MahjongHuaSe.TONG;
            mjCmdMgr.Append(cmd);
        }

        #endregion
    }
}
