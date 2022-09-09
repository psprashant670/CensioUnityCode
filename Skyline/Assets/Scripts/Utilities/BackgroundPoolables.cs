using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BackgroundPoolables : MonoBehaviour
{
    public BackgroundPoolable BackgroundPoolable;
    public List<Transform> SpawnPoints;
    public Vector2 Direction;
    public float Speed;
    public bool RandomizePos = false;
    public bool UseParentRotation = false;

    float Timer;

    void Start()
    {
        Timer = 0;
    }

    void Update() 
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            Timer = Random.Range(BackgroundPoolable.SpawnMin, BackgroundPoolable.SpawnMax);
            Spawn();
        }

        foreach (Transform _child in transform)
        {
            _child.Translate(Direction.normalized*Speed*Time.deltaTime, Space.World);
        }
    }

    public void Spawn()
    {
        GameObject go = LeanPool.Spawn(BackgroundPoolable.Prefab[Random.Range(0, BackgroundPoolable.Prefab.Count)], SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity, transform);
        go.transform.localScale = Vector2.one * Random.Range(BackgroundPoolable.ScaleMin, BackgroundPoolable.ScaleMax);
        if (UseParentRotation)
        {
            go.transform.rotation = transform.rotation;
        }
        if (RandomizePos)
        {
            go.transform.position += Random.onUnitSphere;
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.transform.parent == transform)
        {
            LeanPool.Despawn(other.gameObject);
        }
    }
}

[System.Serializable]
public class BackgroundPoolable
{
    public List<GameObject> Prefab;
    public float SpawnMin, SpawnMax;
    public float ScaleMin, ScaleMax;

}
