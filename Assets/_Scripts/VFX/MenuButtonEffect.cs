using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("UI Elements")]
    public GameObject hoverBackground;
    public GameObject pointerIcon;
    public TextMeshProUGUI buttonText;
    [Header("Colors")]
    public Color normalTextColor = new Color(0.2f, 0.2f, 0.2f);
    public Color hoverTextColor = Color.white;

    void Start()
    {
        HideEffect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideEffect();
    }

    public void OnSelect(BaseEventData eventData)
    {
        ShowEffect();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        HideEffect();
    }

    private void ShowEffect()
    {
        if (hoverBackground) hoverBackground.SetActive(true);
        if (pointerIcon) pointerIcon.SetActive(true);
        if (buttonText) buttonText.color = hoverTextColor;
    }

    private void HideEffect()
    {
        if (hoverBackground) hoverBackground.SetActive(false);
        if (pointerIcon) pointerIcon.SetActive(false);
        if (buttonText) buttonText.color = normalTextColor;
    }
}