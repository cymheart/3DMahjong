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

        /// <summary>
        /// 麻将机状态数据组
        /// </summary>
        public StateDataGroup mjMachineStateData = new StateDataGroup();

        /// <summary>
        /// 玩家状态数据组
        /// </summary>
        public StateDataGroup[] playerStateData = new StateDataGroup[4]
        {
            new StateDataGroup(),
            new StateDataGroup(),
            new StateDataGroup(),
            new StateDataGroup()
        };

        int state = -1;
        int oldState = -1;

        public void Start()
        {
            mjCmdMgr.Init(this);
            oldState = state = 0;
            mjMachineUpdater.Reg("MjMachineStateCheck", StateCheck); 
        }

        public void SetWaitState()
        {
            if (state == 10)
                return;

            oldState = state;
            state = 10;
        }

        public void SetContinueState()
        {
            if (state == 10)
            {
                state = oldState + 1;
                oldState = state;
            }
        }

        public void StateCheck()
        {
            switch(state)
            {
                case 0:
                    Wake();
                    if (state == 0)
                    {
                        state++;
                        oldState = state;
                    }
                    break;

                case 1:
                    PreInit();

                    if (state == 1)
                    {
                        state++;
                        oldState = state;
                    }
                    break;

                case 2:
                    PreLoad();

                    if (state == 2)
                    {
                        state++;
                        oldState = state;
                    }
                    break;

                case 3:
                    Init();

                    if (state == 3)
                    {
                        state++;
                        oldState = state;
                    }
                    break;

                case 4:
                    Load();

                    if (state == 4)
                    {
                        state= -1;
                        oldState = -1;
                        mjMachineUpdater.UnReg("MjMachineStateCheck");
                    }
                    break;
            }
        }


        public void AppendMjOpCmd(MahjongMachineCmd cmd)
        {
            mjCmdMgr.Append(cmd);
        }
    }
}