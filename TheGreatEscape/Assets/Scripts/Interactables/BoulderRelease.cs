using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BoulderRelease : Interactable
{
    public Vector3 A, B;
    public Transform Pivot, Cage;
    public Rigidbody2D Boulder;
    public Collider2D Brick;
    Vector3 NextTarget;

    void Start()
    {
        TriggerOnce = true;
        isInteractable = true;
        NextTarget = A;
    }

    public override void Interact()
    {
        if (isInteractable)
        {
            if (TriggerOnce)
            {
                isInteractable = false;
            }
            Cage.gameObject.SetActive(false);
            Boulder.isKinematic = false;

            base.LogValidClick();
            TweenTo(NextTarget);
        }
    }

    void TweenTo(Vector3 _target)
    {
        isInteractable = false;
        ShowFX(isInteractable);
        NextTarget = NextTarget.Equals(A) ? B : A;
        Pivot.DORotate(_target, 0.1f).OnComplete(()=> {
            StartCoroutine(DisableBrick());
            if (!TriggerOnce)
                isInteractable = true;
                ShowFX(isInteractable);
        });
    }

    IEnumerator DisableBrick()
    {
        yield return new WaitForSeconds(2f);
        Brick.enabled = false;
    }
}
