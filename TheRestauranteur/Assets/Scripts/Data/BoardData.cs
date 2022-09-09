using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoardData : ScriptableObject
{
    public TileType Festival;
    
    public int EmptyCount, WheatCount, GrapeCount, ChocolateCount, MilkCount;
    public int PoolEmptyCount, PoolWheatCount, PoolGrapeCount, PoolChocolateCount, PoolMilkCount;

    [Multiline(20)]
    public string BoardLayout; 
}
