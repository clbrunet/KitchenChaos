using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class StoveRecipeSO : ScriptableObject
{
    public KitchenObjectSO uncooked;
    public KitchenObjectSO cooked;
    public KitchenObjectSO burned;
    public float cookTime = 4f;
}
