using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private Material originalMat;
    private SpriteRenderer sr;

    [SerializeField] private Material onDamageVfxMat;
    [SerializeField] private float OnDamageVfxDuration = .2f;
    private Coroutine onDamageVfxCo;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    public void PlayOnDamageVfx()
    {
        if(onDamageVfxCo != null)
        {
            StopCoroutine(onDamageVfxCo);
        }

        onDamageVfxCo = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageVfxMat;

        yield return new WaitForSeconds(OnDamageVfxDuration);

        sr.material = originalMat;
    }
}
