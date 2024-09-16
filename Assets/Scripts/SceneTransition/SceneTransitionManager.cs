using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private string[] _scenes;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;


    private int _currentSceneIndex = -1;
    private string _mainScene;
    private string _currentScene;
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
        _eventManager = EventManager.Instance;
    }

    private void Start()
    {
        if (_scenes == null || _scenes.Length == 0)
        {
            Debug.LogError("No scenes assigned.");
            return;
        }

        _mainScene = _scenes[0];
        _currentSceneIndex = 0;
        _currentScene = _mainScene;
        fadeCanvasGroup.alpha = 0f;
        Debug.Log(_mainScene);
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadNextSceneWithFade());
    }

    public void LoadMainScene()
    {

        _eventManager?.OnGameSceneStart();
        SceneManager.LoadScene(_mainScene);
        _currentSceneIndex = 0;
        _currentScene = _mainScene;
    }

    public void ResetCurrentScene()
    {
        StartCoroutine(ResetCurrentSceneWithFade());
    }

    private void UnloadCurrentScene()
    {
        if (_currentSceneIndex >= 0 && _currentSceneIndex < _scenes.Length)
        {
           SceneManager.UnloadSceneAsync(_scenes[_currentSceneIndex]);
        }
    }

    private IEnumerator LoadNextSceneWithFade()
    {
        yield return FadeOut();
 

         if (_currentSceneIndex >= 0)
          {
             UnloadCurrentScene();
         }
         _currentSceneIndex++;
       // _currentSceneIndex = 7;

        if (_currentSceneIndex < _scenes.Length)
        {
            _currentScene = _scenes[_currentSceneIndex];
            Debug.Log("Current Scene is " + _currentScene);
            SceneManager.LoadScene(_currentScene);
        }
        else
        {
            LoadMainScene();
            _currentSceneIndex = -1;
        }

        yield return FadeIn();
    }


    private IEnumerator ResetCurrentSceneWithFade()
    {
        yield return FadeOut();

        if (_currentSceneIndex >= 0 && _currentSceneIndex < _scenes.Length)
        {
            SceneManager.UnloadSceneAsync(_scenes[_currentSceneIndex]);
            SceneManager.LoadScene(_scenes[_currentSceneIndex]);
        }

        yield return FadeIn();
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }
}
