using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("Default Selection")]
    public GameObject continueButton;

    [Header("Menu References")]
    public GameObject pauseMenuPanel;
    public InventoryMenu inventoryMenu;
    public GameObject settingsPanel;

    public bool isPaused = false;
    public string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        isPaused = false;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == mainMenuSceneName) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.tabKey.wasPressedThisFrame)
        {
            if (inventoryMenu != null)
            {
                if (inventoryMenu.IsOpen)
                {
                    CloseInventory();
                }
                else if (!isPaused)
                {
                    OpenInventory();
                }
            }
        }

        if (kb.escapeKey.wasPressedThisFrame)
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else if (inventoryMenu != null && inventoryMenu.IsOpen)
            {
                CloseInventory();
            }
            else
            {
                if (isPaused) Resume();
                else Pause();
            }
        }
    }

    public void Pause()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void Resume()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenSettings()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void OpenInventory()
    {
        if (inventoryMenu == null) return;
        inventoryMenu.Open();
        Time.timeScale = 0f;
    }

    public void CloseInventory()
    {
        if (inventoryMenu != null) inventoryMenu.Close();
        Time.timeScale = 1f;
    }

    public void LoadTitleScreen()
    {
        Resume();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}