using UnityEngine;

public class PersistentGUID : MonoBehaviour
{
    [SerializeField] private string guid;

    public string GUID => guid;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
        {
            guid = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}