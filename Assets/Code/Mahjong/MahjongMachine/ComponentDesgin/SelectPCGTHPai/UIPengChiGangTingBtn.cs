using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using CoreDesgin;

namespace ComponentDesgin
{
    public class UIPengChiGangTingBtn
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Scene scene;
        Transform uiCanvasTransform;

        PengChiGangTingHuType type;
        Transform uiBtn;

        ParticleSystem fxAddPS;
        ParticleSystem fxOutGuangQuanPS;
        ParticleSystem uiInGuangQuanPS;

        bool isClicked = false;

        Color startColor = new Color(1, 1, 1, 0);
        Color endColor = new Color(1, 1, 1, 1);

        float duration = 0.3f;

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

        public void Setting(MahjongMachine mjMachine, PengChiGangTingHuType type = PengChiGangTingHuType.PENG)
        {
            this.mjMachine = mjMachine;
            this.type = type;
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
            scene = mjMachine.GetComponent<Scene>();
            uiCanvasTransform = scene.uiCanvasTransform;

            uiBtn = mjAssetsMgr.pengChiGangTingHuBtns[(int)type].transform;
            orgUIBtnScale = uiBtn.localScale;

            if (type != PengChiGangTingHuType.GUO && type != PengChiGangTingHuType.CANCEL)
            {
                fxAddPS = uiBtn.Find("FxAdd").GetComponent<ParticleSystem>();
                fxOutGuangQuanPS = uiBtn.Find("FxOutGuangQuan").GetComponent<ParticleSystem>();
                uiInGuangQuanPS = uiBtn.Find("UIInGuangQuan").GetComponent<ParticleSystem>();
            }


            EventTriggerListener.Get(uiBtn.gameObject).onDown = OnButtonDown;
            EventTriggerListener.Get(uiBtn.gameObject).onClick = OnButtonClick;

            State4();
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
            State4();
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
                        uiBtn.gameObject.SetActive(true);
                        uiBtn.DOScale(orgUIBtnScale, duration).SetEase(Ease.OutBack);
                        uiBtn.GetComponent<Image>().material.DOColor(endColor, duration);

                        if (type == PengChiGangTingHuType.GUO || type == PengChiGangTingHuType.CANCEL)
                            SetState(2, 0);
                        else
                            SetState(1, 0);
                    }
                    break;

                case 1:
                    {
                        fxAddPS.gameObject.SetActive(true);
                        fxOutGuangQuanPS.gameObject.SetActive(true);
                        uiInGuangQuanPS.gameObject.SetActive(true);

                        SetState(2, 0);
                    }
                    break;

                case 2:
                    {

                    }
                    break;

                case 3:
                    {
                        uiBtn.DOScale(new Vector3(orgUIBtnScale.x + 0.1f, orgUIBtnScale.y + 0.1f, 1f), 0.15f);
                        SetState(4, 0.15f);
                    }
                    break;

                case 4:
                    {
                        isClicked = true;
                        State4();
                    }
                    break;
            }
        }


        void State4()
        {
            if (type != PengChiGangTingHuType.GUO && type != PengChiGangTingHuType.CANCEL)
            {
                fxAddPS.Stop();
                fxAddPS.gameObject.SetActive(false);

                fxOutGuangQuanPS.Stop();
                fxOutGuangQuanPS.gameObject.SetActive(false);

                uiInGuangQuanPS.Stop();
                uiInGuangQuanPS.gameObject.SetActive(false);
            }

            uiBtn.localScale = new Vector3(0f, 0f, 1f);
            uiBtn.GetComponent<Image>().color = startColor;

            uiBtn.gameObject.SetActive(false);

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
            SetState(3, 0);
        }

        private void OnButtonDown(GameObject go)
        {
            // SetState(2, 0);
        }
    }
}