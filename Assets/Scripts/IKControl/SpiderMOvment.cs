using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMOvment : MonoBehaviour
{
    public Transform body; // Reference to the spider's body
    public Transform[] legEffectors; // IK effector targets for each leg
    public Vector3[] legRestOffsets; // Rest position offsets relative to the body for each leg
    public float stepDistance = 1f; // Distance the body needs to move to trigger a step
    public float stepSpeed = 2f; // Speed of leg movement
    public float stepOffset = 0.2f; // Delay between front and back legs stepping

    private Vector3 previousBodyPosition; // To store the last position of the body
    private bool[] isLegMoving; // To track if a leg is currently moving
    private int lastLegMoved = 0; // Track which leg moved last to alternate steps

    void Start()
    {
        isLegMoving = new bool[legEffectors.Length];
        previousBodyPosition = body.position; // Initialize previous body position

        // Initialize rest positions based on the offsets from the body
        for (int i = 0; i < legEffectors.Length; i++)
        {
            legEffectors[i].position = body.position + legRestOffsets[i];
        }
    }

    void Update()
    {
        // Check how far the body has moved since the last frame
        float bodyMoveDistance = Vector3.Distance(body.position, previousBodyPosition);

        // If the body has moved more than the step distance, trigger the next leg step
        if (bodyMoveDistance > stepDistance)
        {
            TriggerStep();

            // Update the previous body position for the next frame
            previousBodyPosition = body.position;
        }

        // Update rest positions to follow the body with their respective offsets
        for (int i = 0; i < legEffectors.Length; i++)
        {
            Vector3 newRestPosition = body.position + legRestOffsets[i];
            if (!isLegMoving[i] && Vector3.Distance(legEffectors[i].position, newRestPosition) > stepDistance)
            {
                legEffectors[i].position = newRestPosition; // Update the leg effector target to the new rest position
            }
        }
    }

    // Trigger the next leg to take a step
    void TriggerStep()
    {
        // Alternate leg steps by using the lastLegMoved variable
        int nextLeg = (lastLegMoved + 1) % legEffectors.Length;

        // Start moving the next leg if it's not already moving
        if (!isLegMoving[nextLeg])
        {
            float delay = (nextLeg % 2 == 0) ? 0f : stepOffset; // Delay for back legs
            StartCoroutine(MoveLeg(nextLeg, delay));
            lastLegMoved = nextLeg; // Update the last leg moved
        }
    }

    // Coroutine to move the leg to its rest position
    IEnumerator MoveLeg(int legIndex, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay); // Wait before the back leg starts moving
        }

        isLegMoving[legIndex] = true;

        Vector3 startPos = legEffectors[legIndex].position;
        Vector3 endPos = body.position + legRestOffsets[legIndex]; // Leg steps to new rest position relative to the body

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * stepSpeed;
            legEffectors[legIndex].position = Vector3.Lerp(startPos, endPos, time);
            yield return null;
        }

        isLegMoving[legIndex] = false; // Leg has finished moving
    }
}
