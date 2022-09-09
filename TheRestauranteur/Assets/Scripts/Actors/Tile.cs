using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Tile : MonoBehaviour
{
    public TileType Type;
    public Vector2Int GridPos;
    public List<Sprite> TileVariations;
    public SpriteRenderer SpriteRenderer;

    public AudioInstance AudioInstance;

    Data Data;
    MaterialLog logCache;
    int counter = 0;

    void Awake() 
    {
        SpriteRenderer.sprite = TileVariations[Random.Range(0, TileVariations.Count)];
        Data = Data.instance;
    }

    void IncrementPresented()
    {
        switch (Type)
        {
            case TileType.Wheat:
            LevelManager.instance.festivalData.WheatPresented++;
            break;

            case TileType.Grape:
            LevelManager.instance.festivalData.GrapePresented++;
            break;

            case TileType.Chocolate:
            LevelManager.instance.festivalData.ChocolatePresented++;
            break;

            case TileType.Milk:
            LevelManager.instance.festivalData.MilkPresented++;
            break;
        }
    }

    public void IncrementSelected()
    {
        switch (Type)
        {
            case TileType.Wheat:
            LevelManager.instance.festivalData.WheatSelected++;
            break;

            case TileType.Grape:
            LevelManager.instance.festivalData.GrapeSelected++;
            break;

            case TileType.Chocolate:
            LevelManager.instance.festivalData.ChocolateSelected++;
            break;

            case TileType.Milk:
            LevelManager.instance.festivalData.MilkSelected++;
            break;
        }
    }

    public void Respawn()
    {
        LogTile();
        IncrementPresented();
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
        if (LevelManager.instance.isReady)
        {
            IncrementSelected();
            LogSelected();
            LeanPool.Spawn(AudioInstance);
            UpdateBasket();
        }
    }

    public void LogTile()
    {
        counter = Data.MaterialLogs.Count+1;
        logCache = new MaterialLog();
        logCache.IdUser = Data.UID;
        logCache.LifeNo = (int)LevelManager.instance.CurrentFestival;
        logCache.RawMaterialTileNo = counter;
        logCache.FestivalName = LevelManager.instance.festivalData.festivalName;
        logCache.RawMaterial = Type.ToString();
        logCache.RawMaterialSelected = "N"; // Y or N
        if (Type == LevelManager.instance.CurrentFestival)
        {
            logCache.RawMaterialType = "C";
        }
        else if ((int)Type > (int)LevelManager.instance.CurrentFestival)
        {
            logCache.RawMaterialType = "F";
        }
        else
        {
            logCache.RawMaterialType = "P";
        }
        Data.MaterialLogs.Add(logCache);
    }

    public void LogSelected()
    {
        logCache.RawMaterialSelected = "Y"; // Y or N
    }

    void UpdateBasket()
    {
        switch (Type)
        {
            case TileType.Wheat:
            LevelManager.instance.Wheat++;
            break;

            case TileType.Grape:
            LevelManager.instance.Grape++;
            break;

            case TileType.Chocolate:
            LevelManager.instance.Chocolate++;
            break;

            case TileType.Milk:
            LevelManager.instance.Milk++;
            break;

            default:
            break;
        }

        LevelManager.instance.UpdateBaskets();
    }
}
