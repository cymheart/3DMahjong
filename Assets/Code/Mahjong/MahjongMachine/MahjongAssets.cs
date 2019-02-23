using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace MahjongMachineNS
{
    public class MahjongAssets
    {
        AssetBundle ab;

        public delegate void LoadMahjongAssetsCompletedDelegate();

        /// <summary>
        /// 载入麻将资产完成回调
        /// </summary>
        public LoadMahjongAssetsCompletedDelegate LoadMahjongAssetsCompleted = null;

        public Dictionary<int, GameObject[]> defaultPrefabDict;
        public Dictionary<int, GameObject[]> effectPrefabDict;
        public Dictionary<int, GameObject[]> mjpaiPrefabDict;
        public Dictionary<int, GameObject[]> uiPrefabDict;

        public Dictionary<string, Dictionary<string, ActionDataInfo>> handActionDataInfoDicts;
        public List<ScreenFitInfo> screenFitInfoList;

        public SettingDataAssetsMgr settingDataAssetsMgr = new SettingDataAssetsMgr();

        public Material mjHighLightFaceMat;
        public Material mjNormalFaceMat;

        public void Load(MahjongGame mjGame, LoadMahjongAssetsCompletedDelegate loadMjAssetsCompleted = null)
        {
            LoadMahjongAssetsCompleted =  loadMjAssetsCompleted;
            mjGame.StartCoroutine(LoadMahjongRes());
        }

        public IEnumerator LoadMahjongRes()
        {
            string path = "file://" + Application.dataPath + "/StreamingAssets" + "/majiang.ab";
            //string path = "jar:file://" + Application.dataPath + "!/assets/majiang.ab";
            //bool m_isStartDownload = true;

            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path);
            request.timeout = 30;//设置超时，request.SendWebRequest()连接超时会返回，且isNetworkError为true
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log("Download Error:" + request.error);
            }
            else
            {
                //byte[] bytes = request.downloadHandler.data;
                ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;

            }

            //m_isStartDownload = false;

            /*
            WWW bundle = new WWW(path);
            yield return bundle;
            ab = bundle.assetBundle;
            */

            CreateMahjongRes();

            if (LoadMahjongAssetsCompleted != null)
                LoadMahjongAssetsCompleted();
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

            ab.Unload(false);
        }

        public void Destroy()
        {
            settingDataAssetsMgr.DestroyAssets();
            Resources.UnloadUnusedAssets();
        }

    }
}

