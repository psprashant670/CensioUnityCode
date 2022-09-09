using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHolder : MonoBehaviour
{
    public Vector2Int GridPos;
    public int LinearPos;
    public Tile CurrentTile;

    public void Init(Vector2Int _gridPos, int _linearPos)
    {
        GridPos = _gridPos;
        LinearPos = _linearPos;
        gameObject.name = GridPos.ToString();
    }

    public void SetTile(Tile _tile, bool _keepWordPos)
    {
        CurrentTile = _tile;
        CurrentTile.GridPos = GridPos;
        CurrentTile.transform.SetParent(transform, _keepWordPos);
    }

    public void RemoveTile()
    {
        CurrentTile.transform.parent = null;
        CurrentTile.GridPos = Vector2Int.one * -1;
        CurrentTile = null;
    }
}
