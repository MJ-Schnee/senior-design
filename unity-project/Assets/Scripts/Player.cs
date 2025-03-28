using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color IconColor;
    public Sprite Icon;

    public int PlayerAc, PlayerHp_curr, PlayerHp_max, PlayerSpeed, RemainingSpeed;

    public Renderer TurnIdentifierRenderer;

    public Material
        ActiveTurnMaterial,
        InactiveTurnMaterial,
        SelectedTurnIdMaterial,
        SelectableTurnIdMaterial;

    private bool isMyTurn;

    public Animator Animator;

    public BaseAction Action1, Action2, Action3, Action4;

    public BaseAction currentAction;
    
    public Player currentActionTarget;

    public bool IsDead { get; private set; }

    void Awake()
    {
        IsDead = false;
        PlayerHp_curr = PlayerHp_max;
    }

    public void AddEndTurnCall()
    {
        GameManager.OnEndTurn += OnEndTurn;
    }
    /// <summary>
    /// Triggered every time a turn ends
    /// </summary>
    void OnEndTurn(Player nextPlayer)
    {
        Tile currentTile = GetCurrentTile();
        if (nextPlayer == this)
        {
            if (IsDead)
            {
                if (GameManager.Instance.TeamRevives > 0)
                {
                    RevivePlayer();
                }
                else
                {
                    GameManager.Instance.GameOver();
                }
            }
            // At the start of our turn if we are in a room that is a damage
            // while in room we do the damage
            if (currentTile.getDot())
            {
                this.DealDamage(1);
            }
            isMyTurn = true;
            UiManager.Instance.SetMoveUsable(true);
            UiManager.Instance.SetActionsUsable(true);
            TurnIdentifierRenderer.material = ActiveTurnMaterial;
            RemainingSpeed = PlayerSpeed;
        }
        else
        {
            isMyTurn = false;
            TurnIdentifierRenderer.material = InactiveTurnMaterial;
        }

        currentTile.IsWalkable = isMyTurn;
    }

    /// <summary>
    /// Returns the tile the player is currently standing on
    /// </summary>
    public Tile GetCurrentTile()
    {
        Vector2Int gridLoc = TileHelper.PositionToCoordinate(transform.position);
        Tile currentTile = TileGridManager.Instance.GetTileAtLocation(gridLoc.x, gridLoc.y);
        return currentTile;
    }

    /// <summary>
    /// If on an edge door tile, trigger new room generation
    /// </summary>
    public void DiscoverRoom()
    {
        Tile currentTile = GetCurrentTile();
        if (!currentTile.getDoor())
            return;

        // Calculate the coordinates for the new room based on the edge direction
        Vector2Int currentTilePosition = TileHelper.PositionToCoordinate(currentTile.transform.position);
        Vector2Int edgeDirection = TileGridManager.Instance.GetEdgeDirection(currentTilePosition.x, currentTilePosition.y);
        if (edgeDirection == Vector2Int.zero)
        {
            // Non-edge door
            return;
        }

        Vector2Int newRoomCoordinates = CalculateNewRoomCoordinates(currentTilePosition, edgeDirection);
        TileGridManager.Instance.CreateRandomRoom(newRoomCoordinates.x, newRoomCoordinates.y);

        //If the room we spawned was a trap on creation room we do damage here.
        if(TileGridManager.Instance.getTrap())
        {
            Animator.SetTrigger("Hurt");
            this.DealDamage(5);
        }
    }

    /// <summary>
    /// Helper function to calculate coordinates for new rooms
    /// </summary>
    private Vector2Int CalculateNewRoomCoordinates(Vector2Int currentTilePosition, Vector2Int edgeDirection)
    {
        int roomWidth = TileGridManager.Instance.GetRoomWidth();
        int roomHeight = TileGridManager.Instance.GetRoomLength();

        int x = currentTilePosition.x;
        int y = currentTilePosition.y;

        if (edgeDirection == Vector2Int.right)
        {
            y = CalculateRoomBoundary(y, roomHeight);
            return new Vector2Int(x + roomWidth, y);
        }

        if (edgeDirection == Vector2Int.up)
        {
            x = CalculateRoomBoundary(x, roomWidth);
            return new Vector2Int(x, y + roomHeight);
        }

        if (edgeDirection == Vector2Int.left)
        {
            y = CalculateRoomBoundary(y, roomHeight);
            return new Vector2Int(x, y);
        }

        if (edgeDirection == Vector2Int.down)
        {
            x = CalculateRoomBoundary(x, roomWidth);
            return new Vector2Int(x, y);
        }

        return Vector2Int.zero;
    }

    /// <summary>
    /// Helper function to calculate boundary of new rooms
    /// </summary>
    private int CalculateRoomBoundary(int position, int roomSize)
    {
        if (position > 0)
        {
            return roomSize * (Mathf.FloorToInt(position / roomSize) + 1);
        }
        else
        {
            return roomSize * Mathf.FloorToInt(position / roomSize);
        }
    }

    /// <summary>
    /// Moves player through grid to new position's tile
    /// Coroutine finishes once player arrives
    /// </summary>
    public IEnumerator MoveTo(Transform newTransform)
    {
        float walkSpeed = 8.0f;
        float turnSpeed = 15.0f;

        Vector2Int startTileLoc = TileHelper.PositionToCoordinate(transform.position);
        Vector2Int endTileLoc = TileHelper.PositionToCoordinate(newTransform.position);
        
        List<Tile> tilePath = TileGridManager.Instance.AStarSearch(startTileLoc, endTileLoc);
        
        // Draw line for debug path
        for (int i = 0; i < tilePath.Count - 1; i++)
        {
            Vector3 startTile = tilePath[i].transform.position;
            startTile.y += 1;
            Vector3 endTile = tilePath[i + 1].transform.position;
            endTile.y += 1;
            Debug.DrawLine(startTile, endTile, Color.red, 2.0f);
        }

        // Move player along path
        Animator.SetBool("IsMoving", true);
        if (tilePath != null && tilePath.Count > 0)
        {
            RemainingSpeed -= tilePath.Count;
            UiManager.Instance.SetMoveUsable(false);
            UiManager.Instance.UpdatePlayerPanel(this);
            foreach (Tile tile in tilePath)
            {
                Vector3 targetPosition = tile.transform.position;
                targetPosition.y = transform.position.y;

                // Rotation
                Vector3 direction = (targetPosition - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                        yield return null;
                    }
                }

                // Position
                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkSpeed * Time.deltaTime);
                    yield return null;
                }
                transform.position = targetPosition;
            }
        }
        if (RemainingSpeed > 0)
        {
            UiManager.Instance.SetMoveUsable(true);
        }
        Animator.SetBool("IsMoving", false);

        DiscoverRoom();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UiManager.Instance.InspectorCoroutine = UiManager.Instance.UpdatePlayerInspector(this);
        StartCoroutine(UiManager.Instance.InspectorCoroutine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UiManager.Instance.HidePlayerInspector();
        StopCoroutine(UiManager.Instance.InspectorCoroutine);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ActionTargetingManager.Instance.HandlePlayerClicked(this);
    }

    /// <summary>
    /// Deals damage to this player and plays the hurt animation
    /// </summary>
    public void DealDamage(int amount)
    {
        PlayerHp_curr = Mathf.Max(PlayerHp_curr - amount, 0);

        // Only need to update player panel if current player is damaged
        if (this == GameManager.Instance.TurnOrder.GetCurrentTurn())
        {
            UiManager.Instance.UpdatePlayerPanel(this);
        }

        Animator.SetTrigger("Hurt");

        if (PlayerHp_curr <= 0)
        {
            KillPlayer();
        }
    }

    /// <summary>
    /// Heals player health up to their maximum
    /// </summary>
    public void HealPlayer(int amount)
    {
        PlayerHp_curr = Mathf.Min(PlayerHp_curr + amount, PlayerHp_max);

        // Only need to update player panel if current player is damaged
        if (this == GameManager.Instance.TurnOrder.GetCurrentTurn())
        {
            UiManager.Instance.UpdatePlayerPanel(this);
        }
    }

    /// <summary>
    /// Generic event triggered by action animation to apply action's effects
    /// </summary>
    public void OnActionImpact()
    {
        Debug.Log($"{name}'s OnActionImpact event triggered.");

        currentAction.ApplyImpact(currentActionTarget);
    }

    /// <summary>
    /// Kills this player.
    /// For now, just triggers death animation.
    /// </summary>
    protected virtual void KillPlayer()
    {
        Debug.Log($"{name} died");
        Animator.SetTrigger("Killed");
    }

    /// <summary>
    /// Function called after death animation finishes.
    /// </summary>
    protected virtual void OnDeath()
    {
        IsDead = true;
    }

    /// <summary>
    /// Uses a team revive to bring a player back from the dead and restore the player back to half-health.
    /// </summary>
    protected void RevivePlayer()
    {
        if (!IsDead)
        {
            return;
        }

        Animator.SetTrigger("Revived");
        IsDead = false;
        GameManager.Instance.TeamRevives--;
        UiManager.Instance.UpdateReviveIcons();
        PlayerHp_curr = PlayerHp_max / 2;

        // No need to update player panel if current player isn't one being revived
        if (this == GameManager.Instance.TurnOrder.GetCurrentTurn())
        {
            UiManager.Instance.UpdatePlayerPanel(this);
        }
    }
}
