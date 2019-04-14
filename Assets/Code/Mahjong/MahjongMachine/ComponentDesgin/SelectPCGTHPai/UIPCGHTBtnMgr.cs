using UnityEngine;
using CoreDesgin;

namespace ComponentDesgin
{
    public class UIPCGHTBtnMgr : MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Scene scene;
        Transform uiCanvasTransform;

        Transform pcghtBtnLayout;
        PengChiGangTingHuType[] btnTypes;

        Vector3 layoutOrgPos;


        UIHuBtn uiHuBtn = new UIHuBtn();
        UIPengChiGangTingBtn uiPengBtn = new UIPengChiGangTingBtn();
        UIPengChiGangTingBtn uiChiBtn = new UIPengChiGangTingBtn();
        UIPengChiGangTingBtn uiGangBtn = new UIPengChiGangTingBtn();
        UIPengChiGangTingBtn uiTingBtn = new UIPengChiGangTingBtn();
        UIPengChiGangTingBtn uiGuoBtn = new UIPengChiGangTingBtn();
        UIPengChiGangTingBtn uiCancelBtn = new UIPengChiGangTingBtn();

        int state = -1;
        float stateStartTime;
        float stateLiveTime;

        bool isClicked = false;

        public bool IsClicked
        {
            get
            {
                return isClicked;
            }
        }

        public PengChiGangTingHuType clickedBtnType;

        public override void Init()
        {
            base.Init();
            Setting((MahjongMachine)Parent);
        }

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            scene = mjMachine.GetComponent<Scene>();
            uiCanvasTransform = scene.uiCanvasTransform;
            pcghtBtnLayout = uiCanvasTransform.Find("PengChiGangHuTingBtnLayout");

            layoutOrgPos = pcghtBtnLayout.localPosition;

            uiHuBtn.Setting(mjMachine);
            uiPengBtn.Setting(mjMachine, PengChiGangTingHuType.PENG);
            uiChiBtn.Setting(mjMachine, PengChiGangTingHuType.CHI);
            uiGangBtn.Setting(mjMachine, PengChiGangTingHuType.GANG);
            uiTingBtn.Setting(mjMachine, PengChiGangTingHuType.TING);
            uiGuoBtn.Setting(mjMachine, PengChiGangTingHuType.GUO);
            uiCancelBtn.Setting(mjMachine, PengChiGangTingHuType.CANCEL);
        }

        public void Update()
        {
            uiHuBtn.Update();
            uiPengBtn.Update();
            uiChiBtn.Update();
            uiGangBtn.Update();
            uiTingBtn.Update();
            uiGuoBtn.Update();
            uiCancelBtn.Update();

            if (state < 0 || Time.time - stateStartTime < stateLiveTime)
            {
                return;
            }

            switch (state)
            {
                case 0:
                    {
                        isClicked = false;
                        ShowBtnsByTypes();
                        SetState(1, 0);
                    }
                    break;

                case 1:
                    {
                        isClicked = CheckClicked();
                        if (isClicked)
                        {
                            HideBtnsByTypes();
                            SetState(-1, 0);
                        }
                    }
                    break;
            }
        }

        public void Show(PengChiGangTingHuType[] btnTypes)
        {
            Show(btnTypes, Vector3.zero);
        }

        public void Show(PengChiGangTingHuType[] btnTypes, Vector3 posOffset)
        {
            if (state >= 0)
                return;

            pcghtBtnLayout.localPosition = new Vector3(layoutOrgPos.x + posOffset.x, layoutOrgPos.y + posOffset.y, layoutOrgPos.z);

            isClicked = false;
            this.btnTypes = btnTypes;
            SetState(0, 0);
        }

        public void Hide()
        {
            if (state < 0)
                return;

            isClicked = false;
            HideBtnsByTypes();
            SetState(-1, 0);
        }

        void ShowBtnsByTypes()
        {
            for (int i = 0; i < btnTypes.Length; i++)
            {
                switch (btnTypes[i])
                {
                    case PengChiGangTingHuType.HU:
                        uiHuBtn.SetParent(pcghtBtnLayout);
                        uiHuBtn.Show();
                        break;

                    case PengChiGangTingHuType.PENG:
                        uiPengBtn.SetParent(pcghtBtnLayout);
                        uiPengBtn.Show();
                        break;

                    case PengChiGangTingHuType.CHI:
                        uiChiBtn.SetParent(pcghtBtnLayout);
                        uiChiBtn.Show();
                        break;

                    case PengChiGangTingHuType.GANG:
                        uiGangBtn.SetParent(pcghtBtnLayout);
                        uiGangBtn.Show();
                        break;

                    case PengChiGangTingHuType.TING:
                        uiTingBtn.SetParent(pcghtBtnLayout);
                        uiTingBtn.Show();
                        break;

                    case PengChiGangTingHuType.GUO:
                        uiGuoBtn.SetParent(pcghtBtnLayout);
                        uiGuoBtn.Show();
                        break;

                    case PengChiGangTingHuType.CANCEL:
                        uiCancelBtn.SetParent(pcghtBtnLayout);
                        uiCancelBtn.Show();
                        break;
                }
            }
        }


        void HideBtnsByTypes()
        {
            for (int i = 0; i < btnTypes.Length; i++)
            {
                switch (btnTypes[i])
                {
                    case PengChiGangTingHuType.HU:
                        uiHuBtn.SetParent(uiCanvasTransform);
                        uiHuBtn.Hide();
                        break;

                    case PengChiGangTingHuType.PENG:
                        uiPengBtn.SetParent(uiCanvasTransform);
                        uiPengBtn.Hide();
                        break;

                    case PengChiGangTingHuType.CHI:
                        uiChiBtn.SetParent(uiCanvasTransform);
                        uiChiBtn.Hide();
                        break;

                    case PengChiGangTingHuType.GANG:
                        uiGangBtn.SetParent(uiCanvasTransform);
                        uiGangBtn.Hide();
                        break;

                    case PengChiGangTingHuType.TING:
                        uiTingBtn.SetParent(uiCanvasTransform);
                        uiTingBtn.Hide();
                        break;

                    case PengChiGangTingHuType.GUO:
                        uiGuoBtn.SetParent(uiCanvasTransform);
                        uiGuoBtn.Hide();
                        break;

                    case PengChiGangTingHuType.CANCEL:
                        uiCancelBtn.SetParent(uiCanvasTransform);
                        uiCancelBtn.Hide();
                        break;
                }
            }
        }


        bool CheckClicked()
        {
            for (int i = 0; i < btnTypes.Length; i++)
            {
                switch (btnTypes[i])
                {
                    case PengChiGangTingHuType.HU:
                        if (uiHuBtn.IsClicked == true)
                        {
                            clickedBtnType = PengChiGangTingHuType.HU;
                            return true;
                        }
                        break;

                    case PengChiGangTingHuType.PENG:
                        if (uiPengBtn.IsClicked == true)
                        {
                            clickedBtnType = PengChiGangTingHuType.PENG;
                            return true;
                        }
                        break;

                    case PengChiGangTingHuType.CHI:
                        if (uiChiBtn.IsClicked == true)
                        {
                            clickedBtnType = PengChiGangTingHuType.CHI;
                            return true;
                        }
                        break;

                    case PengChiGangTingHuType.GANG:
                        if (uiGangBtn.IsClicked == true)
                        {
                            clickedBtnType = PengChiGangTingHuType.GANG;
                            return true;
                        }
                        break;

                    case PengChiGangTingHuType.TING:
                        if (uiTingBtn.IsClicked == true)
                        {
                            clickedBtnType = PengChiGangTingHuType.TING;
                            return true;
                        }
                        break;

                    case PengChiGangTingHuType.GUO:
                        if (uiGuoBtn.IsClicked == true)
                        {
                            clickedBtnType = PengChiGangTingHuType.GUO;
                            return true;
                        }
                        break;

                    case PengChiGangTingHuType.CANCEL:
                        if (uiCancelBtn.IsClicked == true)
                        {
                            clickedBtnType = PengChiGangTingHuType.CANCEL;
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        void SetState(int _state, float liveTime)
        {
            state = _state;
            stateStartTime = Time.time;
            stateLiveTime = liveTime;
        }

    }
}