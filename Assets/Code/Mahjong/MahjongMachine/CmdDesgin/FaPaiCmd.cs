using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 发牌命令
    /// </summary>
    public class FaPaiCmd : MahjongMachineCmd
    {
        public int startPaiIdx = 0;
        public List<MahjongFaceValue> mjHandSelfPaiFaceValueList = new List<MahjongFaceValue>();
        public List<MahjongFaceValue> selfHuaList = new List<MahjongFaceValue>();
        public List<MahjongFaceValue> selfBuPaiList = new List<MahjongFaceValue>();

        public FaPaiCmd()
        {
        }
        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            FaPaiAction.Instance.FaPai(startPaiIdx, mjHandSelfPaiFaceValueList, selfHuaList, selfBuPaiList, opCmdNode);
        }
    }
}
