using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Prefab to use for the objects in the pool
    public GameObject prefab;

    // Size of the pool
    public int poolSize = 1024;

    // Array to store the entities in the pool
    static private Entity[] pool;

    // EntityManager to manage the entities
    private EntityManager entityManager;

    private void Start()
    {
        // Get the EntityManager from the default world
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Initialize the pool array
        pool = new Entity[poolSize];

        // Create the specified number of entities and add them to the pool
        for (int i = 0; i < poolSize; i++)
        {
            // Create a new entity
            Entity entity = entityManager.CreateEntity(typeof(PoolableComponent));

            // Add a PoolableComponent to the entity and set it to inactive
            entityManager.SetComponentData(entity, new PoolableComponent
            {
                isActive = false
            });

            // Add the prefab as a GameObjectComponent to the entity
            entityManager.AddComponentObject(entity, prefab);

            // Add the entity to the pool
            pool[i] = entity;
        }
    }

    // Method to get an inactive entity from the pool
    public Entity GetObject()
    {
        // Loop through the pool to find an inactive entity
        for (int i = 0; i < poolSize; i++)
        {
            Entity entity = pool[i];
            PoolableComponent poolable = entityManager.GetComponentData<PoolableComponent>(entity);
            if (!poolable.isActive)
            {
                // If an inactive entity is found, set it to active and return it
                entityManager.SetComponentData(entity, new PoolableComponent
                {
                    isActive = true
                });
                return entity;
            }
        }

        // Return null if there are no inactive entities
        return Entity.Null;
    }

    // Method to return an entity to the pool
    public void ReturnObject(Entity entity)
    {
        // Set the entity's PoolableComponent to inactive
        entityManager.SetComponentData(entity, new PoolableComponent
        {
            isActive = false
        });
    }
}

// ComponentData to track the active state of an entity
public struct PoolableComponent : IComponentData
{
    public bool isActive;
}