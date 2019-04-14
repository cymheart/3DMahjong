using ComponentDesgin;
using CoreDesgin;

namespace CmdDesgin
{
    /// <summary>
    /// 麻将机手部动作命令
    /// </summary>
    public class BaseHandActionCmd : MahjongMachineHandActionCmd
    {
        public PlayerType handStyle;
        public BaseHandActionCmd()
            : base()
        {
        }
    }
}