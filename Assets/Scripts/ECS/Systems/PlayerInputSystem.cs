using Unity.Entities;
using System;
using UnityEngine;

// system managing player inputs
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
        // get joystick reference
        if( joystick == null)
        {
            joystick = UnityEngine.Object.FindObjectOfType<FloatingJoystick>();

        }

        var jw = joystick.Vertical;
        var jh = joystick.Horizontal;

        // loop over entities to be moved
        Entities.
            WithNone<PausedTag>().
            ForEach((ref MoveData moveData, in InputData inputData) =>
        {
            moveData.direction.x = jh;
            moveData.direction.z = jw;

            if(jh != 0 && jw != 0)
            {
                moveData.lastDirection.x = jh;
                moveData.lastDirection.z = jw;
            }

        }).Run();
    }
}
