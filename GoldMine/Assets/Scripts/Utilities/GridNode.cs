using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public Vector2Int GridPos;
    public Tile CurrentTile;
    public int LinearPos;
    public bool isEmpty { get { return GetComponentInChildren<Tile>().isEmpty; } }

    public void ClearChild()
    {
        CurrentTile = null;
        transform.DetachChildren();
    }

    public void SetChild(Tile _tile)
    {
        CurrentTile = _tile;
        _tile.transform.SetParent(transform);
    }
}
