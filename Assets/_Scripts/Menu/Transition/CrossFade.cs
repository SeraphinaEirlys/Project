using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CrossFade : SceneTransition
{
    public CanvasGroup canvasGroup;
    
    public float fadeToBlackDuration = 0.1f; 

    public float fadeToClearDuration = 0.5f;

    public override IEnumerator AnimateTransitionIn()
    {
        yield return canvasGroup.DOFade(1f, fadeToBlackDuration).SetEase(Ease.OutQuint).WaitForCompletion();
    }

    public override IEnumerator AnimateTransitionOut()
    {
        yield return canvasGroup.DOFade(0f, fadeToClearDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
    }
}