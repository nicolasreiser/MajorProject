using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility 
{
    public bool IsOnCooldown();
    public void CastAbility();
}
