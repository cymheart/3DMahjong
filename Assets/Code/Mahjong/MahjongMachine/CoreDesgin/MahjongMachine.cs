using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreDesgin
{
    public class MahjongMachine: MahjongMachineComponent
    { 
        public MahjongGame mjGame;

        public MahjongMachineUpdater mjMachineUpdater = new MahjongMachineUpdater();
        public MahjongMachineCmdMgr mjCmdMgr = new MahjongMachineCmdMgr();


        public void Start()
        {
            mjCmdMgr.Init(this);
            mjMachineUpdater.Reg("MjMachineStateCheck", StateCheck); 
        }

       
        public void StateCheck()
        {
            bool isEnd = Update();
            if (isEnd == true)
            {
                mjMachineUpdater.UnReg("MjMachineStateCheck");
            }
        }


        public void AppendMjOpCmd(MahjongMachineCmd cmd)
        {
            mjCmdMgr.Append(cmd);
        }
    }
}