using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsPageSelection : MonoBehaviour
{
    [Header("Default selection")]
    [SerializeField] private Selectable defaultSelection;

    [Header("Rows of THIS page, from top to bottom")]
    [SerializeField] private Selectable[] pageRows;

    [Header("Shared actions")]
    [SerializeField] private Selectable revertButton;
    [SerializeField] private Selectable backButton;

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

        // Wait until this page allows interaction.
        if (pageCanvasGroup != null &&
            !pageCanvasGroup.interactable)
            return;

        List<Selectable> controls = BuildNavigation();

        if (controls.Count == 0)
            return;

        Selectable first = controls[0];

        if (defaultSelection != null &&
            controls.Contains(defaultSelection))
        {
            first = defaultSelection;
        }

        selectionPending = false;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(first.gameObject);
    }

    private List<Selectable> BuildNavigation()
    {
        List<Selectable> controls = new List<Selectable>();

        if (pageRows != null)
        {
            foreach (Selectable row in pageRows)
                AddControl(controls, row);
        }

        AddControl(controls, revertButton);
        AddControl(controls, backButton);

        for (int i = 0; i < controls.Count; i++)
        {
            Navigation navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,

                // Stay on the first/last item at the boundary.
                selectOnUp = controls[Mathf.Max(0, i - 1)],
                selectOnDown = controls[
                    Mathf.Min(controls.Count - 1, i + 1)],

                // Do not navigate sideways into other menus.
                selectOnLeft = null,
                selectOnRight = null
            };

            controls[i].navigation = navigation;
        }

        return controls;
    }

    private void AddControl(
        List<Selectable> controls,
        Selectable control)
    {
        if (control == null ||
            !control.isActiveAndEnabled ||
            !control.IsInteractable() ||
            controls.Contains(control))
            return;

        controls.Add(control);
    }
}