using System;
using UnityEngine;



[CreateAssetMenu(menuName = "Event Manager")]
public class EventManager : ScriptableObject
{
    private static EventManager _instance;

    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<EventManager>("EventManager");
                if (_instance == null)
                {
                    Debug.LogError("EventManager instance not found. Make sure there is an EventManager asset in a Resources folder.");
                }
            }
            return _instance;
        }
    }

    // Health
    public Action<int> OnUnitHealthChanged;

    // Characer
    public Action<CharacterType, Character, Vector3> OnCharacterDestroyed;
    public Action<CharacterType, Character, Vector3> OnCharacterHurt;
    public Action<string, float> OnWeaponFired;
    public Action OnWeaponStoped;
   
    public Action<bool> IsFacingRight;
  
    // UI
    public Action<UIElementType, string> OnUIChange;

    // PickUps
    public Action<PickUp> OnPickupSpawned;

    // ParticleSystems
    public Action<string, Vector2, float> OnPlayParticleEffect;

    //Audio
    public Action<string, Vector2> OnPlaySoundEffect;

    //PlayerRespawn
   // public Action<Unit> OnPlayerRespawn;

    //Score
    public Action<string> OnScoreIncrease;
    public Action<string> OnGetHighScore;

    //Scene/GameEnd
    public Action OnGameSceneEnd;

    //Pause
    public Action OnPauseGame;

}
