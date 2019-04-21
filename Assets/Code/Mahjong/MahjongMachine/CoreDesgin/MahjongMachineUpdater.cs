using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CoreDesgin
{
    public class MahjongMachineUpdater
    {
        public delegate void UpdaterDelegate();
        Dictionary<string, UpdaterDelegate> updaterDict = new Dictionary<string, UpdaterDelegate>();

        List<string> removeElemList = new List<string>();

        public void Update()
        {
            var varIter = updaterDict.GetEnumerator();
            while (varIter.MoveNext())
            {
                varIter.Current.Value();
            }

            if (removeElemList.Count > 0)
            {
                for (int i = 0; i < removeElemList.Count; i++)
                    updaterDict.Remove(removeElemList[i]);
                removeElemList.Clear();
            }
        }

        public void Reg(string name, UpdaterDelegate action)
        {
            if (!updaterDict.ContainsKey(name))
            {
                updaterDict[name] = action;
            }
        }

        public void UnReg(string name)
        {
            removeElemList.Add(name);
        }

        public void UnAllReg()
        {
            updaterDict.Clear();
            removeElemList.Clear();
        }
    }
}