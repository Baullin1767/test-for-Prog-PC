using UnityEngine;
using Zenject;
using static BuildableItemData;

[RequireComponent(typeof(Collider))]
public class BuildableItem : MonoBehaviour, IInteractable
{
    [Inject] private BuildManager _buildManager;

    public ItemType itemData; 
    public void OnInteract()
    {
        _buildManager.EnterBuildMode(itemData);
        Destroy(gameObject);
    }
}
