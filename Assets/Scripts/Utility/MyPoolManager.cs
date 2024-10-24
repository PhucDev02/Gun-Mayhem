using System.Collections.Generic;
using UnityEngine;

public class ReturnToMyPool : MonoBehaviour
{
    public MyPool pool;

    public void OnDisable()
    {
        pool.AddToPool(gameObject);
    }
}

public class MyPool
{
    private Stack<GameObject> stack = new Stack<GameObject>();
    private GameObject baseObj;
    private GameObject tmp;
    private ReturnToMyPool returnPool;

    public MyPool(GameObject baseObj)
    {
        this.baseObj = baseObj;
    }

    public GameObject Get()
    {
        if (stack.Count > 0)
        {
            tmp = stack.Pop();
            tmp.SetActive(true);
            return tmp;
        }
        tmp = GameObject.Instantiate(baseObj);
        returnPool = tmp.AddComponent<ReturnToMyPool>();
        returnPool.pool = this;
        return tmp;
    }

    public void AddToPool(GameObject obj)
    {
        stack.Push(obj);
    }
}

public class MyPoolManager : MonoBehaviour
{
    public static MyPoolManager Instance;
    private Dictionary<GameObject, MyPool> dicPools = new Dictionary<GameObject, MyPool>();
    private void Awake()
    {
        Instance = this;
    }
    public GameObject GetFromPool(GameObject obj)
    {
        if (dicPools.ContainsKey(obj) == false)
        {
            dicPools.Add(obj, new MyPool(obj));
        }
        return dicPools[obj].Get();
    }
}