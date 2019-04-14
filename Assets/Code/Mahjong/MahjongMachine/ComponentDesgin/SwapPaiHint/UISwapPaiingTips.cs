using CoreDesgin;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ComponentDesgin
{
    public class UISwapPaiingTips : MahjongMachineComponent
    {
        static string textOpp = "本局对家换牌";
        static string textClockWise = "本局顺时针换牌";
        static string textAnitClockWise = "本局逆时针换牌";

        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Transform uiCanvasTransform;
        Scene scene;

        GameObject prefabUISwapPaiingTips;

        Transform uiSwapPaiingTips;
        Transform textSwapType;

        SwapPaiDirection swapDir = SwapPaiDirection.OPPOSITE;

        string text;

        public override void Init()
        {
            base.Init();
            Setting((MahjongMachine)Parent);
        }
        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
            scene = mjMachine.GetComponent<Scene>();
            uiCanvasTransform = scene.uiCanvasTransform;

            prefabUISwapPaiingTips = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_SWAPPAI_TIPS][0];
        }

        public override void Load()
        {
            GameObject go = Object.Instantiate(prefabUISwapPaiingTips, uiCanvasTransform);
            go.SetActive(false);
            mjAssetsMgr.AppendToDestoryPool(go);

            uiSwapPaiingTips = go.transform;
            textSwapType = uiSwapPaiingTips.Find("TextSwapType");
        }

        public void Destory()
        {
            Object.Destroy(uiSwapPaiingTips.gameObject);
        }
        public void SetHintSwapType(SwapPaiDirection swapDir)
        {
            if (swapDir == this.swapDir)
                return;

            this.swapDir = swapDir;

            switch (swapDir)
            {
                case SwapPaiDirection.OPPOSITE:
                    text = textOpp;
                    break;

                case SwapPaiDirection.CLOCKWISE:
                    text = textClockWise;
                    break;

                case SwapPaiDirection.ANTICLOCKWISE:
                    text = textAnitClockWise;
                    break;
            }

            textSwapType.GetComponent<Text>().text = text;
        }

        public void Show()
        {
            uiSwapPaiingTips.gameObject.SetActive(true);
        }

        public void Hide()
        {
            uiSwapPaiingTips.gameObject.SetActive(false);
        }
    }
}
