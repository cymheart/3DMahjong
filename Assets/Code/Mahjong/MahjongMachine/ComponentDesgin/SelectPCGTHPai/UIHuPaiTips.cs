using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ComponentDesgin
{
    public struct HuPaiTipsInfo
    {
        public MahjongFaceValue faceValue;
        public int fanAmount;
        public int zhangAmount;
    }
    public class UIHuPaiTips : MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Scene scene;
        Transform uiCanvasTransform;

        GameObject prefabUiHuPaiTips;
        GameObject prefabUiHuPaiDetailTips;

        GameObject huPaiTips;
        RectTransform huPaiTipsRectTransform;
        Transform huPaiTipsContent;

        Vector2 huPaiTipsOrgSize;
        Vector2 detailTipsSize;

        Vector3 huPaiTipsOrgPos;

        GridLayoutGroup gridLayoutGroup;

        public float spacingx = 15f;
        public float spacingy = 15f;

        List<GameObject> deatilTips = new List<GameObject>();
        List<GameObject> deatilTipsMj = new List<GameObject>();

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

            prefabUiHuPaiTips = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_HUPAI_TIPS][0];
            huPaiTipsOrgSize = prefabUiHuPaiTips.GetComponent<RectTransform>().sizeDelta;
            huPaiTipsOrgPos = prefabUiHuPaiTips.GetComponent<RectTransform>().localPosition;

            prefabUiHuPaiDetailTips = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_HUPAI_DETAIL_TIPS][0];
            detailTipsSize = prefabUiHuPaiDetailTips.GetComponent<RectTransform>().sizeDelta;
        }

        public override void Load()
        {
            huPaiTips = Object.Instantiate(prefabUiHuPaiTips, uiCanvasTransform);
            huPaiTipsRectTransform = huPaiTips.GetComponent<RectTransform>();
            huPaiTipsContent = huPaiTips.transform.Find("Scroll View").Find("Viewport").Find("Content");
            gridLayoutGroup = huPaiTipsContent.GetComponent<GridLayoutGroup>();
            spacingx = gridLayoutGroup.spacing.x;
            spacingy = gridLayoutGroup.spacing.y;

            huPaiTips.SetActive(false);

            mjAssetsMgr.AppendToDestoryPool(huPaiTips);
        }

        public void Destory()
        {
            Object.Destroy(huPaiTips);
        }

        /// <summary>
        /// 为手牌设置胡牌麻将提示
        /// </summary>
        /// <param name="paiType"></param>
        /// <param name="huPaiTipsInfos"></param>
        public void SetHuPaiInfo(HuPaiTipsInfo[] huPaiTipsInfos)
        {
            RemoveAllDetailTips();

            if (huPaiTipsInfos == null)
                return;

            GameObject huPaiDetailTips;
            int colCount = 6;

            int count = Mathf.Min(colCount, huPaiTipsInfos.Length);
            gridLayoutGroup.constraintCount = count;

            float width = huPaiTipsOrgSize.x + (count - 1) * detailTipsSize.x + (count - 1) * spacingx;
            int hCount = (huPaiTipsInfos.Length - 1) / colCount;
            float height = huPaiTipsOrgSize.y + hCount * (detailTipsSize.y + spacingy);

            huPaiTipsRectTransform.sizeDelta = new Vector2(width, height);

            Vector3 pos = huPaiTipsOrgPos;
            pos.y -= huPaiTipsOrgSize.y / 2;
            pos.y += height / 2;
            huPaiTipsRectTransform.localPosition = pos;

            for (int i = 0; i < huPaiTipsInfos.Length; i++)
            {
                huPaiDetailTips = AddDetailTips(huPaiTipsInfos[i]);
            }
        }

        GameObject AddDetailTips(HuPaiTipsInfo huPaiTipsInfo)
        {
            //ui
            GameObject huPaiDetailTips = mjAssetsMgr.huPaiDetailTipsPool.PopGameObject();
            huPaiDetailTips.transform.SetParent(huPaiTipsContent);
            huPaiDetailTips.SetActive(true);

            //麻将
            GameObject mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(huPaiTipsInfo.faceValue);
            mj.SetActive(true);
            mj.layer = mjMachine.uiLayer;
            Transform mjpos = huPaiDetailTips.transform.Find("mjpos");
            mj.transform.SetParent(mjpos);
            mj.transform.localPosition = new Vector3(0, 0, 0);
            mj.transform.localEulerAngles = new Vector3(0, 0, 0);
            mj.transform.localScale = new Vector3(1, 1, 1);

            Transform fanCountTransform = huPaiDetailTips.transform.Find("fanCount");
            fanCountTransform.GetComponent<Text>().text = huPaiTipsInfo.fanAmount.ToString();

            Transform zhangCountTransform = huPaiDetailTips.transform.Find("zhangCount");
            zhangCountTransform.GetComponent<Text>().text = huPaiTipsInfo.zhangAmount.ToString();


            deatilTips.Add(huPaiDetailTips);
            deatilTipsMj.Add(mj);

            return huPaiDetailTips;
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

        public void Show(HuPaiTipsInfo[] huPaiTipsInfos = null)
        {
            huPaiTips.SetActive(true);
            huPaiTips.transform.SetAsLastSibling();

            if (huPaiTipsInfos != null)
                SetHuPaiInfo(huPaiTipsInfos);

        }

        public void Hide()
        {
            huPaiTips.SetActive(false);
        }

    }
}
