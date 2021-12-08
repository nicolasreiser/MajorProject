using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class BulletCollisionEventSystem : JobComponentSystem
{
    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;

    protected override void OnCreate()
    {
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    [BurstCompile]
    struct CollisionEventJob : ICollisionEventsJob
    {
        public ComponentDataFromEntity<BulletData> BulletGroup;
        public ComponentDataFromEntity<Translation> ColliderGroup;
        public ComponentDataFromEntity<PlayerData> PlayerGroup;
        public ComponentDataFromEntity<EnemyData> EnemyGroup;


        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool isBulletA = BulletGroup.HasComponent(entityA);
            bool isBulletB = BulletGroup.HasComponent(entityB);

            bool isColliderA = ColliderGroup.HasComponent(entityA);
            bool isColliderB = ColliderGroup.HasComponent(entityB);

            bool isPlayerA = PlayerGroup.HasComponent(entityA);
            bool isPlayerB = PlayerGroup.HasComponent(entityB);

            bool isEnemyA = EnemyGroup.HasComponent(entityA);
            bool isEnemyB = EnemyGroup.HasComponent(entityB);


            

            if (isBulletA && isColliderB)
            {
                //Bullet hit something V1


                //who does the bullet belong to
                var isBulletAFromPlayer = BulletGroup[entityA].Origin;

                //Damage player or enemy
                switch (isBulletAFromPlayer)
                {
                    case BulletOrigin.Player:

                        if(isEnemyB)
                        {
                            
                            var enemy = EnemyGroup[entityB];
                            var bullet = BulletGroup[entityA];

                            if(bullet.ScatterShot)
                            {
                                enemy.ScatterShot = true;
                                enemy.ScatterShotDamage = bullet.ScatterShotDamage;
                                enemy.ScatterShotCooldown = bullet.ScatterShotCooldown;
                            }

                            enemy.CurrentHealth -= bullet.Damage;
                            if (enemy.CurrentHealth < 0)
                                enemy.CurrentHealth = 0;

                            EnemyGroup[entityB] = enemy;
                        }
                        break;
                    case BulletOrigin.Enemy:

                        if(isPlayerB)
                        {
                            var player = PlayerGroup[entityB];
                            var bullet = BulletGroup[entityA];

                            if (player.IsInvulnerable)
                            {
                                player.CurrentHealth -= bullet.Damage;
                                player.OnHealthChange = true;
                                if (player.CurrentHealth < 0)
                                    player.CurrentHealth = 0;

                                PlayerGroup[entityB] = player;
                            }
                        }
                        break;
                }
                
                var Bullet = BulletGroup[entityA];
                Bullet.ShouldDestroy = true;
                BulletGroup[entityA] = Bullet;
            }
            if (isBulletB && isColliderA)
            {
                //Bullet hit something V2
                var isBulletBFromPlayer = BulletGroup[entityB].Origin;

                //Damage player or enemy
                switch (isBulletBFromPlayer)
                {
                    case BulletOrigin.Player:

                        if (isEnemyA)
                        {

                            var enemy = EnemyGroup[entityA];
                            var bullet = BulletGroup[entityB];

                            enemy.CurrentHealth -= bullet.Damage;
                            if (enemy.CurrentHealth < 0)
                                enemy.CurrentHealth = 0;

                            if (bullet.ScatterShot)
                            {
                                enemy.ScatterShot = true;
                                enemy.ScatterShotDamage = bullet.ScatterShotDamage;
                                enemy.ScatterShotCooldown = bullet.ScatterShotCooldown;
                            }

                            EnemyGroup[entityA] = enemy;

                        }
                        break;
                    case BulletOrigin.Enemy:

                        if (isPlayerA)
                        {
                            var player = PlayerGroup[entityA];
                            var bullet = BulletGroup[entityB];

                            if (!player.IsInvulnerable)
                            {
                                player.CurrentHealth -= bullet.Damage;
                                player.OnHealthChange = true;

                                if (player.CurrentHealth < 0)
                                    player.CurrentHealth = 0;

                                PlayerGroup[entityA] = player;
                            }

                        }
                        break;
                }
                var Bullet = BulletGroup[entityB];
                Bullet.ShouldDestroy = true;
                BulletGroup[entityB] = Bullet;
            }
        }
    }

    protected override JobHandle OnUpdate( JobHandle inputDeps)
    {
        JobHandle jobHandle = new CollisionEventJob
        {
            BulletGroup = GetComponentDataFromEntity<BulletData>(),
            ColliderGroup = GetComponentDataFromEntity<Translation>(),
            PlayerGroup = GetComponentDataFromEntity<PlayerData>(),
            EnemyGroup = GetComponentDataFromEntity<EnemyData>()
        }.Schedule(m_StepPhysicsWorldSystem.Simulation,
                    ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);

        return jobHandle;
    }
}
