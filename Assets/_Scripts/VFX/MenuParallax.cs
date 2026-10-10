using UnityEngine;
using UnityEngine.InputSystem;

public class MenuParallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    [SerializeField] private float offsetMultiplier = 1f;
    [SerializeField] private float smoothTime = 0.3f;

    private Vector3 startPosition;
    private Vector3 velocity;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (Camera.main == null) return;
        if (Mouse.current == null) return;

        Vector2 screenMousePos = Mouse.current.position.ReadValue();

        Vector2 mousePos = Camera.main.ScreenToViewportPoint(screenMousePos);

        float offsetX = (mousePos.x - 0.5f) * offsetMultiplier;
        float offsetY = (mousePos.y - 0.5f) * offsetMultiplier;

        Vector3 targetPosition = startPosition + new Vector3(offsetX, offsetY, 0f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}