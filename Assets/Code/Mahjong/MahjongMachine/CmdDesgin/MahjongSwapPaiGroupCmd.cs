using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 多人交换麻将牌
    /// </summary>
    public class MahjongSwapPaiGroupCmd : MahjongMachineHandActionCmd
    {
        public MahjongSwapPaiCmd[] cmdSeats = new MahjongSwapPaiCmd[4];

        public int[] SwapHandPaiIdx
        {
            set
            {
                cmdSeats[0].fromSeatHandPaiIdx = value;
            }
        }

        public int[] SwapMoPaiIdx
        {
            set
            {
                cmdSeats[0].fromSeatMoPaiIdx = value;
            }
        }

        public MahjongFaceValue[] TakeMjFaceValues
        {
            set
            {
                takeMjFaceValues = value;

                if (swapDir == SwapPaiDirection.OPPOSITE)
                {
                    cmdSeats[2].mjFaceValues = takeMjFaceValues;
                    swapMjCount = cmdSeats[2].mjFaceValues.Length;
                }
                else if (swapDir == SwapPaiDirection.CLOCKWISE)
                {
                    cmdSeats[3].mjFaceValues = takeMjFaceValues;
                    swapMjCount = cmdSeats[3].mjFaceValues.Length;
                }
                else
                {
                    cmdSeats[1].mjFaceValues = takeMjFaceValues;
                    swapMjCount = cmdSeats[1].mjFaceValues.Length;
                }
            }
        }

        public int SwapMjCount
        {
            get
            {
                return swapMjCount;
            }
        }

        public SwapPaiDirection SwapDirection
        {
            set
            {
                swapDir = value;

                if (takeMjFaceValues != null)
                    TakeMjFaceValues = takeMjFaceValues;

                for (int i = 0; i < 4; i++)
                {
                    cmdSeats[i].swapDir = swapDir;
                }

                switch (swapDir)
                {
                    case SwapPaiDirection.CLOCKWISE:
                        cmdSeats[0].seatIdx = 0;
                        cmdSeats[0].toSeatIdx = 1;

                        cmdSeats[1].seatIdx = 1;
                        cmdSeats[1].toSeatIdx = 2;

                        cmdSeats[2].seatIdx = 2;
                        cmdSeats[2].toSeatIdx = 3;

                        cmdSeats[3].seatIdx = 3;
                        cmdSeats[3].toSeatIdx = 0;
                        break;

                    case SwapPaiDirection.ANTICLOCKWISE:
                        cmdSeats[0].seatIdx = 0;
                        cmdSeats[0].toSeatIdx = 3;

                        cmdSeats[3].seatIdx = 3;
                        cmdSeats[3].toSeatIdx = 2;

                        cmdSeats[2].seatIdx = 2;
                        cmdSeats[2].toSeatIdx = 1;

                        cmdSeats[1].seatIdx = 1;
                        cmdSeats[1].toSeatIdx = 0;
                        break;


                    case SwapPaiDirection.OPPOSITE:
                        cmdSeats[0].seatIdx = 0;
                        cmdSeats[0].toSeatIdx = 2;

                        cmdSeats[3].seatIdx = 3;
                        cmdSeats[3].toSeatIdx = 1;

                        cmdSeats[2].seatIdx = 2;
                        cmdSeats[2].toSeatIdx = 0;

                        cmdSeats[1].seatIdx = 1;
                        cmdSeats[1].toSeatIdx = 3;
                        break;
                }

            }
        }

        MahjongFaceValue[] takeMjFaceValues = null;
        int swapMjCount = 0;
        SwapPaiDirection swapDir = SwapPaiDirection.CLOCKWISE;

        public MahjongSwapPaiGroupCmd()
            : base()
        {
            AppendProcessFunc = AppendProcess;

            for (int i = 0; i < 4; i++)
            {
                cmdSeats[i] = MjMachineCmdPool.Instance.CreateCmd<MahjongSwapPaiCmd>();
            }
        }

        LinkedListNode<MahjongMachineCmd>[] AppendProcess()
        {
            LinkedListNode<MahjongMachineCmd>[] cmdNodes = new LinkedListNode<MahjongMachineCmd>[4];

            for (int i = 0; i < 4; i++)
            {
                cmdSeats[i].isBlock = isBlock;
                cmdSeats[i].swapMjCount = SwapMjCount;
                cmdSeats[i].delayExecuteTime = delayExecuteTime;
                cmdNodes[i] = MjMachineCmdPool.Instance.CreateCmdNode(cmdSeats[i]);
            }

            MjMachineCmdPool.Instance.ReleaseCmd(this);
            return cmdNodes;
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }
    }
}
