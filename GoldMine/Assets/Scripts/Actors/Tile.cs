using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Image Image;
    public int LinearPos, CurrentPos;
    public Vector2Int CurrentGridPos { get { return GetComponentInParent<GridNode>().GridPos; } }
    public bool isEmpty = false;

    void Start()
    {
        if (transform.GetComponentInParent<GridNode>()==null)
        {
            Destroy(gameObject);
        }
        CurrentPos = transform.GetComponentInParent<GridNode>().LinearPos;
    }

    public void Init(int _linearPos, Sprite _sprite)
    {
        LinearPos = _linearPos;
        Image.sprite = _sprite;
    }

    public void UpdateCurrentPos()
    {
        CurrentPos = transform.GetComponentInParent<GridNode>().LinearPos;
    }

    public void OnClicked()
    {
        if (isEmpty)
        {
            return;
        }
        // Debug.Log(CurrentPos);
        // Debug.Log(CurrentGridPos);
        LevelManager.instance.GridManager.CheckMove(transform.GetComponentInParent<GridNode>());
    }
}
