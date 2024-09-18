using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    private EventManager _eventManager;
    [SerializeField] private PlayerCharacter _playerPrefab;

    [SerializeField] private GameObject _missionCompleteScreen;
    [SerializeField] private GameObject _missionFailScreen;
    [SerializeField] private GameObject _nextWaveWarning;
    [SerializeField] private UIPause _pauseMenu;

    [SerializeField] private List<EnemyWave> _enemyWaves;
    [SerializeField] private float _nextWaveDelay;

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    private SceneTransitionManager sceneTransitionManager;
    private int _totalWaves;
    private int _currentWaveIndex = -1;
    private List<GameObject> _allEnemies = new List<GameObject>();
    public PlayerCharacter _currentPlayer;
    private bool _isPaused;

    private void Awake()
    {
        sceneTransitionManager = SceneTransitionManager.Instance;
        _eventManager = EventManager.Instance;
        if (Instance == null)
        {
            Instance = this;
          //  DontDestroyOnLoad(gameObject); // Persist GameManager between scenes
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
       // _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (sceneTransitionManager == null)
        {
            sceneTransitionManager = SceneTransitionManager.Instance;
        }

        _totalWaves = _enemyWaves.Count;
        Time.timeScale = 1f;
        _currentPlayer = FindObjectOfType<PlayerCharacter>();



        if (_currentPlayer != null)
        {
            _virtualCamera.Follow = _currentPlayer.transform;
        }

        SpawnPlayerAtLevelStart();

    }

    private void OnEnable()
    {
        _eventManager.OnCharacterDestroyed += CharacterDestroyed;
        _eventManager.OnPauseGame += PauseGame;
    }

    private void OnDisable()
    {
        _eventManager.OnCharacterDestroyed -= CharacterDestroyed;
        _eventManager.OnPauseGame -= PauseGame;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _eventManager.OnPauseGame.Invoke();
        }
    }

    public void CompletedScene()
    {
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.LoadNextScene();
        }
        else
        {
            Debug.LogError("SceneTransitionManager is not assigned in CompletedScene.");
        }
    }

    public void PauseGame()
    {
        if (!_isPaused)
        {
            _pauseMenu.gameObject.SetActive(true);
            if (_currentPlayer != null) _currentPlayer.gameObject.SetActive(false); // Ensure the player exists
            Time.timeScale = 0f;
            _isPaused = true;
        }
        else
        {
            ResumeGame();
        }
    }

    private void ResumeGame()
    {
        _pauseMenu.gameObject.SetActive(false);
        if (_currentPlayer != null) _currentPlayer.gameObject.SetActive(true);
        Time.timeScale = 1f;
        _isPaused = false;
    }


    public void ReturnToMainMenu()
    {

        ClearPlayer();

        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.LoadMainMenu();
        }
        else
        {
            Debug.LogError("SceneTransitionManager is not assigned in ReturnToMainMenu.");
        }

        Time.timeScale = 1f; 
    }

    public void CharacterDestroyed(CharacterType characterType, Character character, Vector3 position)
    {
        Debug.Log("In character destroyed");
        if (characterType == CharacterType.Enemy)
        {
            _allEnemies.Remove(character.gameObject);
            _eventManager.OnUIChange.Invoke(UIElementType.WaveEnemies, _allEnemies.Count.ToString());
        }
        else if (characterType == CharacterType.Player)
        {
            GameOver();
        }

        if (_allEnemies.Count == 0 && _currentWaveIndex == _enemyWaves.Count - 1)
        {
            CompleteLevel();
        }
    }

    private void SpawnEnemy(Character enemy, Transform spawnPoint)
    {
        Character newEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);
        _allEnemies.Add(newEnemy.gameObject);
    }

    public void StartWaveFromTrigger(int waveIndex)
    {
        if (waveIndex < _enemyWaves.Count)
        {
            StartCoroutine(StartNextWave(waveIndex));
        }
    }

    private IEnumerator StartNextWave(int waveIndex)
    {
        _allEnemies.RemoveAll(character => character == null);
        _eventManager.OnUIChange.Invoke(UIElementType.WaveEnemies, _allEnemies.Count.ToString());

        _nextWaveWarning.SetActive(true);
        yield return new WaitForSeconds(_nextWaveDelay);
        _nextWaveWarning.SetActive(false);

        List<EnemyCharacter> newWave = _enemyWaves[waveIndex].GetEnemyUnits();

        foreach (var enemy in newWave)
        {
            if (enemy._enemy != null)
            {
                SpawnEnemy(enemy._enemy, enemy._spawnPoint);
            }
        }
        _eventManager.OnUIChange.Invoke(UIElementType.WaveEnemies, _allEnemies.Count.ToString());

        _currentWaveIndex = waveIndex;
        yield return new WaitUntil(() => _allEnemies.Count == 0 || waveIndex == _enemyWaves.Count - 1);

        if (_allEnemies.Count == 0 && _currentWaveIndex == _enemyWaves.Count - 1)
        {
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        _missionCompleteScreen.SetActive(true);

       // if (_currentPlayer != null) _currentPlayer.gameObject.SetActive(false); // Ensure player exists
    }

    private void GameOver()
    {
        Debug.Log("In game over");
        _missionFailScreen.SetActive(true);
        ClearPlayer();
    }

    public void StartGame()
    {
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.LoadFirstScene(); // Ensure this method is defined in SceneTransitionManager
        }
        else
        {
            Debug.LogError("SceneTransitionManager is not assigned in StartGame.");
        }
    }

    public void RestartLevel()
    {
        _currentWaveIndex = -1;
        sceneTransitionManager = SceneTransitionManager.Instance;

        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.ResetCurrentScene(); // Ensure this method reloads the current level
        }
        else
        {
            Debug.LogError("SceneTransitionManager is not assigned in RestartLevel.");
        }

        StartCoroutine(WaitAndSpawnPlayer());
    }
    private IEnumerator WaitAndSpawnPlayer()
    {
        // Wait for the scene to reload
        yield return new WaitForSeconds(0.1f); // Small delay to ensure scene is reloaded

        SpawnPlayerAtLevelStart();
    }


    public void SpawnPlayerAtLevelStart()
    {
        Vector3 startPos = Vector3.zero;

        if (_currentPlayer != null)
        {
           // _currentPlayer.gameObject.SetActive(true);
            _currentPlayer.transform.position = startPos;
            _virtualCamera.Follow = _currentPlayer.transform;

        }
        else
        {
            SpawnPlayer(startPos);
        }

        if(_virtualCamera == null)
        {
            _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }
    }

    private void SpawnPlayer(Vector3 pos)
    {
        if (_currentPlayer != null) Destroy(_currentPlayer.gameObject); // Ensure no duplicate player
        _currentPlayer = Instantiate(_playerPrefab, pos, Quaternion.identity);
        PickUpManager.instance.PlayerCharacter = _currentPlayer;
        _virtualCamera.Follow = _currentPlayer.transform;
    }

    public void ClearPlayer()
    {
        if (_currentPlayer != null)
        {
            Destroy(_currentPlayer.gameObject); // Clear player when returning to main menu or resetting level
            _currentPlayer = null;
        }
    }

}
