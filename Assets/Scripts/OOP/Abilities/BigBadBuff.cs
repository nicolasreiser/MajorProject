using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBadBuff : BaseAbility, IAbility
{
    private float CurrentCooldown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void CastAbility()
    {
        base.CastAbility();
    }

    public override bool IsOnCooldown()
    {

        return false;
    }
}
