using CoreDesgin;
using System;
using UnityEngine;

namespace ComponentDesgin
{
    public class MjHuTingCheck : MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MjHuTingAlgorithm mjHuTingAlgo;

        public override void Init()
        {
            base.Init();

            mjMachine = (MahjongMachine)Parent;
            mjHuTingAlgo = new MjHuTingAlgorithm();
            mjHuTingAlgo.Train();
        }

        public void CheckTing(byte[] cards, byte laiziIndex, ref TingData[] tingDatas)
        {
            mjHuTingAlgo.CheckTing(cards, laiziIndex, ref tingDatas);
        }
    }
}