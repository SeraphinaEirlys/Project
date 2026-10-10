using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryMenu : MonoBehaviour
{
    [SerializeField] TabGroup tabGroup;
    [SerializeField] float fadeInSpeed = 8f;

    CanvasGroup canvasGroup;

    public bool IsOpen => gameObject.activeSelf;

    void Awake() => canvasGroup = GetComponent<CanvasGroup>();

    public void Open()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
    }

    public void Close() => gameObject.SetActive(false);

    void Update()
    {
        // fade in mượt, dùng unscaledDeltaTime vì lúc này timeScale = 0
        canvasGroup.alpha = Mathf.MoveTowards(
            canvasGroup.alpha, 1f, fadeInSpeed * Time.unscaledDeltaTime);

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.qKey.wasPressedThisFrame) tabGroup.Prev();
        if (kb.eKey.wasPressedThisFrame) tabGroup.Next();
    }
}