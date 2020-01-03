using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ConvertToEntity : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // No special components to add
    }
}
