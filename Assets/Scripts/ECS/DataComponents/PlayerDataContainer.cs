using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct PlayerDataContainer : IBufferElementData
{
    public int Health;
    public int Experience;

    public PlayerDataContainer( int Health, int Experience)
    {
        this.Health = Health;
        this.Experience = Experience;
    }
}
