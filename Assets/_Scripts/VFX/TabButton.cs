using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TabButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] TMP_Text label;
    [SerializeField] CanvasGroup glow;

    [Header("Text color")]
    [SerializeField] Color normalColor   = new Color(0.91f, 0.89f, 0.86f, 0.45f);
    [SerializeField] Color hoverColor    = new Color(0.91f, 0.89f, 0.86f, 0.75f);
    [SerializeField] Color selectedColor = Color.white;

    [Header("Scale")]
    [SerializeField] float hoverScale    = 1.03f;
    [SerializeField] float selectedScale = 1.1f;

    [Header("Hover")]
    [SerializeField, Range(0f, 1f)] float hoverGlow = 0f;

    [Header("Nhấp nháy của tab đang chọn")]
    [SerializeField, Range(0f, 1f)] float glowMin = 0.3f;
    [SerializeField, Range(0f, 1f)] float glowMax = 1f;
    [SerializeField] float breathSpeed = 4f;
    [SerializeField, Range(0f, 1f)] float irregularity = 0.3f;
    [SerializeField, Range(0f, 0.3f)] float breathScale = 0.1f;

    [SerializeField] float speed = 12f;

    TabGroup group;
    RectTransform glowRect;
    int index;
    bool hovered, selected;
    float hoverT, selectT;
    float phase;

    public void Init(TabGroup g, int i) { group = g; index = i; }
    public void SetSelected(bool v) => selected = v;

    public void OnPointerEnter(PointerEventData e) => hovered = true;
    public void OnPointerExit(PointerEventData e)  => hovered = false;
    public void OnPointerClick(PointerEventData e) => group.Select(index);

    void Awake()
    {
        if (glow) glowRect = (RectTransform)glow.transform;
        phase = Random.value * 10f;
    }

    void OnDisable() => hovered = false;

    void Update()
    {
        float dt = speed * Time.unscaledDeltaTime;
        hoverT  = Mathf.MoveTowards(hoverT,  hovered  ? 1f : 0f, dt);
        selectT = Mathf.MoveTowards(selectT, selected ? 1f : 0f, dt);

        Color c = Color.Lerp(normalColor, hoverColor, hoverT);
        label.color = Color.Lerp(c, selectedColor, selectT);

        float s = Mathf.Lerp(1f, hoverScale, hoverT);
        transform.localScale = Vector3.one * Mathf.Lerp(s, selectedScale, selectT);

        float t = Time.unscaledTime * breathSpeed + phase;
        float slow = 0.5f + 0.5f * Mathf.Sin(t);
        float fast = 0.5f + 0.5f * Mathf.Sin(t * 2.7f + 1.3f);
        float wave = Mathf.Lerp(slow, fast, irregularity);

        float selectedAlpha = Mathf.Lerp(glowMin, glowMax, wave) * selectT;
        float hoverAlpha    = hoverT * hoverGlow;

        if (glow) glow.alpha = Mathf.Clamp01(Mathf.Max(selectedAlpha, hoverAlpha));

        if (glowRect)
            glowRect.localScale = Vector3.one * (1f + breathScale * (wave - 0.5f) * selectT);
    }
}