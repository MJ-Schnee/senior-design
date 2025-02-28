using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/TreeSummon")]
public class TreeSummon : BaseAction
{
    public int ActionRadius = 1;

    public GameObject TreePrefab;

    /// <summary>
    /// Triggers action animation which applies upon animation event ApplyImpact
    /// </summary>
    public override void UseAction(Player actingPlayer, Player targetPlayer)
    {
        Debug.Log($"{actingPlayer.name} is {name}ing {targetPlayer.name}!");

        actingPlayer.currentAction = this;
        actingPlayer.currentActionTarget = targetPlayer;

        // Play animation, which triggers ApplyImpact
        actingPlayer.Animator.SetTrigger(AnimationTrigger);
    }

    /// <summary>
    /// Triggered on action animation impact, dealing ActionDamage damage to the target player and surrounding players.
    /// </summary>
    public override void ApplyImpact(Player targetPlayer)
    {
        GameObject vine = Instantiate(TreePrefab, targetPlayer.transform.position, Quaternion.identity);

        Tile targetPlayerTile = targetPlayer.GetCurrentTile();
        List<Tile> tilesInRange = TileGridManager.Instance.FindTilesInRange(targetPlayerTile, ActionRadius);
        foreach (Tile tile in tilesInRange)
        {
            Player playerInRange = TileGridManager.Instance.GetPlayerOnTile(tile);
            if (playerInRange != null)
            {
                // Roll D20 against AC for hit
                int diceResult = Random.Range(1, 21);
                if (diceResult >= playerInRange.PlayerAc)
                {
                    playerInRange.DealDamage(ActionDamage);
                    UiManager.Instance.ShowHitMissIndicator("HIT", Color.green, playerInRange.transform);
                }
                else
                {
                    UiManager.Instance.ShowHitMissIndicator("MISS", Color.red, playerInRange.transform);
                }
            }
        }
    }
}
