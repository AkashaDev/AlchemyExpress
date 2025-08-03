using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashAreaGrid : MonoBehaviour
{

    [SerializeField] private BoxCollider2D trashCollider;
    [SerializeField] private GameObject trashClosedVisual;
    [SerializeField] private GameObject trashOpenVisual;
    private bool isHovered = false;

    public bool IsInsideTrashArea(Vector3 worldPos)
    {
        if (trashCollider == null)
        {
            Debug.LogError("Trash Collider belum diset!");
            return false;
        }

        return trashCollider.OverlapPoint(worldPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IngredientInstance>() != null)
        {
            Debug.Log("Bahan di atas trash area");
        }
    }

    public void SetHoverState(bool hover)
    {
        if (hover == isHovered) return;

        isHovered = hover;
        trashClosedVisual.SetActive(!hover);
        trashOpenVisual.SetActive(hover);
    }
    /* public Vector2 size = new Vector2(3, 3);
     public CauldronSpawner cauldron;

     private Vector2Int gridStart;
     private Vector2Int gridEnd;

     void Awake()
     {
         if (cauldron == null)
         {
             Debug.LogError("CauldronSpawner belum diset!");
             return;
         }


         Vector2 min = (Vector2)transform.position - size / 2f;
         Vector2 max = (Vector2)transform.position + size / 2f;

         gridStart = cauldron.gridBehavior.WorldToGrid(min);
         gridEnd = cauldron.gridBehavior.WorldToGrid(max);

         Debug.Log($"[TrashAreaGrid] Grid dihitung dari posisi '{gameObject.name}' di {transform.position}");
         Debug.Log($"[TrashAreaGrid] Grid dari {gridStart} ke {gridEnd}");
     }

     public bool IsInsideTrashArea(Vector3 worldPos)
     {
         Vector2Int gridPos = cauldron.gridBehavior.WorldToGrid(worldPos);
         Debug.Log($"Cek apakah posisi {gridPos} ada di trash grid area");

         return gridPos.x >= gridStart.x && gridPos.x <= gridEnd.x &&
                gridPos.y >= gridStart.y && gridPos.y <= gridEnd.y;
     }
     void OnDrawGizmos()
     {
         if (cauldron == null || cauldron.gridBehavior == null) return;

         Gizmos.color = Color.red;

         for (int x = gridStart.x; x <= gridEnd.x; x++)
         {
             for (int y = gridStart.y; y <= gridEnd.y; y++)
             {
                 Vector3 center = cauldron.gridBehavior.GridToWorld(new Vector2Int(x, y)) + new Vector3(0.5f, 0.5f, 0f);
                 Gizmos.DrawWireCube(center, Vector3.one);
             }
         }
     }*/
}
