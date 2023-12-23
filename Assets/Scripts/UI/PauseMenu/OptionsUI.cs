using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : UI
{
    public static OptionsUI Instance { get; private set; }
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private ButtonUI controlsButton;
    [SerializeField] private ButtonUI backButton;
    
    [SerializeField] private LineScaler lineScaler;
    
    private void Awake()
    {
        Instance = this;
        Init();
    }

    private void Start()
    {
        sfxSlider.value = AudioManager.Instance.defaultSoundLevel;
        musicSlider.value = AudioManager.Instance.defaultSoundLevel;
        
        sfxSlider.onValueChanged.AddListener(delegate(float level) { VolChange("Sfx", level); });
        musicSlider.onValueChanged.AddListener(delegate(float level) { VolChange("Ost", level); });
        
        controlsButton.AddListener(OnControlsButtonClicked);
        backButton.AddListener(BackToPauseMenu);

        Hide();
        
        GameInput.Instance.OnPauseToggle += BackToPauseMenu;
    }
    
    private void OnDestroy()
    {
        GameInput.Instance.OnPauseToggle -= BackToPauseMenu;
    }
    
    private void ToggleButtons(bool enable)
    {
        musicSlider.enabled = enable;
        sfxSlider.enabled = enable;
        if (enable)
        {
            controlsButton.Enable();
            backButton.Enable();
        }
        else
        {
            controlsButton.Disable();
            backButton.Disable();
        }
    }

    protected override void Activate()
    {
        ToggleButtons(true);
        AudioManager.Instance.PlayOnClick();
        lineScaler.Animate(false);
    }

    protected override void Deactivate()
    {
        AudioManager.Instance.PlayOnClick();
        ToggleButtons(false);
        lineScaler.Animate(true);
    }
    
    private void BackToPauseMenu()
    {
        if (UIManager.CurrentContext != gameObject)
            return;
        
        if (Hide())
        {
            UIManager.CurrentContext = PauseMenu.Instance.gameObject;
            PauseMenu.Instance.Show(false);
        }
    }

    private void OnControlsButtonClicked()
    {
        if (Hide())
        {
            UIManager.CurrentContext = ControlsUI.Instance.gameObject;
            ControlsUI.Instance.Show();
        }
    }

    private void VolChange(string vcaType, float level)
    {
        AudioManager.Instance.PlayOnClick();
        AudioManager.Instance.SetVolume(vcaType, level);
    }
}

