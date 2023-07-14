using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CostsAmounts", menuName = "CostsAmountsScriptableObject")]
public class CostsAmounts : ScriptableObject
{
    public List<int> UpgradeCosts = new List<int>();
}
