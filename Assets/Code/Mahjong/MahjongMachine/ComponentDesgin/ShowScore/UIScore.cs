using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using CoreDesgin;

namespace ComponentDesgin
{
    public class UIScore: MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        Scene scene;
        PreSettingHelper preSettingHelper;
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

        public override void Init()
        {
            base.Init();
            Setting((MahjongMachine)Parent);
        }

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
            scene = mjMachine.GetComponent<Scene>();
            uiCanvasTransform = scene.uiCanvasTransform;

            prefabUIScoreDec = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_SCORE][0];
            prefabUIScoreInc = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_SCORE][1];

            prefabSpeedLine = mjAssetsMgr.effectPrefabDict[(int)PrefabIdx.UI_SPEED_LINE_EFFECT][0];

            prefabStarShan = mjAssetsMgr.effectPrefabDict[(int)PrefabIdx.UI_STAR_SHAN_EFFECT][0];
        }

        public override void Load()
        {
            for (int i = 0; i < 4; i++)
            {
                uiScoreDec[i] = Object.Instantiate(prefabUIScoreDec, uiCanvasTransform);
                uiScoreDec[i].SetActive(false);
                uiScoreDec[i].transform.localPosition = preSettingHelper.uiScorePosSeat[i] + offset;
                mjAssetsMgr.AppendToDestoryPool(uiScoreDec[i]);

                uiScoreInc[i] = Object.Instantiate(prefabUIScoreInc, uiCanvasTransform);
                uiScoreInc[i].SetActive(false);
                uiScoreInc[i].transform.localPosition = preSettingHelper.uiScorePosSeat[i] + offset;
                mjAssetsMgr.AppendToDestoryPool(uiScoreInc[i]);

                uiSpeedLine[i] = Object.Instantiate(prefabSpeedLine, uiCanvasTransform);
                uiSpeedLine[i].transform.localPosition = preSettingHelper.uiScorePosSeat[i];
                uiSpeedLine[i].SetActive(false);
                mjAssetsMgr.AppendToDestoryPool(uiSpeedLine[i]);

                uiStarShan[i] = Object.Instantiate(prefabStarShan, uiCanvasTransform);
                uiStarShan[i].transform.localPosition = preSettingHelper.uiScorePosSeat[i];
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
                            uiSpeedLine[seatIdx].transform.localPosition = preSettingHelper.uiScorePosSeat[seatIdx] + new Vector3(speedLineOffset, 0, 0);
                            uiSpeedLine[seatIdx].GetComponent<ParticleSystem>().Stop();
                        }

                        if (isShowStarShan[seatIdx])
                        {
                            uiStarShan[seatIdx].transform.localPosition = preSettingHelper.uiScorePosSeat[seatIdx] + new Vector3(starShanOffset, 0, 0);
                            uiStarShan[seatIdx].GetComponent<ParticleSystem>().Stop();
                        }

                        uiScore[seatIdx].transform.localPosition = preSettingHelper.uiScorePosSeat[seatIdx] + offset;
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