using System;
using UnityEngine;

public class PlayerMenu : UI
{
    public static PlayerMenu Instance { get; private set; }

    [SerializeField] private ButtonUI backButton;
    [SerializeField] private ButtonUI inventoryMenuButton;
    [SerializeField] private ButtonUI weaponMenuButton;
    [SerializeField] private ButtonUI abilityMenuButton;
    private bool isToggledOn = false;
    
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        
        backButton.AddListener(OnBackButtonClicked);
        inventoryMenuButton.AddListener(OnInventoryMenuButtonClicked);
        weaponMenuButton.AddListener(OnWeaponMenuButtonClicked);
        abilityMenuButton.AddListener(OnAbilityMenuButtonClicked);
        
        GameInput.Instance.OnPlayerMenuToggle += GameInput_OnPlayerMenuToggle;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerMenuToggle -= GameInput_OnPlayerMenuToggle;
    }

    private void GameInput_OnPlayerMenuToggle()
    {
        isToggledOn = !isToggledOn;
        if (isToggledOn)
        {
            Show();
        }
        else
        {
            Hide();
        }
        GameEventsManager.Instance.GameStateEvents.UIToggle(isToggledOn);
    }

    private void OnBackButtonClicked()
    {
        Hide();
    }

    private void OnInventoryMenuButtonClicked()
    {
        InventoryMenu.Instance.Show();
    }

    private void OnWeaponMenuButtonClicked()
    {
        WeaponMenu.Instance.Show();
    }

    private void OnAbilityMenuButtonClicked()
    {
        AbilityMenu.Instance.Show();
    }
}