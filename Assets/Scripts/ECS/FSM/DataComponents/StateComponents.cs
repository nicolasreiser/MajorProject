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
    public float PlayerMaxAttackRange;
    public float EnemyAttackRange;
    public int2 targetPosition;

    public float ShootCooldown;

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
struct FsmStateChanged : IComponentData
{
    public FsmState from;
    public FsmState to;
}

struct Cat : IComponentData
{
    public float health;
    
}