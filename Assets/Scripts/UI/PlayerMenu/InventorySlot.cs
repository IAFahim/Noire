using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Image sprite;
    [SerializeField] private TextMeshProUGUI stackCount;
    [SerializeField] private ButtonUI inventorySlotButton;

    private string currItemDescription;

    private void Awake()
    {
        ToggleEnable(false);
    }

    private void Start()
    {
        inventorySlotButton.AddListener(OnInventorySlotClicked);
    }

    public void Display(CollectableItemSO newItem, int count)
    {
        ToggleEnable(true);
        
        sprite.sprite = newItem.Sprite;
        currItemDescription = newItem.Description;
        if (count > 1)
            stackCount.text = count.ToString();
        else
            stackCount.enabled = false;
    }

    public void Clear()
    {
        ToggleEnable(false);
    }

    private void ToggleEnable(bool val)
    {
        sprite.sprite = defaultSprite;
        stackCount.enabled = val;
    }

    private void OnInventorySlotClicked()
    {
        InventoryMenu.Instance.SetDescriptionText(currItemDescription);
        InventoryMenu.Instance.ToggleDescriptionText(true);
    }
}