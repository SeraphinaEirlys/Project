using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject transitionsContainer;
    public Slider progressBar;

    private SceneTransition[] transitions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null, true);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>(true);
        if (progressBar != null) progressBar.gameObject.SetActive(false);
    }

    public void LoadScene(string sceneName, string transitionName = "CrossFade")
    {
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        SceneTransition transition = transitions.FirstOrDefault(t => t.gameObject.name == transitionName);

        AsyncOperation sceneOp = SceneManager.LoadSceneAsync(sceneName);
        sceneOp.allowSceneActivation = false;

        if (transition != null)
        {
            yield return StartCoroutine(transition.AnimateTransitionIn());
        }

        if (progressBar != null) progressBar.gameObject.SetActive(true);

        while (sceneOp.progress < 0.9f)
        {
            if (progressBar != null) progressBar.value = sceneOp.progress;
            yield return null;
        }

        if (progressBar != null)
        {
            progressBar.value = 1f;
            progressBar.gameObject.SetActive(false);
        }

        sceneOp.allowSceneActivation = true;

        yield return null;

        if (transition != null)
        {
            yield return StartCoroutine(transition.AnimateTransitionOut());
        }
    }
}