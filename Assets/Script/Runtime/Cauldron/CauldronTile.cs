using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronTile : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color baseColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }

    public void SetColor(Color color)
    {
        if (sr != null)
            sr.color = color;
    }

    public void ResetColor()
    {
        if (sr != null)
            sr.color = baseColor;
    }
}
