using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Player> InitialPlayerList;

    public List<Player> CharacterIDToPrefabList;
    public Enemy enemyFab;

    public CircularLinkedList<Player> TurnOrder;

    public static event Action<Player> OnEndTurn;

    private int killCount = 0;
    public int KillCount
    {
        get { return killCount; }
        set { killCount = value; }
    }

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        if (MainMenuManager.Instance != null) {
            foreach (Player player in InitialPlayerList)
            {
                player.transform.position = new Vector3(0, 1000, 0);
                // This doesn't actually destroy the mesh?
                DestroyImmediate(player);
            }
            InitialPlayerList = new List<Player>();
            for (int i = 0; i < 4; i++)
            {
                int cid = MainMenuManager.Instance.CharacterList[i];
                if (cid > 0) {
                    Player p = Instantiate(CharacterIDToPrefabList[cid - 1], new Vector3(((i >> 1) << 1)+9,0,((i << 31) >> 30)+9), Quaternion.identity, transform);
                    p.name = $"Player {i+1}";
                    InitialPlayerList.Add(p);
                }
            }
        }
        TurnOrder = new();
    }

    void Start()
    {
        
        // Wait for UI to Awake
        foreach (Player player in InitialPlayerList)
        {
            AddTurn(player);
        }
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

    public void AddTurn(Player newPlayer, bool afterCurrentPlayer = false)
    {
        if (afterCurrentPlayer)
        {
            TurnOrder.AddAfter(newPlayer, TurnOrder.GetCurrentTurn());
        }
        else
        {
            TurnOrder.AddToEnd(newPlayer);
        }
        UiManager.Instance.UpdateUpNextPanel();
    }

    public bool RemoveTurn(Player removedPlayer)
    {
        bool removed = TurnOrder.Remove(removedPlayer);
        UiManager.Instance.UpdateUpNextPanel();
        return removed;
    }

    // TODO: Make more enemy options so it's actually random lol
    public Enemy GenerateRandomEnemy(int x, int y) {
        Enemy e = Instantiate(enemyFab, new Vector3(x,0,y), Quaternion.identity, transform);
        AddTurn(e, afterCurrentPlayer: true);
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
