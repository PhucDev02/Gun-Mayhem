using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Object
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<T>();
            return instance;
        }
    }
    protected virtual void OnDestroy()
    {
        instance = null;
    }
}