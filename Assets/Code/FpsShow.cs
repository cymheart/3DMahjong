using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsShow : MonoBehaviour
{
    public float fpsMeasuringDelta = 2.0f;

    private float timePassed;
    private int m_FrameCount = 0;
    private float m_FPS = 0.0f;
    GUIStyle bb = new GUIStyle();
    Color color = new Color(1.0f, 0.5f, 0.0f);
    Rect rect;
    private void Start()
    {
       // Application.targetFrameRate = 60;

        timePassed = 0.0f;
        bb.normal.background = null;    //这是设置背景填充的
        bb.normal.textColor = color;
        bb.fontSize = 40;       //当然，这是字体大小
        rect = new Rect((Screen.width / 2) - 40, 0, 200, 200);
    }

    private void Update()
    {
        m_FrameCount = m_FrameCount + 1;
        timePassed = timePassed + Time.deltaTime;

        if (timePassed > fpsMeasuringDelta)
        {
            m_FPS = m_FrameCount / timePassed;

            timePassed = 0.0f;
            m_FrameCount = 0;
        }
    }

    private void OnGUI()
    {
        //居中显示FPS
        GUI.Label(rect , m_FPS.ToString(), bb);
    }
}