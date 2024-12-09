using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace EnemyHandling
{
    public struct EnemyType : IComponentData
    {
        //this identifies what enemy type it is
        public int ID;
    }
}