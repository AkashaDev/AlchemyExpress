using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cauldron Template", menuName = "Potion/Cauldron Template")]
public class CauldronTemplateSO : ScriptableObject
{
    public int width;
    public int height;
    public List<Vector2Int> blockedPositions = new List<Vector2Int>();


    public bool IsBlocked(int x, int y)
    {
        return blockedPositions.Contains(new Vector2Int(x, y));
    }
}
