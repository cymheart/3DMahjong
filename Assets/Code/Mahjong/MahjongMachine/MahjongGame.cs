using Assets;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MahjongMachineNS;
using Task;

public class MahjongGame : MonoBehaviour {

   // AppDomain appdomain;
    object obj;
    MahjongMachine mjMachine;
    DaZhongMahjongRule daZhongMjRule = null;

    TaskPump taskPump = new TaskPump(1);
    CommonTaskProcesser taskProcesser;

    static public MahjongAssets mjAssets;

    float lastEscKeyPressTime = -2f;
    bool isStopedGame  = false;

    private void Awake()
    {
        //  GC.Collect();
        Application.targetFrameRate = 30;
    
        mjAssets = new MahjongAssets();
        mjAssets.Load(this, LoadMjResCompleted);
    }
    void Start ()
    {


    }

    void LoadMjResCompleted()
    {
        mjMachine = new MahjongMachine();
        mjMachine.Start(this);

       taskProcesser = new CommonTaskProcesser(taskPump);
       daZhongMjRule = new DaZhongMahjongRule(mjMachine, taskProcesser);    
    }

    void Update ()
    {
        taskPump.Run();

        if (mjMachine != null)
            mjMachine.UpdateGame();

        MonitorQuitGame();
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
                daZhongMjRule.Stop();
     
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
        daZhongMjRule.Stop();
    }
}
