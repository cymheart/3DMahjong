using UnityEngine;

public class Common
{
    /// <summary>
    /// 获取随机数
    /// </summary>
    /// <param name="count"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="canEqual"></param>
    /// <returns></returns>
    static public int[] GetRandom(int count, int min, int max, bool canEqual = false)
    {
        int[] nums = new int[count];
        int num;

        if (canEqual || count > max)
        {
            for (int i = 0; i < count; i++)
            {
                nums[i] = Random.Range(min, max);
            }

            return nums;
        }


        for (int n = 0; n < count; n++)
        {
            num = Random.Range(min, max);

            for (int i = 0; i < n; i++)
            {
                if (nums[i] == num)
                {
                    num = Random.Range(min, max);
                    i = -1;
                }
            }

            nums[n] = num;
        }

        return nums;
    }


    static public int IndexOf(int[] idxs, int idx)
    {
        if (idxs == null)
            return -1;

        for (int i = 0; i < idxs.Length; i++)
        {
            if (idxs[i] == idx)
                return i;
        }

        return -1;
    }


    static public float Mod(float a, float b)
    {
        int m = (int)(a * 1000f);
        int n = (int)(b * 1000f);

        float tm = m % n;
        tm *= 0.001f;
        return tm;
    }

}

