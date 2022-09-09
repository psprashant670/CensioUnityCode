using UnityEngine;
using DG.Tweening;

public class CatapultRotate : Interactable
{
    public Vector3 A, B;
    public Transform Pivot, Stuck, Free;
    [Header("Explosion")]
    public Transform ExplosionPoint;
    public BoxCollider2D BoxCollider2D;
    public float ExplosionForceMulti = 5f;
    Vector3 NextTarget;
    Collider2D [] inRadius;

    void Start()
    {
        TriggerOnce = true;
        isInteractable = true;
        NextTarget = A;
        BoxCollider2D.enabled = false;
    }

    public override void Interact()
    {
        if (isInteractable)
        {
            if (TriggerOnce)
            {
                isInteractable = false;
            }
            Stuck.gameObject.SetActive(false);
            Free.gameObject.SetActive(true);

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
            Explode();
            if (!TriggerOnce)
                isInteractable = true;
                ShowFX(isInteractable);
        });
    }

    void Explode()
    {

        BoxCollider2D.enabled = true;
        inRadius = Physics2D.OverlapBoxAll((Vector2)transform.position+BoxCollider2D.offset, BoxCollider2D.size, 0, LayerMask.GetMask("Player"));
        foreach (Collider2D _coll in inRadius)
        {
            Vector2 distance = _coll.transform.position - ExplosionPoint.position;
            Rigidbody2D rb2D = _coll.GetComponent<Rigidbody2D>();
            float explosionForce = ExplosionForceMulti/distance.magnitude;
            // Debug.Log(explosionForce);
            if (rb2D != null)
            {
                rb2D.AddForce(distance.normalized * ExplosionForceMulti, ForceMode2D.Impulse);
            }
        }
    }
}
