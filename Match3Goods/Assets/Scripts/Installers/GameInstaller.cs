using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public LevelPathsSO levelPathsSO;
    public GameObject interactableSlotPrefab;
    public LevelManager levelManager;
    public TimerManager timerManager;
    public SlotManager slotManager;
    public PopupManager popupManager;
    public GameStateManager stateManager;
    public override void InstallBindings()
    {
        Container.Bind<LevelPathsSO>().FromInstance(levelPathsSO).AsSingle();
        Container.BindFactory<InteractableSlot, InteractableSlotFactory>()
            .FromComponentInNewPrefab(interactableSlotPrefab)
            .AsSingle();
        Container.Bind<GameStateManager>().FromInstance(stateManager).AsSingle();
        Container.Bind<LevelManager>().FromInstance(levelManager).AsSingle();
        Container.Bind<TimerManager>().FromInstance(timerManager).AsSingle();
        Container.Bind<SlotManager>().FromInstance(slotManager).AsSingle();
        Container.Bind<PopupManager>().FromInstance(popupManager).AsSingle();
    }
}

