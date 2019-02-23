using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Assets
{
    public class DazhongMahjongGameSetting : GameSetting
    {
        Transform btnTransform;
        public SpriteAtlas atlas;

        public DazhongMahjongGameSetting(App app)
            :base(app)
        {
        }

        public override void Destroy()
        {
            Object.Destroy(btnTransform.gameObject);
            atlas = null;
        }

        //平民场
        public void CreatePingMinRoom()
        {
            baseScore = 300;
            minRequireMoney = 1000;
            maxRequireMoney = 15000;

            btnTransform = CreateRoomTransform("select_i_1", minRequireMoney, maxRequireMoney);

            EventTriggerListener.Get(btnTransform.gameObject).onClick = OnButtonClick;
        }


        //文人场
        public void CreateWenRenRoom()
        {
            baseScore = 800;
            minRequireMoney = 10000;
            maxRequireMoney = 70000;

            btnTransform = CreateRoomTransform("select_i_2", minRequireMoney, maxRequireMoney);
        }

        //富豪场
        public void CreateFuHaoRoom()
        {
            baseScore = 2400;
            minRequireMoney = 20000;
            maxRequireMoney = 150000;

            btnTransform = CreateRoomTransform("select_i_3", minRequireMoney, maxRequireMoney);
        }


        //达官场
        public void CreateDaGuanRoom()
        {
            baseScore = 12000;
            minRequireMoney = 50000;
            maxRequireMoney = -1;

            btnTransform = CreateRoomTransform("select_i_4", minRequireMoney, maxRequireMoney);
        }


        //至尊
        public void CreateZhiZunRoom()
        {
            baseScore = 30000;
            minRequireMoney = 150000;
            maxRequireMoney = -1;

            btnTransform = CreateRoomTransform("select_i_5", minRequireMoney, maxRequireMoney);
        }

        private void OnButtonClick(GameObject go)
        {
            Text info = app.uiMain.transform.Find("Info").GetComponent<Text>();
            info.text += "准备加载场景:MajiongGameScene" + "\n";

            SceneManager.LoadSceneAsync("MajiongGameScene");
        }


        Transform CreateRoomTransform(string imgName, float minRequireMoney, float maxRequireMoney)
        {
           Transform tf = CreateBaseTransform();

            tf.gameObject.GetComponent<Image>().sprite = atlas.GetSprite(imgName);
            tf.Find("TextBaseScore").GetComponent<Text>().text = baseScore.ToString();
            tf.Find("TextCoin").GetComponent<Text>().text = TransMoneyRange(minRequireMoney, maxRequireMoney);
            settingView = tf.gameObject;
            return tf;
        }

        Transform CreateBaseTransform()
        {
            GameObject btnSelect = Object.Instantiate(app.btnRoomSelectPrefab);
            Object.DontDestroyOnLoad(btnSelect);

            Button button = btnSelect.GetComponent<Button>();  
            Transform tf = button.transform;


            tf.Find("TextInfo").gameObject.SetActive(false);
            tf.Find("ImageMul").gameObject.SetActive(false);
            tf.Find("TextMul").gameObject.SetActive(false);

            return tf;
        }


    }
}
