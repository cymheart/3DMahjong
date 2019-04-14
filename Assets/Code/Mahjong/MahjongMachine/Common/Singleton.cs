public abstract class Singleton<T> where T : new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (null == instance)
                instance = new T();

            return instance;
        }
    }

    protected Singleton()
    {
        
    }
}
