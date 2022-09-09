using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class AudioInstance : MonoBehaviour
{
    public AudioClip Clip;
    public AudioSource AudioSource;
    public Vector2 Pitch;

    void OnEnable() 
    {
        AudioSource.clip = Clip;
        Random.InitState(Mathf.RoundToInt(Time.time));
        AudioSource.pitch = Random.Range(Pitch.x, Pitch.y);
        AudioSource.Play();
        StartCoroutine(DespawnIn(Clip.length));
    }

    IEnumerator DespawnIn(float _time)
    {
        yield return new WaitForSeconds(_time);
        LeanPool.Despawn(gameObject);
    }
}
