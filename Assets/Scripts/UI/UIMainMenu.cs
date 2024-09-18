using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] SceneTransitionManager sceneTransitionManager;

    [SerializeField] Button _startButton;

    [SerializeField] Button _quitButton;

    [SerializeField] Button _controls;

    [SerializeField] GameObject _controlInfo;

    private bool _controlInfoOn;


    private void Start()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _quitButton.onClick.AddListener(OnQuitButtonClicked);
        _controls.onClick.AddListener(OnControlsButtonClicked);

        _controlInfo.SetActive(false);

        sceneTransitionManager = SceneTransitionManager.Instance;
    }

    private void OnStartButtonClicked()
    {
        if(sceneTransitionManager == null)
        {
            sceneTransitionManager = SceneTransitionManager.Instance;
        }
        sceneTransitionManager.LoadFirstScene();
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void OnControlsButtonClicked()
    {
        if (!_controlInfoOn)
        {
            _controlInfo.SetActive(true);
            _controlInfoOn = true;
        }
        else
        {
            _controlInfo.SetActive(false);
            _controlInfoOn = false;
        }
    }
}
