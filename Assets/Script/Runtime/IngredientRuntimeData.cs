using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientRuntimeData
{
    public Vector2Int[] shapeCells;
    public bool canRotate;

    public IngredientRuntimeData(IngredientSO source)
    {
        shapeCells = (Vector2Int[])source.shapeCells.Clone();
        canRotate = source.canRotate;
    }

    public Vector2Int[] GetRotatedShape(int rotation)
    {
        Vector2Int[] rotated = new Vector2Int[shapeCells.Length];

        for (int i = 0; i < shapeCells.Length; i++)
        {
            Vector2Int c = shapeCells[i];
            switch (rotation % 4)
            {
                case 0: rotated[i] = c; break;
                case 1: rotated[i] = new Vector2Int(-c.y, c.x); break;
                case 2: rotated[i] = new Vector2Int(-c.x, -c.y); break;
                case 3: rotated[i] = new Vector2Int(c.y, -c.x); break;
            }
        }

        return rotated;
    }
}
