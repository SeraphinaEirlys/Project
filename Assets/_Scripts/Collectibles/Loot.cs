using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class Loot : MonoBehaviour
{
    private Player player;
    [SerializeField] private CollectibleSO collectibleSO;
    [SerializeField] private SpriteRenderer sr;

    public Animator anim;
    public TMP_Text itemMessage;

    [SerializeField] private bool canBeCollected;
    [SerializeField] private float collectDelay = 0.2f;

    private PlayerInput playerInput;

    private PersistentGUID persistentGUID;

    private void Awake()
    {
        persistentGUID = GetComponent<PersistentGUID>();
    }

    private void Start()
    {
        if (persistentGUID != null && WorldState.Instance != null && WorldState.Instance.collectedLoot.Contains(persistentGUID.GUID))
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(CollectibleSO collectibleSO)
    {
        this.collectibleSO = collectibleSO;
        if (sr != null && collectibleSO != null)
        {
            sr.sprite = collectibleSO.itemSprite;
        }

        StartCoroutine(EnableCollection());
    }

    private IEnumerator EnableCollection()
    {
        yield return new WaitForSeconds(collectDelay);
        canBeCollected = true;
    }

    private void Update()
    {
        if (!canBeCollected || playerInput == null) return;

        // Kiểm tra nút bấm tương tác (phím Interact hoặc nút Hướng Lên giống Rương)
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        bool pressedUp = moveInput.y > 0.1f;
        bool pressedInteract = playerInput.actions["Interact"].WasPressedThisFrame();

        if (pressedInteract || pressedUp)
        {
            CollectItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var input = collision.GetComponentInParent<PlayerInput>();
        if (input != null)
        {
            playerInput = input;
            player = collision.GetComponentInParent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var input = collision.GetComponentInParent<PlayerInput>();
        if (input != null && input == playerInput)
        {
            playerInput = null;
            player = null;
        }
    }

    private void CollectItem()
    {
        canBeCollected = false;

        if (persistentGUID != null && WorldState.Instance != null)
        {
            WorldState.Instance.collectedLoot.Add(persistentGUID.GUID);
        }

        if (itemMessage != null && collectibleSO != null)
        {
            itemMessage.text = "Found " + collectibleSO.itemName;
        }

        if (anim != null)
        {
            anim.Play("CollectLoot");
        }

        if (collectibleSO != null)
        {
            collectibleSO.Collect(player);
        }

        Destroy(gameObject, 1f);
    }
}