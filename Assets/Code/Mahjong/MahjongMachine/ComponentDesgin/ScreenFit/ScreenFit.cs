using CoreDesgin;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ComponentDesgin
{
    public struct ScreenFitInfo
    {
        public float screenAspect;
        public Vector3 camPosition;
        public Vector3 camEluerAngle;
        public float camFieldOfView;
        public float mjScale;
    }


    public class ScreenFit: MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        Scene scene;
        MahjongAssetsMgr mjAssetsMgr;
        Transform cameraTransform;
        Fit fit;

        public override void PreLoad()
        {
            base.PreLoad();
            cameraTransform = scene.cameraTransform;
            FitScreen(Screen.width, Screen.height);
        }

        public override void Init()
        {
            base.Init();
            Setting((MahjongMachine)Parent);
        }

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            fit = mjMachine.GetComponent<Fit>();
            scene = mjMachine.GetComponent<Scene>();
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();            
        }

        #region 屏幕适配
        /// <summary>
        /// 屏幕适配
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        void FitScreen(float screenWidth, float screenHeight)
        {
            float aspect = screenWidth / screenHeight;
            List<ScreenFitInfo> fitInfo = mjAssetsMgr.screenFitInfoList;

            for (int i = 0; i < fitInfo.Count; i++)
            {
                if (aspect > fitInfo[i].screenAspect - 0.002f && aspect < fitInfo[i].screenAspect + 0.002f)
                {
                    cameraTransform.localPosition = fitInfo[i].camPosition;
                    cameraTransform.localEulerAngles = fitInfo[i].camEluerAngle;
                    cameraTransform.GetComponent<Camera>().fieldOfView = fitInfo[i].camFieldOfView;
                    fit.SetCanvasHandMjScale(fitInfo[i].mjScale);
                    return;
                }
            }

            if (aspect > 1.333f - 0.0002f && aspect < 1.333333f + 0.0002f)  // 4:3
            {
                cameraTransform.localPosition = new Vector3(-0.005f, 0.79f, 0.51f);
                cameraTransform.localEulerAngles = new Vector3(59.6f, -180f, 0);
                cameraTransform.GetComponent<Camera>().fieldOfView = 47.6f;
                fit.SetCanvasHandMjScale(1.4f);
            }
            else if (aspect > 1.599f - 0.0002f && aspect < 1.61f + 0.0002f)   // 16:10
            {
                cameraTransform.localPosition = new Vector3(-0.005f, 0.899f, 0.726f);
                cameraTransform.localEulerAngles = new Vector3(51.857f, -180f, 0);
                cameraTransform.GetComponent<Camera>().fieldOfView = 33.8f;
                fit.SetCanvasHandMjScale(1.16f);
            }
            else  // 16:9 (1920 :1080)
            {
                cameraTransform.localPosition = new Vector3(-0.005f, 0.899f, 0.726f);
                cameraTransform.localEulerAngles = new Vector3(51.857f, -180f, 0);
                cameraTransform.GetComponent<Camera>().fieldOfView = 33.8f;
                fit.SetCanvasHandMjScale(1.3f);
            }
        }
        #endregion

    }
}
