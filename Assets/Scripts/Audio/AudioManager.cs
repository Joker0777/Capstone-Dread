using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private int audioSystemPoolSize;
    [SerializeField] private Transform _audioEffectParent;
    [SerializeField] private AudioEffect[] _audioEffectPrefabs;

    private EventManager _eventManager;

    private void Awake()
    {
        _eventManager = EventManager.Instance;
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeSoundEffectPools();
    }

    private void OnEnable()
    {
        _eventManager.OnPlaySoundEffect += GetAudioEffect;
    }

    private void OnDisable()
    {
        _eventManager.OnPlaySoundEffect -= GetAudioEffect;
    }

    public void GetAudioEffect(string audioEffectTag, Vector2 position)
    {
        AudioEffect audioEffect = ObjectPoolSystem<AudioEffect>.Instance.GetObject(audioEffectTag);

        if (audioEffect != null)
        {
            audioEffect.PlaySoundEffect(position);
           // StartCoroutine(ReturnToPoolAfterPlaying(audioEffect));
        }
    }

    private void InitializeSoundEffectPools()
    {
        foreach (var pool in _audioEffectPrefabs)
        {
            ObjectPoolSystem<AudioEffect>.Instance.AddPool(audioSystemPoolSize, pool.AudioEffectTag, pool, _audioEffectParent);
        }
    }

    private IEnumerator ReturnToPoolAfterPlaying(AudioEffect audioEffect)
    {
        yield return new WaitForSeconds(audioEffect.GetComponent<AudioSource>().clip.length);
        ObjectPoolSystem<AudioEffect>.Instance.ReturnObject(audioEffect.AudioEffectTag, audioEffect);
    }
}
