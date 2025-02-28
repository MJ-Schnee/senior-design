using UnityEngine;

[CreateAssetMenu(menuName = "Actions/MarksmanBolt")]
public class MarksmanBolt : BaseAction
{
    public GameObject BoltPrefab;

    public int BoltSpeed;

    public Vector3 SpawnOffset;

    private Vector3 boltSpawnPos;

    private Quaternion boltSpawnRot;

    /// <summary>
    /// Triggers action animation which applies upon animation event ApplyImpact
    /// </summary>
    public override void UseAction(Player actingPlayer, Player targetPlayer)
    {
        Debug.Log($"{actingPlayer.name} is {name}ing {targetPlayer.name}!");

        actingPlayer.currentAction = this;
        actingPlayer.currentActionTarget = targetPlayer;

        // Shooter faces target
        Vector3 targetPosition = new(targetPlayer.transform.position.x, actingPlayer.transform.position.y, targetPlayer.transform.position.z) ;
        actingPlayer.transform.LookAt(targetPosition);

        // Play animation, which triggers ApplyImpact
        actingPlayer.Animator.SetTrigger(AnimationTrigger);
        boltSpawnPos = actingPlayer.transform.position + SpawnOffset;
        boltSpawnRot = Quaternion.LookRotation((targetPlayer.transform.position - boltSpawnPos).normalized);
    }

    /// <summary>
    /// Triggered from animation timing, spawning the actual bolt object
    /// </summary>
    public override void ApplyImpact(Player targetPlayer)
    {
        GameObject boltGO = Instantiate(BoltPrefab, boltSpawnPos, boltSpawnRot);
        if (boltGO.TryGetComponent<Projectile>(out var boltScript))
        {
            boltScript.Launch(
                targetPlayer,
                BoltSpeed,
                ActionDamage
            );
        }
    }
}
