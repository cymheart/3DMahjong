using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetUpdate : MonoBehaviour
{
    // Use this for initialization
    void Awake()
    {
        //Ensure that there is only one gameQuit in the Scene，即使加载了下个场景Scene  
        GameObject[] netUpdate = GameObject.FindGameObjectsWithTag("NetUpdate");

        if (netUpdate.Length == 2)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
