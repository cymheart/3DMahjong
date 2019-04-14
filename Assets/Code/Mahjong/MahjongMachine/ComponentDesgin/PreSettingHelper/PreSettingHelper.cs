using CoreDesgin;
using UnityEngine;

namespace ComponentDesgin
{
    public class PreSettingHelper : MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        Desk desk;
        MahjongAssetsMgr mjAssetsMgr;
        Transform uiPositionHelperTransform;
        Transform mjTablePositionHelperTransform;
        Transform swapPaiControler;
        Fit fit;


        /// <summary>
        /// 各座位碰吃杠牌惯用手的方向
        /// </summary>
        public HandDirection[] pengPaiHandDirSeat = new HandDirection[]
        {
            HandDirection.RightHand,
            HandDirection.RightHand,
            HandDirection.RightHand,
            HandDirection.RightHand
        };

        #region 设置位置

        /// <summary>
        /// 各座位碰吃杠牌放置的开始位置
        /// </summary>
        public Vector3[] pengPaiStartPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 各座位碰吃杠牌放置的当前位置
        /// </summary>
        public Vector3[] pengPaiCurtPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 各座位交换牌中心位置
        /// </summary>
        public Vector3[] swapPaiCenterPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };


        /// <summary>
        /// 各座位骰子器启动位置
        /// </summary>
        public Vector3[] diceQiDongPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };


        /// <summary>
        /// 各座位碰吃杠听胡牌UI文字特效位置
        /// </summary>
        public Vector3[] pcgthEffectTextPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 各座位UI分数显示位置
        /// </summary>
        public Vector3[] uiScorePosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 各座位UI头像显示位置
        /// </summary>
        public Vector3[] uiTouXiangPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };


        /// <summary>
        /// 各座位定缺移动花色的起始位置
        /// </summary>
        public Vector3[] huaSeStartPosSeat = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 刮风特效位置
        /// </summary>
        public Vector3[] fengEffectPos = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 下雨特效位置
        /// </summary>
        public Vector3[] rainEffectPos = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
        };

        /// <summary>
        /// 桌面麻将托位置
        /// </summary>
        public Vector3[] deskMjTuoPos = new Vector3[]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero
        };


        public Transform[] swapPaiControlerSeat = new Transform[4];

        #endregion


        public override void Init()
        {
            Setting((MahjongMachine)Parent);
        }

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
            desk = mjMachine.GetComponent<Desk>();

            uiPositionHelperTransform = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.UI_POSITION_HELPER][0].transform;
            mjTablePositionHelperTransform = mjAssetsMgr.defaultPrefabDict[(int)PrefabIdx.TABLE_POSITION_HELPER][0].transform;
            swapPaiControler = mjAssetsMgr.uiPrefabDict[(int)PrefabIdx.SWAP_PAI_CONTROLER][0].transform;
        }

        public override void Load()
        {
            CreateInitLayoutPosForSeats();      
        }


        public override void ClearData()
        {
            ResetDeskPengPaiCurtPos();
        }


        #region 生成初始布局位置

        /// <summary>
        /// 生成初始布局位置
        /// </summary>
        void CreateInitLayoutPosForSeats()
        {
            CreateDiceQiDongPosForSeats();
            CreateSwapPaiCenterPosForSeats();
            CreateDeskPengPaiStartPosForSeats();
            CreateFengRainEffectPosForSeats();
            CreatePcgthEffectTextPosForSeats();
            CreateUIScorePosForSeats();
            CreateUITouXiangPosForSeats();
            CreateUIDingQueMoveHuaseStartPosForSeats();
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
            for (int i = 0; i < 4; i++)
                deskMjTuoPos[i] = desk.mjtableTransform.Find(desk.deskMjTuoName[i]).localPosition;
        }

        /// <summary>
        /// 生成碰，吃，杠牌的起始位置
        /// </summary>
        void CreateDeskPengPaiStartPosForSeats()
        {
            Transform pengPaiPos = mjTablePositionHelperTransform.Find("PengPaiPos");
            float y = Desk.deskFacePosY + fit.GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.0002f;

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

        void ResetDeskPengPaiCurtPos()
        {
            pengPaiCurtPosSeat[0] = pengPaiStartPosSeat[0];
            pengPaiCurtPosSeat[1] = pengPaiStartPosSeat[1];
            pengPaiCurtPosSeat[2] = pengPaiStartPosSeat[2];
            pengPaiCurtPosSeat[3] = pengPaiStartPosSeat[3];
        }

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

            swapPaiControlerSeat[0] = swapPaiControler.Find("seat0");
            swapPaiControlerSeat[1] = swapPaiControler.Find("seat1");
            swapPaiControlerSeat[2] = swapPaiControler.Find("seat2");
            swapPaiControlerSeat[3] = swapPaiControler.Find("seat3");

        }

        #endregion
    }
}
