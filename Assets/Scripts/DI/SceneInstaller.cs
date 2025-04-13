using Zenject;

public class SceneInstaller : MonoInstaller
{
    public BuildManager buildManager;
    public BuildableItemData defaultBuildableItemData;

    public override void InstallBindings()
    {
        Container.Bind<IPlayerInput>().To<PCInput>().AsSingle();
        Container.Bind<BuildableItemData>().FromInstance(defaultBuildableItemData).AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerBuildStateMachine>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BuildManager>().FromComponentInHierarchy().AsSingle();
    }
}
