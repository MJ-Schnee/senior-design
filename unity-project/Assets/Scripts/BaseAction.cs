using UnityEngine;

public abstract class BaseAction : ScriptableObject
{
    public string ActionName;

    public string ActionDescription;

    public Color ActionColor;

    public Animation ActionAnim;

    public uint NumTargets;

    public uint ActionDistance;

    public abstract void UseAction(Player ActingPlayer, Player TargetPlayer);
}
