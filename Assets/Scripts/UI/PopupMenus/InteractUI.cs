using System;
using UnityEngine;
using TMPro;

public class InteractUI : UI
{
    [SerializeField] private TextMeshProUGUI interactText;
    private IInteractable interactable;
    private IInteractable lastInteractable;

    private bool isShowing; 

    protected override void Awake()
    {
        alternativeGameObject = true;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {    
        Hide(false);
        
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle += ToggleActive;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle += UIToggleActive;
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle -= ToggleActive;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle += UIToggleActive;
    }
    
    private void ToggleActive(bool paused)
    {
        gameObject.SetActive(!paused);
    }
    
    private void UIToggleActive(bool isToggled, bool triggerBlue)
    {
        gameObject.SetActive(!isToggled);
    }

    private void Update()
    {
        lastInteractable = interactable;
        interactable = Player.Instance.GetInteractableObject();

        if (interactable != null && (!isShowing || interactable != lastInteractable))
        {
            ForceShow();
        }
        else if(interactable == null && isShowing)
        {
            Hide();
        }
    }
    
    protected override void Activate()
    {
        interactText.text = interactable.GetInteractText();
        isShowing = true;
    }
    
    protected override void Deactivate()
    {
        isShowing = false;
    }
}
