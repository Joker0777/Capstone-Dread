using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHud : MonoBehaviour
{
    private UIElement[] _uiElements;

    [SerializeField] EventManager _eventManager;

    private void Awake()
    {
        _uiElements = GetComponentsInChildren<UIElement>();    
    }

    private void OnEnable()
    {
        _eventManager.OnUIChange += UpdateUI;   
    }

    private void OnDisable()
    {
        _eventManager.OnUIChange -= UpdateUI;
    }

    private void UpdateUI(UIElementType elementType, string UIText)
    {       
        for(int i = 0; i< _uiElements.Length; i++) 
        {
            if (_uiElements[i].ElementType == elementType)
            {
                _uiElements[i].SetUI(UIText);
            }
        }
    }
}
