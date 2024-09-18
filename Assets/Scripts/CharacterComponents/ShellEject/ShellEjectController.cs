using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellEjectController : CharacterSystems
{
    [SerializeField] ParticleSystem _particleSystem;

    [SerializeField] private List<ShellMaterial> _shellMaterial;

    private float _fireRate;
    private ParticleSystemRenderer _shellRenderer;
    private string _currentMaterialName;

    public float FireRate { set { _fireRate = value; } }

    protected override void Start()
    {
        base.Start();

        if (_particleSystem != null)
        {
            _shellRenderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
        }
        else
        {
            Debug.Log("No particle system renderer");
        }

        if (_shellMaterial.Count > 0 && _shellRenderer != null)
        {
            _shellRenderer.material = _shellMaterial[0].material;
            _currentMaterialName = _shellRenderer.material.name;
        }
        else
        {
            Debug.LogWarning("No materials found or ParticleSystemRenderer is null.");
        }
    }

    private void OnEnable()
    {
        _eventManager.OnWeaponStoped += StopShellCasings;
        _eventManager.OnWeaponFired += FireShellCasings;
    }



    public void SwitchShellSprite(string shellMaterialName)
    {
        if (_shellRenderer != null)
        {
            ShellMaterial entry = _shellMaterial.Find(item => item.materialName == shellMaterialName);

            if (entry != null)
            {
                _shellRenderer.material = entry.material;
                _currentMaterialName = entry.material.name;
            }
            else
            {
                Debug.LogWarning("Material not found: " + shellMaterialName);
            }
        }
    }

    public void FireShellCasings(string shellTag, float cooldown)
    {
        if (_particleSystem == null) return;

        if(shellTag != _currentMaterialName) 
        { 
            SwitchShellSprite(shellTag);
        }

     //   var emission = _particleSystem.emission;
      //  emission.enabled = true;
      //  float emissionRate = cooldown > 0 ? 1f / cooldown : 0f;
     //   emission.rateOverTime = emissionRate;
      //  _particleSystem.Play();

        _particleSystem.Emit(1);
    }

    public void StopShellCasings()
    {
        if (_particleSystem == null) return;

        var emission = _particleSystem.emission;
        emission.enabled = false;

        _particleSystem.Stop();
    }
}
