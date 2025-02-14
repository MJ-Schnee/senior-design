using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : Player
{
    public int enemyHitBonus, enemyDamage;
    public int enemyAttackRange = 1;

    // Returns the closest alive player, distance to the player, and the x,y coords of the player
    // Equidistant players are chosen at random
    public (Player,int,(int,int)) FindNearestTarget() {
        List<Player> c = new();
        int cdist = 1000000;
        foreach (Player player in GameManager.Instance.InitialPlayerList) {
            if (player.PlayerHp_curr > 0) {
                int dist = (int)Mathf.Max(Mathf.Abs(player.transform.position.x - transform.position.x), Mathf.Abs(player.transform.position.y - transform.position.y));
                if (dist < cdist) {
                    c = new();
                    cdist = dist;
                    c.Add(player);
                }
                else if (dist == cdist) {
                    c.Add(player);
                    }
            }
        }
        Player chosen = c[Random.Range(0,c.Count)];
        return (chosen,cdist,((int)chosen.transform.position.x, (int)chosen.transform.position.y));
    }
    // Returns the Tile closest to the nearest alive player within the enemy's movement speed and not overlaping with that player
    // TODO: make A* routes through walls and other enemies and players impossible for better routing
    public (Transform, bool) FindMovementDestination(int distance, (int, int) coords) {
        List<GameObject> Route = TileGridManager.Instance.FindRoute(((int)transform.position.x, (int)transform.position.y), coords);
        int dist = Mathf.Max(Mathf.Min(Route.Count - 1, this.PlayerSpeed), 0);
        if (Route.Count == 0) {return (transform, true);}
        GameObject endTile = Route[dist];
        return (endTile.transform, dist <= Route.Count - 1 - enemyAttackRange);
    }
    // Enemy Turn AI
    // Currently just moves the enemy as far as it can toward the closest alive player and attacks if the enemy is right next to the player
    // There is no attack animation right now
    // Remember to call this in a coroutine
    private IEnumerator EnemyTurnAI() {
        // Move the enemy toward a player
        (Player Target ,int distance ,(int,int) coords) = FindNearestTarget();
        (Transform destination, bool adjacentToTarget) = FindMovementDestination(distance, coords);
        if (distance > 1) {yield return MoveTo(destination);}
        
        // Attack if possible
        if (adjacentToTarget) {
            // A Hit, TODO: Hit animation
            if (Random.Range(1,21) + enemyHitBonus >= Target.PlayerAc) { Target.PlayerHp_curr = Mathf.Max(0, Target.PlayerHp_curr - enemyDamage);}
            // A Miss, TODO: Miss animation
            else {}
        }
        this.endT();
        GameManager.Instance.CallEndTurn();
    }
    void OnEndTurn(Player nextPlayer)
    {
        if (nextPlayer == this)
        {
            TurnIdentifierRenderer.material = ActiveTurnMaterial;
            StartCoroutine(EnemyTurnAI());
        }
        else
        {
            TurnIdentifierRenderer.material = InactiveTurnMaterial;
        }
    }
    void Awake()
    {
        GameManager.OnEndTurn += this.OnEndTurn;
        animator = GetComponent<Animator>();
        PlayerHp_curr = PlayerHp_max;
    }
}