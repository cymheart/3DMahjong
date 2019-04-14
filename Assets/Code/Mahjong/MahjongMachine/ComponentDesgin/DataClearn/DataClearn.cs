using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ComponentDesgin
{
    public class DataClearn : MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Desk desk;
        Scene scene;
        Fit fit;
        PreSettingHelper preSettingHelper;

        public override void Init()
        {
            Setting((MahjongMachine)Parent);
        }

        public void Setting(MahjongMachine mjMachine)
        {
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
            scene = mjMachine.GetComponent<Scene>();
            fit = mjMachine.GetComponent<Fit>();
            preSettingHelper = mjMachine.GetComponent<PreSettingHelper>();
        }

        public void ResetData()
        {
            for (int i = 0; i < 4; i++)
                mjMachine.playerStateData[0].Clear();

            fit.ClearData();
            preSettingHelper.ClearData();
            desk.ClearData();
        }

    }
}
