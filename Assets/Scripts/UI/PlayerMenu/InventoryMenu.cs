using TMPro;
using UnityEngine;

public class InventoryMenu : UI
{
    public static InventoryMenu Instance { get; private set; }

    [SerializeField] private InventorySO playerInventory;
    [SerializeField] private TextMeshProUGUI descriptionArea;
    private bool isToggledOn = false;

    private InventorySlot[] inventoryDisplay;

    private void Awake()
    {
        Instance = this;
        
        canvasGroup = GetComponent<CanvasGroup>();
        
        ToggleDescriptionText(false);
    }

    private void Start()
    {
        inventoryDisplay = GetComponentsInChildren<InventorySlot>();
        Hide();
    }

    protected override void Activate()
    {
        if(playerInventory.Inventory.Count > inventoryDisplay.Length)
            Debug.LogError("Inventory overflow");
        
        int i = 0;
        foreach (var (item, count) in playerInventory.Inventory)
        {
            inventoryDisplay[i].Display(item, count);
            i++;
        }
    }

    public void SetDescriptionText(string text)
    {
        descriptionArea.text = text;
    }

    public void ToggleDescriptionText(bool activate)
    {
        descriptionArea.enabled = activate;
    }
}