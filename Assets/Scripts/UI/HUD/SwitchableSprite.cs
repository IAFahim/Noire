using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SwitchableSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private Image image;
    private Vector2 originalSizeDelta = new (100, 100);
    private Vector2 scaledSizeDelta = new (200, 200);

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    
    // switch to the `index` sprite
    public void Switch(int index)
    {
        if (index < sprites.Length)
        {
            // scale up the icon for 3.. really shouldn't be doing this but ok
            if (index == 3)
                image.rectTransform.sizeDelta = scaledSizeDelta;
            else
                image.rectTransform.sizeDelta = originalSizeDelta;
            image.sprite = sprites[index];
        }
    }
}