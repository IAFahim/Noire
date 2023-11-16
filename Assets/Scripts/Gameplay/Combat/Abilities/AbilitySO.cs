using UnityEngine;
using System.Collections;

public abstract class AbilitySO : ScriptableObject
{
    [SerializeField] public int abilityID;
    [SerializeField] protected string abilityAnimationTrigger;
    [SerializeField] protected float cooldown = 1;
    [SerializeField] public float staminaCost = 10f;
    [SerializeField] public DreamState[] applicableDreamStates;
    [SerializeField] public bool playerMovableDuringCast;

    private float cooldownCounter;
    
    protected enum AbilityState
    {
        Ready,
        Active,
        OnCooldown
    }

    protected AbilityState state;
    
    // this is called on game awake
    public virtual void Ready() => state = AbilityState.Ready;
    
    // should continue the ability by modifying the state of the object it is attached to
    // when finished, call finish()
    protected abstract void Cast();

    // when finished, should make state = AbilityState.OnCooldown
    // IMPORTANT: must call Player.Instance.ResetStateAfterAction() otherwise the ability is stuck forever
    protected abstract void Finish();

    // called when ability is activated. Should initialize related fields
    protected abstract void Initialize();
    
    // called when the ability is activated
    // returns true if successfully casted the ability. False iff the ability is not ready
    public void Activate()
    {
        state = AbilityState.Active;
        cooldownCounter = cooldown;
        Initialize();
        Cast();
    }

    public bool CanActivate()
    {
        return state == AbilityState.Ready;
    }
    
    // called on each frame during which the ability is activated
    public void DecreaseCooldown()
    {
        if (state == AbilityState.OnCooldown)
        {
            cooldownCounter -= Time.deltaTime;
            if (cooldownCounter < 0)
            {
                state = AbilityState.Ready;
                cooldownCounter = cooldown;
            }
        }
    }
    
    // helpful coroutine
    protected IEnumerator WaitEndOfAction(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Finish();
    }
}
