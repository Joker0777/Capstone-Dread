using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PickUp : MonoBehaviour
{
    [SerializeField] private PickupType _pickupType;
    [SerializeField] string unitTag = "PlayerTarget";
    [SerializeField] float _pickupTimerLength = 15f;
    [SerializeField] WeaponMod mod;
    [SerializeField] ProjectileWeapon weapon;
    [SerializeField] int _healthAmount;
    [SerializeField] int _ammoAmount;
    private Timer _pickUpTimer;


    public PickupType PickupType {  get { return _pickupType; } }

    private void Awake()
    {
        _pickUpTimer = new Timer(_pickupTimerLength);
    }


    private void Start()
    {
        _pickUpTimer.StartTimerBasic();
    }

    private void Update()
    {
        _pickUpTimer.UpdateTimerBasic(Time.deltaTime);

        if (!_pickUpTimer.IsRunningBasic())
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision == null) return;

        PlayerCharacter character = collision.GetComponentInParent<PlayerCharacter>();
      //  Debug.Log(character);
        if (character != null && character.CompareTag(unitTag))
        {
            PickUpManager.instance.CollectPickup(character, this, mod, weapon, _healthAmount, _ammoAmount);
            Destroy(gameObject);
        }       
    }
}
