using UniRx;
using UnityEngine;
using Zenject;

public class PlayerInteraction : MonoBehaviour
{
    [Inject] private PlayerBuildStateMachine buildStateMachine;
    [Inject] private IPlayerInput playerInput;

    [Header("Raycast Settings")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        Observable.EveryUpdate()
            .Where(_ => playerInput.InteractionButton())
            .Subscribe(_ =>
            {
                if (buildStateMachine.CurrentState.Value == BuildState.None)
                {
                    TryInteract();
                }
                else if (buildStateMachine.CurrentState.Value == BuildState.HoldingItem)
                {
                    buildStateMachine.ToggleBuildMode();
                }
            })
            .AddTo(this);
    }


    private void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.OnInteract();
                buildStateMachine.ToggleBuildMode();
            }
        }
    }
}
