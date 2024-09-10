using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParticleEffect : MonoBehaviour
{
    private ParticleSystem[] _particleSystems;

    private float _maxDuration;
    private Timer _timer;
    [SerializeField]private string _particleSystemTag;

    public string ParticleSystemTag
    {
        get { return _particleSystemTag; }
    }
    private void Awake()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
       // GetParticleSystemPlayTime();
  
    }


    public void PlayParticleEffect(Vector3 position, float scale)
    {
        transform.position = position;
 
        
        gameObject.SetActive(true);
        foreach (var ps in _particleSystems)
        {
            ps.transform.localScale = Vector3.one * scale;
            ps.Play();
        }
        StartCoroutine(Deactivate());
    }

    private void GetParticleSystemPlayTime()
    {
        foreach (var ps in _particleSystems)
        {
            if (ps.main.duration > _maxDuration)
            {
                _maxDuration = ps.main.duration;
            }
        }
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(_particleSystems[0].main.duration);
        gameObject.SetActive(false);
    }
}
