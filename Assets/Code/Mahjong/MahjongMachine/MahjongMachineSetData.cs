using Assets;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MahjongMachineNS
{
    public partial class MahjongMachine
    {
        public void Start(MahjongGame mjGame)
        {
            this.mjAssets = MahjongGame.mjAssets;
            this.mjGame = mjGame;
            this.gameObject = mjGame.gameObject;

            Init();


        }

        #region 初始化数据
        void Init()
        {
            SelfSelectDaPaiEnd = SelectDaPaiEnd;
            SelfSelectSwapPaiEnd = SelectSwapPaiEnd;
            SelfSelectQueMenEnd = SelectQueMenEnd;
            HandQiDongDiceMachine = QiDong;

            moPaiToHandPaiCanvasOffset = moPaiToHandPaiOffset * 5000f;

            mjtableTransform = gameObject.transform.Find("MahjongDesk");
            mjTablePositionHelperTransform = mjtableTransform.transform.Find("MahjongTablePositionHelper");


            uiCanvasTransform = gameObject.transform.Find("UICanvas");
            canvasRectTransform = uiCanvasTransform.GetComponent<RectTransform>();
            uiPositionHelperTransform = uiCanvasTransform.Find("UIPositionHelper");


            canvasHandPaiTransform = gameObject.transform.Find("HandPaiCanvas");
            canvasHandPaiRectTransform = canvasHandPaiTransform.GetComponent<RectTransform>();

            cameraTransform = gameObject.transform.Find("3DCamera");
            bgMusicAudioSource = cameraTransform.GetComponent<AudioSource>();




            FitScreen(Screen.width, Screen.height);


            mjAssetsMgr.Setting(this);
            mjAssetsMgr.Load(LoadPoolsComplete);
        }

        void LoadPoolsComplete()
        {
            isGameAllReady = true;

            tingDatas = MahjongAssetsMgr.mjHuTingCheck.CreateTingDataMemory();

            premj = mjAssetsMgr.mjPai[(int)MahjongFaceValue.MJ_ZFB_FACAI];
            premjSize = premj.transform.GetComponent<Renderer>().bounds.size;
            handPaiSelectOffsetHeight = GetCanvasHandMjSizeByAxis(Axis.Y) / 3;

            CreateAudios();
            CreateInitLayoutPosForSeats();

            SetHandShadowShaders();
            SetSeatCanUse();
            SetSeatMjTuoCanDuiPai();

            swapPaiHintArrowEffect.Setting(this);
            swapPaiHintArrowEffect.Load();

            uiSelectSwapHandPai.Setting(this);
            uiSelectSwapHandPai.Load();

            uiSwapPaiingTips.Setting(this);
            uiSwapPaiingTips.Load();

            uiTouXiang.Setting(this);
            uiTouXiang.Load();

            uiHuPaiTips.Setting(this);
            uiHuPaiTips.Load();

            uiChiPaiTips.Setting(this);
            uiChiPaiTips.Load();

            uiHuPaiTipsArrow.Setting(this);
            uiPcghtBtnMgr.Setting(this);

            uiScore.Setting(this);
            uiScore.Load();

            uiSelectQueMen.Setting(this);
            uiSelectQueMen.Load();


            diceMachine.Setting(this);
            diceMachine.Load();

            mjPoint.Setting(this);
            mjPoint.Load();

            mjOpCmdList.Init(this);

            CreateHands();
            SetHandsActionsPlaySpeed();
            SetMjDaPaiFirstHandPosAndEulerAngles();

           
            CreateMahjongPaiDui();
            CreateMjToHandOffsetVector();

            CreateDeskMjPosViewHandDaPaiActionPoints();


            SetDealer(1, FengWei.SOUTH);

            TestMahjongFuncInterface();
        }

        public void Destory()
        {
            for (int i = 0; i < mjDuiPai.Length; i++)
                Object.Destroy(mjDuiPai[i]);

            DestroyAllHand();
            mjAssetsMgr.Destroy();
            mjAssets.Destroy();
        }
        void ResetData()
        {
            for (int i = 0; i < 4; i++)
                playerStateData[0].Clear();

            curtPaiDuiPos = 0;
            paiDuiRichCount = mjPaiTotalCount;

            highLightMjValue = MahjongFaceValue.MJ_UNKNOWN;

            RestDeskPengPaiCurtPos();

            ClearMjMoPaiList();
            ClearMjHandPaiList();
            ClearDeskGlobalMjPaiSetDict();

            for (int i = 0; i < curtDaPaiMjSeatDeskPosIdx.Length; i++)
            {
                curtDaPaiMjSeatDeskPosIdx[i] = new Vector3Int(0, -1, 0);
            };

            for (int i = 0; i < curtHuPaiMjSeatDeskPosIdx.Length; i++)
            {
                curtHuPaiMjSeatDeskPosIdx[i] = -1;
            };

            ShowOrHideMjDuiPai(true);


            GameObject go;
            Dictionary<int, GameObject> dict;
            for (int i = 0; i < deskDaPaiMjDicts.Length; i++)
            {
                dict = deskDaPaiMjDicts[i];
                foreach (var item in dict)
                {
                    go = item.Value;
                    if (go != null)
                    {
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(go);
                    }
                }

                dict.Clear();
            }

            for (int i = 0; i < deskHuPaiMjDicts.Length; i++)
            {
                dict = deskHuPaiMjDicts[i];
                foreach (var item in dict)
                {
                    go = item.Value;
                    if (go != null)
                    {
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(go);
                    }
                }

                dict.Clear();
            }

            for (int i = 0; i < deskPengPaiMjList.Length; i++)
            {
                for (int j = 0; j < deskPengPaiMjList[i].Count; j++)
                {
                    for (int k = 0; k < deskPengPaiMjList[i][j].Length; k++)
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(deskPengPaiMjList[i][j][k]);
                }

                deskPengPaiMjList[i].Clear();
            }

            for (int i = 0; i < deskPengPaiMjPosInfoList.Length; i++)
            {
                deskPengPaiMjPosInfoList[i].Clear();
            }


            for (int i = 0; i < deskGangPaiMjList.Length; i++)
            {
                for (int j = 0; j < deskGangPaiMjList[i].Count; j++)
                {
                    for (int k = 0; k < deskGangPaiMjList[i][j].Length; k++)
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(deskGangPaiMjList[i][j][k]);
                }

                deskGangPaiMjList[i].Clear();
            }


            for (int i = 0; i < deskChiPaiMjList.Length; i++)
            {
                for (int j = 0; j < deskChiPaiMjList[i].Count; j++)
                {
                    for (int k = 0; k < deskChiPaiMjList[i][j].Length; k++)
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(deskChiPaiMjList[i][j][k]);
                }

                deskChiPaiMjList[i].Clear();
            }
        }

        #endregion

     
        #region 生成初始布局位置

        /// <summary>
        /// 生成初始布局位置
        /// </summary>
        void CreateInitLayoutPosForSeats()
        {
            CreateMjHandPaiPosList(mjHandCount);

            CreateDiceQiDongPosForSeats();
            CreateSwapPaiCenterPosForSeats();
            CreateDeskPengPaiStartPosForSeats();
            CreateFengRainEffectPosForSeats();
            CreatePcgthEffectTextPosForSeats();
            CreateUIScorePosForSeats();
            CreateUITouXiangPosForSeats();
            CreateUIDingQueMoveHuaseStartPosForSeats();
            CreateMjDeskDaPaiPosForSeats();
            CreateMjDeskHuPaiPosForSeats();
            CreateDeskMjTuoPosForSeats();
        }


        /// <summary>
        /// 生成骰子启动位置
        /// </summary>
        void CreateDiceQiDongPosForSeats()
        {
            Transform diceQiDongPos = mjTablePositionHelperTransform.Find("DiceQiDongPos");

            diceQiDongPosSeat[0] = diceQiDongPos.Find("seat0").transform.position;
            diceQiDongPosSeat[1] = diceQiDongPos.Find("seat1").transform.position;
            diceQiDongPosSeat[2] = diceQiDongPos.Find("seat2").transform.position;
            diceQiDongPosSeat[3] = diceQiDongPos.Find("seat3").transform.position;
        }

        /// <summary>
        /// 生成各座位碰吃杠听胡牌文字特效位置
        /// </summary>
        void CreatePcgthEffectTextPosForSeats()
        {
            Transform pcghtEffectTextPosTransform = uiPositionHelperTransform.Find("HuChiGangPengTingEffectTextPos");

            pcgthEffectTextPosSeat[0] = pcghtEffectTextPosTransform.Find("seat0").localPosition;
            pcgthEffectTextPosSeat[1] = pcghtEffectTextPosTransform.Find("seat1").localPosition;
            pcgthEffectTextPosSeat[2] = pcghtEffectTextPosTransform.Find("seat2").localPosition;
            pcgthEffectTextPosSeat[3] = pcghtEffectTextPosTransform.Find("seat3").localPosition;
        }

        /// <summary>
        /// 生成各座位UI分数显示位置
        /// </summary>
        void CreateUIScorePosForSeats()
        {
            Transform scorePos = uiPositionHelperTransform.Find("ScorePos");
            uiScorePosSeat[0] = scorePos.Find("seat0").transform.localPosition;
            uiScorePosSeat[1] = scorePos.Find("seat1").transform.localPosition;
            uiScorePosSeat[2] = scorePos.Find("seat2").transform.localPosition;
            uiScorePosSeat[3] = scorePos.Find("seat3").transform.localPosition;
        }

        /// <summary>
        /// 生成各座位UI头像显示位置
        /// </summary>
        void CreateUITouXiangPosForSeats()
        {
            Transform touxiangPos = uiPositionHelperTransform.Find("TouXiangPos");
            uiTouXiangPosSeat[0] = touxiangPos.Find("seat0").transform.localPosition;
            uiTouXiangPosSeat[1] = touxiangPos.Find("seat1").transform.localPosition;
            uiTouXiangPosSeat[2] = touxiangPos.Find("seat2").transform.localPosition;
            uiTouXiangPosSeat[3] = touxiangPos.Find("seat3").transform.localPosition;
        }

        /// <summary>
        /// 生成各座位UI定缺移动花色的初始位置
        /// </summary>
        void CreateUIDingQueMoveHuaseStartPosForSeats()
        {
            Transform huaseStartPos = uiPositionHelperTransform.Find("DingQueMoveHuaSeStartPos");
            huaSeStartPosSeat[1] = huaseStartPos.Find("seat1").transform.position;
            huaSeStartPosSeat[2] = huaseStartPos.Find("seat2").transform.position;
            huaSeStartPosSeat[3] = huaseStartPos.Find("seat3").transform.position;
        }


        /// <summary>
        /// 生成刮风下雨特效位置
        /// </summary>
        void CreateFengRainEffectPosForSeats()
        {
            Transform fengEffectPosTransform = mjTablePositionHelperTransform.Find("FengEffectPos");
            fengEffectPos[0] = fengEffectPosTransform.Find("seat0").localPosition;
            fengEffectPos[1] = fengEffectPosTransform.Find("seat1").localPosition;
            fengEffectPos[2] = fengEffectPosTransform.Find("seat2").localPosition;
            fengEffectPos[3] = fengEffectPosTransform.Find("seat3").localPosition;

            //
            Transform rainEffectPosTransform = mjTablePositionHelperTransform.Find("RainEffectPos");
            rainEffectPos[0] = rainEffectPosTransform.localPosition;
            rainEffectPos[1] = rainEffectPosTransform.localPosition;
            rainEffectPos[2] = rainEffectPosTransform.localPosition;
            rainEffectPos[3] = rainEffectPosTransform.localPosition;
        }


        /// <summary>
        /// 生成桌面麻将托位置
        /// </summary>
        void CreateDeskMjTuoPosForSeats()
        {
            for(int i = 0; i < 4; i++)
                deskMjTuoPos[i] = mjtableTransform.Find(deskMjTuoName[i]).localPosition;
        }

        #region 生成交换牌中心位置
        /// <summary>
        /// 生成交换牌中心位置
        /// </summary>
        void CreateSwapPaiCenterPosForSeats()
        {
            Transform swapPaiPosTransform = mjTablePositionHelperTransform.Find("SwapPaiPos");
            float r = swapPaiPosTransform.position.z;
            swapPaiCenterPosSeat[0] = new Vector3(0, 0, r);
            swapPaiCenterPosSeat[1] = new Vector3(r, 0, 0);
            swapPaiCenterPosSeat[2] = new Vector3(0, 0, -r);
            swapPaiCenterPosSeat[3] = new Vector3(-r, 0, 0);

            Transform swapPaiControler = mjtableTransform.Find("SwapPaiControler");

            swapPaiControlerSeat[0] = swapPaiControler.Find("seat0");
            swapPaiControlerSeat[1] = swapPaiControler.Find("seat1");
            swapPaiControlerSeat[2] = swapPaiControler.Find("seat2");
            swapPaiControlerSeat[3] = swapPaiControler.Find("seat3");

        }

        GameObject CreateSwapPaiGroup(int seatIdx, Vector3 atDeskPos, MahjongFaceValue[] mjFaceValues, SwapPaiDirection dir, bool isShowBack = false)
        {
            float width = GetDeskMjSizeByAxis(Axis.X);
            float y = deskFacePosY + GetDeskMjSizeByAxis(Axis.Z) / 2;
            float totalWidth = mjFaceValues.Length * width;

            if (dir != SwapPaiDirection.OPPOSITE)
                swapPaiControlerSeat[seatIdx].position = new Vector3(0, 0, 0);
            else
                swapPaiControlerSeat[seatIdx].position = atDeskPos;

            swapPaiControlerSeat[seatIdx].localEulerAngles = new Vector3(0, seatIdx * 90, 0);

            switch (seatIdx)
            {
                case 0:
                    {
                        float curtpos = atDeskPos.x + totalWidth / 2 - width / 2;
                        GameObject mj;

                        for (int i = 0; i < mjFaceValues.Length; i++)
                        {
                            mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mj.layer = defaultLayer;
                            FitSeatDeskMj(seatIdx, mj, true, 0.91f, isShowBack);
                            mj.transform.position = new Vector3(curtpos, y, atDeskPos.z);
                            mj.transform.SetParent(swapPaiControlerSeat[seatIdx], true);
                            curtpos -= width;
                        }
                    }
                    break;

                case 1:
                    {
                        float curtpos = atDeskPos.z - totalWidth / 2 + width / 2;
                        GameObject mj;

                        for (int i = 0; i < mjFaceValues.Length; i++)
                        {
                            mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mj.layer = defaultLayer;
                            FitSeatDeskMj(seatIdx, mj, true, 0.91f, isShowBack);
                            mj.transform.position = new Vector3(atDeskPos.x, y, curtpos);
                            mj.transform.SetParent(swapPaiControlerSeat[seatIdx], true);
                            curtpos += width;
                        }
                    }
                    break;


                case 2:
                    {
                        float curtpos = atDeskPos.x - totalWidth / 2 + width / 2;
                        GameObject mj;

                        for (int i = 0; i < mjFaceValues.Length; i++)
                        {
                            mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mj.layer = defaultLayer;
                            FitSeatDeskMj(seatIdx, mj, true, 0.91f, isShowBack);
                            mj.transform.position = new Vector3(curtpos, y, atDeskPos.z);
                            mj.transform.SetParent(swapPaiControlerSeat[seatIdx], true);
                            curtpos += width;
                        }
                    }
                    break;

                case 3:
                    {
                        float curtpos = atDeskPos.z + totalWidth / 2 - width / 2;
                        GameObject mj;

                        for (int i = 0; i < mjFaceValues.Length; i++)
                        {
                            mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValues[i]);
                            mj.layer = defaultLayer;
                            FitSeatDeskMj(seatIdx, mj, true, 0.91f, isShowBack);
                            mj.transform.position = new Vector3(atDeskPos.x, y, curtpos);
                            mj.transform.SetParent(swapPaiControlerSeat[seatIdx], true);
                            curtpos -= width;
                        }
                    }
                    break;

            }

            return swapPaiControlerSeat[seatIdx].gameObject;
        }


        #endregion

        #region 生成碰，吃，杠牌的起始位置
        /// <summary>
        /// 生成碰，吃，杠牌的起始位置
        /// </summary>
        void CreateDeskPengPaiStartPosForSeats()
        {
            Transform pengPaiPos = mjTablePositionHelperTransform.Find("PengPaiPos");
            float y = deskFacePosY + GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.0002f;

            pengPaiStartPosSeat[0] = pengPaiPos.Find("seat0").transform.position;
            pengPaiStartPosSeat[1] = pengPaiPos.Find("seat1").transform.position;
            pengPaiStartPosSeat[2] = pengPaiPos.Find("seat2").transform.position;
            pengPaiStartPosSeat[3] = pengPaiPos.Find("seat3").transform.position;

            pengPaiStartPosSeat[0].y = y;
            pengPaiStartPosSeat[1].y = y;
            pengPaiStartPosSeat[2].y = y;
            pengPaiStartPosSeat[3].y = y;

            pengPaiCurtPosSeat[0] = pengPaiStartPosSeat[0];
            pengPaiCurtPosSeat[1] = pengPaiStartPosSeat[1];
            pengPaiCurtPosSeat[2] = pengPaiStartPosSeat[2];
            pengPaiCurtPosSeat[3] = pengPaiStartPosSeat[3];
        }

        void RestDeskPengPaiCurtPos()
        {
            pengPaiCurtPosSeat[0] = pengPaiStartPosSeat[0];
            pengPaiCurtPosSeat[1] = pengPaiStartPosSeat[1];
            pengPaiCurtPosSeat[2] = pengPaiStartPosSeat[2];
            pengPaiCurtPosSeat[3] = pengPaiStartPosSeat[3];
        }


        /// <summary>
        /// 获取碰吃杠牌的摆放布局位置列表（按照给定牌摆放起始位置paiStartPos）
        /// </summary>
        /// <param name="seatIdx">座位号</param>
        /// <param name="pcgType">碰吃杠类型</param>
        /// <param name="paiStartPos">给定的牌摆放起始位置</param>
        /// <param name="paiLayoutIdx">摆放布局牌型</param>
        /// <param name="mjSpacing"></param>
        /// <returns></returns> 
        PengChiGangPaiPos[] GetPengChiGangPaiPosList(int seatIdx, PengChiGangPaiType pcgType, Vector3 paiStartPos, int paiLayoutIdx, float mjSpacing = 0.0001f)
        {
            HandDirection dir = pengPaiHandDirSeat[seatIdx];
            return GetPengChiGangPaiPosList(seatIdx, dir, pcgType, paiStartPos, paiLayoutIdx, mjSpacing);
        }

        /// <summary>
        /// 获取碰吃杠牌的位置列表
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="dir"></param>
        /// <param name="pcgType"></param>
        /// <param name="paiLayoutIdx"></param>
        /// <param name="mjSpacing"></param>
        /// <returns></returns>
        PengChiGangPaiPos[] GetPengChiGangPaiPosList(
            int seatIdx, HandDirection dir,
            PengChiGangPaiType pcgType, Vector3 paiStartPos, int paiLayoutIdx, float mjSpacing = 0.0001f)
        {
            float mjSizeX = GetDeskPengChiGangMjSizeByAxis(Axis.X);
            float mjSizeY = GetDeskPengChiGangMjSizeByAxis(Axis.Y);
            float handSign = 1;
            float fbSign = 1;

            PengChiGangPaiPos[] mjPos = null;
            Vector3 pevPos = paiStartPos;

            if (pcgType == PengChiGangPaiType.PENG || pcgType == PengChiGangPaiType.CHI)
                mjPos = new PengChiGangPaiPos[3] { new PengChiGangPaiPos(), new PengChiGangPaiPos(), new PengChiGangPaiPos() };
            else
                mjPos = new PengChiGangPaiPos[4] { new PengChiGangPaiPos(), new PengChiGangPaiPos(), new PengChiGangPaiPos(), new PengChiGangPaiPos() };

            switch (seatIdx)
            {
                case 0:
                    if (dir == HandDirection.LeftHand) { handSign = -1; }
                    else { handSign = 1; }
                    fbSign = -1;
                    break;

                case 1:
                    if (dir == HandDirection.LeftHand) { handSign = 1; }
                    else { handSign = -1; }
                    fbSign = -1;
                    break;

                case 2:
                    if (dir == HandDirection.LeftHand) { handSign = 1; }
                    else { handSign = -1; }
                    fbSign = 1;
                    break;

                case 3:
                    if (dir == HandDirection.LeftHand) { handSign = -1; }
                    else { handSign = 1; }
                    fbSign = 1;
                    break;
            }


            switch (seatIdx)
            {
                case 0:
                case 2:

                    if (pcgType == PengChiGangPaiType.CHI)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            mjPos[i].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                            mjPos[i].layouyDirSeat = seatIdx;
                            pevPos.x += handSign * (mjSizeX + mjSpacing);
                        }
                    }
                    else if (pcgType == PengChiGangPaiType.PENG)
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        mjPos[i].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                        mjPos[i].layouyDirSeat = seatIdx;
                                        pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    }
                                }
                                break;

                            case 1:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[0].layouyDirSeat = seatIdx;

                                    pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    mjPos[1].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2);
                                    mjPos[1].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    pevPos.x += handSign * (mjSizeY + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[2].layouyDirSeat = seatIdx;
                                }
                                break;


                            case 2:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2);
                                    mjPos[0].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    mjPos[1].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2);
                                    mjPos[1].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    pevPos.x += handSign * (mjSizeY + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[2].layouyDirSeat = seatIdx;
                                }
                                break;
                        }

                    }
                    else
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[0].layouyDirSeat = seatIdx;

                                    pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    mjPos[1].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2);
                                    mjPos[1].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    mjPos[2].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2);
                                    mjPos[2].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    pevPos.x += handSign * (mjSizeY + mjSpacing);
                                    mjPos[3].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[3].layouyDirSeat = seatIdx;
                                }
                                break;

                            case 1:
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        mjPos[i].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                        mjPos[i].layouyDirSeat = seatIdx;
                                        pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    }
                                }
                                break;

                            case 2:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2);
                                    mjPos[0].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    mjPos[1].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2);
                                    mjPos[1].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    pevPos.x += handSign * (mjSizeY + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[2].layouyDirSeat = seatIdx;

                                    pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    mjPos[3].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[3].layouyDirSeat = seatIdx;

                                }
                                break;


                            case 3:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[0].layouyDirSeat = seatIdx;

                                    pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    mjPos[1].pos = new Vector3(pevPos.x + handSign * mjSizeX / 2, pevPos.y, pevPos.z);
                                    mjPos[1].layouyDirSeat = seatIdx;

                                    pevPos.x += handSign * (mjSizeX + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2);
                                    mjPos[2].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    mjPos[3].pos = new Vector3(pevPos.x + handSign * mjSizeY / 2, pevPos.y, pevPos.z - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2);
                                    mjPos[3].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                }
                                break;

                        }
                    }

                    break;


                case 1:
                case 3:

                    if (pcgType == PengChiGangPaiType.CHI)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            mjPos[i].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                            mjPos[i].layouyDirSeat = seatIdx;
                            pevPos.z += handSign * (mjSizeX + mjSpacing);
                        }

                    }
                    else if (pcgType == PengChiGangPaiType.PENG)
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                for (int i = 0; i < 3; i++)
                                {
                                    mjPos[i].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[i].layouyDirSeat = seatIdx;
                                    pevPos.z += handSign * (mjSizeX + mjSpacing);
                                }
                                break;

                            case 1:
                                mjPos[0].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                mjPos[0].layouyDirSeat = seatIdx;

                                pevPos.z += handSign * (mjSizeX + mjSpacing);
                                mjPos[1].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                mjPos[1].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                pevPos.z += handSign * (mjSizeY + mjSpacing);
                                mjPos[2].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                mjPos[2].layouyDirSeat = seatIdx;
                                break;


                            case 2:
                                mjPos[0].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                mjPos[0].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                mjPos[1].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                mjPos[1].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                pevPos.z += handSign * (mjSizeY + mjSpacing);
                                mjPos[2].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                mjPos[2].layouyDirSeat = seatIdx;
                                break;

                        }

                    }
                    else
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[0].layouyDirSeat = seatIdx;

                                    pevPos.z += handSign * (mjSizeX + mjSpacing);
                                    mjPos[1].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[1].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    mjPos[2].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[2].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    pevPos.z += handSign * (mjSizeY + mjSpacing);
                                    mjPos[3].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[3].layouyDirSeat = seatIdx;
                                }
                                break;

                            case 1:
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        mjPos[i].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                        mjPos[i].layouyDirSeat = seatIdx;
                                        pevPos.z += handSign * (mjSizeX + mjSpacing);
                                    }
                                }
                                break;

                            case 2:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[0].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    mjPos[1].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[1].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    pevPos.z += handSign * (mjSizeY + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[2].layouyDirSeat = seatIdx;

                                    pevPos.z += handSign * (mjSizeX + mjSpacing);
                                    mjPos[3].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[3].layouyDirSeat = seatIdx;
                                }
                                break;

                            case 3:
                                {
                                    mjPos[0].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[0].layouyDirSeat = seatIdx;

                                    pevPos.z += handSign * (mjSizeX + mjSpacing);
                                    mjPos[1].pos = new Vector3(pevPos.x, pevPos.y, pevPos.z + handSign * mjSizeX / 2);
                                    mjPos[1].layouyDirSeat = seatIdx;

                                    pevPos.z += handSign * (mjSizeX + mjSpacing);
                                    mjPos[2].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[2].layouyDirSeat = GetNextSeatIdx(seatIdx);

                                    mjPos[3].pos = new Vector3(pevPos.x - fbSign * mjSizeY / 2 + fbSign * 3 * mjSizeX / 2, pevPos.y, pevPos.z + handSign * mjSizeY / 2);
                                    mjPos[3].layouyDirSeat = GetNextSeatIdx(seatIdx);


                                }
                                break;

                        }
                    }
                    break;
            }

            return mjPos;
        }

        /// <summary>
        /// 获取杠牌麻将牌当前牌号的牌的正反摆放方式
        /// </summary>
        /// <param name="paiLayoutIdx"></param>
        /// <param name="paiIdx"></param>
        /// <returns></returns>
        MahjongFaceSide GetGangPaiFaceSide(int paiLayoutIdx, int paiIdx)
        {
            int randIdx = Random.Range(0, 4);
            if (paiIdx == randIdx)
                return MahjongFaceSide.Front;
            return MahjongFaceSide.Back;
        }

        Vector3 GetNextPengChiGangPaiPos(int seatIdx)
        {
            HandDirection dir = pengPaiHandDirSeat[seatIdx];
            Vector3 curtPos = pengPaiCurtPosSeat[seatIdx];
            float sign;

            switch (seatIdx)
            {
                case 0:
                    if (dir == HandDirection.LeftHand) { sign = -1f; }
                    else { sign = 1f; }
                    curtPos.x += deskPengPaiSpacing * sign;
                    break;

                case 1:
                    if (dir == HandDirection.LeftHand) { sign = 1f; }
                    else { sign = -1f; }
                    curtPos.z += deskPengPaiSpacing * sign;
                    break;

                case 2:
                    if (dir == HandDirection.LeftHand) { sign = 1f; }
                    else { sign = -1f; }
                    curtPos.x += deskPengPaiSpacing * sign; break;

                case 3:
                    if (dir == HandDirection.LeftHand) { sign = -1f; }
                    else { sign = 1f; }
                    curtPos.z += deskPengPaiSpacing * sign;
                    break;
            }

            return curtPos;
        }

        /// <summary>
        /// 获取碰吃杠牌的起始位置[0],中心位置[1], 长度[2]
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="pcgType"></param>
        /// <param name="startPos">起始位置</param>
        /// <param name="paiLayoutIdx"></param>
        /// <param name="mjSpacing"></param>
        /// <returns></returns>
        Vector3[] GetPengChiGangPaiPos(int seatIdx, PengChiGangPaiType pcgType, Vector3 startPos, int paiLayoutIdx, out Vector3 outEndPos, float mjSpacing = 0.0001f)
        {
            HandDirection dir = pengPaiHandDirSeat[seatIdx];

            float mjSizeX = GetDeskPengChiGangMjSizeByAxis(Axis.X);
            float mjSizeY = GetDeskPengChiGangMjSizeByAxis(Axis.Y);
            float handSign = 1;

            Vector3 endPos = startPos;
            Vector3 centerPos = new Vector3(0, 0, 0);
            float len = 0;

            switch (seatIdx)
            {
                case 0:
                    if (dir == HandDirection.LeftHand) { handSign = -1; }
                    else { handSign = 1; }
                    break;

                case 1:
                    if (dir == HandDirection.LeftHand) { handSign = 1; }
                    else { handSign = -1; }
                    break;

                case 2:
                    if (dir == HandDirection.LeftHand) { handSign = 1; }
                    else { handSign = -1; }
                    break;

                case 3:
                    if (dir == HandDirection.LeftHand) { handSign = -1; }
                    else { handSign = 1; }
                    break;
            }


            switch (seatIdx)
            {
                case 0:
                case 2:

                    if (pcgType == PengChiGangPaiType.CHI)
                    {
                        endPos.x += 3 * handSign * (mjSizeX + mjSpacing);
                        centerPos = endPos;
                        centerPos.x = (startPos.x + endPos.x) / 2f;
                        len = endPos.x - startPos.x;
                    }
                    else if (pcgType == PengChiGangPaiType.PENG)
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    endPos.x += 3 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;

                            case 1:
                                {
                                    endPos.x += handSign * (mjSizeX + mjSpacing) + handSign * (mjSizeY + mjSpacing) + handSign * mjSizeX;
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;


                            case 2:
                                {
                                    endPos.x += handSign * (mjSizeY + mjSpacing) + handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    endPos.x += handSign * (mjSizeX + mjSpacing) + handSign * (mjSizeY + mjSpacing) + handSign * mjSizeX;
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;

                            case 1:
                                {
                                    endPos.x += 4 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;

                            case 2:
                            case 3:
                                {
                                    endPos.x += handSign * (mjSizeY + mjSpacing) + 2 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.x = (startPos.x + endPos.x) / 2f;
                                    len = endPos.x - startPos.x;
                                }
                                break;
                        }
                    }

                    break;


                case 1:
                case 3:

                    if (pcgType == PengChiGangPaiType.CHI)
                    {
                        endPos.z += 3 * handSign * (mjSizeX + mjSpacing);
                        centerPos = endPos;
                        centerPos.z = (startPos.z + endPos.z) / 2f;
                        len = endPos.z - startPos.z;
                    }
                    else if (pcgType == PengChiGangPaiType.PENG)
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    endPos.z += 3 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;

                            case 1:
                                {
                                    endPos.z += handSign * (mjSizeX + mjSpacing) + handSign * (mjSizeY + mjSpacing) + handSign * mjSizeX;
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;


                            case 2:
                                {
                                    endPos.z += handSign * (mjSizeY + mjSpacing) + handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (paiLayoutIdx)
                        {
                            case 0:
                                {
                                    endPos.z += handSign * (mjSizeX + mjSpacing) + handSign * (mjSizeY + mjSpacing) + handSign * mjSizeX;
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;

                            case 1:
                                {
                                    endPos.z += 4 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;

                            case 2:
                            case 3:
                                {
                                    endPos.z += handSign * (mjSizeY + mjSpacing) + 2 * handSign * (mjSizeX + mjSpacing);
                                    centerPos = endPos;
                                    centerPos.z = (startPos.z + endPos.z) / 2f;
                                    len = endPos.z - startPos.z;
                                }
                                break;

                        }
                    }
                    break;
            }

            outEndPos = endPos;
            return new Vector3[] { startPos, centerPos, new Vector3(len, len, len) };
        }


        /// <summary>
        /// 下一个碰吃杠牌的起始位置[0]和中心位置[1], 长度[2]
        /// </summary>
        /// <param name="seatIdx">座位号</param>
        /// <param name="pcgType">碰吃杠类型</param>
        /// <param name="paiLayoutIdx">摆放牌型</param>
        /// <param name="mjSpacing">每个麻将之间的间距</param>
        /// <returns></returns>
        Vector3[] NextPengChiGangPaiPos(int seatIdx, PengChiGangPaiType pcgType, int paiLayoutIdx, float mjSpacing = 0.0001f)
        {
            Vector3 startPos = GetNextPengChiGangPaiPos(seatIdx);
            Vector3 outEndValue;

            Vector3[] posinfo = GetPengChiGangPaiPos(seatIdx, pcgType, startPos, paiLayoutIdx, out outEndValue, mjSpacing);
            pengPaiCurtPosSeat[seatIdx] = outEndValue;
            return posinfo;
        }

        /// <summary>
        /// 偏移碰吃杠牌的起始位置到指定起始距离(moveHandDist)
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="startPos"></param>
        /// <param name="moveHandDist">指定起始距离</param>
        /// <returns></returns>
        Vector3[] OffsetPengChiGangPaiStartPosByMoveDist(int seatIdx, Vector3[] startPos, float moveHandDist = 0)
        {
            HandDirection dir = pengPaiHandDirSeat[seatIdx];
            Vector3 centerPos = startPos[1];
            float sign = 1;
            Vector3 pevPos = startPos[0];
            float moveDist = 0;

            switch (seatIdx)
            {
                case 0:
                    if (dir == HandDirection.LeftHand) { sign = -1; }
                    else { sign = 1; }
                    break;

                case 1:
                    if (dir == HandDirection.LeftHand) { sign = 1; }
                    else { sign = -1; }
                    break;

                case 2:
                    if (dir == HandDirection.LeftHand) { sign = 1; }
                    else { sign = -1; }
                    break;

                case 3:
                    if (dir == HandDirection.LeftHand) { sign = -1; }
                    else { sign = 1; }
                    break;
            }

            switch (seatIdx)
            {
                case 0:
                case 2:
                    moveDist = sign * moveHandDist;
                    pevPos.x += moveDist;
                    centerPos.x += moveDist;
                    break;

                case 1:
                case 3:
                    moveDist = sign * moveHandDist;
                    pevPos.z += moveDist;
                    centerPos.z += moveDist;
                    break;
            }

            return new Vector3[] { pevPos, centerPos };
        }

        /// <summary>
        /// 根据给定碰吃杠牌布局位置列表(pcgPaiPos)，生成碰吃杠牌实体列表
        /// </summary>
        /// <param name="pcgPaiPos">给定碰吃杠牌布局位置列表</param>
        /// <param name="mjFaceValues"></param>
        /// <param name="paiType"></param>
        /// <returns></returns>
        GameObject[] CreatePengChiGangPaiList(
            PengChiGangPaiPos[] pcgPaiPos, MahjongFaceValue[] mjFaceValues,
            PengChiGangPaiType paiType)
        {
            GameObject mj;
            List<GameObject> mjList = new List<GameObject>();
            bool isBackSide = false;

            if (paiType == PengChiGangPaiType.AN_GANG)
                isBackSide = true;

            int randIdx = Random.Range(0, 9);
            MahjongFaceValue mjFaceValue;

            for (int i = 0; i < pcgPaiPos.Length; i++)
            {
                if (isBackSide)
                {
                    mjFaceValue = MahjongFaceValue.MJ_ZFB_FACAI;
                }
                else
                {
                    mjFaceValue = mjFaceValues[i];
                }

                mj = mjAssetsMgr.PopMjFromDeskOrSelfHandMjPool(mjFaceValue);
                mj.layer = defaultLayer;

                if (paiType == PengChiGangPaiType.GANG)
                {
                    if (i == randIdx || randIdx >= 4)
                        isBackSide = false;
                    else
                        isBackSide = true;
                }

                FitSeatDeskPengChiGangMj(pcgPaiPos[i].layouyDirSeat, mj, true, 0.91f, isBackSide);
                mj.transform.position = pcgPaiPos[i].pos;
                mjList.Add(mj);
            }

            return mjList.ToArray();
        }


        bool IsAtBackPos(int seatIdx, Vector3 pos, Vector3 orgPos)
        {
            HandDirection dir = pengPaiHandDirSeat[seatIdx];

            switch (seatIdx)
            {
                case 0:
                    if (dir == HandDirection.LeftHand)
                    {
                        if (pos.x < orgPos.x)
                            return true;
                    }
                    else
                    {
                        if (pos.x > orgPos.x)
                            return true;
                    }
                    break;

                case 1:
                    if (dir == HandDirection.LeftHand)
                    {
                        if (pos.z > orgPos.z)
                            return true;
                    }

                    else
                    {
                        if (pos.z < orgPos.z)
                            return true;
                    }
                    break;

                case 2:
                    if (dir == HandDirection.LeftHand)
                    {
                        if (pos.x > orgPos.x)
                            return true;
                    }

                    else
                    {
                        if (pos.x < orgPos.x)
                            return true;
                    }
                    break;

                case 3:
                    if (dir == HandDirection.LeftHand)
                    {
                        if (pos.z < orgPos.z)
                            return true;
                    }

                    else
                    {
                        if (pos.z > orgPos.z)
                            return true;
                    }

                    break;
            }

            return false;
        }


        /// <summary>
        /// 增加桌面牌到碰吃杠牌列表
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="pcgType"></param>
        /// <param name="paiList">桌面牌列表</param>
        void AddDeskPaiToPengChiGangPaiList(int seatIdx, PengChiGangPaiType pcgType, GameObject[] paiList, Vector3[] paiPosInfo)
        {
            switch (pcgType)
            {
                case PengChiGangPaiType.PENG:
                    deskPengPaiMjList[seatIdx].Add(paiList);
                    deskPengPaiMjPosInfoList[seatIdx].Add(paiPosInfo);
                    break;

                case PengChiGangPaiType.CHI:
                    deskChiPaiMjList[seatIdx].Add(paiList);
                    break;

                case PengChiGangPaiType.GANG:
                    deskGangPaiMjList[seatIdx].Add(paiList);
                    break;
            }
        }

        #endregion

        #region 生成桌面能摆放的打出牌的所有麻将位置
        /// <summary>
        /// 生成桌面能摆放的打出牌的所有麻将位置
        /// </summary>
        void CreateMjDeskDaPaiPosForSeats()
        {
            float spacing = 0.0005f;


            int maxMjCount = 0;
            for (int i = 0; i < deskRowMjLayoutCounts.Length; i++)
            {
                if (deskRowMjLayoutCounts[i] > maxMjCount)
                    maxMjCount = deskRowMjLayoutCounts[i];
            }

            handActionPoints = new float[deskRowMjLayoutCounts.Length, maxMjCount, (int)ActionCombineNum.End];
            pointActionCombineNumLists = new List<ActionCombineNum>[deskRowMjLayoutCounts.Length, maxMjCount];


            CreateMjDeskPaiPosSeat0(deskRowMjLayoutCounts, 0.09f, spacing, spacing);
            CreateMjDeskPaiPosSeat1(deskRowMjLayoutCounts, 0.11f, spacing, spacing);
            CreateMjDeskPaiPosSeat2(deskRowMjLayoutCounts, 0.09f, spacing, spacing);
            CreateMjDeskPaiPosSeat3(deskRowMjLayoutCounts, 0.11f, spacing, spacing);
        }

        /// <summary>
        /// 生成麻将在桌面的位置
        /// </summary>
        /// <param name="rowMjLayoutCounts">桌面麻将数量布局</param>
        /// <param name="posOffset">离桌面中心位置的距离</param>
        /// <param name="mjHorSpacing">麻将间的水平间隔</param>
        /// <param name="mjVerSpacing">麻将间的垂直间隔</param>
        void CreateMjDeskPaiPosSeat0(int[] rowMjLayoutCounts, float posOffset, float mjHorSpacing, float mjVerSpacing)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[0];
            float mjWidth = GetDeskMjSizeByAxis(Axis.X);
            float mjHeight = GetDeskMjSizeByAxis(Axis.Y);

            float rowMjTotalWidth;
            float rowMjPosZ, rowMjPosX, rowMjPosY;
            List<Vector3> rowMjList;

            rowMjPosY = deskFacePosY + GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.0002f;

            for (int i = 0; i < rowMjLayoutCounts.Length; i++)
            {
                rowMjTotalWidth = rowMjLayoutCounts[i] * mjWidth + (rowMjLayoutCounts[i] - 1) * mjHorSpacing;
                rowMjPosX = rowMjTotalWidth / 2 - mjWidth / 2;
                rowMjPosZ = posOffset + i * (mjHeight + mjVerSpacing) + mjHeight / 2;

                rowMjList = new List<Vector3>();

                for (int j = 0; j < rowMjLayoutCounts[i]; j++)
                {
                    rowMjList.Add(new Vector3(rowMjPosX, rowMjPosY, rowMjPosZ));
                    rowMjPosX -= mjWidth + mjHorSpacing;
                }

                mjSeatDeskPaiPosList.Add(rowMjList);
            }
        }

        /// <summary>
        /// 生成麻将在桌面的位置
        /// </summary>
        /// <param name="rowMjLayoutCounts">桌面麻将数量布局</param>
        /// <param name="posOffset">离桌面中心位置的距离</param>
        /// <param name="mjHorSpacing">麻将间的水平间隔</param>
        /// <param name="mjVerSpacing">麻将间的垂直间隔</param>
        void CreateMjDeskPaiPosSeat1(int[] rowMjLayoutCounts, float posOffset, float mjHorSpacing, float mjVerSpacing)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[1];

            float mjWidth = GetDeskMjSizeByAxis(Axis.X);
            float mjHeight = GetDeskMjSizeByAxis(Axis.Y);

            float rowMjTotalWidth;
            float rowMjPosZ, rowMjPosX, rowMjPosY;
            List<Vector3> rowMjList;

            rowMjPosY = deskFacePosY + GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.0002f;

            for (int i = 0; i < rowMjLayoutCounts.Length; i++)
            {
                rowMjTotalWidth = rowMjLayoutCounts[i] * mjWidth + (rowMjLayoutCounts[i] - 1) * mjHorSpacing;
                rowMjPosZ = -rowMjTotalWidth / 2 + mjWidth / 2;
                rowMjPosX = posOffset + i * (mjHeight + mjVerSpacing) + mjHeight / 2;

                rowMjList = new List<Vector3>();

                for (int j = 0; j < rowMjLayoutCounts[i]; j++)
                {
                    rowMjList.Add(new Vector3(rowMjPosX, rowMjPosY, rowMjPosZ));
                    rowMjPosZ += mjWidth + mjHorSpacing;
                }

                mjSeatDeskPaiPosList.Add(rowMjList);
            }
        }

        /// <summary>
        /// 生成麻将在桌面的位置
        /// </summary>
        /// <param name="rowMjLayoutCounts">桌面麻将数量布局</param>
        /// <param name="posOffset">离桌面中心位置的距离</param>
        /// <param name="mjHorSpacing">麻将间的水平间隔</param>
        /// <param name="mjVerSpacing">麻将间的垂直间隔</param>
        void CreateMjDeskPaiPosSeat2(int[] rowMjLayoutCounts, float posOffset, float mjHorSpacing, float mjVerSpacing)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[2];

            float mjWidth = GetDeskMjSizeByAxis(Axis.X);
            float mjHeight = GetDeskMjSizeByAxis(Axis.Y);

            float rowMjTotalWidth;
            float rowMjPosZ, rowMjPosX, rowMjPosY;
            List<Vector3> rowMjList;

            rowMjPosY = deskFacePosY + GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.0002f;

            for (int i = 0; i < rowMjLayoutCounts.Length; i++)
            {
                rowMjTotalWidth = rowMjLayoutCounts[i] * mjWidth + (rowMjLayoutCounts[i] - 1) * mjHorSpacing;
                rowMjPosX = -rowMjTotalWidth / 2 + mjWidth / 2;
                rowMjPosZ = -posOffset - i * (mjHeight + mjVerSpacing) - mjHeight / 2;

                rowMjList = new List<Vector3>();

                for (int j = 0; j < rowMjLayoutCounts[i]; j++)
                {
                    rowMjList.Add(new Vector3(rowMjPosX, rowMjPosY, rowMjPosZ));
                    rowMjPosX += mjWidth + mjHorSpacing;
                }

                mjSeatDeskPaiPosList.Add(rowMjList);
            }
        }

        /// <summary>
        /// 生成麻将在桌面的位置
        /// </summary>
        /// <param name="rowMjLayoutCounts">桌面麻将数量布局</param>
        /// <param name="posOffset">离桌面中心位置的距离</param>
        /// <param name="mjHorSpacing">麻将间的水平间隔</param>
        /// <param name="mjVerSpacing">麻将间的垂直间隔</param>
        void CreateMjDeskPaiPosSeat3(int[] rowMjLayoutCounts, float posOffset, float mjHorSpacing, float mjVerSpacing)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[3];

            float mjWidth = GetDeskMjSizeByAxis(Axis.X);
            float mjHeight = GetDeskMjSizeByAxis(Axis.Y);

            float rowMjTotalWidth;
            float rowMjPosZ, rowMjPosX, rowMjPosY;
            List<Vector3> rowMjList;

            rowMjPosY = deskFacePosY + GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.0002f;

            for (int i = 0; i < rowMjLayoutCounts.Length; i++)
            {
                rowMjTotalWidth = rowMjLayoutCounts[i] * mjWidth + (rowMjLayoutCounts[i] - 1) * mjHorSpacing;
                rowMjPosZ = rowMjTotalWidth / 2 - mjWidth / 2;
                rowMjPosX = -posOffset - i * (mjHeight + mjVerSpacing) - mjHeight / 2;

                rowMjList = new List<Vector3>();

                for (int j = 0; j < rowMjLayoutCounts[i]; j++)
                {
                    rowMjList.Add(new Vector3(rowMjPosX, rowMjPosY, rowMjPosZ));
                    rowMjPosZ -= mjWidth + mjHorSpacing;
                }

                mjSeatDeskPaiPosList.Add(rowMjList);
            }
        }

        Vector3 GetMjDeskPaiPos(int seat, int row, int col)
        {
            return mjSeatDeskPaiPosLists[seat][row][col];
        }

        /// <summary>
        /// 指向下一个桌面打牌麻将位置
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        Vector3 NextDeskMjPos(int seatIdx)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[seatIdx];
            curtDaPaiMjSeatDeskPosIdx[seatIdx] = GetNextDeskMjPosIdx(seatIdx);

            float mjHeight = GetDeskMjSizeByAxis(Axis.Z);
            Vector3 mjPos = mjSeatDeskPaiPosList[curtDaPaiMjSeatDeskPosIdx[seatIdx].x][curtDaPaiMjSeatDeskPosIdx[seatIdx].y];
            mjPos.y += mjHeight * curtDaPaiMjSeatDeskPosIdx[seatIdx].z;
            return mjPos;
        }


        /// <summary>
        /// 指向上一个桌面打牌麻将位置
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        Vector3 PrevDeskMjPos(int seatIdx)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[seatIdx];
            curtDaPaiMjSeatDeskPosIdx[seatIdx] = GetPrevDeskMjPosIdx(seatIdx);

            if (curtDaPaiMjSeatDeskPosIdx[seatIdx].x < 0 || curtDaPaiMjSeatDeskPosIdx[seatIdx].y < 0)
                return Vector3.zero;

            float mjHeight = GetDeskMjSizeByAxis(Axis.Z);
            Vector3 mjPos = mjSeatDeskPaiPosList[curtDaPaiMjSeatDeskPosIdx[seatIdx].x][curtDaPaiMjSeatDeskPosIdx[seatIdx].y];
            mjPos.y += mjHeight * curtDaPaiMjSeatDeskPosIdx[seatIdx].z;

            return mjPos;
        }


        /// <summary>
        /// 获取当前座位打牌麻将的最后桌面位置，以行列数据形式返回
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        Vector3Int GetCurtDeskMjPosIdx(int seatIdx)
        {
            return curtDaPaiMjSeatDeskPosIdx[seatIdx];
        }

        /// <summary>
        /// 获取当前座位打牌麻将最后桌面位置,以行列层数据形式返回
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        Vector3Int GetNextDeskMjPosIdx(int seatIdx)
        {
            Vector3Int posIdx = curtDaPaiMjSeatDeskPosIdx[seatIdx];
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[seatIdx];

            int colIdx = posIdx.y;
            int colCount = mjSeatDeskPaiPosList[posIdx.x].Count;

            if (colIdx < colCount - 1)
            {
                colIdx++;
                return new Vector3Int(posIdx.x, colIdx, posIdx.z);
            }
            else if (posIdx.x < mjSeatDeskPaiPosList.Count - 1)
            {
                return new Vector3Int(posIdx.x + 1, 0, posIdx.z);
            }

            return new Vector3Int(0, 0, posIdx.z + 1);
        }


        /// <summary>
        /// 获取当前座位打牌麻将最后桌面位置的前一个位置,以行列数据形式返回
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        Vector3Int GetPrevDeskMjPosIdx(int seatIdx)
        {
            Vector3Int posIdx = curtDaPaiMjSeatDeskPosIdx[seatIdx];
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[seatIdx];

            int colIdx = posIdx.y;
            int colCount = mjSeatDeskPaiPosList[posIdx.x].Count;

            if (colIdx >= 0)
            {
                colIdx--;
                return new Vector3Int(posIdx.x, colIdx, posIdx.z);
            }
            else if (posIdx.x > 0)
            {
                colCount = mjSeatDeskPaiPosList[posIdx.x - 1].Count;
                return new Vector3Int(posIdx.x - 1, colCount - 1, posIdx.z);
            }
            else if (posIdx.z > 0)
            {
                colCount = mjSeatDeskPaiPosList[mjSeatDeskPaiPosList.Count - 1].Count;
                return new Vector3Int(mjSeatDeskPaiPosList.Count - 1, colCount - 1, posIdx.z - 1);
            }

            return new Vector3Int(0, -1, 0);
        }

        #endregion

        #region 生成桌面能摆放的胡牌麻将位置
        /// <summary>
        /// 生成桌面能摆放的胡牌麻将位置
        /// </summary>
        void CreateMjDeskHuPaiPosForSeats()
        {
            mjSeatDeskHuPaiPosList = new Vector3[4, huPaiDeskPosMjLayoutRowCount, huPaiDeskPosMjLayoutColCount];
            float mjSpacing = 0.001f;

            CreateMjDeskHuPaiPosSeat0(huPaiDeskPosMjLayoutRowCount, huPaiDeskPosMjLayoutColCount, new Vector3(-0.27f, 0, 0.299f), mjSpacing);
            CreateMjDeskHuPaiPosSeat1(huPaiDeskPosMjLayoutRowCount, huPaiDeskPosMjLayoutColCount, new Vector3(0.2952f, 0, 0.27f), mjSpacing);
            CreateMjDeskHuPaiPosSeat2(huPaiDeskPosMjLayoutRowCount, huPaiDeskPosMjLayoutColCount, new Vector3(0.27f, 0, -0.2867f), mjSpacing);
            CreateMjDeskHuPaiPosSeat3(huPaiDeskPosMjLayoutRowCount, huPaiDeskPosMjLayoutColCount, new Vector3(-0.2952f, 0, -0.27f), mjSpacing);
        }

        void CreateMjDeskHuPaiPosSeat0(int rowCount, int colCount, Vector3 startPos, float mjSpacing)
        {
            int seat = 0;
            float mjSizeX = GetDeskMjSizeByAxis(Axis.X);
            float mjSizeY = GetDeskMjSizeByAxis(Axis.Y);
            float x = startPos.x - mjSizeX / 2;
            float z = startPos.z;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    mjSeatDeskHuPaiPosList[seat, i, j] = new Vector3(x, deskFacePosY, z);
                    x -= mjSizeX + mjSpacing;
                }

                x = startPos.x - mjSizeX / 2;
                z += mjSizeY;
            }
        }

        void CreateMjDeskHuPaiPosSeat1(int rowCount, int colCount, Vector3 startPos, float mjSpacing)
        {
            int seat = 1;
            float mjSizeX = GetDeskMjSizeByAxis(Axis.X);
            float mjSizeY = GetDeskMjSizeByAxis(Axis.Y);
            float z = startPos.z + mjSizeX / 2;
            float x = startPos.x;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    mjSeatDeskHuPaiPosList[seat, i, j] = new Vector3(x, deskFacePosY, z);
                    z += mjSizeX + mjSpacing;
                }

                z = startPos.z + mjSizeX / 2;
                x += mjSizeY;
            }
        }

        void CreateMjDeskHuPaiPosSeat2(int rowCount, int colCount, Vector3 startPos, float mjSpacing)
        {
            int seat = 2;
            float mjSizeX = GetDeskMjSizeByAxis(Axis.X);
            float mjSizeY = GetDeskMjSizeByAxis(Axis.Y);
            float x = startPos.x + mjSizeX / 2;
            float z = startPos.z;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    mjSeatDeskHuPaiPosList[seat, i, j] = new Vector3(x, deskFacePosY, z);
                    x += mjSizeX + mjSpacing;
                }

                x = startPos.x + mjSizeX / 2;
                z -= mjSizeY;
            }
        }

        void CreateMjDeskHuPaiPosSeat3(int rowCount, int colCount, Vector3 startPos, float mjSpacing)
        {
            int seat = 3;
            float mjSizeX = GetDeskMjSizeByAxis(Axis.X);
            float mjSizeY = GetDeskMjSizeByAxis(Axis.Y);
            float z = startPos.z - mjSizeX / 2;
            float x = startPos.x;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    mjSeatDeskHuPaiPosList[seat, i, j] = new Vector3(x, deskFacePosY, z);
                    z -= mjSizeX + mjSpacing;
                }

                z = startPos.z + mjSizeX / 2;
                x -= mjSizeY;
            }

        }

        Vector3 NextDeskHuPaiMjPos(int seatIdx)
        {
            curtHuPaiMjSeatDeskPosIdx[seatIdx] += 1;
            int idx = curtHuPaiMjSeatDeskPosIdx[seatIdx];
            Vector3 pos = GetDeskHuPaiMjPos(seatIdx, idx);
            return pos;
        }

        int GetNextDeskHuPaiMjPosIdx(int seatIdx)
        {
            int idx = curtHuPaiMjSeatDeskPosIdx[seatIdx] + 1;
            return idx;
        }

        int GetCurtDeskHuPaiMjPosIdx(int seatIdx)
        {
            return curtHuPaiMjSeatDeskPosIdx[seatIdx];
        }
        Vector3 GetDeskHuPaiMjPos(int seatIdx, int idx)
        {
            float mjSizeZ = GetDeskMjSizeByAxis(Axis.Z);

            int rowCount = mjSeatDeskHuPaiPosList.GetLength(1);
            int colCount = mjSeatDeskHuPaiPosList.GetLength(2);

            int realIdxZ = idx / (rowCount * colCount);
            int realIdx = idx % (rowCount * colCount);

            int row = realIdx / colCount;
            int col = realIdx % colCount;

            Vector3 pos = mjSeatDeskHuPaiPosList[seatIdx, row, col];
            pos.y += realIdxZ * mjSizeZ + mjSizeZ / 2 + 0.0002f;

            return pos;
        }

        #endregion

        #region 生成手牌的所有麻将初始位置
        /// <summary>
        /// 生成碰，吃，杠牌的起始位置
        /// </summary>
        /// <param name="mjHandTotalCount"></param>
        void CreateMjHandPaiPosList(int mjHandTotalCount)
        {


            float mjOffsetLR = 0;
            float mjSpacing = 0.0001f;
            float mjOffsetFB = 0.02f;

            CreateMjHandPaiCanvasPosListSeat0(mjHandTotalCount, 0, -50, 5);
            CreateMjHandPaiPosListSeat0(mjHandTotalCount, mjSpacing, mjOffsetLR, 0.313f, mjOffsetFB);
            CreateMjHandPaiPosListSeat1(mjHandTotalCount, mjSpacing, mjOffsetLR, 0.356f, mjOffsetFB);
            CreateMjHandPaiPosListSeat2(mjHandTotalCount, mjSpacing, mjOffsetLR, -0.313f, -mjOffsetFB);
            CreateMjHandPaiPosListSeat3(mjHandTotalCount, mjSpacing, mjOffsetLR, -0.343f, -mjOffsetFB);
        }

        void CreateMjHandPaiCanvasPosListSeat0(int mjCount, float mjSpacing, float mjOffsetLR, float mjOffsetUD)
        {
            float mjsize = GetCanvasHandMjSizeByAxis(Axis.X);
            float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
            float mjStartPos = -mjWallLen / 2 + mjsize / 2 + mjOffsetLR;
            float mjAxisSpacing = mjsize + mjSpacing;

            float mjsizeZ = GetCanvasHandMjSizeByAxis(Axis.Z);
            float mjsizeY = GetCanvasHandMjSizeByAxis(Axis.Y);
            float height = canvasHandPaiRectTransform.sizeDelta.y;

            for (int i = 0; i < mjCount; i++)
            {
                mjSeatHandPaiPosLists[0].Add(new Vector3(mjStartPos + i * mjAxisSpacing, -height / 2 + mjsizeY / 2 + mjOffsetUD, mjsizeZ / 2));
            }
        }

        void CreateMjHandPaiPosListSeat0(int mjCount, float mjSpacing, float mjOffsetLR, float posFB, float mjOffsetFB)
        {
            Transform mjtuo_self_tf = mjtableTransform.Find("desk_mjtuo_self");
            float mjsize = GetHandMjSizeByAxis(Axis.X);
            float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
            float mjStartPos = mjWallLen / 2 - mjsize / 2 + mjtuo_self_tf.transform.position.x + mjOffsetLR;
            float mjAxisSpacing = mjsize + mjSpacing;

            float mjsizeY = GetHandMjSizeByAxis(Axis.Y);

            for (int i = 0; i < mjCount; i++)
            {
                mjSeatHandPaiPosLists[4].Add(new Vector3(mjStartPos - i * mjAxisSpacing, deskFacePosY + 0.0002f + mjsizeY / 2, posFB + mjOffsetFB));
            }
        }

        void CreateMjHandPaiPosListSeat1(int mjCount, float mjSpacing, float mjOffsetLR, float posFB, float mjOffsetFB)
        {
            Transform mjtuo_west_tf = mjtableTransform.Find("desk_mjtuo_west");
            float mjsize = GetHandMjSizeByAxis(Axis.X);
            float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
            float mjStartPos = -mjWallLen / 2 + mjsize / 2 + mjtuo_west_tf.transform.position.z + mjOffsetLR;
            float mjAxisSpacing = mjsize + mjSpacing;
            float mjsizeY = GetHandMjSizeByAxis(Axis.Y);

            for (int i = 0; i < mjCount; i++)
            {
                mjSeatHandPaiPosLists[1].Add(new Vector3(posFB + mjOffsetFB, deskFacePosY + 0.0002f + mjsizeY / 2, mjStartPos + i * mjAxisSpacing));
            }
        }

        void CreateMjHandPaiPosListSeat2(int mjCount, float mjSpacing, float mjOffsetLR, float posFB, float mjOffsetFB)
        {
            Transform mjtuo_north_tf = mjtableTransform.Find("desk_mjtuo_north");
            float mjsize = GetHandMjSizeByAxis(Axis.X);
            float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
            float mjStartPos = -mjWallLen / 2 + mjsize / 2 + mjtuo_north_tf.transform.position.x + mjOffsetLR;
            float mjAxisSpacing = mjsize + mjSpacing;
            float mjsizeY = GetHandMjSizeByAxis(Axis.Y);

            for (int i = 0; i < mjCount; i++)
            {
                mjSeatHandPaiPosLists[2].Add(new Vector3(mjStartPos + i * mjAxisSpacing, deskFacePosY + 0.0002f + mjsizeY / 2, posFB + mjOffsetFB));
            }
        }

        void CreateMjHandPaiPosListSeat3(int mjCount, float mjSpacing, float mjOffsetLR, float posFB, float mjOffsetFB)
        {
            Transform mjtuo_east_tf = mjtableTransform.Find("desk_mjtuo_east");
            float mjsize = GetHandMjSizeByAxis(Axis.X);
            float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
            float mjStartPos = mjWallLen / 2 - mjsize / 2 + mjtuo_east_tf.transform.position.z + mjOffsetLR;
            float mjAxisSpacing = mjsize + mjSpacing;
            float mjsizeY = GetHandMjSizeByAxis(Axis.Y);

            for (int i = 0; i < mjCount; i++)
            {
                mjSeatHandPaiPosLists[3].Add(new Vector3(posFB + mjOffsetFB, deskFacePosY + 0.0002f + mjsizeY / 2, mjStartPos - i * mjAxisSpacing));
            }
        }

        #endregion

        #endregion

        #region 生成桌面麻将位置显示手部打牌动作概率点数
        void CreateDeskMjPosViewHandDaPaiActionPoints()
        {
            SetAllHandActionViewPoint(1f);

            SetHandActionViewPoint(ActionCombineNum.ChaPai, 0);
            SetHandActionViewPoint(ActionCombineNum.PengPai, 0);
            SetHandActionViewPoint(ActionCombineNum.ChiPai, 0);
            SetHandActionViewPoint(ActionCombineNum.GangPai, 0);
            SetHandActionViewPoint(ActionCombineNum.HuPai, 0);
            SetHandActionViewPoint(ActionCombineNum.TuiDaoPai, 0);
            SetHandActionViewPoint(ActionCombineNum.QiDongDiceMachine, 0);

            SetHandActionViewPoint(ActionCombineNum.DaPai5, 15f);
            SetHandActionViewPoint(ActionCombineNum.DaPai5, 0, 0);

            SetHandActionViewPoint(ActionCombineNum.DaPai1_MovPai1_TaiHand1, 10f);
            SetHandActionViewPoint(ActionCombineNum.DaPai1_MovPai1_TaiHand1, 3, 0);

            SetHandActionViewPoint(ActionCombineNum.DaPai1_MovPai1_TaiHand2, 10f);
            SetHandActionViewPoint(ActionCombineNum.DaPai1_MovPai1_TaiHand2, 3, 0);

            SetHandActionViewPoint(ActionCombineNum.DaPai1_TaiHand2, 5f);
            SetHandActionViewPoint(ActionCombineNum.DaPai3_TaiHand, 3f);

            SetHandActionViewPoint(ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand, 10f);

            SetHandActionViewPoint(ActionCombineNum.DaPai1_ZhengPai_TaiHand, 0, 0);


            //ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand
            SetHandActionViewPoint(ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand, 0, 0, 0);
            SetHandActionViewPoint(ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand, 1, 0, 0);
            SetHandActionViewPoint(ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand, 2, 0f);

            //ActionCombineNum.DaPai2_MovPai_TaiHand1
            SetHandActionViewPoint(ActionCombineNum.DaPai2_MovPai_TaiHand1, 2, 0f);
            SetHandActionViewPoint(ActionCombineNum.DaPai2_MovPai_TaiHand1, 0, 1.5f);
            SetHandActionViewPoint(ActionCombineNum.DaPai2_MovPai_TaiHand1, 1, 1.5f);

            //ActionCombineNum.DaPai2_MovPai_TaiHand2
            SetHandActionViewPoint(ActionCombineNum.DaPai2_MovPai_TaiHand2, 2, 0f);
            SetHandActionViewPoint(ActionCombineNum.DaPai2_MovPai_TaiHand2, 0, 1.5f);
            SetHandActionViewPoint(ActionCombineNum.DaPai2_MovPai_TaiHand2, 1, 1.5f);

            float n;
            int pt;
            List<ActionCombineNum> actionCombinNumList;

            //归一化几率点数
            for (int i = 0; i < handActionPoints.GetLength(0); i++)
            {
                for (int j = 0; j < handActionPoints.GetLength(1); j++)
                {
                    float totalValue = 0;
                    for (int k = 0; k < handActionPoints.GetLength(2); k++)
                    {
                        totalValue += handActionPoints[i, j, k];
                    }

                    for (int k = 0; k < handActionPoints.GetLength(2); k++)
                    {
                        n = handActionPoints[i, j, k];
                        n /= totalValue;
                        n *= 100.0f;
                        pt = (int)Mathf.Ceil(n);
                        handActionPoints[i, j, k] = pt;


                        if (pt != 0)
                        {
                            actionCombinNumList = pointActionCombineNumLists[i, j];

                            if (actionCombinNumList == null)
                            {
                                actionCombinNumList = new List<ActionCombineNum>();
                                pointActionCombineNumLists[i, j] = actionCombinNumList;
                            }

                            for (int m = 0; m < pt; m++)
                            {
                                actionCombinNumList.Add((ActionCombineNum)k);
                            }
                        }
                    }

                    actionCombinNumList = pointActionCombineNumLists[i, j];

                    if (actionCombinNumList != null)
                    {
                        int randIdx;
                        int count = actionCombinNumList.Count;
                        ActionCombineNum tmp;

                        for (int x = 0; x < count; x++)
                        {
                            randIdx = Random.Range(0, count);
                            tmp = actionCombinNumList[randIdx];
                            actionCombinNumList[randIdx] = actionCombinNumList[x];
                            actionCombinNumList[x] = tmp;
                        }
                    }
                }
            }
        }

        void SetHandActionViewPoint(ActionCombineNum actionCombineNum, int rowIdx, int colIdx, float point)
        {
            if (rowIdx >= handActionPoints.GetLength(0) || rowIdx < 0 ||
                colIdx >= handActionPoints.GetLength(1) || colIdx < 0)
                return;

            handActionPoints[rowIdx, colIdx, (int)actionCombineNum] = point;
        }

        void SetHandActionViewPoint(ActionCombineNum actionCombineNum, int rowIdx, float point)
        {
            if (rowIdx >= handActionPoints.GetLength(0) || rowIdx < 0)
                return;

            for (int i = 0; i < handActionPoints.GetLength(1); i++)
            {
                handActionPoints[rowIdx, i, (int)actionCombineNum] = point;
            }
        }

        void SetHandActionViewPoint(ActionCombineNum actionCombineNum, float point)
        {
            for (int row = 0; row < handActionPoints.GetLength(0); row++)
            {
                SetHandActionViewPoint(actionCombineNum, row, point);
            }
        }

        void SetAllHandActionViewPoint(int rowIdx, float point)
        {
            for (int i = 0; i < (int)ActionCombineNum.End; i++)
            {
                SetHandActionViewPoint((ActionCombineNum)i, rowIdx, point);
            }
        }

        void SetAllHandActionViewPoint(float point)
        {
            for (int row = 0; row < handActionPoints.GetLength(0); row++)
            {
                SetAllHandActionViewPoint(row, point);
            }
        }

        /// <summary>
        /// 获取当前麻将桌打牌位置的随机手部动作编号
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public ActionCombineNum GetRandomHandDaPaiActionNumForNextDeskMjPos(int seatIdx)
        {
            int randIdx = Random.Range(0, 100);
            Vector3Int posIdx = GetNextDeskMjPosIdx(seatIdx);
            return pointActionCombineNumLists[posIdx.x, posIdx.y][randIdx];
        }

        #endregion

        #region 屏幕适配
        /// <summary>
        /// 屏幕适配
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        void FitScreen(float screenWidth, float screenHeight)
        {
            float aspect = screenWidth / screenHeight;
            List<ScreenFitInfo> fitInfo = mjAssets.screenFitInfoList;

            for (int i = 0; i < fitInfo.Count; i++)
            {
                if (aspect > fitInfo[i].screenAspect - 0.002f && aspect < fitInfo[i].screenAspect + 0.002f)
                {
                    cameraTransform.localPosition = fitInfo[i].camPosition;
                    cameraTransform.localEulerAngles = fitInfo[i].camEluerAngle;
                    cameraTransform.GetComponent<Camera>().fieldOfView = fitInfo[i].camFieldOfView;
                    SetCanvasHandMjScale(fitInfo[i].mjScale);
                    return;
                }
            }

            if (aspect > 1.333f - 0.0002f && aspect < 1.333333f + 0.0002f)  // 4:3
            {
                cameraTransform.localPosition = new Vector3(-0.005f, 0.79f, 0.51f);
                cameraTransform.localEulerAngles = new Vector3(59.6f, -180f, 0);
                cameraTransform.GetComponent<Camera>().fieldOfView = 47.6f;
                SetCanvasHandMjScale(1.4f);
            }
            else if (aspect > 1.599f - 0.0002f && aspect < 1.61f + 0.0002f)   // 16:10
            {
                cameraTransform.localPosition = new Vector3(-0.005f, 0.899f, 0.726f);
                cameraTransform.localEulerAngles = new Vector3(51.857f, -180f, 0);
                cameraTransform.GetComponent<Camera>().fieldOfView = 33.8f;
                SetCanvasHandMjScale(1.16f);
            }
            else  // 16:9 (1920 :1080)
            {
                cameraTransform.localPosition = new Vector3(-0.005f, 0.899f, 0.726f);
                cameraTransform.localEulerAngles = new Vector3(51.857f, -180f, 0);
                cameraTransform.GetComponent<Camera>().fieldOfView = 33.8f;
                SetCanvasHandMjScale(1.3f);
            }
        }
        #endregion

        public void AppendMjOpCmd(MahjongMachineCmd cmd)
        {
            mjOpCmdList.Append(cmd);
        }

        /// <summary>
        /// 获取刮风下雨等特效的组合特效中的子特效
        /// </summary>
        /// <param name="effects"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public EffectFengRainEtcType GetEffectFengRainEtcType(EffectFengRainEtcType effects, int idx)
        {
            int mask = 0x1 << idx;
            return (EffectFengRainEtcType)((int)effects & mask);
        }

        /// <summary>
        /// 设置屏幕手牌麻将的缩放尺寸
        /// </summary>
        /// <param name="scale"></param>
        void SetCanvasHandMjScale(float scale = 0)
        {
            if (scale == 0)
            {
                float s1 = 1920f / 1080f;
                float s2 = (float)Screen.width / (float)Screen.height;
                scale = (s2 / s1) * 1.3f;
            }

            canvasHandMjScale = new Vector3(scale, scale, scale);
        }

        #region DeskGlobalMjPaiSetDict操作
        void AppendMjToDeskGlobalMjPaiSetDict(MahjongFaceValue mjFaceValue, GameObject mj)
        {
            List<GameObject> mjList;

            if (deskGlobalMjPaiSetDict.ContainsKey(mjFaceValue))
            {
                mjList = deskGlobalMjPaiSetDict[mjFaceValue];
            }
            else
            {
                mjList = new List<GameObject>();
                deskGlobalMjPaiSetDict[mjFaceValue] = mjList;
            }

            if (highLightMjValue == mjFaceValue)
                highLightMjValue = MahjongFaceValue.MJ_UNKNOWN;

            mjList.Add(mj);
        }

        void RemoveMjFromDeskGlobalMjPaiSetDict(MahjongFaceValue mjFaceValue, GameObject mj)
        {
            if (!deskGlobalMjPaiSetDict.ContainsKey(mjFaceValue))
                return;

            deskGlobalMjPaiSetDict[mjFaceValue].Remove(mj);
        }

        GameObject[] GetMjGroupByDeskGlobalMjPaiSetDict(MahjongFaceValue mjFaceValue)
        {
            if (!deskGlobalMjPaiSetDict.ContainsKey(mjFaceValue))
                return null;

            List<GameObject> mjList = deskGlobalMjPaiSetDict[mjFaceValue];
            return mjList.ToArray();
        }


        void ClearDeskGlobalMjPaiSetDict()
        {
            deskGlobalMjPaiSetDict.Clear();
        }

        #endregion


        void CreateHandPaiCardList(ref byte[] cards)
        {
            if (cards == null)
                return;

            int cardIdx;
            for (int i = 0; i < mjSeatHandPaiLists[0].Count; i++)
            {
                if (mjSeatHandPaiLists[0][i] == null)
                    continue;

                cardIdx = (int)(mjSeatHandPaiLists[0][i].GetComponent<MjPaiData>().mjFaceValue);
                if (cardIdx >= 34)
                    continue;

                cards[cardIdx]++;
            }

            for (int i = 0; i < mjSeatMoPaiLists[0].Count; i++)
            {
                if (mjSeatMoPaiLists[0][i] == null)
                    continue;

                cardIdx = (int)(mjSeatMoPaiLists[0][i].GetComponent<MjPaiData>().mjFaceValue);
                if (cardIdx >= 34)
                    continue;

                cards[cardIdx]++;
            }
        }

        void CreateHuPaiInfos(List<GameObject> paiList, TingData[] tingDatas, ref int[] huPaiIdxs, ref List<HuPaiTipsInfo[]> huPaiInfos)
        {
            int[] tmpHuPaiIdxs = new int[100];
            int tingHuPaiCount = 0;

            HuPaiTipsInfo[] tmpHuPaiTipsInfos = new HuPaiTipsInfo[100];
            int huPaiTipsCount = 0;

            int richTingHuMjCount = 0;
            MahjongFaceValue tingHuFaceValue;
            int cardIdx;
            GameObject[] deskMjs;

            for (int i = 0; tingDatas[i].tingCardIdx != -1; i++)
            {
                for (int j = 0; j < paiList.Count; j++)
                {
                    cardIdx = (int)(paiList[j].GetComponent<MjPaiData>().mjFaceValue);
                    if (cardIdx != tingDatas[i].tingCardIdx)
                        continue;

                    tmpHuPaiIdxs[tingHuPaiCount++] = j;

                    for (int k = 0; k <= tingDatas[i].huCardsEndIdx; k++)
                    {
                        tingHuFaceValue = (MahjongFaceValue)(tingDatas[i].huCards[k]);

                        deskMjs = GetMjGroupByDeskGlobalMjPaiSetDict(tingHuFaceValue);

                        richTingHuMjCount = 4;
                        if (deskMjs != null)
                            richTingHuMjCount -= deskMjs.Length;
                        richTingHuMjCount -= GetHandPaiCountForMjFaceValue(tingHuFaceValue) + GetMoPaiCountForMjFaceValue(tingHuFaceValue);

                        if (richTingHuMjCount <= 0)
                            continue;

                        HuPaiTipsInfo info = new HuPaiTipsInfo();
                        info.faceValue = (MahjongFaceValue)(tingDatas[i].huCards[k]);
                        info.zhangAmount = richTingHuMjCount;

                        tmpHuPaiTipsInfos[huPaiTipsCount++] = info;
                    }

                    if (huPaiTipsCount > 0)
                    {
                        HuPaiTipsInfo[] huPaiTipsInfos = new HuPaiTipsInfo[huPaiTipsCount];
                        for (int f = 0; f < huPaiTipsCount; f++)
                            huPaiTipsInfos[f] = tmpHuPaiTipsInfos[f];

                        huPaiInfos.Add(huPaiTipsInfos);
                        huPaiTipsCount = 0;
                    }
                    else
                    {
                        tingHuPaiCount--;
                    }

                    break;
                }
            }

            if (tingHuPaiCount > 0)
            {
                huPaiIdxs = new int[tingHuPaiCount];
                for (int f = 0; f < tingHuPaiCount; f++)
                    huPaiIdxs[f] = tmpHuPaiIdxs[f];

                tingHuPaiCount = 0;
            }
        }


        #region 声音设置
        void CreateAudios()
        {
            speakTypes = mjAssets.settingDataAssetsMgr.parseTextSetting.GetStringGroup("SPEAK");

            speakAudiosDicts = new Dictionary<int, AudioClip[]>[(int)PlayerType.NONE];
            musicAudiosDict = mjAssets.settingDataAssetsMgr.GetAudiosDict("MUSIC_AUDIO");
            effectAudiosDict = mjAssets.settingDataAssetsMgr.GetAudiosDict("EFFECT_AUDIO");

            SetSpeakAudioDict(voiceType);
        }

        public void SetSpeakAudioDict(string audioType)
        {
            for (int i = 0; i < (int)PlayerType.NONE; i++)
            {
                speakAudiosDicts[i] = mjAssets.settingDataAssetsMgr.GetAudiosDict(audioType, i);
            }
        }

        public void PlaySpeakAudio(PlayerType playerType, MahjongFaceValue mjFaceValue, Vector3 playPos, int idx = 0)
        {
            AudioClip clip = GetSpeakAudio(playerType, (int)mjFaceValue, idx);
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, playPos);
        }

        public void PlaySpeakAudio(PlayerType playerType, AudioIdx audioIdx, Vector3 playPos, int idx = 0)
        {
            AudioClip clip = GetSpeakAudio(playerType, (int)audioIdx, idx);
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, playPos);
        }

        public AudioClip GetSpeakAudio(PlayerType playerType, int audioIdx, int idx = 0)
        {
            Dictionary<int, AudioClip[]> audioClipDict = speakAudiosDicts[(int)playerType];

            if (audioClipDict == null || !audioClipDict.ContainsKey(audioIdx))
                return null;

            return mjAssets.settingDataAssetsMgr.GetAudio(audioClipDict, audioIdx, idx);
        }

        public AudioClip GetMusicAudio(int audioIdx, int idx = 0)
        {
            return mjAssets.settingDataAssetsMgr.GetAudio(musicAudiosDict, audioIdx, idx);
        }

        public void PlayEffectAudio(AudioIdx audioIdx, int idx = 0)
        {
            AudioClip clip = GetEffectAudio((int)audioIdx, idx);
            AudioSource.PlayClipAtPoint(clip, cameraTransform.position);
        }

        public AudioClip GetEffectAudio(int audioIdx, int idx = 0)
        {
            return mjAssets.settingDataAssetsMgr.GetAudio(effectAudiosDict, audioIdx, idx);
        }

        #endregion

        #region 麻将尺寸位置相关功能
        /// <summary>
        /// 获取预制麻将相关轴方向的尺寸
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetPrefabMjSizeByAxis(Axis axis)
        {
            float size = 0;

            switch (axis)
            {
                case Axis.X:
                    size = premjSize.x;
                    break;

                case Axis.Y:
                    size = premjSize.y;
                    break;

                case Axis.Z:
                    size = premjSize.z;
                    break;
            }

            return size;
        }

        /// <summary>
        /// 获取预制麻将的尺寸
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPrefabMjSize()
        {
            return premjSize;
        }

        /// <summary>
        /// 获取墙堆麻将相关轴方向的尺寸
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetWallDuiMjSizeByAxis(Axis axis)
        {
            float size = 0;

            switch (axis)
            {
                case Axis.X:
                    size = premjSize.x;
                    size *= wallduiMjScale.x;
                    break;

                case Axis.Y:
                    size = premjSize.y;
                    size *= wallduiMjScale.y;
                    break;

                case Axis.Z:
                    size = premjSize.z;
                    size *= wallduiMjScale.z;
                    break;
            }

            return size;
        }

        /// <summary>
        /// 获取桌面麻将相关轴方向的尺寸
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetDeskMjSizeByAxis(Axis axis)
        {
            float size = 0;

            switch (axis)
            {
                case Axis.X: size = premjSize.x; size *= deskMjScale.x; break;
                case Axis.Y: size = premjSize.y; size *= deskMjScale.y; break;
                case Axis.Z: size = premjSize.z; size *= deskMjScale.z; break;
            }

            return size;
        }

        /// <summary>
        /// 获取桌面麻将的尺寸
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDeskMjSize()
        {
            Vector3 size = premjSize;
            size.x *= deskMjScale.x;
            size.y *= deskMjScale.y;
            size.z *= deskMjScale.z;
            return size;
        }

        /// <summary>
        /// 获取桌面碰吃杠麻将相关轴方向的尺寸
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetDeskPengChiGangMjSizeByAxis(Axis axis)
        {
            float size = 0;

            switch (axis)
            {
                case Axis.X: size = premjSize.x; size *= deskPcgMjScale.x; break;
                case Axis.Y: size = premjSize.y; size *= deskPcgMjScale.y; break;
                case Axis.Z: size = premjSize.z; size *= deskPcgMjScale.z; break;
            }

            return size;
        }

        /// <summary>
        /// 获取手牌麻将相关轴方向的尺寸
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetHandMjSizeByAxis(Axis axis)
        {
            float size = 0;

            switch (axis)
            {
                case Axis.X: size = premjSize.x; size *= deskMjScale.x; break;
                case Axis.Y: size = premjSize.y; size *= deskMjScale.y; break;
                case Axis.Z: size = premjSize.z; size *= deskMjScale.z; break;
            }

            return size;
        }

        /// <summary>
        /// 获取手牌麻将的尺寸
        /// </summary>
        /// <returns></returns>
        public Vector3 GetHandMjSize()
        {
            Vector3 size = premjSize;
            size.x *= deskMjScale.x;
            size.y *= deskMjScale.y;
            size.z *= deskMjScale.z;
            return size;
        }

        /// <summary>
        /// 获取Canvas手牌麻将相关轴方向的prefab尺寸
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetCanvasHandMjPrefabSizeByAxis(Axis axis)
        {
            float size = 0;
            float scale = 1;

            switch (axis)
            {
                case Axis.X:
                    size = premjSize.x;
                    scale = 1f / canvasHandPaiRectTransform.localScale.x;
                    size *= scale;
                    break;

                case Axis.Y:
                    size = premjSize.y;
                    scale = 1f / canvasHandPaiRectTransform.localScale.y;
                    size *= scale;
                    break;

                case Axis.Z:
                    size = premjSize.z;
                    scale = 1f / canvasHandPaiRectTransform.localScale.z;
                    size *= scale;
                    break;
            }

            return size;
        }

        /// <summary>
        /// 获取Canvas手牌麻将相关轴方向的尺寸
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetCanvasHandMjSizeByAxis(Axis axis)
        {
            float size = 0;
            float scale = 1;

            switch (axis)
            {
                case Axis.X:
                    size = premjSize.x;
                    scale = canvasHandMjScale.x / canvasHandPaiRectTransform.localScale.x;
                    size *= scale;
                    break;

                case Axis.Y:
                    size = premjSize.y;
                    scale = canvasHandMjScale.y / canvasHandPaiRectTransform.localScale.y;
                    size *= scale;
                    break;

                case Axis.Z:
                    size = premjSize.z;
                    scale = canvasHandMjScale.z / canvasHandPaiRectTransform.localScale.z;
                    size *= scale;
                    break;
            }

            return size;
        }

        /// <summary>
        /// 获取对应座位桌面麻将的适配欧拉角
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public Vector3 GetSeatDeskMjFitEulerAngles(int seatIdx, bool isBackSide = false)
        {
            if (!isBackSide)
                return seatDeskMjEulerAngles[seatIdx];

            return seatBackDeskMjEulerAngles[seatIdx];
        }

        /// <summary>
        /// 获取对应座位手牌麻将的适配欧拉角
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public Vector3 GetSeatHandMjFitEulerAngles(int seatIdx)
        {
            return seatHandMjEulerAngles[seatIdx];
        }

        /// <summary>
        /// 获取CanvasHand麻将的适配欧拉角
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public Vector3 GetCanvasHandMjFitEulerAngles()
        {
            return canvasHandMjFitEulerAngles;
        }

        /// <summary>
        /// 为对应座位适配桌面麻将尺寸，方向等
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="fitMj"></param>
        /// <param name="isCreateShadow"></param>
        /// <param name="shadowRage"></param>
        public void FitSeatDeskMj(int seatIdx, GameObject fitMj, bool isCreateShadow = true, float shadowRage = 0.91f, bool isShowBackSide = false, bool isScale = true, int shadowIdx = 0)
        {
            FitSeatDeskMj(seatIdx, fitMj, deskMjScale, isCreateShadow, shadowRage, isShowBackSide, isScale, shadowIdx);
        }

        /// <summary>
        /// 为对应座位适配桌面碰吃杠麻将尺寸，方向等
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="fitMj"></param>
        /// <param name="isCreateShadow"></param>
        /// <param name="shadowRage"></param>
        public void FitSeatDeskPengChiGangMj(int seatIdx, GameObject fitMj, bool isCreateShadow = true, float shadowRage = 0.91f, bool isShowBackSide = false, bool isScale = true)
        {
            FitSeatDeskMj(seatIdx, fitMj, deskPcgMjScale, isCreateShadow, shadowRage, isShowBackSide, isScale);
        }

        public void FitSeatDeskMj(int seatIdx, GameObject fitMj, Vector3 scaleValue, bool isCreateShadow = true, float shadowRage = 0.91f, bool isShowBackSide = false, bool isScale = true, int shadowIdx = 0)
        {
            if (fitMj == null)
                return;

            if (isScale)
                fitMj.transform.localScale = scaleValue;

            if (!isShowBackSide)
                fitMj.transform.eulerAngles = seatDeskMjEulerAngles[seatIdx];
            else
                fitMj.transform.eulerAngles = seatBackDeskMjEulerAngles[seatIdx];

            fitMj.SetActive(true);

            if (!isCreateShadow)
                return;

            Transform shadow = fitMj.transform.GetChild(shadowIdx);

            if (isShowBackSide)
            {
                shadow.localPosition = mjAssetsMgr.shadowBackSidePos;  // new Vector3(shadow.localPosition.x, shadow.localPosition.y, shadow.localPosition.z + premjSize.z);
            }

            shadow.gameObject.SetActive(true);
        }


        /// <summary>
        /// 为对应座位适配墙堆麻将尺寸，方向等
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="fitMj"></param>
        /// <param name="isCreateShadow"></param>
        /// <param name="shadowRage"></param>
        public void FitSeatWallDuiMj(int seatIdx, GameObject fitMj, bool isCreateShadow = true, float shadowRage = 1.08f)
        {
            if (fitMj == null)
                return;

            fitMj.transform.localScale = wallduiMjScale;
            fitMj.transform.eulerAngles = seatWallDuiMjEulerAngles[seatIdx];

            if (!isCreateShadow)
                return;

            GameObject mjShadow = mjAssets.effectPrefabDict[(int)PrefabIdx.MJ_SHADOW][0];
            GameObject shadow = Object.Instantiate(mjShadow);

            shadow.transform.localScale = new Vector3(
                fitMj.transform.localScale.x * mjShadow.transform.localScale.x * shadowRage,
                fitMj.transform.localScale.y * mjShadow.transform.localScale.y * shadowRage, 1);

            shadow.transform.eulerAngles = fitMj.transform.eulerAngles;

            shadow.transform.position = new Vector3(
                fitMj.transform.position.x,
                fitMj.transform.position.y - GetWallDuiMjSizeByAxis(Axis.Z) / 2 - 0.0001f,
                fitMj.transform.position.z);

            shadow.transform.SetParent(fitMj.transform, true);

        }

        /// <summary>
        /// 为对应座位适配手牌麻将尺寸，方向等
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="fitMj"></param>
        /// <param name="isCreateShadow"></param>
        /// <param name="shadowRage"></param>
        public void FitSeatHandMj(int seatIdx, GameObject fitMj, bool isCreateShadow = true, int shadowIdx = 0)
        {
            if (fitMj == null)
                return;


            fitMj.SetActive(true);
            fitMj.transform.localScale = deskMjScale;
            fitMj.transform.eulerAngles = seatHandMjEulerAngles[seatIdx];

            if (!isCreateShadow)
                return;

            fitMj.transform.GetChild(shadowIdx).gameObject.SetActive(true);
        }

        /// <summary>
        /// 为对应座位适配Canvas手牌麻将尺寸，方向等
        /// </summary>
        /// <param name="fitMj"></param>
        public void FitSeatCanvasHandMj(GameObject fitMj, bool isCreateShadow = true, float shadowRage = 1f, bool isEnableBoxCollider = true)
        {
            if (fitMj == null)
                return;

            fitMj.SetActive(true);
            fitMj.GetComponent<BoxCollider>().enabled = isEnableBoxCollider;
            fitMj.transform.localScale = canvasHandMjScale;
            fitMj.transform.eulerAngles = new Vector3(-canvasHandPaiRectTransform.eulerAngles.x, 0, 0);

            if (!isCreateShadow)
                return;

            fitMj.transform.GetChild(1).gameObject.SetActive(true);
        }

        /// <summary>
        /// 关闭麻将阴影
        /// </summary>
        /// <param name="mj"></param>
        public void OffMjShadow(GameObject mj)
        {
            if (mj == null)
                return;

            Transform tf = mj.transform;
            tf.GetChild(0).gameObject.SetActive(false);
            if (tf.childCount >= 2)
                tf.GetChild(1).gameObject.SetActive(false);
        }

        /// <summary>
        /// 打开麻将阴影
        /// </summary>
        /// <param name="mj"></param>
        public void OnMjShadow(GameObject mj, int shadowIdx = 1)
        {
            if (mj == null)
                return;

            if (shadowIdx >= mj.transform.childCount)
                return;

            mj.transform.GetChild(shadowIdx).gameObject.SetActive(true);
        }

        #endregion

        int GetDeskDaPaiMjDictKey(int row, int col, int floor)
        {
            return row * 1000 + col * 100 + floor;
        }

        public void ShowOrHideMjDuiPai(bool isShow = false)
        {
            for (int i = 0; i < mjDuiPai.Length; i++)
            {
                if (mjDuiPai[i] == null)
                    continue;

                if (mjDuiPai[i].activeInHierarchy == !isShow)
                    mjDuiPai[i].SetActive(isShow);
            }
        }

        /// <summary>
        /// 设置圈风位
        /// </summary>
        /// <param name="dealerSeatIdx"></param>
        public void SetDealer(int dealerSeatIdx, FengWei fengWei)
        {
            int fw = (int)fengWei;

            for (int i = 0; i < 4; i++)
            {
                orderSeatIdx[i] = dealerSeatIdx;

                fw %= 4;
                seatFengWei[dealerSeatIdx] = (FengWei)fw;
                fw++;

                dealerSeatIdx--;
                if (dealerSeatIdx == -1)
                    dealerSeatIdx = 3;
            }

            diceMachine.SetSeatFengWei(dealerSeatIdx, fengWei);
        }

        /// <summary>
        /// 获取对应座位的风位
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public FengWei GetSeatFengWei(int seatIdx)
        {
            return seatFengWei[seatIdx];
        }

        /// <summary>
        /// 根据玩家打牌顺序，获取下一个可以使用的座位号
        /// </summary>
        /// <param name="curtPlayerOrderIdx"></param>
        /// <returns></returns>
        int GetNextCanUseSeatIdxByOrderSeat(ref int curtPlayerOrderIdx)
        {
            int curtSeatIdx = orderSeatIdx[curtPlayerOrderIdx];

            for (int i = 0; i < 3; i++)
            {
                ++curtPlayerOrderIdx;

                if (curtPlayerOrderIdx == playerCount)
                {
                    curtPlayerOrderIdx = 0;
                }

                curtSeatIdx = orderSeatIdx[curtPlayerOrderIdx];
                if (isCanUseSeatPlayer[curtSeatIdx])
                    return curtSeatIdx;
            }

            return curtSeatIdx;
        }


        /// <summary>
        /// 获取下一个座位序号
        /// </summary>
        int GetNextSeatIdx(int curtSeatIdx, bool isClockwise = true)
        {
            if (isClockwise)
            {
                curtSeatIdx++;
                curtSeatIdx %= playerCount;
            }
            else
            {
                curtSeatIdx--;
                if (curtSeatIdx < 0)
                    curtSeatIdx = 3;
            }

            return curtSeatIdx;
        }

        MahjongFaceValue[] GetSelfHandMahjongFaceValues(int mjHandPosIdx, int mjCount)
        {
            if (playerStateData[0].handPaiValueList.Count < mjCount + mjHandPosIdx)
                return null;

            MahjongFaceValue[] faceValues = new MahjongFaceValue[mjCount];
            int n = 0;

            for (int i = mjHandPosIdx; i < mjHandPosIdx + mjCount; i++)
            {
                faceValues[n++] = playerStateData[0].handPaiValueList[i];
            }

            return faceValues;
        }

        /// <summary>
        /// 设置对应座位号是否可用
        /// </summary>
        /// <param name="canDuiPai"></param>
        void SetSeatCanUse(bool[] canUseSeat = null)
        {
            realPlayerCount = 0;

            for (int i = 0; i < playerCount; i++)
            {
                if (canUseSeat != null)
                {
                    if (canUseSeat[i])
                    {
                        isCanUseSeatPlayer[i] = true;
                        realPlayerCount++;
                    }
                    else
                    {
                        isCanUseSeatPlayer[i] = false;
                    }
                }
                else
                {
                    if (isCanUseSeatPlayer[i])
                        realPlayerCount++;
                }
            }
        }

        /// <summary>
        /// 设置对应座位号的麻将托是否可以放牌堆
        /// </summary>
        /// <param name="canDuiPai"></param>
        void SetSeatMjTuoCanDuiPai(bool[] canDuiPai = null)
        {
            canDuiPaiMjTuoCount = 0;

            for (int i = 0; i < 4; i++)
            {
                if (canDuiPai != null)
                {
                    if (canDuiPai[i])
                    {
                        isSeatMjTuoCanDuiPai[i] = true;
                        canDuiPaiMjTuoCount++;
                    }
                    else
                    {
                        isSeatMjTuoCanDuiPai[i] = false;
                    }
                }
                else
                {
                    if (isSeatMjTuoCanDuiPai[i])
                        canDuiPaiMjTuoCount++;
                }
            }
        }

        #region 手牌列表相关功能

        /// <summary>
        /// 清理麻将列表数据
        /// </summary>
        /// <param name="mjPaiList"></param>
        /// <param name="type"></param>
        void ClearMjPaiList(List<GameObject> mjPaiList, int type = 0)
        {
            for (int j = 0; j < mjPaiList.Count; j++)
            {
                if (mjPaiList[j] != null)
                {
                    if (type == 0)
                    {
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mjPaiList[j]);
                    }
                    else
                    {
                        mjAssetsMgr.PushMjToOtherHandMjPool(mjPaiList[j]);
                    }
                }
            }
        }

        void ClearMjHandPaiList()
        {
            for (int i = 0; i < mjSeatHandPaiLists.Length; i++)
            {
                ClearMjPaiList(mjSeatHandPaiLists[i], i);
                mjSeatHandPaiLists[i].Clear();
            }
        }

        void ClearMjMoPaiList()
        {
            for (int i = 0; i < mjSeatMoPaiLists.Length; i++)
            {
                ClearMjPaiList(mjSeatMoPaiLists[i], i);
                mjSeatMoPaiLists[i].Clear();
            }
        }

        int GetHandPaiCountForMjFaceValue(MahjongFaceValue mjFaceValue)
        {
            MahjongFaceValue curtFaceValue;
            int count = 0;

            for (int i = 0; i < mjSeatHandPaiLists[0].Count; i++)
            {
                curtFaceValue = mjSeatHandPaiLists[0][i].GetComponent<MjPaiData>().mjFaceValue;

                if (curtFaceValue == mjFaceValue)
                    count++;
            }

            return count;
        }

        int GetMoPaiCountForMjFaceValue(MahjongFaceValue mjFaceValue)
        {
            MahjongFaceValue curtFaceValue;
            int count = 0;

            for (int i = 0; i < mjSeatMoPaiLists[0].Count; i++)
            {
                curtFaceValue = mjSeatMoPaiLists[0][i].GetComponent<MjPaiData>().mjFaceValue;

                if (curtFaceValue == mjFaceValue)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// 获取手牌麻将列表
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public List<GameObject> GetHandPaiList(int seatIdx)
        {
            return mjSeatHandPaiLists[seatIdx];
        }

        MahjongFaceValue GetHandPaiMjFaceValue(int seatIdx, int paiIdx)
        {
            return GetHandPaiList(seatIdx)[paiIdx].GetComponent<MjPaiData>().mjFaceValue;
        }


        /// <summary>
        /// 获取摸牌麻将列表
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public List<GameObject> GetMoPaiList(int seatIdx)
        {
            return mjSeatMoPaiLists[seatIdx];
        }

        /// <summary>
        /// 获取麻将在手牌列表中的编号位置
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="mj"></param>
        /// <returns></returns>
        public int GetPaiIdxInHandPaiList(int seatIdx, GameObject mj)
        {
            return mjSeatHandPaiLists[seatIdx].IndexOf(mj);
        }


        /// <summary>
        /// 获取麻将在摸牌列表中的编号位置
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="mj"></param>
        /// <returns></returns>
        public int GetPaiIdxInMoPaiList(int seatIdx, GameObject mj)
        {
            return mjSeatMoPaiLists[seatIdx].IndexOf(mj);
        }


        /// <summary>
        /// 获取手牌列表中最后一张牌的idx
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        int GetHandPaiListLastPaiIdx(int seatIdx)
        {
            GameObject mj;
            for (int i = mjSeatHandPaiLists[seatIdx].Count - 1; i >= 0; i--)
            {
                mj = mjSeatHandPaiLists[seatIdx][i];
                if (mj != null)
                    return i;
            }

            return -1;
        }

        int GetEmptyHandPaiIdx(int seatIdx)
        {
            for (int i = 0; i < mjSeatHandPaiLists[seatIdx].Count; i++)
            {
                if (mjSeatHandPaiLists[seatIdx][i] == null)
                    return i;
            }

            return -1;
        }

        void EmptyHandPaiIdx(int seatIdx)
        {
            for (int i = 0; i < mjSeatHandPaiLists[seatIdx].Count; i++)
            {
                mjSeatHandPaiLists[seatIdx][i] = null;
            }
        }

        #endregion

        #region 高亮桌面指定的麻将
        /// <summary>
        /// 高亮桌面指定的麻将
        /// </summary>
        /// <param name="highLightDeskMjFaceValue"></param>
        void OnDeskMjHighLight(MahjongFaceValue highLightDeskMjFaceValue)
        {
            if (highLightDeskMjFaceValue == MahjongFaceValue.MJ_UNKNOWN)
                return;

            GameObject[] mjGroup = GetMjGroupByDeskGlobalMjPaiSetDict(highLightDeskMjFaceValue);
            if (mjGroup == null)
                return;

            Renderer renderer;
            for (int i = 0; i < mjGroup.Length; i++)
            {
                renderer = mjGroup[i].GetComponent<Renderer>();
                renderer.sharedMaterial = mjAssets.mjHighLightFaceMat;
            }

            highLightMjValue = highLightDeskMjFaceValue;
        }

        /// <summary>
        /// 关闭桌面指定麻将的高亮
        /// </summary>
        /// <param name="offHighLightDeskMjFaceValue"></param>
        void OffDeskMjHighLight(MahjongFaceValue offHighLightDeskMjFaceValue)
        {
            if (offHighLightDeskMjFaceValue == MahjongFaceValue.MJ_UNKNOWN)
                return;

            GameObject[] mjGroup = GetMjGroupByDeskGlobalMjPaiSetDict(offHighLightDeskMjFaceValue);
            if (mjGroup == null)
                return;

            Renderer renderer;
            for (int i = 0; i < mjGroup.Length; i++)
            {
                renderer = mjGroup[i].GetComponent<Renderer>();
                renderer.sharedMaterial = mjAssets.mjNormalFaceMat;
            }

            highLightMjValue = MahjongFaceValue.MJ_UNKNOWN;
        }

        #endregion

        #region 生成桌面麻将牌堆

        /// <summary>
        /// 生成桌面麻将牌堆
        /// </summary>
        void CreateMahjongPaiDui()
        {
            int duiCount = mjPaiTotalCount / 2;
            int rCount = mjPaiTotalCount % 2;

            int tuoMjCount = duiCount / canDuiPaiMjTuoCount * 2;
            rCount = (duiCount % canDuiPaiMjTuoCount) * 2 + rCount;

            int useMjTuoCount = canDuiPaiMjTuoCount;

            if (canDuiPaiMjTuoCount == 3)
                useMjTuoCount -= 1;
            else if (canDuiPaiMjTuoCount == 4)
                useMjTuoCount -= 2;
            else
                useMjTuoCount = 1;

            int m2 = rCount / useMjTuoCount;
            int n2 = rCount % useMjTuoCount;


            int MjCountSeat0 = 0;
            int MjCountSeat1 = 0;
            int MjCountSeat2 = 0;
            int MjCountSeat3 = 0;




            if (isSeatMjTuoCanDuiPai[0])
            {
                MjCountSeat0 = tuoMjCount + m2 + n2;
                CreateMahjongPaiDuiSeat(0, 1, MjCountSeat0, 0.0002f, "desk_mjtuo_self", new Vector3(0, 0, 0.015f));
            }

            if (isSeatMjTuoCanDuiPai[1])
            {
                MjCountSeat1 = tuoMjCount;
                CreateMahjongPaiDuiSeat(1, MjCountSeat0 + 1, MjCountSeat1, 0.0005f, "desk_mjtuo_west", new Vector3(0.003f, 0, 0));
            }

            if (isSeatMjTuoCanDuiPai[2])
            {
                MjCountSeat2 = tuoMjCount + m2;
                CreateMahjongPaiDuiSeat(2, MjCountSeat0 + MjCountSeat1 + 1, MjCountSeat2, 0.0002f, "desk_mjtuo_north", new Vector3(0, 0, 0.004f));
            }

            if (isSeatMjTuoCanDuiPai[3])
            {
                MjCountSeat3 = tuoMjCount;
                CreateMahjongPaiDuiSeat(3, MjCountSeat0 + MjCountSeat1 + MjCountSeat2 + 1, MjCountSeat3, 0.0005f, "desk_mjtuo_east", new Vector3(0.004f, 0, 0.01f));
            }

        }

        void CreateMahjongPaiDuiSeat(int seat, int paiStartIdx, int mjCount, float mjSpacing, string mjTuoName, Vector3 mjTuoCenterOffset)
        {
            int upMjCount = mjCount / 2;
            int downMjCount = upMjCount;
            int richMjCount = mjCount % 2;

            if (richMjCount != 0)
                downMjCount += richMjCount;

            int paiIdx = paiStartIdx;
            Transform mjtuo_tf = mjtableTransform.Find(mjTuoName);
            float mjtuoHeight = mjtuo_tf.GetComponent<Renderer>().bounds.size.y;
            float tuoFacePosY = mjtuo_tf.position.y + mjTuoCenterOffset.y + mjtuoHeight / 2;

            GameObject premj = mjAssetsMgr.emptyMjPai;
            float mjSizeX = GetWallDuiMjSizeByAxis(Axis.X);
            float mjSizeZ = GetWallDuiMjSizeByAxis(Axis.Z);

            float mjAxisSpacing = mjSizeX + mjSpacing;
            float mjWallLen = downMjCount * mjSizeX + (downMjCount - 1) * mjSpacing;
            float mjStartPos = 0;


            switch (seat)
            {
                case 0:
                    mjStartPos = mjtuo_tf.position.x + mjTuoCenterOffset.x - mjWallLen / 2 + mjSizeX / 2;
                    break;

                case 1:
                    mjStartPos = mjtuo_tf.position.z + mjTuoCenterOffset.z + mjWallLen / 2 - mjSizeX / 2;
                    break;

                case 2:
                    mjStartPos = mjtuo_tf.position.x + mjTuoCenterOffset.x + mjWallLen / 2 - mjSizeX / 2;
                    break;

                case 3:
                    mjStartPos = mjtuo_tf.position.z + mjTuoCenterOffset.z - mjWallLen / 2 + mjSizeX / 2;
                    break;
            }

            float yOffset = mjSizeZ / 2 + mjSizeZ + 0.0002f;
            int udMjCount = upMjCount;

            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < udMjCount; i++)
                {
                    GameObject mj = Object.Instantiate(premj);
                    FitSeatWallDuiMj(seat, mj);
                    mj.name = "mjDuiPai_" + paiIdx;

                    switch (seat)
                    {
                        case 0:
                            mj.transform.position = new Vector3(mjStartPos + i * mjAxisSpacing, tuoFacePosY + yOffset, mjtuo_tf.position.z + mjTuoCenterOffset.z);
                            break;
                        case 1:
                            mj.transform.position = new Vector3(mjtuo_tf.position.x + mjTuoCenterOffset.x, tuoFacePosY + yOffset, mjStartPos - i * mjAxisSpacing);
                            break;
                        case 2:
                            mj.transform.position = new Vector3(mjStartPos - i * mjAxisSpacing, tuoFacePosY + yOffset, mjtuo_tf.position.z + mjTuoCenterOffset.z);
                            break;
                        case 3:
                            mj.transform.position = new Vector3(mjtuo_tf.position.x + mjTuoCenterOffset.x, tuoFacePosY + yOffset, mjStartPos + i * mjAxisSpacing);
                            break;
                    }

                    mj.transform.SetParent(mjtuo_tf, true);
                    mjDuiPai[paiIdx] = mj;

                    if (j == 0)
                    {
                        GameObject mjShadow = mjAssets.effectPrefabDict[(int)PrefabIdx.MJ_SHADOW][0];
                        Vector3 shadowLocalScale = mjShadow.transform.localScale;
                        Transform shadow = mj.transform.GetChild(0);
                        shadow.transform.localScale = new Vector3(wallduiMjScale.x * shadowLocalScale.x * 0.95f, wallduiMjScale.y * shadowLocalScale.y * 0.95f, 1);

                        mjDuiPaiUpDown[paiIdx] = MahjongUpDown.MG_UP;
                    }
                    else
                        mjDuiPaiUpDown[paiIdx] = MahjongUpDown.MG_DOWN;

                    if (j == 1 && i >= upMjCount - 1)
                        paiIdx++;
                    else
                        paiIdx += 2;
                }

                udMjCount = downMjCount;
                paiIdx = paiStartIdx + 1;
                yOffset = mjSizeZ / 2 + 0.0002f;
            }
        }
        #endregion


    }
}