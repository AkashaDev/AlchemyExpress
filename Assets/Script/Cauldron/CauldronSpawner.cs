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
        BuildGridVisual();
        InitializeGridLogic();
    }

    void BuildGridVisual()
    {
        tileRenderers = new SpriteRenderer[template.width, template.height];

        for (int y = 0; y < template.height; y++)
        {
            for (int x = 0; x < template.width; x++)
            {
                Vector3 worldPos = tileParent.position + new Vector3(x, y, 0);
                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, tileParent);

                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                tileRenderers[x, y] = sr;

                if (template.IsBlocked(x, y))
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
        gridBehavior.SetTileRenderers(tileRenderers);
        gridBehavior.Initialize(template.width, template.height, tileParent.position, template.blockedTiles);
    }
}
