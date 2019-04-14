using DG.Tweening;
using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ComponentDesgin
{
    public struct HuaSeInfo
    {
        public int seatIdx;
        public MahjongHuaSe huaSe;
    }

    public class UISelectQueMen:MahjongMachineComponent
    {
        public MahjongMachine mjMachine;
        public MahjongAssetsMgr mjAssetsMgr;
        SettingDataAssetsMgr settingDataAssetsMgr;
        Scene scene;
        PreSettingHelper preSettingHelper;
        Transform uiCanvasTransform;
        public UITouXiang uiTouXiang;

        public Vector3[] huaSeStartPosSeat;

        public GameObject prefabUISelectQueYiMen;
        public GameObject prefabUISprite;
        public GameObject prefabUIHuaSeFlag;



        Dictionary<int, Sprite[]> queYiMenSpritesDict;
        Sprite[] queHuaSe;
        public Sprite[] queHuaSeMove;
        public Sprite[] huaSeFlag;


        MahjongHuaSe defaultGuangHuaSe = MahjongHuaSe.WANG;
        MahjongHuaSe clickHuaSe = MahjongHuaSe.WANG;

        public GameObject uiSelectQueYiMen;
        GameObject uiWangTongTiaoMove;
        GameObject uiWangTongTiaoFlag;

        Image uiWangTongTiaoMoveImage;
        Image uiWangTongTiaoFlagImage;

        Transform[] uiDingQueZhongTips = new Transform[4];
        UITextDianWaitting[] uiTextDianWaittings = new UITextDianWaitting[4];

        UISelectQueMenMoveHuaSe[] uiQueMenMoveHuaSe = new UISelectQueMenMoveHuaSe[4];

        Transform[] wangTongTiao = new Transform[3];
        Transform tips;

        Vector3[] wangTongTiaoOrgPos = new Vector3[3];

        ParticleSystem clickGuangPS;
        ParticleSystem guangPS;

        int state = -1;
        float stateStartTime;
        float stateLiveTime;
        float startTime = 9999;

        float esp = 0.05f;

        List<HuaSeInfo> playQueMenList = new List<HuaSeInfo>();

        public int selectedQueMenPlayerCount = 0;
        bool isCompleteQueMenSelected = false;
        bool isClicked = false;

        public bool IsCompleteQueMenSelected
        {
            get
            {
                return isCompleteQueMenSelected;
            }
        }

        public bool IsClicked
        {
            get
            {
                return isClicked;
            }
        }

        public MahjongHuaSe ClickedHuaSe
        {
            get
            {
                return clickHuaSe;
            }
        }

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
            uiTouXiang = mjMachine.GetComponent<UITouXiang>();
            preSettingHelper = mjMachine.GetComponent<PreSettingHelper>();
            huaSeStartPosSeat = preSettingHelper.huaSeStartPosSeat;

            prefabUISelectQueYiMen = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_SELECT_QUEMEN][0];
            prefabUISprite = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_SPRITE][0];
            prefabUIHuaSeFlag = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_HUASE_FLAG][0];

            queYiMenSpritesDict = settingDataAssetsMgr.GetSpritesIntKeyDict((int)SpritesType.QUE_YI_MEN);
            queHuaSe = queYiMenSpritesDict[(int)SpriteIdx.QUE_HUA_SE];
            queHuaSeMove = queYiMenSpritesDict[(int)SpriteIdx.QUE_HUA_SE_MOVE];
            huaSeFlag = queYiMenSpritesDict[(int)SpriteIdx.HUA_SE_FLAG];

            for (int i = 1; i <= 3; i++)
            {
                uiQueMenMoveHuaSe[i] = new UISelectQueMenMoveHuaSe(this, i);
            }

        }

        public override void Load()
        {
            uiSelectQueYiMen = Object.Instantiate(prefabUISelectQueYiMen, uiCanvasTransform);
            uiSelectQueYiMen.SetActive(false);
            mjAssetsMgr.AppendToDestoryPool(uiSelectQueYiMen);

            uiDingQueZhongTips[1] = uiSelectQueYiMen.transform.Find("UIDingQueZhongTips1");
            GameObject dian1 = uiDingQueZhongTips[1].Find("dian1").gameObject;
            GameObject dian2 = uiDingQueZhongTips[1].Find("dian2").gameObject;
            GameObject dian3 = uiDingQueZhongTips[1].Find("dian3").gameObject;
            uiTextDianWaittings[1] = new UITextDianWaitting(new GameObject[] { dian1, dian2, dian3 });

            uiDingQueZhongTips[2] = uiSelectQueYiMen.transform.Find("UIDingQueZhongTips2");
            dian1 = uiDingQueZhongTips[2].Find("dian1").gameObject;
            dian2 = uiDingQueZhongTips[2].Find("dian2").gameObject;
            dian3 = uiDingQueZhongTips[2].Find("dian3").gameObject;
            uiTextDianWaittings[2] = new UITextDianWaitting(new GameObject[] { dian1, dian2, dian3 });

            uiDingQueZhongTips[3] = uiSelectQueYiMen.transform.Find("UIDingQueZhongTips3");
            dian1 = uiDingQueZhongTips[3].Find("dian1").gameObject;
            dian2 = uiDingQueZhongTips[3].Find("dian2").gameObject;
            dian3 = uiDingQueZhongTips[3].Find("dian3").gameObject;
            uiTextDianWaittings[3] = new UITextDianWaitting(new GameObject[] { dian1, dian2, dian3 });


            wangTongTiao[(int)MahjongHuaSe.WANG] = uiSelectQueYiMen.transform.Find("Wang");
            wangTongTiaoOrgPos[(int)MahjongHuaSe.WANG] = wangTongTiao[(int)MahjongHuaSe.WANG].localPosition;

            wangTongTiao[(int)MahjongHuaSe.TONG] = uiSelectQueYiMen.transform.Find("Tong");
            wangTongTiaoOrgPos[(int)MahjongHuaSe.TONG] = wangTongTiao[(int)MahjongHuaSe.TONG].localPosition;

            wangTongTiao[(int)MahjongHuaSe.TIAO] = uiSelectQueYiMen.transform.Find("Tiao");
            wangTongTiaoOrgPos[(int)MahjongHuaSe.TIAO] = wangTongTiao[(int)MahjongHuaSe.TIAO].localPosition;

            guangPS = uiSelectQueYiMen.transform.Find("Guang").GetComponent<ParticleSystem>();
            clickGuangPS = uiSelectQueYiMen.transform.Find("ClickGuang").GetComponent<ParticleSystem>();

            tips = uiSelectQueYiMen.transform.Find("Tips");


            uiWangTongTiaoMove = Object.Instantiate(prefabUISprite, uiSelectQueYiMen.transform);
            mjAssetsMgr.AppendToDestoryPool(uiWangTongTiaoMove);
            uiWangTongTiaoMove.SetActive(false);
            uiWangTongTiaoMove.GetComponent<RectTransform>().sizeDelta = wangTongTiao[0].GetComponent<RectTransform>().sizeDelta;
            uiWangTongTiaoMoveImage = uiWangTongTiaoMove.GetComponent<Image>();


            uiWangTongTiaoFlag = Object.Instantiate(prefabUIHuaSeFlag, uiSelectQueYiMen.transform);
            mjAssetsMgr.AppendToDestoryPool(uiWangTongTiaoFlag);
            uiWangTongTiaoFlag.SetActive(false);
            uiWangTongTiaoFlagImage = uiWangTongTiaoFlag.transform.Find("HuaSe").GetComponent<Image>();


            EventTriggerListener.Get(wangTongTiao[0].gameObject).onClick = OnButtonClick;
            EventTriggerListener.Get(wangTongTiao[1].gameObject).onClick = OnButtonClick;
            EventTriggerListener.Get(wangTongTiao[2].gameObject).onClick = OnButtonClick;



            uiQueMenMoveHuaSe[1].Load();
            uiQueMenMoveHuaSe[2].Load();
            uiQueMenMoveHuaSe[3].Load();
        }

        public void Show(MahjongHuaSe mjHuaSe = MahjongHuaSe.WANG)
        {
            if (state >= 0)
                return;

            selectedQueMenPlayerCount = 0;
            isCompleteQueMenSelected = false;

            for (int i = 1; i <= 3; i++)
            {
                uiDingQueZhongTips[i].gameObject.SetActive(true);
                uiTextDianWaittings[i].Play();
            }

            isClicked = false;
            defaultGuangHuaSe = mjHuaSe;
            SetState(0, 0);
        }


        public void AppendPlayQueMenForSeatToList(int seatIdx, MahjongHuaSe huase)
        {
            HuaSeInfo info = new HuaSeInfo();
            info.seatIdx = seatIdx;
            info.huaSe = huase;
            playQueMenList.Add(info);
        }

        public void PlayQueMenFromList()
        {
            if (playQueMenList.Count == 0)
                return;

            for (int i = 0; i < playQueMenList.Count; i++)
                PlaySelectQueMenForOtherSeat(playQueMenList[i].seatIdx, playQueMenList[i].huaSe);

            playQueMenList.Clear();
        }

        public void PlaySelectQueMenForOtherSeat(int seatIdx, MahjongHuaSe huase)
        {
            if (seatIdx == 0)
                return;

            uiDingQueZhongTips[seatIdx].gameObject.SetActive(false);
            uiTextDianWaittings[seatIdx].Stop();
            uiQueMenMoveHuaSe[seatIdx].Play(huase);
        }


        public void Update()
        {
            if (state < 0)
                return;

            for (int i = 1; i <= 3; i++)
            {
                uiTextDianWaittings[i].Update();
                uiQueMenMoveHuaSe[i].Update();
            }

            if (TimeAt(0.5f))
            {
                SetState(4, 0);
            }
            else if (TimeAt(0.6f))
            {
                SetState(3, 0);
            }
            else if (TimeAt(0.8f))
            {
                SetState(5, 0);
            }
            else if (TimeAt(1.6f))
            {
                SetState(12, 0);
            }

            if (state < 0 || Time.time - stateStartTime < stateLiveTime)
            {
                return;
            }

            switch (state)
            {
                case 0:
                    {

                        startTime = 99999;
                        clickGuangPS.gameObject.SetActive(false);
                        guangPS.gameObject.SetActive(true);
                        tips.gameObject.SetActive(true);

                        wangTongTiao[0].gameObject.SetActive(true);
                        wangTongTiao[1].gameObject.SetActive(true);
                        wangTongTiao[2].gameObject.SetActive(true);

                        guangPS.transform.localPosition = wangTongTiao[(int)defaultGuangHuaSe].localPosition;
                        uiSelectQueYiMen.SetActive(true);
                        SetState(11, 0);
                    }
                    break;

                case 1:
                    {
                        isClicked = true;
                        selectedQueMenPlayerCount++;

                        wangTongTiao[0].gameObject.SetActive(false);
                        wangTongTiao[1].gameObject.SetActive(false);
                        wangTongTiao[2].gameObject.SetActive(false);
                        wangTongTiao[(int)clickHuaSe].gameObject.SetActive(true);

                        tips.gameObject.SetActive(false);

                        uiWangTongTiaoMove.transform.localPosition = wangTongTiao[(int)clickHuaSe].localPosition;
                        uiWangTongTiaoMoveImage.sprite = queHuaSe[(int)clickHuaSe];
                        uiWangTongTiaoMoveImage.color = Color.white;
                        uiWangTongTiaoMove.SetActive(true);


                        uiWangTongTiaoFlag.transform.position = uiTouXiang.GetHuaSeFlagWorldPosition(0);
                        uiWangTongTiaoFlagImage.sprite = huaSeFlag[(int)clickHuaSe];
                        uiWangTongTiaoFlag.SetActive(false);

                        clickGuangPS.transform.localPosition = wangTongTiao[(int)clickHuaSe].localPosition;
                        clickGuangPS.gameObject.SetActive(true);
                        clickGuangPS.Play();

                        guangPS.gameObject.SetActive(false);
                        guangPS.Stop();

                        SetState(2, 0.3f);

                    }
                    break;

                case 2:
                    {
                        startTime = Time.time;

                        clickGuangPS.Stop();
                        clickGuangPS.gameObject.SetActive(false);
                        Vector3 pos = uiTouXiang.GetHuaSeFlagWorldPosition(0);
                        uiWangTongTiaoMove.transform.DOMove(pos, 0.6f);
                        SetState(11, 0);
                    }
                    break;

                case 3:
                    {
                        uiWangTongTiaoMove.transform.DOScale(new Vector3(0.3f, 0.3f, 1), 0.2f);
                        uiWangTongTiaoMove.GetComponent<Image>().material.DOFade(0, 1f);

                        uiWangTongTiaoFlagImage.color = new Color(1, 1, 1, 0);
                        uiWangTongTiaoFlagImage.material.DOFade(1, 0.4f);
                        uiWangTongTiaoFlag.SetActive(true);

                        SetState(11, 0);
                    }
                    break;

                case 4:
                    {
                        Vector3 pos = uiTouXiang.GetHuaSeFlagWorldPosition(0);
                        wangTongTiao[(int)clickHuaSe].transform.DOMove(pos, 0.3f);
                        SetState(11, 0);
                    }
                    break;

                case 5:
                    {
                        wangTongTiao[(int)clickHuaSe].transform.DOScale(new Vector3(0.3f, 0.3f, 1), 0.2f);
                        wangTongTiao[(int)clickHuaSe].GetComponent<Image>().material.DOFade(0, 0.4f);
                        SetState(11, 0);
                    }
                    break;

                case 12:
                    {
                        startTime = 99999;
                        uiWangTongTiaoMove.SetActive(false);
                        uiWangTongTiaoMove.transform.localScale = Vector3.one;

                        for (int i = 0; i < 3; i++)
                        {
                            wangTongTiao[i].GetComponent<Image>().color = Color.white;
                            wangTongTiao[i].localPosition = wangTongTiaoOrgPos[i];
                            wangTongTiao[i].localScale = Vector3.one;
                            wangTongTiao[i].gameObject.SetActive(false);
                        }

                        uiWangTongTiaoFlag.SetActive(false);
                        uiTouXiang.ShowHuaSeFlag(0, huaSeFlag[(int)clickHuaSe]);

                        SetState(13, 0);
                    }
                    break;

                case 13:
                    {
                        if (selectedQueMenPlayerCount == 4)
                        {
                            isCompleteQueMenSelected = true;
                            SetState(-1, 0);
                        }
                    }
                    break;

            }
        }


        bool TimeAt(float tm)
        {
            return Time.time - startTime >= tm - esp && Time.time - startTime <= tm + esp;
        }

        private void OnButtonClick(GameObject go)
        {
            if (go == wangTongTiao[(int)MahjongHuaSe.WANG].gameObject)
                clickHuaSe = MahjongHuaSe.WANG;
            else if (go == wangTongTiao[(int)MahjongHuaSe.TONG].gameObject)
                clickHuaSe = MahjongHuaSe.TONG;
            else if (go == wangTongTiao[(int)MahjongHuaSe.TIAO].gameObject)
                clickHuaSe = MahjongHuaSe.TIAO;

            SetState(1, 0);
        }

        void SetState(int _state, float liveTime)
        {
            state = _state;
            stateStartTime = Time.time;
            stateLiveTime = liveTime;
        }
    }
}
