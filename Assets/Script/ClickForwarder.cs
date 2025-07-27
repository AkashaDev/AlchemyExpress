using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickForwarder : MonoBehaviour
{
    void OnMouseDown()
    {
        transform.parent?.GetComponent<IngredientBlock>()?.SendMessage("OnMouseDown");
    }

    void OnMouseUp()
    {
        transform.parent?.GetComponent<IngredientBlock>()?.SendMessage("OnMouseUp");
    }
}
