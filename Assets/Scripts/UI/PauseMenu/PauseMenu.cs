using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PauseMenu : UI 
{
    [SerializeField] private ButtonUI resumeButton;
    [SerializeField] private ButtonUI mainMenuButton;
    [SerializeField] private ButtonUI optionsButton;
    
    [SerializeField] private LineScaler lineScaler;
    
    public static PauseMenu Instance { get; private set; }

    public bool IsGamePaused = false;
    
    private void Awake()
    {
        Instance = this;
        Init();
    }
    
    private void Start()
    {
        resumeButton.AddListener(() =>
        {
            ForceHide();
            IsGamePaused = false;
        });
        mainMenuButton.AddListener(OnMainMenuClick);
        optionsButton.AddListener(OnOptionsMenuClick);
        
        gameObject.SetActive(false);
        
        GameInput.Instance.OnPauseToggle += TogglePauseGame;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPauseToggle -= TogglePauseGame;
    }
    
    private void ToggleButtons(bool enable)
    {
        if (enable)
        {
            resumeButton.Enable();
            mainMenuButton.Enable();
            optionsButton.Enable();
        }
        else
        {
            resumeButton.Disable();
            mainMenuButton.Disable();
            optionsButton.Disable();
        }
    }

    private void OnMainMenuClick()
    {
        ToggleButtons(false);
        TogglePauseGame();
        Loader.Load(GameScene.MainMenuScene);
    }
    
    private void OnOptionsMenuClick()
    {
        if (Hide(false))
        {
            UIManager.CurrentContext = OptionsUI.Instance.gameObject;
            OptionsUI.Instance.Show();
        }
    }

    protected override void Activate()
    {
        ToggleButtons(true);
        AudioManager.Instance.PlayOnClick();
        GameEventsManager.Instance.GameStateEvents.PauseToggle(true);
        HUD.Instance.Hide();
        lineScaler.Animate(false);
    }

    protected override void Deactivate()
    {
        ToggleButtons(false);
        AudioManager.Instance.PlayOnClick();
        GameEventsManager.Instance.GameStateEvents.PauseToggle(false);
        HUD.Instance.ShowAfterDelay(2);
        UIManager.CurrentContext = null;
        lineScaler.Animate(true);
    }

    private void TogglePauseGame()
    {
        if (UIManager.CurrentContext != null && UIManager.CurrentContext != gameObject)
            return;
        
        if (IsGamePaused && Hide())
        {
            IsGamePaused = false;
        }
        else if(!IsGamePaused && Show())
        {
            IsGamePaused = true;
        }
    }
}