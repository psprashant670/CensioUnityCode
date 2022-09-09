using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSwingController : MonoBehaviour
{
    public List<SwingSpawnner> Spawnners;
    public List<GameObject> BlockPrefabs;
    List<int> Indexes;
    int indexA, indexB, Selected;

    void Start()
    {
        Indexes = new List<int>();
        for (int i = 0; i < BlockPrefabs.Count; i++)
        {
            Indexes.Add(i);
        }
        IListExtensions.Shuffle(Indexes);
        indexA = Indexes[0];
        Indexes.RemoveAt(0);
        indexB = Indexes[0];
        Indexes.RemoveAt(0);

        StartCoroutine(WaitTillStart());
    }

    public void OnRequestBlockRecieved(SwingSpawnner _spawner)
    {
        if (_spawner == Spawnners[0])
        {
            Indexes.Add(indexA);
            IListExtensions.Shuffle(Indexes);
            indexA = Indexes[0];
            Indexes.RemoveAt(0);
            Spawnners[0].Spawn(BlockPrefabs[indexA]);
        }
        else if (_spawner == Spawnners[1])
        {
            Indexes.Add(indexB);
            IListExtensions.Shuffle(Indexes);
            indexB = Indexes[0];
            Indexes.RemoveAt(0);
            Spawnners[1].Spawn(BlockPrefabs[indexB]);
        }

        SelectRandomBlock();
    }

    void SelectRandomBlock()
    {
        BlockColor color = Random.Range(0, 2)==0?Spawnners[0].CurrentColor:Spawnners[1].CurrentColor;
        LevelManager.instance.SetBlockColor(color);
        Spawnners[0].Holder.GetChild(0).GetComponent<BuildingBlock>().SetCorrectBlock();
        Spawnners[1].Holder.GetChild(0).GetComponent<BuildingBlock>().SetCorrectBlock();
    }

    IEnumerator WaitTillStart()
    {
        yield return new WaitUntil(()=> LevelManager.instance.hasBegun );
        Spawnners[0].Spawn(BlockPrefabs[indexA]);
        Spawnners[1].Spawn(BlockPrefabs[indexB]);
        SelectRandomBlock();
    }

    private void OnEnable() 
    {
        foreach (SwingSpawnner _spawner in Spawnners)
        {
            _spawner.RequestBlock += OnRequestBlockRecieved;
        }
    }

    private void OnDisable() 
    {
        foreach (SwingSpawnner _spawner in Spawnners)
        {
            _spawner.RequestBlock -= OnRequestBlockRecieved;
        }
    }
}

public static class IListExtensions 
{
    public static void Shuffle<T>(this IList<T> ts) 
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}