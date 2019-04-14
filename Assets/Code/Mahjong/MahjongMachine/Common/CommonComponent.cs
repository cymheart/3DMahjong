using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CommonComponent
{
    Dictionary<Type, CommonComponent> componentDict = new Dictionary<Type, CommonComponent>();
    public CommonComponent Parent { get; private set; }

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddComponent<T>() where T : CommonComponent, new()
    {
        T component = new T();
        componentDict[typeof(T)] = component;
        component.Parent = this;
        return component;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <returns></returns>
    public Type AddComponent(Type type)
    {
        object obj = Activator.CreateInstance(type, true);
        CommonComponent component = obj as CommonComponent;
        componentDict[type] = component;
        component.Parent = this;
        return obj as Type;
    }


    public T GetComponent<T>() where T: CommonComponent
    {
        CommonComponent component;
        bool ret = componentDict.TryGetValue(typeof(T), out component);
        if (ret == true)
            return (T)component;
        return null;
    }

    public CommonComponent[] GetAllComponent()
    {
        var varIter = componentDict.GetEnumerator();
        CommonComponent[] componets = new CommonComponent[componentDict.Count];
        int i = 0;
        while (varIter.MoveNext())
        {
            componets[i++] = varIter.Current.Value;
        }

        return componets;
    }
}

