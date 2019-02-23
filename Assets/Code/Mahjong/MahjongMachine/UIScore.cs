using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace MahjongMachineNS
{
    public class UIScore
    {
        MahjongMachine mjMachine;
        MahjongAssets mjAssets;
        MahjongAssetsMgr mjAssetsMgr;
        MahjongGame mjGame;
        Transform uiCanvasTransform;

        GameObject prefabUIScoreDec;
        GameObject prefabUIScoreInc;
        GameObject prefabSpeedLine;
        GameObject prefabStarShan;

        GameObject[] uiScoreInc = new GameObject[4];
        GameObject[] uiScoreDec = new GameObject[4];
        GameObject[] uiScore = new GameObject[4];

        GameObject[] uiSpeedLine = new GameObject[4];
        bool[] isShowSpeedLine = new bool[4] { false, false, false, false };

        GameObject[] uiStarShan = new GameObject[4];
        bool[] isShowStarShan = new bool[4] { false, false, false, false };

        float textOffset = -300f;
        float speedLineOffset = -400f;
        float starShanOffset = -100f;
        Vector3 offset = new Vector3(0, 35, 0);

        int[] state = new int[] { -1, -1, -1, -1 };
        float[] stateStartTime = new float[] { 0, 0, 0, 0 };
        float[] stateLiveTime = new float[] { 0, 0, 0, 0 };

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjGame = mjMachine.mjGame;
            mjAssets = mjMachine.mjAssets;
            mjAssetsMgr = mjMachine.mjAssetsMgr;
            uiCanvasTransform = mjMachine.uiCanvasTransform;

            prefabUIScoreDec = mjAssets.uiPrefabDict[(int)PrefabIdx.UI_SCORE][0];
            prefabUIScoreInc = mjAssets.uiPrefabDict[(int)PrefabIdx.UI_SCORE][1];

            prefabSpeedLine = mjAssets.effectPrefabDict[(int)PrefabIdx.UI_SPEED_LINE_EFFECT][0];

            prefabStarShan = mjAssets.effectPrefabDict[(int)PrefabIdx.UI_STAR_SHAN_EFFECT][0];
        }

        public void Load()
        {
            for (int i = 0; i < 4; i++)
            {
                uiScoreDec[i] = Object.Instantiate(prefabUIScoreDec, uiCanvasTransform);
                uiScoreDec[i].SetActive(false);
                uiScoreDec[i].transform.localPosition = mjMachine.uiScorePosSeat[i] + offset;
                mjAssetsMgr.AppendToDestoryPool(uiScoreDec[i]);

                uiScoreInc[i] = Object.Instantiate(prefabUIScoreInc, uiCanvasTransform);
                uiScoreInc[i].SetActive(false);
                uiScoreInc[i].transform.localPosition = mjMachine.uiScorePosSeat[i] + offset;
                mjAssetsMgr.AppendToDestoryPool(uiScoreInc[i]);

                uiSpeedLine[i] = Object.Instantiate(prefabSpeedLine, uiCanvasTransform);
                uiSpeedLine[i].transform.localPosition = mjMachine.uiScorePosSeat[i];
                uiSpeedLine[i].SetActive(false);
                mjAssetsMgr.AppendToDestoryPool(uiSpeedLine[i]);

                uiStarShan[i] = Object.Instantiate(prefabStarShan, uiCanvasTransform);
                uiStarShan[i].transform.localPosition = mjMachine.uiScorePosSeat[i];
                uiStarShan[i].SetActive(false);
                mjAssetsMgr.AppendToDestoryPool(uiStarShan[i]);
            }
        }

        public void Show(int[] seatScores)
        {
            for (int i = 0; i < 4; i++)
            {
                if (seatScores[i] == 0 || state[i] >= 0)
                    continue;

                if (seatScores[i] < 0)
                {
                    uiScore[i] = uiScoreDec[i];
                    isShowSpeedLine[i] = false;
                    isShowStarShan[i] = false;
                }
                else
                {
                    uiScore[i] = uiScoreInc[i];
                    isShowSpeedLine[i] = true;
                    isShowStarShan[i] = true;
                }

                SetState(i, 0, 0);
            }
        }

        public void Update()
        {
            UpdateSeat(0);
            UpdateSeat(1);
            UpdateSeat(2);
            UpdateSeat(3);
        }

        void UpdateSeat(int seatIdx)
        {
            if (state[seatIdx] < 0 ||
                Time.time - stateStartTime[seatIdx] < stateLiveTime[seatIdx])
            {
                return;
            }

            switch (state[seatIdx])
            {
                case 0:
                    {
                        if (isShowSpeedLine[seatIdx])
                        {
                            uiSpeedLine[seatIdx].transform.localPosition = mjMachine.uiScorePosSeat[seatIdx] + new Vector3(speedLineOffset, 0, 0);
                            uiSpeedLine[seatIdx].GetComponent<ParticleSystem>().Stop();
                        }

                        if (isShowStarShan[seatIdx])
                        {
                            uiStarShan[seatIdx].transform.localPosition = mjMachine.uiScorePosSeat[seatIdx] + new Vector3(starShanOffset, 0, 0);
                            uiStarShan[seatIdx].GetComponent<ParticleSystem>().Stop();
                        }

                        uiScore[seatIdx].transform.localPosition = mjMachine.uiScorePosSeat[seatIdx] + offset;
                        uiScore[seatIdx].SetActive(true);
                        SetState(seatIdx, 1, 0.02f);
                    }
                    break;

                case 1:
                    {
                        if (isShowSpeedLine[seatIdx])
                            uiSpeedLine[seatIdx].SetActive(true);

                        if (isShowStarShan[seatIdx])
                            uiStarShan[seatIdx].SetActive(true);

                        Vector3 pos = uiScore[seatIdx].transform.localPosition;
                        pos.x += textOffset;
                        uiScore[seatIdx].transform.localPosition = pos;
                        uiScore[seatIdx].transform.DOBlendableLocalMoveBy(new Vector3(-textOffset, 0, 0), 0.15f);
                        SetState(seatIdx, 2, 1f);
                    }
                    break;

                case 2:
                    {
                        if (isShowSpeedLine[seatIdx])
                            uiSpeedLine[seatIdx].SetActive(false);

                        if (isShowStarShan[seatIdx])
                            uiStarShan[seatIdx].SetActive(false);

                        uiScore[seatIdx].SetActive(false);
                        SetState(seatIdx, -1, 0);
                    }
                    break;
            }
        }

        void SetState(int seatIdx, int _state, float liveTime)
        {
            state[seatIdx] = _state;
            stateStartTime[seatIdx] = Time.time;
            stateLiveTime[seatIdx] = liveTime;
        }

        public void Destory()
        {
            for (int i = 0; i < 4; i++)
            {
                Object.Destroy(uiScoreInc[i]);
                Object.Destroy(uiScoreDec[i]);
                Object.Destroy(uiSpeedLine[i]);
                Object.Destroy(uiStarShan[i]);
            }
        }
    }
}