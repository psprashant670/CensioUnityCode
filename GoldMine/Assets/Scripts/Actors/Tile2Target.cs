using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile2Target : MonoBehaviour
{
    public Tile2 Parent;


    void Start()
    {
        Parent = GetComponentInParent<Tile2>();
        Parent.isTarget = true;
    }

}
