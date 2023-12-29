using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIBlurBackground : UI
{
    [SerializeField] ScriptableRendererFeature kawaseBlur;
    
    protected override void Awake()
    {
        Init();
    }

    private void Start()
    {
        Hide();
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle += OnPause;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle += OnUIPause;
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle -= OnPause;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle -= OnUIPause;
    }
    
    private void OnPause(bool paused)
    {
        if(paused)
            ForceShow();
        else
            ForceHide();
    }

    private void OnUIPause(bool paused, bool triggerBlur)
    {
        if (!gameObject.activeSelf || !triggerBlur)
            return;
        
        if(paused)
            ForceShow();
        else
            ForceHide();
    }

    protected override void Activate()
    {
        kawaseBlur.SetActive(true);
    }

    protected override void Deactivate()
    {
        kawaseBlur.SetActive(false);
    }
}
