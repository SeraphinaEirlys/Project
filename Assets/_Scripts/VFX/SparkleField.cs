using UnityEngine;
using UnityEngine.UI;

public class SparkleField : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] Sprite starSprite;
    [SerializeField] Sprite glowSprite;

    [Header("Số lượng và kích thước")]
    [SerializeField] int count = 9;
    [SerializeField] Vector2 sizeRange = new Vector2(40f, 120f);
    [SerializeField] Vector2 excludeCenterSize = new Vector2(800f, 150f);

    [Header("Màu")]
    [SerializeField] Color starColor = Color.white;
    [SerializeField] Color glowColor = new Color(0.62f, 0.78f, 1f, 1f);
    [SerializeField] float glowScale = 2.2f;
    [SerializeField, Range(0f, 1f)] float glowAlpha = 0.5f;

    [Header("Nhịp chớp")]
    [SerializeField] Vector2 speedRange = new Vector2(0.6f, 1.4f);
    [SerializeField, Range(0.5f, 8f)] float sharpness = 2.5f;
    [SerializeField, Range(0f, 1f)] float minScale = 0.35f;
    [SerializeField, Range(0f, 1f)] float minAlpha = 0.1f;

    class Star
    {
        public RectTransform root;
        public Image core, glow;
        public float speed, progress;
    }

    Star[] stars;
    RectTransform area;

    void Start()
    {
        area = (RectTransform)transform;
        stars = new Star[count];

        for (int i = 0; i < count; i++)
        {
            var rootGo = new GameObject("Star", typeof(RectTransform));
            rootGo.transform.SetParent(transform, false);
            var root = (RectTransform)rootGo.transform;

            var star = new Star { root = root };
            
            if (glowSprite != null)
                star.glow = MakeImage(root, "Glow", glowSprite, 100f);

            star.core = MakeImage(root, "Core", starSprite, 100f);

            ResetStar(star, true);
            stars[i] = star;
        }
    }

    void ResetStar(Star s, bool isStart)
    {
        float size = Random.Range(sizeRange.x, sizeRange.y);
        s.root.sizeDelta = new Vector2(size, size);
        
        if (s.glow != null) s.glow.rectTransform.sizeDelta = new Vector2(size * glowScale, size * glowScale);
        s.core.rectTransform.sizeDelta = new Vector2(size, size);

        Vector2 pos = Vector2.zero;
        for (int attempt = 0; attempt < 20; attempt++)
        {
            pos = new Vector2(
                Random.Range(-area.rect.width * 0.5f, area.rect.width * 0.5f),
                Random.Range(-area.rect.height * 0.5f, area.rect.height * 0.5f));

            if (Mathf.Abs(pos.x) > excludeCenterSize.x * 0.5f || Mathf.Abs(pos.y) > excludeCenterSize.y * 0.5f)
            {
                break;
            }
        }
        s.root.anchoredPosition = pos;

        s.speed = Random.Range(speedRange.x, speedRange.y);
        s.progress = isStart ? Random.Range(0f, 1f) : 0f; 
    }

    static Image MakeImage(RectTransform parent, string name, Sprite sprite, float size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.sizeDelta = new Vector2(size, size);

        var img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.raycastTarget = false;
        img.color = new Color(1f, 1f, 1f, 0f);
        return img;
    }

    void Update()
    {
        if (stars == null) return;

        foreach (var s in stars)
        {
            s.progress += Time.unscaledDeltaTime * s.speed;
            if (s.progress >= 1f)
            {
                ResetStar(s, false);
            }

            float w = Mathf.Sin(s.progress * Mathf.PI);
            float v = Mathf.Lerp(minAlpha, 1f, Mathf.Pow(w, sharpness));

            s.core.color = new Color(starColor.r, starColor.g, starColor.b, v * starColor.a);
            if (s.glow != null)
                s.glow.color = new Color(glowColor.r, glowColor.g, glowColor.b, v * glowAlpha);

            s.root.localScale = Vector3.one * Mathf.Lerp(minScale, 1f, v);
        }
    }
}