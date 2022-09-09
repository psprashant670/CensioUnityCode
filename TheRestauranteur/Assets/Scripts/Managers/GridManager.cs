using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    public Data Data;
    public Vector2Int GridSize;
    public Vector2 TileOffset;
    public Transform Root;
    public TileHolder TileHolderPrefab;
    public TileHolder[,] Grid;
    public List<Tile> GridTileList;
    public TilePoolManager TilePoolManager;

    LayerMask TileLayer;

    [SerializeField]
    List<Match> Match;
    Vector2Int[] boundsDelta = new Vector2Int[4];

    //swipe
    Tile Selected;
    Vector2 StartPos;
    float lastTouchTime = 0;
    public bool isSwapping = false;

    void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        TileLayer = LayerMask.GetMask("Tile");

        Match = new List<Match>();
        boundsDelta[0] = new Vector2Int(-1,0);
        boundsDelta[1] = new Vector2Int(1,0);
        boundsDelta[2] = new Vector2Int(0,-1);
        boundsDelta[3] = new Vector2Int(0,1);
    }

    public void LoadBoard()
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        // TilePoolManager.SpawnActive(GridSize.x * GridSize.y);
        TilePoolManager.LoadBoardFromString();

        InitGrid();
        InitBoard();
        Match = FindMatch(Grid);

        stopWatch.Stop();
        System.TimeSpan ts = stopWatch.Elapsed;
        // Debug.Log("Board generated in " + ts.Milliseconds+" miliseconds");

        LogBoard();
        LogState();

        LevelManager.instance.isReady = true;
    }

    public void ClearBoard()
    {
        LevelManager.instance.isReady = false;
        foreach (Tile _tile in TilePoolManager.ActiveTiles)
        {
            Destroy(_tile.gameObject);
        }
        foreach (Tile _tile in TilePoolManager.Pool)
        {
            Destroy(_tile.gameObject);
        }
        TilePoolManager.ActiveTiles.Clear();
        TilePoolManager.Pool.Clear();
    }

    public void InitGrid()
    {
        Grid = new TileHolder[GridSize.x, GridSize.y];

        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                Grid[x, y] = Instantiate(TileHolderPrefab, Root);
                Grid[x, y].transform.parent = Root;
                Grid[x, y].transform.localPosition = new Vector2(TileOffset.x*x, TileOffset.y*y);
                Grid[x, y].Init(new Vector2Int(x, y), (GridSize.x*y)+x);
            }
        }
    }

    public void InitBoard()
    {
        GridTileList = new List<Tile>();

        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                Tile _tile = TilePoolManager.ActiveTiles[((GridSize.x*y)+x)];
                _tile.Respawn();
                Grid[x, y].SetTile(_tile, false);
                GridTileList.Add(_tile);
            }
        }
    }

    public void LogBoard()
    {
        string log = "";

        for (int y = GridSize.y-1; y >= 0; y--)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                log += (int)Grid[x, y].CurrentTile.Type+" ";
            }
            log += "\n";
        }

        Data.instance.CurrentBoard = log;

        if (GameManager.instance.isDebugBuild)
        {
            int poolCountEmpty = TilePoolManager.ActiveTiles.FindAll((pool) => pool.Type == TileType.Empty).Count;
            int poolCountWheat = TilePoolManager.ActiveTiles.FindAll((pool) => pool.Type == TileType.Wheat).Count;
            int poolCountGrape = TilePoolManager.ActiveTiles.FindAll((pool) => pool.Type == TileType.Grape).Count;
            int poolCountChocolate = TilePoolManager.ActiveTiles.FindAll((pool) => pool.Type == TileType.Chocolate).Count;
            int poolCountMilk = TilePoolManager.ActiveTiles.FindAll((pool) => pool.Type == TileType.Milk).Count;
            Debug.Log("Tile mapping : 0 = Empty tile,   1 = Wheat,   2 = Grape,   3 = Chocolate,   4 = Milk");
            Debug.Log("Starting board : \n"+log);
            Debug.Log("Number of active tiles on board : Empty="+poolCountEmpty+"  Wheat="+poolCountWheat+"  Grape="+poolCountGrape+"  Chocolate="+poolCountChocolate+"  Milk="+poolCountMilk);
        }
    }

    // ------------------------------------------------------------ Swipe ------------------------------------------------------------
    public void ClickStart(Vector2 _wordPos)
    {
        StartPos = _wordPos;
        lastTouchTime = Time.time;

        RaycastHit2D ray = Physics2D.Raycast(_wordPos, Vector2.zero, 1f, TileLayer);
        if (ray.collider != null)
        {
            Selected = ray.collider.GetComponent<Tile>();
        }
    }

    public void ClickEnd(Vector2 _wordPos)
    {
        Vector2 dir = StartPos - _wordPos;
        // Debug.Log(dir.normalized);
        // Debug.Log(Time.time - lastTouchTime);
        // Debug.Log(Vector2.Distance(StartPos, _wordPos));
        if (Selected != null && Vector2.Distance(StartPos, _wordPos)>0.42f)
        {
            if(dir.normalized.y > 0 && dir.normalized.x > -0.5f && dir.normalized.x < 0.5f)
            {
                // Debug.Log("down");
                TrySwap(Selected, Selected.GridPos + new Vector2Int(0, -1));
            }
            if(dir.normalized.y < 0 && dir.normalized.x > -0.5f && dir.normalized.x < 0.5f)
            {
                // Debug.Log("up");
                TrySwap(Selected, Selected.GridPos + new Vector2Int(0, 1));
            }
            if(dir.normalized.x < 0 && dir.normalized.y > -0.5f && dir.normalized.y < 0.5f)
            {
                // Debug.Log("right");
                TrySwap(Selected, Selected.GridPos + new Vector2Int(1, 0));
            }
            if(dir.normalized.x > 0 && dir.normalized.y > -0.5f && dir.normalized.y < 0.5f)
            {
                // Debug.Log("left");
                TrySwap(Selected, Selected.GridPos + new Vector2Int(-1, 0));
            }

            // Selected.IncrementSelected();
            Selected = null;
        }
    }

    void TrySwap(Tile _tile, Vector2Int _targetPos)
    {
        // check if the target direction is within bounds
        if (_targetPos.x < GridSize.x && _targetPos.x >= 0 && _targetPos.y < GridSize.y && _targetPos.y >= 0 && !isSwapping) 
        {
            isSwapping = true;
            // _tile.LogSelected();
            LevelManager.instance.LastSelected = _tile;
            StartCoroutine(SwapTiles(_tile, Grid[_targetPos.x, _targetPos.y].CurrentTile));
        }
    }

    List<Match> FindMatch(TileHolder[,] _grid)
    {
        int matchCount = 0;
        //creating an arraylist to store the matching tiles
        Match.Clear();
        Tile prevTile = null;

        // horizontal
        for (int y = 0; y < GridSize.y; y++)
        {
            prevTile = Grid[0, y].CurrentTile;
            matchCount = 1;

            for (int x = 1; x <= GridSize.x; x++)
            {
                if (x == GridSize.x)
                {
                    if (matchCount>2)
                    {
                        Match found = new Match();
                        found.Matches = new List<Tile>();
                        while (matchCount>0)
                        {
                            found.Matches.Add(Grid[x-(matchCount), y].CurrentTile);
                            matchCount--;
                        }
                        Match.Add(found);
                    }
                    continue;
                }

                if (prevTile.Type == Grid[x, y].CurrentTile.Type)
                {
                    matchCount++;
                }
                else
                {
                    if (matchCount>2)
                    {
                        Match found = new Match();
                        found.Matches = new List<Tile>();
                        while (matchCount>0)
                        {
                            found.Matches.Add(Grid[x-(matchCount), y].CurrentTile);
                            matchCount--;
                        }
                        Match.Add(found);
                    }
                    matchCount = 1;
                }
                prevTile = Grid[x, y].CurrentTile;
            }
        }

        // vertical
        for (int x = 0; x < GridSize.x; x++)
        {
            prevTile = Grid[x, 0].CurrentTile;
            matchCount = 1;

            for (int y = 1; y <= GridSize.y; y++)
            {
                if (y == GridSize.y)
                {
                    if (matchCount>2)
                    {
                        Match found = new Match();
                        found.Matches = new List<Tile>();
                        while (matchCount>0)
                        {
                            found.Matches.Add(Grid[x, y-(matchCount)].CurrentTile);
                            matchCount--;
                        }
                        Match.Add(found);
                    }
                    continue;
                }

                if (prevTile.Type == Grid[x, y].CurrentTile.Type)
                {
                    matchCount++;
                }
                else
                {
                    if (matchCount>2)
                    {
                        Match found = new Match();
                        found.Matches = new List<Tile>();
                        while (matchCount>0)
                        {
                            found.Matches.Add(Grid[x, y-(matchCount)].CurrentTile);
                            matchCount--;
                        }
                        Match.Add(found);
                    }
                    matchCount = 1;
                }
                prevTile = Grid[x, y].CurrentTile;
            }
        }

        return Match;
    }

    void ReshuffleSwap(Tile _toSwap)
    {
        int count = GridTileList.Count;
        List<Vector2Int> validBounds = GetValidBounds(_toSwap);
        // TileType safeType = 

        while (count >= 0)
        {
            if (GridTileList[count].Type != _toSwap.Type)
            {
                
            }

            count--;
        }
    }

    List<Vector2Int> GetValidBounds(Tile _center)
    {
        Vector2Int boundsTest = Vector2Int.zero;
        bool xTest, yTest;

        List<Vector2Int> validBounds = new List<Vector2Int>();

        for (int i = 0; i < boundsDelta.Length; i++)
        {
            int x = boundsDelta[i].x;
            int y = boundsDelta[i].y;

            boundsTest.x = _center.GridPos.x+x;
            boundsTest.y = _center.GridPos.y+y;
            xTest = (boundsTest.x<GridSize.x && boundsTest.x>-1);
            yTest = (boundsTest.y<GridSize.y && boundsTest.y>-1);

            if (xTest && yTest)
            {
                validBounds.Add(boundsTest);
            }
        }

        return validBounds;
    }

    void MoveTilesDown()
    {
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y-1; y++)
            {
                if (Grid[x, y].CurrentTile == null)
                {
                    for (int i = y+1; i < GridSize.y; i++)
                    {
                        if (Grid[x, i].CurrentTile != null)
                        {
                            Tile _toMove = Grid[x, i].CurrentTile;
                            Grid[x, i].RemoveTile();
                            Grid[x, y].SetTile(_toMove, true);
                            _toMove.transform.DOLocalMove(Vector3.zero, 0.2f);
                            break;
                        }
                    }
                }
            }
        }
    }
    
    void LogState()
    {
        if (!GameManager.instance.isDebugBuild)
        {
            return;
        }
        int poolCountEmpty = TilePoolManager.Pool.FindAll((pool) => pool.Type == TileType.Empty).Count;
        int poolCountWheat = TilePoolManager.Pool.FindAll((pool) => pool.Type == TileType.Wheat).Count;
        int poolCountGrape = TilePoolManager.Pool.FindAll((pool) => pool.Type == TileType.Grape).Count;
        int poolCountChocolate = TilePoolManager.Pool.FindAll((pool) => pool.Type == TileType.Chocolate).Count;
        int poolCountMilk = TilePoolManager.Pool.FindAll((pool) => pool.Type == TileType.Milk).Count;
        string log = "No of Matches found = "+Match.Count+"\n";
        log += "Tiles Matched : Wheat="+LevelManager.instance.Wheat+" Grape="+LevelManager.instance.Grape+" Chocolate="+LevelManager.instance.Chocolate+" Milk="+LevelManager.instance.Milk;
        log += "\n";
        log += "Inactive tiles that can be spawnned in : Empty="+poolCountEmpty+" Wheat="+poolCountWheat+" Grape="+poolCountGrape+" Chocolate="+poolCountChocolate+" Milk="+poolCountMilk;
        Debug.Log(log);
    }

    IEnumerator SwapTiles(Tile _source, Tile _target)
    {
        TileHolder holderSource = Grid[_source.GridPos.x, _source.GridPos.y];
        TileHolder holderTarget = Grid[_target.GridPos.x, _target.GridPos.y];
        holderSource.SetTile(_target, true);
        holderTarget.SetTile(_source, true);
        yield return null;

        _source.transform.DOLocalMove(Vector3.zero, 0.3f);
        _target.transform.DOLocalMove(Vector3.zero, 0.3f);
        yield return new WaitForSeconds(0.6f);

        List<Match> _matches = FindMatch(Grid);
        if (_matches.Count <= 0)
        {
            holderSource.SetTile(_source, true);
            holderTarget.SetTile(_target, true);
            _source.transform.DOLocalMove(Vector3.zero, 0.3f);
            _target.transform.DOLocalMove(Vector3.zero, 0.3f);
            yield return new WaitForSeconds(0.6f);

            isSwapping = false;
        }
        else
        {
            // while (Match.Count > 0)
            // {
                // LogState();
                TilePoolManager.Requeue(Match);
                TilePoolManager.ShufflePool();
                yield return new WaitForSeconds(0.08f);

                MoveTilesDown();
                yield return new WaitForSeconds(0.42f);

                for (int y = 0; y < GridSize.y; y++)
                {
                    for (int x = 0; x < GridSize.x; x++)
                    {
                        if (Grid[x, y].CurrentTile == null)
                        {
                            Tile _tile = TilePoolManager.Pool[0];
                            TilePoolManager.Pool.RemoveAt(0);
                            TilePoolManager.ActiveTiles.Add(_tile);
                            Grid[x, y].SetTile(_tile, false);
                            GridTileList.Add(_tile);
                            _tile.transform.localPosition = new Vector3(0, 0.8f*GridSize.y, 0);
                            _tile.Respawn();
                            _tile.transform.DOLocalMove(Vector3.zero, 0.2f);
                        }
                    }
                }
                LogState();
                LevelManager.instance.CheckFestivalData();
                yield return new WaitForSeconds(0.42f);
                _matches = FindMatch(Grid);
            // }

            isSwapping = false;
        }
    }
}

[System.Serializable]
public class TilePoolManager
{
    public List<Tile> ActiveTiles, Pool = new List<Tile>();
    public List<PoolData> PoolDatas; // prefab data only
    public Dictionary<TileType, List<Tile>> TilesDictionary = new Dictionary<TileType, List<Tile>>();

    public void SpawnActive(int _gridSize)
    {
        // spawn passive pool first
        foreach (PoolData _poolData in PoolDatas)
        {
            // List<Tile> list = TilesDictionary.GetValueOrDefault(_poolData.Type);
            // list = new List<Tile>();
            for (int i = 0; i < _poolData.Count; i++)
            {
                Tile tile = GameObject.Instantiate(_poolData.Prefab);
                Pool.Add(tile);
                tile.Despawn();
                // list.Add(tile);
            }
        }

        // check if pool has enough tiles to spawn in
        if (Pool.Count < _gridSize)
        {
            Debug.LogError("Not Enough Tiles!");
            return;
        }

        // shuffle and populate active array before spawnning
        ShufflePool();
        for (int i = _gridSize-1; i >= 0; i--)
        {
            ActiveTiles.Add(Pool[i]);
            Pool.RemoveAt(i);
        }
    }

    public void LoadBoardFromString()
    {
        BoardData boardData = Data.instance.GetBoardData(LevelManager.instance.CurrentFestival);
        PoolDatas.Find((pool)=> pool.Type == TileType.Empty).Count = boardData.EmptyCount;
        PoolDatas.Find((pool)=> pool.Type == TileType.Empty).ExtraCount = boardData.PoolEmptyCount;
        PoolDatas.Find((pool)=> pool.Type == TileType.Wheat).Count = boardData.WheatCount;
        PoolDatas.Find((pool)=> pool.Type == TileType.Wheat).ExtraCount = boardData.PoolWheatCount;
        PoolDatas.Find((pool)=> pool.Type == TileType.Grape).Count = boardData.GrapeCount;
        PoolDatas.Find((pool)=> pool.Type == TileType.Grape).ExtraCount = boardData.PoolGrapeCount;
        PoolDatas.Find((pool)=> pool.Type == TileType.Chocolate).Count = boardData.ChocolateCount;
        PoolDatas.Find((pool)=> pool.Type == TileType.Chocolate).ExtraCount = boardData.PoolChocolateCount;
        PoolDatas.Find((pool)=> pool.Type == TileType.Milk).Count = boardData.MilkCount;
        PoolDatas.Find((pool)=> pool.Type == TileType.Milk).ExtraCount = boardData.PoolMilkCount;
        
        foreach (PoolData _poolData in PoolDatas)
        {
            // List<Tile> list = new List<Tile>();
            // TilesDictionary.TryAdd(_poolData.Type, list);
            for (int i = 0; i < _poolData.ExtraCount; i++)
            {
                Tile tile = GameObject.Instantiate(_poolData.Prefab);
                Pool.Add(tile);
                tile.Despawn();
            }
        }

        string[] rows = boardData.BoardLayout.Split(new string[] {"\r\n", "\n"}, System.StringSplitOptions.RemoveEmptyEntries);

        for (int r = rows.Length-1; r >= 0; r--)
        {
            string[] row = rows[r].Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < row.Length; i++)
            {
                // Debug.Log(row[i]);
                TileType _type = (TileType) int.Parse(row[i].Trim());
                Tile tile = GameObject.Instantiate(PoolDatas.Find((_pool)=>_pool.Type == _type).Prefab);
                ActiveTiles.Add(tile);
                // TilesDictionary.GetValueOrDefault(_type).Add(tile);
            }
        }
    }

    public void ShuffleActive()
    {
        var count = ActiveTiles.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ActiveTiles[i];
            ActiveTiles[i] = ActiveTiles[r];
            ActiveTiles[r] = tmp;
        }
    }

    public void ShufflePool()
    {
        var count = Pool.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = Pool[i];
            Pool[i] = Pool[r];
            Pool[r] = tmp;
        }
    }

    public void Requeue(List<Match> _matches)
    {
        for (int i = 0; i < _matches.Count; i++)
        {
            for (int t = 0; t < _matches[i].Matches.Count; t++)
            {
                Tile _tile = _matches[i].Matches[t];
                if (!Pool.Contains(_tile) && ActiveTiles.Contains(_tile))
                {
                    Pool.Add(_tile);
                    ActiveTiles.Remove(_tile);
                    GridManager.instance.Grid[_tile.GridPos.x, _tile.GridPos.y].RemoveTile();
                    GridManager.instance.GridTileList.Remove(_tile);
                    _tile.Despawn();
                }
            }
        }
    }
}

[System.Serializable]
public class PoolData
{
    public TileType Type;
    public Tile Prefab;
    public int Count, ExtraCount;
}

[System.Serializable]
public class Match
{
    public List<Tile> Matches;
}

public enum TileType
{
    Empty,
    Wheat,
    Grape,
    Chocolate,
    Milk
}
