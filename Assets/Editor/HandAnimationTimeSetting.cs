using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using System.IO;
using MahjongMachineNS;
//继承自EditorWindow类
public class HandAnimationTimeSetting : EditorWindow
{
    int seatIndex = 0;
    int handTypeIndex = 0;
    int handDirIndex = 0;
    GameObject root;
    Dictionary<string, AnimationClip> animClipDict = new Dictionary<string, AnimationClip>();
    static Dictionary<string, Dictionary<string, ActionDataInfo>> handActionDataInfoDicts = null;
    static bool isReadFile = true;
    string[] exActionNames = { "ChaPai_MovHand", "MoveHandOutScreen", "MoveHandToDstPos" };
    MahjongAssets mjAssets;

    //利用构造函数来设置窗口名称
    HandAnimationTimeSetting()
    {
        this.titleContent = new GUIContent("手部动作时间设置  ");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("MyTools/手部动作时间设置")]
    static void showWindow()
    {
        ParseHandActionSpeedData();
        GetWindow(typeof(HandAnimationTimeSetting)); 
    }
    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("座位号", GUILayout.MaxWidth(80));
        seatIndex = EditorGUILayout.Popup(seatIndex, new string[] { "0","1","2","3"});

        GUILayout.Label("手类型", GUILayout.MaxWidth(80));
        handTypeIndex = EditorGUILayout.Popup(handTypeIndex, new string[] { "男", "女"});

        GUILayout.Label("左右手", GUILayout.MaxWidth(80));
        handDirIndex = EditorGUILayout.Popup(handDirIndex, new string[] { "左手", "右手" });
        GUILayout.EndHorizontal();

        string name;

        if (handTypeIndex == 0)
            name = "Male";
        else
            name = "Female";

        if (handDirIndex == 0)
            name += "LeftHand";
        else
            name += "RightHand";

        name += seatIndex;
        root = GameObject.Find("Root");

        if (root == null)
        {
            GUILayout.EndVertical();
            return;
        }

        if (handActionDataInfoDicts == null)
        {
            mjAssets = MahjongGame.mjAssets;

            if(mjAssets != null)
                handActionDataInfoDicts = mjAssets.handActionDataInfoDicts;
        }

        Transform tf = root.transform.Find(name);

        if(tf == null)
        {
            GUILayout.EndVertical();
            return;
        }

        Animation anim = tf.GetComponent<Animation>();
        Dictionary<string, ActionDataInfo> actionDataDict = null;

        if (anim != null)
        {
            if (handActionDataInfoDicts == null)
                ParseHandActionSpeedData();

            actionDataDict = handActionDataInfoDicts[name];
            ActionDataInfo actionData;

            SetHandAnimationInfo(anim);
            int count = animClipDict.Count + exActionNames.Length;

            GUILayout.Label("动作数量:" + count, GUILayout.MaxWidth(200));

            foreach(var item in animClipDict)
            {
                AnimationClip clip = item.Value;
                GUILayout.BeginHorizontal();
                GUILayout.Label(clip.name, GUILayout.MaxWidth(270));
                anim[clip.name].speed = EditorGUILayout.Slider(anim[clip.name].speed, 0.1f, 7f);


                if (actionDataDict.ContainsKey(clip.name))
                {
                    actionData = actionDataDict[clip.name];
                }
                else
                {
                    actionData = new ActionDataInfo();
                    actionData.crossFadeNormalTime = 1f;
                }

                actionData.crossFadeNormalTime = EditorGUILayout.Slider(actionData.crossFadeNormalTime, 0f, 1f);
                actionDataDict[clip.name] = actionData;

                GUILayout.EndHorizontal();

            }

            for (int i = 0; i < exActionNames.Length; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(exActionNames[i], GUILayout.MaxWidth(270));

                if (actionDataDict.ContainsKey(exActionNames[i]))
                {
                    actionData = actionDataDict[exActionNames[i]];
                }
                else
                {
                    actionData = new ActionDataInfo();
                    actionData.crossFadeNormalTime = 1f;
                    actionDataDict[exActionNames[i]] = actionData;
                }

                actionData.speed = EditorGUILayout.Slider(actionData.speed, 0.1f, 7f);
                actionData.crossFadeNormalTime = EditorGUILayout.Slider(actionData.crossFadeNormalTime, 0f, 1f);
                actionDataDict[exActionNames[i]] = actionData;
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.EndVertical();
            return;
        }

        GUILayout.Space(50);

        if (GUILayout.Button("保存动作到文件"))
        {
            SaveAllAction();
        }


        GUILayout.Space(200);

      
        if (GUILayout.Button("复制动作到其它手势"))
        {
            CopyActionToOtherHand(anim, actionDataDict, name);
        }

        GUILayout.EndVertical();
    }

    //选中状态下的初始化
    private void SetHandAnimationInfo(Animation anim)
    {
        if (anim == null)
            return;

        try
        {
            foreach (AnimationState state in anim)
            {
                if(!animClipDict.ContainsKey(state.name))
                    animClipDict.Add(state.name, anim.GetClip(state.name));
            }
        }
        catch
        {

        }
    }

    void CopyActionToOtherHand(Animation orgClipAnim, Dictionary<string, ActionDataInfo> orgActionDataInfoDict, string orgHandName)
    {
        string[] handNames = { "FemaleLeftHand", "FemaleRightHand", "MaleLeftHand", "MaleRightHand" };
        string name;

        root = GameObject.Find("Root");
        Animation anim;
        Transform tf;
        Dictionary<string, ActionDataInfo> curtActionDataInfoDict = null;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < handNames.Length; j++)
            {
                name = handNames[j] + i;
                if (name == orgHandName)
                    continue;

                curtActionDataInfoDict = handActionDataInfoDicts[name];

                tf = root.transform.Find(name);
                anim = tf.GetComponent<Animation>();

                foreach (AnimationState state in anim)
                {
                    if (curtActionDataInfoDict.ContainsKey(state.name))
                    {
                        anim[state.name].speed = orgClipAnim[state.name].speed;
                        curtActionDataInfoDict[state.name] = orgActionDataInfoDict[state.name];
                    }
                }

                for (int k = 0; k < exActionNames.Length; k++)
                {
                    if (curtActionDataInfoDict.ContainsKey(exActionNames[k]))
                    {
                        curtActionDataInfoDict[exActionNames[k]] = orgActionDataInfoDict[exActionNames[k]];
                    }           
                }
            }
        }

    }

    //用于保存当前信息
    void SaveAllAction()
    {
        string[] handNames = { "FemaleLeftHand", "FemaleRightHand", "MaleLeftHand", "MaleRightHand" };
        Directory.CreateDirectory("Assets/Resources/");
        StreamWriter sw = new StreamWriter("Assets/Resources/" + "/" + "HandActionSpeed.txt", false);
        string name;

        root = GameObject.Find("Root");
        Animation anim;
        Transform tf;
        string line;
        Dictionary<string, ActionDataInfo> curtActionDataInfoDict = null;

        for (int i=0; i < 4; i++)
        {
            for(int j = 0; j< handNames.Length; j++)
            {
                name = handNames[j] + i;
                curtActionDataInfoDict = handActionDataInfoDicts[name];

                tf = root.transform.Find(name);
                anim = tf.GetComponent<Animation>();

                sw.WriteLine("=======================================");
                int count = anim.GetClipCount() + exActionNames.Length;
                line = name + " " + count;
                sw.WriteLine(line);

                foreach (AnimationState state in anim)
                {
                    if(curtActionDataInfoDict.ContainsKey(state.name))
                    {
                        line = state.name + " " + anim[state.name].speed + " " + curtActionDataInfoDict[state.name].crossFadeNormalTime;
                    }
                    else
                    {
                        line = state.name + " " + anim[state.name].speed + " 1";
                    }

                    sw.WriteLine(line);
                }

                for (int k = 0; k < exActionNames.Length; k++)
                {
                    if (curtActionDataInfoDict.ContainsKey(exActionNames[k]))
                    {
                        line = exActionNames[k] + " " + curtActionDataInfoDict[exActionNames[k]].speed + " " + curtActionDataInfoDict[exActionNames[k]].crossFadeNormalTime;
                    }
                    else
                    {
                        line = exActionNames[k] + " " + 0.6 + " 1";
                    }

                    sw.WriteLine(line);
                }
            }
        }

        sw.Close();
    }

    static void ParseHandActionSpeedData()
    {
        handActionDataInfoDicts = new Dictionary<string, Dictionary<string, ActionDataInfo>>();

        string[] handNames = { "FemaleLeftHand", "FemaleRightHand", "MaleLeftHand", "MaleRightHand" };
        string name;
        Dictionary<string, ActionDataInfo> curtActionDataInfoDict = null;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < handNames.Length; j++)
            {
                name = handNames[j] + i;
                curtActionDataInfoDict = new Dictionary<string, ActionDataInfo>();
                handActionDataInfoDicts[name] = curtActionDataInfoDict;
            }
        }

        if (isReadFile == false)
            return;

        using (StreamReader sr = new StreamReader("Assets/Resources/" + "/" + "HandActionSpeed.txt"))
        {
            string line;
            int lineIndex = 0;
            int actionCount = 0;
            string[] datas;
            ActionDataInfo actionDataInfo;

            do
            {
                line = sr.ReadLine();

                if (line == null)
                    break;

                if (line[0] == '=')
                    continue;

                if (lineIndex == 0)
                {
                    datas = line.Split(' ');
                    curtActionDataInfoDict = handActionDataInfoDicts[datas[0]];
                    actionCount = int.Parse(datas[1]);
                    lineIndex = 1;
                }
                else
                {
                    if (lineIndex <= actionCount)
                    {
                        datas = line.Split(' ');
                        actionDataInfo = new ActionDataInfo();
                        actionDataInfo.speed = float.Parse(datas[1]);
                        actionDataInfo.crossFadeNormalTime = float.Parse(datas[2]);
                        curtActionDataInfoDict[datas[0]] = actionDataInfo;
                        lineIndex++;
                    }

                    if (lineIndex > actionCount)
                    {
                        lineIndex = 0;
                    }
                }
            } while (true);
        }
    }
}