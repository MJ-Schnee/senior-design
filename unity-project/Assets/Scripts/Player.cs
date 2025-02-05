using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public class Player : MonoBehaviour
{
    public int turn;
    public bool myTurn;
    private Renderer cubeRenderer;

    // Start is called before the first frame update
    void Start()
    {
        myTurn = true;
       GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
       cylinder.transform.position = this.transform.position;
       cylinder.transform.position += new Vector3(0,-.5f,0);
       cylinder.transform.localScale += new Vector3(.5f,-.95f,.5f);
       // Get the Renderer component from the new cube
       cubeRenderer = cylinder.GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if(myTurn)
        {
            cubeRenderer.material.SetColor("_Color", Color.green);
        }
        else
        {
            cubeRenderer.material.SetColor("_Color", Color.gray);
        }
    }
}
