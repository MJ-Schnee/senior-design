using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    enum UiState
    {
        Idle,
        PerformingAction,
        Moving,
    }

    public static UiManager Instance;

    public GameObject TurnIconPrefab;

    public Transform UpNextPanel;

    [SerializeField] Image playerImage;

    [SerializeField] TMP_Text playerName, playerAc, playerHp_max, playerHp_curr, playerSpeed;

    List<GameObject> turnIcons;

    private Player currentPlayer;

    private UiState uiState;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        turnIcons = new();

        uiState = UiState.Idle;
    }

    void Start()
    {
        GameManager.OnEndTurn += HandleEndTurn;
        
        UpdatePlayerPanel(GameManager.Instance.TurnOrder.GetCurrentTurn());
        UpdateUpNextPanel();
    }

    void UpdatePlayerPanel(Player player)
    {
        
        int playerNum = int.Parse(player.name[7..]);
        playerImage.color = player.IconColor;
        playerName.text = $"Player {playerNum}";
        playerAc.text = "12";
        playerHp_max.text = "20";
        playerHp_curr.text = "20";
        playerSpeed.text = player.PlayerSpeed.ToString();
    }

    void UpdateUpNextPanel()
    {
        // Destroy all current icons in panel
        foreach (var icon in turnIcons)
        {
            Destroy(icon);
        }
        turnIcons.Clear();

        // Draw all icons (repeat until screen is filled)
        var turnOrder = GameManager.Instance.TurnOrder.GetTurnOrder();
        int maxIcons = 18;
        for (int i = 0; i < maxIcons; i++)
        {
            var player = turnOrder[i % turnOrder.Count];
            GameObject icon = Instantiate(TurnIconPrefab, UpNextPanel);

            // Example coloring for different players
            icon.TryGetComponent(out Image iconImage);
            int playerNum = int.Parse(player.name[7..]);
            icon.name = $"Player {playerNum} icon ({i})";
            iconImage.color = player.IconColor;

            turnIcons.Add(icon);
        }
    }

    void HandleEndTurn(Player nextTurnPlayer)
    {
        UpdatePlayerPanel(nextTurnPlayer);
        UpdateUpNextPanel();
        currentPlayer = nextTurnPlayer;
    }

    public void OnMoveButtonClick()
    {
        if (uiState == UiState.Moving)
        {
            uiState = UiState.Idle;
        }
        else
        {
            uiState = UiState.Moving;
        }

        if (uiState == UiState.Moving)
        {
            TileGridManager.Instance.HighlightReachableTiles(
                (int)Math.Round(currentPlayer.transform.position.x),
                (int)Math.Round(currentPlayer.transform.position.z),
                currentPlayer.RemainingSpeed
            );
        }
        else
        {
            TileGridManager.Instance.UnhighlightAllTiles();
        }
    }

    public IEnumerator HandleTileClick(Tile tileClicked)
    {
        if (uiState == UiState.Moving)
        {
            if (tileClicked.IsHighlighted)
            {
                uiState = UiState.PerformingAction;
                TileGridManager.Instance.UnhighlightAllTiles();

                // Keep UI in state until coroutine ends
                yield return StartCoroutine(currentPlayer.MoveTo(tileClicked.gameObject.transform));

                uiState = UiState.Idle;
            }
        }
    }
}
