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

        public void Update()
        {
            var varIter = updaterDict.GetEnumerator();
            while (varIter.MoveNext())
            {
                varIter.Current.Value();
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
            updaterDict.Remove(name);
        }
    }
}