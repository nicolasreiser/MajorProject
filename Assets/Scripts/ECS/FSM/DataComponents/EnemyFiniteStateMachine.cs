using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
struct EnemyFiniteStateMachine : IComponentData
{
    public FsmState currentState;
}
