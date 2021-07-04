using System.Collections;
using System.Collections.Generic;
//UNUSED CLASS. MAY BE USED FOR GENERAL SPELLS LATER.
public class Attack
{
    /*
     * Types and params are as follows:
     * Type                             Params
     * ---------------------------------------------------
     * 0: Phys Dmg                      | {Base Power, Accuracy, Cooldown}
     * 1: Mag Dmg                       | {Base Power, Accuracy, Cooldown}
     * 2: Refund Cost if Killed (Phys)  | {Base Power, Mana Cost, Health Cost, Cooldown}
     * 3: Hit All Lower Def             | {}
     */
    public int type;
    public int[] moveParams;
    public Attack(int type, int[] moveParams)
    {
        this.type = type;
        this.moveParams = moveParams;
    }
}
