using CoreDesgin;
using System;
using UnityEngine;

namespace ComponentDesgin
{
    public class MjHuTingCheck : MahjongMachineComponent
    {
        MahjongMachine mjMachine;
        MjHuTingAlgorithm mjHuTingAlgo;
        public TingData[] tingDatas;

        public override void Init()
        {
            base.Init();

            mjMachine = (MahjongMachine)Parent;
            mjHuTingAlgo = new MjHuTingAlgorithm();
            mjHuTingAlgo.Train();

            tingDatas = mjHuTingAlgo.CreateTingDataMemory();
        }

        public void CheckTing(byte[] cards, byte laiziIndex)
        {
            mjHuTingAlgo.CheckTing(cards, laiziIndex, ref tingDatas);
        }
    }
}