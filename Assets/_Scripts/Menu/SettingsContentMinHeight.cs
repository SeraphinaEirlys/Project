using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(LayoutElement))]
public class SettingsContentMinHeight : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;

    private LayoutElement layoutElement;

    private void OnEnable()
    {
        layoutElement = GetComponent<LayoutElement>();
        UpdateMinimumHeight();
    }

    private void LateUpdate()
    {
        UpdateMinimumHeight();
    }

    private void UpdateMinimumHeight()
    {
        if (viewport == null)
            return;

        if (layoutElement == null)
            layoutElement = GetComponent<LayoutElement>();

        float height = Mathf.Max(0f, viewport.rect.height);

        if (!Mathf.Approximately(layoutElement.minHeight, height))
            layoutElement.minHeight = height;
    }
}