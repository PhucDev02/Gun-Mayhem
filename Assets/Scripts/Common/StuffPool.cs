using System.Collections.Generic;
using UnityEngine;
// ko biet dat  ten la gi
public class StuffPool : MonoBehaviour
{
    [SerializeField] List<GameObject> list = new();
    public GameObject Get()
    {
        foreach (var item in list)
        {
            if (!item.activeInHierarchy)
            {
                item.SetActive(true);
                return item;
            }
        }
        var newItem = Instantiate(list[0],transform);
        list.Add(newItem);
        return newItem;
    }
}
