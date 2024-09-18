using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParticleSystemManager : MonoBehaviour
{
    [SerializeField] private int particleSystemPoolSize;
    [SerializeField] private Transform particleSystemParent;
    [SerializeField] private ParticleEffect[] _particleEffectPrefabs;

    private EventManager _eventManager;

    private void Awake()
    {
        _eventManager = EventManager.Instance;
    }

    private void Start()
    {
        InitializeParticlePools();
    }

    private void OnEnable()
    {
        _eventManager.OnPlayParticleEffect += GetParticleEffect;
    }

    private void OnDisable()
    {
        _eventManager.OnPlayParticleEffect -= GetParticleEffect;
    }

    public void GetParticleEffect(string particleEffectTag, Vector2 position, float scale)
    {
        ParticleEffect particleEffect = ObjectPoolSystem<ParticleEffect>.Instance.GetObject(particleEffectTag);
        particleEffect?.PlayParticleEffect(position, scale);
    }

    public void InitializeParticlePools()
    {
        foreach (var pool in _particleEffectPrefabs)
        {
            ObjectPoolSystem<ParticleEffect>.Instance.AddPool(particleSystemPoolSize, pool.ParticleSystemTag, pool, particleSystemParent);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeParticlePools();
    }
}
