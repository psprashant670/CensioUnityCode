using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public Vector2Int GridSize;
    public GridNode GridNodePrefab;
    public Tile TilePrefab;
    public List<Sprite> Sprites;

    public GridNode[,] Grid;
    public List<GridNode> GridNodeList;

    public GridNode EmptyNode { get; set; }

    public float Width;
    public float Height;

    public bool canMove = true;
    public RectTransform Canvas;

    void Start()
    {
        Canvas = InGameUI.instance.GetComponent<RectTransform>();
        Generate1();
        // ReSpawnGrid();
        // StartGrid();
        Grid = new GridNode[GridSize.x, GridSize.y];
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                Grid[x, y] = GridNodeList[(GridSize.x*x)+y];
                if (Grid[x, y].isEmpty)
                {
                    EmptyNode = Grid[x, y];
                    EmptyNode.CurrentTile.Image.enabled = false;
                }
            }
        }
        // EmptyNode = Grid[Random.Range(0, GridSize.x-1), Random.Range(0, GridSize.y-1)];
        // EmptyNode.CurrentTile.Image.enabled = false;
        // EmptyNode.CurrentTile.isEmpty = true;
        // Shuffle();
    }

    public void ReSpawnGrid()
    {
        Width = GetComponent<RectTransform>().sizeDelta.x;
        Height = GetComponent<RectTransform>().sizeDelta.y;
        Vector2 scaleMulti = Canvas.localScale;
        GridNodeList = new List<GridNode>();
        int spriteIndex = 0;

        // clear children
        for (int i = transform.childCount-1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // create children
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                int LinearPos = (GridSize.x*x)+y;
                GridNode go = Instantiate(GridNodePrefab, transform, true);
                go.LinearPos = LinearPos;
                go.name = LinearPos.ToString();
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(y*(Width/GridSize.x), x*-(Height/GridSize.y));
                go.GridPos = new Vector2Int(x, y);
                GridNodeList.Add(go);

                Tile _tile = Instantiate(TilePrefab, go.transform, true);
                _tile.Init(LinearPos, Sprites[spriteIndex]);
                _tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                _tile.GetComponent<RectTransform>().sizeDelta = new Vector2((Width/GridSize.x)*scaleMulti.x, (Height/GridSize.y)*scaleMulti.y);
                _tile.Image.sprite = Sprites[spriteIndex];
                go.CurrentTile = _tile;
                spriteIndex++;
            }
        }
    }

    public void CheckMove(GridNode _node)
    {
        if (!canMove)
        {
            return;
        }

        Vector2Int boundsTest = Vector2Int.zero;
        bool xTest, yTest;
        Tile toMove;
        Vector2Int[] arr = new Vector2Int[4];
        arr[0] = new Vector2Int(-1,0);
        arr[1] = new Vector2Int(1,0);
        arr[2] = new Vector2Int(0,-1);
        arr[3] = new Vector2Int(0,1);

        for (int i = 0; i < arr.Length; i++)
        {
            int x = arr[i].x;
            int y = arr[i].y;

            boundsTest.x = _node.GridPos.x+x;
            boundsTest.y = _node.GridPos.y+y;
            xTest = (boundsTest.x<GridSize.x && boundsTest.x>-1);
            yTest = (boundsTest.y<GridSize.y && boundsTest.y>-1);

            if (xTest && yTest && Grid[boundsTest.x, boundsTest.y] == EmptyNode)
            {
                //move
                Data.instance.CurrentLevel.noOfMoves++;
                toMove = _node.CurrentTile;
                _node.ClearChild();
                _node.SetChild(EmptyNode.CurrentTile);
                _node.CurrentTile.UpdateCurrentPos();
                EmptyNode.ClearChild();
                EmptyNode.SetChild(toMove);
                EmptyNode.CurrentTile.UpdateCurrentPos();
                EmptyNode.CurrentTile.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.3f);
                EmptyNode = _node;
                canMove = false;
                _node.CurrentTile.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.3f).OnComplete(()=>
                {
                    canMove = true;
                });
                CheckWinCondition();
                return;
            }
        }
    }

    public void CheckWinCondition()
    {
        if (LevelManager.instance.hasBegun == false)
        {
            return;
        }

        int correct = 0;
        foreach (GridNode _node in GridNodeList)
        {
            if (_node.LinearPos == _node.CurrentTile.LinearPos)
            {
                correct++;
            }
        }
        Data.instance.CurrentLevel.correctMoves = correct;

        if (correct==GridNodeList.Count)
        {
            Debug.Log("complete");
            StartCoroutine(ShowTile());
        }
    }

    public void LogTiles()
    {
        //log tiles
        Data.instance.TilesList.Clear();
        for (int i = 0; i < GridNodeList.Count; i++)
        {
            Game1CorrectTilesLog log = new Game1CorrectTilesLog();
            log.AttemptNo = Data.instance.CurrentLevel.attemptNumber;
            log.GameName = Data.instance.gameName;
            log.IdLevel = Data.instance.CurrentLevel.idLevel;
            log.IdUser = Data.instance.UID;
            log.TileNo = GridNodeList[i].CurrentTile.LinearPos;
            log.TilePosition = GridNodeList[i].CurrentTile.CurrentPos;
            log.Status = GridNodeList[i].CurrentTile.LinearPos==GridNodeList[i].CurrentTile.CurrentPos ? 1 : 0;
            Data.instance.TilesList.Add(log);
        }

        // Data.instance.PostGame1CorrectTilesLog_Co(GameManager.instance);
    }



    [ContextMenu("Create Grid")]
    public void CreateGrid()
    {
        ReSpawnGrid();
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    [ContextMenu("Start Grid")]
    public void StartGrid()
    {
        Grid = new GridNode[GridSize.x, GridSize.y];
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                Grid[x, y] = GridNodeList[(GridSize.x*x)+y];
            }
        }
        EmptyNode = Grid[Random.Range(0, GridSize.x-1), Random.Range(0, GridSize.y-1)];
        EmptyNode.CurrentTile.Image.enabled = false;
        EmptyNode.CurrentTile.isEmpty = true;
    }

    [ContextMenu("Clear Grid")]
    public void ClearChildren()
    {
        // clear children
        for (int i = transform.childCount-1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    [ContextMenu("Shuffle")]
    public void Shuffle(int _offset, int _count)
    {
        string cheat = "[";
        List<string> chearArr = new List<string>();

        for (int i = 0; i < _count; i++)
        {
            // int shuffleWith = Random.Range(i, GridNodeList.Count);
            // if (shuffleWith == i)
            // {
            //     shuffleWith = Random.Range(i, GridNodeList.Count);
            //     if (shuffleWith == i)
            //     {
            //         continue;
            //     }
            // }
            // // Debug.Log(i +""+ shuffleWith);
            // Tile temp = GridNodeList[i].CurrentTile;
            // GridNodeList[i].ClearChild();
            // GridNodeList[i].SetChild(GridNodeList[shuffleWith].CurrentTile);
            // GridNodeList[shuffleWith].ClearChild();
            // GridNodeList[shuffleWith].SetChild(temp);
            // GridNodeList[i].CurrentTile.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            // GridNodeList[shuffleWith].CurrentTile.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // List<Vector2Int> randValues = GetRandomAdjacent(GridNodeList[i].CurrentTile);
            // Vector2Int selected = randValues[Random.Range(0, randValues.Count)];
            // int shuffleWith = (GridSize.x*selected.x)+selected.y;
            // Debug.Log(GridNodeList[i]+" "+selected);
            // Tile temp = GridNodeList[i].CurrentTile;
            // GridNodeList[i].ClearChild();
            // GridNodeList[i].SetChild(GridNodeList[shuffleWith].CurrentTile);
            // GridNodeList[shuffleWith].ClearChild();
            // GridNodeList[shuffleWith].SetChild(temp);
            // GridNodeList[i].CurrentTile.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            // GridNodeList[shuffleWith].CurrentTile.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            Random.InitState(i+_offset);
            List<Vector2Int> randValues = GetRandomAdjacent(EmptyNode.CurrentTile);
            Vector2Int selected = randValues[Random.Range(0, randValues.Count)];

            int shuffleWith = (GridSize.x*selected.x)+selected.y;
            // chearArr.Add("\""+(shuffleWith.ToString()+"->"+EmptyNode.LinearPos.ToString())+"\"");
            chearArr.Add("\""+(EmptyNode.LinearPos+1).ToString()+"\"");

            Tile temp = EmptyNode.CurrentTile;
            EmptyNode.ClearChild();
            EmptyNode.SetChild(GridNodeList[shuffleWith].CurrentTile);
            GridNodeList[shuffleWith].ClearChild();
            GridNodeList[shuffleWith].SetChild(temp);
            EmptyNode.CurrentTile.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            GridNodeList[shuffleWith].CurrentTile.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            EmptyNode = GridNodeList[shuffleWith];
        }
        
        chearArr.Reverse();
        for (int i = 0; i < chearArr.Count; i++)
        {
            if (i==chearArr.Count-1)
            {
                cheat += chearArr[i];
            }
            else
            {
                cheat += chearArr[i]+", ";
            }
        }
        cheat += "]";
        #if UNITY_EDITOR
            Debug.Log(cheat);
        #endif
    }

    [ContextMenu("Generate 1")]
    public void Generate1()
    {
        CreateGrid();
        StartGrid();
        Shuffle(1+(int)(System.DateTime.Now.ToFileTime()), 100);
    } 

    [ContextMenu("Generate 2")]
    public void Generate2()
    {
        CreateGrid();
        StartGrid();
        Shuffle(8+(int)(System.DateTime.Now.ToFileTime()), 100);
    }  

    List<Vector2Int> GetRandomAdjacent(Tile _center)
    {
        Vector2Int boundsTest = Vector2Int.zero;
        bool xTest, yTest;
        Vector2Int[] arr = new Vector2Int[4];
        arr[0] = new Vector2Int(-1,0);
        arr[1] = new Vector2Int(1,0);
        arr[2] = new Vector2Int(0,-1);
        arr[3] = new Vector2Int(0,1);

        List<Vector2Int> rand = new List<Vector2Int>();

        for (int i = 0; i < arr.Length; i++)
        {
            int x = arr[i].x;
            int y = arr[i].y;

            boundsTest.x = _center.CurrentGridPos.x+x;
            boundsTest.y = _center.CurrentGridPos.y+y;
            xTest = (boundsTest.x<GridSize.x && boundsTest.x>-1);
            yTest = (boundsTest.y<GridSize.y && boundsTest.y>-1);

            if (xTest && yTest)
            {
                rand.Add(boundsTest);
            }
        }

        return rand;
    }

    IEnumerator ShowTile()
    {
        LevelManager.instance.hasBegun = false;
        EmptyNode.CurrentTile.Image.enabled = true;
        Color ogColor = EmptyNode.CurrentTile.Image.color;
        EmptyNode.CurrentTile.Image.color = new Color(ogColor.r, ogColor.g, ogColor.b, 0f);
        EmptyNode.CurrentTile.Image.DOColor(ogColor, 1f);
        yield return new WaitForSeconds(2f);
        
        LogTiles();
        LevelManager.instance.SetLevelCompleted();
        InGameUI.instance.ShowWinPanel();
    }

    void OnDisable() 
    {
        InGameUI.instance.AttemptNo_txt.gameObject.SetActive(false);
        InGameUI.instance.Timer_txt.gameObject.SetActive(false);
        InGameUI.instance.TimerUI.gameObject.SetActive(false);
    }
}
