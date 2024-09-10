using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private PickupType _pickupType;
    [SerializeField] string unitTag = "PlayerTarget";
    [SerializeField] float _pickupTimerLength = 15f;
    [SerializeField] WeaponMod mod;
    [SerializeField] ProjectileWeapon weapon;
    [SerializeField] int _healthAmount;
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

        Character character = collision.GetComponentInParent<Character>();
      //  Debug.Log(character);
        if (character != null && character.CompareTag(unitTag))
        {
            PickUpManager.instance.CollectPickup(character, this, mod, weapon, _healthAmount);
            Destroy(gameObject);
        }       
    }
}
