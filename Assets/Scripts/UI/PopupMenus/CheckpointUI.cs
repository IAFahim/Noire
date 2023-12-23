using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointUI : UI
{
    public static CheckpointUI Instance;
    
    [Header("Buttons")]
    [SerializeField] private ButtonUI resumeButton;
    [SerializeField] private ButtonUI restButton;
    [SerializeField] private ButtonUI backToBedrockPlains;
    
    [Header("Line Animations")] 
    [SerializeField] private LineScaler lineScaler;
    private bool isToggledOn = false;
    
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Start()
    {
        Hide(false);
        
        resumeButton.AddListener(OnResumeButtonClicked);
        restButton.AddListener(OnRestButtonClicked);
        backToBedrockPlains.AddListener(OnBackToBedrockPlainsButtonClicked);
        
        GameInput.Instance.OnPauseToggle += OnPauseToggle;
    }
    
    private void OnDestroy()
    {
        GameInput.Instance.OnPauseToggle -= OnPauseToggle;
    }

    protected override void Activate()
    {
        if (SceneManager.GetActiveScene().name == "BedrockPlains")
            backToBedrockPlains.Disable();
        UIManager.CurrentContext = gameObject;
        GameEventsManager.Instance.GameStateEvents.UIToggle(true, false);
        lineScaler.Animate(false);
        isToggledOn = true;
    }
    
    protected override void Deactivate()
    {
        GameEventsManager.Instance.GameStateEvents.UIToggle(false, false);
        lineScaler.Animate(true);
    }
    
    protected override void LateDeactivate()
    {
        UIManager.CurrentContext = null;
    }
    
    private void OnPauseToggle()
    {
        if (UIManager.CurrentContext != gameObject)
            return;

        if (isToggledOn && Hide())
            isToggledOn = false;
    }

    private void OnResumeButtonClicked()
    {
        if (Hide())
            isToggledOn = false;
    }

    
    private void OnRestButtonClicked()
    {
        GameEventsManager.Instance.PlayerEvents.Rest();
        DataPersistenceManager.Instance.SaveGame();
    }
    
    private void OnBackToBedrockPlainsButtonClicked()
    {
        Hide();
        Loader.Load(GameScene.BedrockPlains);
    }
}