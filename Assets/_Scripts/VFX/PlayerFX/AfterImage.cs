using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color color;
    
    [Header("Settings")]
    public float activeTime = 0.3f;
    private float alpha;
    private float alphaMultiplier = 0.85f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        alpha = alphaMultiplier;
        color = sr.color; 
    }

    void Update()
    {
        alpha -= Time.deltaTime / activeTime;
        color.a = alpha;
        sr.color = color;

        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}