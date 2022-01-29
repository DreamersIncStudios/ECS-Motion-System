using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dreamers.InventorySystem.MissionSystem.Interfaces
{
    public interface IOnDeathHandler
    {
        NPC GetNPCType{get;}
        void OnDeath();
    }

    public interface IOnCollectHandler
    {
    
        void OnPickUp();
    }
    public interface IOnDestoryHandler
    {

        void OnDestory();
    }
    public interface IOnCompleteHandler
    {

        void OnCompleteDefend();
    }
}