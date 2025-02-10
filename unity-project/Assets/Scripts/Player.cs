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

    private bool isMyTurn = false;

    private Animator animator;

    void Awake()
    {
        GameManager.OnEndTurn += OnEndTurn;
        animator = GetComponent<Animator>();
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
    }

    public IEnumerator MoveTo(Transform newTransform)
    {
        float walkSpeed = 7.0f;
        float turnSpeed = 10.0f;

        (int, int) startTileLoc = ((int)transform.position.x, (int)transform.position.z);
        (int, int) endTileLoc = ((int)newTransform.position.x, (int)newTransform.position.z);
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
