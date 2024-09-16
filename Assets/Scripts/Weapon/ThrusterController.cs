using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterController : MonoBehaviour
{
    private Animator _animator;

    //[SerializeField] private EventManager _eventManager;
    [SerializeField] private string _thrusterParticleEffectTag;
    [SerializeField] private float _thrusterParticleScale;

    private EventManager _eventManager;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _eventManager = EventManager.Instance;
    }

    private void OnEnable()
    {
        if (_eventManager == null)
        {
            _eventManager = EventManager.Instance;
        }

     // _eventManager.IsThrusting += ThrusterAnimation;

    }

    private void OnDisable()
    {
        if (_eventManager != null)
        {
        //  _eventManager.IsThrusting -= ThrusterAnimation;
        }
    }

    private void OnDestroy()
    {
        if (_eventManager != null)
        {
       //   _eventManager.IsThrusting -= ThrusterAnimation;
        }
    }

    private void ThrusterAnimation(bool thrustingState)
    {
        _animator.SetBool("IsThrusting", thrustingState);
        if (thrustingState)
        {
            _eventManager?.OnPlayParticleEffect?.Invoke(_thrusterParticleEffectTag, transform.position, _thrusterParticleScale);
            _eventManager?.OnPlaySoundEffect?.Invoke(_thrusterParticleEffectTag + "Effect", transform.position);
        }
    }


}
