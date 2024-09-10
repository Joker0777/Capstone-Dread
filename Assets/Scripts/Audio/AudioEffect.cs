using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffect : MonoBehaviour
{
    private AudioSource _audioSource;

 
    [SerializeField] private string _audioEffectTag;

    public string AudioEffectTag
    {
        get { return _audioEffectTag; }
    }
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }


    public void PlaySoundEffect(Vector3 position)
    {
        transform.position = position;

       // Debug.Log("In Play Sound effect effect tag is " + _audioEffectTag);


        gameObject.SetActive(true);
        _audioSource.Play();
        StartCoroutine(Deactivate());
    }



    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(_audioSource.clip.length);
        gameObject.SetActive(false);
    }
}
