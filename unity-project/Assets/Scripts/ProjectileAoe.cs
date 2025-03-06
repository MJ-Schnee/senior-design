using System.Collections.Generic;
using UnityEngine;

public class ProjectileAoe : Projectile
{
    private int aoeRadius = 1;

    /// <summary>
    /// Used to initialize projectile with aoe radius
    /// </summary>
    public void Launch(Player target, float speed, int damage, int aoeRadius)
    {
        targetPlayer = target;
        this.speed = speed;
        this.damage = damage;
        this.aoeRadius = aoeRadius;
    }

    /// <summary>
    /// Apply damage to hit player and everyone in radius
    /// </summary>
    protected override void OnImpact()
    {
        Tile targetPlayerTile = targetPlayer.GetCurrentTile();
        List<Tile> tilesInRange = TileGridManager.Instance.FindTilesInRange(targetPlayerTile, aoeRadius);
        foreach (Tile tile in tilesInRange)
        {
            Player playerInRange = TileGridManager.Instance.GetPlayerOnTile(tile);
            if (playerInRange != null)
            {
                // Roll D20 against AC for hit
                int diceResult = Random.Range(1, 21);
                if (diceResult >= playerInRange.PlayerAc)
                {
                    playerInRange.DealDamage(damage);
                    UiManager.Instance.ShowHitMissIndicator("HIT", Color.green, playerInRange.transform);
                }
                else
                {
                    UiManager.Instance.ShowHitMissIndicator("MISS", Color.red, playerInRange.transform);
                }
            }
        }

        Destroy(gameObject);
    }
}
