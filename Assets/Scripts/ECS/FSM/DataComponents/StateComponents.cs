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
public struct IdleState : IComponentData
{
    public float PlayerDistance;
    public float MaxPlayerDistance;
}

public struct AttackState : IComponentData
{
    public float PlayerDistance;

}

public struct PathfindState : IComponentData
{
    public float PlayerDistance;
    public float2 NextNode;

}
public struct DeathState : IComponentData
{

}
struct FsmStateChanged : IComponentData
{
    public FsmState from;
    public FsmState to;
}

struct EnemyFiniteStateMachine : IComponentData
{
    public FsmState currentState;
}

struct Cat : IComponentData
{
    public float health;
    
}