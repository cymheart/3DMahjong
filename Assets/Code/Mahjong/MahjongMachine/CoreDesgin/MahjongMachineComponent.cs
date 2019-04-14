using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreDesgin
{
    /// <summary>
    /// 麻将机组件接口
    /// </summary>
    public interface MjMachineComponentInterface
    {
        void Wake();
        void PreInit();

        void PreLoad();

        void Init();
        void Load();

        void Destory();
    }


    /// <summary>
    /// 麻将机组件
    /// </summary>
    public class MahjongMachineComponent:CommonComponent, MjMachineComponentInterface
    {
        void StateJieDuan(int type)
        {
            MahjongMachineComponent mjMachineComponent;
            CommonComponent[] components = GetAllComponent();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].GetType() == typeof(MahjongMachineComponent))
                {
                    mjMachineComponent = components[i] as MahjongMachineComponent;

                    switch (type)
                    {
                        case 0: mjMachineComponent.Wake(); break;
                        case 1: mjMachineComponent.PreInit(); break;
                        case 2: mjMachineComponent.PreLoad(); break;
                        case 3: mjMachineComponent.Init(); break;
                        case 4: mjMachineComponent.Load(); break;
                        case 5: mjMachineComponent.Destory(); break;
                    }
                }
            }
        }

        public virtual void Wake()
        {
            StateJieDuan(0);
        }

        public virtual void PreInit()
        {
            StateJieDuan(1);
        }

        public virtual void PreLoad()
        {
            StateJieDuan(2);
        }

        /// <summary>
        /// 初始化其子组件
        /// </summary>
        public virtual void Init()
        {
            StateJieDuan(3);
        }

        /// <summary>
        /// 加载设置子组件内部数据
        /// </summary>
        public virtual void Load()
        {
            StateJieDuan(4);
        }

        public virtual void Destory()
        {
            StateJieDuan(5);
        }
    }
}
