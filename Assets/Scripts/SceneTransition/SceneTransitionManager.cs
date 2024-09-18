using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private string _mainMenu; 
    [SerializeField] private string[] _scenes; 

    private int _currentSceneIndex = -1;
    private EventManager _eventManager;

    public static SceneTransitionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
 
    }

    private void Start()
    {
        if (_scenes == null || _scenes.Length == 0)
        {
            Debug.LogError("No scenes assigned.");
            return;
        }
        _eventManager = EventManager.Instance;
        _currentSceneIndex = 0;
    }

    public void LoadNextScene()
    {
        _currentSceneIndex++;
        if (_currentSceneIndex < _scenes.Length)
        {
            _eventManager.OnGameSceneStart?.Invoke();
            SceneManager.LoadScene(_scenes[_currentSceneIndex]);

        }
        else
        {
            LoadMainMenu(); 
        }
    }

    public void LoadMainMenu()
    {
        _eventManager.OnGameEnd?.Invoke();
        SceneManager.LoadScene(_mainMenu);
        _currentSceneIndex = -1;

    }

    public void ResetCurrentScene()
    {
        if (_currentSceneIndex >= 0 && _currentSceneIndex < _scenes.Length)
        {
            _eventManager.OnGaameRestart?.Invoke();
            SceneManager.LoadScene(_scenes[_currentSceneIndex]);
        }
    }

    public void LoadFirstScene()
    {
        _currentSceneIndex = 0;
        SceneManager.LoadScene(_scenes[_currentSceneIndex]);
    }
}
