using DG.Tweening;
using CoreDesgin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace ComponentDesgin
{
    public class MahjongAssetsMgr: MahjongMachineComponent
    {
        public delegate void LoadMahjongPoolsCompletedDelegate();

        /// <summary>
        /// 载入麻将池资源完成回调
        /// </summary>
        public LoadMahjongPoolsCompletedDelegate LoadMahjongPoolsCompleted = null;

        MahjongMachine mjMachine;
        MahjongGame mjGame;
        Scene scene;
        Desk desk;
        SettingDataAssetsMgr settingDataAssetsMgr;
  
        public Dictionary<int, GameObject[]> defaultPrefabDict;
        public Dictionary<int, GameObject[]> effectPrefabDict;
        public Dictionary<int, GameObject[]> mjpaiPrefabDict;
        public Dictionary<int, GameObject[]> uiPrefabDict;


        public GameObjectPool longjuanfengEffectPool = new GameObjectPool();
        public GameObjectPool rainEffectPool = new GameObjectPool();

        public GameObjectPool pcgPaiParticlePool = new GameObjectPool();
        public GameObjectPool huPaiGetMjParticlePool = new GameObjectPool();
        public GameObjectPool huPaiShanDianParticlePool = new GameObjectPool();

        public GameObjectPool huPaiTextParticlePool = new GameObjectPool();
        public GameObjectPool chiPaiTextParticlePool = new GameObjectPool();
        public GameObjectPool pengPaiTextParticlePool = new GameObjectPool();
        public GameObjectPool gangPaiTextParticlePool = new GameObjectPool();
        public GameObjectPool tingPaiTextParticlePool = new GameObjectPool();
        public GameObjectPool zimoPaiTextParticlePool = new GameObjectPool();

        public GameObjectPool touxiangHeadParticlePool = new GameObjectPool();
        public GameObjectPool touxiangParticlePool = new GameObjectPool();

        public GameObjectPool huPaiDetailTipsPool = new GameObjectPool();
        public GameObjectPool huPaiInHandPaiTipsArrowPool = new GameObjectPool();

        public GameObjectPool chiPaiDetailTipsPool = new GameObjectPool();

        public GameObject[] handShadowPlanes = new GameObject[4];

        List<Tweener> destoryTweenerPool = new List<Tweener>();
        List<GameObject> destoryGameObjectPool = new List<GameObject>();


        public GameObject[] mjPai;
        public GameObject emptyMjPai;

        public List<GameObject> otherHandPaiMjPool = new List<GameObject>();
        int otherHandPaiMjPoolIdx = 0;

        public List<List<GameObject>> deskOrSelfHandPaiMjPool = new List<List<GameObject>>();

        public GameObject[] pengChiGangTingHuBtns = new GameObject[(int)PengChiGangTingHuType.CANCEL + 1];

        public Vector3 premjSize;
        public Vector3 shadowBackSidePos;
        bool flag = true;
        public float huPaiTipsAndDetailOffsetX = 0;

        public Dictionary<string, Dictionary<string, ActionDataInfo>> handActionDataInfoDicts;
        public List<ScreenFitInfo> screenFitInfoList;

        public Material mjHighLightFaceMat;
        public Material mjNormalFaceMat;

        AssetBundle ab;

        public override void Wake()
        {
            base.Wake();

            mjMachine = (MahjongMachine)Parent;
            mjGame = mjMachine.mjGame;
            settingDataAssetsMgr = mjMachine.GetComponent<SettingDataAssetsMgr>();

            mjGame.StartCoroutine(LoadMahjongRes());
            mjMachine.SetWaitState();
        }

        public override void PreLoad()
        {
            base.PreLoad();
            LoadPools();
        }

        public IEnumerator LoadMahjongRes()
        {
            string path = "file://" + Application.dataPath + "/StreamingAssets" + "/majiang.ab";
            //string path = "jar:file://" + Application.dataPath + "!/assets/majiang.ab";
            WWW bundle = new WWW(path);
            yield return bundle;
            ab = bundle.assetBundle;
            
            CreateMahjongRes();

            mjMachine.SetContinueState(); 
        }



        public void CreateMahjongRes()
        {
            settingDataAssetsMgr.AssetBundle = ab;
            settingDataAssetsMgr.ParseSettingData(new string[] { "AudioSettingData", "SpriteSettingData", "PrefabSettingData" });
            settingDataAssetsMgr.ParseHandActionSpeedData("HandActionSpeed");
            settingDataAssetsMgr.ParseDefineData(new string[] { "ScreenFitSettingData" });
            settingDataAssetsMgr.CreateScreenFitInfo();


            defaultPrefabDict = settingDataAssetsMgr.GetPrefabsIntKeyDict((int)PrefabsType.DEFAULT);
            effectPrefabDict = settingDataAssetsMgr.GetPrefabsIntKeyDict((int)PrefabsType.EFFECT);
            mjpaiPrefabDict = settingDataAssetsMgr.GetPrefabsIntKeyDict((int)PrefabsType.MJPAI);
            uiPrefabDict = settingDataAssetsMgr.GetPrefabsIntKeyDict((int)PrefabsType.UI);
            screenFitInfoList = settingDataAssetsMgr.GetScreenFitInfoList();

            handActionDataInfoDicts = settingDataAssetsMgr.GetHandActionDataInfoDicts();

            mjHighLightFaceMat = ab.LoadAsset<Material>("mjFaceMat2");
            mjNormalFaceMat = ab.LoadAsset<Material>("mjFaceMat");

            CreateMahjongPai();

            ab.Unload(false);
        }

        public void LoadPools()
        {
            longjuanfengEffectPool.CreatePool(effectPrefabDict[(int)PrefabIdx.LONGJUANFENG_EFFECT][0], 4);
            rainEffectPool.CreatePool(effectPrefabDict[(int)PrefabIdx.RAIN_EFFECT][0], 4);


            pcgPaiParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.PENGPAI_DUST_EFFECT][0], 8);
            huPaiGetMjParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.HUPAI_GET_MJ_EFFECT][0], 3);
            huPaiShanDianParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.HUPAI_SHAN_DIAN_EFFECT][0], 3);


            huPaiTextParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.HUPAI_TEXT_EFFECT][0], 4, scene.uiCanvasTransform);
            chiPaiTextParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.CHIPAI_TEXT_EFFECT][0], 4, scene.uiCanvasTransform);
            pengPaiTextParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.PENGPAI_TEXT_EFFECT][0], 4, scene.uiCanvasTransform);
            gangPaiTextParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.GANGPAI_TEXT_EFFECT][0], 4, scene.uiCanvasTransform);
            tingPaiTextParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.TINGPAI_TEXT_EFFECT][0], 4, scene.uiCanvasTransform);
            zimoPaiTextParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.ZIMO_TEXT_EFFECT][0], 4, scene.uiCanvasTransform);


            touxiangHeadParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.TOUXIANG_HEAD_FIRE_EFFECT][0], 4, scene.uiCanvasTransform);
            touxiangParticlePool.CreatePool(effectPrefabDict[(int)PrefabIdx.TOUXIANG_FIRE_EFFECT][0], 15, scene.uiCanvasTransform);


            huPaiDetailTipsPool.CreatePool(uiPrefabDict[(int)PrefabIdx.UI_HUPAI_DETAIL_TIPS][0], 10, scene.uiCanvasTransform);
            chiPaiDetailTipsPool.CreatePool(uiPrefabDict[(int)PrefabIdx.UI_CHIPAI_DETAIL_TIPS][0], 10, scene.uiCanvasTransform);

            GameObject huPaiTips = uiPrefabDict[(int)PrefabIdx.UI_HUPAI_TIPS][0];
            GameObject huPaiDetailTips = uiPrefabDict[(int)PrefabIdx.UI_HUPAI_DETAIL_TIPS][0];
            float x1 = huPaiTips.GetComponent<RectTransform>().localPosition.x - huPaiTips.GetComponent<RectTransform>().sizeDelta.x / 2;
            float x2 = huPaiDetailTips.GetComponent<RectTransform>().localPosition.x;
            huPaiTipsAndDetailOffsetX = x2 - x1;


            huPaiInHandPaiTipsArrowPool.CreatePool(uiPrefabDict[(int)PrefabIdx.UI_HUPAI_INHANDPAI_TIPS_ARROW][0], 20, scene.canvasHandPaiTransform);

            CreateHandShadowPlanes();    
            CreateDeskOrSelfHandMjPool();
            CreateOtherHandMjPool();
            CreatePengChiGangTingHuBtns();

            if (LoadMahjongPoolsCompleted != null)
                LoadMahjongPoolsCompleted();

        }

        void CreatePengChiGangTingHuBtns()
        {
            int prefabIdx = (int)PrefabIdx.UI_PENG_BTN;
            GameObject prefabGo;

            for (int i = (int)PengChiGangTingHuType.PENG; i <= (int)PengChiGangTingHuType.CANCEL; i++)
            {
                prefabIdx = GetPrefabIdxForPengChiGangTingHuType((PengChiGangTingHuType)i);
                prefabGo = uiPrefabDict[prefabIdx][0];
                pengChiGangTingHuBtns[i] = Object.Instantiate(prefabGo, scene.uiCanvasTransform);
                pengChiGangTingHuBtns[i].SetActive(false);
            }
        }

        int GetPrefabIdxForPengChiGangTingHuType(PengChiGangTingHuType type)
        {
            int prefabIdx = (int)PrefabIdx.UI_PENG_BTN;

            switch (type)
            {
                case PengChiGangTingHuType.PENG: prefabIdx = (int)PrefabIdx.UI_PENG_BTN; break;
                case PengChiGangTingHuType.CHI: prefabIdx = (int)PrefabIdx.UI_CHI_BTN; break;
                case PengChiGangTingHuType.GANG: prefabIdx = (int)PrefabIdx.UI_GANG_BTN; break;
                case PengChiGangTingHuType.TING: prefabIdx = (int)PrefabIdx.UI_TING_BTN; break;
                case PengChiGangTingHuType.HU: prefabIdx = (int)PrefabIdx.UI_HU_BTN; break;
                case PengChiGangTingHuType.GUO: prefabIdx = (int)PrefabIdx.UI_GUO_BTN; break;
                case PengChiGangTingHuType.CANCEL: prefabIdx = (int)PrefabIdx.UI_CANCEL_BTN; break;
            }

            return prefabIdx;
        }

        void CreateHandShadowPlanes()
        {
            for (int i = 0; i < 4; i++)
            {
                handShadowPlanes[i] = Object.Instantiate(defaultPrefabDict[(int)PrefabIdx.HAND_SHADOW_PLANE][0]);
                handShadowPlanes[i].SetActive(false);
            }
        }

        void CreateMahjongPai()
        {
            mjPai = new GameObject[mjpaiPrefabDict.Count];

            foreach (var item in mjpaiPrefabDict)
            {
                mjPai[item.Key] = item.Value[0];
            }

            emptyMjPai = mjPai[(int)MahjongFaceValue.MJ_ZFB_FACAI];  //ab.LoadAsset<GameObject>("mj_empty_pai");
            premjSize = emptyMjPai.transform.GetComponent<Renderer>().bounds.size;
        }

        void CreateDeskOrSelfHandMjPool()
        {
            for (int i = 0; i <= (int)MahjongFaceValue.MJ_UNKNOWN; i++)
            {
                GameObject premj;

                if (i == (int)MahjongFaceValue.MJ_UNKNOWN)
                    premj = mjPai[(int)MahjongFaceValue.MJ_ZFB_FACAI];
                else
                    premj = mjPai[i];

                GameObject mj;
                int count = 10;

                if (i >= (int)MahjongFaceValue.MJ_HUA_CHUN &&
                    i <= (int)MahjongFaceValue.MJ_HUA_JU)
                {
                    count = 10;
                }

                deskOrSelfHandPaiMjPool.Add(new List<GameObject>());

                for (int j = 0; j < count; j++)
                {
                    mj = CreateDeskOrSelfHandMj(premj, (MahjongFaceValue)i);
                    deskOrSelfHandPaiMjPool[i].Add(mj);
                    // Object.DontDestroyOnLoad(mj);
                }
            }
        }

        GameObject CreateDeskOrSelfHandMj(GameObject premj, MahjongFaceValue mjFaceValue)
        {
            GameObject mj = Object.Instantiate(premj);
            BoxCollider c = mj.AddComponent<BoxCollider>();
            c.enabled = false;

            MjPaiData mjPaiData = mj.AddComponent<MjPaiData>();
            mjPaiData.mjFaceValue = mjFaceValue;

            GameObject shadow = Object.Instantiate(effectPrefabDict[(int)PrefabIdx.MJ_SHADOW][0]);
            shadow.name = "shadow";
            Transform tf = shadow.transform;
            tf.SetParent(mj.transform, true);

            if (flag)
            {
                shadowBackSidePos = new Vector3(tf.localPosition.x, tf.localPosition.y, tf.localPosition.z + premjSize.z);
                flag = false;
            }

            shadow.SetActive(false);

            GameObject shadowSmall = Object.Instantiate(effectPrefabDict[(int)PrefabIdx.MJ_SHADOW_SMALL][0]);
            shadowSmall.name = "shadowSmall";
            shadowSmall.transform.SetParent(mj.transform, true);
            shadowSmall.SetActive(false);

            mj.SetActive(false);

            return mj;
        }

        public GameObject PopMjFromDeskOrSelfHandMjPool(MahjongFaceValue mjFaceValue)
        {
            if (mjFaceValue >= MahjongFaceValue.MJ_UNKNOWN || mjFaceValue < 0)
                return null;

            List<GameObject> mjList = deskOrSelfHandPaiMjPool[(int)mjFaceValue];

            for (int i = 0; i < mjList.Count; ++i)
            {
                if (!mjList[i].activeInHierarchy)
                {
                    return mjList[i];
                }
            }


            GameObject premj;
            if (mjFaceValue == MahjongFaceValue.MJ_UNKNOWN)
                premj = mjPai[(int)MahjongFaceValue.MJ_ZFB_FACAI];
            else
                premj = mjPai[(int)mjFaceValue];

            GameObject mj = CreateDeskOrSelfHandMj(premj, mjFaceValue);
            //Object.DontDestroyOnLoad(mj);
            mjList.Add(mj);
            return mj;
        }

        public void PushMjToDeskOrSelfHandMjPool(GameObject mj)
        {
            mj.transform.SetParent(null, true);
            mj.transform.localScale = Vector3.one;

            Transform shadow = mj.transform.GetChild(0);

            if (shadow.localPosition.z > 0)
                shadow.localPosition = new Vector3(shadow.localPosition.x, shadow.localPosition.y, shadow.localPosition.z - premjSize.z);

            mj.GetComponent<BoxCollider>().enabled = false;
            mj.SetActive(false);
            mj.transform.GetChild(0).gameObject.SetActive(false);
            mj.transform.GetChild(1).gameObject.SetActive(false);
        }

        void CreateOtherHandMjPool()
        {
            GameObject premj = emptyMjPai;
            GameObject mj;

            for (int i = 0; i < 50; i++)
            {
                mj = CreateOtherHandMj(premj);
                // Object.DontDestroyOnLoad(mj);
                otherHandPaiMjPool.Add(mj);
            }
        }

        GameObject CreateOtherHandMj(GameObject premj)
        {
            GameObject mj = Object.Instantiate(premj);
            GameObject shadowSmall = Object.Instantiate(effectPrefabDict[(int)PrefabIdx.MJ_SHADOW_SMALL][0]);
            shadowSmall.name = "shadowSmall";
            shadowSmall.transform.SetParent(mj.transform, true);
            shadowSmall.SetActive(false);

            mj.SetActive(false);

            return mj;
        }

        public GameObject PopMjFromOtherHandMjPool()
        {
            for (int i = 0; i < otherHandPaiMjPool.Count; ++i)   //把对象池遍历一遍
            {
                int temI = (otherHandPaiMjPoolIdx + i) % otherHandPaiMjPool.Count;
                if (!otherHandPaiMjPool[temI].activeInHierarchy)
                {
                    otherHandPaiMjPoolIdx = (temI + 1) % otherHandPaiMjPool.Count;
                    return otherHandPaiMjPool[temI];
                }
            }

            GameObject mj = CreateOtherHandMj(emptyMjPai);
            //  Object.DontDestroyOnLoad(mj);
            otherHandPaiMjPool.Add(mj);

            return mj;
        }

        public void PushMjToOtherHandMjPool(GameObject mj)
        {
            mj.SetActive(false);
            mj.transform.GetChild(0).gameObject.SetActive(false);
        }


        public void RecoverParticles()
        {
            longjuanfengEffectPool.RecoverGameObjectsForParticles();
            rainEffectPool.RecoverGameObjectsForParticles();
            touxiangParticlePool.RecoverGameObjectsForParticles();
            pcgPaiParticlePool.RecoverGameObjectsForParticles();
            huPaiGetMjParticlePool.RecoverGameObjectsForParticles();
            huPaiShanDianParticlePool.RecoverGameObjectsForParticles();
            huPaiTextParticlePool.RecoverGameObjectsForParticles();
            pengPaiTextParticlePool.RecoverGameObjectsForParticles();
            chiPaiTextParticlePool.RecoverGameObjectsForParticles();
            gangPaiTextParticlePool.RecoverGameObjectsForParticles();
            tingPaiTextParticlePool.RecoverGameObjectsForParticles();
            zimoPaiTextParticlePool.RecoverGameObjectsForParticles();
        }

        public void AppendToDestoryPool(GameObject go)
        {
            destoryGameObjectPool.Add(go);
        }

        public void DestoryGameObjectPool()
        {
            for (int i = 0; i < destoryGameObjectPool.Count(); i++)
            {
                Object.Destroy(destoryGameObjectPool[i]);
            }

            destoryGameObjectPool.Clear();
        }

        public void AppendToDestoryPool(Tweener tweener)
        {
            destoryTweenerPool.Add(tweener);
        }

        public void DestoryTweenerPool()
        {
            for (int i = 0; i < destoryTweenerPool.Count(); i++)
            {
                destoryTweenerPool[i].Kill();
            }

            destoryTweenerPool.Clear();
        }

        public override void Destory()
        {
            base.Destory();

            DestoryGameObjectPool();
            DestoryTweenerPool();

            pcgPaiParticlePool.Destory();
            touxiangParticlePool.Destory();
            touxiangHeadParticlePool.Destory();
            huPaiGetMjParticlePool.Destory();
            huPaiShanDianParticlePool.Destory();

            MjMachineCmdPool.Instance.DestroyPool();

            for (int i = 0; i < deskOrSelfHandPaiMjPool.Count; i++)
            {
                List<GameObject> mjList = deskOrSelfHandPaiMjPool[i];

                for (int j = 0; j < mjList.Count; j++)
                {
                    Object.Destroy(mjList[j]);
                }
            }

            for (int i = 0; i < (int)MahjongFaceValue.MJ_UNKNOWN; i++)
            {
                deskOrSelfHandPaiMjPool[i].Clear();
            }

            deskOrSelfHandPaiMjPool.Clear();

            //
            for (int i = 0; i < handShadowPlanes.Length; i++)
            {
                Object.Destroy(handShadowPlanes[i]);
            }

            //
            for (int i = 0; i < otherHandPaiMjPool.Count; i++)
            {
                Object.Destroy(otherHandPaiMjPool[i]);
            }

            otherHandPaiMjPool.Clear();


            //
            settingDataAssetsMgr.DestroyAssets();
            Resources.UnloadUnusedAssets();
        }
    }

    public class MjPaiData : MonoBehaviour
    {
        public MahjongFaceValue mjFaceValue;
    }
}