using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct PrefabEntityStorage : IComponentData
{
    public Entity EnemyProjectile;
    public Entity PlayerProjectile;

    public Entity MeleeEnemy;
    public Entity RangedEnemy;
    public Entity BombEnemy;

    public Entity Player;

    public Entity DeathExplosion;
    public Entity PlayerBulletExplosion;
    public Entity EnemyBulletExplosion;
    public Entity EnemyScatter;
        
}
