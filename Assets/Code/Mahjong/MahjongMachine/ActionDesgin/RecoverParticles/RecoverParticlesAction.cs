using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionDesgin
{
    public class RecoverParticlesAction : MahjongMachineAction
    {
        public static RecoverParticlesAction Instance { get; } = new RecoverParticlesAction();
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
