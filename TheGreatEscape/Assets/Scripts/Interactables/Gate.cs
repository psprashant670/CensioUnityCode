using UnityEngine;
using DG.Tweening;

public class Gate : Interactable
{
    public Transform Door;
    public Vector2 A, B;
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
        Door.DOLocalMove(_target, 1.2f).OnComplete(()=> {
            if (!TriggerOnce)
                isInteractable = true;
                ShowFX(isInteractable);
        });
    }
}
