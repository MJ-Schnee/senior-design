using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    /// <summary>
    /// Simple enemy AI: attempts to run away from closest player and then throw object at them.
    /// </summary>
    protected override IEnumerator EnemyTurnAI()
    {        
        // Find the nearest player
        (Player targetPlayer, int distFromTarget, Tile targetTile) = FindNearestTarget();

        BaseAction rangedAction = Action1;
        int maxRange = Action1.ActionRange;

        List<Tile> reachableTiles = TileGridManager.Instance.FindTilesInRange(GetCurrentTile(), PlayerSpeed);

        // Find tile furthest from players but within attack range
        Tile bestTile = GetCurrentTile(); // fallback = current tile
        int bestDistanceToTarget = -1;
        Vector2Int targetPos = TileHelper.PositionToCoordinate(targetTile.transform.position);
        foreach (Tile candidate in reachableTiles)
        {
            Vector2Int cPos = TileHelper.PositionToCoordinate(candidate.transform.position);
            int dx = Mathf.Abs(cPos.x - targetPos.x);
            int dz = Mathf.Abs(cPos.y - targetPos.y);
            int distanceToTarget = Mathf.Max(dx, dz);

            if (distanceToTarget <= maxRange)
            {
                // We want the tile that gives the largest distanceToTarget
                if (distanceToTarget > bestDistanceToTarget)
                {
                    bestDistanceToTarget = distanceToTarget;
                    bestTile = candidate;
                }
            }
        }

        if (bestTile != GetCurrentTile())
        {
            yield return MoveTo(bestTile.transform);
        }

        // Attempt to use ranged action
        if (rangedAction != null)
        {
            List<Tile> tilesInRange = TileGridManager.Instance.FindTilesInRange(GetCurrentTile(), rangedAction.ActionRange);
            if (tilesInRange.Contains(targetPlayer.GetCurrentTile()))
            {
                rangedAction.UseAction(this, targetPlayer);
            }
        }

        GameManager.Instance.CallEndTurn();
    }
}
