using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterSystems : MonoBehaviour
{
    protected Character character;

    protected EventManager _eventManager;

    protected virtual void Awake()
    {
        _eventManager = EventManager.Instance;
        if (!transform.TryGetComponent<Character>(out character))
        {
            Debug.Log("Unit not assigned to parent.");
        }      
    }

    protected virtual void Start()
    {

        if (_eventManager == null)
        {
            Debug.Log("EventManager is not assigned.");
        }
    }

}
