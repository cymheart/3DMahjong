using CoreDesgin;
using UnityEngine;

namespace ComponentDesgin
{
    public class Scene : MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;

        public int uiLayer = LayerMask.NameToLayer("UI");
        public int defaultLayer = LayerMask.NameToLayer("Default");

        public Transform root;
        public Transform mjtableTransform;
        public Transform canvasHandPaiTransform;
        public Transform cameraTransform;
        public Transform uiCanvasTransform;
        public RectTransform canvasRectTransform;

        
        public override void PreInit()
        {
            base.PreInit();

            GameObject prefabScene = mjAssetsMgr.defaultPrefabDict[(int)PrefabIdx.SCENE][0];
            root = Object.Instantiate(prefabScene).transform;

            mjtableTransform = root.Find("MahjongDesk").transform;
            canvasHandPaiTransform = root.Find("HandPaiCanvas").transform;
            cameraTransform = root.Find("3DCamera").transform;
            uiCanvasTransform = root.Find("UICanvas").transform;
            canvasRectTransform = uiCanvasTransform.GetComponent<RectTransform>();
        }
    }
}