using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/TeamHeal")]
public class TeamHeal : BaseAction
{
    public int HealAmount = 4;

    /// <summary>
    /// Triggers action animation which applies upon animation event ApplyImpact
    /// </summary>
    public override void UseAction(Player actingPlayer, Player targetPlayer)
    {
        Debug.Log($"{actingPlayer.name} is using a team heal!");

        actingPlayer.currentAction = this;

        // Play animation, which triggers ApplyImpact
        actingPlayer.Animator.SetTrigger(AnimationTrigger);
    }

    /// <summary>
    /// Triggered on action animation impact, healing HealPlayer health of the target player.
    /// </summary>
    public override void ApplyImpact(Player targetPlayer)
    {
        // Heal each player on team
        foreach (Player playerToHeal in GameManager.Instance.InitialPlayerList)
        {
            if (playerToHeal.IsDead)
            {
                continue;
            }

            int currHealth = playerToHeal.PlayerHp_curr;
            playerToHeal.HealPlayer(HealAmount);
            int postHealHealth = playerToHeal.PlayerHp_curr;
            int healthChange = postHealHealth - currHealth;
            UiManager.Instance.ShowHitMissIndicator($"+{healthChange} HP", Color.green, playerToHeal.transform);
        }
    }
}
