using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
public class Don: Champion
{
    //private static Don instance = new Don(615, 67, 34, 10, 37, 0.5f, 8, 100);
    private int moxie;
    private int moxieGen=0;
    private bool toggle = false, toggleR = false;
    public Don(float hp, float atk, float def, float mga, float mgd, float speed, float regen, int moxie) {
        //Instantiate champ stats
        this.hp = hp;
        this.atk = atk;
        this.def = def;
        this.mga = mga;
        this.mgd = mgd;
        this.speed = speed;
        this.moxie = moxie;
        this.regen = regen;
        maxHealth = hp;
    }
    // public static Don Instance
    // {
    //     get { return instance; }
    // }
    public override string AutoAttack(Champion target)
    {
        float[] targetStats = target.GetStats();
        targetStats[0] -= DamageCalc(atk, targetStats[2]);
        target.SetStats(targetStats);
        moxie += 5;
        return "Don perfoms an Auto Attack. Moxie up!";
    }
    //Gambit: Costs 15 moxie and 10% max health. If the attack kills the target the cost is refunded
    public override string QAttack(Champion target)
    {
        float[] targetStats = target.GetStats();
        targetStats[0] -= DamageCalc(atk+(70+(atk*0.25f)), targetStats[2]);
        Debug.Log(targetStats[0]);
        hp -= 0.1f * maxHealth;
        moxie -= 35;
        target.SetStats(targetStats);
        qCooldown = true;
        return "Don used Gambit. Risky move!";
    }
    public override string WAttack(Champion target)
    {
        Debug.Log("B4 "+toggle);
        if (!toggle)
        {
            moxieGen = 5;
            speed *= 1.5f;
            atk *= 1.3f;
            def *= 1.4f;
            mgd *= 1.4f;
            regen = maxHealth * -0.005f;
            toggle = true;
        }
        else
        {
            speed /= 1.5f;
            atk /= 1.3f;
            moxieGen = 0;
            def /= 1.4f;
            mgd /= 1.4f;
            regen = maxHealth * 0.02f;
            hp *= 1.05f;
            if (hp > maxHealth) hp = maxHealth;
            toggle = false;
        }
        wCooldown = true;
        Debug.Log("After " + toggle);
        if(toggle)
            return "Don triggers his adrenaline. Let's go!";
        return "Don tones down his adrenaline. Rest up a bit...";
    }
    public override bool ToggleW(){
        return toggle;
    }
    public override bool ToggleR(){
        return toggleR;
    }
    //Brave Flurry: Hits all enemies with a flurry of rapid strikes that lower defense
    public override string EAttack(Champion target)
    {
        float[] targetStats = target.GetStats();
        targetStats[0] -= DamageCalc(60 + (atk * 0.1f), targetStats[2]);
        targetStats[1] *= 0.8f;
        moxie -= 15;
        target.SetStats(targetStats);
        eCooldown = true;
        //StartCoroutine();
        //Test
        return "Don used Brave Flurry. Enemy defense down!";
    }
    //Hero's Decree: If below 50% health, restore 35% max health. Until his death, Don deals and receives 50% more damage and gains 25% speed buff.
    public override string RAttack(Champion target)
    {
        Debug.Log("R");
        if(hp<maxHealth/2)
            hp += maxHealth * 0.35f;
        atk *= 1.50f;
        def *= 0.5f;
        mga *= 1.5f;
        mgd *= 0.5f;
        regen *= 1.25f;
        rCooldown = true;
        toggleR = true;
        RToggleCD();
        return "Hero's Decree";
    }

    private async void RToggleCD(){
        await Task.Delay(20000);
        atk /= 1.50f;
        def /= 0.5f;
        mga /= 1.5f;
        mgd /= 0.5f;
        regen /= 1.25f;
        toggleR=false;
    }
    public override void OnHit()
    {
        moxie += 5;
    }
    //Checks for if Don can use spells
    public override bool CanQ() { 
        if(!qCooldown)
            return moxie > 35 && hp * 10 > maxHealth;
        return false;
    }
    public override bool CanW()
    {
        return !wCooldown;
    }
    public override bool CanE() { 
        if(!eCooldown)
            return moxie > 15;
        return false;
    }
    public override bool CanR() {
        return !rCooldown;
    }
    //Returns descriptions for each spell that display on hover
    public override string[] DescribeQ()
    {
        return new string[] { "Gambit", "Don recklessly swings at the target, sacrificing 15 moxie and 5% max health. If he kills the target with this attack, all attack costs are refunded." };
    }
    public override string[] DescribeW()
    {
        return new string[] { "Adrenaline", "(Toggle) While activated, Don hypes himself up, gaining +5 moxie/sec and increased stats but losing -2% max hp/sec. While not activated Don regens 2% more hp/sec." };
    }
    public override string[] DescribeE()
    {
        return new string[] { "Brave Flurry", "A series of rapid yet inaccurate strikes that hits all enemies. Lowers all enemies physical defense stat." };
    }
    public override string[] DescribeR()
    {
        return new string[] { "Hero's Decree", "If Don is below 50% health, restore 35% max health. For the next 20 seconds, Don deals and receives 50% more damage and gains 25% speed buff." };
    }
    //Returns current amount of mana at index 0, max mana at index 1, and mana regen rate at index 2
    public override int[] GetResource()
    {
        return new int[] { moxie, 100, moxieGen, 0 };
    }
    public override void SetResource(int mana)
    {
        moxie = mana;
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
