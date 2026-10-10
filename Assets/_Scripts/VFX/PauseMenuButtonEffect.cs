using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class PauseMenuButtonEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler,
    ISelectHandler,
    IDeselectHandler,
    ISubmitHandler
{
    [Header("Audio")]
    public string hoverSoundName = "UI_Hover";

    [Header("UI Elements")]
    public GameObject hoverBackground;
    public TextMeshProUGUI buttonText;
    public CanvasGroup selectionCanvasGroup;
    public RectTransform arrowLeft;
    public RectTransform arrowRight;

    [Header("Colors")]
    public Color normalTextColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color hoverTextColor = Color.white;

    [Header("Glow")]
    [Range(0f, 1f)] public float hoverAlpha = 0.65f;
    [Range(0f, 1f)] public float clickFlashAlpha = 1f;
    [Min(0.01f)] public float clickFlashDuration = 0.16f;

    [Header("Arrow Motion")]
    [Min(0f)] public float arrowMoveDistance = 4f;
    [Min(0.1f)] public float arrowMoveSpeed = 2.5f;

    private Button button;
    private bool isHighlighted;
    private Vector2 leftStartPosition;
    private Vector2 rightStartPosition;
    private Coroutine flashRoutine;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (selectionCanvasGroup == null && hoverBackground != null)
            selectionCanvasGroup = hoverBackground.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (arrowLeft != null)
            leftStartPosition = arrowLeft.anchoredPosition;

        if (arrowRight != null)
            rightStartPosition = arrowRight.anchoredPosition;

        bool alreadySelected =
            EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == gameObject;

        ApplyEffect(alreadySelected, false);
    }

    private void OnDisable()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        ApplyEffect(false, false);
        ResetArrowPositions();
    }

    private void Update()
    {
        if (!isHighlighted)
        {
            ResetArrowPositions();
            return;
        }

        float pulse = (Mathf.Sin(Time.unscaledTime * arrowMoveSpeed) + 1f) * 0.5f;
        float offset = pulse * arrowMoveDistance;

        if (arrowLeft != null)
            arrowLeft.anchoredPosition =
                leftStartPosition + Vector2.left * offset;

        if (arrowRight != null)
            arrowRight.anchoredPosition =
                rightStartPosition + Vector2.right * offset;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button == null || !button.IsInteractable())
            return;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);

        ApplyEffect(true, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyEffect(false, false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        ApplyEffect(true, true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ApplyEffect(false, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayClickFlash();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        PlayClickFlash();
    }

    private void PlayClickFlash()
    {
        if (button == null || !button.IsInteractable() ||
            selectionCanvasGroup == null)
            return;

        if (hoverBackground != null)
            hoverBackground.SetActive(true);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(ClickFlashRoutine());
    }

    private IEnumerator ClickFlashRoutine()
    {
        selectionCanvasGroup.alpha = clickFlashAlpha;

        yield return new WaitForSecondsRealtime(clickFlashDuration);

        selectionCanvasGroup.alpha = isHighlighted ? hoverAlpha : 0f;

        if (!isHighlighted && hoverBackground != null)
            hoverBackground.SetActive(false);

        flashRoutine = null;
    }

    private void ApplyEffect(bool visible, bool playSound)
    {
        if (button != null && !button.IsInteractable())
            visible = false;

        bool justHighlighted = visible && !isHighlighted;
        isHighlighted = visible;

        if (hoverBackground != null)
            hoverBackground.SetActive(visible);

        if (selectionCanvasGroup != null)
            selectionCanvasGroup.alpha = visible ? hoverAlpha : 0f;

        if (buttonText != null)
            buttonText.color = visible ? hoverTextColor : normalTextColor;

        if (justHighlighted &&
            playSound &&
            SoundManager.Instance != null &&
            !string.IsNullOrEmpty(hoverSoundName))
        {
            SoundManager.Instance.PlaySound2D(hoverSoundName);
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