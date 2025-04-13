using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public class BuildManager : MonoBehaviour
{
    [Inject] private DiContainer _container;
    [Inject] private BuildableItemData _buildData;
    [Inject] private PlayerBuildStateMachine buildStateMachine;
    [Inject] private IPlayerInput playerInput;

    public bool HasPreviewObject => currentGhostObject != null;
    public bool CanPlace => canPlace;

    private GameObject currentGhostObject;
    private GameObject prefabToBuild;
    private ItemData currentObjectItemData;
    private bool canPlace; 
    private float currentYRotation = 0f;


    [Header("Build Settings")]
    public LayerMask buildSurfaceMask;
    public LayerMask AllMasks;
    public Transform playerCamera;

    [Header("Placement Settings")]
    public float raycastDistance = 5f;


    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    [Header("Rotation Settings")]
    public float rotationStep = 45f;

    void Start()
    {
        foreach (var item in _buildData.itemsData)
        {
            buildSurfaceMask |= item.surfaceLayerMask;
            AllMasks |= 1 << item.prefab.layer;
        }

        Observable.EveryUpdate()
            .Where(_ => buildStateMachine.CurrentState.Value == BuildState.HoldingItem)
            .Subscribe(_ => 
            {
                UpdateGhostPosition();
                HandleMouseScrollRotation();
            })
            .AddTo(this); 

        buildStateMachine.CurrentState
            .AsObservable()
            .Subscribe(state =>
            {
                if (state == BuildState.None)
                    PlaceObject();
            })
            .AddTo(this);
    }

    public void EnterBuildMode(ItemType itemType)
    {
        currentObjectItemData = _buildData.itemsData.First(x => x.itemType == itemType);
        prefabToBuild = currentObjectItemData.prefab;
        currentGhostObject = SpawnBuildableItem(prefabToBuild, prefabToBuild.transform.position, prefabToBuild.transform.rotation);
        SetGhostAppearance();
    }
    private void UpdateGhostPosition()
    {
        Vector3 rayOrigin = playerCamera.position;
        Vector3 rayDirection = playerCamera.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, raycastDistance, buildSurfaceMask))
        {
            Bounds bounds = GetCombinedBounds(currentGhostObject);
            Vector3 normal = hit.normal.normalized;

            Vector3 localNormal = currentGhostObject.transform.InverseTransformDirection(normal);
            Vector3 offsetLocal = Vector3.Scale(bounds.extents, new Vector3(Mathf.Abs(localNormal.x), Mathf.Abs(localNormal.y), Mathf.Abs(localNormal.z)));
            Vector3 offsetWorld = currentGhostObject.transform.TransformDirection(offsetLocal);

            currentGhostObject.transform.position = hit.point + normal * offsetWorld.magnitude;
            canPlace = IsInLayerMask(hit.collider.gameObject, currentObjectItemData.surfaceLayerMask)
                && !HasAnyCollisionWithWorld();
            SetGhostMaterialValid(canPlace);
        }
        else
        {
            currentGhostObject.transform.position = rayOrigin + rayDirection * raycastDistance;
            SetGhostMaterialValid(false);
            canPlace = false;
        }
    }
    bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }


    private Bounds GetCombinedBounds(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);

        Bounds combined = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            combined.Encapsulate(renderers[i].bounds);
        }

        return combined;
    }

    private void HandleMouseScrollRotation()
    {
        if (playerInput.RotateRight())
        {
            currentYRotation += 45f;
            ApplyRotation();
        }
        else if (playerInput.RotateLeft())
        {
            currentYRotation -= 45f;
            ApplyRotation();
        }
    }


    private void ApplyRotation()
    {
        currentGhostObject.transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }

    private void SetGhostMaterialValid(bool isValid)
    {
        var mat = isValid ? validMaterial : invalidMaterial;
        currentGhostObject.GetComponent<Renderer>().material = validMaterial;

        foreach (var renderer in currentGhostObject.GetComponentsInChildren<Renderer>())
        {
            renderer.material = mat;
        }
    }
    private void PlaceObject()
    {
        if (!canPlace) return;

        GameObject objectToPlace = SpawnBuildableItem(prefabToBuild, currentGhostObject.transform.position, currentGhostObject.transform.rotation);
        objectToPlace.GetComponent<Collider>().isTrigger = false;
        Destroy(currentGhostObject);
        currentGhostObject = null;
        prefabToBuild = null;
    }
    public GameObject SpawnBuildableItem(GameObject prefabToBuild, Vector3 position, Quaternion rotation)
    {
        return _container.InstantiatePrefab(prefabToBuild, position, rotation, null);
    }

    private void SetGhostAppearance()
    {
        currentGhostObject.layer = LayerMask.NameToLayer("IgnoreRaycast");
        SetGhostMaterialValid(true);
    }
    private bool HasAnyCollisionWithWorld()
    {
        Collider[] colliders = currentGhostObject.GetComponentsInChildren<Collider>();

        foreach (var col in colliders)
        {

            Vector3 center = col.bounds.center;
            Vector3 halfExtents = col.bounds.extents;

            Collider[] hits = Physics.OverlapBox(center, halfExtents - Vector3.one * 0.01f, Quaternion.identity, AllMasks);
            foreach (var hit in hits)
            {
                if (!hit.transform.IsChildOf(currentGhostObject.transform))
                    return true;
            }
        }

        return false;
    }

}
