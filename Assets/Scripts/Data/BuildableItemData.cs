using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildableItemData", menuName = "Configs/CreateBuildableItemData")]
public class BuildableItemData : ScriptableObject
{
    public ItemData[] itemsData;
   
}

[Serializable]
public struct ItemData
{
    public ItemType itemType;
    public GameObject prefab;
    public LayerMask surfaceLayerMask;
}

public enum ItemType
{
    Cube,
    Sphere
}