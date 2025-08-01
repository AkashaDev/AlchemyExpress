using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerTestBehavior : MonoBehaviour
{
    public void ReceivePotion(Potion potion)
    {
        Debug.Log($"Customer menerima potion: {potion.potionName}");
    }
}
