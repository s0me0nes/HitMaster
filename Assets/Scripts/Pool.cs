using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : MonoBehaviour
{ 
    public T Prefab { get; }
    public bool AutoExpand { get; set; }
    public Transform Container { get; }

    private List<T> _pool;

    public Pool(T prefab, int count)
    {
        Prefab = prefab;
        Container = null;
        CreatePool(count);
    }

    public Pool(T prefab, Transform container, int count)
    {
        Prefab = prefab;
        Container = container;
        CreatePool(count);
    }

    private void CreatePool(int count)
    {
        _pool = new List<T>(count);

        for (int i = 0; i < count; i++)
            CreateObject();
    }

    private T CreateObject(bool isActiveByDefault = false)
    {
        T obj = Object.Instantiate(Prefab, Container);
        obj.gameObject.SetActive(isActiveByDefault);
        _pool.Add(obj);
        return obj;
    }

    public bool HasFreeElement(out T element)
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (_pool[i].gameObject.activeInHierarchy)
                continue;
            
            element = _pool[i];
            _pool[i].gameObject.SetActive(true);
            return true;
        }

        element = null;
        return false;
    }

    public T GetFreeElement()
    {
        if (HasFreeElement(out T element))
            return element;

        if (AutoExpand)
            return CreateObject(true);

        return null;
    }
}