using System.Collections;
using Unity.Entities;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] TextMesh score;
    Entity player;
    EntityManager entityManager;
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        player = entityManager.CreateEntityQuery(typeof(PlayerComponent)).GetSingletonEntity();
    }

    private void Update()
    {
        if(player != null)
        {
            score.text = entityManager.GetComponentData<PlayerComponent>(player).Score.ToString();
        }
    }
}