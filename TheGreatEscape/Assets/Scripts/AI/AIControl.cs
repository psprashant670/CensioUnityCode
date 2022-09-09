using UnityEngine;

public class AIControl : MonoBehaviour
{
    public bool AffectsPlayer, AffectsGuards, AllowLeft = true, AllowRight = true;

    public bool CheckDirection(Vector2 _pos)
    {
        bool state = false;
        if (AllowLeft && (transform.position.x - _pos.x)>0)
        {
            state = true;
        }
        if (AllowRight && (transform.position.x - _pos.x)<0)
        {
            state = true;
        }
        return state;
    }

    [Header("Player State Override")]
    public PlayerState OverrideState;
    public bool TriggerOnce;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag.Equals("Player") && OverrideState!=PlayerState.None)
        {
            PrinceController princeController = other.GetComponent<PrinceController>();
            princeController.SetState(OverrideState);
        }

        if (TriggerOnce)
        {
            gameObject.SetActive(false);
        }
    }
}
