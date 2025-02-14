using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Player> InitialPlayerList;

    public Enemy enemyFab;

    public CircularLinkedList<Player> TurnOrder;

    public static event Action<Player> OnEndTurn;

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        TurnOrder = new();

        foreach (Player player in InitialPlayerList)
        {
            AddTurn(player);
        }
    }

    void Start()
    {
        StartCoroutine(StartFirstTurn());
    }

    IEnumerator StartFirstTurn()
    {
        // Wait for everything to load
        yield return new WaitForEndOfFrame();
        OnEndTurn(TurnOrder.GetCurrentTurn());
    }

    public Player EndTurn()
    {
        Player nextTurn = TurnOrder.StartNextTurn();
        OnEndTurn(nextTurn);
        return nextTurn;
    }

    public void CallEndTurn()
    {
        EndTurn();
    }

    public void AddTurn(Player newPlayer)
    {
        TurnOrder.Add(newPlayer);
    }

    public bool RemoveTurn(Player removedPlayer)
    {
        return TurnOrder.Remove(removedPlayer);
    }
    // TODO: Make more enemy options so it's actually random lol
    public Enemy GenerateRandomEnemy(int x, int y) {
        Enemy e = Instantiate(enemyFab, new Vector3(x,0,y), Quaternion.identity, transform);
        AddTurn(e);
        return e;
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
