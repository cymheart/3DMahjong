using UnityEngine;

namespace Assets
{
    public class GameSetting
    {
        public GameObject settingView;


        public string name;
        public int baseScore;      //底分
        public float minMultiple;        //最小倍数
        public float maxMultiple;        //最大倍数
        public  float minRequireMoney;  //最少需要金币
        public float maxRequireMoney;  //最多需要金币
        public int playerAmount;    //玩家数量

        public App app;

        public GameSetting(App app)
        {
            this.app = app;
        }

        public virtual void Destroy()
        {
            Object.Destroy(settingView);
        }

        public string TransMoneyRange(float minMoney, float maxMoney)
        {
            string a = TransMoneyUnit(minMoney);

            if (maxMoney < 0)
                return a + "以上";

            string b = TransMoneyUnit(maxMoney);

            return a + "~" + b;
        }
        public string TransMoneyUnit(float money)
        {
            if (money < 1000)
            {
                return money.ToString();
            }
            if (money >= 1000 && money < 10000)
            {
                return money / 1000 + "千";
            }

            return money / 10000 + "万";
        }

    }
}
