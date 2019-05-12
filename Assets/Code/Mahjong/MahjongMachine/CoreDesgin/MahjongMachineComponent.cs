
using System.Collections.Generic;

namespace CoreDesgin
{
    /// <summary>
    /// 麻将机组件
    /// </summary>
    public class MahjongMachineComponent : CommonComponent
    {
        public delegate void InitProcessDelegate();

        struct InitFuncListOrderInfo
        {
            public int order;
            public InitProcessDelegate initFunc;
        }

        class OrderComparer : IComparer<InitFuncListOrderInfo>
        {
            public int Compare(InitFuncListOrderInfo left, InitFuncListOrderInfo right)
            {
                if (left.order > right.order)
                    return 1;
                else if (left.order == right.order)
                    return 0;
                else
                    return -1;
            }
        }

        List<InitFuncListOrderInfo> childInitFuncOrderInfoList = new List<InitFuncListOrderInfo>();
        OrderComparer orderComparer = new OrderComparer();

        bool isSorted = false;
        bool isWait = false;
        int waitChildComponentIdx = 0;
        int waitInitProcessIdx = 0;
        int state = 0;
       

        public void AddInitMethodToParent(InitProcessDelegate initProcessFunc, int order)
        {
            if (Parent == null || Parent.GetType() != typeof(MahjongMachineComponent))
                return;

            InitFuncListOrderInfo info = new InitFuncListOrderInfo();
            info.order = order;
            info.initFunc = initProcessFunc;

            var parentMjMachineComponent = Parent as MahjongMachineComponent;
            parentMjMachineComponent.childInitFuncOrderInfoList.Add(info);
        }

        public virtual void SetInitMethod()
        {
            SetChildrenComponentInitMethod();
        }

        void SetChildrenComponentInitMethod()
        {
            MahjongMachineComponent mjMachineChildrenComponent;
            CommonComponent[] components = GetAllComponent();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].GetType() == typeof(MahjongMachineComponent))
                {
                    mjMachineChildrenComponent = components[i] as MahjongMachineComponent;
                    mjMachineChildrenComponent.SetInitMethod();
                }
            }
        }


        bool Init()
        {
            if (waitChildComponentIdx != -1)
            {
                waitChildComponentIdx = InitChildrenComponent(waitChildComponentIdx);
                return false;
            }

            if (isSorted == false)
            {
                childInitFuncOrderInfoList.Sort(orderComparer);
                isSorted = true;
            }

            for (int i= waitInitProcessIdx; i < childInitFuncOrderInfoList.Count; i++)
            {
                childInitFuncOrderInfoList[i].initFunc();
                if(isWait == false)
                {
                    waitInitProcessIdx = i + 1;
                    return false;
                }          
            }

            return true;
        }

        int InitChildrenComponent(int componentIdx)
        {
            MahjongMachineComponent mjMachineChildrenComponent;
            CommonComponent[] components = GetAllComponent();
            bool isInitCompleted;

            for (int i = componentIdx; i < components.Length; i++)
            {
                if (components[i].GetType() == typeof(MahjongMachineComponent))
                {
                    mjMachineChildrenComponent = components[i] as MahjongMachineComponent;
                    isInitCompleted = mjMachineChildrenComponent.Init();

                    if (isInitCompleted == false)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }


        public void SetWait(bool isWait)
        {
            this.isWait = isWait;

            if (Parent.GetType() != typeof(MahjongMachineComponent))
                return;

            var mjMachineParent = Parent as MahjongMachineComponent;
            if (mjMachineParent != null)
                mjMachineParent.SetWait(isWait);
        }

        public bool Update()
        {
            if (isWait)
                return false;

            bool isInitCompleted = Init();

            if(isInitCompleted)
                return true;
   
            return false;
          
        }

    }
}
