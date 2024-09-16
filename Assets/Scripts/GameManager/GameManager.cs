using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private EventManager _eventManager;
    [SerializeField] Character _player;

    [SerializeField] private GameObject _missionCompleteScreen;
    [SerializeField] private GameObject _missionFailScreen;
    [SerializeField] private GameObject _nextWaveWarning;
    [SerializeField] private UIPause _pauseMenu;

    [SerializeField] private List<EnemyWave> _enemyWaves;
    [SerializeField] private float _nextWaveDelay;

    private int _totalWaves;
    private int _currentWaveIndex = -1;
    private List<GameObject> _allEnemies = new List<GameObject>();
    private Character _currentPlayer;
    private bool _isPaused;

    private void Awake()
    {
        _eventManager = EventManager.Instance;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _totalWaves = _enemyWaves.Count;
        Time.timeScale = 1f;
        // SpawnPlayer(transform.position); // Assuming you want to handle player spawning elsewhere
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

    public void PauseGame()
    {
        if (!_isPaused)
        {
            _pauseMenu.gameObject.SetActive(true);
            _currentPlayer.gameObject.SetActive(false);
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
        _currentPlayer.gameObject.SetActive(true);
        Time.timeScale = 1f;
        _isPaused = false;
    }

    public void CharacterDestroyed(CharacterType characterType, Character character, Vector3 position)
    {
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
        //_eventManager.OnUIChange.Invoke(UIElementType.WaveEnemies, newWave.Count.ToString());

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
        _eventManager.OnGameSceneEnd?.Invoke();
        _player.gameObject.SetActive(false);
    }

    private void GameOver()
    {
        _missionFailScreen.SetActive(true);
        _eventManager.OnGameSceneEnd?.Invoke();
        _player.gameObject.SetActive(false);
    }

    private void SpawnPlayer(Vector3 pos)
    {
        _currentPlayer = Instantiate(_player, pos, Quaternion.identity);
    }
}
