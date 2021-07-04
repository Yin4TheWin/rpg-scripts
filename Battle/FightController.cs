using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FightController : MonoBehaviour
{
    public GameObject aImage, qImage, wImage, eImage, rImage;
    public GameObject actionButton, actionMenu, enemyObject, playerObject, enemyCanvasObject;
    //Info panels for spells when you hover over them. They are hidden by default.
    public GameObject aPanel, qPanel, wPanel, ePanel, rPanel, shopPanel, farmPanel, swapPanel;
    //The text objects in the above info panels
    public Text aName, aDesc, qName, qDesc, wName, wDesc, eName, eDesc, rName, rDesc, goldText;
    //Borders that appear when a toggled ability is toggled
    public GameObject wToggle, rToggle;
    //Cooldown timers for each spell
    public Text qCooldownText, wCooldownText, eCooldownText, rCooldownText;
    //Array of text for displaying player stats
    public Text[] statsText;
    public AudioSource playerSounds, enemySounds;
    //Array of sound effects for all champs in the game.
    //Indexes 0-2: Don
    public AudioClip[] battleSoundEffects;
    //Source code for Healthbar can be found in "Store Assets/SimpleHealthBar"
    public Healthbar champBar, healthBar, manaBar;
    private Champion current;
    private Champion[] foes=new Champion[3];
    //Stack to store list of moves used in reverse chronological order
    private Stack movesList = new Stack();
    public Text actionText;
    private int turn = 1, gold = 0, playerIndex, enemyIndex;
    private string currChamp;
    //Enemy resource bar and model arrays
    public GameObject[] enemyArr, enemyResourceArr;
    //Arrows that show which enemy is currently targeted
    public GameObject[] arrowsArr;
    //Boolean arrays to check which enemies are alive
    private bool[] livingEnemies={false, false, false};
    public Animator canvAn;
    private bool end=false;
    int c=0;
    // Start is called before the first frame update
    void Start()
    {
        //The first selected player and targeted foe should be the one in the center position
        playerIndex=Storage.currentChamps.Length/2;
        enemyIndex=1;
        Debug.Log(enemyIndex);
        currChamp=Storage.currentChamps[playerIndex];
        //Check which enemies are living.
        livingEnemies[1]=true;
        livingEnemies[0]=Storage.currentFoes.Length>1;
        livingEnemies[2]=Storage.currentFoes.Length>2;
        //Finds the correct "Champion" object based on their names
        current = Storage.getChamps(currChamp);
        for(int x=0;x<foes.Length;x++){
            if(livingEnemies[x] && Storage.currentFoes.Length>1)
                foes[x]=Storage.getChamps(Storage.currentFoes[x]);
            else if(livingEnemies[x])
                foes[x]=Storage.getChamps(Storage.currentFoes[0]);
            else
                foes[x]=null;
        }
        StartCoroutine(GainGold());
        //Set player and enemy resource bars to correct values
        SetBars();
        //Set spell images to correct images.
        aImage.GetComponent<Image>().sprite=GetSpellImage("Melee","AA");
        qImage.GetComponent<Image>().sprite=GetSpellImage(currChamp,"Q");
        wImage.GetComponent<Image>().sprite=GetSpellImage(currChamp,"W");
        eImage.GetComponent<Image>().sprite=GetSpellImage(currChamp,"E");
        rImage.GetComponent<Image>().sprite=GetSpellImage(currChamp,"R");
    }

    // Update is called once per frame
    void Update()
    {
        if(Storage.dance){
            playerObject.transform.eulerAngles = new Vector3(
                    playerObject.transform.eulerAngles.x,
                    0,
                    playerObject.transform.eulerAngles.z
            );
            playerObject.GetComponent<Animator>().Play("Dance");
        }
        //If user clicks on a different enemy, set that as the new target
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    //Our custom method.
                    int[] properPos={-30,0,30}; 
                    if(raycastHit.transform.gameObject.tag=="1")
                        enemyIndex=0;
                    if(raycastHit.transform.gameObject.tag=="2")
                        enemyIndex=1;
                    if(raycastHit.transform.gameObject.tag=="3")
                        enemyIndex=2;
                    playerObject.transform.eulerAngles = new Vector3(
                            playerObject.transform.eulerAngles.x,
                            properPos[enemyIndex],
                            playerObject.transform.eulerAngles.z
                    );
                }
           }
        }
        c=0;
        //Check which enemies are alive. If any are dead, hide them from view.
        for(int x=0;x<livingEnemies.Length;x++){
            if(foes[x]==null||(int)enemyResourceArr[x].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Healthbar>().health<=0) c++;
            if((int)enemyResourceArr[x].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Healthbar>().health<=0){
                //arrowsArr[x].SetActive(false);
                livingEnemies[x]=false;
                //If the dead enemy was the targeted enemy, move on to a new target
                if(x==enemyIndex){
                    if(livingEnemies[0]){
                        enemyIndex=0;
                        playerObject.transform.eulerAngles = new Vector3(
                            playerObject.transform.eulerAngles.x,
                            -30,
                            playerObject.transform.eulerAngles.z
                        );
                    }
                    else
                    {
                        enemyIndex=livingEnemies[1]?1:2;
                        playerObject.transform.eulerAngles = new Vector3(
                            playerObject.transform.eulerAngles.x,
                            livingEnemies[1]?0:30,
                            playerObject.transform.eulerAngles.z
                        );
                    }
                }
            }
            //HideArrows();
            for(int i=0;i<arrowsArr.Length;i++)
            arrowsArr[i].SetActive(i==enemyIndex);
            //arrowsArr[enemyIndex].GetComponent<Animator>().Play("Animation");
            enemyArr[x].SetActive(livingEnemies[x]);
            if(x<Storage.currentFoes.Length)
                enemyArr[x].transform.Find(Storage.currentFoes[x]).gameObject.SetActive(livingEnemies[x]);
            else    
                enemyArr[x].transform.Find(Storage.currentFoes[0]).gameObject.SetActive(livingEnemies[x]);
            enemyResourceArr[x].SetActive(livingEnemies[x]);
        }
        //If all enemies are dead, stop the fight.
        if(c==3){
            end=true;
            Storage.battle=false;
            canvAn.Play("Victory");
            if(Input.GetKeyDown("space")){
                SceneManager.LoadSceneAsync("Map0");
                Storage.dance=false;
            }
        }
        if(end)
            HideArrows();
        else {
            UpdateLog();
            UpdateToggles();
            goldText.text = gold + "";
            current.hp = (int)healthBar.health;
            current.SetResource((int) manaBar.health);
            //The three enemy resource bars are action (0), health (1), and mana (2). The current enemy's bars are referred to as enemyResourceArr[enemyIndex].transform.GetChild(0).GetChild(x)... where x is the corresponding index
            for(int x=0;x<3;x++){
                if(foes[x]!=null){
                    foes[x].hp = (int)enemyResourceArr[x].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Healthbar>().health;
                    foes[x].SetResource((int)enemyResourceArr[x].transform.GetChild(0).GetChild(2).gameObject.GetComponent<Healthbar>().health);
                }
            }
            //Darken color of spells if not enough energy to perform them
            enableSpells(champBar.health >= 33, current.CanQ(), current.CanW(), current.CanE(), current.CanR());
            //Do moves if you have enough energy
            if (champBar.health >= 33)
            {
                if (Input.GetKeyDown("a") || Input.GetKeyDown("q") && current.CanQ() || Input.GetKeyDown("w") && current.CanW() || Input.GetKeyDown("e") && current.CanE() || Input.GetKeyDown("r") && current.CanR())
                {
                    string animName="AA";
                    //Check which key is pressed and apply the appropriate effects
                    if (Input.GetKeyDown("a")) {
                    playerSounds.clip = battleSoundEffects[0]; playerSounds.Play(); movesList.Push("Turn " + turn + ": " + current.AutoAttack(foes[enemyIndex]) + "\n---\n"); }
                    if (Input.GetKeyDown("q")) { 
                        animName="Q";
                        playerSounds.clip = battleSoundEffects[1]; 
                        playerSounds.Play(); 
                        movesList.Push("Turn " + turn + ": " + current.QAttack(foes[enemyIndex]) + "\n---\n");
                        StartCoroutine(QCooldown(2.0f));
                    }
                    if (Input.GetKeyDown("w")) { 
                        playerSounds.clip = battleSoundEffects[1]; 
                        playerSounds.Play(); 
                        movesList.Push("Turn " + turn + ": " + current.WAttack(foes[enemyIndex]) + "\n---\n");
                        StartCoroutine(WCooldown(5.0f));
                    }
                    if (Input.GetKeyDown("e")) { 
                        playerSounds.clip = battleSoundEffects[2]; 
                        playerSounds.Play(); 
                        movesList.Push("Turn " + turn + ": " + current.EAttack(foes[enemyIndex]) + "\n---\n"); 
                        StartCoroutine(ECooldown(1.0f));
                    }
                    if (Input.GetKeyDown("r")) { 
                        playerSounds.clip = battleSoundEffects[2]; 
                        playerSounds.Play(); 
                        movesList.Push("Turn " + turn + ": " + current.RAttack(foes[enemyIndex]) + "\n---\n"); 
                        StartCoroutine(RCooldown(90.0f));
                    }
                    //Play attacking animation
                    playerObject.GetComponent<Animator>().Play(animName);
                    playerObject.GetComponent<Animator>().Play(animName,-1,0);
                    //General effects after using all spells: check for on-hit bonuses, incrememnt turn counter, update all bar values
                    SetRegen();
                    foes[enemyIndex].OnHit();
                    turn++;
                    champBar.health -= 33;
                    healthBar.health = current.hp;
                    manaBar.health = current.GetResource()[0];
                    enemyResourceArr[enemyIndex].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Healthbar>().health = foes[enemyIndex].hp;
                    enemyResourceArr[enemyIndex].transform.GetChild(0).GetChild(2).gameObject.GetComponent<Healthbar>().health = foes[enemyIndex].GetResource()[0];
                }
            }
            //Enemy action, similar to above player action
            for(int x=0;x<livingEnemies.Length;x++){
                if (livingEnemies[x]&&enemyResourceArr[x].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Healthbar>().health >= 33 && Random.Range(0,30)==19)
                {
                    enemyArr[x].transform.Find(Storage.currentFoes[Storage.currentFoes.Length>1?x:0]).GetComponent<Animator>().Play("Attack");
                    enemyArr[x].transform.Find(Storage.currentFoes[Storage.currentFoes.Length>1?x:0]).GetComponent<Animator>().Play("Attack",-1,0);
                    if (foes[x]!=null && foes[x].hp >= foes[x].maxHealth / 2 || !foes[x].CanW())
                    {
                        enemySounds.clip = battleSoundEffects[3]; enemySounds.Play();
                        movesList.Push("Turn " + turn + ": " + foes[x].QAttack(current) + "\n---\n");
                    }
                    else if (foes[x]!=null && foes[x].CanW())
                    {
                        enemySounds.clip = battleSoundEffects[4]; enemySounds.Play();
                        movesList.Push("Turn " + turn + ": " + foes[x].WAttack(current) + "\n---\n");
                    }
                    SetRegen();
                    current.OnHit();
                    turn++;
                    enemyResourceArr[x].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Healthbar>().health -= 33;
                    healthBar.health = current.hp;
                    manaBar.health = current.GetResource()[0];
                    if(foes[x]!=null){
                        enemyResourceArr[x].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Healthbar>().health = foes[x].hp;
                        enemyResourceArr[x].transform.GetChild(0).GetChild(2).gameObject.GetComponent<Healthbar>().health = foes[x].GetResource()[0];
                    }
                }
            }
        }
    }

    //If you cannot perform a spell for any reason, darken it to indicate it cannot be used.
    void enableSpells(bool enable, bool canQ, bool canW, bool canE, bool canR)
    {
        aImage.GetComponent<Image>().color = enable ? new Color32(255, 255, 255, 255) : new Color32(51, 43, 43, 255);
        qImage.GetComponent<Image>().color = canQ && enable ? new Color32(255, 255, 255, 255) : new Color32(51, 43, 43, 255);
        wImage.GetComponent<Image>().color = canW && enable ? new Color32(255, 255, 255, 255) : new Color32(51, 43, 43, 255);
        eImage.GetComponent<Image>().color = canE && enable ? new Color32(255, 255, 255, 255) : new Color32(51, 43, 43, 255);
        rImage.GetComponent<Image>().color = canR && enable ? new Color32(255, 255, 255, 255) : new Color32(51, 43, 43, 255);
    }
    
    //On click events for toggling action menu
    public void OpenMenu()
    {
        Debug.Log("Open");
       actionMenu.SetActive(true);
       actionButton.SetActive(false);
    }

    public void CloseMenu()
    {
        Debug.Log("Close");
        actionMenu.SetActive(false);
        actionButton.SetActive(true);
    }

    //Update action log text
    public void UpdateLog()
    {
        for(int x=0;x<current.GetStats().Length; x++)
        {
            statsText[x].text = current.GetStats()[x]+"";
        }
        string str = "Action Log: Recent moves will show up here.\n";
        foreach (object obj in movesList)
            str += (string)obj;
        actionText.text = str;
    }

    private void SetBars()
    {
        //Set player HP and secondary resource bars to correct values
        healthBar.maximumHealth = current.hp;
        healthBar.health = current.hp;
        manaBar.maximumHealth = current.GetResource()[1];
        manaBar.health = current.GetResource()[3];
        manaBar.regenerateHealth = true;
        manaBar.healthPerSecond = current.GetResource()[2];
        for(int x=0;x<livingEnemies.Length;x++) {
            if(foes[x]!=null && livingEnemies[x]){
                //Set enemy HP and secondary resource bars to correct values
                enemyResourceArr[x].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Healthbar>().maximumHealth = foes[x].hp;
                enemyResourceArr[x].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Healthbar>().health = foes[x].hp;
                enemyResourceArr[x].transform.GetChild(0).GetChild(2).gameObject.GetComponent<Healthbar>().maximumHealth = foes[x].GetResource()[1];
                enemyResourceArr[x].transform.GetChild(0).GetChild(2).gameObject.GetComponent<Healthbar>().health = foes[x].GetResource()[1];
                enemyResourceArr[x].transform.GetChild(0).GetChild(2).gameObject.GetComponent<Healthbar>().regenerateHealth = true;
                enemyResourceArr[x].transform.GetChild(0).GetChild(2).gameObject.GetComponent<Healthbar>().healthPerSecond = foes[x].GetResource()[2];
                //Custom function to refresh values of health bar
                enemyResourceArr[x].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Healthbar>().Refresh();
                enemyResourceArr[x].transform.GetChild(0).GetChild(2).gameObject.GetComponent<Healthbar>().Refresh();
                float enemyRegenRate = 1 / foes[x].speed;
                enemyResourceArr[x].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Healthbar>().healthPerSecond = 33 / enemyRegenRate;
            }
        }
        //Set regen rate of action bar based on speed
        healthBar.Refresh();
        manaBar.Refresh();
        float playerRegenRate = 1 / current.speed;
        champBar.healthPerSecond = 33 / playerRegenRate;
    }

    private void SetRegen()
    {
        manaBar.healthPerSecond = current.GetResource()[2];
        enemyResourceArr[enemyIndex].transform.GetChild(0).GetChild(2).gameObject.GetComponent<Healthbar>().healthPerSecond = foes[enemyIndex].GetResource()[2];
        healthBar.healthPerSecond = current.regen;
        enemyResourceArr[enemyIndex].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Healthbar>().healthPerSecond = foes[enemyIndex].regen;
        float playerRegenRate = 1 / current.speed;
        float enemyRegenRate = 1 / foes[enemyIndex].speed;
        champBar.healthPerSecond = 33 / playerRegenRate;
        enemyResourceArr[enemyIndex].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Healthbar>().healthPerSecond = 33 / enemyRegenRate;
        healthBar.Refresh();
        manaBar.Refresh();
        enemyResourceArr[enemyIndex].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Healthbar>().Refresh();
        enemyResourceArr[enemyIndex].transform.GetChild(0).GetChild(2).gameObject.GetComponent<Healthbar>().Refresh();
        champBar.Refresh();
        enemyResourceArr[enemyIndex].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Healthbar>().Refresh();
    }

    public void OnHover(int val)
    {
        //A value is passed in for which spell user is currently hovering over. 0 is Q, 1 is W, 2 is E, 3 is R.
        // 4: T, 5: Y, 6: Shop, 7: Farm, 8: Swap, 9: Auto
        switch (val)
        {
            case 0:
                qPanel.SetActive(true);
                qName.text = current.DescribeQ()[0];
                qDesc.text = "\n----\n" + current.DescribeQ()[1];
                break;
            case 1:
                wPanel.SetActive(true);
                wName.text = current.DescribeW()[0];
                wDesc.text = "\n----\n" + current.DescribeW()[1];
                break;
            case 2:
                ePanel.SetActive(true);
                eName.text = current.DescribeE()[0];
                eDesc.text = "\n----\n" + current.DescribeE()[1];
                break;
            case 3:
                rPanel.SetActive(true);
                rName.text = current.DescribeR()[0];
                rDesc.text = "\n----\n" + current.DescribeR()[1];
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                shopPanel.SetActive(true);
                break;
            case 7:
                farmPanel.SetActive(true);
                break;
            case 8:
                swapPanel.SetActive(true);
                break;
            case 9:
                aPanel.SetActive(true);
                aName.text = current.DescribeAuto()[0];
                aDesc.text = "\n----\n" + current.DescribeAuto()[1];
                break;
        }
    }

    public void HoverExit()
    {
        qPanel.SetActive(false);
        wPanel.SetActive(false);
        ePanel.SetActive(false);
        rPanel.SetActive(false);
        aPanel.SetActive(false);
        shopPanel.SetActive(false);
        swapPanel.SetActive(false);
        farmPanel.SetActive(false);
    }

    private IEnumerator GainGold()
    {
        while (true)
        {
            gold++;
            yield return new WaitForSeconds(1.0f);
        }
    }
    //Cooldown coroutines. Don't let player use spell until cooldown time has passed and display remaining CD time on screen.
    private IEnumerator QCooldown(float cdTime)
    {
        qCooldownText.gameObject.SetActive(true);
        Debug.Log("Before cooldown");
        float displayTime = cdTime;
        for (float x = 0; x < cdTime; x += 1.0f)
        {
            if (!current.qCooldown)
                break;
            qCooldownText.text = (int)displayTime + "";
            yield return new WaitForSeconds(1.0f);
            displayTime -= 1.0f;
        }
        current.qCooldown = false;
        qCooldownText.gameObject.SetActive(false);
        Debug.Log("After cooldown");
    }
    private IEnumerator WCooldown(float cdTime)
    {
        wCooldownText.gameObject.SetActive(true);
        Debug.Log("Before cooldown");
        float displayTime = cdTime;
        for (float x = 0; x < cdTime; x += 1.0f)
        {
            if (!current.wCooldown)
                break;
            wCooldownText.text = (int)displayTime + "";
            yield return new WaitForSeconds(1.0f);
            displayTime -= 1.0f;
        }
        current.wCooldown = false;
        wCooldownText.gameObject.SetActive(false);
        Debug.Log("After cooldown");
    }
    private IEnumerator ECooldown(float cdTime)
    {
        eCooldownText.gameObject.SetActive(true);
        Debug.Log("Before cooldown");
        float displayTime = cdTime;
        for(float x = 0; x < cdTime; x += 1.0f)
        {
            if (!current.eCooldown)
                break;
            eCooldownText.text = (int)displayTime + "";
            yield return new WaitForSeconds(1.0f);
            displayTime -= 1.0f;
        }
        current.eCooldown = false;
        eCooldownText.gameObject.SetActive(false);
        Debug.Log("After cooldown");
    }
    private IEnumerator RCooldown(float cdTime)
    {
        rCooldownText.gameObject.SetActive(true);
        float displayTime = cdTime;
        for(float x = 0; x < cdTime; x += 1.0f)
        {
            if (!current.rCooldown)
                break;
            rCooldownText.text = (int)displayTime + "";
            yield return new WaitForSeconds(1.0f);
            displayTime -= 1.0f;
        }
        current.rCooldown = false;
        rCooldownText.gameObject.SetActive(false);
        Debug.Log("After cooldown");
    }

    public void UpdateToggles(){
            wToggle.SetActive(current.ToggleW());
            rToggle.SetActive(current.ToggleR());
    }

    //Spell Images
    private Sprite GetSpellImage(string champ, string spell){
        return Resources.Load<Sprite>(""+champ+spell);
    }

    private void HideArrows(){
        arrowsArr[0].SetActive(false);
        arrowsArr[1].SetActive(false);
        arrowsArr[2].SetActive(false);
    }
}