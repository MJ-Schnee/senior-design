using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    // Returns the closest alive player, distance to the player, and tile the player is on
    // Equidistant players are chosen at random
    public (Player, int, Tile) FindNearestTarget()
    {
        List<Player> c = new();
        int shortestDist = int.MaxValue;
        foreach (Player player in GameManager.Instance.InitialPlayerList) {
            if (player.PlayerHp_curr > 0) {
                int dist = Mathf.RoundToInt(Vector3.Distance(player.transform.position, transform.position));
                if (dist < shortestDist) {
                    c.Clear();
                    shortestDist = dist;
                    c.Add(player);
                }
                else if (dist == shortestDist) {
                    c.Add(player);
                }
            }
        }
        Player chosen = c[Random.Range(0, c.Count)];
        return (chosen, shortestDist, chosen.GetCurrentTile());
    }

    // Returns tile transform closest to the nearest alive player within the enemy's movement speed
    public Transform FindMovementDestination(Tile playerTile)
    {
        Vector2Int currentTilePosition = TileHelper.PositionToCoordinate(GetCurrentTile().transform.position);
        Vector2Int playerTilePosition = TileHelper.PositionToCoordinate(playerTile.transform.position);
        List<Tile> route = TileGridManager.Instance.AStarSearch(currentTilePosition, playerTilePosition);
        
        int dist = Mathf.Clamp(route.Count - 1, 0, PlayerSpeed);
        if (route.Count == 0)
        {
            return transform;
        }
        
        Tile endTile = route[dist];
        return endTile.transform;
    }

    /// <summary>
    /// Simple enemy AI: attempts to move next to closest player and then attack with the first attack available.
    /// </summary>
    private IEnumerator EnemyTurnAI()
    {
        // Move the enemy toward a player
        (Player targetPlayer, int distFromTarget, Tile targetTile) = FindNearestTarget();
        Transform destination = FindMovementDestination(targetTile);
        if (distFromTarget > 1)
        {
            yield return MoveTo(destination);
        }
        
        // Attack if possible
        BaseAction[] actions = {Action1, Action2, Action3, Action4};
        List<Tile> tilesInRange = TileGridManager.Instance.FindTilesInRange(GetCurrentTile(), Action1.ActionRange);
        foreach (BaseAction action in actions)
        {
            if (action != null)
            {
                if (tilesInRange.Contains(targetPlayer.GetCurrentTile()))
                {
                    action.UseAction(this, targetPlayer);
                }
            }
        }
        GameManager.Instance.CallEndTurn();
    }

    /// <summary>
    /// Triggered every time a turn ends
    /// </summary>
    /// <param name="nextPlayer"></param>
    void OnEndTurn(Player nextPlayer)
    {
        Tile currentTile = GetCurrentTile();

        if (nextPlayer == this)
        {
            currentTile.IsWalkable = true;
            TurnIdentifierRenderer.material = ActiveTurnMaterial;
            RemainingSpeed = PlayerSpeed;
            StartCoroutine(EnemyTurnAI());
        }
        else
        {
            TurnIdentifierRenderer.material = InactiveTurnMaterial;
            currentTile.IsWalkable = false;
        }
    }

    void Awake()
    {
        GameManager.OnEndTurn += OnEndTurn;
        Animator = GetComponentInChildren<Animator>();
        PlayerHp_curr = PlayerHp_max;
        // TODO: Individualize icons by enemy type
        IconColor = Color.red;
    }

    /// <summary>
    /// Function called after death animation finishes.
    /// Destroys game object, removes from turn order, and increases kill count.
    /// </summary>
    protected override void OnDeath()
    {
        Debug.Log($"{name} has been removed and its kill counted!");
        GameManager.Instance.KillCount++;
        GameManager.Instance.RemoveTurn(this);
        gameObject.SetActive(false);
        Destroy(gameObject);
        GameManager.OnEndTurn -= OnEndTurn;
    }
}
