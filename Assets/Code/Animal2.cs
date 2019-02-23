using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Animal2 : MonoBehaviour {

    TPAtlas tpAtlas2;
    int state = 0;
    float startTime;
    float curtTime;
    float nextStartTime;
    public float delayPerUnit = 0.2f;
    public float nextStartDelay = 0f;


    List<TPFrameData> frameDataList;
    Vector3 orgPosition;

    float cameraHeight;
    float cameraWidth;

    Image img;
    RectTransform rectTransform;
    float sx;
    float sy;
    void Start()
    {
        tpAtlas2 = ResPool.app.tpAtlas2;

        //摄像机的尺寸
        float orthographicSize = Camera.main.orthographicSize;

        //宽高比
        float aspectRatio = Camera.main.aspect;

        //摄像机的单位高度
        cameraHeight = orthographicSize * 2;

        //摄像机的单位宽度
        cameraWidth = cameraHeight * aspectRatio;

     

        int sw = Screen.width;
        int sh = Screen.height;

       // tpAtlas = new TPAtlas();
        //tpAtlas.CreateSpriteFrame(plist);

        frameDataList = tpAtlas2.sheets;

        // string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        //  Debug.LogWarning("#PLisToSprites start:" + selectionPath);
        // string fileContent = string.Empty;

        //spriterenderer = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        sx = rectTransform.localScale.x;
        sy = rectTransform.localScale.y;



        //sx *= rectTransform.sizeDelta.x / w;
        //sy *= rectTransform.sizeDelta.y / h;

        //  orgPosition = new Vector3(spriterenderer.transform.position.x, spriterenderer.transform.position.y);
        orgPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y);

    }

    public void Scale(float xscale, float yscale)
    {
        rectTransform.localScale = new Vector3(xscale, yscale, 1);
        sx = rectTransform.localScale.x;
        sy = rectTransform.localScale.y;
    }

    //void Update()
    //{
    //    switch(state)
    //    {
    //        case 0:
    //            curtTime = startTime = Time.time;
    //            state = 1;
    //            break;

    //        case 1:
    //            curtTime = Time.time;
    //            break;
    //    }


    //    float tm = curtTime - startTime;
    //    int pos = (int)(tm / delayPerUnit);
    //    int index = pos % frameDataList.Count;

    //    TPFrameData framData = frameDataList[index];

    //    float sx = spriterenderer.transform.localScale.x;
    //    float sy = spriterenderer.transform.localScale.y;

    //    float ox = framData.offset.x  / framData.sprite.pixelsPerUnit * sx;
    //    float oy = framData.offset.y / framData.sprite.pixelsPerUnit * sy;

    //    Vector3 newPos = new Vector3(orgPosition.x + ox, orgPosition.y + oy, orgPosition.z);
    //    spriterenderer.transform.position = newPos;
    //    spriterenderer.sprite = framData.sprite;

    //    img.transform.position = newPos;
    //    img.sprite = framData.sprite;


    //}


    void Update()
    {
        switch (state)
        {
            case 0:
                curtTime = startTime = Time.time;
                state = 1;
                break;

            case 1:
                curtTime = Time.time;
                break;

            case 2:
                curtTime = Time.time;
                if (curtTime < nextStartTime)
                    return;
                state = 1;
                break;
        }

        float tm = curtTime - startTime;
        int pos = (int)(tm / delayPerUnit);
        int index = pos % frameDataList.Count;

        if(index == frameDataList.Count - 1)
        {
            state = 2;
            nextStartTime = curtTime + nextStartDelay;
        }


        TPFrameData framData = frameDataList[index];

        //float sx = rectTransform.sizeDelta.x / framData.frame.width;
        //float sy = rectTransform.sizeDelta.y / framData.frame.height;

   
        float ox = framData.offset.x / 1 * sx;
        float oy = framData.offset.y / 1 * sy;


        Vector3 a = new Vector3();
        float angle;
        rectTransform.localRotation.ToAngleAxis(out angle, out a);

        Vector3 mx = new Vector3(ox, 0, 0);
        mx = Quaternion.AngleAxis(angle, a) * mx;

        Vector3 my = new Vector3(0, oy, 0);
        my = Quaternion.AngleAxis(angle, a) * my;

        Vector3 m = mx + my;
        Vector3 newPos = new Vector3(orgPosition.x + m.x, orgPosition.y + m.y, orgPosition.z);
        rectTransform.localPosition = newPos;
        img.sprite = framData.sprite;
        rectTransform.sizeDelta = new Vector2(framData.frame.width, framData.frame.height);
    }
}
