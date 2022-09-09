using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tile2 : MonoBehaviour
{
    public int LinearPos;
    public Vector2Int GridPos;
    public bool hasObstacle = false, isValid = false, isTarget = false;
    public GameObject Obstacle, ValidIndicator;
    public Vector2Int TargetDir;

    void Start()
    {

    }

    void Update()
    {

    }

    public void SetValid(bool _state)
    {
        isValid = _state;
        ValidIndicator.SetActive(_state);
    }

    public void Touch()
    {

    }

    private void OnEnable() {
        
    }

    private void OnDisable() {
        
    }
}
