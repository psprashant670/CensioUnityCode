using System.Collections;
using UnityEngine;

public class FixedCollisions : MonoBehaviour
{
    public int CollisionCount = 1;
    public bool affectedByKillBox = false;

    void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag.Equals("Enemy") || other.gameObject.tag.Equals("Destructable"))
        {
            CollisionCount--;
            if (CollisionCount <= 0)
            {
                StartCoroutine(LateDisable());
            }
        }
        else if (other.gameObject.tag.Equals("Player"))
        {
            StartCoroutine(LateDisable());
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (affectedByKillBox && other.tag.Equals("Destructable"))
        {
            StartCoroutine(LateDisable());
        }
    }

    IEnumerator LateDisable()
    {
        yield return null;
        yield return null;
        gameObject.SetActive(false);
    }
}
