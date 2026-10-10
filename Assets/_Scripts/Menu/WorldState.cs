using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static WorldState Instance { get; private set; }

    public HashSet<string> collectedLoot = new HashSet<string>();
    public HashSet<string> openedChests = new HashSet<string>();
    public HashSet<string> defeatedEnemies = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}