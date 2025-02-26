using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [TextArea]
    public string Message;

    private float tooltipTimerSec = 2.0f;

    private IEnumerator timerCoroutine;

    void Awake()
    {
        timerCoroutine = ShowTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(timerCoroutine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HideTooltip();
    }

    IEnumerator ShowTooltip()
    {
        yield return new WaitForSecondsRealtime(tooltipTimerSec);
        TooltipManager.Instance.SetAndShowTooltip(Message);
    }

    void HideTooltip()
    {
        TooltipManager.Instance.HideTooltip();

        StopCoroutine(timerCoroutine);
        timerCoroutine = ShowTooltip();
    }
}
