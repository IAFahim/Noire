using UnityEngine;

public class PauseMenu : UI 
{
    public static PauseMenu Instance { get; private set; }
    
    [SerializeField] private ButtonUI resumeButton;
    [SerializeField] private ButtonUI mainMenuButton;
    [SerializeField] private ButtonUI optionsButton;
    
    [SerializeField] private LineScaler lineScaler;
    
    public bool IsGamePaused = false;
    
    protected override void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
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

        Hide(false);
        
        GameInput.Instance.OnPauseToggle += TogglePauseGame;
    }

    private void OnDestroy()
    {
        if (Instance == this)
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
        Hide(false);
        
        ToggleButtons(false);
        AudioManager.Instance.PlayOnClick();
        UIManager.CurrentContext = null;
        lineScaler.Animate(true);
        GameEventsManager.Instance.GameStateEvents.PauseToggle(false);
        IsGamePaused = false;
        
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
        HUD.Instance.ForceHide();
        UIManager.CurrentContext = gameObject;
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