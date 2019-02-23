using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnDownUpScaleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        EventTriggerListener.Get(gameObject).onDown = OnButtonDown;
        EventTriggerListener.Get(gameObject).onUp = OnButtonUp;
    }

    // Update is called once per frame
    private void OnButtonDown(GameObject go)
    {
        //在这里监听按钮的点击事件
        Animal animal = transform.Find("Image2").GetComponent<Animal>();
        animal.Scale(0.95f, 0.95f);
    }

    private void OnButtonUp(GameObject go)
    {
        Animal animal = transform.Find("Image2").GetComponent<Animal>();
        animal.Scale(1f, 1f);
    }
}
