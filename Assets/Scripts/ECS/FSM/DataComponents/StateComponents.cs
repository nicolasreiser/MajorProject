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
    
}

public struct AttackState : IComponentData
{

}

public struct PathfindState : IComponentData
{

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