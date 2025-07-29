using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cauldron Template", menuName = "Potion/Cauldron Template")]
public class CauldronTemplateSO : ScriptableObject
{
    public int width;
    public int height;
    public bool[] blockedTiles;

    public bool IsBlocked(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return true;
        return blockedTiles[y * width + x];
    }
}
