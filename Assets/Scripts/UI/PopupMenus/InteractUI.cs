using UnityEngine;
using TMPro;

public class InteractUI : UI
{
    [SerializeField] private TextMeshProUGUI interactText;
    private IInteractable interactable;
    private IInteractable lastInteractable;

    private bool isShowing;
    private bool disabled;

    protected override void Awake()
    {
        alternativeGameObject = true;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {    
        Hide();
        
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle += ToggleActive;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle += UIToggleActive;
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle -= ToggleActive;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle -= UIToggleActive;
    }
    
    private void ToggleActive(bool paused)
    {
        disabled = paused;
        Update();
    }
    
    private void UIToggleActive(bool isToggled, bool triggerBlur)
    {
        disabled = isToggled;
        Update();
    }

    private void Update()
    {
        if (disabled)
            return;
        
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
        interactText.text = interactable?.GetInteractText();
        isShowing = true;
    }
    
    protected override void Deactivate()
    {
        isShowing = false;
    }
}
