using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    private Animator _animator;
    public string _poolTag;
    private Transform _gunBarrelPosition;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }
    private void LateUpdate()
    {
        if (_gunBarrelPosition != null)
        {

            transform.position = _gunBarrelPosition.position;
            transform.rotation = _gunBarrelPosition.rotation;
        }
    }

    private void OnEnable()
    {
        PlayMuzzleFlash();
    }
    public void SetBarrelPosition(Transform barrelPosion)
    {
        _gunBarrelPosition = barrelPosion;
    }

    private void PlayMuzzleFlash()
    {
        if (_animator != null)
        {
            _animator.Play(_poolTag); 
            StartCoroutine(DeactivateAfterAnimation());
        }
        else
        {
            Debug.LogWarning("Animator component is missing.");
            gameObject.SetActive(false); // Deactivate if no animator found.
        }
    }

    private IEnumerator DeactivateAfterAnimation()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

       _gunBarrelPosition = null;
       this.gameObject.SetActive(false);
    }
}
