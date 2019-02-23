using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Assets
{ 
    public class Game
    {
        List<GameSetting> gameSettingList = new List<GameSetting>();
        public GameObject btnMahjong = null;
        public Sprite select_room_title;

        App app;
        public Game(App app)
        {
            this.app = app;
        }

        public void Destroy()
        {
            Object.Destroy(btnMahjong);
            select_room_title = null;

            foreach (GameSetting gs in gameSettingList)
                gs.Destroy();
        }


        public void CreateXueLiuMahjongGame(SpriteAtlas atlas)
        {
            btnMahjong = Object.Instantiate(app.gameBtnPrefab);
            Object.DontDestroyOnLoad(btnMahjong);
            Button button = btnMahjong.GetComponent<Button>();

            button.gameObject.GetComponent<Image>().sprite = atlas.GetSprite("hall_btn_room3");

            SpriteState ss = new SpriteState();
            ss.pressedSprite = atlas.GetSprite("hall_btn_room3_1");
            button.spriteState = ss;

            //
            select_room_title = atlas.GetSprite("select_room_3");
        }

        public void CreateDazhongMahjongGame(SpriteAtlas atlas)
        {
            btnMahjong = Object.Instantiate(app.gameBtnPrefab);
            Object.DontDestroyOnLoad(btnMahjong);

            Button button = btnMahjong.GetComponent<Button>();

            button.gameObject.GetComponent<Image>().sprite = atlas.GetSprite("hall_btn_room1");

            SpriteState ss = new SpriteState();
            ss.pressedSprite = atlas.GetSprite("hall_btn_room1_1");
            button.spriteState = ss;

            //
            select_room_title = atlas.GetSprite("select_room_1");

    
        }

        public void CreateXuezhangMahjongGame(SpriteAtlas atlas)
        {
            btnMahjong = Object.Instantiate(app.gameBtnPrefab);
            Object.DontDestroyOnLoad(btnMahjong);
            Button button = btnMahjong.GetComponent<Button>();

            button.gameObject.GetComponent<Image>().sprite = atlas.GetSprite("hall_btn_room2");

            SpriteState ss = new SpriteState();
            ss.pressedSprite = atlas.GetSprite("hall_btn_room2_1");
            button.spriteState = ss;

            //
            select_room_title = atlas.GetSprite("select_room_2");

        }


        public void CreateFengkuangMahjongGame(SpriteAtlas atlas)
        {
            btnMahjong = Object.Instantiate(app.gameBtnPrefab);
            Object.DontDestroyOnLoad(btnMahjong);
            Button button = btnMahjong.GetComponent<Button>();

            button.gameObject.GetComponent<Image>().sprite = atlas.GetSprite("hall_btn_room7");

            SpriteState ss = new SpriteState();
            ss.pressedSprite = atlas.GetSprite("hall_btn_room7_1");
            button.spriteState = ss;

            //
            select_room_title = atlas.GetSprite("select_room_7");

        }

        public void AddChildToParent(GameObject contentParent)
        {
            foreach(var gameSetting in gameSettingList)
            {
                gameSetting.settingView.transform.SetParent(contentParent.transform, false);
            }
        }

        public void RemoveAllSettingViewFromParent()
        {
            foreach (var gameSetting in gameSettingList)
            {
                gameSetting.settingView.transform.SetParent(null, false);
            }
        }


        public void AppendSetting(GameSetting gameSetting)
        {
            gameSettingList.Add(gameSetting);
        }
    }
}
