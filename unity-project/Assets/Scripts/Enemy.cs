using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    public int enemyHitBonus, enemyDamage;
    public int enemyAttackRange = 1;

    // Returns the closest alive player, distance to the player, and the x,y coords of the player
    // Equidistant players are chosen at random
    public (Player, int, (int, int)) FindNearestTarget()
    {
        List<Player> c = new();
        int shortestDist = int.MaxValue;
        foreach (Player player in GameManager.Instance.InitialPlayerList) {
            if (player.PlayerHp_curr > 0) {
                int dist = (int)Vector3.Distance(player.transform.position, transform.position);
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
        return (chosen, shortestDist, ((int)chosen.transform.position.x, (int)chosen.transform.position.z));
    }

    // Returns tile transform closest to the nearest alive player within the enemy's movement speed and if that tile is adjacent to player
    public (Transform, bool) FindMovementDestination((int, int) playerCoords)
    {
        (int, int) startCoords = ((int)transform.position.x, (int)transform.position.z);
        List<GameObject> route = TileGridManager.Instance.FindRoute(startCoords, playerCoords);
        
        int dist = Mathf.Clamp(route.Count - 1, 0, PlayerSpeed);
        if (route.Count == 0)
        {
            return (transform, true);
        }
        
        GameObject endTile = route[dist];
        return (endTile.transform, dist <= route.Count - 1 - enemyAttackRange);
    }

    // Enemy Turn AI
    // Currently just moves the enemy as far as it can toward the closest alive player and attacks if the enemy is right next to the player
    // There is no attack animation right now
    // Remember to call this in a coroutine
    private IEnumerator EnemyTurnAI()
    {
        // Move the enemy toward a player
        (Player Target, int distance, (int, int) coords) = FindNearestTarget();
        (Transform destination, bool adjacentToTarget) = FindMovementDestination(coords);
        if (distance > 1)
        {
            yield return MoveTo(destination);
        }
        
        // Attack if possible
        if (adjacentToTarget)
        {
            // Check for his or miss
            int enemyHit = Random.Range(1,21) + enemyHitBonus;
            if (enemyHit >= Target.PlayerAc)
            {
                // TODO: Damage player, Hit animation
                // Target.PlayerHp_curr = Mathf.Max(0, Target.PlayerHp_curr - enemyDamage);
            }
            else
            {
                // TODO: Miss animation
            }
        }
        this.endT();
        GameManager.Instance.CallEndTurn();
    }

    void OnEndTurn(Player nextPlayer)
    {
        Tile currentTile = GetCurrentTile();

        if (nextPlayer == this)
        {
            currentTile.IsWalkable = true;
            TurnIdentifierRenderer.material = ActiveTurnMaterial;
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
        animator = GetComponent<Animator>();
        PlayerHp_curr = PlayerHp_max;
    }
}