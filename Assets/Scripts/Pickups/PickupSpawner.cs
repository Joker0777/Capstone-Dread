using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{



    [SerializeField] PickUp[] pickUps;

    public List<GameObject> spawnedPickUps;

    private EventManager eventManager;

    public List<GameObject> SpawnedPickUps
    {
        get { return spawnedPickUps; }
    }

    private void Awake()
    {
        eventManager = EventManager.Instance;
    }
 

    private void OnEnable()
    {
       // eventManager.OnUnitDestroyed += SpawnPickup;
    }

    private void OnDisable()
    {
      //  eventManager.OnUnitDestroyed -= SpawnPickup;
    }

    private void SpawnPickup(CharacterType unitType, Vector3 position)
    {
       // if(unitType == UnitType.Enemy) 
        {
         //   int randomIndex = Random.Range(0, pickUps.Length);
         //   PickUp currentPickup;

            // spawnedPickUps.Add(Instantiate<PickUp>(pickUps[randomIndex], position, Quaternion.identity));
         //   currentPickup = Instantiate(pickUps[randomIndex], position, Quaternion.identity);

          //  if(currentPickup != null) 
         //   { 
         //       spawnedPickUps.Add(currentPickup.gameObject);
         //   }
        }
    }

}
