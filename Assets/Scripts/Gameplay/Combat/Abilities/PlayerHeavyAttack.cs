using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHeavyAttack", menuName = "Abilities/PlayerHeavyAttack")]
public class PlayerHeavyAttack : AbilitySO
{
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private float overChargeDuration = 3f;
    [SerializeField] private string releaseAnimationTrigger;
    private float initialTimer;
    
    protected override void Cast()
    {
        initialTimer = Time.time;
        
        Player.Instance.ResetAnimationTrigger(releaseAnimationTrigger);
        Player.Instance.SetAnimatorTrigger(abilityAnimationTrigger);
        Player.Instance.ChargeSwordAnimation();
        Player.Instance.StartCoroutine(OverCharge());
    }

    private void Release()
    {
        Player.Instance.SetAnimatorTrigger(releaseAnimationTrigger);
        Player.Instance.MoveFor(20f, 0.2f, Player.Instance.transform.forward);
        Player.Instance.HandleAttackOnHitEffects();
    }

    protected override void Interrupt()
    {
        state = AbilityState.OnCooldown;
        Player.Instance.StopChargeSwordAnimation();
        
        float charge = Time.time - initialTimer;
        if (charge >= 1)
        {
            Player.Instance.StartCoroutine(Player.Instance.WaitForAndReset(attackDuration));
            Release();
        }
        else
        {
            Player.Instance.ResetStateAfterAction();
        }
    }

    private IEnumerator OverCharge()
    {
        yield return new WaitForSeconds(overChargeDuration);
        state = AbilityState.OnCooldown;
        Player.Instance.StopChargeSwordAnimation();
        Player.Instance.ResetStateAfterAction();
    }
}
