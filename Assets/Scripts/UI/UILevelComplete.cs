using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevelComplete : MonoBehaviour
{
    [SerializeField] protected Button _menuButton;
    [SerializeField] protected Button _nextButton;

    public bool _nextLLevelNotCompleted;

    private SceneTransitionManager _sceneTransitionManager;

    protected virtual void Start()
    {
        _menuButton.onClick.AddListener(OnMenuButtonClicked);
        _nextButton.onClick.AddListener(OnNextLevelClicked);
        _sceneTransitionManager = SceneTransitionManager.Instance;
        this.gameObject.SetActive(false);
       
    }

 
    protected virtual void OnMenuButtonClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    protected void OnNextLevelClicked()
    {
        if (_nextLLevelNotCompleted)
        {
            OnMenuButtonClicked();
        }
        else
        {
            GameManager.Instance.CompletedScene();
        }

    }

  
}
