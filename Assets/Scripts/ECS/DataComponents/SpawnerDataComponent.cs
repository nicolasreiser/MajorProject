using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct SpawnerDataComponent : IBufferElementData
{
    public int EnemiesAmmount;
    public int InitialDelay;
    public int DelayBetweenSpawns;
    public bool MeleeEnemy;
    public bool RangedEnemy;
    public bool BombEnemy;

    public SpawnerDataComponent(int EnemiesAmmount, int InitialDelay, int DelayBetweenSpawns, EnemyType[] enemyTypes)
    {
        this.EnemiesAmmount = EnemiesAmmount;
        this.InitialDelay = InitialDelay;
        this.DelayBetweenSpawns = DelayBetweenSpawns;
        MeleeEnemy = false;
        BombEnemy = false;
        RangedEnemy = false;
        foreach (var item in enemyTypes)
        {
            switch (item)
            {
                case EnemyType.Bomb:
                    BombEnemy = true;
                    break;
                case EnemyType.Melee:
                    MeleeEnemy = true;
                    break;
                case EnemyType.Ranged:
                    RangedEnemy = true;
                    break;
            }
        }
    }
}
