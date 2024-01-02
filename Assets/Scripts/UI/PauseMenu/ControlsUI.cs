﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ControlsUI : UI
{
    public static ControlsUI Instance { get; private set; }

    [SerializeField] private ButtonUI moveUpButton;
    [SerializeField] private ButtonUI moveDownButton;
    [SerializeField] private ButtonUI moveLeftButton; 
    [SerializeField] private ButtonUI moveRightButton;
    [SerializeField] private ButtonUI lightAttackButton;
    [SerializeField] private ButtonUI chargedAttackButton;
    [SerializeField] private ButtonUI dashButton;
    [SerializeField] private ButtonUI interactButton;
    [SerializeField] private ButtonUI ability1Button;
    [SerializeField] private ButtonUI ability2Button;
    [SerializeField] private ButtonUI ability3Button;
    
    [SerializeField] private UI rebindUI;
    
    [SerializeField] private ButtonUI resetButton;
    [SerializeField] private ButtonUI backButton;
    [SerializeField] private UI container;

    [SerializeField] private LineScaler lineScaler;
    
    private IEnumerable<RectTransform> layoutGroupTransformsInChildren;
    
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
        
        layoutGroupTransformsInChildren = GetComponentsInChildren<LayoutGroup>()
            .Select(x => x.GetComponent<RectTransform>());
    }

    private void Start()
    {
        moveUpButton.AddListener(() => {RebindBinding(GameInput.Bindings.MoveUp);  SetCurrentContext();});
        moveDownButton.AddListener(() => {RebindBinding(GameInput.Bindings.MoveDown); SetCurrentContext();});
        moveLeftButton.AddListener(() => {RebindBinding(GameInput.Bindings.MoveLeft); SetCurrentContext();});
        moveRightButton.AddListener(() => {RebindBinding(GameInput.Bindings.MoveRight); SetCurrentContext();});
       
        lightAttackButton.AddListener(() => {RebindBinding(GameInput.Bindings.LightAttack); SetCurrentContext();});
        chargedAttackButton.AddListener(() => { RebindBinding(GameInput.Bindings.ChargedAttack); SetCurrentContext();});
        dashButton.AddListener(() => { RebindBinding(GameInput.Bindings.Dash); SetCurrentContext();});
        interactButton.AddListener(() => { RebindBinding(GameInput.Bindings.Interact); SetCurrentContext();});
     
        ability1Button.AddListener(() => {RebindBinding(GameInput.Bindings.Ability1); SetCurrentContext(); });
        ability2Button.AddListener(() => {RebindBinding(GameInput.Bindings.Ability2); SetCurrentContext(); });
        ability3Button.AddListener(() => {RebindBinding(GameInput.Bindings.Ability3); SetCurrentContext(); });

        UpdateVisual();
        
        backButton.AddListener(DisplayOptionsMenu);
        resetButton.AddListener(OnResetAllBindings);

        rebindUI.Hide(false);
        container.Hide(false);
        Hide(false);
        
        GameInput.Instance.OnPauseToggle += DisplayOptionsMenu;
    }

    private void ToggleButtons(bool enable)
    {
        if (enable)
        {
            moveUpButton.Enable();
            moveDownButton.Enable();
            moveLeftButton.Enable();
            moveRightButton.Enable();
            lightAttackButton.Enable();
            chargedAttackButton.Enable();
            dashButton.Enable();
            interactButton.Enable();
            ability1Button.Enable();
            ability2Button.Enable();
            ability3Button.Enable();
            backButton.Enable();
        }
        else
        {
            moveUpButton.Disable();
            moveDownButton.Disable();
            moveLeftButton.Disable();
            moveRightButton.Disable();
            lightAttackButton.Disable();
            chargedAttackButton.Disable();
            dashButton.Disable();
            interactButton.Disable();
            ability1Button.Disable();
            ability2Button.Disable();
            ability3Button.Disable();
            backButton.Disable();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            GameInput.Instance.OnPauseToggle -= DisplayOptionsMenu;
        }
    }

    private void DisplayOptionsMenu()
    {
        if (UIManager.CurrentContext != gameObject)
            return;
        
        if (Hide())
        {
            UIManager.CurrentContext = OptionsUI.Instance.gameObject;
            OptionsUI.Instance.Show();
        }
    }

    protected override void Activate()
    {
        foreach (var t in layoutGroupTransformsInChildren)
            LayoutRebuilder.ForceRebuildLayoutImmediate(t);
        ToggleButtons(true);
        AudioManager.Instance.PlayOnClick();
        container.ForceShow();
        UIManager.CurrentContext = gameObject;
        lineScaler.Animate(false);
    }

    protected override void Deactivate()
    {
        ToggleButtons(false);
        AudioManager.Instance.PlayOnClick();
        container.Hide();
        lineScaler.Animate(true);
    }

    private void UpdateVisual()
    {
        moveUpButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.MoveUp);
        moveDownButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.MoveDown);
        moveLeftButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.MoveLeft);
        moveRightButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.MoveRight);
        
        lightAttackButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.LightAttack);
        chargedAttackButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.ChargedAttack);
        dashButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Dash);
        interactButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Interact);
        
        ability1Button.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Ability1);
        ability2Button.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Ability2);
        ability3Button.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Ability3);
    }

    private void SetCurrentContext()
    {
        UIManager.CurrentContext = rebindUI.gameObject;
    }

    private void OnResetAllBindings()
    {
        GameInput.Instance.ResetAllBindings();
        UpdateVisual();
        foreach (var t in layoutGroupTransformsInChildren)
            LayoutRebuilder.ForceRebuildLayoutImmediate(t);
        WarningText.Instance.ShowPopup(2, "Bindings have been reset to default");
    }
    
    private void RebindBinding(GameInput.Bindings binding)
    {
        rebindUI.Show();
        Hide();
        
        GameInput.Instance.RebindBinding(binding, () =>
        {
            AudioManager.Instance.PlayOnClick();
            UpdateVisual();
            ForceShow();
            rebindUI.ForceHide();
        }, () =>
        {
            WarningText.Instance.ShowPopup(2, "You cannot rebind a key to an already existing binding");
        },  () =>
        {
            WarningText.Instance.ShowPopup(2, "The Mouse cannot be binded to this action");
        });
    }
}

