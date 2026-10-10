using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextGlowPulse : MonoBehaviour
{
    [Header("Màu glow")]
    [SerializeField] Color glowColor = new Color(0.5f, 0.89f, 1f, 1f);

    [Header("Glow sát chữ (TMP)")]
    [SerializeField, Range(0f, 1f)] float textMinAlpha = 0.5f;
    [SerializeField, Range(0f, 1f)] float textMaxAlpha = 1f;
    [SerializeField, Range(0f, 1f)] float textMinOuter = 0.6f;
    [SerializeField, Range(0f, 1f)] float textMaxOuter = 1f;
    [SerializeField, Range(0f, 1f)] float textInner = 0.1f;
    [SerializeField, Range(0.1f, 2f)] float textPower = 0.6f;   // thấp = tỏa mềm và rộng hơn

    [Header("Vầng sáng lớn phía sau (kéo TitleHalo vào)")]
    [SerializeField] Image halo;
    [SerializeField, Range(0f, 1f)] float haloMinAlpha = 0.25f;
    [SerializeField, Range(0f, 1f)] float haloMaxAlpha = 0.6f;
    [SerializeField, Range(0f, 0.3f)] float haloBreathScale = 0.05f;

    [Header("Nhịp")]
    [SerializeField] float speed = 1.2f;
    [SerializeField, Range(0f, 1f)] float irregularity = 0.2f;

    TMP_Text text;
    Material mat;
    float phase;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        mat = text.fontMaterial;                    // bản sao riêng của chữ này
        mat.EnableKeyword(ShaderUtilities.Keyword_Glow);
        mat.SetFloat(ShaderUtilities.ID_GlowOffset, 0f);
        mat.SetFloat(ShaderUtilities.ID_GlowInner, textInner);
        mat.SetFloat(ShaderUtilities.ID_GlowPower, textPower);
        text.UpdateMeshPadding();
        phase = Random.value * 10f;
    }

    void Update()
    {
        float t = Time.unscaledTime * speed + phase;
        float slow = 0.5f + 0.5f * Mathf.Sin(t);
        float fast = 0.5f + 0.5f * Mathf.Sin(t * 1.9f + 1.1f);
        float w = Mathf.Lerp(slow, fast, irregularity);

        // glow sát chữ
        Color c = glowColor;
        c.a = Mathf.Lerp(textMinAlpha, textMaxAlpha, w);
        mat.SetColor(ShaderUtilities.ID_GlowColor, c);
        mat.SetFloat(ShaderUtilities.ID_GlowOuter, Mathf.Lerp(textMinOuter, textMaxOuter, w));

        // vầng sáng lớn
        if (halo != null)
        {
            Color h = glowColor;
            h.a = Mathf.Lerp(haloMinAlpha, haloMaxAlpha, w);
            halo.color = h;
            halo.rectTransform.localScale = Vector3.one * (1f + haloBreathScale * (w - 0.5f));
        }
    }

    void OnDestroy()
    {
        if (mat != null) Destroy(mat);
    }
}