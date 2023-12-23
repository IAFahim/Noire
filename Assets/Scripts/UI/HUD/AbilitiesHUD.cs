using UnityEngine;

public class AbilitiesHUD : UI
{
    public static AbilitiesHUD Instance;
    
    [SerializeField] private AbilitiesSprite[] abilitiesSprites;

    protected override void Awake()
    {
        Instance = this;
        // this needs to be called in Awake! (its fine since GameEventsManager always awoken first
        GameEventsManager.Instance.PlayerEvents.OnUpdateAbility += PlayerEventsOnOnUpdateAbility;
    }

    private void PlayerEventsOnOnUpdateAbility(int idx, AbilitySO newAbility)
    {
        abilitiesSprites[idx].UpdateAbilitySlot(newAbility);
    }
}