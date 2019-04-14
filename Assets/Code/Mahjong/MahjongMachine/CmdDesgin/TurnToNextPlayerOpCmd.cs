using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 轮转玩家
    /// </summary>
    public class TurnToNextPlayerOpCmd : MahjongMachineCmd
    {
        public int[] waitActionEndPlayerSeatIdxs;
        public int turnToPlayerSeatIdx;
        public int time;

        public TurnToNextPlayerOpCmd()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        /// <summary>
        /// 判断是否可以轮转到下一个玩家
        /// </summary>
        /// <returns></returns>
        public override bool CanExecute()
        {
            LinkedList<MahjongMachineCmd>[] playerActionOpCmdLists = cmdList.playerActionOpCmdLists;

            if (playerActionOpCmdLists[turnToPlayerSeatIdx].Count != 0)
                return false;

            LinkedList<MahjongMachineCmd> cmdListForSeat;
            int waitActionEndSeatIdx;
            for (int i = 0; i < waitActionEndPlayerSeatIdxs.Length; i++)
            {
                waitActionEndSeatIdx = waitActionEndPlayerSeatIdxs[i];
                cmdListForSeat = playerActionOpCmdLists[waitActionEndSeatIdx];

                if (cmdListForSeat.Count != 0)
                {
                    for (LinkedListNode<MahjongMachineCmd> node = cmdListForSeat.First; node != null; node = node.Next)
                    {
                        Type daPaiType = typeof(MahjongDaPaiOpCmd);
                        Type curtType = node.Value.GetType();

                        if (curtType == daPaiType &&
                                mjMachine.playerStateData[waitActionEndSeatIdx].state < DaPaiStateData.DA_PAI_CHUPAI_TAIHAND)
                            return false;
                    }
                }
            }

            return true;
        }


        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            MahjongDiceMachine diceMachine = mjMachine.GetComponent<MahjongDiceMachine>();
            Fit fit = mjMachine.GetComponent<Fit>();

            FengWei fw = fit.GetSeatFengWei(turnToPlayerSeatIdx);
            diceMachine.OnFengWei(fw);
            diceMachine.SetLimitTime(time);
            diceMachine.StartTime();
            cmdList.RemoveCmd(opCmdNode);
        }
    }

}
