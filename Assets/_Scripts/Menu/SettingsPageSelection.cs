using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsPageSelection : MonoBehaviour
{
    [SerializeField] private Selectable defaultSelection;

    private CanvasGroup pageCanvasGroup;
    private bool selectionPending;

    private void Awake()
    {
        pageCanvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        RequestSelection();
    }

    private void OnDisable()
    {
        selectionPending = false;
    }

    public void RequestSelection()
    {
        selectionPending = true;
    }

    private void LateUpdate()
    {
        if (!selectionPending || EventSystem.current == null)
            return;

        if (pageCanvasGroup != null &&
            !pageCanvasGroup.interactable)
            return;

        if (defaultSelection == null ||
            !defaultSelection.isActiveAndEnabled ||
            !defaultSelection.IsInteractable())
            return;

        selectionPending = false;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(
            defaultSelection.gameObject);
    }
}