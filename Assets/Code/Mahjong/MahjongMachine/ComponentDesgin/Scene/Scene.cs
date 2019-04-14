using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ComponentDesgin
{
    public class Scene : MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Fit fit;


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