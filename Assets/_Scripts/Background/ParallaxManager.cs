using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        [Tooltip("Transform của Layer cha")]
        public Transform layer;

        [Range(0f, 1f)]
        public float parallaxFactor;

        [HideInInspector] public float startPosX;
        [HideInInspector] public float lengthX;
    }

    public ParallaxLayer[] layers;
    public Transform camTransform;

    void Start()
    {
        if (camTransform == null)
            camTransform = Camera.main.transform;

        foreach (var layer in layers)
        {
            if (layer.layer == null) continue;

            layer.startPosX = layer.layer.position.x;

            SpriteRenderer sr = layer.layer.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                layer.lengthX = sr.bounds.size.x;
            }
            else
            {
                Debug.LogWarning($"Layer {layer.layer.name} thiếu SpriteRenderer trên GameObject cha!");
            }
        }
    }

    void FixedUpdate()
    {
        if (camTransform == null) return;

        float camX = camTransform.position.x;

        foreach (var layer in layers)
        {
            if (layer.layer == null) continue;

            float distance = camX * layer.parallaxFactor;

            float movement = camX * (1f - layer.parallaxFactor);

            Vector3 currentPos = layer.layer.position;
            layer.layer.position = new Vector3(layer.startPosX + distance, currentPos.y, currentPos.z);

            if (movement > layer.startPosX + layer.lengthX)
            {
                layer.startPosX += layer.lengthX;
            }
            else if (movement < layer.startPosX - layer.lengthX)
            {
                layer.startPosX -= layer.lengthX;
            }
        }
    }
}