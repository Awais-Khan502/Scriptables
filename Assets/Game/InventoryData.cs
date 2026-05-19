// InventoryData.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public int maxSlots;
    public float maxWeight;
    public List<string> itemIds;
    public Dictionary<string, int> itemQuantities; // itemId -> quantity
}
