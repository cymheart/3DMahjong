using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ComponentDesgin
{
    public class UIChiPaiTips : MahjongMachineComponent
    {
        static string[] mjposNames = { "mjpos1", "mjpos2", "mjpos3" };

        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Scene scene;
        Transform uiCanvasTransform;

        GameObject prefabUiPaiTips;
        GameObject prefabUiPaiDetailTips;

        GameObject paiTips;
        RectTransform paiTipsRectTransform;

        Vector2 paiTipsOrgSize;
        Vector2 detailTipsSize;

        public float spacing = 20f;

        List<GameObject> deatilTips = new List<GameObject>();
        List<GameObject> deatilTipsMj = new List<GameObject>();

        List<MahjongFaceValue[]> tipsInfos = null;

        public int selectedIdx = -1;

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

            prefabUiPaiTips = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_CHIPAI_TIPS][0];
            paiTipsOrgSize = prefabUiPaiTips.GetComponent<RectTransform>().sizeDelta;

            prefabUiPaiDetailTips = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_CHIPAI_DETAIL_TIPS][0];
            detailTipsSize = prefabUiPaiDetailTips.GetComponent<RectTransform>().sizeDelta;
        }

        public override void Load()
        {
            paiTips = Object.Instantiate(prefabUiPaiTips, uiCanvasTransform);
            paiTipsRectTransform = paiTips.GetComponent<RectTransform>();
            paiTips.SetActive(false);

            mjAssetsMgr.AppendToDestoryPool(paiTips);
        }

        public void Destory()
        {
            Object.Destroy(paiTips);
        }

        /// <summary>
        /// 为手牌设置吃牌麻将提示
        /// </summary>
        /// <param name="paiType"></param>
        /// <param name="huPaiTipsInfos"></param>
        public void SetChiPaiInfo(List<MahjongFaceValue[]> tipsInfos)
        {
            this.tipsInfos = tipsInfos;

            RemoveAllDetailTips();

            if (tipsInfos == null)
                return;

            GameObject huPaiDetailTips;
            float curtpos = 0;


            float width = paiTipsOrgSize.x + (tipsInfos.Count - 1) * detailTipsSize.x + (tipsInfos.Count - 2) * spacing;
            paiTipsRectTransform.sizeDelta = new Vector2(width, paiTips.GetComponent<RectTransform>().sizeDelta.y);
            Vector3 pos = paiTipsRectTransform.localPosition;
            paiTipsRectTransform.localPosition = new Vector3(0, pos.y, pos.z);

            float xpos = -paiTipsRectTransform.sizeDelta.x / 2 + mjAssetsMgr.huPaiTipsAndDetailOffsetX;
            huPaiDetailTips = AddDetailTips(tipsInfos[0], 0);
            RectTransform rectTransform = huPaiDetailTips.GetComponent<RectTransform>();
            pos = rectTransform.localPosition;
            rectTransform.localPosition = new Vector3(xpos, pos.y, pos.z);
            pos = rectTransform.localPosition;

            curtpos = pos.x + detailTipsSize.x + spacing;


            for (int i = 1; i < tipsInfos.Count; i++)
            {
                huPaiDetailTips = AddDetailTips(tipsInfos[i], i);
                rectTransform = huPaiDetailTips.GetComponent<RectTransform>();

                pos = rectTransform.localPosition;
                pos.x = curtpos;
                rectTransform.localPosition = pos;
                curtpos += rectTransform.sizeDelta.x + spacing;
            }
        }

        GameObject AddDetailTips(MahjongFaceValue[] tipsInfo, int idx)
        {
            //ui
            GameObject paiDetailTips = mjAssetsMgr.chiPaiDetailTipsPool.PopGameObject();
            paiDetailTips.transform.SetParent(paiTips.transform);
            paiDetailTips.SetActive(true);

            PaiDetailTipsScript paiDetailTipsScript = paiDetailTips.GetComponent<PaiDetailTipsScript>();
            if (paiDetailTipsScript == null)
                paiDetailTipsScript = paiDetailTips.AddComponent<PaiDetailTipsScript>();

            paiDetailTipsScript.idx = idx;

            EventTriggerListener.Get(paiDetailTips).onClick = OnClick;

            for (int i = 0; i < tipsInfo.Length; i++)
            {
                //麻将
                GameObject mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(tipsInfo[i]);
                mj.SetActive(true);
                mj.layer = mjMachine.uiLayer;
                Transform mjpos = paiDetailTips.transform.Find(mjposNames[i]);
                mj.transform.SetParent(mjpos);
                mj.transform.localPosition = new Vector3(0, 0, 0);
                mj.transform.localEulerAngles = new Vector3(0, 0, 0);
                mj.transform.localScale = new Vector3(1, 1, 1);

                deatilTipsMj.Add(mj);
            }


            deatilTips.Add(paiDetailTips);
            return paiDetailTips;
        }

        public void RemoveAllDetailTips()
        {
            for (int i = 0; i < deatilTips.Count; i++)
            {
                mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(deatilTipsMj[i]);
                mjAssetsMgr.huPaiDetailTipsPool.PushGameObject(deatilTips[i]);
            }
            deatilTipsMj.Clear();
            deatilTips.Clear();
        }

        public void Show(List<MahjongFaceValue[]> tipsInfos = null)
        {
            paiTips.SetActive(true);
            paiTips.transform.SetAsLastSibling();

            if (tipsInfos != null)
                SetChiPaiInfo(tipsInfos);

            selectedIdx = -1;
        }

        public void Hide()
        {
            selectedIdx = -1;
            paiTips.SetActive(false);
        }

        private void OnClick(GameObject go)
        {
            selectedIdx = go.GetComponent<PaiDetailTipsScript>().idx;
            paiTips.SetActive(false);
        }

        class PaiDetailTipsScript : MonoBehaviour
        {
            public int idx = -1;
        }

    }
}


