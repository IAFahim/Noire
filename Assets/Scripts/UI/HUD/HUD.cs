using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : UI
{
    public static HUD Instance { get; private set; }
    
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private PlayerStaminaSO PlayerStaminaSO;
    [SerializeField] private PlayerStatisticsSO dreamShardsSO;
    [SerializeField] private PlayerStatisticsSO dreamThreadsSO;

    [SerializeField] private TextMeshProUGUI dreamShardsCount;
    [SerializeField] private TextMeshProUGUI dreamThreadsCount;
    
    [SerializeField] private SwitchableSprite icon; 

    [SerializeField] private Bar neutralDrowsiness;
    [SerializeField] private Bar lucidDrowsiness;
    [SerializeField] private Bar deepDrowsiness;
    [SerializeField] private Slider stamina;

    private int neutralBarLength;

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
        neutralBarLength = Player.Instance.DeepThreshold - Player.Instance.LucidThreshold - 1;
        
        GameEventsManager.Instance.PlayerEvents.OnUpdateHealthBar += UpdateHealthBar;
        GameEventsManager.Instance.PlayerEvents.OnUpdateStaminaBar += UpdateStaminaBar;
        GameEventsManager.Instance.PlayerEvents.OnDreamShardsChangeFinished += UpdateDreamShardsCount;
        GameEventsManager.Instance.PlayerEvents.OnDreamThreadsChangeFinished += UpdateDreamThreadsCount;

        Show();
        UpdateHealthBar();
        UpdateStaminaBar();
        UpdateDreamShardsCount();
        UpdateDreamThreadsCount();
        icon.Switch(2);
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            GameEventsManager.Instance.PlayerEvents.OnUpdateHealthBar -= UpdateHealthBar;
            GameEventsManager.Instance.PlayerEvents.OnUpdateStaminaBar -= UpdateStaminaBar;
            GameEventsManager.Instance.PlayerEvents.OnDreamShardsChangeFinished -= UpdateDreamShardsCount;
            GameEventsManager.Instance.PlayerEvents.OnDreamThreadsChangeFinished -= UpdateDreamThreadsCount;
        }
    }

    private void UpdateHealthBar()
    {
        ToggleHealthBars();
        var drowsiness = playerHealthSO.CurrentDrowsiness;
        
        if (drowsiness <= Player.Instance.LucidThreshold) // lucid
        {
            icon.Switch(3);
            lucidDrowsiness.Display(drowsiness);
            neutralDrowsiness.Display(0);
        }
        else if (drowsiness >= Player.Instance.DeepThreshold) // deep
        {
            icon.Switch(4);
            lucidDrowsiness.Display(Player.Instance.LucidThreshold);
            neutralDrowsiness.Display(neutralBarLength);
            deepDrowsiness.Display(drowsiness - Player.Instance.DeepThreshold + 1);   
        }
        else // neutral
        {
            var i = drowsiness - Player.Instance.LucidThreshold;
            icon.Switch(i - 1);
            neutralDrowsiness.Display(i);
        }
    }

    private void ToggleHealthBars()
    {
        if (playerHealthSO.CurrentDrowsiness < Player.Instance.DeepThreshold)
        {
            deepDrowsiness.gameObject.SetActive(false);
        }
        else
        {
            deepDrowsiness.gameObject.SetActive(true);
        }
    }

    private void UpdateStaminaBar()
    {
        stamina.value = PlayerStaminaSO.CurrentStaminaPercentage;
    }
    
    private void UpdateDreamShardsCount()
    {
        dreamShardsCount.text = dreamShardsSO.GetCurrencyCount().ToString();
    }
    
    private void UpdateDreamThreadsCount()
    {
        dreamThreadsCount.text = dreamThreadsSO.GetCurrencyCount().ToString();
    }
}
