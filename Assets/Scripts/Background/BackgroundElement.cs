using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundElement : MonoBehaviour
{
    [SerializeField] private float _scrollSpeed;


    private void Start()
    {
        BackgroundTransformPosition = transform.position;
    }


    public float ScrollSpeed 
    {  
        get 
        { 
            return _scrollSpeed; 
        }   
    }

    public Vector3 BackgroundTransformPosition 
    {  
        get 
        { 
            return transform.position; 
        }
        set
        {
            transform.position = value;
        }
    }
}
