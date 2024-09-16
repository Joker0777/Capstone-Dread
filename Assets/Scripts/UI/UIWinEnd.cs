using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIWinEnd : MonoBehaviour
{
    [SerializeField] protected Button _menuButton;
    [SerializeField] protected Button _quitButton;

    protected virtual void Start()
    {
        _menuButton.onClick.AddListener(OnPlayButtonClicked);
        _quitButton.onClick.AddListener(OnQuitButtonClicked);

        this.gameObject.SetActive(false);
    }

 
    protected virtual void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    protected void OnQuitButtonClicked()
    {
        Application.Quit();
    }

  
}
