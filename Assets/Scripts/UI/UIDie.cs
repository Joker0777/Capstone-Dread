using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIDie : MonoBehaviour
{
    [SerializeField] protected Button _menuButton;
    [SerializeField] protected Button _restartButton;

    private SceneTransitionManager _sceneTransitionManager;


    protected virtual void Start()
    {
        _menuButton.onClick.AddListener(OnMenuButtonClicked);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
        _sceneTransitionManager = SceneTransitionManager.Instance;
        this.gameObject.SetActive(false);
    }


    protected virtual void OnMenuButtonClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    protected void OnRestartButtonClicked()
    {
        GameManager.Instance.RestartLevel();
        this.gameObject.SetActive(false);
    }
}
