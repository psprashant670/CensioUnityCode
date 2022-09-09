using UnityEngine;
using DG.Tweening;

public class Rope : Interactable
{
    public float A, B;
    float NextTarget;
    SpriteRenderer SpriteRenderer;
    BoxCollider2D BoxCollider;

    [Header("Player Climb")]
    public Transform Top, Bot;

    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider = GetComponent<BoxCollider2D>();
        BoxCollider.enabled = false;
        NextTarget = A;
        isInteractable = true;
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
            TweenTo();
        }
    }

    void TweenTo()
    {
        isInteractable = false;
        ShowFX(isInteractable);
        NextTarget = (NextTarget == A) ? B : A;
        DOTween.To(SetSpriteSize, SpriteRenderer.size.y, NextTarget, 1f).OnComplete(()=> {
            if (!TriggerOnce)
                isInteractable = true;
                BoxCollider.enabled = (NextTarget == B) ? true : false;
                SpriteRenderer.size = new Vector2(SpriteRenderer.size.x, NextTarget);
                ShowFX(isInteractable);
        });
    }

    void SetSpriteSize(float _size)
    {
        SpriteRenderer.size = new Vector2(SpriteRenderer.size.x, _size);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag.Equals("Player"))
        {
            PrinceController prince = other.GetComponent<PrinceController>();
            float topDist = Vector2.Distance(other.transform.position, Top.position);
            float botDist = Vector2.Distance(other.transform.position, Bot.position);
            prince.SetState(PlayerState.Climb);
            if (topDist<botDist)
            {
                other.transform.position = Top.position;
                other.transform.DOMove(Bot.position, 2f).OnComplete(()=>{
                    prince.SetState(PlayerState.Run);
                    prince.Jump(prince.Forward+Vector2.down);
                    TweenTo();
                });
            }
            else
            {
                other.transform.position = Bot.position;
                other.transform.DOMove(Top.position, 2f).OnComplete(()=>{
                    prince.SetState(PlayerState.Run);
                    prince.Jump((prince.Forward+Vector2.up)*1.2f);
                    TweenTo();
                });
            }
        }
    }
}
