using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerSpeed;

    private bool isMyTurn;

    private Renderer turnIdentifierRenderer;

    void Awake()
    {
        GameManager.OnEndTurn += OnEndTurn;
    }

    void Start()
    {
       GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
       cylinder.transform.position = transform.position;
       cylinder.transform.position += new Vector3(0,-.5f,0);
       cylinder.transform.localScale += new Vector3(.5f,-.95f,.5f);
       
       // Get the Renderer component from the new cube
       turnIdentifierRenderer = cylinder.GetComponent<Renderer>();
    }

    void OnEndTurn(Player nextPlayer)
    {
        if (nextPlayer == this)
        {
            isMyTurn = true;
            turnIdentifierRenderer.material.SetColor("_Color", Color.green);
        }
        else
        {
            isMyTurn = false;
            turnIdentifierRenderer.material.SetColor("_Color", Color.gray);
        }
    }

    public void MoveTo(Transform newTransform)
    {
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
    }
}
