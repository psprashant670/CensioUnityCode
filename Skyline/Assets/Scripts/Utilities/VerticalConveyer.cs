using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

[RequireComponent(typeof(BoxCollider2D))]
public class VerticalConveyer : MonoBehaviour
{
    public List<GameObject> BlockPrefabs;
    public Vector2 Direction;
    public float SpawnTime;
    WaitForSeconds wait;

    void Start()
    {
        wait = new WaitForSeconds(SpawnTime);
        LevelManager.instance.SetBlockColor(BlockColor.Green);
        StartCoroutine(WaitTillStart());
    }

    void Update() 
    {
        foreach (Transform _child in transform)
        {
            _child.Translate(Direction*Time.deltaTime, Space.World);
        }
    }

    IEnumerator WaitTillStart()
    {
        yield return new WaitUntil(()=> LevelManager.instance.hasBegun );
        StartCoroutine("Spawning");
    }

    IEnumerator Spawning()
    {
        while (LevelManager.instance.hasBegun)
        {
            GameObject go = LeanPool.Spawn(BlockPrefabs[Random.Range(0, BlockPrefabs.Count)], transform);
            Data.instance.CurrentLevel.blocksPresented++;
            go.GetComponent<BuildingBlock>().OnSpawn(Data.instance.CurrentLevel.blocksPresented);
            go.GetComponent<BuildingBlock>().SetCorrectBlock();
            yield return wait;
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.tag.Equals("Block") && !other.GetComponent<BuildingBlock>().hasLanded)
        {
            other.GetComponent<BuildingBlock>().CreateLog();
            other.GetComponent<BuildingBlock>().Landed?.Invoke(other.GetComponent<BuildingBlock>());
            LeanPool.Despawn(other.gameObject);
        }
    }
}
