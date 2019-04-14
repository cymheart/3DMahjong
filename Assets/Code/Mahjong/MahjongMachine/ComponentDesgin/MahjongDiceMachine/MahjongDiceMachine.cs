using DG.Tweening;
using CoreDesgin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ComponentDesgin
{
    /// <summary>
    /// 骰子机状态
    /// </summary>
    public enum DiceMachineState
    {
        START_RUN,

        GAIZI_OPENING,
        GAIZI_OPEND,

        GAIZI_CLOSING,
        GAIZI_CLOSED,

        THROWING_DICE,
        THROWED_DICE,

        TIMING,
        TIMER_END,

        END_RUNNING,
        END_RUN,
    }

    public class MahjongDiceMachine: MahjongMachineComponent
    {
        /// <summary>
        /// 骰子翻转到指定点数的角度
        /// </summary>
        Vector3[] dicePointByEulerAngles = new Vector3[]
        {
            Vector3.zero,
            new Vector3(90,0,0),      //1
            new Vector3(0,90,0),      //2
            Vector3.zero,       //3
            new Vector3(180,0,0),     //4
            new Vector3(0,-90,0),     //5
            new Vector3(90,-90,90)    //6
        };


        float[,] FengWeiRot = new float[4, 4]
        {
            { -90f, 0, 90f, 180f},
            { 0f, 90, 180f, -90f },
            { 90f, 180, -90f, 0f },
            { 180f, -90, 0f, 90f },
        };


        MahjongMachine mjMachine;
        MahjongGame mjGame;
        Desk desk;
        MahjongAssetsMgr mjAssetsMgr;
        SettingDataAssetsMgr settingDataAssetsMgr;
        Transform mjTable;
        GameObject prefabDiceMachine;

        Transform diceMachineTransform;

        Transform gaiziTransform;
        Material gaiziMat = null;

        Transform diceTransform;
        Animation diceAnim;
        Transform dice1Transform;
        Transform dice2Transform;

        Transform qiDongQiDictBoxTransform;

        Color openGaiZiColor = new Color(0.737f, 0.737f, 0.737f, 0f);
        Color closeGaiZiColor = new Color(0.737f, 0.737f, 0.737f, 1f);

        DiceMachineState state = DiceMachineState.END_RUN;

        public int[] dicePoint = new int[2] { 1, 1 };

        Transform qiDongQi;
        Transform[] fengWeiQiDongQiOff = new Transform[4];
        Transform[] fengWeiQiDongQiOn = new Transform[4];
        FengWei curtOnFengWei = FengWei.EAST;

        Tweener qiDongQiMatTweener;
        Color qiDongQiOnStartFlashColor = new Color(0.66f, 0.66f, 0.66f, 1f);
        Color qiDongQiOnEndFlashColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        Transform[] timerNumTransform = new Transform[2];
        SpriteRenderer[] timerNumSpriteRenderer = new SpriteRenderer[2];

        CountdownSecondTimer cdsTimer = new CountdownSecondTimer();
        Dictionary<int, Sprite[]> numSpritesDict;
        Sprite[] blueNums;


        public override void Init()
        {
            Setting((MahjongMachine)Parent);
        }

        /// <summary>
        /// 设置骰子机
        /// </summary>
        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjGame = mjMachine.mjGame;
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
            settingDataAssetsMgr = mjMachine.GetComponent<SettingDataAssetsMgr>();
            desk = mjMachine.GetComponent<Desk>();          
        }

        public override void Load()
        {
            mjTable = desk.mjtableTransform;
            numSpritesDict = settingDataAssetsMgr.GetSpritesIntKeyDict((int)SpritesType.DICE_MACHINE_TIMER_NUM);
            blueNums = numSpritesDict[(int)SpriteIdx.BLUE_NUMS];
            prefabDiceMachine = mjAssetsMgr.defaultPrefabDict[(int)PrefabIdx.DICE_MACHINE][0];

            diceMachineTransform = Object.Instantiate(prefabDiceMachine, mjTable).transform;
            mjAssetsMgr.AppendToDestoryPool(diceMachineTransform.gameObject);

            gaiziTransform = diceMachineTransform.Find("QiDongQiGanZi");
            Renderer renderer = gaiziTransform.GetComponent<Renderer>();
            gaiziMat = renderer.sharedMaterial;

            diceTransform = diceMachineTransform.Find("dice");
            diceAnim = diceTransform.GetComponent<Animation>();
            dice1Transform = diceTransform.Find("diceRot").Find("dice1");
            dice2Transform = diceTransform.Find("diceRot").Find("dice2");

            qiDongQiDictBoxTransform = diceMachineTransform.Find("QiDongQiDictBox");

            timerNumTransform[0] = diceMachineTransform.Find("Num1");
            timerNumSpriteRenderer[0] = timerNumTransform[0].GetComponent<SpriteRenderer>();
            timerNumTransform[0].gameObject.SetActive(false);

            timerNumTransform[1] = diceMachineTransform.Find("Num2");
            timerNumSpriteRenderer[1] = timerNumTransform[1].GetComponent<SpriteRenderer>();
            timerNumTransform[1].gameObject.SetActive(false);

            qiDongQi = diceMachineTransform.Find("QiDongQi");

            fengWeiQiDongQiOff[(int)FengWei.EAST] = qiDongQi.Find("QiDongQiEast");
            fengWeiQiDongQiOff[(int)FengWei.SOUTH] = qiDongQi.Find("QiDongQiSouth");
            fengWeiQiDongQiOff[(int)FengWei.WEST] = qiDongQi.Find("QiDongQiWest");
            fengWeiQiDongQiOff[(int)FengWei.NORTH] = qiDongQi.Find("QiDongQiNorth");

            fengWeiQiDongQiOn[(int)FengWei.EAST] = qiDongQi.Find("QiDongQiEastLight");
            fengWeiQiDongQiOn[(int)FengWei.SOUTH] = qiDongQi.Find("QiDongQiSouthLight");
            fengWeiQiDongQiOn[(int)FengWei.WEST] = qiDongQi.Find("QiDongQiWestLight");
            fengWeiQiDongQiOn[(int)FengWei.NORTH] = qiDongQi.Find("QiDongQiNorthLight");


            renderer = qiDongQi.Find("QiDongQiEastLight").GetComponent<Renderer>();

            renderer.sharedMaterial.SetColor("_Color", qiDongQiOnStartFlashColor);
            qiDongQiMatTweener = renderer.sharedMaterial.DOColor(qiDongQiOnEndFlashColor, 0.5f);
            qiDongQiMatTweener.SetAutoKill(false);
            qiDongQiMatTweener.SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            qiDongQiMatTweener.Pause();
            mjAssetsMgr.AppendToDestoryPool(qiDongQiMatTweener);
        }

        public void Destory()
        {
            qiDongQiMatTweener.Kill();
            Object.Destroy(diceMachineTransform.gameObject);
        }

        /// <summary>
        /// 设置指定座位的风位
        /// </summary>
        /// <param name="seatIdx"></param>
        public void SetSeatFengWei(int seatIdx, FengWei fengWei)
        {
            qiDongQi.transform.localEulerAngles = new Vector3(0, FengWeiRot[seatIdx, (int)fengWei], 0);
        }

        public void OnFengWei(FengWei fengWei)
        {
            OffFengWei(curtOnFengWei);

            fengWeiQiDongQiOn[(int)fengWei].gameObject.SetActive(true);
            fengWeiQiDongQiOff[(int)fengWei].gameObject.SetActive(false);
            qiDongQiMatTweener.Play();
            curtOnFengWei = fengWei;
        }

        public void OffFengWei(FengWei fengWei)
        {
            qiDongQiMatTweener.Pause();

            fengWeiQiDongQiOn[(int)fengWei].gameObject.SetActive(false);
            fengWeiQiDongQiOff[(int)fengWei].gameObject.SetActive(true);
        }

        public void SetLimitTime(int tm)
        {
            cdsTimer.SetLimitTime(tm);
        }

        public void StartTime()
        {
            cdsTimer.StartTime();
        }

        public int GetCurtTime()
        {
            return cdsTimer.GetCurtTime();
        }

        public void Update()
        {
            switch (state)
            {
                case DiceMachineState.START_RUN:
                    OpenGanZi();
                    break;

                case DiceMachineState.GAIZI_OPEND:
                    ThrowDice(dicePoint[0], dicePoint[1]);
                    break;

                case DiceMachineState.THROWED_DICE:
                    CloseGanZi();
                    break;

                case DiceMachineState.GAIZI_CLOSED:
                    StartTime();
                    state = DiceMachineState.TIMING;
                    timerNumTransform[0].gameObject.SetActive(true);
                    timerNumTransform[1].gameObject.SetActive(true);
                    break;

                case DiceMachineState.TIMING:
                    {
                        int tm = cdsTimer.Timing();
                        int[] nums = cdsTimer.GetGetCurtTimeNums();

                        timerNumSpriteRenderer[0].sprite = blueNums[nums[0]];
                        timerNumSpriteRenderer[1].sprite = blueNums[nums[1]];

                        if (tm == 0)
                            state = DiceMachineState.TIMER_END;
                    }
                    break;
            }
        }

        public void StartRun(int dice1Point = -1, int dice2Point = -1)
        {
            if (state != DiceMachineState.END_RUN)
                return;

            if (dice1Point <= -1)
                dice1Point = Random.Range(1, 7);

            if (dice2Point <= -1)
                dice2Point = Random.Range(1, 7);

            dicePoint[0] = dice1Point;
            dicePoint[1] = dice2Point;

            state = DiceMachineState.START_RUN;
        }

        public void EndRun(bool isDirect = false)
        {
            if (state == DiceMachineState.END_RUNNING ||
                state == DiceMachineState.END_RUN)
                return;

            float waitTime = 1f;
            if (isDirect)
                waitTime = 0.01f;

            state = DiceMachineState.END_RUNNING;
            gaiziMat.DOColor(closeGaiZiColor, waitTime);
            mjGame.StartCoroutine(ActionEndRun(waitTime));

        }

        public void OpenGanZi()
        {
            if (state != DiceMachineState.START_RUN &&
                state != DiceMachineState.GAIZI_CLOSED)
                return;

            state = DiceMachineState.GAIZI_OPENING;
            diceTransform.gameObject.SetActive(true);
            // qiDongQiDictBoxTransform.gameObject.SetActive(true);
            gaiziMat.DOColor(openGaiZiColor, 1f);
            mjGame.StartCoroutine(ActionOpenOrCloseGaiZi(0, 1f));
        }

        public void CloseGanZi()
        {
            if (state != DiceMachineState.GAIZI_OPEND &&
                state != DiceMachineState.THROWED_DICE)
                return;

            state = DiceMachineState.GAIZI_CLOSING;
            gaiziMat.DOColor(closeGaiZiColor, 1f);
            mjGame.StartCoroutine(ActionOpenOrCloseGaiZi(1, 1f));
        }

        /// <summary>
        /// 掷骰子
        /// </summary>
        public void ThrowDice(int dice1Point, int dice2Point)
        {
            if (state != DiceMachineState.GAIZI_OPEND)
                return;

            state = DiceMachineState.THROWING_DICE;

            dice1Point = Mathf.Min(dice1Point, 6);
            dice1Point = Mathf.Max(dice1Point, 1);

            dice2Point = Mathf.Min(dice2Point, 6);
            dice2Point = Mathf.Max(dice2Point, 1);

            mjGame.StartCoroutine(ActionThrowDice(dice1Point, dice2Point));
        }

        IEnumerator ActionOpenOrCloseGaiZi(int type, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            if (type == 0)
            {
                state = DiceMachineState.GAIZI_OPEND;
            }
            else
            {
                state = DiceMachineState.GAIZI_CLOSED;
                diceTransform.gameObject.SetActive(false);
                //  qiDongQiDictBoxTransform.gameObject.SetActive(false);
            }
        }

        IEnumerator ActionEndRun(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            state = DiceMachineState.END_RUN;
            diceTransform.gameObject.SetActive(false);
            OffFengWei(curtOnFengWei);

            cdsTimer.SetCurtTime(0);
            timerNumSpriteRenderer[0].sprite = blueNums[0];
            timerNumSpriteRenderer[1].sprite = blueNums[0];

            timerNumTransform[0].gameObject.SetActive(false);
            timerNumTransform[1].gameObject.SetActive(false);
        }

        /// <summary>
        /// 掷骰子动作
        /// </summary>
        /// <returns></returns>
        IEnumerator ActionThrowDice(int dice1Point, int dice2Point)
        {
            AudioClip clip = mjMachine.GetComponent<Audio>().GetEffectAudio((int)AudioIdx.AUDIO_EFFECT_DICE);
            AudioSource.PlayClipAtPoint(clip, qiDongQiDictBoxTransform.position);

            float waitTime = diceAnim.GetClip("ReadyZhuanDice").length / diceAnim["ReadyZhuanDice"].speed;
            diceAnim.Play("ReadyZhuanDice");
            yield return new WaitForSeconds(waitTime);

            diceAnim["LoopZhuanDice"].speed = 4f;
            waitTime = diceAnim.GetClip("LoopZhuanDice").length / diceAnim["LoopZhuanDice"].speed;
            diceAnim.Play("LoopZhuanDice");
            yield return new WaitForSeconds(waitTime * 4);

            diceAnim["LoopZhuanDice"].speed = 1.5f;
            waitTime = diceAnim.GetClip("LoopZhuanDice").length / diceAnim["LoopZhuanDice"].speed;
            diceAnim.Play("LoopZhuanDice");
            yield return new WaitForSeconds(waitTime * 2);


            diceAnim["SlowSpeedZhuanDice"].speed = 1f;
            waitTime = diceAnim.GetClip("SlowSpeedZhuanDice").length / diceAnim["SlowSpeedZhuanDice"].speed;
            diceAnim.Play("SlowSpeedZhuanDice");
            yield return new WaitForSeconds(waitTime);

            diceAnim["LastAdjustDice"].speed = 0.6f;
            diceAnim.Play("LastAdjustDice");
            waitTime = diceAnim.GetClip("LastAdjustDice").length / diceAnim["LastAdjustDice"].speed;

            float x1 = Random.Range(-0.006f, 0.006f);
            float y1 = Random.Range(-0.006f, 0.006f);

            float x2 = Random.Range(-0.006f, 0.006f);
            float y2 = Random.Range(-0.006f, 0.006f);

            dice1Transform.DOLocalMove(new Vector3(x1, y1, 0), waitTime).SetRelative();
            dice1Transform.DOLocalRotate(dicePointByEulerAngles[dice1Point], waitTime);

            dice2Transform.DOLocalMove(new Vector3(x2, y2, 0), waitTime).SetRelative();
            dice2Transform.DOLocalRotate(dicePointByEulerAngles[dice2Point], waitTime);

            yield return new WaitForSeconds(1f);

            state = DiceMachineState.THROWED_DICE;

        }

    }
}