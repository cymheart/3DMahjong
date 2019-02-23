using Assets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
public class GameListScript : MonoBehaviour {

    // Use this for initialization
    public GameObject goScrollView;
    public GameObject goContent;
    int selectCount = 0;

    Button btnOk;


    // IEnumerator Start()
    void Start()
    {
        //
        btnOk = transform.Find("BtnOk").GetComponent<Button>();
        EventTriggerListener.Get(btnOk.gameObject).onClick = OnButtonClick;

        List<GameObject> itemList = ResPool.app.selectGameItems;

        foreach (var item in itemList)
        {
            item.GetComponent<GameItemSelectScript>().gameListScript = this;
            item.transform.SetParent(goContent.transform, false);
        }
    }


    private void OnButtonClick(GameObject go)
    {
        string name;

        List<string> nameList = new List<string>();

        foreach (Transform child in goContent.transform)
        {
            Toggle toggle = child.GetComponent<Toggle>();

            if (toggle.isOn == true)
            {

                Transform img = child.Find("Image");
                name = img.GetComponent<Image>().sprite.name.Replace("(Clone)","");
                nameList.Add(name);
            }
        }


        App app = ResPool.app;
        app.uiMain.ChangeState_2_to_0(nameList.ToArray());
    }


    public void AddSelectCount()
    {
        selectCount++;
        if (selectCount == 3)
            GreyItems();
    }

    public void RemoveSelectCount()
    {
        selectCount--;

        if (selectCount == 2)
            NormalItems();
    }

    void GreyItems()
    {
      //  Transform[] grandFa;
      //  grandFa = goContent.GetComponentsInChildren<Transform>();
       // Material material = new Material(Shader.Find("UI/Default Grey"));

        Material material = Resources.Load<Material>("Grey");


        foreach (Transform child in goContent.transform)
        {
            Toggle toggle = child.GetComponent<Toggle>();

            if (toggle.isOn == false){
                Transform img = child.Find("Image");
                img.GetComponent<Image>().material = material;
            }
            else
            {
                Transform img = child.Find("Image");
                img.GetComponent<Image>().material = null;
            }
        } 
    }

    void NormalItems()
    {
        foreach (Transform child in goContent.transform)
        {
            Transform img = child.Find("Image");
            img.GetComponent<Image>().material = null;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
