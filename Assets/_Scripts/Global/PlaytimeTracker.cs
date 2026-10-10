using UnityEngine;

public class PlaytimeTracker : MonoBehaviour
{
    public static PlaytimeTracker Instance { get; private set; }
    public float totalPlaytimeInSeconds;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        totalPlaytimeInSeconds += Time.deltaTime;
    }

    public string GetFormattedPlaytime()
    {
        int hours = Mathf.FloorToInt(totalPlaytimeInSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalPlaytimeInSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalPlaytimeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}