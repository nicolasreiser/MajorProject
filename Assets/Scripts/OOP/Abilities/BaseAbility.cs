using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility : MonoBehaviour , IAbility
{

    public float BaseCooldown;

    public virtual void CastAbility() { }

    public virtual bool IsOnCooldown()
    {
        return true;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
