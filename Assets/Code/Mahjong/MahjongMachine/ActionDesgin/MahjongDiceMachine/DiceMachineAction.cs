using ComponentDesgin;
using CoreDesgin;

namespace ActionDesgin
{
    public class DiceMachineAction : MahjongMachineAction
    {
        private static DiceMachineAction instance = null;
        public static DiceMachineAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new DiceMachineAction();
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
            mjMachineUpdater.Reg("DiceMachine", Update);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("DiceMachine");
        }

        /// <summary>
        /// 更新骰子机状态
        /// </summary>
        public void Update()
        {
            diceMachine.Update();
        }

    }
}
