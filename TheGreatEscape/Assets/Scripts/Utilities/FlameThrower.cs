using System.Collections;
using UnityEngine;

public class FlameThrower : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    public BoxCollider2D BoxCollider2D;
    public float OnTime = 2f, OffTime = 6f;
    public bool TriggerControled = false, TriggerOnce = true;

    bool isFiring = false;

    void Start()
    {
        if (!TriggerControled)
        {
            isFiring = true;
            StartCoroutine(AutoFlame());
        }
        else
        {
            isFiring = true;
            ToggleFlame();
        }
    }

    public void Trigger()
    {
        if (TriggerOnce)
        {
            Data.instance.ClickLog.Add(GetComponent<Obstacle>().ObstacleName);
            TriggerOnce = false;
            isFiring = false;
            ToggleFlame();
            StartCoroutine(AutoFlame());
        }
    }

    public void ToggleFlame()
    {
        isFiring = !isFiring;
        BoxCollider2D.enabled = isFiring;
        if (isFiring)
        {
            ParticleSystem.Play();
        }
        else
        {
            ParticleSystem.Stop();
        }
    }

    IEnumerator AutoFlame()
    {
        while (true)
        {
            yield return new WaitForSeconds(isFiring? OnTime : OffTime);
            ToggleFlame();
        }
    }
}
