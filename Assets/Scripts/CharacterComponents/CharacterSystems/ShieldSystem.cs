using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldSystem : MonoBehaviour
{

    [SerializeField] private float _initialTrasparency = 0f;
    [SerializeField] private float _endTransparency = 1f;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private int _shieldDamage = 20;

    [SerializeField] private LayerMask _damageLayer;
    [SerializeField] private string _targetTag;

    private SpriteRenderer _shieldSprite;
    private Color _endColor;
    private EventManager _eventManager;

    

    protected void Awake()
    {
        _shieldSprite = GetComponent<SpriteRenderer>();
        _endColor = _shieldSprite.color;
        SetTransparencyColor(0f, _endColor);
        _eventManager = EventManager.Instance;
    }

 
    public void ActivateShield()
    {
        StopAllCoroutines();
        SetTransparencyColor(_initialTrasparency, _endColor);
        StartCoroutine(FadeShield(_initialTrasparency, _endTransparency, false)); ;
    }

    public void DisableShield()
    {
        StopAllCoroutines();
        SetTransparencyColor(_endTransparency, _endColor);
        StartCoroutine(FadeShield(_endTransparency, _initialTrasparency, true));
    }


    IEnumerator FadeShield(float initialTransparency, float endTransparency, bool disable)
    {     
        float currentTimer = 0;
   
        while(currentTimer < _fadeDuration)
        {
            float transparency = Mathf.Lerp(initialTransparency, endTransparency, currentTimer / _fadeDuration);
              
            SetTransparencyColor(transparency, _endColor);

            currentTimer += Time.deltaTime;
            yield return null;
        }

        SetTransparencyColor(endTransparency, _endColor);

        if (disable) 
        {
            gameObject.SetActive(false);
        }
 
    }

    private void SetTransparencyColor(float transparency, Color color)
    {
        Color newColor = color;
        newColor.a = transparency;
        _shieldSprite.color = newColor;      
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 shieldHit = collision.contacts[0].point;

        if (collision.gameObject.CompareTag(_targetTag) || 1 << ((collision.gameObject.layer) & _damageLayer) != 0)
        {
            collision.collider?.attachedRigidbody?.GetComponent<IDamagable>().DamageTaken(_shieldDamage);

        }
        _eventManager.OnPlayParticleEffect?.Invoke("Shield", (Vector2)shieldHit, .5f);
        _eventManager.OnPlaySoundEffect?.Invoke("ShieldHitEffect", (Vector2)shieldHit);
    }
}
