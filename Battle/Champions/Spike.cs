using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spike : Champion
{
    //private static Spike instance = new Spike(300, 60, 37, 60, 37, 0.35f, 7, 100);
    private int mana;
    public Spike(float hp, float atk, float def, float mga, float mgd, float speed, float regen, int mana)
    {
        //Instantiate champ stats
        this.hp = hp;
        this.atk = atk;
        this.def = def;
        this.mga = mga;
        this.mgd = mgd;
        this.speed = speed;
        this.mana = mana;
        this.regen = regen;
        maxHealth = hp;
    }
    // public static Spike Instance
    // {
    //     get { return instance; }
    // }
    public override string AutoAttack(Champion target)
    {
        return null;
    }
    //Slap: slap the opponent for physical damage. No cost
    public override string QAttack(Champion target)
    {
        float[] targetStats = target.GetStats();
        targetStats[0] -= DamageCalc(80 + (atk * 0.1f), targetStats[2]);
        target.SetStats(targetStats);
        return "The Spike slapped you. Ouch!";
    }
    //Prayer: Heal for 25% max health
    public override string WAttack(Champion target)
    {
        float[] targetStats = target.GetStats();
        Debug.Log("W");
        hp += maxHealth * 0.2f;
        if (hp > maxHealth) hp = maxHealth;
        mana -= 50;
        target.SetStats(targetStats);
        return "The Spike prays. Somehow, its health is restored.";
    }
    //Fireball: Uses basic magic attack. No cost.
    public override string EAttack(Champion target)
    {
        float[] targetStats = target.GetStats();
        targetStats[0] -= DamageCalc(80 + (mga * 0.1f), targetStats[4]);
        mana -= 25;
        target.SetStats(targetStats);
        return "Fireball";
    }
    public override string RAttack(Champion target)
    {
        float[] targetStats = target.GetStats();
        Debug.Log("R");
        target.SetStats(targetStats);
        return null;
    }
    //Checks for if Spike can use spells
    public override bool CanQ() { return true; }
    public override bool CanW() { return mana > 50; }
    public override bool CanE() { return mana > 25;  }

    public override string PickMove(Champion target){
        return "";
    }

    //Returns current amount of mana at index 0, max mana at index 1, mana regen rate at index 2, and starting mana at index 3
    public override int[] GetResource()
    {
        return new int[] { mana, 100, 3, 100 };
    }
    public override void SetResource(int mana)
    {
        this.mana = mana;
    }
    public override void Reset()
    {
        hp = 100;
        atk = 70;
        def = 55;
        mga = 10;
        mgd = 60;
        speed = 0.5f;
    }
}
