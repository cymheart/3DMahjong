using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ComponentDesgin
{
    public class UIHuPaiHandPaiIdxTipsArrow : MahjongMachineComponent
    {

        MahjongMachine mjMachine;
        Fit fit;
        MahjongAssetsMgr mjAssetsMgr;
        Desk desk;
        Transform uiCanvasTransform;
        List<GameObject> arrowList = new List<GameObject>();

        public override void Init()
        {
            base.Init();
            Setting((MahjongMachine)Parent);
        }

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            desk = mjMachine.GetComponent<Desk>();
            fit = mjMachine.GetComponent<Fit>();
        }


        public void Show(int[] huPaiInHandPaiIdxs, int[] huPaiInMoPaiIdxs)
        {
            Hide();

            if (huPaiInHandPaiIdxs != null)
            {
                List<GameObject> handPaiList = desk.GetHandPaiList(0);
                if (handPaiList != null)
                {
                    float h = fit.GetCanvasHandMjSizeByAxis(Axis.Y);
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
                List<GameObject> moPaiList = desk.GetMoPaiList(0);

                if (moPaiList != null)
                {
                    float h = fit.GetCanvasHandMjSizeByAxis(Axis.Y);
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

