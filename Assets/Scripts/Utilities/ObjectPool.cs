using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private List<T> _objectPool;
    private T _object;
    private Transform _parentTransform;
    private int _poolSize;
    private string _objectTag;

    public string ObjectTag { get { return _objectTag; } }
    public int CurrentPoolSize { get { return _objectPool.Count; } }

    public ObjectPool(T obj, int poolSize, Transform parentTransform, string tag)
    {
        _object = obj;
        _parentTransform = parentTransform;
        _poolSize = poolSize;
        _objectTag = tag;

        CreatePool();
    }

    public T GetObject()
    {
        foreach (T obj in _objectPool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }
        T newObject = CreateObject(true);
        return newObject;
    }

    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    public void DeactivateAll()
    {
        foreach (T obj in _objectPool)
        {
            obj.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        for (int i = _objectPool.Count - 1; i >= 0; i--)
        {
            if (_objectPool[i] != null)
            {
                Object.Destroy(_objectPool[i].gameObject);
            }
        }
        _objectPool.Clear();
    }

    public void CreatePool()
    {
        _objectPool = new List<T>();

        for (int i = 0; i < _poolSize; i++)
        {
            CreateObject(false);
        }
    }

    private T CreateObject(bool isActive)
    {
        T nextObject = Object.Instantiate(_object, _parentTransform);
        nextObject.gameObject.SetActive(true);
        _objectPool.Add(nextObject);
        nextObject.gameObject.SetActive(isActive);
        return nextObject;
    }
}
