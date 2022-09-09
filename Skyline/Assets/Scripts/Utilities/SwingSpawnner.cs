using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System;

public class SwingSpawnner : MonoBehaviour
{
    public GameObject BlockPrefab;
    public Transform Pivot, Holder, RotA, RotB;
    public GameObject Rope;
    public bool isManualSpawn = false;
    public Action<SwingSpawnner> RequestBlock;
    WaitForSeconds wait;
    public BlockColor CurrentColor;
    public AnimationCurve Curve;

    void Start()
    {
        Rope.SetActive(false);
        StartCoroutine(WaitTillStart());
    }

    void Update() 
    {
        // Pivot.rotation = Quaternion.Lerp(RotA.rotation, RotB.rotation, Mathf.Abs(Time.time%2-1));
        Pivot.rotation = Quaternion.Lerp(RotA.rotation, RotB.rotation, Curve.Evaluate(Time.time));
    }

    public void Spawn()
    {
        if (LevelManager.instance.hasBegun)
        {
            GameObject go = LeanPool.Spawn(BlockPrefab, Holder);
            Data.instance.CurrentLevel.blocksPresented++;
            go.GetComponent<BuildingBlock>().OnSpawn(Data.instance.CurrentLevel.blocksPresented);
            go.GetComponent<BuildingBlock>().Touched += OnDropped;
            go.GetComponent<BuildingBlock>().Landed += TrySpawnNext;
            go.GetComponent<BuildingBlock>().SetCorrectBlock();
            Rope.SetActive(true);
        }
    }

    public void Spawn(GameObject _prefab)
    {
        if (LevelManager.instance.hasBegun)
        {
            GameObject go = LeanPool.Spawn(_prefab, Holder);
            Data.instance.CurrentLevel.blocksPresented++;
            go.GetComponent<BuildingBlock>().OnSpawn(Data.instance.CurrentLevel.blocksPresented);
            go.GetComponent<BuildingBlock>().Touched += OnDropped;
            go.GetComponent<BuildingBlock>().Landed += TrySpawnNext;
            CurrentColor = go.GetComponent<BuildingBlock>().Color;
            Rope.SetActive(true);
        }
    }

    public void OnDropped(BuildingBlock _ref)
    {
        _ref.Touched -= OnDropped;
        Rope.SetActive(false);
    }

    public void TrySpawnNext(BuildingBlock _ref)
    {
        _ref.Landed -= TrySpawnNext;
        if (Holder.childCount <= 0 && !isManualSpawn && LevelManager.instance.hasBegun)
        {
            Invoke("Spawn", 0.5f);
        }
        else if (Holder.childCount <= 0 && isManualSpawn && LevelManager.instance.hasBegun)
        {
            RequestBlock?.Invoke(this);
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

    IEnumerator WaitTillStart()
    {
        yield return new WaitUntil(()=> LevelManager.instance.hasBegun );
        if (!isManualSpawn)
        {
            LevelManager.instance.SetBlockColor(BlockPrefab.GetComponent<BuildingBlock>().Color);
            Spawn();
        }
    }
}
