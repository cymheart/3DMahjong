using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CoreDesgin;
using Task;
using ComponentDesgin;
using ActionDesgin;
using CmdDesgin;

public class MahjongGame : MonoBehaviour
{
    MahjongMachine mjMachine;
    MahjongAssetsMgr mjAssetsMgr;

    //DaZhongMahjongRule daZhongMjRule = null;

    TaskPump taskPump = new TaskPump(1);
    CommonTaskProcesser taskProcesser;

    float lastEscKeyPressTime = -2f;
    bool isStopedGame = false;

    private void Awake()
    {
        //  GC.Collect();
        Application.targetFrameRate = 30;

        mjMachine = new MahjongMachine();

        taskProcesser = new CommonTaskProcesser(taskPump);
        //daZhongMjRule = new DaZhongMahjongRule(mjMachine, taskProcesser);

        MjMachineCmdPool.Instance.CreatePool(mjMachine);
        AppendComponent();
        InstallAction();
        InstallCmd();


    }

    void Start()
    {
        mjMachine.Start();
    }

    /// <summary>
    /// 添加组件到麻将机
    /// </summary>
    void AppendComponent()
    {
        Type[] componentTypes = new Type[]
        {
            typeof(Audio), typeof(Desk), typeof(Fit),
            typeof(Hand), typeof(MahjongAssetsMgr), typeof(MahjongDiceMachine),
            typeof(MahjongPoint), typeof(PreSettingHelper), typeof(Scene),
            typeof(ScreenFit), typeof(UIPCGHTBtnMgr), typeof(UISelectQueMen),
            typeof(UISelectSwapHandPai), typeof(UIScore), typeof(SwapPaiHintArrowEffect),
            typeof(UISwapPaiingTips), typeof(UITouXiang),typeof(MjHuTingCheck),
            typeof(HandActionStates),typeof(HandActionStates),typeof(HandActionStates),typeof(HandActionStates),
        };

        AppendComponent(componentTypes);

    }

    void AppendComponent(Type[] componentTypes)
    {
        for (int i = 0; i < componentTypes.Length; i++)
        {
            mjMachine.AddComponent(componentTypes[i]);
        }
    }

    /// <summary>
    /// 安装动作
    /// </summary>
    void InstallAction()
    {
        MahjongMachineAction.Install(BuHuaAction.Instance, mjMachine);
        MahjongMachineAction.Install(ChaPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(DaPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(FaPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(HighLightMjAction.Instance, mjMachine);
        MahjongMachineAction.Install(HuPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(DiceMachineAction.Instance, mjMachine);
        MahjongMachineAction.Install(MahjongPointerAction.Instance, mjMachine);
        MahjongMachineAction.Install(MoPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(PengChiGangPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(QiDongDiceMachineAction.Instance, mjMachine);
        MahjongMachineAction.Install(RecoverParticlesAction.Instance, mjMachine);
        MahjongMachineAction.Install(SelectDaPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(SelectPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(SelectPCGTHPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(SelectQueMenAction.Instance, mjMachine);
        MahjongMachineAction.Install(SelectSwapPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(SortPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(SwapPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(TuiDaoPaiAction.Instance, mjMachine);
        MahjongMachineAction.Install(XiPaiAction.Instance, mjMachine);
    }

    /// <summary>
    /// 安装命令
    /// </summary>
    void InstallCmd()
    {
        MahjongMachineCmd.Install<MahjongBuHuaPaiOpCmd>(MahjongBuHuaPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<MahjongChaPaiOpCmd>(MahjongChaPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<MahjongDaPaiOpCmd>(MahjongDaPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<FaPaiCmd>(FaPaiCmd.InitCmd);
        MahjongMachineCmd.Install<MahjongHuPaiOpCmd>(MahjongHuPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<MahjongMoPaiOpCmd>(MahjongMoPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<MahjongPcgPaiOpCmd>(MahjongPcgPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<PlayEffectAudioOpCmd>(PlayEffectAudioOpCmd.InitCmd);
        MahjongMachineCmd.Install<QiDongDiceMachineCmd>(QiDongDiceMachineCmd.InitCmd);
        MahjongMachineCmd.Install<QueMenCmd>(QueMenCmd.InitCmd);
        MahjongMachineCmd.Install<ReqSelectDaPaiOpCmd>(ReqSelectDaPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<ReqSelectPCGTHPaiOpCmd>(ReqSelectPCGTHPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<ReqSelectQueMenOpCmd>(ReqSelectQueMenOpCmd.InitCmd);
        MahjongMachineCmd.Install<ReqSelectSwapPaiOpCmd>(ReqSelectSwapPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<ShowScoreCmd>(ShowScoreCmd.InitCmd);
        MahjongMachineCmd.Install<MahjongSortPaiOpCmd>(MahjongSortPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<MahjongSwapPaiCmd>(MahjongSwapPaiCmd.InitCmd);
        MahjongMachineCmd.Install<ShowSwapPaiHintCmd>(ShowSwapPaiHintCmd.InitCmd);
        MahjongMachineCmd.Install<MahjongTuiDaoPaiOpCmd>(MahjongTuiDaoPaiOpCmd.InitCmd);
        MahjongMachineCmd.Install<TurnToNextPlayerOpCmd>(TurnToNextPlayerOpCmd.InitCmd);
        MahjongMachineCmd.Install<XiPaiCmd>(XiPaiCmd.InitCmd);
        MahjongMachineCmd.Install<HideSwapPaiUICmd>(HideSwapPaiUICmd.InitCmd);
    }

    void Update()
    {
      //  taskPump.Run();
        MonitorQuitGame();

        mjMachine.mjMachineUpdater.Update();
    }

    public void MonitorQuitGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.time - lastEscKeyPressTime < 0.4f)
            {
                isStopedGame = true;

                mjMachine.Destory();
                mjMachine = null;
               // daZhongMjRule.Stop();

                Application.Quit();
            }
            else
            {
                lastEscKeyPressTime = Time.time;
            }
        }
    }

    void OnDestroy()
    {
        mjMachine.Destory();
        //daZhongMjRule.Stop();
    }
}