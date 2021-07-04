using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip dancingStart, dancingLoop, fightStart, fightLoop;
    bool flagged=false;
    IEnumerator bat, win;
    void Start()
    {
        Debug.Log("VOL "+audioSource.volume);
        bat=BattleMusic();
        win=FadeOut(audioSource, 2.0f, 0.0f);
        audioSource.clip=fightLoop;
        DontDestroyOnLoad(transform.gameObject);
    }

    void Update(){
        if(Storage.battle&&!flagged&&Storage.prevBattlePos.Length>0){
            Debug.Log("VOL "+audioSource.volume);
            StartCoroutine(bat);
            flagged=true;
        }
        if(!Storage.battle&&flagged){
            Debug.Log("END");
            StartCoroutine(win);
        }
        if(!Storage.battle&&!flagged&&SceneManager.GetActiveScene().name=="Map0"){
            audioSource.Stop();
            StopCoroutine(win);
            win=FadeOut(audioSource, 2.0f, 0.0f);
            Storage.dance=false;
        }
    }
    IEnumerator BattleMusic(){
        audioSource.volume=1.0f;
        audioSource.clip=fightStart;
        audioSource.Play();
        Debug.Log("Length"+fightStart.length);
        yield return new WaitForSeconds(fightStart.length-0.2f);
        audioSource.Stop();
        audioSource.clip=fightLoop;
        audioSource.loop=true;
        audioSource.Play();
    }
    public IEnumerator FadeOut(AudioSource audioSource, float duration, float targetVolume)
    {
        flagged=false;
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(1.0f, targetVolume, currentTime / duration);
            yield return null;
        }
        audioSource.Stop();
        StopCoroutine(bat);
        bat=BattleMusic();
        audioSource.volume=0.65f;
        audioSource.loop=false;
        audioSource.clip=dancingStart;
        Storage.dance=true;
        audioSource.Play();
        yield return new WaitForSeconds(dancingStart.length);
        audioSource.loop=true;
        audioSource.clip=dancingLoop;
        audioSource.Play();
    }
}