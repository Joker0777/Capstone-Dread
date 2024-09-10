using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    [SerializeField] protected Button _menuButton;
    [SerializeField] protected Button _quitButton;

    private EventManager _eventManager;

    private bool _isPaused;

    private void Awake()
    {
        _eventManager = EventManager.Instance;
    }
    private void Start()
    {
        _menuButton.onClick.AddListener(OnPlayButtonClicked);
        _quitButton.onClick.AddListener(OnQuitButtonClicked);

        this.gameObject.SetActive(false);
    }


    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

}
