using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObeserverPattern;

public class CauldronSpawner : MonoBehaviour
{
    [Header("Template & Visual")]
    public CauldronTemplateSO template;
    public GameObject tilePrefab;
    public Transform tileParent;

    [Header("Logic Grid")]
    public CauldronGridBehavior gridBehavior;
    public SpriteRenderer[,] tileRenderers;
    private CauldronTemplateSO currentTemplate;

    private void OnEnable()
    {
        EventManager.Subscribe<QuestAboutToStartEvent>(HandleQuestStart);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<QuestAboutToStartEvent>(HandleQuestStart);
    }

    void Start()
    {
        gridBehavior.SetTileRenderers(tileRenderers);
    }

    private void HandleQuestStart(QuestAboutToStartEvent e)
    {
        if (e.questData == null || e.questData.cauldronTemplate == null)
        {
            Debug.LogError("QuestData atau CauldronTemplate tidak ada!");
            return;
        }

        RebuildGrid(e.questData.cauldronTemplate);
    }

    public void RebuildGrid(CauldronTemplateSO newTemplate)
    {
        if (currentTemplate == newTemplate) return;
        
        currentTemplate = newTemplate;
        
        foreach (Transform child in tileParent)
        {
            Destroy(child.gameObject);
        }

        InitializeGridLogic(newTemplate);
        BuildGridVisual(newTemplate);
        gridBehavior.SetTileRenderers(tileRenderers);
    }

    void BuildGridVisual(CauldronTemplateSO template)
    {
        tileRenderers = new SpriteRenderer[template.width, template.height];
        float xOffset = (template.width - 1) * 0.5f;
        float yOffset = (template.height - 1) * 0.5f;

        for (int y = 0; y < template.height; y++)
        {
            for (int x = 0; x < template.width; x++)
            {
                bool isBlocked = template.blockedPositions.Contains(new Vector2Int(x - (template.width / 2), y - (template.height / 2)));

                if (isBlocked) continue;

                float worldX = x - xOffset;
                float worldY = y - yOffset;
                Vector3 worldPos = tileParent.position + new Vector3(worldX, worldY, 0);
                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, tileParent);
                tileRenderers[x, y] = tile.GetComponent<SpriteRenderer>();
            }
        }
    }

    void InitializeGridLogic(CauldronTemplateSO template)
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
