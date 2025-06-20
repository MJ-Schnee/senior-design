using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Quest quest;
    public List<Player> InitialPlayerList;

    public List<Player> CharacterIDToPrefabList;

    public List<Enemy> enemyPrefabs;

    public CircularLinkedList<Player> TurnOrder;

    public static event Action<Player> OnEndTurn;

    private int killCount = 0;
    private bool treasureRoom = false;

    public UnityEvent KillEvent = new UnityEvent();
    public int KillCount
    {
        get { return killCount; }
        set { killCount = value; KillEvent.Invoke();}
    }

    public UnityEvent TreasureEvent = new UnityEvent();
    public bool TreasureRoom
    {
        get { return treasureRoom; }
        set {treasureRoom = value; TreasureEvent.Invoke();}
    }

    [SerializeField]
    private int teamRevives = 2;
    public int TeamRevives { get => teamRevives; set => teamRevives = value; }

    [SerializeField]
    private GameObject gameOverScreen;

    [SerializeField]
    private GameObject gameWinScreen;
    
    [SerializeField]
    private GameObject MenuScreen;

    [SerializeField]
    private GameObject GameGui;

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        if (MainMenuManager.Instance != null) {
            foreach (Player player in InitialPlayerList)
            {
                DestroyImmediate(player.gameObject);
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
            quest = (Quest)GameObject.Find("GameManager").AddComponent(MainMenuManager.Instance.QuestSelection) ;
            DestroyImmediate(MainMenuManager.Instance.gameObject);    
            MainMenuManager.Instance = null;     
        }
        else
        {
            quest = GameObject.Find("GameManager").AddComponent<TreasureQuest>();
        }
        quest.StartQuest();
        TurnOrder = new();
    }

    void Start()
    {
        OnEndTurn = default;
        Instance = this;
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
        newPlayer.AddEndTurnCall();
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

    /// <summary>
    /// Spawns a random enemy from list of enemy prefabs at location
    /// </summary>
    public Enemy GenerateRandomEnemy(int x, int y) {
        int randomEnemy = UnityEngine.Random.Range(0, enemyPrefabs.Count);
        Enemy enemy = Instantiate(enemyPrefabs[randomEnemy], new Vector3(x, 0, y), Quaternion.identity, transform);
        AddTurn(enemy, afterCurrentPlayer: true);
        return enemy;
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }

    public void GameWin()
    {
        gameWinScreen.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
    
    public void ExitToDesktop()
    {
        Application.Quit();
    }

    public void OpenMenu()
    {
        MenuScreen.SetActive(true);
        GameGui.SetActive(false);
    }

    public void CloseMenu()
    {
        GameGui.SetActive(true);
        MenuScreen.SetActive(false);
    }

    public void Fullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void OnDestroy()
    {
        TurnOrder.Clear();
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
