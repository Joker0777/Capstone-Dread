using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class UIElement : MonoBehaviour
{
    [SerializeField] protected UIElementType elementType;

    public UIElementType ElementType
    {
        get { return elementType; }
    }

    public abstract void SetUI(string elementText);
}
