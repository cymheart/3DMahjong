using System.Collections.Generic;
using UnityEngine;

namespace MahjongMachineNS
{
    public class UIHuPaiHandPaiIdxTipsArrow
    {

        MahjongMachine mjMachine;
        MahjongAssets mjAssets;
        MahjongAssetsMgr mjAssetsMgr;
        MahjongGame mjGame;
        Transform uiCanvasTransform;
        List<GameObject> arrowList = new List<GameObject>();

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjGame = mjMachine.mjGame;
            mjAssets = mjMachine.mjAssets;
            mjAssetsMgr = mjMachine.mjAssetsMgr;
            uiCanvasTransform = mjMachine.canvasHandPaiTransform;
        }

        public void Show(int[] huPaiInHandPaiIdxs, int[] huPaiInMoPaiIdxs)
        {
            Hide();

            if (huPaiInHandPaiIdxs != null)
            {
                List<GameObject> handPaiList = mjMachine.GetHandPaiList(0);
                if (handPaiList != null)
                {
                    float h = mjMachine.GetCanvasHandMjSizeByAxis(Axis.Y);
                    float offset = h / 2 + h / 6;
                    GameObject arrow;
                    Vector3 mjpos;

                    for (int i = 0; i < huPaiInHandPaiIdxs.Length; i++)
                    {
                        if (huPaiInHandPaiIdxs[i] >= handPaiList.Count)
                            break;

                        mjpos = handPaiList[huPaiInHandPaiIdxs[i]].transform.localPosition;
                        mjpos.y += offset;

                        arrow = mjAssetsMgr.huPaiInHandPaiTipsArrowPool.PopGameObject();
                        arrow.SetActive(true);
                        arrow.transform.localPosition = mjpos;
                        arrow.transform.SetParent(handPaiList[huPaiInHandPaiIdxs[i]].transform);
                        arrowList.Add(arrow);
                    }
                }
            }

            if (huPaiInMoPaiIdxs != null)
            {
                List<GameObject> moPaiList = mjMachine.GetMoPaiList(0);

                if (moPaiList != null)
                {
                    float h = mjMachine.GetCanvasHandMjSizeByAxis(Axis.Y);
                    float offset = h / 2 + h / 6;
                    GameObject arrow;
                    Vector3 mjpos;

                    for (int i = 0; i < huPaiInMoPaiIdxs.Length; i++)
                    {
                        if (huPaiInMoPaiIdxs[i] >= moPaiList.Count)
                            break;

                        mjpos = moPaiList[huPaiInMoPaiIdxs[i]].transform.localPosition;
                        mjpos.y += offset;

                        arrow = mjAssetsMgr.huPaiInHandPaiTipsArrowPool.PopGameObject();
                        arrow.SetActive(true);
                        arrow.transform.localPosition = mjpos;
                        arrow.transform.SetParent(moPaiList[huPaiInMoPaiIdxs[i]].transform);
                        arrowList.Add(arrow);
                    }
                }
            }
        }

        public void Hide()
        {
            for (int i = 0; i < arrowList.Count; i++)
            {
                mjAssetsMgr.huPaiInHandPaiTipsArrowPool.PushGameObject(arrowList[i]);
            }

            arrowList.Clear();
        }

    }
}


