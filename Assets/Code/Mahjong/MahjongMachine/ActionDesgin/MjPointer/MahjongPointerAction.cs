using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionDesgin
{
    public class MahjongPointerAction: MahjongMachineAction
    {
        public static MahjongPointerAction Instance { get; } = new MahjongPointerAction();
        MahjongPoint mahjongPoint;
        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);
            mahjongPoint = mjMachine.GetComponent<MahjongPoint>();
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("MahjongPointer", UpdateDapaiMjPointPos);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("MahjongPointer");
        }

        /// <summary>
        /// 更新打出麻将的指示位置
        /// </summary>
        public void UpdateDapaiMjPointPos()
        {
            Desk desk = mjMachine.GetComponent<Desk>();

            if (desk.lastDaPaiMj)
                mahjongPoint.Show(desk.lastDaPaiMj);
            else
                mahjongPoint.Hide();
        }

    }
}
