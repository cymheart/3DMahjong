using ComponentDesgin;
using CoreDesgin;
using UnityEngine;

namespace ActionDesgin
{
    /// <summary>
    /// 交换牌提示动作
    /// </summary>
    public class SwapPaiHintAction : BaseHandAction
    {
        private static SwapPaiHintAction instance = null;
        public static SwapPaiHintAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new SwapPaiHintAction();
                return instance;
            }
        }

        SwapPaiHintArrowEffect swapPaiHintArrowEffect;
        UISwapPaiingTips uiSwapPaiingTips;
        StateDatas<SwapPaiHintState> swapPaiHintStateDataGroup;
    

        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);
            swapPaiHintArrowEffect = mjMachine.GetComponent<SwapPaiHintArrowEffect>();
            uiSwapPaiingTips = mjMachine.GetComponent<UISwapPaiingTips>();
            swapPaiHintStateDataGroup = states.swapPaiHintStateData;
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("SwapPaiHint", ActionSwapPaiHint);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("SwapPaiHint");
        }


        #region 换牌提示动作
        /// <summary>
        /// 显示交换牌提示
        /// </summary>
        public void ShowSwapPaiHint(SwapPaiDirection swapPaiDir)
        {
            if (swapPaiHintStateDataGroup.state != SwapPaiHintState.HINT_END)
            {
                return;
            }

            SwapPaiHintStateData stateData = swapPaiHintStateDataGroup.GetComponent<SwapPaiHintStateData>();

            stateData.SetData(swapPaiDir);
            swapPaiHintStateDataGroup.SetState(SwapPaiHintState.HINT_START, Time.time, -1);
        }


        /// <summary>
        /// 换牌提示动作
        /// </summary>
        public void ActionSwapPaiHint()
        {
            if (swapPaiHintStateDataGroup.state < SwapPaiHintState.HINT_START ||
                swapPaiHintStateDataGroup.state >= SwapPaiHintState.HINT_END ||
                Time.time - swapPaiHintStateDataGroup.stateStartTime < swapPaiHintStateDataGroup.stateLiveTime)
            {
                return;
            }

            SwapPaiHintStateData swapPaiHintStateData = swapPaiHintStateDataGroup.GetComponent<SwapPaiHintStateData>();

            switch (swapPaiHintStateDataGroup.state)
            {
                case SwapPaiHintState.HINT_START:
                    {
                        uiSwapPaiingTips.SetHintSwapType(swapPaiHintStateData.swapPaiDirection);
                        uiSwapPaiingTips.Show();
                        swapPaiHintArrowEffect.ShowArrow(swapPaiHintStateData.swapPaiDirection);

                        swapPaiHintStateDataGroup.SetState(SwapPaiHintState.HINTTING, Time.time, 2f);
                    }
                    break;

                case SwapPaiHintState.HINTTING:
                    {
                        uiSwapPaiingTips.Hide();
                        swapPaiHintArrowEffect.HideArrow(swapPaiHintStateData.swapPaiDirection);
                        swapPaiHintStateDataGroup.state = SwapPaiHintState.HINT_END;
                    }
                    break;
            }
        }

        #endregion



    }
}
