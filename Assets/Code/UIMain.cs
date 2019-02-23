using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using Assets;
using UnityEngine.U2D;
using System.Collections.Generic;
using Net;

public class UIMain : MonoBehaviour
{
    Button[] btnSelectGame = new Button[3];
    Button btnReturn;
    Button btnPull;
    Transform panelFreeze;

    public GameObject selectGameContent;

    
    List<Vector2> selectItemOffsetList = new List<Vector2>();
    int itemIdx = 0;
    int state = 0;

    int state01_animal_Count = 0;
    int state10_animal_Count = 0;
    int state02_animal_Count = 0;
    int state20_animal_Count = 0;

    bool isStartSelectGameBtnAnim = false;

    App app;
   
    void Start()
    {
        app = ResPool.app;
        app.uiMain = this;

        CreateSelectGamesButton();
     
        btnReturn = transform.Find("BtnReturn").GetComponent<Button>();
        EventTriggerListener.Get(btnReturn.gameObject).onClick = OnButtonClick;

        btnPull = transform.Find("BtnPull").GetComponent<Button>();
        EventTriggerListener.Get(btnPull.gameObject).onClick = OnButtonClick;

        panelFreeze = transform.Find("PanelFreeze");
        EventTriggerListener.Get(panelFreeze.gameObject).onClick = OnButtonClick;

    }

    float GetScreenScale()
    {
        float scale = (1920.0f / Screen.width) / (1080.0f / Screen.height);
        return scale;
    }


    void CreateSelectGamesButton(int type = 0)
    {
        float ybase = 299;
        float ypos;
        Vector2 sizeDelta;
        Vector3 scale;
        Vector3 postion;

        for (int i = 0; i < 3; i++)
        {
            btnSelectGame[i] = app.curtGames[i].btnMahjong.GetComponent<Button>();
            sizeDelta = btnSelectGame[i].GetComponent<RectTransform>().sizeDelta;
            scale = btnSelectGame[i].GetComponent<RectTransform>().localScale;
            postion = btnSelectGame[i].GetComponent<RectTransform>().localPosition;

            ypos = ybase - i * sizeDelta.y * scale.y;

            if (type == 0)
            {
                btnSelectGame[i].GetComponent<RectTransform>().localPosition = new Vector3(postion.x, ypos, 0);
            }
            else
            {
                float sw = transform.GetComponent<RectTransform>().sizeDelta.x;
                btnSelectGame[i].GetComponent<RectTransform>().localPosition = new Vector3(sw/2 + sizeDelta.x/ 2 * scale.x, ypos, 0);
            }

            app.curtGames[i].btnMahjong.GetComponent<Transform>().SetParent(transform, false);
            Transform selectGameList = transform.Find("ImgSelectGameList");
            app.curtGames[i].btnMahjong.GetComponent<Transform>().SetSiblingIndex(selectGameList.GetSiblingIndex());
            EventTriggerListener.Get(btnSelectGame[i].gameObject).onClick = OnButtonClick;
        }
    }

    private void OnButtonClick(GameObject go)
    {
  
        if (btnSelectGame[0] != null &&
            go == btnSelectGame[0].gameObject)
        {
            ChangeState_0_to_1(0);
        }
        else if(btnSelectGame[1] != null && 
            go == btnSelectGame[1].gameObject)
        {
            ChangeState_0_to_1(1);
        }
        else if(btnSelectGame[2] != null && 
            go == btnSelectGame[2].gameObject)
        {
            ChangeState_0_to_1(2);
        }
        else if (go == btnReturn.gameObject)
        {
            ChangeState_1_to_0();
        }
        else if (go == btnPull.gameObject)
        {
            ChangeState_0_to_2();
        }
        else if (go == panelFreeze.gameObject)
        {
            ChangeState_2_to_0();
        }
    }


    void ChangeState_0_to_1(int gameBtnIdx)
    {
        if (state != 0 || state == 1)
            return;

        float s = GetScreenScale();

        Transform waiterTransform = transform.Find("ImgWaiter");
        Transform rankingTransform = transform.Find("RankingListView");
        CanvasGroup rankingCanvasGroup = rankingTransform.GetComponent<CanvasGroup>();
        Transform funcPanelBgTransform = transform.Find("ImgFuncItemBg");
        Transform funcTransform = transform.Find("PanelFuncItems");
        Transform playerHeadTransform = transform.Find("PanelPlayerIcon");
        Transform btnRetTransform = transform.Find("BtnReturn");
        Transform btnPullTransform = transform.Find("BtnPull");
        Transform selectGameTransform = transform.Find("PanelSelectGame");

        Image btnPullImg = btnPullTransform.GetComponent<Image>();
        state01_animal_Count++;
        btnPullImg.DOColor(new Color(1, 1, 1, 0), 0.15f).onComplete = delegate ()
        {
            state01_animal_Count--;
            if (state01_animal_Count == 0)
                state = 1;
        };



        var tweener = waiterTransform.DOMoveX(-6.0f/s, 0.2f).SetEase(Ease.Flash).SetRelative();
        state01_animal_Count++;

        tweener.onComplete = delegate ()
        {
            state01_animal_Count--;
            if (state01_animal_Count == 0)
                state = 1;
        };

        //
        RectTransform playerHeadeRectTf = playerHeadTransform.GetComponent<RectTransform>();
        Vector3 newPos = new Vector3(playerHeadeRectTf.localPosition.x + 120, playerHeadeRectTf.localPosition.y, playerHeadeRectTf.localPosition.z);
        playerHeadeRectTf.localPosition = newPos;


       // rankingTransform.gameObject.SetActive(false);
        rankingCanvasGroup.alpha = 0;

        funcPanelBgTransform.gameObject.SetActive(false);
        funcTransform.gameObject.SetActive(false);

        btnRetTransform.gameObject.SetActive(true);


        //

        for (int i = 0; i < 3; i++)
        {
            if (btnSelectGame[i] == null)
                continue;

            CanvasGroup btnCanvasGroup = btnSelectGame[i].GetComponent<CanvasGroup>();
            tweener = btnCanvasGroup.DOFade(0, 0.5f);
            state01_animal_Count++;
            tweener.onComplete = delegate ()
            {
                state01_animal_Count--;
                if (state01_animal_Count == 0)
                    state = 1;
            };
        }

        //
        SelectGameItemsAnimal(gameBtnIdx);

        //
        Transform titleTransform = selectGameTransform.Find("ImageTitle");
        titleTransform.GetComponent<Image>().sprite = app.curtGames[gameBtnIdx].select_room_title;

        selectGameTransform.gameObject.SetActive(true);
        tweener = selectGameTransform.DOMoveX(3.06f/s, 0.2f).SetEase(Ease.Flash);
        state01_animal_Count++;

        tweener.onComplete = delegate ()
        {
            state01_animal_Count--;
            if (state01_animal_Count == 0)
                state = 1;
        };
    }

    void SelectGameItemsAnimal(int gameBtnIdx)
    {
        selectItemOffsetList.Clear();
        float s = GetScreenScale();

        List<Transform> tfList = new List<Transform>();
       
        foreach (Transform go in selectGameContent.transform)
        {
            tfList.Add(go);
        }

        foreach(var go in tfList)
            go.SetParent(null, false);


        Transform selectGameTransform = transform.Find("PanelSelectGame");
    
        app.curtGames[gameBtnIdx].AddChildToParent(selectGameContent);

        Transform selectGameScrollView = transform.Find("PanelSelectGame").Find("Scroll View");

        Vector2 contentSize = selectGameScrollView.GetComponent<RectTransform>().sizeDelta;

        int itemCount = selectGameContent.transform.childCount;
        int rowMaxCount = itemCount / 2 + itemCount % 2;

        Transform child = selectGameContent.transform.GetChild(0);
        RectTransform childRect = child.GetComponent<RectTransform>();

        float childWidth = childRect.sizeDelta.x * childRect.localScale.x;
        float childHeight = childRect.sizeDelta.y * childRect.localScale.y;

        float richRowSpacing = (contentSize.x - childWidth * 3);
        float n = 0.85f;
        float rowSpacing = richRowSpacing * (1 - n) / 2;
        float rowPadding = richRowSpacing * n / 2;

        float richColSpacing = (contentSize.y - childHeight * 2);
        float ncol = 0.85f;
        float colSpacing = richColSpacing * (1 - ncol);
        float colPadding = richColSpacing * ncol / 2;

        float contentWidth = rowPadding * 2 + (rowMaxCount - 1) * rowSpacing + rowMaxCount * childWidth;
        float contentHeight = colPadding * 2 + colSpacing + 2 * childHeight;

        selectGameContent.GetComponent<RectTransform>().sizeDelta = new Vector2(contentWidth, contentHeight);

        int i = 1;
        float x = -contentWidth / 2;
        float y = contentHeight / 2;

        float row1_x = x + rowPadding;
        float row1_y = y - colPadding;

        float row2_x = x + rowPadding;
        float row2_y = row1_y - childHeight - colSpacing;

        float endx, endy;

        foreach (Transform go in selectGameContent.transform)
        {
            if (i % 2 != 0)
            {
                endx = row1_x + childWidth / 2;
                endy = row1_y - childHeight / 2;
                row1_x += childWidth + rowSpacing;
            }
            else
            {
                endx = row2_x + childWidth / 2;
                endy = row2_y - childHeight / 2;
                row2_x += childWidth + rowSpacing;
            }

            RectTransform rt = go.GetComponent<RectTransform>();
            Vector2 localPos = rt.localPosition;

            float offsetx = (endx - localPos.x) / 100.0f;
            float offsety = (endy - localPos.y) / 100.0f;

            selectItemOffsetList.Add(new Vector2(offsetx / s, offsety / s));
            i++;
        }

        state01_animal_Count++;
        ItemAnimComplete();
    }

    void ItemAnimComplete()
    {   
        Transform childOG = selectGameContent.transform.GetChild(itemIdx);
        Tweener tweener = childOG.DOBlendableMoveBy(new Vector3(selectItemOffsetList[itemIdx].x, selectItemOffsetList[itemIdx].y, 0), 0.07f).SetEase(Ease.Flash);

        itemIdx++;

        if (itemIdx == selectGameContent.transform.childCount)
        {
            itemIdx = 0;
            state01_animal_Count--;
            if (state01_animal_Count == 0)
                state = 1;
            return;
        }

        tweener.onComplete = ItemAnimComplete;
    }


    void ChangeState_1_to_0()
    {
        if (state == 0 || state != 1)
            return;

        float s = GetScreenScale();

        Transform playerHeadTransform = transform.Find("PanelPlayerIcon");
        Transform waiterTransform = transform.Find("ImgWaiter");
        Transform funcPanelBgTransform = transform.Find("ImgFuncItemBg");
        Transform funcTransform = transform.Find("PanelFuncItems");
        Transform rankingTransform = transform.Find("RankingListView");
        CanvasGroup rankingCanvasGroup = rankingTransform.GetComponent<CanvasGroup>();
        Transform btnPullTransform = transform.Find("BtnPull");
        Transform selectGameTransform = transform.Find("PanelSelectGame");

        Image btnPullImg = btnPullTransform.GetComponent<Image>();
        state10_animal_Count++;
        btnPullImg.DOColor(new Color(1, 1, 1, 1), 0.15f).onComplete = delegate ()
        {
            state10_animal_Count--;
            if (state10_animal_Count == 0)
                state = 0;
        };


        funcPanelBgTransform.gameObject.SetActive(true);
        funcTransform.gameObject.SetActive(true);

        Tweener tweener;
        rankingCanvasGroup.alpha = 0;
        tweener = rankingCanvasGroup.DOFade(1, 0.5f);
        state10_animal_Count++;
        tweener.onComplete = delegate ()
        {
            state10_animal_Count--;
            if (state10_animal_Count == 0)
                state = 0;
        };

        //
        for (int i = 0; i < 3; i++)
        {
            if (btnSelectGame[i] == null)
                continue;

            CanvasGroup btnCanvasGroup = btnSelectGame[i].GetComponent<CanvasGroup>();
            tweener = btnCanvasGroup.DOFade(1, 0.5f);
            state10_animal_Count++;
            tweener.onComplete = delegate ()
            {
                state10_animal_Count--;
                if (state10_animal_Count == 0)
                    state = 0;
            };
        }


        tweener = waiterTransform.DOMoveX(6.0f/s, 0.2f).SetEase(Ease.Flash).SetRelative();
        state10_animal_Count++;

        tweener.onComplete = delegate ()
        {
            state10_animal_Count--;
            if (state10_animal_Count == 0)
                state = 0;
        };



        tweener = playerHeadTransform.DOMoveX(-1.2f/s, 0.4f).SetEase(Ease.Flash).SetRelative();
        state10_animal_Count++;

        tweener.onComplete = delegate ()
        {
            state10_animal_Count--;
            if (state10_animal_Count == 0)
                state = 0;
        };


        btnReturn.gameObject.SetActive(false);

        //  
        float w = selectGameTransform.GetComponent<RectTransform>().sizeDelta.x / 2;
        float sw = transform.GetComponent<RectTransform>().sizeDelta.x;
        selectGameTransform.GetComponent<RectTransform>().localPosition = new Vector3(sw / 2 + w, selectGameTransform.GetComponent<RectTransform>().localPosition.y);
        selectGameTransform.gameObject.SetActive(false);


        Transform selectGameScrollView = transform.Find("PanelSelectGame").Find("Scroll View");
        Vector2 contentSize = selectGameScrollView.GetComponent<RectTransform>().sizeDelta;
        Transform child = selectGameContent.transform.GetChild(0);
        RectTransform childRect = child.GetComponent<RectTransform>();
        float childWidth = childRect.sizeDelta.x * childRect.localScale.x;
        float childHeight = childRect.sizeDelta.y * childRect.localScale.y;


        float y = contentSize.y/2 + childHeight / 2;
        float x = contentSize.x/2 - childWidth / 2;

        foreach (Transform go in selectGameContent.transform)
        {
            go.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
        }

    }


    void ChangeState_0_to_2()
    {
        if (state != 0 || state == 2)
            return;

        panelFreeze.gameObject.SetActive(true);


        Transform gameListTransform = transform.Find("ImgSelectGameList");
        Vector3 pos = gameListTransform.GetComponent<RectTransform>().localPosition;

        float s = (1920.0f / Screen.width) / (1080.0f / Screen.height);
        var tweener = gameListTransform.DOMoveX(-7.1f/s, 0.2f).SetEase(Ease.Flash).SetRelative();

        state02_animal_Count++;

        tweener.onComplete = delegate ()
        {
            state02_animal_Count--;
            if (state02_animal_Count == 0)
                state = 2;
        };

    }

    public void ChangeState_2_to_0(string[] names = null)
    {
        if (state != 2 || state == 0)
            return;

        float s = GetScreenScale();

        if (names != null && names.Length > 0)
        {
            app.CreateCurtGames(names);
            CreateSelectGamesButton(1);
            isStartSelectGameBtnAnim = true;
        }

        Transform gameListTransform = transform.Find("ImgSelectGameList");

        var tweener = gameListTransform.DOMoveX(7.1f/s, 0.2f).SetEase(Ease.Flash).SetRelative();
        state20_animal_Count++;

        tweener.onComplete = delegate ()
        {
            if (isStartSelectGameBtnAnim == true)
            {
                BtnAnimComplete();
            }
            else
            {
                state20_animal_Count--;
                if (state20_animal_Count == 0)
                {
                    isStartSelectGameBtnAnim = false;
                    state = 0;
                }
            }
        };

        panelFreeze.gameObject.SetActive(false);
    }

    void BtnAnimComplete()
    {
        float s = GetScreenScale();
        Tweener tweener = btnSelectGame[itemIdx].transform.DOMoveX(5.75f / s, 0.15f).SetEase(Ease.OutBack);
     
        itemIdx++;

        if (itemIdx == 3)
        {
            itemIdx = 0;
            state20_animal_Count--;
            if (state20_animal_Count == 0)
                state = 0;

            isStartSelectGameBtnAnim = false;
            return;
        }

        tweener.onComplete = BtnAnimComplete;
    }
}