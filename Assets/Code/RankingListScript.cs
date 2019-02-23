using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingListScript : MonoBehaviour {

    // Use this for initialization
    public GameObject goScrollView;
    public GameObject goContent;
    public GameObject prefab;
    public GameObject upItem;
    RectTransform prefabRectTransform;

    List<GameObject> itemList = new List<GameObject>();
    int itemCount = 100;
    float scroRect_MinHight;
    float scroRect_MaxHight;


    void Start () {

        //  var pPrefab  = Resources.Load("Assets/Button (1)", typeof(GameObject)) as GameObject;
        prefabRectTransform = prefab.GetComponent<RectTransform>();

        RectTransform goScrollViewRectTransform = goScrollView.GetComponent<RectTransform>();


        for (int i = 0; i < itemCount; i++)
        {
            var item = Instantiate(prefab);//示例化  
            itemList.Add(item);
         //   item.transform.parent = goContent.transform;
           item.transform.SetParent(goContent.transform, false);

          //  item.gameObject.SetActive(false);
        }

        float height = prefabRectTransform.rect.height * itemCount;

        RectTransform goContentRectTransform = goContent.GetComponent<RectTransform>();
        goContentRectTransform.sizeDelta = new Vector2(prefabRectTransform.rect.width - goScrollViewRectTransform.rect.width, height);

        scroRect_MinHight = goScrollViewRectTransform.gameObject.GetComponent<Transform>().position.y * 100 - goScrollViewRectTransform.rect.height / 2;
        scroRect_MaxHight = goScrollViewRectTransform.gameObject.GetComponent<Transform>().position.y * 100 + goScrollViewRectTransform.rect.height /2;

      

        StartCoroutine(IEShowInit());
    }

    public void StasrtCorout()
    {
        StartCoroutine(IEShowInit());
    }


    //2.拖动完毕委托事件   
    public void VCValueChanged()
    {
        goContent.GetComponent<VerticalLayoutGroup>().enabled = false;
        itemList.ForEach((p) => { CheckPos(p.transform); });

      //  goContent.GetComponent<VerticalLayoutGroup>().enabled = true;
    }

    //3.检测 在scrollrect 视图内  
    void CheckPos(Transform obj)
    {
        var pos = obj.position;
        var go = obj.gameObject;

        float ymin = pos.y*100 - prefabRectTransform.rect.height / 2;
        float ymax = pos.y*100 + prefabRectTransform.rect.height / 2;

        if ((ymin >= scroRect_MinHight && ymin <= scroRect_MaxHight) || 
            (ymax >= scroRect_MinHight && ymax <= scroRect_MaxHight))
            ShowItem(go);
        else
            HideItem(go);
    }
    //4.隐藏或显示  
    void ShowItem(GameObject go)
    {
        go.transform.GetComponent<Image>().enabled = true;
        go.transform.Find("Image").GetComponent<Image>().enabled = true;
        go.transform.Find("Image (1)").GetComponent<Image>().enabled = true;
        go.transform.Find("Image (2)").GetComponent<Image>().enabled = true;
    }
    void HideItem(GameObject go)
    {
        go.transform.GetComponent<Image>().enabled = false;
        go.transform.Find("Image").GetComponent<Image>().enabled = false;
        go.transform.Find("Image (1)").GetComponent<Image>().enabled = false;
        go.transform.Find("Image (2)").GetComponent<Image>().enabled = false;
    }

    IEnumerator IEShowInit()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            VCValueChanged();
        }
    }
    // Update is called once per frame
    void Update ()
    {
        //for (int i = 0; i < itemCount; i++)
        //{
        //    itemList[i].transform.SetParent(null, false);
        //}

        //for (int i = 0; i < itemCount; i++)
        //{
        //    itemList[i].transform.SetParent(goContent.transform, false);
        //}



        //RectTransform goScrollViewRectTransform = goScrollView.GetComponent<RectTransform>();


        //RectTransform goContentRectTransform = goContent.GetComponent<RectTransform>();
        //float y = goContentRectTransform.localPosition.y;





        //float len = 0;
        //int idx = (int)Mathf.Floor(y / prefabRectTransform.sizeDelta.y);

        //for (int i = 0; i < idx; i++)
        //{
        //    len += itemList[i].GetComponent<RectTransform>().sizeDelta.y;
        //  //  itemList[i].gameObject.SetActive(false);
        //}


        //if (len > 0)
        //{
        //    upItem.GetComponent<RectTransform>().sizeDelta = new Vector2(prefabRectTransform.rect.width, len);
        //   // upItem.gameObject.SetActive(true);
        //}
        //else
        //{
        //   // upItem.gameObject.SetActive(false);
        //}


        //float height = y - len + goScrollViewRectTransform.sizeDelta.y;

        //int count = (int)Mathf.Ceil(height / prefabRectTransform.sizeDelta.y);
        //int idxEnd = Mathf.Min(itemCount - 1, idx + count);


        //for (int i = idx; i <= idxEnd; i++)
        //{
        //   // itemList[i].gameObject.SetActive(true);
        //    itemList[i].GetComponent<CanvasGroup>().alpha = 1;
        //}

        //for (int i = idxEnd + 1; i < itemCount; i++)
        //{
        //    itemList[i].GetComponent<CanvasGroup>().alpha = 0;

        //   // itemList[i].gameObject.SetActive(false);
        //}



    }
}
