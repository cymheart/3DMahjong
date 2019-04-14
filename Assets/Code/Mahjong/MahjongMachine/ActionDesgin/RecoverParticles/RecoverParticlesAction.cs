using ComponentDesgin;
using CoreDesgin;

namespace ActionDesgin
{
    public class RecoverParticlesAction : MahjongMachineAction
    {

        private static RecoverParticlesAction instance = null;
        public static RecoverParticlesAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new RecoverParticlesAction();
                return instance;
            }
        }

        MahjongAssetsMgr mjAssetsMgr;
        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("RecoverParticles", mjAssetsMgr.RecoverParticles);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("RecoverParticles");
        }
    }
}
