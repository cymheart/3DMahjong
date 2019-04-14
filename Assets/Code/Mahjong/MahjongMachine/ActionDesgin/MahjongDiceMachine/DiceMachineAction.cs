using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionDesgin
{
    public class DiceMachineAction : MahjongMachineAction
    {
        public static DiceMachineAction Instance { get; } = new DiceMachineAction();
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
