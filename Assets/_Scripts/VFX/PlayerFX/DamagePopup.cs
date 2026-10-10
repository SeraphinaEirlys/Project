using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TMP_Text textMesh;
    public float speed = 2f;
    public float lifetime = 1f;

    private float timer;
    private Color textColor;

    public void Setup(int damageAmount)
    {
        textMesh.text = Mathf.Abs(damageAmount).ToString();
        textColor = textMesh.color;
    }

    private void Update()
    {
        transform.position += Vector3.up * (speed * Time.deltaTime);
        timer += Time.deltaTime;

        float alpha = 1f - (timer / lifetime);
        textMesh.color = new Color(textColor.r, textColor.g, textColor.b, alpha);

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}