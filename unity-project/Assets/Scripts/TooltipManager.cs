using UnityEngine;
using TMPro;
using System;

public class TooltipManager : MonoBehaviour
{

    public static TooltipManager Instance;

    public TextMeshProUGUI TextComponent;

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
    }

    void Start()
    {
        gameObject.SetActive(false);
        Cursor.visible = true;
    }

    void Update()
    {
        Vector3 newPos = Input.mousePosition;

        // Ensure tooltip doesn't go off screen
        gameObject.TryGetComponent(out RectTransform bgTransform);
        newPos.x = Math.Min(newPos.x, 1920 - bgTransform.rect.width);
        newPos.y = Math.Min(newPos.y, 1080 - bgTransform.rect.height);

        transform.position = newPos;
    }

    public void SetAndShowTooltip(string message)
    {
        gameObject.SetActive(true);
        TextComponent.text = message;

        // Adjust size of tooltip window
        float textPaddingSize = 4f;
        Vector2 backgroundSize = new(TextComponent.preferredWidth + textPaddingSize * 2f, TextComponent.preferredHeight + textPaddingSize * 2f);
        gameObject.TryGetComponent(out RectTransform bgTransform);
        bgTransform.sizeDelta = backgroundSize;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        TextComponent.text = string.Empty;
        
        // Move tooltip off screen
        Vector3 newPos = new(1920, 1080);
        transform.position = newPos;
    }
}
