using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MahjongMachineNS
{
    public class UISelectQueMenMoveHuaSe
    {
        public MahjongAssetsMgr mjAssetsMgr;

        UISelectQueMen uiQueMen;

        GameObject uiHuaSeMove;
        Image uiHuaSeMoveImage;

        GameObject uiHuaSeFlag;
        Image uiHuaSeFlagImage;

        int seatIdx;
        MahjongHuaSe huaSe;

        int state = -1;
        float stateStartTime;
        float stateLiveTime;
        public UISelectQueMenMoveHuaSe(UISelectQueMen uiQueMen, int seatIdx)
        {
            this.uiQueMen = uiQueMen;
            this.mjAssetsMgr = uiQueMen.mjAssetsMgr;
            this.seatIdx = seatIdx;
        }

        public void Load()
        {
            uiHuaSeMove = Object.Instantiate(uiQueMen.prefabUISprite, uiQueMen.uiSelectQueYiMen.transform);
            mjAssetsMgr.AppendToDestoryPool(uiHuaSeMove);
            uiHuaSeMoveImage = uiHuaSeMove.GetComponent<Image>();
            uiHuaSeMove.SetActive(false);

            uiHuaSeFlag = Object.Instantiate(uiQueMen.prefabUIHuaSeFlag, uiQueMen.uiSelectQueYiMen.transform);
            mjAssetsMgr.AppendToDestoryPool(uiHuaSeFlag);

            ParticleSystem.MainModule main = uiHuaSeFlag.transform.Find("Guang").GetComponent<ParticleSystem>().main;
            main.duration = 0.4f;
            main.startLifetime = 0.4f;

            main = uiHuaSeFlag.transform.Find("OutGuangQuan").GetComponent<ParticleSystem>().main;
            main.duration = 0.6f;
            main.startLifetime = 0.6f;

            uiHuaSeFlag.SetActive(false);
            uiHuaSeFlagImage = uiHuaSeFlag.transform.Find("HuaSe").GetComponent<Image>();
        }


        public void Play(MahjongHuaSe huaSe)
        {
            if (state >= 0)
                return;

            this.huaSe = huaSe;
            SetState(0, 0);
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
                        uiHuaSeMoveImage.sprite = uiQueMen.queHuaSeMove[(int)huaSe];
                        uiHuaSeMove.GetComponent<RectTransform>().sizeDelta = new Vector2(uiQueMen.queHuaSeMove[(int)huaSe].rect.width * 1.3f, uiQueMen.queHuaSeMove[(int)huaSe].rect.height * 1.3f);
                        uiHuaSeMove.transform.position = uiQueMen.huaSeStartPosSeat[seatIdx];
                        uiHuaSeMoveImage.color = Color.white;
                        uiHuaSeMove.SetActive(true);

                        uiHuaSeFlag.transform.position = uiQueMen.uiTouXiang.GetHuaSeFlagWorldPosition(seatIdx);
                        uiHuaSeFlagImage.sprite = uiQueMen.huaSeFlag[(int)huaSe];
                        uiHuaSeFlag.SetActive(false);

                        Vector3 pos = uiQueMen.uiTouXiang.GetHuaSeFlagWorldPosition(seatIdx);
                        uiHuaSeMove.transform.DOMove(pos, 0.5f);
                        SetState(1, 0.5f);
                    }
                    break;

                case 1:
                    {
                        uiHuaSeMoveImage.DOFade(0, 0.4f);
                        uiHuaSeMove.transform.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.4f);

                        uiHuaSeFlagImage.color = new Color(1, 1, 1, 0);
                        uiHuaSeFlagImage.DOFade(1, 0.6f);
                        uiHuaSeFlag.SetActive(true);

                        SetState(2, 0.6f);

                    }
                    break;

                case 2:
                    {
                        uiHuaSeMove.SetActive(false);
                        uiHuaSeMove.transform.localScale = Vector3.one;
                        uiHuaSeFlag.SetActive(false);

                        uiQueMen.uiTouXiang.ShowHuaSeFlag(seatIdx, uiQueMen.huaSeFlag[(int)huaSe]);
                        uiQueMen.selectedQueMenPlayerCount++;
                        SetState(-1, 0);
                    }
                    break;

            }
        }

        void SetState(int _state, float liveTime)
        {
            state = _state;
            stateStartTime = Time.time;
            stateLiveTime = liveTime;
        }

    }
}