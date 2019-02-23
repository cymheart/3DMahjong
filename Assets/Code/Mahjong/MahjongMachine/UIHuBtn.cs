using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace MahjongMachineNS
{
    public class UIHuBtn
    {
        private static int maskOffset;

        MahjongMachine mjMachine;
        MahjongAssets mjAssets;
        MahjongGame mjGame;
        MahjongAssetsMgr mjAssetsMgr;
        Transform uiCanvasTransform;

        Transform uiBtn;
        ParticleSystem uiBtnPS;
        ParticleSystem huTextPS;
        ParticleSystem huTextHuXiPS;
        ParticleSystem meiHuaImgPS;
        ParticleSystem outsideExtGuangQuanPS;
        ParticleSystem petalsPS, petalsPS2, petalsPS3;

        bool isClicked = false;

        Tweener meiHuaImgScale;
        Material mat;
        Vector3 orgUIBtnScale;

        int state = -1;
        float stateStartTime;
        float stateLiveTime;
        public bool IsClicked
        {
            get
            {
                return isClicked;
            }
        }

        static UIHuBtn()
        {
            maskOffset = Shader.PropertyToID("_MaskOffset");
        }

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjGame = mjMachine.mjGame;
            mjAssets = mjMachine.mjAssets;
            mjAssetsMgr = mjMachine.mjAssetsMgr;
            uiCanvasTransform = mjMachine.uiCanvasTransform;

            uiBtn = mjAssetsMgr.pengChiGangTingHuBtns[(int)PengChiGangTingHuType.HU].transform;
            orgUIBtnScale = uiBtn.localScale;


            uiBtnPS = uiBtn.GetComponent<ParticleSystem>();
            huTextPS = uiBtn.Find("HuText").GetComponent<ParticleSystem>();
            huTextHuXiPS = uiBtn.Find("HuTextHuXi").GetComponent<ParticleSystem>();
            meiHuaImgPS = uiBtn.Find("MeiHuaImg").GetComponent<ParticleSystem>();
            outsideExtGuangQuanPS = uiBtn.Find("OutsideExtGuangQuan").GetComponent<ParticleSystem>();
            petalsPS = uiBtn.Find("Petals").GetComponent<ParticleSystem>();
            petalsPS2 = uiBtn.Find("Petals2").GetComponent<ParticleSystem>();
            petalsPS3 = uiBtn.Find("Petals3").GetComponent<ParticleSystem>();

            Image img = uiBtn.GetComponent<Image>();
            mat = img.material;


            meiHuaImgScale = meiHuaImgPS.transform.DOLocalRotate(new Vector3(0, 0, 10f), 1.5f);
            meiHuaImgScale.SetAutoKill(false);
            meiHuaImgScale.SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            meiHuaImgScale.Pause();

            mjAssetsMgr.AppendToDestoryPool(meiHuaImgScale);

            EventTriggerListener.Get(uiBtn.gameObject).onDown = OnButtonDown;
            EventTriggerListener.Get(uiBtn.gameObject).onClick = OnButtonClick;

            State5();
        }

        public void Destory()
        {
            meiHuaImgScale.Kill();
        }

        public void SetParent(Transform parent)
        {
            uiBtn.SetParent(parent);
        }


        public void Show()
        {
            if (state >= 0)
                return;

            isClicked = false;
            SetState(0, 0);
        }

        public void Hide()
        {
            if (state < 0)
                return;

            isClicked = false;
            State5();
        }

        public void Update()
        {
            if (state < 0 || Time.time - stateStartTime < stateLiveTime)
            {
                return;
            }

            switch (state)
            {
                case 0:
                    {

                        float waitTime = 0.5f;
                        uiBtn.gameObject.SetActive(true);

                        uiBtn.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), waitTime);
                        uiBtn.DOScale(orgUIBtnScale, waitTime).SetEase(Ease.OutBack);

                        uiBtnPS.Play();

                        SetState(1, 1.4f);
                    }
                    break;

                case 1:
                    {
                        huTextPS.Stop();
                        huTextPS.gameObject.SetActive(false);

                        huTextHuXiPS.gameObject.SetActive(true);

                        petalsPS.gameObject.SetActive(true);
                        petalsPS2.gameObject.SetActive(true);
                        petalsPS3.gameObject.SetActive(true);

                        var meiHuaImgColorOverLifetime = meiHuaImgPS.colorOverLifetime;
                        meiHuaImgColorOverLifetime.enabled = false;

                        outsideExtGuangQuanPS.gameObject.SetActive(true);
                        meiHuaImgScale.Play();

                        SetState(2, 0);
                    }
                    break;

                case 2:
                    {
                        float offset = Common.Mod(Time.time - stateStartTime, 3f) / 3f;
                        mat.SetFloat(maskOffset, offset);
                    }
                    break;

                case 3:
                    {
                        // uiBtn.transform.localScale = new Vector3(1.02f, 1.02f, 1f);
                    }
                    break;

                case 4:
                    {

                        uiBtn.DOScale(new Vector3(orgUIBtnScale.x + 0.1f, orgUIBtnScale.y + 0.1f, 1f), 0.15f);
                        SetState(5, 0.15f);
                    }

                    break;

                case 5:
                    {
                        isClicked = true;
                        State5();
                    }
                    break;
            }
        }

        void State5()
        {
            mat.SetFloat(maskOffset, 0);

            var meiHuaImgColorOverLifetime = meiHuaImgPS.colorOverLifetime;
            meiHuaImgColorOverLifetime.enabled = true;
            meiHuaImgScale.Restart();
            meiHuaImgScale.Pause();


            uiBtnPS.Stop();
            uiBtn.gameObject.SetActive(false);

            huTextHuXiPS.Stop();
            huTextHuXiPS.gameObject.SetActive(false);

            huTextPS.gameObject.SetActive(true);
            huTextPS.Stop();
            outsideExtGuangQuanPS.Stop();
            outsideExtGuangQuanPS.gameObject.SetActive(false);

            petalsPS.gameObject.SetActive(false);
            petalsPS2.gameObject.SetActive(false);
            petalsPS3.gameObject.SetActive(false);

            uiBtn.localScale = new Vector3(orgUIBtnScale.x * 2f, orgUIBtnScale.y * 2f, 1f);
            uiBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);

            meiHuaImgPS.transform.localEulerAngles = Vector3.zero;

            SetState(-1, 0);
        }

        void SetState(int _state, float liveTime)
        {
            state = _state;
            stateStartTime = Time.time;
            stateLiveTime = liveTime;
        }


        private void OnButtonClick(GameObject go)
        {
            SetState(4, 0);
        }

        private void OnButtonDown(GameObject go)
        {
            SetState(3, 0);
        }

    }
}
