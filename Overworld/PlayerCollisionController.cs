using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisionController : MonoBehaviour
{
    public Animator canvasAnimator;
    private bool stopLoading=false;
    Dictionary<string, string[][]> encounters=new Dictionary<string, string[][]>();
    // Start is called before the first frame update
    void Start()
    {
        transform.position=new Vector3(0,0,0);
        Debug.Log(transform.position.x);
        // if(Storage.prevBattlePos.Length>0){
        //     //Load position for prevBattlePos
        //     Debug.Log("Position X "+Storage.prevBattlePos[0]);
        //     transform.position=new Vector3(Storage.prevBattlePos[0], Storage.prevBattlePos[1], Storage.prevBattlePos[2]);
        //     Debug.Log(transform.position.x);
        //     Storage.prevBattlePos=new float[0];
        // } else {
        //     //Load position from save file
        //     transform.position=new Vector3(Storage.savedPos[0], Storage.savedPos[1], Storage.savedPos[2]);
        // }
        encounters.Add("Slime", new string[][] {
            new string[]{ "Slime" }, new string[]{ "Slime" },
            new string[]{ "Slime", "Slime" },new string[]{ "Slime", "Slime" },
            new string[]{ "Slime", "Slime", "Slime" }, new string[]{ "Slime", "Slime", "Slime" }, 
            new string[] {"Shell Slime", "Slime"}, new string[] {"Shell Slime", "Slime", "Slime"},
            new string[] {"Shell Slime", "Slime", "Shell Slime"}
        });
        encounters.Add("Shell Slime", new string[][] {
            new string[]{ "Shell Slime" }, new string[]{ "Shell Slime" },
            new string[]{ "Shell Slime", "Shell Slime" },new string[]{ "Shell Slime", "Shell Slime" },
            new string[]{ "Shell Slime", "Shell Slime", "Shell Slime" }, new string[]{ "Shell Slime", "Shell Slime", "Shell Slime" }, 
            new string[] {"Slime", "Shell Slime"}, new string[] {"Slime", "Shell Slime", "Shell Slime"},
            new string[] {"Slime", "Shell Slime", "Slime"}
        });
        //DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col){
        if (!stopLoading&&col.gameObject.tag == "Foe"){
            Debug.Log("Slime hit");
            StartCoroutine(LoadBattle(col.gameObject.name));
        } if(col.gameObject.name=="Slime"&&stopLoading)
            Debug.Log("HELLO");
    }

    IEnumerator LoadBattle(string name)
    {
        Storage.prevBattlePos=new float[3]{transform.position.x, transform.position.y, transform.position.z};
        Debug.Log("POSITION X "+transform.position.x);
        stopLoading=true;
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        canvasAnimator.Play("FadeOut");
        Storage.battle=true;
        yield return new WaitForSeconds(1.5f);
        Storage.currentFoes=encounters[name][Random.Range(0, encounters[name].Length)];
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SampleScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
