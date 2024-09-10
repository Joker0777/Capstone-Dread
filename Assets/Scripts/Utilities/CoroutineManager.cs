using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{

    private static CoroutineManager _instance;

    public static CoroutineManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject coroutineManager = new GameObject("CoroutineManager");
                _instance = coroutineManager.AddComponent<CoroutineManager>();
            }
            return _instance;
        }
    }

    public void Coroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
    
    public void StopRunningCoroutine(IEnumerator coroutine) 
    { 
        StopCoroutine(coroutine);
    }
}

