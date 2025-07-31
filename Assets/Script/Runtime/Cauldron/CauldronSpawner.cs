using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronSpawner : MonoBehaviour
{
    [Header("Template & Visual")]
    public CauldronTemplateSO template;
    public GameObject tilePrefab;
    public Transform tileParent;

    [Header("Logic Grid")]
    public CauldronGridBehavior gridBehavior;
    public SpriteRenderer[,] tileRenderers;

    void Start()
    {
        InitializeGridLogic();
        BuildGridVisual();
        gridBehavior.SetTileRenderers(tileRenderers);
    }

    void BuildGridVisual()
    {
        tileRenderers = new SpriteRenderer[template.width, template.height];

        // Perubahan: Gunakan offset floating point untuk semua ukuran grid
        float xOffset = (template.width - 1) * 0.5f;
        float yOffset = (template.height - 1) * 0.5f;

        for (int y = 0; y < template.height; y++)
        {
            for (int x = 0; x < template.width; x++)
            {
                // Perubahan: Hitung posisi grid relatif terhadap pusat
                float gridX = x - xOffset;
                float gridY = y - yOffset;

                Vector3 worldPos = tileParent.position + new Vector3(gridX, gridY, 0);
                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, tileParent);

                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                tileRenderers[x, y] = sr;

                if (template.IsBlocked(Mathf.RoundToInt(gridX), Mathf.RoundToInt(gridY)))
                {
                    if (sr != null)
                        sr.color = Color.black;
                }
            }
        }
    }

    void InitializeGridLogic()
    {
        if (gridBehavior == null)
        {
            Debug.LogError("[CauldronSpawner] GridBehavior is missing!");
            return;
        }

        bool[] blockedTileArray = new bool[template.width * template.height];
        int xOffset = template.width / 2;
        int yOffset = template.height / 2;

        foreach (var pos in template.blockedPositions)
        {
            int arrayX = pos.x + xOffset;
            int arrayY = pos.y + yOffset;

            if (arrayX >= 0 && arrayX < template.width && arrayY >= 0 && arrayY < template.height)
            {
                blockedTileArray[arrayY * template.width + arrayX] = true;
            }
            else
            {
                Debug.LogWarning($"Blocked tile position {pos} is out of bounds in template.");
            }
        }

        gridBehavior.Initialize(template.width, template.height, tileParent.position, blockedTileArray);
    }
}
