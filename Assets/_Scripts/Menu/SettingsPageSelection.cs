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

    [Header("Navigation layout")]
    [SerializeField, Min(1)] private int navigationColumns = 1;

    [Header("Shared actions")]
    [SerializeField] private Selectable revertButton;
    [SerializeField] private Selectable backButton;

    private CanvasGroup pageCanvasGroup;

    [Header("Optional secondary binding")]
    [SerializeField] private Selectable primaryBindingButton;
    [SerializeField] private Selectable secondaryBindingButton;

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

        int settingCount = controls.Count;
        int columns = Mathf.Max(1, navigationColumns);

        AddControl(controls, revertButton);
        AddControl(controls, backButton);

        for (int i = 0; i < controls.Count; i++)
        {
            Selectable current = controls[i];

            Navigation navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = current,
                selectOnDown = current,
                selectOnLeft = null,
                selectOnRight = null
            };

            if (i < settingCount)
            {
                int column = i % columns;

                // Move vertically within the same column.
                if (i >= columns)
                    navigation.selectOnUp = controls[i - columns];

                if (i + columns < settingCount)
                {
                    navigation.selectOnDown = controls[i + columns];
                }
                else if (settingCount < controls.Count)
                {
                    // Last row goes to the first shared action.
                    navigation.selectOnDown = controls[settingCount];
                }

                // Move horizontally without crossing into another row.
                if (column > 0)
                    navigation.selectOnLeft = controls[i - 1];

                if (column < columns - 1 && i + 1 < settingCount)
                    navigation.selectOnRight = controls[i + 1];
            }
            else
            {
                // Revert and Back remain a vertical list.
                if (i > settingCount)
                {
                    navigation.selectOnUp = controls[i - 1];
                }
                else if (settingCount > 0)
                {
                    // First shared action returns to the last row's left cell.
                    int lastRowStart =
                        ((settingCount - 1) / columns) * columns;

                    navigation.selectOnUp = controls[lastRowStart];
                }

                if (i + 1 < controls.Count)
                    navigation.selectOnDown = controls[i + 1];
            }

            current.navigation = navigation;
        }
        ConnectSecondaryBinding(controls);

        return controls;
    }

    private void ConnectSecondaryBinding(List<Selectable> controls)
    {
        if (primaryBindingButton == null ||
            secondaryBindingButton == null ||
            !controls.Contains(primaryBindingButton) ||
            !secondaryBindingButton.isActiveAndEnabled ||
            !secondaryBindingButton.IsInteractable())
            return;

        Navigation primaryNav = primaryBindingButton.navigation;
        Selectable rightNeighbor = primaryNav.selectOnRight;

        // The secondary button shares the primary button's
        // vertical neighbors.
        Navigation secondaryNav = primaryNav;
        secondaryNav.mode = Navigation.Mode.Explicit;
        secondaryNav.selectOnLeft = primaryBindingButton;
        secondaryNav.selectOnRight = rightNeighbor;
        secondaryBindingButton.navigation = secondaryNav;

        // Primary -> Secondary -> right-hand setting.
        primaryNav.selectOnRight = secondaryBindingButton;
        primaryBindingButton.navigation = primaryNav;

        if (rightNeighbor != null)
        {
            Navigation neighborNav = rightNeighbor.navigation;
            neighborNav.selectOnLeft = secondaryBindingButton;
            rightNeighbor.navigation = neighborNav;
        }
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