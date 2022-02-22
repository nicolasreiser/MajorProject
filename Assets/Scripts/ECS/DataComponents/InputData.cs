using UnityEngine;
using Unity.Entities;

//Entity storing player keyboard input
// for debuging purposes

[GenerateAuthoringComponent]
public struct InputData : IComponentData
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
}
