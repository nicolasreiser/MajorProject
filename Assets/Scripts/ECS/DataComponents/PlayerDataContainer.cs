using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// storage entity containing a list with the levels and health of the player
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
