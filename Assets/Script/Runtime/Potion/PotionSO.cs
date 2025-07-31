using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Potion/Potion")]
public class PotionSO : ScriptableObject
{
    public string potionName;
    public Sprite icon;
}
