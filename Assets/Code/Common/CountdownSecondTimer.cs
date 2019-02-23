using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CountdownSecondTimer
{
    int limitTime = 20;
    int curtTime = 20;
    float startTimingTime = 0;
    public void SetLimitTime(int tm)
    {
        tm = Mathf.Max(tm, 0);
        tm = Mathf.Min(tm, 99);
        limitTime = tm;
    }

    public void SetCurtTime(int tm)
    {
        curtTime = tm;
    }

    public void StartTime()
    {
        startTimingTime = Time.time;
        curtTime = limitTime;
    }

    public int[] GetGetCurtTimeNums()
    {
        int num1 = curtTime / 10;
        int num2 = curtTime - num1 * 10;
        return new int[] { num1, num2 };
    }

    public int GetCurtTime()
    {
        return curtTime;
    }

    public int Timing()
    {
        int tm = (int)Mathf.Floor(Time.time - startTimingTime);

        if (limitTime - tm < 0)
        {
            curtTime = 0;
            return 0;
        }

        curtTime = limitTime - tm;
        return curtTime;
    }

}
