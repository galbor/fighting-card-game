using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T:MonoBehaviour
{
    private Stack<T> _pool;
    private T _template;
    private Transform _parent;

    public Pool(T template, Transform parent)
    {
        _pool = new Stack<T>();
        _template = template;
        _parent = parent;
    }
    
    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Push(obj);
    }

    public T GetFromPool()
    {
        T res;
        if (_pool.Count > 0)
            res = _pool.Pop();
        else
        {
            res = Object.Instantiate(_template.gameObject, _parent).GetComponent<T>();
        }
        res.gameObject.SetActive(true);
        return res;
    }
}