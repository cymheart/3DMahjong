using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MahjongMachineNS
{
    public partial class MahjongMachine
    {
        #region 测试功能

        Button btnAction;
        int mn = 0;
        int idx = 0;
        void TestMahjongFuncInterface()
        {
            btnAction = uiCanvasTransform.Find("TestUI").Find("btnAction").GetComponent<Button>();
            EventTriggerListener.Get(btnAction.gameObject).onClick = OnButtonClick;
        }
        private void OnButtonClick(GameObject go)
        {
            if (go == btnAction.gameObject)
            {
                Action();
            }
        }

        public List<MahjongFaceValue> CreateSelfHandPaiFaceValueList(MahjongFaceValue[] mjFaceValueList)
        {
            List<MahjongFaceValue> list = new List<MahjongFaceValue>();
            foreach (MahjongFaceValue value in mjFaceValueList)
            {
                list.Add(value);
            }

            return list;
        }

        #endregion


        void Action()
        {
            string playerName = uiCanvasTransform.Find("TestUI").Find("PlayerDropdown").Find("Label").GetComponent<Text>().text;
            string actionName = uiCanvasTransform.Find("TestUI").Find("ActionDropdown").Find("Label").GetComponent<Text>().text;
            int seatIdx = 1;

            switch (playerName)
            {
                case "玩家0": seatIdx = 0; break;
                case "玩家1": seatIdx = 1; break;
                case "玩家2": seatIdx = 2; break;
                case "玩家3": seatIdx = 3; break;
            }

            ActionCombineNum actionCombineNum = GetRandomHandDaPaiActionNumForNextDeskMjPos(seatIdx);
            MahjongFaceValue fv = (MahjongFaceValue)Random.Range(0, 42);

            MahjongFaceValue[] fvs = new MahjongFaceValue[] { MahjongFaceValue.MJ_TIAO_4, MahjongFaceValue.MJ_TIAO_6, MahjongFaceValue.MJ_TIAO_4, MahjongFaceValue.MJ_TIAO_6, MahjongFaceValue.MJ_TONG_6 };

            switch (actionName)
            {
                case "洗牌":
                    {
                        XiPaiCmd cmd = (XiPaiCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.XiPai);
                        cmd.isBlock = true;
                        mjOpCmdList.Append(cmd);

                        FaPaiCmd cmd2 = (FaPaiCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.FaPai);

                        MahjongFaceValue[] values = new MahjongFaceValue[]
                        {
                        MahjongFaceValue.MJ_TONG_1, MahjongFaceValue.MJ_TONG_1, MahjongFaceValue.MJ_TONG_1,
                        MahjongFaceValue.MJ_TONG_2, MahjongFaceValue.MJ_TONG_3, MahjongFaceValue.MJ_TONG_4,
                        MahjongFaceValue.MJ_TONG_5, MahjongFaceValue.MJ_TONG_6, MahjongFaceValue.MJ_TONG_6,
                        MahjongFaceValue.MJ_TONG_6,MahjongFaceValue.MJ_TONG_6,MahjongFaceValue.MJ_TONG_7,
                        MahjongFaceValue.MJ_TONG_8,MahjongFaceValue.MJ_FENG_DONG,
                        };

                       // cmd2.delayExecuteTime = 2f;
                        cmd2.startPaiIdx = 15;
                        cmd2.mjHandSelfPaiFaceValueList = CreateSelfHandPaiFaceValueList(values);
                        //cmd.Append(mjOpCmdList.CreateCmdNode(cmd2));

                        mjOpCmdList.Append(cmd2);

                    }
                    break;

                case "发牌":
                    {
                        FaPaiCmd cmd = (FaPaiCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.FaPai);

                        MahjongFaceValue[] values = new MahjongFaceValue[]
                        {
                        MahjongFaceValue.MJ_TONG_1, MahjongFaceValue.MJ_TONG_1, MahjongFaceValue.MJ_TONG_1,
                        MahjongFaceValue.MJ_TONG_2, MahjongFaceValue.MJ_TONG_3, MahjongFaceValue.MJ_TONG_4,
                        MahjongFaceValue.MJ_TONG_5, MahjongFaceValue.MJ_TONG_6, MahjongFaceValue.MJ_TONG_6,
                        MahjongFaceValue.MJ_TONG_6,MahjongFaceValue.MJ_TONG_6,MahjongFaceValue.MJ_TONG_7,
                        MahjongFaceValue.MJ_TONG_8,MahjongFaceValue.MJ_FENG_DONG,
                        };

                        cmd.startPaiIdx = 15;
                        cmd.mjHandSelfPaiFaceValueList = CreateSelfHandPaiFaceValueList(values);

                        mjOpCmdList.Append(cmd);
                    }
                    break;


                case "头像流光On":
                    uiTouXiang.LiuGuangTurnTo(seatIdx);
                    uiTouXiang.ShowHuaSeFlag(seatIdx, MahjongHuaSe.TIAO);
                    break;

                case "头像流光Off":
                    uiTouXiang.LiuGuangStop(seatIdx);
                    break;

                case "设置头像分值":
                    uiTouXiang.SetScore(seatIdx, 3600);
                    break;

                case "启动动作":
                    {
                        QiDongDiceMachine(seatIdx);
                    }
                    break;

                case "启动骰子机":
                    {
                        diceMachine.StartRun();
                    }
                    break;

                case "选择打牌":
                    {
                        ReqSelectDaPaiOpCmd cmdx = (ReqSelectDaPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ReqSelectDaPai);
                        mjOpCmdList.Append(cmdx);
                    }
                    break;

                case "cmd":
                    {
                        QiDongDiceMachineCmd qiDongCmd = (QiDongDiceMachineCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.QiDongDiceMachine);
                        qiDongCmd.seatIdx = 0;
                        mjOpCmdList.Append(qiDongCmd);

                        ReqSelectDaPaiOpCmd cmdx = (ReqSelectDaPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ReqSelectDaPai);
                        mjOpCmdList.Append(cmdx);



                        MahjongDaPaiOpCmd cmd = (MahjongDaPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.DaPai);
                        cmd.seatIdx = 1;
                        cmd.handStyle = PlayerType.FEMALE;
                        cmd.paiIdx = 3;
                        cmd.paiType = HandPaiType.HandPai;
                        cmd.mjFaceValue = MahjongFaceValue.MJ_FENG_DONG;
                        mjOpCmdList.Append(cmd);

                        MahjongMoPaiOpCmd cmdm = (MahjongMoPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.MoPai);
                        cmdm.seatIdx = 1;
                        cmdm.handStyle = PlayerType.FEMALE;
                        cmdm.mjFaceValue = MahjongFaceValue.MJ_TIAO_1;
                        mjOpCmdList.Append(cmdm);

                        MahjongChaPaiOpCmd cmdc = (MahjongChaPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ChaPai);
                        cmdc.seatIdx = 1;
                        cmdc.adjustDirection = HandPaiAdjustDirection.GoToHandLeftDir;
                        cmdc.dstHandPaiIdx = 3;
                        cmdc.orgPaiIdx = 0;
                        cmdc.orgPaiType = HandPaiType.MoPai;
                        cmdc.handStyle = PlayerType.FEMALE;
                        mjOpCmdList.Append(cmdc);

                        TurnNextPlayerOpCmd turnCmd = (TurnNextPlayerOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.TurnNextPlayer);
                        turnCmd.waitActionEndPlayerSeatIdxs = new int[] { 1 };
                        turnCmd.turnToPlayerSeatIdx = 2;
                        turnCmd.time = 10;
                        mjOpCmdList.Append(turnCmd);

                        cmd = (MahjongDaPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.DaPai);
                        cmd.seatIdx = 2;
                        cmd.handStyle = PlayerType.FEMALE;
                        cmd.paiIdx = 0;
                        cmd.paiType = HandPaiType.HandPai;
                        cmd.mjFaceValue = MahjongFaceValue.MJ_TIAO_6;
                        mjOpCmdList.Append(cmd);

                        cmdm = (MahjongMoPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.MoPai);
                        cmdm.seatIdx = 2;
                        cmdm.handStyle = PlayerType.FEMALE;
                        cmdm.mjFaceValue = MahjongFaceValue.MJ_TIAO_4;
                        mjOpCmdList.Append(cmdm);

                        cmdc = (MahjongChaPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ChaPai);
                        cmdc.seatIdx = 2;
                        cmdc.adjustDirection = HandPaiAdjustDirection.GoToHandLeftDir;
                        cmdc.dstHandPaiIdx = 3;
                        cmdc.orgPaiIdx = 0;
                        cmdc.orgPaiType = HandPaiType.MoPai;
                        cmdc.handStyle = PlayerType.FEMALE;
                        mjOpCmdList.Append(cmdc);


                        MahjongHuPaiOpCmd cmd2 = (MahjongHuPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.HuPai);
                        cmd2.seatIdx = 1;
                        cmd2.handStyle = PlayerType.FEMALE;
                        cmd2.huTargetSeatIdx = 2;
                        cmd2.huTargetMjIdx = new Vector3Int(0, 0, 0);
                        cmd2.huPaiFaceValue = MahjongFaceValue.MJ_TONG_9;
                        mjOpCmdList.Append(cmd2);


                        ////turnCmd = new TurnNextPlayerOpCmd();
                        ////turnCmd.waitActionEndPlayerSeatIdxs = new int[] { 2 };
                        ////turnCmd.turnToPlayerSeatIdx = 3;
                        ////turnCmd.time = 10;
                        ////mjOpCmdList.Append(turnCmd);


                        ////cmdm = new MahjongMoPaiOpCmd();
                        ////cmdm.seatIdx = 3;
                        ////cmdm.handStyle = PlayerType.FEMALE;
                        ////cmdm.mjFaceValue = MahjongFaceValue.MJ_TIAO_4;
                        ////mjOpCmdList.Append(cmdm);


                        ////cmd = new MahjongDaPaiOpCmd();
                        ////cmd.seatIdx = 3;
                        ////cmd.handStyle = PlayerType.FEMALE;
                        ////cmd.paiIdx = 0;
                        ////cmd.paiType = HandPaiType.HandPai;
                        ////cmd.mjFaceValue = MahjongFaceValue.MJ_WANG_7;
                        ////mjOpCmdList.Append(cmd);

                        ////cmdc = new MahjongChaPaiOpCmd();
                        ////cmdc.seatIdx = 3;
                        ////cmdc.adjustDirection = HandPaiAdjustDirection.GoToHandLeftDir;
                        ////cmdc.dstHandPaiIdx = 3;
                        ////cmdc.orgPaiIdx = 0;
                        ////cmdc.orgPaiType = HandPaiType.MoPai;
                        ////cmdc.handStyle = PlayerType.FEMALE;
                        ////mjOpCmdList.Append(cmdc);


                        ////turnCmd = new TurnNextPlayerOpCmd();

                        ////turnCmd.waitActionEndPlayerSeatIdxs = new int[] { 3 };
                        ////turnCmd.turnToPlayerSeatIdx = 0;
                        ////turnCmd.time = 10;
                        ////mjOpCmdList.Append(turnCmd);




                        ////cmd = new MahjongDaPaiOpCmd();
                        ////cmd.seatIdx = 0;
                        ////cmd.handStyle = PlayerType.FEMALE;
                        ////cmd.paiIdx = 0;
                        ////cmd.paiType = HandPaiType.HandPai;
                        ////cmd.mjFaceValue = MahjongFaceValue.MJ_WANG_8;
                        ////mjOpCmdList.Append(cmd);
                    }

                    break;

                case "cmd2":
                    {
                        ReqSelectQueMenOpCmd cmdx = (ReqSelectQueMenOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ReqSelectQueMen);
                        cmdx.defaultQueMenHuaSe = MahjongHuaSe.WANG;
                        mjOpCmdList.Append(cmdx);

                        //MahjongSwapPaiGroupCmd cmd = new MahjongSwapPaiGroupCmd();
                        //cmd.SwapDirection = SwapPaiDirection.OPPOSITE;
                        //cmd.SwapHandPaiIdx = new int[] { 1, 2, 3 };
                        //cmd.TakeMjFaceValues = new MahjongFaceValue[] { MahjongFaceValue.MJ_TIAO_3, MahjongFaceValue.MJ_WANG_4, MahjongFaceValue.MJ_WANG_9 };
                        //mjOpCmdList.Append(cmd);
                    }
                    break;

                case "cmd3":
                    {
                        ReqSelectQueMenOpCmd cmdx = (ReqSelectQueMenOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ReqSelectQueMen);
                        cmdx.defaultQueMenHuaSe = MahjongHuaSe.WANG;
                        mjOpCmdList.Append(cmdx);

                        MahjongHuPaiOpCmd cmd = (MahjongHuPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.HuPai);
                        cmd.seatIdx = 1;
                        cmd.handStyle = PlayerType.FEMALE;
                        cmd.huTargetSeatIdx = 2;
                        cmd.huTargetMjIdx = new Vector3Int(0, 0, 0);
                        cmd.huPaiFaceValue = MahjongFaceValue.MJ_TONG_9;
                        cmd.delayExecuteTime = 2f;
                        cmdx.Append(mjOpCmdList.CreateCmdNode(cmd));

                        ShowScoreCmd scoreCmd = (ShowScoreCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ShowScore);
                        scoreCmd.seatScores = new int[] { -200, -300, -1550, 6000 };
                        scoreCmd.delayExecuteTime = 0.5f;
                        cmd.Append(mjOpCmdList.CreateCmdNode(scoreCmd));

                    }
                    break;


                case "请求碰吃杠听胡":
                    {
                        ReqSelectPCGTHPaiOpCmd cmd = (ReqSelectPCGTHPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ReqSelectPCGTHPai);
                        cmd.pcgthBtnTypes = new PengChiGangTingHuType[]
                        {
                            PengChiGangTingHuType.HU, PengChiGangTingHuType.PENG,
                            PengChiGangTingHuType.CHI, PengChiGangTingHuType.TING,
                            PengChiGangTingHuType.GANG , PengChiGangTingHuType.GUO
                        };

                        cmd.chiPaiMjValueList = new List<MahjongFaceValue[]>();

                        MahjongFaceValue[] values = new MahjongFaceValue[]
                        {
                            MahjongFaceValue.MJ_TONG_3,
                            MahjongFaceValue.MJ_TONG_4,
                            MahjongFaceValue.MJ_TONG_5,
                        };

                        MahjongFaceValue[] values2 = new MahjongFaceValue[]
                     {
                            MahjongFaceValue.MJ_TONG_5,
                            MahjongFaceValue.MJ_TONG_6,
                            MahjongFaceValue.MJ_TONG_7,
                     };

                        cmd.chiPaiMjValueList.Add(values);
                        cmd.chiPaiMjValueList.Add(values2);


                        cmd.tingPaiInHandPaiIdxs = new int[] { 2, 3, 6, 8 };

                        HuPaiTipsInfo[] tingPaiTipsInfos = new HuPaiTipsInfo[4];

                        tingPaiTipsInfos[0].faceValue = MahjongFaceValue.MJ_TIAO_8;
                        tingPaiTipsInfos[0].fanAmount = 5;
                        tingPaiTipsInfos[0].zhangAmount = 6;

                        tingPaiTipsInfos[1].faceValue = MahjongFaceValue.MJ_TIAO_2;
                        tingPaiTipsInfos[2].faceValue = MahjongFaceValue.MJ_TIAO_3;
                        tingPaiTipsInfos[3].faceValue = MahjongFaceValue.MJ_WANG_5;


                        HuPaiTipsInfo[] tingPaiTipsInfos2 = new HuPaiTipsInfo[3];

                        tingPaiTipsInfos2[0].faceValue = MahjongFaceValue.MJ_TIAO_8;
                        tingPaiTipsInfos2[0].fanAmount = 5;
                        tingPaiTipsInfos2[0].zhangAmount = 6;

                        tingPaiTipsInfos2[1].faceValue = MahjongFaceValue.MJ_TIAO_2;
                        tingPaiTipsInfos2[2].faceValue = MahjongFaceValue.MJ_WANG_5;


                        cmd.tingPaiInfosInHandPai = new List<HuPaiTipsInfo[]>
                    {
                        tingPaiTipsInfos,
                        tingPaiTipsInfos2,
                        tingPaiTipsInfos,
                        tingPaiTipsInfos2

                    };


                        mjOpCmdList.Append(cmd);
                    }
                    break;

                case "选择交换牌命令":
                    {
                        ReqSelectSwapPaiOpCmd cmd = (ReqSelectSwapPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.ReqSelectSwapPai);
                        mjOpCmdList.Append(cmd);
                    }
                    break;

                case "显示分数":
                    {
                        uiScore.Show(new int[] { 2000, 110, 560, 1255 });
                    }
                    break;


                case "缺一门":
                    {
                        if (seatIdx == 0)
                            uiSelectQueMen.Show(MahjongHuaSe.TIAO);
                        else
                        {
                            uiSelectQueMen.PlaySelectQueMenForOtherSeat(seatIdx, MahjongHuaSe.TONG);
                        }
                    }
                    break;



                case "交换牌":
                    SwapPai(seatIdx, 1, 3, new int[] { 3, 3, 3 }, new MahjongFaceValue[] { MahjongFaceValue.MJ_FENG_NAN, MahjongFaceValue.MJ_TIAO_4, MahjongFaceValue.MJ_TONG_3 }, new int[] { 3, 7, 9 });
                    break;

                case "显示胡牌按键":
                    //uiPcghtBtnMgr.Show(new PengChiGangTingHuType[] {
                    //    PengChiGangTingHuType.HU, PengChiGangTingHuType.PENG,
                    //    PengChiGangTingHuType.CHI, PengChiGangTingHuType.TING,
                    //    PengChiGangTingHuType.GANG , PengChiGangTingHuType.GUO});


                    uiPcghtBtnMgr.Show(new PengChiGangTingHuType[] {
                        PengChiGangTingHuType.HU,PengChiGangTingHuType.GANG , PengChiGangTingHuType.GUO});

                    break;

                case "显示碰牌按键":
                    // uiPcgtBtn.Show();
                    break;

                case "摸牌":

                    MoPai(seatIdx, MahjongFaceValue.MJ_TIAO_4);

                    // uiPcgtBtn.Show();
                    // uiHuBtn.Show();

                    //ReqSelectDaPaiOpCmd cmdxx = new ReqSelectDaPaiOpCmd();
                    //cmdxx.huPaiInHandPaiIdxs = new int[] { 2, 3, 6, 8 };

                    //HuPaiTipsInfo[] huPaiTipsInfos = new HuPaiTipsInfo[4];

                    //huPaiTipsInfos[0].faceValue = MahjongFaceValue.MJ_TIAO_8;
                    //huPaiTipsInfos[0].fanAmount = 5;
                    //huPaiTipsInfos[0].zhangAmount = 6;

                    //huPaiTipsInfos[1].faceValue = MahjongFaceValue.MJ_TIAO_2;
                    //huPaiTipsInfos[2].faceValue = MahjongFaceValue.MJ_TIAO_3;
                    //huPaiTipsInfos[3].faceValue = MahjongFaceValue.MJ_WANG_5;


                    //HuPaiTipsInfo[] huPaiTipsInfos2 = new HuPaiTipsInfo[3];

                    //huPaiTipsInfos2[0].faceValue = MahjongFaceValue.MJ_TIAO_8;
                    //huPaiTipsInfos2[0].fanAmount = 5;
                    //huPaiTipsInfos2[0].zhangAmount = 6;

                    //huPaiTipsInfos2[1].faceValue = MahjongFaceValue.MJ_TIAO_2;
                    //huPaiTipsInfos2[2].faceValue = MahjongFaceValue.MJ_WANG_5;


                    //cmdxx.huPaiInfosInHandPai = new List<HuPaiTipsInfo[]>
                    //{
                    //    huPaiTipsInfos,
                    //    huPaiTipsInfos2,
                    //    huPaiTipsInfos,
                    //    huPaiTipsInfos2

                    //};
                    //mjOpCmdList.Append(cmdxx);

                    //MoPai(seatIdx, MahjongFaceValue.MJ_TIAO_5);
                    //uiSwapPaiingTips.SetHintSwapType(SwapPaiDirection.ANTICLOCKWISE);
                    //uiSwapPaiingTips.Show();

                    //swapPaiHintArrowEffect.ShowAnitClockWiseArrow();

                    // HuPaiTipsInfo[] huPaiTipsInfos = new HuPaiTipsInfo[4];

                    // huPaiTipsInfos[0].faceValue = MahjongFaceValue.MJ_TIAO_8;
                    // huPaiTipsInfos[0].fanAmount = 5;
                    // huPaiTipsInfos[0].zhangAmount = 6;

                    //huPaiTipsInfos[1].faceValue = MahjongFaceValue.MJ_TIAO_2;
                    // huPaiTipsInfos[2].faceValue = MahjongFaceValue.MJ_TIAO_3;
                    // huPaiTipsInfos[3].faceValue = MahjongFaceValue.MJ_WANG_5;

                    // uiHuPaiTips.SetHuPaiInfo(huPaiTipsInfos);
                    // uiHuPaiTips.Show();

                    break;

                case "打牌":
                    idx %= fvs.Length;
                    DaPai(seatIdx, PlayerType.FEMALE, 0, 0, fvs[idx], false, actionCombineNum);
                    idx++;
                    break;

                case "高亮桌面麻将":
                    OnDeskMjHighLight(MahjongFaceValue.MJ_TONG_6);
                    break;

                case "关闭高亮桌面麻将":
                    OffDeskMjHighLight(MahjongFaceValue.MJ_TONG_6);
                    break;

                case "插牌":
                    ChaPai(seatIdx, PlayerType.FEMALE, 0, 2, HandPaiType.MoPai, HandPaiAdjustDirection.GoToHandLeftDir);
                    break;

                case "整理牌":
                    SortPai(seatIdx);
                    break;

                case "自摸":
                    HuPai(seatIdx, PlayerType.FEMALE, -1, new Vector3Int(0, mn++, 0), fv, ActionCombineNum.HuPai);
                    break;

                case "胡牌":
                    HuPai(seatIdx, PlayerType.FEMALE, 0, new Vector3Int(0, mn++, 1), fv, ActionCombineNum.HuPai);
                    break;

                case "补花":

                    MahjongBuHuaPaiOpCmd buHuaCmd = (MahjongBuHuaPaiOpCmd)CmdPool.Instance.CreateCmd(MahjongOpCode.BuHuaPai);
                    buHuaCmd.seatIdx = 0;
                    buHuaCmd.handStyle = PlayerType.FEMALE;
                    buHuaCmd.buHuaPaiFaceValue = MahjongFaceValue.MJ_HUA_DONG;
                    mjOpCmdList.Append(buHuaCmd);

                  //  BuHua(seatIdx, PlayerType.FEMALE, fv, ActionCombineNum.HuPai);
                    break;

                case "推倒牌":
                    List<MahjongFaceValue> handpaiList = new List<MahjongFaceValue>
                        {
                            MahjongFaceValue.MJ_FENG_DONG, MahjongFaceValue.MJ_TIAO_9, MahjongFaceValue.MJ_TIAO_8, MahjongFaceValue.MJ_TIAO_7,
                             MahjongFaceValue.MJ_TIAO_6, MahjongFaceValue.MJ_TIAO_5, MahjongFaceValue.MJ_TIAO_4, MahjongFaceValue.MJ_TIAO_3, MahjongFaceValue.MJ_TIAO_2, MahjongFaceValue.MJ_TIAO_1

                        };

                    TuiDaoPai(seatIdx, PlayerType.FEMALE, handpaiList, ActionCombineNum.TuiDaoPai);
                    break;

                case "碰九条":
                    PengChiGangPai(
                        seatIdx, PlayerType.FEMALE, true, 0.05f,
                        new MahjongFaceValue[] { MahjongFaceValue.MJ_TIAO_9, MahjongFaceValue.MJ_TIAO_9, MahjongFaceValue.MJ_TIAO_9, MahjongFaceValue.MJ_TIAO_9 },
                        PengChiGangPaiType.PENG, 0, new Vector3Int(0, mn++, 0), EffectFengRainEtcType.EFFECT_RAIN, ActionCombineNum.PengPai);
                    break;

                case "碰六筒":
                    PengChiGangPai(
                        seatIdx, PlayerType.FEMALE, true, 0.05f,
                        new MahjongFaceValue[] { MahjongFaceValue.MJ_TONG_6, MahjongFaceValue.MJ_TONG_6, MahjongFaceValue.MJ_TONG_6, MahjongFaceValue.MJ_TONG_6 },
                        PengChiGangPaiType.PENG, 0, new Vector3Int(0, mn++, 0), EffectFengRainEtcType.EFFECT_FENG, ActionCombineNum.PengPai);
                    break;

                case "碰后杠六筒":
                    PengChiGangPai(
                        seatIdx, PlayerType.FEMALE, true, 0.05f,
                        new MahjongFaceValue[] { MahjongFaceValue.MJ_TONG_6, MahjongFaceValue.MJ_TONG_6, MahjongFaceValue.MJ_TONG_6, MahjongFaceValue.MJ_TONG_6 },
                        PengChiGangPaiType.BU_GANG, -1, new Vector3Int(0, 0, 0), EffectFengRainEtcType.EFFECT_NONE, ActionCombineNum.PengPai);
                    break
                        ;
                case "Dapai0":
                    DaPai(seatIdx, PlayerType.FEMALE, 0, HandPaiType.HandPai, fv, false, ActionCombineNum.DaPai5);
                    break;

                case "DaPai1":
                    DaPai(seatIdx, PlayerType.FEMALE, 0, HandPaiType.HandPai, fv, false, ActionCombineNum.DaPai1_TaiHand2);
                    break;

                case "DaPai2":
                    DaPai(seatIdx, PlayerType.FEMALE, 0, HandPaiType.HandPai, fv, false, ActionCombineNum.DaPai1_MovPai1_TaiHand1);
                    break;

                case "DaPai3":
                    DaPai(seatIdx, PlayerType.FEMALE, 0, HandPaiType.HandPai, fv, false, ActionCombineNum.DaPai1_MovPai1_TaiHand2);
                    break;

                case "DaPai4":
                    DaPai(seatIdx, PlayerType.FEMALE, 0, HandPaiType.HandPai, fv, false, ActionCombineNum.DaPai1_ZhengPai_TaiHand);
                    break;

                case "DaPai5":
                    DaPai(seatIdx, PlayerType.FEMALE, 0, HandPaiType.HandPai, fv, false, ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand);
                    break;

                case "DaPai8":
                    DaPai(seatIdx, PlayerType.FEMALE, 0, HandPaiType.HandPai, fv, false, ActionCombineNum.DaPai2_MovPai_TaiHand1);
                    break;

                case "DaPai9":
                    DaPai(seatIdx, PlayerType.FEMALE, 0, HandPaiType.HandPai, fv, false, ActionCombineNum.DaPai2_MovPai_TaiHand2);
                    break;

                case "DaPai10":
                    DaPai(seatIdx, PlayerType.FEMALE, 0, HandPaiType.HandPai, fv, false, ActionCombineNum.DaPai3_TaiHand);

                    break;

                case "DaPai11":
                    DaPai(seatIdx, PlayerType.FEMALE, 0, HandPaiType.HandPai, fv, false, ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand);
                    break;

                case "HuPai":
                    HuPai(seatIdx, PlayerType.FEMALE, 0, new Vector3Int(0, mn++, 0), fv, ActionCombineNum.HuPai);
                    break;
            }
        }


    }
}