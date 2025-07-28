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

    public Vector2Int[] GetRotatedShape(int rotation, Vector2Int pivot)
    {
        Vector2Int[] rotated = new Vector2Int[shapeCells.Length];

        for (int i = 0; i < shapeCells.Length; i++)
        {
            Vector2Int c = shapeCells[i];

            // Translasi ke origin (relative to pivot)
            int relX = c.x - pivot.x;
            int relY = c.y - pivot.y;

            // Rotasi searah jarum jam 90°
            Vector2Int rotatedRel;
            switch (rotation % 4)
            {
                case 0: rotatedRel = new Vector2Int(relX, relY); break;
                case 1: rotatedRel = new Vector2Int(-relY, relX); break;
                case 2: rotatedRel = new Vector2Int(-relX, -relY); break;
                case 3: rotatedRel = new Vector2Int(relY, -relX); break;
                default: rotatedRel = new Vector2Int(relX, relY); break;
            }

            // Translasi balik ke posisi pivot
            rotated[i] = pivot + rotatedRel;
        }

        return rotated;
    }
}
