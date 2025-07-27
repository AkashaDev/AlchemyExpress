using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientObject : MonoBehaviour
{
    public IngridientData data;
    public int variantIndex = 0;

    private List<GameObject> tiles = new List<GameObject>();
    private GameObject tilePrefab;

    public void Init(IngridientData data,GameObject tilePrefab) 
    {
        this.data = data;
        this.tilePrefab = tilePrefab;
        GenerateTiles();

    }

    public void Rotate() 
    {
        if(data.shapeVariants.Count <= 1) 
            return;
        variantIndex = (variantIndex + 1) % data.shapeVariants.Count;
        GenerateTiles();
    }
    private void GenerateTiles() 
    {
        foreach (var t in tiles)
            Destroy(t);
        tiles.Clear();

        var shape = data.shapeVariants[variantIndex];
        for (int i = 0; i < shape.tilePositions.Count; i++)
        {
            var pos = shape.tilePositions[i];
            var tile = Instantiate(tilePrefab, this.transform);
            tile.transform.localPosition = new Vector3(pos.x, pos.y, 0);

            var renderer = tile.GetComponent<SpriteRenderer>();

            Color visibleColor = data.displayColor;
            visibleColor.a = 1f;
            renderer.color = visibleColor;

            if (data.tilesSprite != null && i < data.tilesSprite.Count)
            {
                renderer.sprite = data.tilesSprite[i];
            }

            tiles.Add(tile);
        }
        UpdateColliderToMatchTiles();
    }
    private void UpdateColliderToMatchTiles()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();

        if (tiles.Count == 0)
            return;

        Bounds totalBounds = tiles[0].GetComponent<SpriteRenderer>().bounds;

        foreach (var tile in tiles)
        {
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                totalBounds.Encapsulate(sr.bounds);
            }
        }

        Vector3 center = transform.InverseTransformPoint(totalBounds.center);
        Vector3 size = transform.InverseTransformVector(totalBounds.size);

        collider.offset = center;
        collider.size = size;
    }
}
