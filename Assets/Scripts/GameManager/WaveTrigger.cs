using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    [SerializeField] private int _waveIndex;
    [SerializeField] private string _triggerTag;

    private bool _triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(_triggerTag) && !_triggered)
        {
            _triggered = true;  
            GameManager.Instance.StartWaveFromTrigger(_waveIndex);
        }
    }
}
