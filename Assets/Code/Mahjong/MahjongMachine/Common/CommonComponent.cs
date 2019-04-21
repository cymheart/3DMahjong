using System;
using System.Collections.Generic;

public class CommonComponent
{
    Dictionary<Type, List<CommonComponent>> componentDict = new Dictionary<Type, List<CommonComponent>>();
    public CommonComponent Parent { get; private set; }

    int count = 0;

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddComponent<T>() where T : CommonComponent, new()
    {
        T component = new T();
        component.Parent = this;

        Type key = typeof(T);
        List<CommonComponent> elemList;

        if (componentDict.ContainsKey(key))
        {
            elemList = componentDict[key];
            elemList.Add(component);
        }
        else
        {
            elemList = new List<CommonComponent>();
            componentDict[key] = elemList;
        }

        elemList.Add(component);
        count++;

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
        component.Parent = this;

        Type key = type;
        List<CommonComponent> elemList;

        if (componentDict.ContainsKey(key))
        {
            elemList = componentDict[key];
            elemList.Add(component);
        }
        else
        {
            elemList = new List<CommonComponent>();
            componentDict[key] = elemList;
        }

        elemList.Add(component);
        count++;

        return obj as Type;
    }

    /// <summary>
    /// 添加多个同样的组件
    /// </summary>
    /// <returns></returns>
    public Type[] AddComponent(Type type, int count)
    {
        Type[] ctype = new Type[count];
        for(int i=0; i<count; i++)
        {
            ctype[i] = AddComponent(type);
        }

        this.count += count;
        return ctype;
    }

    public T GetComponent<T>() where T: CommonComponent
    {
        List<CommonComponent> components;
        bool ret = componentDict.TryGetValue(typeof(T), out components);
        if (ret == true)
            return (T)(components[0]);
        return null;
    }

    public T[] GetComponents<T>() where T : CommonComponent
    {
        List<CommonComponent> components;
        bool ret = componentDict.TryGetValue(typeof(T), out components);
        if (ret == true)
            return (T[])(components.ToArray());
        return null;
    }


    public CommonComponent[] GetAllComponent()
    {     
        CommonComponent[] componets = new CommonComponent[count];
        List<CommonComponent> tmpComponentList;
        int j = 0;

        var varIter = componentDict.GetEnumerator();
        while (varIter.MoveNext())
        {
            tmpComponentList = varIter.Current.Value;
            for (int i = 0; i< tmpComponentList.Count; i++ )
            {
                componets[j++] = tmpComponentList[i];
            }
        }

        return componets;
    }
}

