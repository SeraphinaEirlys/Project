using UnityEngine;

public class VFXAutoDestroy : MonoBehaviour
{
    public float destroyTime = 0.3f;
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}