using UnityEngine;

public class PlayerPersistence : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Health health;

    private void Start()
    {
        SaveManager saveManager = ServiceLocator.Get<SaveManager>();

        if (saveManager == null)
        {
            Debug.LogError("SaveManager was not found in the ServiceLocator.");
            return;
        }

        saveManager.Register(this);
    }

    public void SaveData(SaveData data)
    {
        data.playerPosition = transform.position;
        data.playerHealth = health.health;
    }

    public void LoadData(SaveData data)
    {
        transform.position = data.playerPosition;
        health.SetHealth(data.playerHealth);
    }
}