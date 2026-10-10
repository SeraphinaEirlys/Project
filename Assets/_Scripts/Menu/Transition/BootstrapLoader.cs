using UnityEngine;
using UnityEngine.SceneManagement;

public static class BootstrapLoader
{
    private const string bootstrapScene = "Bootstrap";
    private const string mainMenuScene = "MainMenu";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureBootstrapLoaded(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureBootstrapLoaded(scene);
    }

    private static void EnsureBootstrapLoaded(Scene currentScene)
    {
        if (currentScene.name == mainMenuScene) return;

        if (!SceneManager.GetSceneByName(bootstrapScene).isLoaded)
        {
            SceneManager.LoadSceneAsync(bootstrapScene, LoadSceneMode.Additive);
        }
    }
}