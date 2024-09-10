using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tenticle : MonoBehaviour
{
    [SerializeField] private int _length;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Vector3[] _segments;
    [SerializeField] private Vector3[] _segmentsV;

    [SerializeField] private Transform _targetDirection;
    [SerializeField] private float _targetDistance;
    [SerializeField] private float _smoothSpeed;

    [SerializeField] private float _wiggleSpeed = 1;
   [SerializeField] private float _wiggleMagnitude = 0.5f;
  //  [SerializeField] private Transform _wiggleDirection;

    [SerializeField] private Transform[] _bodyParts;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = _length;
        _segments = new Vector3[_length];
        _segmentsV = new Vector3[_length];

        ResetPos();
    }

    // Update is called once per frame
    void Update()
    {
       // _wiggleDirection.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * _wiggleSpeed) * _wiggleMagnitude);

     //   _segments[0] = _targetDirection.position;

     //   for(int i = 1; i < _segments.Length; i++)
     //   {
      //      Vector3 targetPos = _segments[i - 1] + (_segments[i] - _segments[i - 1]).normalized * _targetDistance;
      //      _segments[i] = Vector3.SmoothDamp(_segments[i], targetPos, ref _segmentsV[i], _smoothSpeed);
      //      _bodyParts[i - 1].transform.position = _segments[i];
      //  }
      //  lineRenderer.SetPositions(_segments);

        _segments[0] = _targetDirection.position;

        for (int i = 1; i < _segments.Length; i++)
        {
            Vector3 targetPos = _segments[i - 1] + (_segments[i] - _segments[i - 1]).normalized * _targetDistance;
            _segments[i] = Vector3.SmoothDamp(_segments[i], targetPos, ref _segmentsV[i], _smoothSpeed);

            // Apply a sine wave movement to the y position of the body parts
            Vector3 bodyPartPos = _segments[i];
            bodyPartPos.y += Mathf.Sin(Time.time * _wiggleSpeed + i) * _wiggleMagnitude;
            _bodyParts[i - 1].transform.position = bodyPartPos;
        }
        lineRenderer.SetPositions(_segments);
    }

    private void ResetPos()
    {
        _segments[0] = _targetDirection.position;
        for(int i =1; i< _length; i++)
        {
            _segments[i] = _segments[i-1] + _targetDirection.right * _targetDistance;
        }
        lineRenderer.SetPositions( _segments);
    }
}
