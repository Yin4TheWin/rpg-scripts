using UnityEngine;
using System.Collections;
 
public class CameraMove : MonoBehaviour
{
    public float Boundary = 50.0f;
    public float speed = 15.0f;
   
    private float ScreenWidth;
    private float ScreenHeight;
   
    void Start()
    {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
    }
   
    void Update()
    {
        if(Input.mousePosition.x > ScreenWidth - Boundary)
        {
            Vector3 myPos = transform.position;
            myPos.x += speed * Time.deltaTime;
            transform.position = myPos;
        }
       
        if(Input.mousePosition.x < 0 + Boundary)
        {
            Vector3 myPos = transform.position;
            myPos.x -= speed * Time.deltaTime;
            transform.position = myPos;
        }
       
        if(Input.mousePosition.y > ScreenHeight - Boundary)
        {
            Vector3 myPos = transform.position;
            myPos.z += speed * Time.deltaTime;
            transform.position = myPos;
        }
       
        if(Input.mousePosition.y < 0 + Boundary)
        {
            Vector3 myPos = transform.position;
            myPos.z -= speed * Time.deltaTime;
            transform.position = myPos;
        }
    }
}
 