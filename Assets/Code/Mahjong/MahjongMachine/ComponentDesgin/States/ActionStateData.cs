using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ComponentDesgin
{
    public class ActionStateData : CommonComponent
    {
        public LinkedListNode<MahjongMachineCmd> opCmdNode;
        public Transform[] handShadowAxis = new Transform[2];
    }
}
