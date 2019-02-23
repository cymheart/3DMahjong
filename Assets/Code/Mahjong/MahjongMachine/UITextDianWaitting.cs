using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MahjongMachineNS
{
    public class UITextDianWaitting
    {
        GameObject[] dians;
        int curtIdx = -1;
        int count = 0;

        int state = -1;
        float stateStartTime;
        float stateLiveTime;


        public UITextDianWaitting(GameObject[] dians)
        {
            this.dians = dians;
            count = dians.Length + 1;
            curtIdx = 0;
        }

        public void Play()
        {
            curtIdx = -1;

            for (int i = 0; i < dians.Length; i++)
                dians[i].SetActive(false);

            SetState(0, 0);
        }
        public void Stop()
        {
            SetState(-1, 0);
            curtIdx = -1;

            for (int i = 0; i < dians.Length; i++)
                dians[i].SetActive(false);
        }

        public void Update()
        {
            if (state < 0 || Time.time - stateStartTime < stateLiveTime)
            {
                return;
            }

            curtIdx = (++curtIdx) % count;

            if (curtIdx == dians.Length)
            {
                for (int i = 0; i < dians.Length; i++)
                    dians[i].SetActive(false);
            }
            else
            {
                dians[curtIdx].SetActive(true);
            }

            SetState(0, 0.3f);
        }

        void SetState(int _state, float liveTime)
        {
            state = _state;
            stateStartTime = Time.time;
            stateLiveTime = liveTime;
        }
    }
}

