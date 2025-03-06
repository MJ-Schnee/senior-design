using UnityEngine;

[CreateAssetMenu(menuName = "Actions/AcidThrow")]
public class AcidThrow : BaseAction
{
    public GameObject BottlePrefab;

    public int BottleSpeed;

    public int SplashRadius = 2;

    public Vector3 SpawnOffset;

    private Vector3 bottleSpawnPos;

    private Quaternion bottleSpawnRot;

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
        bottleSpawnPos = actingPlayer.transform.position + SpawnOffset;
        bottleSpawnRot = Quaternion.LookRotation((targetPlayer.transform.position - bottleSpawnPos).normalized);
    }

    /// <summary>
    /// Triggered from animation timing, spawning the actual acid bottle
    /// </summary>
    public override void ApplyImpact(Player targetPlayer)
    {
        GameObject bottleGO = Instantiate(BottlePrefab, bottleSpawnPos, bottleSpawnRot);
        if (bottleGO.TryGetComponent<ProjectileAoe>(out var bottleScript))
        {
            bottleScript.Launch(
                targetPlayer,
                BottleSpeed,
                ActionDamage,
                SplashRadius
            );
        }
    }
}
