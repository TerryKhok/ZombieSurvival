using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will share logic for any unit on the field. Could be friend or foe, controlled or not.
/// Things like taking damage, dying, animation triggers etc
/// </summary>

public class UnitBase : MonoBehaviour {

    public Stats Stats { get; private set; }
    public virtual void SetStats(Stats stats) => Stats = stats;

    //ƒ_ƒ[ƒWˆ—
    public virtual int ManageHealth(int curHP,int dmg)
    {
        curHP += dmg;

        //if HP = 0 => dies

        return(curHP);
    }

    

    
}