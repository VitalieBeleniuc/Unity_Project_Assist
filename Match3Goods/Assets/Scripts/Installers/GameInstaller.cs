using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public LevelPathsSO levelPathsSO;
    public override void InstallBindings()
    {
        Container.Bind<GameService>().AsSingle(); // test service
        Container.Bind<LevelPathsSO>().FromInstance(levelPathsSO).AsSingle();
    }
}

