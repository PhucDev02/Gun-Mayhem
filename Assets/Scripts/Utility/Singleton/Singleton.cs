using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool dontDestroyOnLoad = false;

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var singletonObj = new GameObject();
                singletonObj.name = typeof(T).ToString();
                _instance = singletonObj.AddComponent<T>();
            }

            return _instance;
        }
    }

    public virtual void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("SingleAccessPoint, Destroy duplicate instance " + name + " of " + Instance.name,
                gameObject);
            Destroy(gameObject);
            return;
        }

        _instance = GetComponent<T>();

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (_instance == null)
        {
            Debug.LogWarning("SingleAccessPoint<" + typeof(T).Name + "> Instance null in Awake", gameObject);
            return;
        }

        Debug.Log("SingleAccessPoint instance found " + Instance.GetType().Name);
    }
}