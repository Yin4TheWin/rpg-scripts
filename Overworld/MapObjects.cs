 using UnityEngine;
 
public class MapObjects : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverSound, clickedSound;
    bool hover=false;

    public void OnMouseOver()
    {
        // do mouse hover stuff
        transform.gameObject.GetComponent<Animator>().Play("MapZoom");
        if(!hover){
            audioSource.PlayOneShot(hoverSound);
            hover=true;
        }
    }
 
   public void OnMouseExit()
   {
       // reset to normal
       transform.gameObject.GetComponent<Animator>().Play("MapOut");
       hover=false;
   }

   public void OnMouseDown(){
       audioSource.PlayOneShot(clickedSound);
   }
}