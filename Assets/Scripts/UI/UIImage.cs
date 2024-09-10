using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImage : UIElement
{
    [SerializeField] private List<UIImageElement> uIImageElements;

    private UIImageElement _currentImageElement;
    private Image _currentImage;


    private void Awake()
    {
        _currentImage = GetComponent<Image>();
    }

    private void SetImage(string type)
    {
        if (uIImageElements != null && uIImageElements.Count > 0)
        {
            foreach (var UIImage in uIImageElements)
            {
                if (UIImage.Type == type)
                {
                    _currentImage.sprite = UIImage.Sprite;
                    break;
                }
            }
        }
    }

    public override void SetUI(string imageType)
    {
      SetImage(imageType);
    }
}
