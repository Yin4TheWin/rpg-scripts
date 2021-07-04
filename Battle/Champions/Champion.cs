using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Champion
{
    public float hp, atk, def, mga, mgd, speed, regen, maxHealth;
    public bool qCooldown = false, wCooldown = false, eCooldown = false, rCooldown=false, tCooldown=false, yCooldown=false;
    public Champion() {}
    public Champion(float hp, float atk, float def, float mga, float mgd, float speed, float regen, float maxHealth)
    {
        //Instantiate champ stats
        this.hp = hp;
        this.atk = atk;
        this.def = def;
        this.mga = mga;
        this.mgd = mgd;
        this.speed = speed;
        this.regen = regen;
        this.maxHealth = maxHealth;
    }

    //Methods for Q, W, E, R, and AA abilities. For specifics see Scripts/Champions/...
    public virtual string QAttack(Champion target) { return ""; }
    public virtual string WAttack(Champion target) { return null; }
    public virtual string EAttack(Champion target) { return ""; }
    public virtual string RAttack(Champion target) { return null; }
    //Same methods with all team members and current index as additional parameters
    public virtual string QAttack(Champion target, Champion[] allies, int index) { return ""; }
    public virtual string WAttack(Champion target, Champion[] allies, int index) { return null; }
    public virtual string EAttack(Champion target, Champion[] allies, int index) { return ""; }
    public virtual string RAttack(Champion target, Champion[] allies, int index) { return null; }
    //AI make move decision
    public virtual string PickMove(Champion target){ return ""; }
    public virtual void Reset() { }
    public virtual void OnHit() { }
    public virtual string AutoAttack(Champion target) { return null; }
    //Returns current amount of mana at index 0 and max mana at index 1
    public virtual int[] GetResource() { return null; }
    public virtual void SetResource(int mana) { }
    //Check if the following spells can be used
    public virtual bool CanQ() { return false; }
    public virtual bool CanW() { return false; }
    public virtual bool CanE() { return false; }
    public virtual bool CanR() { return false; }
    public bool CanT()
    {
        return !tCooldown;
    }
    public bool CanY()
    {
        return !yCooldown;
    }
    //Returns string array, index 0 is name of spell and index 1 is short desc.
    public virtual string[] DescribeQ() { return null; }
    public virtual string[] DescribeW() { return null; }
    public virtual string[] DescribeE() { return null; }
    public virtual string[] DescribeR() { return null; }
    //Check if a toggled ability is toggled
    public virtual bool ToggleW(){ return false; }
    public virtual bool ToggleR(){ return false; }

    public string[] DescribeAuto() {
        return new string[] {"Auto Attack","A basic attack that has no cost and does base damage equal to your current attack stat."};
    }
    //Multiplier for damage
    public float DamageCalc(float atk, float def)
    {
        //Where atk is relevant attack stat of attacker and def is relevant defense stat of attacker's target.
        if (def >= 0)
            return atk * (100.0f / (100 + def));
        return 2 - atk * (100.0f / (100 - def));
    }

    //Quickly get stats as an array
    public float[] GetStats()
    {
        return new float[] {hp,atk,def,mga,mgd,speed};
    }

    public void SetStats(float[] stats)
    {
        hp = stats[0];
        atk = stats[1];
        def = stats[2];
        mga = stats[3];
        mgd = stats[4];
        speed = stats[5];
    }
}
