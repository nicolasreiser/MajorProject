using Unity.Entities;
using System;
using UnityEngine;

public class PlayseInputSystem : SystemBase
{
    FloatingJoystick joystick;

    protected override void OnCreate()
    {
        base.OnCreate();
        joystick = UnityEngine.Object.FindObjectOfType<FloatingJoystick>();

    }
    protected override void OnUpdate()
    {
        if( joystick == null)
        {
            joystick = UnityEngine.Object.FindObjectOfType<FloatingJoystick>();

        }
        var jw = joystick.Vertical;
        var jh = joystick.Horizontal;
        Entities.
            WithNone<PausedTag>().
            ForEach((ref MoveData moveData, in InputData inputData) =>
        {
            moveData.direction.x = jh;
            moveData.direction.z = jw;

        }).Run();
    }
}
