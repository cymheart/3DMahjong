using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ComponentDesgin
{
    public class UISelectSwapHandPai: MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Scene scene;
        SettingDataAssetsMgr settingDataAssetsMgr;
        Transform uiCanvasTransform;

        GameObject prefabUiSelectHuaSeHandPaiTips;

        Transform uiSelectHuaSeHandPai = null;
        Transform tips;
        Transform btnOK;
        RectTransform btnOkRect;
        RectTransform imageQue;
        RectTransform imageDing;
        RectTransform imageLeftKuoHao;
        RectTransform imageRightKuoHao;
        RectTransform imageNum1;
        RectTransform imageNum2;

        Dictionary<int, Sprite[]> selectSpriteDict;
        Sprite[] que_text;
        Sprite[] ding_text;
        Sprite[] numSprites;
        Sprite[] greyNumSprites;
        Sprite[] kuohaoSprites;

        Transform[] uiXuanPaiZhongTips = new Transform[4];
        UITextDianWaitting[] uiTextDianWaittings = new UITextDianWaitting[4];

        bool isDisable = false;

        float numScaleX;
        float numScaleY;

        int state = -1;
        float stateStartTime;
        float stateLiveTime;

        bool isCompleteSwapPaiSelected = false;
        bool isClicked = false;

        public bool IsCompleteSwapPaiSelected
        {
            get
            {
                return isCompleteSwapPaiSelected;
            }
        }

        public bool IsOkClicked
        {
            get
            {
                return isClicked;
            }
        }

        CountdownSecondTimer cdsTimer = new CountdownSecondTimer();

        public override void Init()
        {
            base.Init();
            Setting((MahjongMachine)Parent);
        }

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
            settingDataAssetsMgr = mjMachine.GetComponent<SettingDataAssetsMgr>();
            scene = mjMachine.GetComponent<Scene>();
            uiCanvasTransform = scene.uiCanvasTransform;
            prefabUiSelectHuaSeHandPaiTips = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_SELECT_SWAP_HANDPAI][0];

            selectSpriteDict = settingDataAssetsMgr.GetSpritesIntKeyDict((int)SpritesType.SELECT);
            que_text = selectSpriteDict[(int)SpriteIdx.SELECT_QUE_TEXT];
            ding_text = selectSpriteDict[(int)SpriteIdx.SELECT_DING_TEXT];
            numSprites = selectSpriteDict[(int)SpriteIdx.SELECT_NUMS];
            greyNumSprites = selectSpriteDict[(int)SpriteIdx.SELECT_NUMS_GREY];
            kuohaoSprites = selectSpriteDict[(int)SpriteIdx.SELECT_KUOHAO];
        }

        public override void Load()
        { 
            GameObject go = Object.Instantiate(prefabUiSelectHuaSeHandPaiTips, uiCanvasTransform);
            go.SetActive(false);
            uiSelectHuaSeHandPai = go.transform;
            mjAssetsMgr.AppendToDestoryPool(go);


            uiXuanPaiZhongTips[1] = uiSelectHuaSeHandPai.transform.Find("UIXuanPaiZhongTips1");
            GameObject dian1 = uiXuanPaiZhongTips[1].Find("dian1").gameObject;
            GameObject dian2 = uiXuanPaiZhongTips[1].Find("dian2").gameObject;
            GameObject dian3 = uiXuanPaiZhongTips[1].Find("dian3").gameObject;
            uiTextDianWaittings[1] = new UITextDianWaitting(new GameObject[] { dian1, dian2, dian3 });

            uiXuanPaiZhongTips[2] = uiSelectHuaSeHandPai.transform.Find("UIXuanPaiZhongTips2");
            dian1 = uiXuanPaiZhongTips[2].Find("dian1").gameObject;
            dian2 = uiXuanPaiZhongTips[2].Find("dian2").gameObject;
            dian3 = uiXuanPaiZhongTips[2].Find("dian3").gameObject;
            uiTextDianWaittings[2] = new UITextDianWaitting(new GameObject[] { dian1, dian2, dian3 });

            uiXuanPaiZhongTips[3] = uiSelectHuaSeHandPai.transform.Find("UIXuanPaiZhongTips3");
            dian1 = uiXuanPaiZhongTips[3].Find("dian1").gameObject;
            dian2 = uiXuanPaiZhongTips[3].Find("dian2").gameObject;
            dian3 = uiXuanPaiZhongTips[3].Find("dian3").gameObject;
            uiTextDianWaittings[3] = new UITextDianWaitting(new GameObject[] { dian1, dian2, dian3 });

            tips = uiSelectHuaSeHandPai.Find("Tips");
            btnOK = tips.Find("BtnOK");
            btnOkRect = btnOK.GetComponent<RectTransform>();

            imageQue = btnOK.Find("ImageQue").GetComponent<RectTransform>();
            imageDing = btnOK.Find("ImageDing").GetComponent<RectTransform>();
            imageLeftKuoHao = btnOK.Find("ImageLeftKuoHao").GetComponent<RectTransform>();
            imageRightKuoHao = btnOK.Find("ImageRightKuoHao").GetComponent<RectTransform>();
            imageNum1 = btnOK.Find("ImageNum1").GetComponent<RectTransform>();
            imageNum2 = btnOK.Find("ImageNum2").GetComponent<RectTransform>();

            Rect rect = imageNum1.GetComponent<Image>().sprite.rect;
            numScaleX = imageNum1.sizeDelta.x / rect.width;
            numScaleY = imageNum1.sizeDelta.y / rect.height;

            cdsTimer.SetLimitTime(20);

            EventTriggerListener.Get(btnOK.gameObject).onClick = OnButtonClick;
        }

        public void Destory()
        {
            Object.Destroy(uiSelectHuaSeHandPai.gameObject);
        }

        public void Show(bool isEnable = false)
        {
            if (isEnable)
                Enable();
            else
                Disable();

            for (int i = 1; i <= 3; i++)
            {
                uiXuanPaiZhongTips[i].gameObject.SetActive(true);
                uiTextDianWaittings[i].Play();
            }

            isClicked = false;
            uiSelectHuaSeHandPai.gameObject.SetActive(true);
            tips.gameObject.SetActive(true);
            cdsTimer.StartTime();

            SetState(0, 0);
        }

        public int GetCurtTime()
        {
            return cdsTimer.GetCurtTime();
        }

        void OnButtonClick(GameObject go)
        {
            if (state < 0)
                return;

            if (isDisable == true)
                return;

            SetState(1, 0);
        }

        public void CompleteSwapPaiSelected()
        {
            if (state < 0)
                return;

            SetState(3, 0);

        }


        public void Update()
        {
            if (state < 0)
                return;

            for (int i = 1; i <= 3; i++)
            {
                uiTextDianWaittings[i].Update();
            }

            switch (state)
            {
                case 0:
                    {
                        cdsTimer.Timing();
                        int[] nums = cdsTimer.GetGetCurtTimeNums();

                        if (isDisable == false)
                        {
                            imageNum1.GetComponent<Image>().sprite = numSprites[nums[0]];
                            imageNum2.GetComponent<Image>().sprite = numSprites[nums[1]];
                        }
                        else
                        {
                            imageNum1.GetComponent<Image>().sprite = greyNumSprites[nums[0]];
                            imageNum2.GetComponent<Image>().sprite = greyNumSprites[nums[1]];
                        }

                        imageNum1.GetComponent<Image>().SetNativeSize();
                        imageNum2.GetComponent<Image>().SetNativeSize();
                        imageNum1.sizeDelta = new Vector2(imageNum1.sizeDelta.x * numScaleX, imageNum1.sizeDelta.y * numScaleY);
                        imageNum2.sizeDelta = new Vector2(imageNum2.sizeDelta.x * numScaleX, imageNum2.sizeDelta.y * numScaleY);

                        if (nums[0] == 0)
                            LayoutSingleNum();
                        else
                            LayoutDoubleNum();


                        if (GetCurtTime() == 0)
                        {
                            SetState(1, 0);
                        }
                    }
                    break;

                case 1:
                    {
                        isClicked = true;
                        tips.gameObject.SetActive(false);
                        SetState(2, 0);
                    }
                    break;

                case 3:
                    {
                        isCompleteSwapPaiSelected = true;

                        uiSelectHuaSeHandPai.gameObject.SetActive(false);

                        for (int i = 1; i <= 3; i++)
                            uiTextDianWaittings[i].Stop();

                        SetState(-1, 0);
                    }
                    break;
            }

        }

        void LayoutDoubleNum()
        {
            imageNum1.gameObject.SetActive(true);

            float leftKuoHaoSpacing = -3f;
            float rightKuoHaoSpacing = -5f;
            float numSpacing = -3f;
            float startposx = imageLeftKuoHao.localPosition.x + imageLeftKuoHao.sizeDelta.x / 2 + leftKuoHaoSpacing;
            Vector3 pos;

            pos = imageNum1.localPosition;
            pos.x = startposx + imageNum1.sizeDelta.x / 2;
            imageNum1.localPosition = pos;
            startposx += imageNum1.sizeDelta.x + numSpacing;

            pos = imageNum2.localPosition;
            pos.x = startposx + imageNum2.sizeDelta.x / 2;
            imageNum2.localPosition = pos;
            startposx += imageNum2.sizeDelta.x + rightKuoHaoSpacing;

            pos = imageRightKuoHao.localPosition;
            pos.x = startposx + imageRightKuoHao.sizeDelta.x / 2;
            imageRightKuoHao.localPosition = pos;
        }

        void LayoutSingleNum()
        {
            imageNum1.gameObject.SetActive(false);

            float leftKuoHaoSpacing = -3f;
            float rightKuoHaoSpacing = -5f;
            float startposx = imageLeftKuoHao.localPosition.x + imageLeftKuoHao.sizeDelta.x / 2 + leftKuoHaoSpacing;
            Vector3 pos;

            pos = imageNum2.localPosition;
            pos.x = startposx + imageNum2.sizeDelta.x / 2;
            imageNum2.localPosition = pos;
            startposx += imageNum2.sizeDelta.x + rightKuoHaoSpacing;

            pos = imageRightKuoHao.localPosition;
            pos.x = startposx + imageRightKuoHao.sizeDelta.x / 2;
            imageRightKuoHao.localPosition = pos;
        }



        public void Disable()
        {
            if (isDisable != false)
                return;

            isDisable = true;

            btnOK.GetComponent<Button>().interactable = false;

            imageQue.GetComponent<Image>().sprite = que_text[1];
            imageDing.GetComponent<Image>().sprite = ding_text[1];

            imageLeftKuoHao.GetComponent<Image>().sprite = kuohaoSprites[2];
            imageRightKuoHao.GetComponent<Image>().sprite = kuohaoSprites[3];

            int[] nums = cdsTimer.GetGetCurtTimeNums();
            imageNum1.GetComponent<Image>().sprite = greyNumSprites[nums[0]];
            imageNum2.GetComponent<Image>().sprite = greyNumSprites[nums[1]];
        }

        public void Enable()
        {
            if (isDisable == false)
                return;

            isDisable = false;

            btnOK.GetComponent<Button>().interactable = true;

            imageQue.GetComponent<Image>().sprite = que_text[0];
            imageDing.GetComponent<Image>().sprite = ding_text[0];

            imageLeftKuoHao.GetComponent<Image>().sprite = kuohaoSprites[0];
            imageRightKuoHao.GetComponent<Image>().sprite = kuohaoSprites[1];

            int[] nums = cdsTimer.GetGetCurtTimeNums();
            imageNum1.GetComponent<Image>().sprite = numSprites[nums[0]];
            imageNum2.GetComponent<Image>().sprite = numSprites[nums[1]];
        }

        void SetState(int _state, float liveTime)
        {
            state = _state;
            stateStartTime = Time.time;
            stateLiveTime = liveTime;
        }
    }
}

