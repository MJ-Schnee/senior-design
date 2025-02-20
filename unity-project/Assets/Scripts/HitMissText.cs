using UnityEngine;
using TMPro;

/// <summary>
/// Floating text which rises above a player. Intended to read HIT/MISS.
/// </summary>
public class HitMissText : MonoBehaviour
{
    public Transform Target;

    public Vector3 BaseOffset = new(0, 2f, 0);

    public float MoveUpSpeed = 1.5f;
    
    public float Lifetime = 0.75f;

    public float ScaleMultiplier = 7f;
    
    private float timer = 0;
    
    private TextMeshProUGUI textUi;

    void Awake()
    {
        textUi = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Target == null)
        {
            Destroy(gameObject);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= Lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Convert to screen space
        float extraUp = MoveUpSpeed * timer;
        Vector3 worldPos = Target.position + BaseOffset + (Vector3.up * extraUp);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = screenPos;

        // Scale inversely with camera zoom
        float orthoSize = Camera.main.orthographicSize; 
        float scaleFactor = ScaleMultiplier / orthoSize;
        transform.localScale = Vector3.one * scaleFactor;

        // Fade over time
        textUi.alpha -= Time.deltaTime / Lifetime * 0.025f;
    }

    public void SetText(string message, Color color)
    {
        if (textUi != null)
        {
            textUi.text = message;
            textUi.color = color;
            textUi.alpha = 0.02f;
        }
    }
}
