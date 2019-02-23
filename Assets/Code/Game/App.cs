
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using MahjongMachineNS;

namespace Assets
{
    public class App
    {
        //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
        //大家在正式项目中请全局只创建一个AppDomain
      //  public AppDomain appdomain;

        public Game[] curtGames = new Game[3];
        Dictionary<string, Game> GameDict = new Dictionary<string, Game>();

        public List<GameObject> selectGameItems = new List<GameObject>();


        //
        public GameObject btnRoomSelectPrefab;
        public GameObject gameBtnPrefab;
        public GameObject gameSelectItemPrefab;
        public AssetBundle ab;
        public SpriteAtlas atlas;
        public SpriteAtlas mjAtlas;
        public TPAtlas tpAtlas;
        public TPAtlas tpAtlas2;

        public UIMain uiMain;
        public ResPool resPool;

        Texture2D tex;
        Texture2D tex2;

        public GameObject mj_dui_east_shadow;
        public GameObject mj_dui_east;

        public GameObject mj_dui_west_shadow;
        public GameObject mj_dui_west;

        public GameObject mj_dui_zheng_shadow;
        public GameObject mj_dui_zheng;

        public GameObject mj_hand_fan;
        public GameObject mj_hand_main;
        public GameObject mj_hand_east;
        public GameObject mj_hand_west;
        public GameObject mj_hand_north;

        public GameObject mj_ping_ce_east;
        public GameObject mj_ping_ce_west;
        public GameObject mj_ping_zheng_north;
        public GameObject mj_ping_zheng_self;

        public GameObject selfHandPai;

        public Dictionary<MahjongFaceValue, string> mjSpriteNameDict = new Dictionary<MahjongFaceValue, string>();
        

        static string[] faceNames = new string[] 
        {
            "hall_btn_room1",
            "hall_btn_room2",
            "hall_btn_room3",
            "hall_btn_room4",
            "hall_btn_room5",
            "hall_btn_room6",
            "hall_btn_room7",
            "hall_btn_room9",
            "hall_btn_room10",
            "hall_btn_room11",
            "hall_btn_room12",
        };



        public void CreateRes(AssetBundle ab)
        {
            btnRoomSelectPrefab = Resources.Load("Prefabs/BtnRoomSelect", typeof(GameObject)) as GameObject;
            gameBtnPrefab = Resources.Load("Prefabs/BtnXueliuMahjong", typeof(GameObject)) as GameObject;
            gameSelectItemPrefab = Resources.Load("Prefabs/GameItem", typeof(GameObject)) as GameObject;
            //string path = "AssetBundles/dd/gamebtn.res";

            // ab = AssetBundle.LoadFromFile(_path);
            this.ab = ab;
            atlas = ab.LoadAsset<SpriteAtlas>("BtnAtlas");

            tex = ab.LoadAsset<Texture2D>("effect_ui_tubiaoliuguang010");
            TextAsset ta = ab.LoadAsset<TextAsset>("effect_ui_tubiaoliuguang010");
            tpAtlas = new TPAtlas();
            tpAtlas.CreateSpriteFrame(ta.text, tex);


            tex2 = ab.LoadAsset<Texture2D>("effect_ui_jinbi010");
            ta = ab.LoadAsset<TextAsset>("effect_ui_jinbi010");
            tpAtlas2 = new TPAtlas();
            tpAtlas2.CreateSpriteFrame(ta.text, tex2);

            CreateGameSelectItems();
        }

  
        public void LoadHotFixAssembly(byte[] dll, byte[] pdb)
        {
            ////首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
            //appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            ////正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
            ////正式发布的时候需要大家自行从其他地方读取dll

            //using (System.IO.MemoryStream fs = new MemoryStream(dll))
            //{
            //    using (System.IO.MemoryStream p = new MemoryStream(pdb))
            //    {
            //        appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
            //    }
            //}
        }


        public void CreateDefaultUI()
        {
            CreateDazhongMajangGame(atlas, 0);
            CreateXuLiuMajangGame(atlas, 1);
            CreateXueZhanMajangGame(atlas, 2);
            CreateFengkuangMajangGame(atlas, 3);
        }

        public void CreateGameSelectItems()
        {
            List<Sprite> spriteList = new List<Sprite>();
            foreach (var name in faceNames)
            {
                spriteList.Add(atlas.GetSprite(name));
            }

            foreach (var sprite in spriteList)
            {
                var item = UnityEngine.Object.Instantiate(gameSelectItemPrefab);
                item.transform.Find("Image").GetComponent<Image>().sprite = sprite;
                UnityEngine.Object.DontDestroyOnLoad(item);
                selectGameItems.Add(item);
            }
        }


        public void CreateCurtGames(string[] names)
        {
            if (names == null || names.Length == 0)
                return;

            for (int i = 0; i < names.Length; i++)
            {
                curtGames[i].btnMahjong.transform.SetParent(null, false);
                curtGames[i] = GameDict[names[i]];
            }
        }

        void CreateDazhongMajangGame(SpriteAtlas atlas, int idx)
        {
            Game dzmjGame = new Game(this);
            dzmjGame.CreateDazhongMahjongGame(atlas);

            DazhongMahjongGameSetting setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreatePingMinRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateWenRenRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateFuHaoRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateDaGuanRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateZhiZunRoom();
            dzmjGame.AppendSetting(setting);

            curtGames[idx] = dzmjGame;
            GameDict.Add("hall_btn_room1", dzmjGame);
        }


        void CreateXuLiuMajangGame(SpriteAtlas atlas, int idx)
        {
            Game dzmjGame = new Game(this);
            dzmjGame.CreateXueLiuMahjongGame(atlas);

            DazhongMahjongGameSetting setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreatePingMinRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateWenRenRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateFuHaoRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateDaGuanRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateZhiZunRoom();
            dzmjGame.AppendSetting(setting);

            curtGames[idx] = dzmjGame;
            GameDict.Add("hall_btn_room3", dzmjGame);
        }


        void CreateXueZhanMajangGame(SpriteAtlas atlas, int idx)
        {
            Game dzmjGame = new Game(this);
            dzmjGame.CreateXuezhangMahjongGame(atlas);

            DazhongMahjongGameSetting setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreatePingMinRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateWenRenRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateFuHaoRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateDaGuanRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateZhiZunRoom();
            dzmjGame.AppendSetting(setting);

            curtGames[idx] = dzmjGame;
            GameDict.Add("hall_btn_room2", dzmjGame);
        }


        void CreateFengkuangMajangGame(SpriteAtlas atlas, int idx)
        {
            Game dzmjGame = new Game(this);
            dzmjGame.CreateFengkuangMahjongGame(atlas);

            DazhongMahjongGameSetting setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreatePingMinRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateWenRenRoom();
            dzmjGame.AppendSetting(setting);


            setting = new DazhongMahjongGameSetting(this);
            setting.atlas = atlas;
            setting.CreateFuHaoRoom();
            dzmjGame.AppendSetting(setting);



            GameDict.Add("hall_btn_room7", dzmjGame);
        }

    }
}
