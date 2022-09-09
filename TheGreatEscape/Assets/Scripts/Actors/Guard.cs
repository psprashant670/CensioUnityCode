using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CapsuleCollider2D))]
public class Guard : MonoBehaviour
{
    public float Speed;
    public bool isStandingStill = false;

    Animator Animator;
    Rigidbody2D rb2D;
    SpriteRenderer SpriteRenderer;
    Vector2 Velocity;
    bool isWalking = false, isAffectedByAITriggers = true;


    void Start()
    {
        Animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        isWalking = true;
        Animator.SetBool("isWalking", isWalking);

        if (isStandingStill)
        {
            isWalking = false;
            Animator.SetBool("isWalking", isWalking);
        }
    }

    void FixedUpdate()
    {
        if (isWalking)
        {
            Velocity = new Vector2((SpriteRenderer.flipX? -Speed : Speed), rb2D.velocity.y);
            rb2D.velocity = Velocity;
        }
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
    
    public void InvestigateArea(Transform _area)
    {
        isAffectedByAITriggers = false;
        StopCoroutine(Turn());
        // Debug.Log(_area.position.x - transform.position.x); // negative = turn left, pos = right -1 0 1
        if (_area.position.x - transform.position.x < 0) // move left
        {
            SpriteRenderer.flipX = true;
        }
        else // move right
        {
            SpriteRenderer.flipX = false;
        }
        StartCoroutine(WalkToTarget((Vector2)_area.position));
    }

    IEnumerator Turn()
    {
        if (isAffectedByAITriggers)
        {
            rb2D.velocity = Vector2.zero;
            isWalking = false;
            Animator.SetBool("isWalking", isWalking);
        }
        yield return new WaitForSeconds(2f);
        if (isAffectedByAITriggers)
        {
            SpriteRenderer.flipX = !SpriteRenderer.flipX;
            isWalking = true;
            Animator.SetBool("isWalking", isWalking);
        }
    }

    IEnumerator WalkToTarget(Vector2 _target)
    {
        isWalking = true;
        Animator.SetBool("isWalking", isWalking);
        yield return new WaitUntil(()=> (Mathf.Abs(_target.x - transform.position.x) <= 0.8f) );
        isStandingStill = true;
        isWalking = false;
        Animator.SetBool("isWalking", isWalking);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag.Equals("AI") && isAffectedByAITriggers)
        {
            AIControl AI = other.GetComponent<AIControl>();
            if (AI.AffectsGuards && AI.CheckDirection(transform.position))
            {
                StopCoroutine(Turn());
                StartCoroutine(Turn());
            }
        }
        else if (other.tag.Equals("Die") && other.GetComponent<FlameThrower>()!=null && other.GetComponent<FlameThrower>().TriggerControled)
        {
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.collider.tag.Equals("Impact") && other.rigidbody.velocity.magnitude>0.01f)
        {
            Die();
        }
        else if (other.collider.tag.Equals("Player"))
        {
            Data.instance.CurrentLevel.ObstacleName = GetComponent<Obstacle>().ObstacleName;
            other.gameObject.GetComponent<PrinceController>().SetState(PlayerState.Death);
        }
    }
}
