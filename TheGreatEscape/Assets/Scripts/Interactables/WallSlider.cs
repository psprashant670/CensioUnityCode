using UnityEngine;
using DG.Tweening;

public class WallSlider : Interactable
{
    public Vector2 A, B;
    public Transform Target;
    Vector2 NextTarget;

    void Start()
    {
        isInteractable = true;
        NextTarget = A;
        ShowLights(false);
    }

    public override void Interact()
    {
        if (isInteractable)
        {
            if (TriggerOnce)
            {
                isInteractable = false;
            }

            base.LogValidClick();
            TweenTo(NextTarget);
        }
    }

    void TweenTo(Vector2 _target)
    {
        isInteractable = false;
        ShowFX(isInteractable);
        ShowLights(NextTarget.Equals(A));
        NextTarget = NextTarget.Equals(A) ? B : A;
        Target.DOLocalMove(_target, 1.2f).OnComplete(()=> {
            if (!TriggerOnce)
                isInteractable = true;
                ShowFX(isInteractable);
        });
    }
}
