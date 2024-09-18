using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    [SerializeField] protected Button _menuButton;

    private GameManager _gameManager;

    private void Start()
    {
        _menuButton.onClick.AddListener(OnPlayButtonClicked);
        _gameManager = GameManager.Instance;

        this.gameObject.SetActive(false);
    }


    private void OnPlayButtonClicked()
    {
        _gameManager.ReturnToMainMenu();
    }

}
