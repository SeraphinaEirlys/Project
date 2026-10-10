using UnityEngine;
using Unity.Cinemachine; 
using System.Collections;

public class CameraBounds : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DelayedSetup());
    }

    private IEnumerator DelayedSetup()
    {
        yield return new WaitForSeconds(0.1f);
        SetupConfiner();
    }

    public void SetupConfiner()
    {
        CinemachineCamera vCam = FindAnyObjectByType<CinemachineCamera>();
        
        if (vCam != null)
        {
            CinemachineConfiner2D confiner = vCam.GetComponent<CinemachineConfiner2D>();
            
            if (confiner != null)
            {
                confiner.BoundingShape2D = GetComponent<Collider2D>();
                confiner.InvalidateBoundingShapeCache();
            }
        }
    }
}