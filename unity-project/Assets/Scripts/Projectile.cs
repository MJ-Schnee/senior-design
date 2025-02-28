using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Player targetPlayer;
    private float speed;
    private int damage;

    /// <summary>
    /// Used to initialize projectile
    /// </summary>
    public virtual void Launch(Player target, float speed, int damage)
    {
        targetPlayer = target;
        this.speed = speed;
        this.damage = damage;
    }

    void Update()
    {
        // Move projectile towards target
        // (1.5f is some vertical offset)
        Vector3 targetPos = targetPlayer.transform.position + Vector3.up * 1.5f; 

        Vector3 direction = targetPos - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // If we're close enough to "hit" the target
        if (direction.magnitude <= distanceThisFrame)
        {
            OnImpact();
        }
        else
        {
            // Move forward
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
            // Rotate to face direction
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// Apply damage to hit player
    /// </summary>
    protected virtual void OnImpact()
    {
        // Roll D20 against AC for hit
        int diceResult = Random.Range(1, 21);
        if (diceResult >= targetPlayer.PlayerAc)
        {
            targetPlayer.DealDamage(damage);
            UiManager.Instance.ShowHitMissIndicator("HIT", Color.green, targetPlayer.transform);
        }
        else
        {
            UiManager.Instance.ShowHitMissIndicator("MISS", Color.red, targetPlayer.transform);
        }

        Destroy(gameObject);
    }
}
