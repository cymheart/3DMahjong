using ComponentDesgin;
using CoreDesgin;

namespace ActionDesgin
{
    public class MahjongPointerAction: MahjongMachineAction
    {
        private static MahjongPointerAction instance = null;
        public static MahjongPointerAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new MahjongPointerAction();
                return instance;
            }
        }

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
