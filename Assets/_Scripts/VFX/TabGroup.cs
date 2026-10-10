using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] TabButton[] tabs;
    [SerializeField] CanvasGroup[] pages;
    [SerializeField] float fadeSpeed = 10f;

    int current;

    void Awake()
    {
        for (int i = 0; i < tabs.Length; i++) tabs[i].Init(this, i);
        Select(0);
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
        current = (i + tabs.Length) % tabs.Length;
        for (int k = 0; k < tabs.Length; k++)
            tabs[k].SetSelected(k == current);
    }
}