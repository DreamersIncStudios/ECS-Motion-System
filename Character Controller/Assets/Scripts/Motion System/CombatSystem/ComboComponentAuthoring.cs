using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Core.SaveSystems;
public class ComboComponentAuthoring : MonoBehaviour,IConvertGameObjectToEntity,ISave
{
    public Combos Combo;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new ComboComponent() { animator = GetComponent<Animator>(), combo = Instantiate(Combo)};
        dstManager.AddComponentData(entity, data);

    }

    public void Load(string jsonData)
    {
        throw new System.NotImplementedException();
    }
    public void Save() {
        SaveSystem.Instance.gameData.GetCharacterSaveData.PlayerCombos = GetSaveData();
    
    }
    public SaveData GetSaveData()
    {
        return Combo.GetSaveData();
    }
}
public class ComboComponent : IComponentData
{
    public Combos combo;
    public Animator animator;
}
