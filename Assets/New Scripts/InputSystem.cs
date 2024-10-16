using UnityEngine;
using Unity.Entities;
using System;

public partial class InputSystem : SystemBase
{
    private Controls controls;

    protected override void OnCreate()
    {
        if(!SystemAPI.TryGetSingleton<InputComponent>(out var input))
        {
            EntityManager.CreateEntity(typeof(InputComponent));
        }

        controls = new Controls();
        controls.Enable();
    }
    protected override void OnUpdate()
    {
        Vector2 moveVector = controls.ActionMap.Movement.ReadValue<Vector2>();
        Vector2 mouse = controls.ActionMap.MousePosition.ReadValue<Vector2>();
        bool shoot = controls.ActionMap.Shoot.ReadValue<float>() == 1 ? true : false ;

        SystemAPI.SetSingleton(new InputComponent { mouse = mouse, movement = moveVector, shoot = shoot });
    }
}

