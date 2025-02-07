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
}
