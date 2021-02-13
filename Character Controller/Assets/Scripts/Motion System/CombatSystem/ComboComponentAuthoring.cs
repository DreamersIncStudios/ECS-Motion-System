//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Entities;

//public class ComboComponentAuthoring : MonoBehaviour,IConvertGameObjectToEntity
//{
//    public Combos Combo;
//    public Animator GetAnimator;
//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        var data = new ComboComponent() { animator = GetAnimator, combo = Combo };
//        dstManager.AddComponentData(entity, data);

//    }


//}
//public class ComboComponent : IComponentData
//{
//    public Combos combo;
//    public Animator animator;
//}
