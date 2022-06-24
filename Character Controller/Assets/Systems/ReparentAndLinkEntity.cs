using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using ECS.Utilities;

public class ReparentAndLinkEntity : MonoBehaviour,IConvertGameObjectToEntity
{
    public List<GameObject> ChildrenToLink;

    public Entity entity;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

       this.entity = entity;
        dstManager.AddBuffer<LinkedEntityGroup>(entity);
        dstManager.AddBuffer<Child>(entity);
        dstManager.AddComponent<Unity.Transforms.CopyTransformFromGameObject>(entity);
    }

    // Start is called before the first frame update
    void Start()
    {
        var parentData = new Parent() { Value = entity };
        foreach (var item in ChildrenToLink)
        {
            LinkChild(item, parentData);
        }
    }

    public void LinkChild(GameObject child, Parent parentData) {
        EntityManager dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var link = child.GetComponent<ReparentAndLinkEntity>();
        if (link)
        {
            dstManager.GetBuffer<LinkedEntityGroup>(entity).Add(new LinkedEntityGroup() { Value = link.entity });
            dstManager.GetBuffer<LinkedEntityGroup>(link.entity).Add
                 (new LinkedEntityGroup() { Value = entity });
            dstManager.AddComponentData(link.entity, parentData);
            dstManager.AddComponent<LocalToParent>(link.entity);
            dstManager.GetBuffer<Child>(entity).Add(new Child() { Value = link.entity });
            child.transform.SetParent(transform);

        }
    }

    public void LinkChild(GameObject child)
    {
        EntityManager dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var parentData = new Parent() { Value = entity };
        var link = child.GetComponent<ReparentAndLinkEntity>();
        if (link)
        {
            dstManager.GetBuffer<LinkedEntityGroup>(entity).Add(new LinkedEntityGroup() { Value = link.entity });
            dstManager.GetBuffer<LinkedEntityGroup>(link.entity).Add
                 (new LinkedEntityGroup() { Value = entity });
            dstManager.AddComponentData(link.entity, parentData);
            dstManager.AddComponent<LocalToParent>(link.entity);
            dstManager.GetBuffer<Child>(entity).Add(new Child() { Value = link.entity });
            child.transform.SetParent(transform);

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
