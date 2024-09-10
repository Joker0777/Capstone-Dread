using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIText : UIElement
{
     private TextMeshProUGUI _textUI;
     private string _updatedUIText = "";

      public string UpdatedUIText
      {
         set 
         { 
             _updatedUIText = value;
             _textUI.text = $"_ {UpdatedUIText}";
         }
       get { return _updatedUIText; }
      }


    private void Awake()
    {
        _textUI = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        _textUI.text = $"_ {UpdatedUIText}";
    }

    public override void SetUI(string elementText)
    {
        UpdatedUIText = elementText;
    }
}
