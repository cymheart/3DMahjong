using ComponentDesgin;

namespace ActionDesgin
{
    public class SwapPaiHintStateData: ActionStateData
    {
        public SwapPaiDirection swapPaiDirection = SwapPaiDirection.CLOCKWISE;

        public void SetData(SwapPaiDirection swapPaiDirection)
        {
            this.swapPaiDirection = swapPaiDirection;
        }
    }
}
