using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMenu : UI
{
    public static PlayerMenu Instance { get; private set; }

    [Header("Buttons")]
    [SerializeField] private ButtonUI backButton;
    [SerializeField] private ButtonUI inventoryMenuButton;
    [SerializeField] private ButtonUI weaponMenuButton;
    [SerializeField] private ButtonUI abilityMenuButton;
    
    [Header("Line Animations")] 
    [SerializeField] private LineScaler lineScaler;
    [SerializeField] private LineScaler lineScalerMainFrame;
    private bool isToggledOn = false;

    private UI lastContext;
    
    protected override void Awake()
    {
        base.Awake();
        
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Hide(false);
        
        backButton.AddListener(OnBackButtonClicked);
        inventoryMenuButton.AddListener(OnInventoryMenuButtonClicked);
        weaponMenuButton.AddListener(OnWeaponMenuButtonClicked);
        abilityMenuButton.AddListener(OnAbilityMenuButtonClicked);
        
        GameInput.Instance.OnPlayerMenuToggle += OnPlayerMenuToggle;
        GameInput.Instance.OnPauseToggle += OnPauseToggle;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            GameInput.Instance.OnPlayerMenuToggle -= OnPlayerMenuToggle;
            GameInput.Instance.OnPauseToggle -= OnPauseToggle;
        }
    }

    protected override void Activate()
    {
        UIManager.CurrentContext = gameObject;
        GameEventsManager.Instance.GameStateEvents.UIToggle(true);
        HUD.Instance.Hide();
        lineScaler.Animate(false);
        lineScalerMainFrame.Animate(false, true);
    }

    protected override void Deactivate()
    {
        lastContext?.Hide();
        GameEventsManager.Instance.GameStateEvents.UIToggle(false);
        HUD.Instance.ShowAfterDelay(2);
        lineScaler.Animate(true);
        lineScalerMainFrame.Animate(true);
    }

    protected override void LateDeactivate()
    {
        UIManager.CurrentContext = null;
    }

    /// shows/hides when [m] is pressed
    private void OnPlayerMenuToggle()
    {
        if (UIManager.CurrentContext != null && UIManager.CurrentContext != gameObject)
            return;
        
        if (isToggledOn && Hide())
        {
            isToggledOn = false;
        }
        else if (!isToggledOn && Show())
        {
            isToggledOn = true;
        }
    }
    
    /// hides when [esc] is pressed
    private void OnPauseToggle()
    {
        if (UIManager.CurrentContext != gameObject)
            return;

        if (isToggledOn && Hide())
            isToggledOn = false;
    }

    private void OnBackButtonClicked()
    {
        if (Hide())
            isToggledOn = false;
    }

    private void OnInventoryMenuButtonClicked()
    {
        if (lastContext && lastContext != InventoryMenu.Instance)
            lastContext.ForceHide();
        
        InventoryMenu.Instance.ForceShow();
        lastContext = InventoryMenu.Instance;
    }

    private void OnWeaponMenuButtonClicked()
    {
        if (lastContext && lastContext != WeaponMenu.Instance)
            lastContext.ForceHide();
        
        WeaponMenu.Instance.ForceShow();
        lastContext = WeaponMenu.Instance;
    }

    private void OnAbilityMenuButtonClicked()
    {
        if (lastContext && lastContext != AbilityMenu.Instance)
            lastContext.ForceHide();

        AbilityMenu.Instance.ForceShow();
        lastContext = AbilityMenu.Instance;
    }
}