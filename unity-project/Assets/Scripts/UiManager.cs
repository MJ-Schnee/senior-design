using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    public GameObject TurnIconPrefab;

    public Transform UpNextPanel;

    [SerializeField] Image playerImage;

    [SerializeField] TMP_Text playerName, playerAc, playerHp_max, playerHp_curr;

    List<GameObject> turnIcons;

    void Awake()
    {
        turnIcons = new();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnEndTurn += HandleEndTurn;
        
        UpdatePlayerPanel(GameManager.Instance.TurnOrder.GetCurrentTurn());
        UpdateUpNextPanel();
    }

    void UpdatePlayerPanel(Player player)
    {
        
        int playerNum = int.Parse(player.name[7..]);
        float playerColorPoint = playerNum * 60 / 255.0f;
        playerImage.color = new Color(playerColorPoint, playerColorPoint, playerColorPoint);
        playerName.text = $"Player {playerNum}";
        playerAc.text = "12";
        playerHp_max.text = "20";
        playerHp_curr.text = "20";
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
            float playerColorPoint = playerNum * 60 / 255.0f;
            iconImage.color = new Color(playerColorPoint, playerColorPoint, playerColorPoint);

            turnIcons.Add(icon);
        }
    }

    void HandleEndTurn(Player nextTurnPlayer)
    {
        UpdatePlayerPanel(nextTurnPlayer);
        UpdateUpNextPanel();
    }
}
