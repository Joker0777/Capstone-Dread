using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPositonAdjust : MonoBehaviour
{
    public Transform lowerSpine;   // Reference to the lower spine transform
    public Transform gun;          // Reference to the gun transform
    public Vector3 gunOffset;      // Offset to maintain the gun's relative position
    public float damping = 5.0f;   // The speed of smoothing

    private Vector3 initialGunPosition;  // To store the initial local position of the gun
    private Vector3 targetPosition;      // To store the target position for the gun

    public Vector3 GunOffSet { get { return gunOffset; } set { gunOffset = value; } }

    void Start()
    {
        // Store the initial local position of the gun
        initialGunPosition = gun.localPosition;
    }

    void Update()
    {
        // Calculate the target position based on the lower spine's local position
        float spineY = lowerSpine.localPosition.y;
        targetPosition = new Vector3(initialGunPosition.x, initialGunPosition.y + spineY + gunOffset.y, initialGunPosition.z);

        // Smoothly interpolate towards the target position using damping
        gun.localPosition = Vector3.Lerp(gun.localPosition, targetPosition, Time.deltaTime * damping);
    }

}
