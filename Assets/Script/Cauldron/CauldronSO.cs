using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Potion/Cauldron")]
public class CauldronSO : ScriptableObject
{
    public int width;
    public int height;
    public int[] tiles;

    void Awake()
    {
        tiles = new int[width * height];
    }
}
