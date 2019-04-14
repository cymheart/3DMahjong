using CoreDesgin;
using System;
using UnityEngine;

namespace ComponentDesgin
{
    public class MjHuTingAlgorithm : MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Fit fit;


        public int uiLayer = LayerMask.NameToLayer("UI");
        public int defaultLayer = LayerMask.NameToLayer("Default");

        public Transform root;

        /// <summary>
        /// 麻将桌
        /// </summary>
        public Transform uiCanvasTransform;
        public RectTransform canvasRectTransform;
   

        public Transform cameraTransform;


        public override void PreInit()
        {
            base.PreInit();

            GameObject prefabMjTable = mjAssetsMgr.defaultPrefabDict[(int)PrefabIdx.TABLE_POSITION_HELPER][0];
            GameObject prefabCanvasHandPai = mjAssetsMgr.defaultPrefabDict[(int)PrefabIdx.TABLE_POSITION_HELPER][0];

            mjtableTransform = Object.Instantiate(prefabMjTable).transform;
            canvasHandPaiTransform = Object.Instantiate(prefabCanvasHandPai).transform;
            canvasHandPaiRectTransform = canvasHandPaiTransform.GetComponent<RectTransform>();

        }
    }
}