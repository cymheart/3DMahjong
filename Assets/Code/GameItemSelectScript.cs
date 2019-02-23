using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameItemSelectScript : MonoBehaviour {

    // Use this for initialization
    public GameListScript gameListScript; 
    public void ValueChanged()
    {
        Toggle toggle = GetComponent<Toggle>();

        if(toggle.isOn == false)
        {
            gameListScript.RemoveSelectCount();
        }
        else
        {
            gameListScript.AddSelectCount();
        }
    }
}
