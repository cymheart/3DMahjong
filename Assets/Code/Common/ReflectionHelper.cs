using System;
using System.Reflection;
/// <summary>
/// 反射帮助类
/// </summary>
public static class ReflectionHelper
{
    /// <summary>
    /// 创建对象实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fullName">命名空间.类型名</param>
    /// <param name="assemblyName">程序集</param>
    /// <returns></returns>
    public static T CreateInstance<T>(string fullName, string assemblyName)
    {
        string path = fullName + "," + assemblyName;//命名空间.类型名,程序集
        Type o = Type.GetType(path);//加载类型
        object obj = Activator.CreateInstance(o, true);//根据类型创建实例
        return (T)obj;//类型转换并返回
    }

    /// <summary>
    /// 创建对象实例
    /// </summary>
    /// <typeparam name="T">要创建对象的类型</typeparam>
    /// <param name="assemblyName">类型所在程序集名称</param>
    /// <param name="nameSpace">类型所在命名空间</param>
    /// <param name="className">类型名</param>
    /// <returns></returns>
    public static T CreateInstance<T>(string assemblyName, string nameSpace, string className)
    {
        try
        {
            string fullName = nameSpace + "." + className;//命名空间.类型名
                                                          //此为第一种写法
            object ect = Assembly.Load(assemblyName).CreateInstance(fullName);//加载程序集，创建程序集里面的 命名空间.类型名 实例
            return (T)ect;//类型转换并返回
                          //下面是第二种写法
                          //string path = fullName + "," + assemblyName;//命名空间.类型名,程序集
                          //Type o = Type.GetType(path);//加载类型
                          //object obj = Activator.CreateInstance(o, true);//根据类型创建实例
                          //return (T)obj;//类型转换并返回
        }
        catch
        {
            //发生异常，返回类型的默认值
            return default(T);
        }
    }
}