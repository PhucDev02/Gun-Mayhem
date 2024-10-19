using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Game.Core.Singleton;
using UnityEngine;

public enum PoolObject
{
    Booster,
    TowerBullet
}

public class ObjectPool : Singleton<ObjectPool>
{
    [SerializeField] private Pool[] Pools;
    private void Awake()
    {
        foreach (var pool in Pools)
        {
            pool.ListObject = new List<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                pool.ListObject.Add(CreateGameObject(pool.Prefab));
            }
        }
    }
    private GameObject CreateGameObject(GameObject prefab)
    {

        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Spawn(PoolObject tag)
    {
        foreach (var pool in Pools)
        {
            if (pool.objectTag.Equals(tag))
            {
                foreach (var obj in pool.ListObject)
                {
                    if (!obj.activeInHierarchy)
                    {
                        obj.SetActive(true);
                        obj.transform.SetParent(this.transform);
                        return obj;
                    }
                }
                // expand pool
                if (pool.Expandable)
                {
                    GameObject obj = CreateGameObject(pool.Prefab);
                    pool.ListObject.Add(obj);
                    obj.SetActive(true);
                    obj.transform.SetParent(this.transform);
                    return obj;
                }
                else
                {
                    Debug.LogWarning("The pool with tag " + tag + " is not expandable!");
                    return null;
                }
            }
        }

        Debug.LogWarning("The pool with tag " + tag + " is not exist!");
        return null;
    }
    public void Recall(GameObject obj)
    {
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.SetActive(false);
    }

    public void RecallAll()
    {
        foreach (var pool in Pools)
        {
            foreach (var obj in pool.ListObject)
            {
                Recall(obj);
            }
        }
    }
}
[Serializable]
public class Pool
{
    public PoolObject objectTag;
    public GameObject Prefab;

    public int Size = 1;

    public bool Expandable = true;

    [HideInInspector]
    public List<GameObject> ListObject;

}