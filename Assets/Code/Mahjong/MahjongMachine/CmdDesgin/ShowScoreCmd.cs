using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 显示分数
    /// </summary>
    public class ShowScoreCmd : MahjongMachineCmd
    {
        public int[] seatScores = new int[4]
        {
        0,0,0,0
        };

        UIScore uiScore;


        public ShowScoreCmd()
        {
            uiScore = mjMachine.GetComponent<UIScore>();
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            uiScore.Show(seatScores);
            cmdList.RemoveCmd(opCmdNode);
        }
    }
}
