using CoreDesgin;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ComponentDesgin
{
    public partial class Fit
    {
        public override void SetInitMethod()
        {
            base.SetInitMethod();

            AddInitMethodToParent(Setting, 3);
            AddInitMethodToParent(Load, 4);
        }



        public void Setting()
        {
            mjMachine = (MahjongMachine)Parent;
            desk = mjMachine.GetComponent<Desk>();
            moPaiToHandPaiCanvasOffset = moPaiToHandPaiOffset * 5000f;
            premj = mjAssetsMgr.mjPai[(int)MahjongFaceValue.MJ_ZFB_FACAI];
            premjSize = premj.transform.GetComponent<Renderer>().bounds.size;

            uiPositionHelperTransform =;
            mjTablePositionHelperTransform = ;
            swapPaiControler = ;

            SetSeatCanUse();
            SetSeatMjTuoCanDuiPai();
        }

        public void Load()
        {
            handPaiSelectOffsetHeight = GetCanvasHandMjSizeByAxis(Axis.Y) / 3;
            CreateInitLayoutPosForSeats();
        }

        public override void ClearData()
        {
            curtPaiDuiPos = 0;
            paiDuiRichCount = mjPaiTotalCount;
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
        public int GetNextCanUseSeatIdxByOrderSeat(ref int curtPlayerOrderIdx)
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
        public int GetNextSeatIdx(int curtSeatIdx, bool isClockwise = true)
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


        #region 麻将尺寸位置相关功能

        /// <summary>
        /// 设置屏幕手牌麻将的缩放尺寸
        /// </summary>
        /// <param name="scale"></param>
        public void SetCanvasHandMjScale(float scale = 0)
        {
            if (scale == 0)
            {
                float s1 = 1920f / 1080f;
                float s2 = (float)Screen.width / (float)Screen.height;
                scale = (s2 / s1) * 1.3f;
            }

            canvasHandMjScale = new Vector3(scale, scale, scale);
        }

        
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
                    scale = 1f / desk.canvasHandPaiRectTransform.localScale.x;
                    size *= scale;
                    break;

                case Axis.Y:
                    size = premjSize.y;
                    scale = 1f / desk.canvasHandPaiRectTransform.localScale.y;
                    size *= scale;
                    break;

                case Axis.Z:
                    size = premjSize.z;
                    scale = 1f / desk.canvasHandPaiRectTransform.localScale.z;
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
                    scale = canvasHandMjScale.x / desk.canvasHandPaiRectTransform.localScale.x;
                    size *= scale;
                    break;

                case Axis.Y:
                    size = premjSize.y;
                    scale = canvasHandMjScale.y / desk.canvasHandPaiRectTransform.localScale.y;
                    size *= scale;
                    break;

                case Axis.Z:
                    size = premjSize.z;
                    scale = canvasHandMjScale.z / desk.canvasHandPaiRectTransform.localScale.z;
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

            GameObject mjShadow = mjAssetsMgr.effectPrefabDict[(int)PrefabIdx.MJ_SHADOW][0];
            GameObject shadow = UnityEngine.Object.Instantiate(mjShadow);

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
            fitMj.transform.eulerAngles = new Vector3(-desk.canvasHandPaiRectTransform.eulerAngles.x, 0, 0);

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

    }
}
