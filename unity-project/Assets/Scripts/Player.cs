using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color IconColor;

    public int PlayerAc, PlayerHp_curr, PlayerHp_max, PlayerSpeed, RemainingSpeed;

    public Renderer TurnIdentifierRenderer;

    public Material ActiveTurnMaterial, InactiveTurnMaterial;

    private bool isMyTurn;

    [SerializeField]
    protected Animator animator;

    void Awake()
    {
        GameManager.OnEndTurn += OnEndTurn;
        PlayerHp_curr = PlayerHp_max;
    }

    void OnEndTurn(Player nextPlayer)
    {
        if (nextPlayer == this)
        {
            isMyTurn = true;
            TurnIdentifierRenderer.material = ActiveTurnMaterial;
            RemainingSpeed = PlayerSpeed;
        }
        else
        {
            isMyTurn = false;
            TurnIdentifierRenderer.material = InactiveTurnMaterial;
        }

        Tile currentTile = GetCurrentTile();
        currentTile.IsWalkable = isMyTurn;
    }

    public Tile GetCurrentTile()
    {
        int positionX = Mathf.RoundToInt(transform.position.x);
        int positionZ = Mathf.RoundToInt(transform.position.z);
        Tile currentTile = TileGridManager.Instance.getTile(positionX, positionZ);
        return currentTile;
    }

    // This function checks if the tile we are on is a door tile and an edge tile,
    // If it is then we create a new room, inputing the coordinates of the top cornor of the new room.
    // TODO make this work in the negative directions
    public void endT()
    {
        // Get our current position
        int positionX = Mathf.RoundToInt(transform.position.x);
        int positionZ = Mathf.RoundToInt(transform.position.z);

        // Checks to make sure this is a door tile and an edge tile
        // If its not an edge tile we know we already "opened" this door

        int checkE = TileGridManager.Instance.checkEdge(positionX, positionZ);
        Tile checkDoor = TileGridManager.Instance.getTile(positionX, positionZ);
        bool checkD = checkDoor.getDoor();
        if(checkD)
        {
            //Based on which direction the door is facing we create our new room
            if(checkE == 1)
            {
                // for the positive X direction
                if(positionX > 0)
                {
                    // for first new room Z is some number prob between -1 and 1
                    // we set that to 10 so its the top corner
                    // we know we are already at the edge of X so we just add the size of our new room
                    int Z = 10 * (Mathf.FloorToInt (positionZ/10) + 1);
                    TileGridManager.Instance.newRoom(positionX + 20, Z);
                }
                /*else
                {
                    TileGridManager.Instance.newRoom(positionX - 20, positionZ);
                }*/
            }
            else if(checkE == 2)
            {
                if(positionZ > 0)
                {
                    // for first new room X is some number prob between -1 and 1
                    // we set that to 10 so its the top corner
                    // we know we are already at the edge of Z so we just add the size of our new room
                    int X = 10 * (Mathf.FloorToInt (positionX/10) + 1);
                    TileGridManager.Instance.newRoom(X, positionZ + 20);
                }
                /*
                else
                {
                    TileGridManager.Instance.newRoom(positionX, positionZ - 20);
                }*/
            }
        }
    }

    public IEnumerator MoveTo(Transform newTransform)
    {
        float walkSpeed = 7.0f;
        float turnSpeed = 10.0f;

        (int, int) startTileLoc = (Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        (int, int) endTileLoc = (Mathf.RoundToInt(newTransform.position.x), Mathf.RoundToInt(newTransform.position.z));
        List<GameObject> tilePath = TileGridManager.Instance.FindRoute(startTileLoc, endTileLoc);
        
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
        animator.SetBool("IsMoving", true);
        if (tilePath != null && tilePath.Count > 0)
        {
            RemainingSpeed -= tilePath.Count;
            foreach (GameObject tile in tilePath)
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
            }
        }
        animator.SetBool("IsMoving", false);
        // Calling function to check if we ended on a door and edge tile
        // TODO make this work in the OnEndTurn function or 
        // otherwise run when the player hits end turn button
        this.endT();
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
}
