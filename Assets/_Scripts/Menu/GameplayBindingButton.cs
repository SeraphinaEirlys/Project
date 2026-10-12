using UnityEngine;

public class GameplayBindingButton : MonoBehaviour
{
    [SerializeField] private GameplayRebindController controller;
    [SerializeField] private string actionName;
    [SerializeField] private string originalPath;

    public void StartRebind()
    {
        if (AttackBindingSlotSelector.BlocksMenuInput) return;
        if (controller == null)
        {
            Debug.LogWarning("Assign Gameplay Rebind Controller on " + name);
            return;
        }
        controller.BeginRebind(actionName, originalPath, gameObject);
    }
}
