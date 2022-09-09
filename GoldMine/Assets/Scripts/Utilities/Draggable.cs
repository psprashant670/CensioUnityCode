using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Draggable : MonoBehaviour
{
    public Tile2 Parent;
    public bool isDragged = false;
    public LayerMask TileLayer;
    public List<Tile2> ValidTiles;
    Camera cam;
    BoxCollider2D BoxCollider2D;

    void Start()
    {
        cam = Camera.main;
        BoxCollider2D = GetComponent<BoxCollider2D>();
        Parent = GetComponentInParent<Tile2>();
        Parent.hasObstacle = true;
        ValidTiles = new List<Tile2>();
    }

    void FixedUpdate() 
    {
        if (isDragged)
        {
            transform.position = cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        // Debug.Log(other.name);
    }

    public void Pick(GridManager2 _gridManager)
    {
        Vector2Int MyPos = Parent.GridPos;
        ValidTiles.Clear();

        //validate left
        for (int i = MyPos.x-1; i >= 0; i--)
        {
            Tile2 _tile = _gridManager.Grid[i, MyPos.y];
            if (_tile.hasObstacle || (_tile.isTarget && !tag.Equals("Player")))
            {
                break;
            }
            if (tag.Equals("Player") && _tile.isTarget && isTargetBlocked(_tile))
            {
                break;
            }
            ValidTiles.Add(_tile);
            _tile.ValidIndicator.SetActive(true);
        }

        //validate right
        for (int i = MyPos.x+1; i < _gridManager.GridSize.x; i++)
        {
            Tile2 _tile = _gridManager.Grid[i, MyPos.y];
            if (_tile.hasObstacle || (_tile.isTarget && !tag.Equals("Player")))
            {
                break;
            }
            if (tag.Equals("Player") && _tile.isTarget && isTargetBlocked(_tile))
            {
                break;
            }
            ValidTiles.Add(_tile);
            _tile.ValidIndicator.SetActive(true);
        }

        //validate top
        for (int i = MyPos.y-1; i >= 0; i--)
        {
            Tile2 _tile = _gridManager.Grid[MyPos.x, i];
            if (_tile.hasObstacle || (_tile.isTarget && !tag.Equals("Player")))
            {
                break;
            }
            if (tag.Equals("Player") && _tile.isTarget && isTargetBlocked(_tile))
            {
                break;
            }
            ValidTiles.Add(_tile);
            _tile.ValidIndicator.SetActive(true);
        }

        //validate down
        for (int i = MyPos.y+1; i < _gridManager.GridSize.y; i++)
        {
            Tile2 _tile = _gridManager.Grid[MyPos.x, i];
            if (_tile.hasObstacle || (_tile.isTarget && !tag.Equals("Player")))
            {
                break;
            }
            if (tag.Equals("Player") && _tile.isTarget && isTargetBlocked(_tile))
            {
                break;
            }
            ValidTiles.Add(_tile);
            _tile.ValidIndicator.SetActive(true);
        }
    }

    bool isTargetBlocked(Tile2 _target)
    {
        bool isBlocked = false;
        if (_target.TargetDir.x != 0)
        {
            if (_target.TargetDir.x == 1)
            {
                isBlocked = (_target.GridPos.y!=Parent.GridPos.y && _target.GridPos.x<Parent.GridPos.x);
            }
            else if (_target.TargetDir.x == -1)
            {
                isBlocked = (_target.GridPos.y!=Parent.GridPos.y && _target.GridPos.x>Parent.GridPos.x);
            }
        }
        else if (_target.TargetDir.y != 0)
        {
            if (_target.TargetDir.y == 1)
            {
                isBlocked = (_target.GridPos.x!=Parent.GridPos.x && _target.GridPos.y<=Parent.GridPos.y);
            }
            else if (_target.TargetDir.y == -1)
            {
                isBlocked = (_target.GridPos.x!=Parent.GridPos.x && _target.GridPos.y>Parent.GridPos.y);
            }
        }
        return isBlocked;
    }

    public void Drop()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(BoxCollider2D.bounds.center, BoxCollider2D.bounds.size, 0, TileLayer);

        Tile2 closest = Parent;
        for (int i = 0; i < colliders.Length; i++)
        {
            bool _closer = Vector2.Distance(transform.position, closest.transform.position) > Vector2.Distance(transform.position, colliders[i].transform.position);
            bool _valid = ValidTiles.Contains(colliders[i].GetComponent<Tile2>());
            if (_closer && _valid)
            {
                closest = colliders[i].GetComponent<Tile2>();
            }
        }

        Parent.hasObstacle = false;
        Parent = closest;
        Parent.hasObstacle = true;
        transform.parent = closest.transform;
        transform.DOLocalMove(Vector2.zero, .3f).OnComplete(()=>
        {
            if (tag.Equals("Player") && Parent.isTarget)
            {
                transform.DOScale(Vector2.zero, 0.5f);
                Invoke("TargetReached", 0.5f);
            }
        });

        foreach (Tile2 _valid in ValidTiles)
        {
            _valid.isValid = false;
            _valid.ValidIndicator.SetActive(false);
        }
        ValidTiles.Clear();
    }
    
    void TargetReached()
    {
        LevelManager.instance.SetLevelCompleted();
        LevelManager.instance.hasBegun = false;
        InGameUI.instance.ShowWinPanel();
        Debug.Log("complete");
    }
}
