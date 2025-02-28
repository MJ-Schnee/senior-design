using UnityEngine;

[CreateAssetMenu(menuName = "Actions/GenericAction", fileName = "NewAction")]
public class BaseAction : ScriptableObject
{
    public string AnimationTrigger;

    public string ActionName;

    [TextArea]
    public string ActionDescription;

    public Color ActionColor;

    public int NumTargets = 1;

    public int ActionRange;

    public int ActionDamage = 1;

    /// <summary>
    /// Triggers action animation which applies upon animation event ApplyImpact
    /// </summary>
    public virtual void UseAction(Player actingPlayer, Player targetPlayer)
    {
        Debug.Log($"{actingPlayer.name} is {name}ing {targetPlayer.name}!");

        // Make players face each other
        Vector3 targetPosition = new(targetPlayer.transform.position.x, actingPlayer.transform.position.y, targetPlayer.transform.position.z) ;
        Vector3 actingPosition = new(actingPlayer.transform.position.x, targetPlayer.transform.position.y, actingPlayer.transform.position.z) ;
        actingPlayer.transform.LookAt(targetPosition);
        targetPlayer.transform.LookAt(actingPosition);

        actingPlayer.currentAction = this;
        actingPlayer.currentActionTarget = targetPlayer;

        // Play animation, which triggers ApplyImpact
        actingPlayer.Animator.SetTrigger(AnimationTrigger);
    }

    /// <summary>
    /// Triggered on action animation impact, dealing ActionDamage damage to the target player.
    /// </summary>
    public virtual void ApplyImpact(Player targetPlayer)
    {
        // Roll D20 against AC for hit
        int diceResult = Random.Range(1, 21);
        if (diceResult >= targetPlayer.PlayerAc)
        {
            targetPlayer.DealDamage(ActionDamage);
            UiManager.Instance.ShowHitMissIndicator("HIT", Color.green, targetPlayer.transform);
        }
        else
        {
            UiManager.Instance.ShowHitMissIndicator("MISS", Color.red, targetPlayer.transform);
        }
    }
}
