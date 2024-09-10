using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    private float _angleToTurn;
    [SerializeField]private float _rotationSpeed = 90f;

    public void RotateTurretToTarget(Vector2 targetDirection)
    {
        _angleToTurn = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,_angleToTurn-90f), _rotationSpeed *Time.deltaTime);
    }
    
}
