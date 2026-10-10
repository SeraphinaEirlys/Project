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
    public GameObject pauseMenuPanel; // Giao diện cột ở giữa
    public InventoryMenu inventoryMenu; // Giao diện Inventory (Tab)
    public GameObject settingsPanel; // Tạm chứa cái OptionPanel của Settings (nếu làm chung scene)

    public bool isPaused = false;
    public string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Giải quyết lỗi DontDestroyOnLoad
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
        // Không hoạt động nếu đang ở MainMenu
        if (SceneManager.GetActiveScene().name == mainMenuSceneName) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        // Xử lý nút TAB: Chỉ mở Inventory nếu Pause Menu KHÔNG mở
        if (kb.tabKey.wasPressedThisFrame)
        {
            if (inventoryMenu != null)
            {
                if (inventoryMenu.IsOpen)
                {
                    CloseInventory();
                }
                else if (!isPaused) // Nếu game không bị pause bởi PauseMenu thì mới được mở Inventory
                {
                    OpenInventory();
                }
            }
        }

        // Xử lý nút ESC: Ưu tiên đóng các menu con trước, sau đó mới đóng/mở PauseMenu
        if (kb.escapeKey.wasPressedThisFrame)
        {
            // 1. Nếu Settings đang mở -> Đóng Settings, quay lại PauseMenu
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            // 2. Nếu Inventory đang mở -> Đóng Inventory
            else if (inventoryMenu != null && inventoryMenu.IsOpen)
            {
                CloseInventory();
            }
            // 3. Nếu không có gì mở -> Đóng/Mở PauseMenu chính
            else
            {
                if (isPaused) Resume();
                else Pause();
            }
        }
    }

    // ---------- Pause Menu Chính ----------
    public void Pause()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        // Xóa lựa chọn mặc định.
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

    // ---------- Settings ----------
    public void OpenSettings()
    {
        // Ẩn các nút của PauseMenu đi, bật bảng Setting lên
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    // ---------- Inventory ----------
    public void OpenInventory()
    {
        if (inventoryMenu == null) return;
        inventoryMenu.Open();
        Time.timeScale = 0f;
        // Không gán isPaused = true ở đây, để hệ thống phân biệt được là đang mở Inventory hay PauseMenu
    }

    public void CloseInventory()
    {
        if (inventoryMenu != null) inventoryMenu.Close();
        Time.timeScale = 1f;
    }

    // ---------- Chức năng khác ----------
    public void LoadTitleScreen()
    {
        Resume(); // Đảm bảo reset lại Time.timeScale = 1f
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}