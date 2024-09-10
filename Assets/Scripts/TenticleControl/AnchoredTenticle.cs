using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchoredTenticle : MonoBehaviour
{
    [SerializeField] private int _length;                    // Number of segments in the tentacle
    [SerializeField] private LineRenderer lineRenderer;      // LineRenderer to visualize the tentacle
    [SerializeField] private Transform[] _bodyParts;         // Array of transforms for each body part in the tentacle
    [SerializeField] private float _targetDistance;          // Maximum distance between each segment
    [SerializeField] private float _smoothSpeed;             // Smooth damping speed for segment movement

    [SerializeField] private Transform _targetDirection;     // Target transform the tentacle will follow

    [SerializeField] private RotateToTarget rotateToTarget;

    private Vector3[] _segments;                             // Positions of each segment
    private Vector3[] _segmentsV;                            // Velocity of each segment (used for smooth damping)

    void Start()
    {
        _length = _bodyParts.Length + 1;                    // Set the length to include body parts plus one for the target position
        lineRenderer.positionCount = _length;               // Set number of positions in the LineRenderer
        _segments = new Vector3[_length];                   // Initialize segment positions
        _segmentsV = new Vector3[_length];                  // Initialize segment velocities

        ResetPos();                                         // Set initial positions of segments
    }

    void Update()
    {
     

        // Get the position of the anchor (last segment)
        Vector3 anchorPosition = _segments[_length - 1];

        // Calculate the desired position of the head (first segment) based on the target
        Vector3 desiredPosition = _targetDirection.position;

        // Calculate the vector from the anchor to the desired position
        Vector3 anchorToDesired = desiredPosition - anchorPosition;

        // Clamp the desired position within the maximum length of the chain
        if (anchorToDesired.magnitude > _targetDistance * (_length - 1))
        {
            desiredPosition = anchorPosition + anchorToDesired.normalized * _targetDistance * (_length - 1);
        }

        // Set the position of the head segment
        _segments[0] = desiredPosition;

        // Update positions of the remaining segments
        for (int i = 1; i < _segments.Length; i++)
        {
            Vector3 previousSegment = _segments[i - 1];
            Vector3 currentSegment = _segments[i];

            // Calculate the target position for the current segment
            Vector3 targetPos = previousSegment + (currentSegment - previousSegment).normalized * _targetDistance;

            // Smoothly move the current segment towards the target position
            _segments[i] = Vector3.SmoothDamp(_segments[i], targetPos, ref _segmentsV[i], _smoothSpeed);

            // Position the body part at the corresponding segment
            if (i - 1 < _bodyParts.Length)
            {
                _bodyParts[i - 1].position = _segments[i];
            }
        }

        // Update the LineRenderer with the new segment positions
        lineRenderer.SetPositions(_segments);
    }

    private void ResetPos()
    {
        // Initialize segment positions starting from the target position
        _segments[0] = _targetDirection.position;

        for (int i = 1; i < _length; i++)
        {
            // Place segments at a fixed distance apart
            _segments[i] = _segments[i - 1] + _targetDirection.right * _targetDistance;
        }

        // Set the initial positions in the LineRenderer
        lineRenderer.SetPositions(_segments);
    }
}
