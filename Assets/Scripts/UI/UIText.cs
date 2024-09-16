using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIText : UIElement
{
     private TextMeshProUGUI _textUI;
     private string _updatedUIText = "";
    [SerializeField] private string _initialText = "_";

      public string UpdatedUIText
      {
         set 
         { 
             _updatedUIText = value;
             _textUI.text = $"{_initialText} {UpdatedUIText}";
         }
       get { return _updatedUIText; }
      }


    private void Awake()
    {
        _textUI = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        _textUI.text = $"{_initialText} {UpdatedUIText}";
    }

    public override void SetUI(string elementText)
    {
        UpdatedUIText = elementText;
    }
}
