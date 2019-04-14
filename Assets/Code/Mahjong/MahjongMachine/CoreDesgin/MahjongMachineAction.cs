namespace CoreDesgin
{
    public class MahjongMachineAction
    {
        public MahjongMachine mjMachine;
        public StateDataGroup[] playerStateData;
        public MahjongMachineCmdMgr mjCmdMgr;
        public MahjongMachineUpdater mjMachineUpdater;


        public MahjongMachineAction()
        {
        
        }

        public virtual void Init(MahjongMachine mjMachine)
        {
            if (mjMachine == null)
                return;

            this.mjMachine = mjMachine;
            playerStateData = mjMachine.playerStateData;
            mjCmdMgr = mjMachine.mjCmdMgr;
            mjMachineUpdater = mjMachine.mjMachineUpdater;
        }

        /// <summary>
        /// 动作安装
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mjMachine"></param>
        public static void Install<T>(T action, MahjongMachine mjMachine) where T : MahjongMachineAction
        {   
            action.Init(mjMachine);
            action.Install();
        }

        /// <summary>
        /// 动作安装
        /// </summary>
        public virtual void Install()
        {
        }

        /// <summary>
        /// 动作卸载
        /// </summary>
        public virtual void UnInstall()
        {
           
        }
    }
}
