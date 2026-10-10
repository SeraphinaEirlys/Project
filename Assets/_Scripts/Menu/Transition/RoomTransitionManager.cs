using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransitionManager : MonoBehaviour
{
    private bool isTransitioning = false;
    private string currentRoom = "";

    private void Start()
    {
        if (isTransitioning) 
            return;

        EnterRoom("", "");
    }

    public void EnterRoom(string sceneName, string spawnID)
    {
        StartCoroutine(Transition(sceneName, spawnID));
    }

    private IEnumerator Transition(string sceneName, string spawnID)
    {
        isTransitioning = true; 
        
        Player player = FindAnyObjectByType<Player>();
        if (player != null) 
        {
            player.isControlLocked = true;
            
            player.ChangeState(player.idleState);
            player.rb.linearVelocity = Vector2.zero;
        }

        if (LevelManager.Instance != null)
        {
            SceneTransition crossFade = LevelManager.Instance.transitionsContainer.GetComponentInChildren<CrossFade>();
            if (crossFade != null) yield return StartCoroutine(crossFade.AnimateTransitionIn());
        }

        if (!string.IsNullOrEmpty(currentRoom))
        {
            yield return SceneManager.UnloadSceneAsync(currentRoom);
        }
        if (!string.IsNullOrEmpty(sceneName))
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            currentRoom = sceneName;
            SetupRoom(spawnID);
        }
        else
        {
            currentRoom = SceneManager.GetActiveScene().name;
            SetupRoom(spawnID);
        }

        yield return new WaitForSeconds(0.25f); 

        if (LevelManager.Instance != null)
        {
            SceneTransition crossFade = LevelManager.Instance.transitionsContainer.GetComponentInChildren<CrossFade>();
            if (crossFade != null) yield return StartCoroutine(crossFade.AnimateTransitionOut());
        }

        if (player != null) player.isControlLocked = false;
        
        isTransitioning = false; 
    }

    private void SetupRoom(string spawnID)
    {
        if (string.IsNullOrEmpty(spawnID)) return;

        SpawnPoint[] spawns = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        if (spawns.Length == 0) return;

        SpawnPoint spawnToUse = spawns[0];
        foreach (SpawnPoint spawn in spawns)
        {
            if (spawn.spawnID == spawnID)
            {
                spawnToUse = spawn;
                break;
            }
        }

        transform.position = spawnToUse.transform.position;
    }
}