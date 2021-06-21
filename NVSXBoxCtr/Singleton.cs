using System;

public abstract class Singleton<T> where T : class, new()
{
    private static readonly object lockObject = new object();
    private static readonly Lazy<T> instance = new Lazy<T>(() => new T());

    public static T Instance { get { lock (lockObject) return instance.Value; } }
}