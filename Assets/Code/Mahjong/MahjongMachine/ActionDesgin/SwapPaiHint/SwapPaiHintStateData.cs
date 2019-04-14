using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionDesgin
{
    public class SwapPaiHintStateData: ActionStateData
    {
        public const int HINT_START = 0;
        public const int HINTTING = 1;
        public const int HINT_END = 2;

        public SwapPaiDirection swapPaiDirection = SwapPaiDirection.CLOCKWISE;

        public void SetData(SwapPaiDirection swapPaiDirection)
        {
            this.swapPaiDirection = swapPaiDirection;
        }
    }
}
