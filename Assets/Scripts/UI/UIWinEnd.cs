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
    [SerializeField] protected TextMeshProUGUI _highScoreText;

    private EventManager _eventManager;

    private void Awake()
    {
        _eventManager = EventManager.Instance;
    }
    protected virtual void Start()
    {
        _menuButton.onClick.AddListener(OnPlayButtonClicked);
        _quitButton.onClick.AddListener(OnQuitButtonClicked);

        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _eventManager.OnGetHighScore += SetHighScoreText;
    }

    private void OnDisable()
    {
        _eventManager.OnGetHighScore -= SetHighScoreText;
    }

    protected virtual void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    protected void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void SetHighScoreText(string text)
    {
        _highScoreText.text = "HighScore: " + text;
    }
}
