using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlatformTween : MonoBehaviour
{
    public Vector2 A, B;
    public float WaitTime = 0f, TimeToTween = 2f;

    Vector2 NextTarget;

    void Start()
    {
        NextTarget = A;
        TweenTo(NextTarget);
    }

    void TweenTo(Vector2 _target)
    {
        NextTarget = NextTarget.Equals(A) ? B : A;
        transform.DOMove(_target, TimeToTween).SetEase(Ease.Linear).OnComplete(()=> StartCoroutine(DelayedTween()));
    }

    IEnumerator DelayedTween()
    {
        yield return new WaitForSeconds(WaitTime);
        TweenTo(NextTarget);
    }
}
