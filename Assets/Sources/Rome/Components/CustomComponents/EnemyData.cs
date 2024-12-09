using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace EnemyHandling
{
    public struct EnemyData : IComponentData
    {
        public int health;
        public int armor;
        public int flatDamageReduction;
        public bool hasOnDeathEvent;
        public bool hasOnHitEvent;
        public float baseSpeed;
        public float finalSpeed;
        public float baseDamage;
        public float finalDamage;
        public bool hasRangedAttack;
        public bool hasChargeUpBeforeRangedAttack;
        public float rangedAttackChargeUpDuration;
        public bool hasChargeUpBeforeMeleeAttack;
        public float meleeAttackChargeUpDuration;
        public float baseProjectileSpeed;
        public float finalProjectileSpeed;
        public float knockbackResist;
        //requires a minimum score of x to spawn //not fully implemented
        public int minScoreRequirement;
        //requires a minimum score of x to spawn //not fully implemented
        public int maxScoreRequirement;
        public bool hasTriggeredBerserk;
        public float berserkSpeedBonus;
        public float berserkDamageBonus;
    }
}