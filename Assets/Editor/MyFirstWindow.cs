using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using System.IO;

//继承自EditorWindow类
public class MyFirstWindow : EditorWindow
{
    string bugReporterName = "";
    string description = "";
    GameObject buggyGameObject;

    //利用构造函数来设置窗口名称
    MyFirstWindow()
    {
        this.titleContent = new GUIContent("Bug Reporter");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("MyTools/Bug Reporter")]
    static void showWindow()
    {
        GetWindow(typeof(MyFirstWindow));
    }
    void OnGUI()
    {
        GUILayout.BeginVertical();

        //绘制标题
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Bug Reporter");

        //绘制文本
        GUILayout.Space(10);
        bugReporterName = EditorGUILayout.TextField("Bug Name", bugReporterName);

        //绘制当前正在编辑的场景
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 12;
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUILayout.Label("Currently Scene:" + EditorSceneManager.GetActiveScene().name);

        //绘制当前时间
        GUILayout.Space(10);
        GUILayout.Label("Time:" + System.DateTime.Now);

        //绘制对象
        GUILayout.Space(10);
        buggyGameObject = (GameObject)EditorGUILayout.ObjectField("Buggy Game Object", buggyGameObject, typeof(GameObject), true);

        //绘制描述文本区域
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Description", GUILayout.MaxWidth(80));
        description = EditorGUILayout.TextArea(description, GUILayout.MaxHeight(75));
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //添加名为"Save Bug"按钮，用于调用SaveBug()函数
        if (GUILayout.Button("Save Bug"))
        {
            SaveBug();
        }

        //添加名为"Save Bug with Screenshot"按钮，用于调用SaveBugWithScreenshot() 函数
        if (GUILayout.Button("Save Bug With Screenshot"))
        {
            SaveBugWithScreenshot();
        }

        GUILayout.EndVertical();
    }

    //用于保存当前信息
    void SaveBug()
    {
        Directory.CreateDirectory("Assets/BugReports/" + bugReporterName);
        StreamWriter sw = new StreamWriter("Assets/BugReports/" + bugReporterName + "/" + bugReporterName + ".txt");
        sw.WriteLine(bugReporterName);
        sw.WriteLine(System.DateTime.Now.ToString());
        sw.WriteLine(EditorSceneManager.GetActiveScene().name);
        sw.WriteLine(description);
        sw.Close();
    }

    void SaveBugWithScreenshot()
    {
        Directory.CreateDirectory("Assets/BugReports/" + bugReporterName);
        StreamWriter sw = new StreamWriter("Assets/BugReports/" + bugReporterName + "/" + bugReporterName + ".txt");
        sw.WriteLine(bugReporterName);
        sw.WriteLine(System.DateTime.Now.ToString());
        sw.WriteLine(EditorSceneManager.GetActiveScene().name);
        sw.WriteLine(description);
        sw.Close();
        ScreenCapture.CaptureScreenshot("Assets/BugReports/" + bugReporterName + "/" + bugReporterName + "Screenshot.png");
    }
}