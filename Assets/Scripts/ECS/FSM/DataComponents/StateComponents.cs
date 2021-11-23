using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


enum FsmState
{
    Null,
    Idle,
    Attack,
    Pathfind,
    Death,
}


[GenerateAuthoringComponent]

public struct AttackState : IComponentData
{
    public float PlayerDistance;
    public float EnemyAttackRange;
    public float EnemyMaxAttackRange;
    public int EnemyDetectionRange;
    public int DamageToDeal;
    public int2 targetPosition;

    public float BaseShootCooldown;
    public float CurrentShootCooldown;

}
[GenerateAuthoringComponent]

public struct PathfindState : IComponentData
{
    public float PlayerDistance;
    public float PlayerOurOfRangeDistance;
    public float EnemyAttackRange;
    public int2 targetPosition;
    public float PathfindCooldown;

}
[GenerateAuthoringComponent]

public struct DeathState : IComponentData
{

}
[GenerateAuthoringComponent]
struct FsmStateChanged : IComponentData
{
    public FsmState from;
    public FsmState to;
}

