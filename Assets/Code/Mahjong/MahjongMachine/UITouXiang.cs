using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MahjongMachineNS
{
    public class UITouXiang
    {
        MahjongMachine mjMachine;
        MahjongAssets mjAssets;
        MahjongAssetsMgr mjAssetsMgr;
        Transform canvas;

        GameObject prefabPlayerTouXiang;


        Dictionary<int, Sprite[]> queYiMenSpritesDict;
        Sprite[] huaSeFlag;

        Transform[] playerTouXiangSeat = new Transform[4];
        Transform[] tingPaiFlagSeat = new Transform[4];
        Transform[] huaSeFlagSeat = new Transform[4];
        Text[] scoreSeat = new Text[4];
        CanvasGroup[] canvasGroup = new CanvasGroup[4];
        Tweener[] fadeTweener = new Tweener[4];

        UITouXiangLiuGuangEffect[] seatTouXiangLiuGuangEffects = new UITouXiangLiuGuangEffect[4];

        int[] huaSeDir = new int[] { 1, 1, 1, 1 };

        int[] state = new int[] { -1, -1, -1, -1 };

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjAssets = mjMachine.mjAssets;
            mjAssetsMgr = mjMachine.mjAssetsMgr;
            canvas = mjMachine.uiCanvasTransform;

            prefabPlayerTouXiang = mjAssets.uiPrefabDict[(int)PrefabIdx.UI_TOUXIANG][0];


            queYiMenSpritesDict = mjAssets.settingDataAssetsMgr.GetSpritesIntKeyDict((int)SpritesType.QUE_YI_MEN);
            huaSeFlag = queYiMenSpritesDict[(int)SpriteIdx.HUA_SE_FLAG];
        }

        public void Load()
        {
            for (int i = 0; i < 4; i++)
            {

                playerTouXiangSeat[i] = Object.Instantiate(prefabPlayerTouXiang, canvas).transform;
                playerTouXiangSeat[i].localPosition = mjMachine.uiTouXiangPosSeat[i];
                tingPaiFlagSeat[i] = playerTouXiangSeat[i].Find("TingPaiFlag");
                huaSeFlagSeat[i] = playerTouXiangSeat[i].Find("HuaSeFlag");
                scoreSeat[i] = playerTouXiangSeat[i].Find("Coin").GetComponent<Text>();

                canvasGroup[i] = playerTouXiangSeat[i].GetComponent<CanvasGroup>();
                HideTingFlag(i);
                HideHuaSeFlag(i);
                mjAssetsMgr.AppendToDestoryPool(playerTouXiangSeat[i].gameObject);


                fadeTweener[i] = canvasGroup[i].DOFade(0.25f, 0.8f);
                fadeTweener[i].SetAutoKill(false);
                fadeTweener[i].SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                fadeTweener[i].Pause();
                mjAssetsMgr.AppendToDestoryPool(fadeTweener[i]);
            }

            GameObject touXiangLiuGuang;

            for (int i = 0; i < MahjongMachine.playerCount; i++)
            {
                if (!mjMachine.isCanUseSeatPlayer[i])
                {
                    playerTouXiangSeat[i].gameObject.SetActive(false);
                    continue;
                }

                SetHuaSeFlagDir(i);

                playerTouXiangSeat[i].gameObject.SetActive(true);
                touXiangLiuGuang = playerTouXiangSeat[i].Find("TouXiangLiuGuang").gameObject;
                seatTouXiangLiuGuangEffects[i] = new UITouXiangLiuGuangEffect();
                seatTouXiangLiuGuangEffects[i].Setting(mjMachine, touXiangLiuGuang);
                seatTouXiangLiuGuangEffects[i].Stop();

            }
        }

        public void Destory()
        {
            for (int i = 0; i < 4; i++)
            {
                Object.Destroy(playerTouXiangSeat[i]);
                fadeTweener[i].Kill();
            }
        }

        public void SetDuration(float duration)
        {
            for (int i = 0; i < 4; i++)
                seatTouXiangLiuGuangEffects[i].duration = duration;
        }

        public void SetHuaSeFlagDir(int seatIdx, int dir = 1)
        {
            huaSeDir[seatIdx] = dir;
            SetHuaSeFlagPosition(seatIdx);
        }

        void SetHuaSeFlagPosition(int seatIdx)
        {
            float x = playerTouXiangSeat[seatIdx].Find("Bg").GetComponent<RectTransform>().sizeDelta.x / 2;
            Vector3 orgpos = huaSeFlagSeat[seatIdx].gameObject.transform.localPosition;

            if (huaSeDir[seatIdx] == 0)
            {
                huaSeFlagSeat[seatIdx].gameObject.transform.localPosition = new Vector3(-x, orgpos.y, 0);
            }
            else
            {
                huaSeFlagSeat[seatIdx].gameObject.transform.localPosition = new Vector3(x, orgpos.y, 0);
            }
        }

        public void SetScore(int seatIdx, int score)
        {
            scoreSeat[seatIdx].text = score.ToString();
        }

        public void SetTouXiang(int seatIdx, Sprite img)
        {
            playerTouXiangSeat[seatIdx].Find("TouXiang").GetComponent<Image>().sprite = img;
        }

        public void ShowTingFlag(int seatIdx)
        {
            tingPaiFlagSeat[seatIdx].gameObject.SetActive(true);
        }

        public void HideTingFlag(int seatIdx)
        {
            tingPaiFlagSeat[seatIdx].gameObject.SetActive(false);
        }

        public void ShowHuaSeFlag(int seatIdx, Sprite huaseSprite)
        {
            huaSeFlagSeat[seatIdx].GetComponent<Image>().sprite = huaseSprite;
            huaSeFlagSeat[seatIdx].gameObject.SetActive(true);
        }

        public void ShowHuaSeFlag(int seatIdx, MahjongHuaSe huaSe)
        {
            huaSeFlagSeat[seatIdx].GetComponent<Image>().sprite = huaSeFlag[(int)huaSe];
            huaSeFlagSeat[seatIdx].gameObject.SetActive(true);
        }


        public void HideHuaSeFlag(int seatIdx)
        {
            huaSeFlagSeat[seatIdx].gameObject.SetActive(false);
        }


        public Vector3 GetHuaSeFlagWorldPosition(int seatIdx)
        {
            return huaSeFlagSeat[seatIdx].gameObject.transform.position;
        }

        public Vector3 GetHuaSeFlagSize(int seatIdx)
        {
            return huaSeFlagSeat[seatIdx].gameObject.GetComponent<RectTransform>().sizeDelta;
        }

        /// <summary>
        /// 头像流光轮转到指定座位
        /// </summary>
        /// <param name="seatIdx"></param>
        public void LiuGuangTurnTo(int seatIdx)
        {
            for (int i = 0; i < seatTouXiangLiuGuangEffects.Count(); i++)
            {
                if (seatTouXiangLiuGuangEffects[i] != null)
                {
                    if (i == seatIdx)
                    {
                        canvasGroup[i].alpha = 1;
                        state[i] = 0;
                        seatTouXiangLiuGuangEffects[i].Run();
                    }
                    else
                    {
                        canvasGroup[i].alpha = 1;
                        fadeTweener[i].Pause();
                        state[i] = -1;
                        seatTouXiangLiuGuangEffects[i].Stop();
                    }
                }
            }
        }

        public void LiuGuangStop(int seatIdx)
        {
            for (int i = 0; i < seatTouXiangLiuGuangEffects.Count(); i++)
            {
                if (seatTouXiangLiuGuangEffects[i] != null)
                {
                    if (i == seatIdx)
                    {
                        canvasGroup[i].alpha = 1;
                        fadeTweener[i].Pause();
                        state[i] = -1;
                        seatTouXiangLiuGuangEffects[i].Stop();
                    }
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < MahjongMachine.playerCount; i++)
            {
                if (playerTouXiangSeat[i] == null ||
                    playerTouXiangSeat[i].gameObject.activeSelf == false)
                    continue;

                if (mjMachine.isCanUseSeatPlayer[i])
                    seatTouXiangLiuGuangEffects[i].Update();


                switch (state[i])
                {
                    case 0:
                        float totalTime = seatTouXiangLiuGuangEffects[i].GetTotalTime();
                        float duration = seatTouXiangLiuGuangEffects[i].duration;

                        if (totalTime >= duration * 0.8f)
                        {
                            fadeTweener[i].Play();
                            state[i] = -1;
                        }

                        break;
                }
            }
        }
    }
}