using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SettingsSliderEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Control")]
    [SerializeField] private Slider targetSlider;

    [Header("Visuals")]
    [SerializeField] private CanvasGroup selectionGroup;
    [SerializeField] private TMP_Text labelText;

    [Header("Appearance")]
    [SerializeField, Range(0f, 1f)]
    private float hoverAlpha = 0.3f;

    [Header("Arrow Motion")]
    [SerializeField] private RectTransform arrowLeft;
    [SerializeField] private RectTransform arrowRight;

    [SerializeField, Min(0f)]
    private float arrowMoveDistance = 4f;

    [SerializeField, Min(0.1f)]
    private float arrowMoveSpeed = 2.5f;

    private Vector2 leftStartPosition;
    private Vector2 rightStartPosition;

    [SerializeField]
    private Color normalTextColor =
    new Color(0.8f, 0.8f, 0.8f, 1f);

    [SerializeField]
    private Color selectedTextColor = Color.white;

    private bool pointerInside;
    private bool highlighted;

    private void OnEnable()
    {
        if (arrowLeft != null)
            leftStartPosition = arrowLeft.anchoredPosition;

        if (arrowRight != null)
            rightStartPosition = arrowRight.anchoredPosition;

        pointerInside = false;
        Refresh(true);
    }

    private void OnDisable()
    {
        pointerInside = false;
        ApplyVisual(false);
        ResetArrowPositions();
    }

    private void Update()
    {
        Refresh(false);

        if (!highlighted)
        {
            ResetArrowPositions();
            return;
        }

        float pulse =
            (Mathf.Sin(Time.unscaledTime * arrowMoveSpeed) + 1f) * 0.5f;

        float offset = pulse * arrowMoveDistance;

        if (arrowLeft != null)
        {
            arrowLeft.anchoredPosition =
                leftStartPosition + Vector2.left * offset;
        }

        if (arrowRight != null)
        {
            arrowRight.anchoredPosition =
                rightStartPosition + Vector2.right * offset;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetSlider == null ||
            !targetSlider.IsInteractable())
            return;

        pointerInside = true;

        // Select the Slider, not the layout row.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(
                targetSlider.gameObject);
        }

        Refresh(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
        Refresh(true);
    }

    private void Refresh(bool force)
    {
        bool selected =
            targetSlider != null &&
            EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject ==
            targetSlider.gameObject;

        bool visible =
            targetSlider != null &&
            targetSlider.isActiveAndEnabled &&
            targetSlider.IsInteractable() &&
            (pointerInside || selected);

        if (force || visible != highlighted)
            ApplyVisual(visible);
    }

    private void ApplyVisual(bool visible)
    {
        highlighted = visible;

        if (selectionGroup != null)
        {
            selectionGroup.alpha = visible ? hoverAlpha : 0f;
            selectionGroup.gameObject.SetActive(visible);
        }

        if (labelText != null)
        {
            labelText.color =
                visible ? selectedTextColor : normalTextColor;
        }
    }

    private void ResetArrowPositions()
    {
        if (arrowLeft != null)
            arrowLeft.anchoredPosition = leftStartPosition;

        if (arrowRight != null)
            arrowRight.anchoredPosition = rightStartPosition;
    }
}