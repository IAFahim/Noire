using UnityEngine;
using UnityEngine.UI;
using Application = UnityEngine.Device.Application;

public class MainMenu : UI
{
    public static MainMenu Instance { get; private set; }
    
    [SerializeField] private ButtonUI newGameButton;
    [SerializeField] private ButtonUI continueGameButton;
    [SerializeField] private ButtonUI loadGameButton;
    [SerializeField] private ButtonUI quitGameButton;
    [SerializeField] private ButtonUI settingsButton;

    private void Awake()
    {
        Instance = this;
        Init();
    }

    private void Start() 
    {
        newGameButton.AddListener(OnNewGameClicked);
        continueGameButton.AddListener(OnContinueGameClicked);
        loadGameButton.AddListener(OnLoadGameClicked);
        quitGameButton.AddListener(OnQuitGameClicked);
        settingsButton.AddListener(OnSettingsClicked);
        
        Show();
    }

    private void ToggleButtons(bool enable)
    {
        if (enable)
        {
            newGameButton.Enable();
            continueGameButton.Enable();
            loadGameButton.Enable();
            quitGameButton.Enable();
        }
        else
        {
            newGameButton.Disable();
            continueGameButton.Disable();
            loadGameButton.Disable();
            quitGameButton.Disable();
        }
    }

    private void OnNewGameClicked()
    {
        Hide();
        SaveSlotsMenu.Instance.DisplayNewGameMenu();
    }

    private void OnContinueGameClicked()
    {
        Loader.Load(DataPersistenceManager.Instance.LastCheckPointScene);
    }
    
    private void OnLoadGameClicked() 
    {
        Hide();
        SaveSlotsMenu.Instance.DisplayLoadGameMenu();
    }

    private void OnQuitGameClicked()
    {
        Hide();
        ConfirmationPopupMenu.Instance.ActivateMenu("Quit the Game?", () =>
        {
            Application.Quit();
        }, () =>
        {
            Show();
        });
    }

    protected override void Deactivate()
    {
        ToggleButtons(false);
    }

    private void OnSettingsClicked()
    {
        Hide();
        OptionsMainMenu.Instance.Show();
    }
    
    // UI baseclass
    protected override void Activate()
    {
        ToggleButtons(true);
        if (!DataPersistenceManager.Instance.HasGameData()) 
        {
            continueGameButton.Disable();
            loadGameButton.Disable();
        }
    }
}