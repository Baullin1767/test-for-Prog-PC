using UniRx;
using UnityEngine;
using Zenject;

public class PlayerBuildStateMachine : MonoBehaviour
{
    private readonly ReactiveProperty<BuildState> _currentState = new(BuildState.None);
    public IReadOnlyReactiveProperty<BuildState> CurrentState => _currentState;

    private BuildManager _buildManager;

    [Inject]
    public void Construct(BuildManager buildManager)
    {
        _buildManager = buildManager;
    }
    public void ToggleBuildMode()
    {
        if (_currentState.Value == BuildState.None)
        {
            if(_buildManager.HasPreviewObject)
                _currentState.Value = BuildState.HoldingItem;
        }
        else if (_currentState.Value == BuildState.HoldingItem)
        {
            if (_buildManager.CanPlace)
                _currentState.Value = BuildState.None;
        }
    }
}
public enum BuildState
{
    None,         
    HoldingItem 
}
