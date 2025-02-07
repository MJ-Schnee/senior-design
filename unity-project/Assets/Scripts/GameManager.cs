using System;
using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CircularLinkedList<GameObject> TurnOrder;

    public static event Action<GameObject> OnEndTurn;

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        TurnOrder = new();

        
        // Initial test "players"
        for (var i = 0; i < 4; i++)
        {
            GameObject newObj = new($"Player {i}");
            AddTurn(newObj);
        }
    }

    public GameObject EndTurn()
    {
        GameObject nextTurn = TurnOrder.StartNextTurn();
        OnEndTurn(nextTurn);
        return nextTurn;
    }

    public void CallEndTurn()
    {
        EndTurn();
    }

    public void AddTurn(GameObject newPlayer)
    {
        TurnOrder.Add(newPlayer);
    }

    public bool RemoveTurn(GameObject removedPlayer)
    {
        return TurnOrder.Remove(removedPlayer);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager GameManagerScript = (GameManager)target;

        if (GUILayout.Button("End Turn"))
        {
            GameManagerScript.EndTurn();
        }
    }
}

#endif
