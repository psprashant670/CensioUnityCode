using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CapsuleCollider2D))]
public class PrinceController : MonoBehaviour
{
    public float Speed;
    public Animator Animator;
    public SpriteRenderer SpriteRenderer;

    public bool isMoving, isGrounded, isAlive, isClimbing;
    public LayerMask GroundCheckMask;
    public Vector2 Forward { get { return SpriteRenderer.flipX? -transform.right : transform.right; } }
    Rigidbody2D rb2D;
    Vector2 Velocity;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        isAlive = true;
        isClimbing = false;
        isMoving = false;
        isGrounded = false;
        SetState(PlayerState.Stop);
    }

    void FixedUpdate()
    {
        if (isMoving && !isClimbing)
        {
            Velocity = new Vector2((SpriteRenderer.flipX? -Speed : Speed), rb2D.velocity.y);
            // rb2D.velocity = Velocity;
            transform.Translate(transform.right * Time.deltaTime * (SpriteRenderer.flipX? -Speed : Speed), Space.Self);
        }
        
    }

    public void SetState(PlayerState _state)
    {
        switch (_state)
        {
            case PlayerState.Stop:
            rb2D.velocity = Vector2.zero;
            isMoving = false;
            Animator.SetBool("isMoving", isMoving);
            isClimbing = false;
            Animator.SetBool("isClimbing", isClimbing);
            break;

            case PlayerState.Run:
            rb2D.velocity = Vector2.zero;
            isMoving = true;
            Animator.SetBool("isMoving", isMoving);
            isClimbing = false;
            Animator.SetBool("isClimbing", isClimbing);
            break;

            case PlayerState.Climb:
            rb2D.velocity = Vector2.zero;
            isClimbing = true;
            Animator.SetBool("isClimbing", isClimbing);
            break;

            case PlayerState.Death:
            Die();
            break;

            case PlayerState.FallLeft:
            rb2D.velocity = rb2D.velocity + Vector2.down*2f + Vector2.left;
            break;

            case PlayerState.FallRight:
            rb2D.velocity = rb2D.velocity + Vector2.down*2f + Vector2.right;
            break;

            default:
            break;
        }
    }

    public void Jump(Vector2 _dir)
    {
        rb2D.AddForce(_dir, ForceMode2D.Impulse);
    }

    public void Die()
    {
        gameObject.SetActive(false);
        GameManager.instance.MenuState = MenuState.Dead;
        Data.instance.CurrentLevel.playerDied = true;
        if (LevelManager.instance.AttemptNo >= Data.instance.MaxAttempts)
        {
            GameManager.instance.MenuState = MenuState.Failed;
        }
        LevelManager.instance.SetLevelFailedResult();
        GameManager.instance.LoadSceneWithDelay("Menu", 1.2f);
    }

    bool checkGrounded()
    {
        RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, Vector2.down, 0.7f, GroundCheckMask);
        // groundCheck.collider.CompareTag("Ground")
        return (groundCheck.collider != null);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag.Equals("AI"))
        {
            AIControl AI = other.GetComponent<AIControl>();
            if (AI.AffectsPlayer && AI.CheckDirection(transform.position))
            {
                StopCoroutine(Turn());
                StartCoroutine(Turn());
            }
        }
        else if (other.tag.Equals("Die"))
        {
            if (other.GetComponent<Obstacle>() != null)
            {
                Data.instance.CurrentLevel.ObstacleName = other.GetComponent<Obstacle>().ObstacleName;
            }
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.collider.tag.Equals("Die") && other.relativeVelocity.magnitude>1.2f)
        {
            if (other.collider.GetComponent<Obstacle>() != null)
            {
                Data.instance.CurrentLevel.ObstacleName = other.collider.GetComponent<Obstacle>().ObstacleName;
            }
            Die();
        }
        else if (other.collider.tag.Equals("Impact") && other.rigidbody.velocity.magnitude>0.1f)
        {
            if (other.collider.GetComponent<Obstacle>() != null)
            {
                Data.instance.CurrentLevel.ObstacleName = other.collider.GetComponent<Obstacle>().ObstacleName;
            }
            Die();
        }
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, transform.position + (Vector3.down*0.7f));
    }

    IEnumerator Turn()
    {
        SetState(PlayerState.Stop);
        yield return new WaitForSeconds(0.1f);
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
        SetState(PlayerState.Run);
    }
}

public enum PlayerState
{
    None,
    Stop,
    Run,
    Climb,
    Death,
    FallLeft,
    FallRight
}
