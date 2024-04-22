using DreamersInc.ComboSystem;
using Unity.Entities;
using UnityEngine;

namespace DreamersInc.InputSystems
{
    [CreateAssetMenu(fileName = "ControllerData", menuName = "ScriptableObjects/Controller Input Options", order = 1)]
    public class ControllerOptions : ScriptableObject
    {
        public bool PressHold = true;

        public void ChangeControllerSettings()
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            manager.CreateEntityQuery(typeof(Player_Control)).TryGetSingletonEntity<Player_Control>(out var player);
            var inputHandler = manager.GetComponentData<Command>(player);
            inputHandler.StyleModPressHold = PressHold;
        }
    }
    
}