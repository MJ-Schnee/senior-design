using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central place for handling "select X targets" logic and
/// updating turn indicators to SELECTABLE, SELECTED, etc.
/// </summary>
public class ActionTargetingManager : MonoBehaviour
{
    public static ActionTargetingManager Instance;

    private BaseAction currentAction;
    private Player actingPlayer;

    // Everyone in range of the action
    private List<Player> selectablePlayers = new();

    // Players selected by the user
    private List<Player> selectedPlayers = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTargetSelection(BaseAction action, Player actingPlayer)
    {
        ClearSelection();

        currentAction = action;
        this.actingPlayer = actingPlayer;

        // Find all players in range
        Player[] allPlayers = FindObjectsOfType<Player>();
        int actionRange = (int)action.ActionDistance;
        Vector3 actingPos = actingPlayer.transform.position;
        foreach (var p in allPlayers)
        {
            if (p == actingPlayer)
            {
                continue;
            }

            int dx = Mathf.Abs(Mathf.RoundToInt(p.transform.position.x - actingPos.x));
            int dz = Mathf.Abs(Mathf.RoundToInt(p.transform.position.z - actingPos.z));
            if (dx + dz <= actionRange)
            {
                selectablePlayers.Add(p);
                p.TurnIdentifierRenderer.material = p.SelectableTurnIdMaterial;
            }
        }
    }

    /// <summary>
    /// Decide if click is valid, then handle selection states
    /// </summary>
    public void HandlePlayerClicked(Player player)
    {
        // If we aren't in a selection session, ignore
        if (currentAction == null) return;

        // If this player isn't in the selectable list, ignore
        if (!selectablePlayers.Contains(player)) return;

        // If already selected, ignore or unselect based on your design preference
        if (selectedPlayers.Contains(player)) return;

        // Mark as selected
        selectedPlayers.Add(player);
        player.TurnIdentifierRenderer.material = player.SelectedTurnIdMaterial;

        // If we have enough selected players, finalize
        if (selectedPlayers.Count >= currentAction.NumTargets)
        {
            FinalizeSelection();
        }
    }

    /// <summary>
    /// When we have chosen targets, automatically execute the action
    /// on the selected players, revert (un)selected turn indicators, and clean up
    /// </summary>
    private void FinalizeSelection()
    {
        // Revert unselected players to their normal (inactive) state
        foreach (var p in selectablePlayers)
        {
            if (!selectedPlayers.Contains(p))
            {
                p.TurnIdentifierRenderer.material = p.InactiveTurnMaterial;
            }
        }

        // Execute the action on the chosen targets
        foreach (var target in selectedPlayers)
        {
            currentAction.UseAction(actingPlayer, target);
        }

        // Wrap up
        ClearSelection();
    }

    /// <summary>
    /// Resets internal state and visuals.
    /// </summary>
    private void ClearSelection()
    {
        // Restore materials on all previously selectable players
        foreach (Player selectablePlayer in selectablePlayers)
        {
            // Possibly revert them to Inactive or normal if you prefer
            selectablePlayer.TurnIdentifierRenderer.material = selectablePlayer.InactiveTurnMaterial;
        }
        selectablePlayers.Clear();
        selectedPlayers.Clear();

        currentAction = null;
        actingPlayer = null;
    }
}
