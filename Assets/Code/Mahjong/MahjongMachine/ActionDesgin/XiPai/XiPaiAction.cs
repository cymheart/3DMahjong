using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ComponentDesgin;

namespace ActionDesgin
{
    /// <summary>
    /// 洗牌动作
    /// </summary>
    public class XiPaiAction: BaseHandAction
    {
        private static XiPaiAction instance = null;
        public static XiPaiAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new XiPaiAction();
                return instance;
            }
        }

        MahjongDiceMachine diceMachine;

        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);
            diceMachine = mjMachine.GetComponent<MahjongDiceMachine>();
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("xipai", ActionXiPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("xipai");
        }

        #region 洗牌动作
        /// <summary>
        /// 洗牌
        /// </summary>
        public void XiPai(int dealerSeatIdx, FengWei fengWei, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            if (mjMachineStateData.state != MjMachineState.END)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            NewRound();

            XiPaiStateData stateData = mjMachineStateData.GetComponent<XiPaiStateData>();

            stateData.SetXiPaiData(dealerSeatIdx, fengWei, opCmdNode);
            mjMachineStateData.SetState(MjMachineState.XIPAI_START, Time.time, -1);
        }


        /// <summary>
        /// 洗牌动作
        /// </summary>
        public void ActionXiPai()
        {
            if (mjMachineStateData.state < MjMachineState.XIPAI_START ||
                mjMachineStateData.state > MjMachineState.XIPAI_END ||
                Time.time - mjMachineStateData.stateStartTime < mjMachineStateData.stateLiveTime)
            {
                return;
            }

            XiPaiStateData stateData = mjMachineStateData.GetComponent<XiPaiStateData>();

            switch (mjMachineStateData.state)
            {
                case MjMachineState.XIPAI_START:
                    {
                        fit.SetDealer(stateData.dealerSeatIdx, stateData.fengWei);
                        diceMachine.SetSeatFengWei(stateData.dealerSeatIdx, stateData.fengWei);

                        MjTuoXiPaiShengqi(desk.deskMjTuoName[1], preSettingHelper.deskMjTuoPos[1], 0.2416f, MahjongGameDir.MG_DIR_X);
                        MjTuoXiPaiShengqi(desk.deskMjTuoName[3], preSettingHelper.deskMjTuoPos[3], -0.2416f, MahjongGameDir.MG_DIR_X);
                        MjTuoXiPaiShengqi(desk.deskMjTuoName[2], preSettingHelper.deskMjTuoPos[2], -0.2185f, MahjongGameDir.MG_DIR_Z);
                        MjTuoXiPaiShengqi(desk.deskMjTuoName[0], preSettingHelper.deskMjTuoPos[0], 0.2185f, MahjongGameDir.MG_DIR_Z);

                        mjMachineStateData.SetState(MjMachineState.XIPAI_END, Time.time, 1f);

                    }
                    break;

                case MjMachineState.XIPAI_END:
                    {
                        mjMachineStateData.state = MjMachineState.END;
                        ProcessCommonActionmjCmdMgr(stateData);
                    }
                    break;
            }
        }
        void MjTuoXiPaiShengqi(string mjtuoName, Vector3 mjtuoOrgPos, float xorzOffsetDstValue, MahjongGameDir dir)
        {
            Transform mjtuo_tf = desk.mjtableTransform.Find(mjtuoName);
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


        /// <summary>
        /// 新的一局
        /// </summary>
        void NewRound()
        {
            diceMachine.EndRun();
            mjMachine.ResetData();
        }

        #endregion


    }
}
