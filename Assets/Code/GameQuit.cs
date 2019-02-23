using UnityEngine;
using System.Collections;
public class GameQuit : MonoBehaviour
{
    private int mPressTimes = 0;
    // Use this for initialization  
    void Start()
    {
        //Ensure that there is only one gameQuit in the Scene，即使加载了下个场景Scene  
        GameObject[] gameQuits = GameObject.FindGameObjectsWithTag("GameQuit");

        if (gameQuits.Length == 2)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame  
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {//KeyCode.Escape表示键盘ESC,手机的返回键  
            mPressTimes++;
            StartCoroutine("ResetMPressTimes", 1.0f);//若过了1秒都没有按第2次则重置mPressTimes  
            if (mPressTimes == 2)
            {
                Application.Quit();
            }
        }
    }

    IEnumerator ResetMPressTimes(float sec)
    {
        yield return new WaitForSeconds(sec);
        mPressTimes = 0;
    }
}