using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPoolSystem<T> where T : MonoBehaviour
{
    private Dictionary<string, ObjectPool<T>> _objectPools;

    private static ObjectPoolSystem<T> _instance;

    public static ObjectPoolSystem<T> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ObjectPoolSystem<T>();
            }
            return _instance;
        }
    }

    public ObjectPoolSystem()
    {
        _objectPools = new Dictionary<string, ObjectPool<T>>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetObjectPools();
    }

    private void ResetObjectPools()
    {
        foreach (var objectPool in _objectPools.Values)
        {
            objectPool.Clear();
        }

        foreach (var objectPool in _objectPools.Values)
        {
            objectPool.CreatePool();
        }
    }

    public void AddPool(int size, string name, T obj, Transform parent)
    {
        if (obj != null && !_objectPools.ContainsKey(name))
        {
            _objectPools.Add(name, new ObjectPool<T>(obj, size, parent, name));
        }
    }

    public void RemovePool(string name)
    {
        if (_objectPools.TryGetValue(name, out ObjectPool<T> _))
        {
            _objectPools.Remove(name);
        }
    }

    public T GetObject(string type)
    {
        if (_objectPools.TryGetValue(type, out ObjectPool<T> value))
        {
            return value.GetObject();
        }
        return null;
    }

    public void DeactivateObjects(string type)
    {
        if (_objectPools.TryGetValue(type, out ObjectPool<T> value))
        {
            value.DeactivateAll();
        }
    }

    public void ReturnObject(string type, T obj)
    {
        if (_objectPools.TryGetValue(type, out ObjectPool<T> pool))
        {
            pool.ReturnObject(obj);
        }
    }
}
