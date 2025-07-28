using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronRuntimeData
{
    public int width;
    public int height;
    public int[] tiles;

    public CauldronRuntimeData(CauldronSO source)
    {
        width = source.width;
        height = source.height;
        tiles = (int[])source.tiles.Clone();
    }

    public bool IsBlocked(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return true;
        return tiles[y * width + x] == 1;
    }
}
