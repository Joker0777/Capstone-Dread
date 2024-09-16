using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovment : MonoBehaviour
{
    private float _startPos;
    [SerializeField] private float _length;
    private GameObject _camera;

    [SerializeField]private float _parallaxEffect;




    void Start()
    {
        _camera = GameObject.Find("Virtual Camera");
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float temp = (_camera.transform.position.x * (1 - _parallaxEffect));
        float distance = (_camera.transform.position.x * _parallaxEffect);

        transform.position = new Vector3(_startPos + distance, transform.position.y, transform.position.z);

        // Fix the boundary condition
        if (temp > _startPos + _length)
        {
            _startPos += _length;
        }
        else if (temp < _startPos - _length) // Fix comparison to avoid flicker
        {
            _startPos -= _length;
        }

    }
}
