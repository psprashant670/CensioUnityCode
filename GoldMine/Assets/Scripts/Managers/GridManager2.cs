using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridManager2 : MonoBehaviour
{
    public Vector2Int GridSize;
    public Vector2 TileOffset;
    public Tile2 TilePrefab;
    public Transform Root;

    public Tile2[,] Grid;
    public List<Tile2> GridTileList;

    [SerializeField]
    LayerMask Draggable;
    Draggable CurrentDraggable;

    void Start()
    {
        Grid = new Tile2[GridSize.x, GridSize.y];
        foreach (Tile2 _tile in GridTileList)
        {
            Grid[_tile.GridPos.x, _tile.GridPos.y] = _tile;
        }
    }

    public void ReSpawnGrid()
    {
        GridTileList = new List<Tile2>();

        // clear children
        for (int i = Root.childCount-1; i >= 0; i--)
        {
            DestroyImmediate(Root.GetChild(i).gameObject);
        }

        // create children
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                int LinearPos = (GridSize.x*x)+y;
                Tile2 go = Instantiate(TilePrefab, Root, true);
                go.LinearPos = LinearPos;
                go.name = LinearPos.ToString();
                go.transform.localPosition = new Vector2(TileOffset.x*x, TileOffset.y*y);
                go.transform.localScale = Vector2.one;
                go.GridPos = new Vector2Int(x, y);
                GridTileList.Add(go);
            }
        }
    }

    [ContextMenu("Create Grid")]
    public void CreateGrid()
    {
        ReSpawnGrid();
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    public void Touch(InputAction.CallbackContext value)
    {
        if (!LevelManager.instance.hasBegun || InGameUI.instance.isPanelOpen)
        {
            return;
        }
        if (value.performed)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue()), new Vector2(0.01f, 0.01f), 0.02f, Draggable);

            if (hit.collider!=null)
            {
                CurrentDraggable = hit.collider.GetComponent<Draggable>();
                CurrentDraggable.isDragged = true;
                CurrentDraggable.Pick(this);
                Data.instance.CurrentLevel.noOfMoves++;
            }
        }
        else if (value.canceled)
        {
            if (CurrentDraggable != null)
            {
                CurrentDraggable.isDragged = false;
                CurrentDraggable.Drop();
                CurrentDraggable = null;
            }
        }
    }

    private void OnEnable() 
    {
        InputManager.MouseClick += Touch;
    }

    private void OnDisable() 
    {
        InputManager.MouseClick -= Touch;
        InGameUI.instance.AttemptNo_txt.gameObject.SetActive(false);
        InGameUI.instance.Timer_txt.gameObject.SetActive(false);
        InGameUI.instance.TimerUI.gameObject.SetActive(false);
    }
}
