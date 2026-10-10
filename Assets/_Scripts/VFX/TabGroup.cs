using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] TabButton[] tabs;
    [SerializeField] CanvasGroup[] pages;
    [SerializeField] float fadeSpeed = 10f;

    int current;
    public int CurrentIndex => current;

    void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
            tabs[i].Init(this, i);

        current = 0;

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].alpha = 0f;
            pages[i].interactable = i == current;
            pages[i].blocksRaycasts = i == current;
        }

        Select(current);
    }

    void Update()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            bool on = i == current;
            pages[i].alpha = Mathf.MoveTowards(
                pages[i].alpha, on ? 1f : 0f, fadeSpeed * Time.unscaledDeltaTime);
            pages[i].interactable = on;
            pages[i].blocksRaycasts = on;
        }
    }

    public void Next() => Select(current + 1);
    public void Prev() => Select(current - 1);

    public void Select(int i)
    {
        if (tabs.Length == 0)
            return;

        current = (i + tabs.Length) % tabs.Length;

        for (int k = 0; k < tabs.Length; k++)
            tabs[k].SetSelected(k == current);

        if (current < pages.Length && pages[current] != null)
        {
            SettingsPageSelection selection =
                pages[current].GetComponent<SettingsPageSelection>();

            if (selection != null)
                selection.RequestSelection();
        }
    }
}