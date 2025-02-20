using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    enum UiState
    {
        Idle,
        PerformingAction,
        Moving,
        ActionTargeting,
    }

    public static UiManager Instance;

    public GameObject TurnIconPrefab;

    public Transform UpNextPanel;

    #region Player Panel Stats
    [Header("Player Panel Stats")]
    [SerializeField]
    Image playerImage;

    [SerializeField]
    TMP_Text playerName;
    
    [SerializeField]
    TMP_Text playerAc;
    
    [SerializeField]
    TMP_Text playerHp_max;
    
    [SerializeField]
    TMP_Text playerHp_curr;
    
    [SerializeField]
    TMP_Text playerSpeed;
    #endregion

    #region Player Inspector Stats
    [Header("Player Inspector Stats")]
    [SerializeField]
    GameObject otherPlayerStats;

    [SerializeField]
    Image otherPlayerImage;

    [SerializeField]
    TMP_Text otherPlayerName;
    
    [SerializeField]
    TMP_Text otherPlayerAc;
    
    [SerializeField]
    TMP_Text otherPlayerHp_max;
    
    [SerializeField]
    TMP_Text otherPlayerHp_curr;
    
    [SerializeField]
    TMP_Text otherPlayerSpeed;
    #endregion

    List<GameObject> turnIcons;

    private Player currentPlayer;

    private UiState uiState;
    
    public float InspectorTimerSec = 0.5f;

    public IEnumerator InspectorCoroutine;

    #region Action Buttons
    [Header("Action Buttons")]
    [SerializeField]
    GameObject ActionButton1;
    
    [SerializeField]
    GameObject ActionButton2;
    
    [SerializeField]
    GameObject ActionButton3;
    
    [SerializeField]
    GameObject ActionButton4;
    #endregion

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

        Button playerImageButton = playerImage.GetComponent<Button>();
        playerImageButton.onClick.AddListener(() => CameraController.Instance.CenterObject(currentPlayer.gameObject));
    }

    public void UpdatePlayerPanel(Player player)
    {
        // Update player stats
        playerImage.color = player.IconColor;
        playerName.text = player.name;
        playerAc.text = player.PlayerAc.ToString("D2");
        playerHp_max.text = player.PlayerHp_max.ToString("D2");
        playerHp_curr.text = player.PlayerHp_curr.ToString("D2");
        playerSpeed.text = player.RemainingSpeed.ToString("D2");

        // Update action buttons
        BaseAction[] playerActions = {player.Action1, player.Action2, player.Action3, player.Action4};
        GameObject[] actionButtons = {ActionButton1, ActionButton2, ActionButton3, ActionButton4};
        for (int i = 0; i < 4; i++)
        {
            BaseAction playerAction = playerActions[i];
            GameObject actionButton = actionButtons[i];

            if (playerAction == null)
            {
                actionButton.SetActive(false);
            }
            else
            {
                actionButton.SetActive(true);

                // Visual changes
                actionButton.GetComponent<Image>().color = playerAction.ActionColor;
                actionButton.GetComponentInChildren<TMP_Text>().text = playerAction.ActionName;
                actionButton.GetComponent<Tooltip>().Message = playerAction.ActionDescription;

                // Update button click to use action
                actionButton.GetComponent<Button>().onClick.RemoveAllListeners();
                actionButton.GetComponent<Button>().onClick.AddListener(() => OnActionButtonClick(playerAction));
            }
        }
    }

    public void UpdateUpNextPanel()
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
            Player player = turnOrder[i % turnOrder.Count];
            GameObject icon = Instantiate(TurnIconPrefab, UpNextPanel);

            // Center camera on icon player
            Button iconButton = icon.GetComponent<Button>();
            iconButton.onClick.AddListener(() => CameraController.Instance.CenterObject(player.gameObject));

            // Show player stats on icon hover
            EventTrigger iconEventTrigger = icon.GetComponent<EventTrigger>();
            EventTrigger.Entry iconPointerEnter = new(){ eventID = EventTriggerType.PointerEnter };
            iconPointerEnter.callback.AddListener((data) =>
            {
                InspectorCoroutine = UpdatePlayerInspector(player);
                StartCoroutine(InspectorCoroutine);
            });
            iconEventTrigger.triggers.Add(iconPointerEnter);
            EventTrigger.Entry iconPointerExit = new(){ eventID = EventTriggerType.PointerExit };
            iconPointerExit.callback.AddListener((data) =>
            {
                StopCoroutine(InspectorCoroutine);
                HidePlayerInspector();
            });
            iconEventTrigger.triggers.Add(iconPointerExit);

            // Example coloring for different players
            icon.TryGetComponent(out Image iconImage);
            if (player.GetType() == typeof(Enemy)) {
                icon.name = "Example Enemy";
            }
            else
            {
                int playerNum = int.Parse(player.name[7..]);
                icon.name = $"Player {playerNum} icon ({i})";
            }
            iconImage.color = player.IconColor;

            turnIcons.Add(icon);
        }
    }

    void HandleEndTurn(Player nextTurnPlayer)
    {
        uiState = UiState.Idle;
        TileGridManager.Instance.UnhighlightAllTiles();

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
                Mathf.RoundToInt(currentPlayer.transform.position.x),
                Mathf.RoundToInt(currentPlayer.transform.position.z),
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

    public IEnumerator UpdatePlayerInspector(Player otherPlayer)
    {
        yield return new WaitForSecondsRealtime(InspectorTimerSec);

        if (currentPlayer == otherPlayer)
        {
            otherPlayerStats.SetActive(false);
            yield break;
        }

        otherPlayerImage.color = otherPlayer.IconColor;
        otherPlayerName.text = otherPlayer.name;
        otherPlayerAc.text = otherPlayer.PlayerAc.ToString("D2");
        otherPlayerHp_max.text = otherPlayer.PlayerHp_max.ToString("D2");
        otherPlayerHp_curr.text = otherPlayer.PlayerHp_curr.ToString("D2");
        otherPlayerSpeed.text = otherPlayer.PlayerSpeed.ToString("D2");
        otherPlayerStats.SetActive(true);
    }

    public void HidePlayerInspector()
    {
        otherPlayerStats.SetActive(false);
    }

    public void OnActionButtonClick(BaseAction playerAction)
    {
        ActionTargetingManager.Instance.StartTargetSelection(
            playerAction,
            currentPlayer
        );
    }
}
