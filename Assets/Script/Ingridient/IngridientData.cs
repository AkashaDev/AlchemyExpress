using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIngridient", menuName = "Alchemy/Ingridient")]
public class IngridientData : ScriptableObject
{
    [Header("Basic Info")]
    public string ingridientName;
    public Sprite icon;
    public Color displayColor;

    [Header("Visuals / Sprite")]
    public List<Sprite> tilesSprite;

    [Header("Shapes")]
    public List<ShapeVariant> shapeVariants;
}
[System.Serializable]
public class ShapeVariant 
{
    public List<Vector2Int> tilePositions;
    public bool canRotate = true;
}