using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer;
        [Range(0f, 1f)]
        public float parallaxFactor;
        [HideInInspector] public float startPosX;
        [HideInInspector] public float lengthX;
    }

    public ParallaxLayer[] layers;
    private Transform camTransform;
    
    private void Start()
    {
        if (Camera.main != null) camTransform = Camera.main.transform;

        Collider2D confiner = GetComponent<Collider2D>();
        float roomCenterX = (confiner != null) ? confiner.bounds.center.x : transform.position.x;

        foreach (var layer in layers)
        {
            if (layer.layer == null) continue;

            layer.startPosX = layer.layer.position.x - (roomCenterX * layer.parallaxFactor);

            SpriteRenderer sr = layer.layer.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                layer.lengthX = sr.bounds.size.x;
            }
        }
    }

    void LateUpdate()
    {
        if (camTransform == null)
        {
            if (Camera.main != null) camTransform = Camera.main.transform;
            return;
        }

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